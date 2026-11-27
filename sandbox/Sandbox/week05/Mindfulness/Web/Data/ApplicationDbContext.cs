using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mindfulness.Web.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<SavedActivity> Activities => Set<SavedActivity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<SavedActivity>()
            .HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(activity => activity.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<SavedActivity>().HasIndex(activity => new { activity.UserId, activity.RequestId }).IsUnique();
    }
}
