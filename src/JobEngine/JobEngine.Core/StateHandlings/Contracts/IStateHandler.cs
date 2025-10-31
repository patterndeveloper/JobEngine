namespace JobEngine.Core.StateHandlings.Contracts;

public interface IStateHandler
{
    void Apply(StateContext context);
    void UnApply(StateContext context);
}
