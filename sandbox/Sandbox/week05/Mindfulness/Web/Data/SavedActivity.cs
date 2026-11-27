namespace Mindfulness.Web.Data;

public sealed class SavedActivity
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public Guid RequestId { get; set; }
    public required string ActivityName { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public int RequestedSeconds { get; set; }
    public double ElapsedSeconds { get; set; }
    public required string ResponsesJson { get; set; }
}
