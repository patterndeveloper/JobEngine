using System.Linq.Expressions;

namespace JobEngine.Core.Services.Contracts;

public interface IJobClient
{
    void Schedule(Expression<Action> action, TimeSpan delay);
}