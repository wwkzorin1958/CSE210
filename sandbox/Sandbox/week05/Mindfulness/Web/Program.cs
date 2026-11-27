using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Mindfulness.Web.Data;

var builder = WebApplication.CreateBuilder(args);

string certificatePath = Environment.GetEnvironmentVariable("MINDFULNESS_CERT_PATH")
    ?? throw new InvalidOperationException("Set MINDFULNESS_CERT_PATH to your HTTPS PFX certificate.");
string certificatePassword = Environment.GetEnvironmentVariable("MINDFULNESS_CERT_PASSWORD")
    ?? throw new InvalidOperationException("Set MINDFULNESS_CERT_PASSWORD to the certificate password.");
string bindAddress = Environment.GetEnvironmentVariable("MINDFULNESS_BIND_ADDRESS") ?? "0.0.0.0";
string portText = Environment.GetEnvironmentVariable("MINDFULNESS_HTTPS_PORT") ?? "5443";
if (!IPAddress.TryParse(bindAddress, out IPAddress? address) ||
    !int.TryParse(portText, out int port) || port < 1 || port > 65535)
{
    throw new InvalidOperationException("MINDFULNESS_BIND_ADDRESS or MINDFULNESS_HTTPS_PORT is invalid.");
}

X509Certificate2 certificate = X509CertificateLoader.LoadPkcs12FromFile(
    certificatePath, certificatePassword);
if (!certificate.HasPrivateKey || DateTime.UtcNow < certificate.NotBefore.ToUniversalTime() ||
    DateTime.UtcNow > certificate.NotAfter.ToUniversalTime())
{
    throw new InvalidOperationException("The HTTPS certificate must have a private key and be currently valid.");
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 4 * 1024 * 1024;
    options.Listen(address, port, endpoint => endpoint.UseHttps(certificate));
});

string dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
Directory.CreateDirectory(dataDirectory);
var connection = new SqliteConnectionStringBuilder
{
    DataSource = Path.Combine(dataDirectory, "mindfulness.db"),
    ForeignKeys = true
};
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection.ToString()));
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
    options.Password.RequiredLength = 12;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Login";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");
builder.Services.AddRazorPages();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        string client = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        if (HttpMethods.IsPost(context.Request.Method) &&
            (context.Request.Path.Equals("/Login", StringComparison.OrdinalIgnoreCase) ||
             context.Request.Path.Equals("/Register", StringComparison.OrdinalIgnoreCase)))
        {
            return RateLimitPartition.GetFixedWindowLimiter(client, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
        }

        return RateLimitPartition.GetNoLimiter(client);
    });
});

var app = builder.Build();
using (IServiceScope scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreatedAsync();
}

app.UseExceptionHandler("/Error");
app.UseHsts();
app.Use(async (context, next) =>
{
    context.Response.Headers.CacheControl = "no-store";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    await next();
});
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(AppContext.BaseDirectory, "Sounds")),
    RequestPath = "/sounds"
});
app.UseRouting();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
