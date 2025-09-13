namespace JobEngine.Core.Aggregates;

public enum JobState
{
    Scheduled = 1,
    Enqueued = 2,
    Processing = 3,
    Failed = 4,
    Success = 5
}