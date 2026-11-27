using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mindfulness.Web.Data;

namespace Mindfulness.Web.Pages;

[Authorize]
public sealed class HistoryModel(ApplicationDbContext database) : PageModel
{
    public List<HistoryEntry> Entries { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("The signed-in account has no user ID.");
        List<SavedActivity> activities = await database.Activities.AsNoTracking()
            .Where(activity => activity.UserId == userId)
            .OrderByDescending(activity => activity.Id)
            .ToListAsync(cancellationToken);
        Entries = activities.Select(activity => new HistoryEntry(
            activity.Id, activity.ActivityName, activity.StartedAt, activity.RequestedSeconds,
            activity.ElapsedSeconds, JsonSerializer.Deserialize<List<SavedResponse>>(activity.ResponsesJson)
                ?? throw new InvalidDataException("A saved activity has invalid responses."))).ToList();
    }
}

public sealed record HistoryEntry(int Id, string ActivityName, DateTimeOffset StartedAt,
    int RequestedSeconds, double ElapsedSeconds, List<SavedResponse> Responses);
