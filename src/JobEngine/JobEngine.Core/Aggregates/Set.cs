namespace JobEngine.Core.Aggregates;

public class Set
{
    public string Key { get; set; } = default!;
    public string Value { get; set; } = default!;
    public int Score { get; set; }
    public DateTime ExpireAt { get; set; }
}