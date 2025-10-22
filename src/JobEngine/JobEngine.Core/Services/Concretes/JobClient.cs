using JobEngine.Core.Aggregates;
using JobEngine.Core.Services.Contracts;
using System.Linq.Expressions;
using System.Text.Json;

namespace JobEngine.Core.Services.Concretes;

public class JobClient : IJobClient
{
    private readonly IRepository _repository;

    public JobClient(IRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);

        _repository = repository;
    }


    public void Schedule(Expression<Action> action, TimeSpan delay)
    {
        ArgumentNullException.ThrowIfNull(action);

        var methodCallExpression = action.Body as MethodCallExpression;

        if (methodCallExpression is null)
        {
            throw new ArgumentException("Expression body should be of type `MethodCallExpression`", nameof(action));
        }

        var methodCallInfo = MethodCallInfo.FromExpression(methodCallExpression);

        var invocationData = new InvocationData(methodCallInfo.MethodName,
                                                methodCallInfo.DeclaringType,
                                                methodCallInfo.ParameterTypes);

        var invocationDataAsJson = JsonSerializer.Serialize(invocationData);
        var argumentsAsJson = JsonSerializer.Serialize(methodCallInfo.Arguments);

        var job = new Job(invocationDataAsJson,
                          argumentsAsJson,
                          DateTime.UtcNow,
                          DateTime.UtcNow.AddDays(1));

        var jobId = _repository.CreateJobWithExpiration(job);
    }
}