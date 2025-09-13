namespace JobEngine.Core.Aggregates;

public class State
{
    public long JobId { get; set; }
    public long Id { get; set; }
    public JobState JobState { get; set; }
    public string? Reason { get; set; }
    public string? Data { get; set; }
    public DateTime CreatedAt { get; set; }
}