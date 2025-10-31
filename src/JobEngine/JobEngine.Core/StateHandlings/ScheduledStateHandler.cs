using JobEngine.Core.Aggregates;
using JobEngine.Core.StateHandlings.Contracts;

namespace JobEngine.Core.StateHandlings;

public class ScheduledStateHandler : IStateHandler
{
    public void Apply(StateContext context)
    {
        var score = (DateTime.UtcNow - DateTime.UnixEpoch).Seconds;
        context.WriteOnlyTransaction.AddToSet(SetKeys.Schedule, Convert.ToString(context.JobId), score);
    }

    public void UnApply(StateContext context)
    {
        context.WriteOnlyTransaction.RemoveFromSet(SetKeys.Schedule, Convert.ToString(context.JobId));
    }
}