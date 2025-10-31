namespace JobEngine.Core.StateHandlings.Contracts;

public interface IStateMachine
{
    void ChangeState(StateContext context);
}
