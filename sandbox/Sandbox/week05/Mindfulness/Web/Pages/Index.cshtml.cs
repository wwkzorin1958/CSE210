using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mindfulness.Web.Data;

namespace Mindfulness.Web.Pages;

[Authorize]
public sealed class IndexModel(ApplicationDbContext database) : PageModel
{
    public List<string> ListingPrompts { get; private set; } = new();
    public List<string> ReflectionPrompts { get; private set; } = new();
    public List<string> ReflectionQuestions { get; private set; } = new();

    public void OnGet()
    {
        ListingActivity listing = new("Listing", "", 1, 0);
        ReflectingActivity reflecting = new("Reflecting", "", 1);
        ListingPrompts = listing.GetPrompt();
        ReflectionPrompts = reflecting.GetPrompt();
        ReflectionQuestions = reflecting.GetQuestions();
    }

    public async Task<IActionResult> OnPostSaveAsync([FromBody] SaveActivityRequest? request, CancellationToken cancellationToken)
    {
        if (request == null || request.RequestId == Guid.Empty ||
            request.ActivityName is not ("Breathing" or "Reflecting" or "Listing") ||
            request.RequestedSeconds < 1 || request.RequestedSeconds > 86400 ||
            !double.IsFinite(request.ElapsedSeconds) || request.ElapsedSeconds < 0 ||
            request.StartedAt == default || request.Responses == null || request.Responses.Count > 500 ||
            request.Responses.Any(response => response == null || string.IsNullOrWhiteSpace(response.Prompt) ||
                response.Prompt.Length > 1000 || response.Answers == null ||
                response.Answers.Any(answer => answer == null || answer.Length > 2000)) ||
            request.Responses.Sum(response => response.Answers.Count) > 500)
        {
            return BadRequest(new { error = "Activity data is invalid or exceeds the supported limits." });
        }

        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The signed-in account has no user ID.");
        SavedActivity? existing = await database.Activities.AsNoTracking()
            .SingleOrDefaultAsync(activity => activity.UserId == userId && activity.RequestId == request.RequestId, cancellationToken);
        if (existing != null)
        {
            return new JsonResult(new { id = existing.Id });
        }

        SavedActivity activity = new()
        {
            UserId = userId,
            RequestId = request.RequestId,
            ActivityName = request.ActivityName,
            StartedAt = request.StartedAt,
            RequestedSeconds = request.RequestedSeconds,
            ElapsedSeconds = request.ElapsedSeconds,
            ResponsesJson = JsonSerializer.Serialize(request.Responses)
        };
        database.Activities.Add(activity);
        try
        {
            await database.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqliteException { SqliteErrorCode: 19 })
        {
            existing = await database.Activities.AsNoTracking()
                .SingleOrDefaultAsync(entry => entry.UserId == userId && entry.RequestId == request.RequestId, cancellationToken);
            if (existing == null) throw;
            return new JsonResult(new { id = existing.Id });
        }
        Response.Headers.Location = Url.Page("/History") + $"#activity-{activity.Id}";
        return new JsonResult(new { id = activity.Id }) { StatusCode = StatusCodes.Status201Created };
    }
}

public sealed record SaveActivityRequest(
    Guid RequestId,
    string ActivityName,
    DateTimeOffset StartedAt,
    int RequestedSeconds,
    double ElapsedSeconds,
    List<SavedResponse> Responses);

public sealed record SavedResponse(string Prompt, List<string> Answers);
