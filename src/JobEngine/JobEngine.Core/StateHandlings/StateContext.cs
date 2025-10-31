using JobEngine.Core.Aggregates;
using JobEngine.Core.StateHandlings.Contracts;
using JobEngine.SqlServer.Services;

namespace JobEngine.Core.StateHandlings;

public class StateContext
{
    public long JobId { get; set; }
    public State FromState { get; set; } = default!;
    public State ToState { get; set; } = default!;
    public IWriteOnlyTransaction WriteOnlyTransaction { get; set; } = default!;
}


public class StateMachine : IStateMachine
{
    private SortedDictionary<JobState, List<IStateHandler>> _stateHandlers = [];


    public StateMachine()
    {
        _stateHandlers.Add(JobState.Scheduled, []);
    }


    public void ChangeState(StateContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(context.ToState);

        if(context.FromState is not null)
        {
            var fromHandlers = _stateHandlers.GetValueOrDefault(context.FromState.JobState) ?? [];

            foreach(var fromHandler in fromHandlers)
            {
                fromHandler.UnApply(context);
            }
        }

        var toHandlers = _stateHandlers.GetValueOrDefault(context.ToState.JobState);

        if(toHandlers is null)
        {
            throw new Exception($"There is no StateHandler registered for going to `{context.ToState.JobState}`");
        }

        context.WriteOnlyTransaction.SetJobState(context.JobId, context.ToState);

        foreach(var toHandler in toHandlers)
        {
            toHandler.Apply(context);
        }
    }
}