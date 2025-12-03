using JobEngine.Core.Aggregates;
using JobEngine.Core.Exceptions;
using JobEngine.Core.Services.Contracts;
using JobEngine.Core.StateHandlings;
using JobEngine.Core.StateHandlings.Contracts;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using System.Text.Json;

namespace JobEngine.Core.Services.Concretes;

public class JobClient : IJobClient
{
    private readonly IRepository _repository;
    private readonly IStateMachine _stateMachine;
    private const int _failLimit = 5;

    public JobClient(IRepository repository, IStateMachine stateMachine)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(stateMachine);

        _repository = repository;
        _stateMachine = stateMachine;
    }


    public void Schedule(Expression<Action> action, TimeSpan delay)
    {
        try
        {
            ScheduleCore(action, delay);
        }
        catch (Exception ex)
        {
            throw new JobClientException("An error occured in creating background job", ex);
        }
    }


    private void ScheduleCore(Expression<Action> action, TimeSpan delay)
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

        var failCount = 0;

        var jobId = RetryOnException(_repository.CreateJobWithExpiration, job, ref failCount);



        var toState = new State(jobId,
                                JobState.Scheduled,
                                "Schedule new job from user api call",
                                null,
                                DateTime.UtcNow);

        using var writeOnlyTransaction = _repository.CreateWriteOnlyTransaction();

        var stateContext = new StateContext(jobId,
                                            null,
                                            toState,
                                            writeOnlyTransaction);

        failCount = 0;

        RetryOnException(ctx =>
        {
            _stateMachine.ChangeState(ctx);
            writeOnlyTransaction.Commit();
        }, stateContext, ref failCount);
    }


    private TResult RetryOnException<TResult>(Func<TResult> func, ref int failCount)
    {
        List<Exception> exceptions = default!;

        while (true)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                //TODO: Log ex

                exceptions ??= [];
                exceptions.Add(ex);

                failCount++;

                if (failCount > _failLimit)
                {
                    break;
                }
            }
        }

        if (exceptions.Count == 1)
        {
            var edi = ExceptionDispatchInfo.Capture(exceptions[0]);
            edi.Throw();
        }

        var aggregateException = new AggregateException(exceptions);
        throw aggregateException;
    }


    private TResult RetryOnException<TContext, TResult>(Func<TContext, TResult> func, TContext context, ref int failCount)
    {
        List<Exception> exceptions = default!;

        while (true)
        {
            try
            {
                return func(context);
            }
            catch (Exception ex)
            {
                //TODO: Log ex

                exceptions ??= [];
                exceptions.Add(ex);

                failCount++;

                if (failCount > _failLimit)
                    break;

            }
        }

        if (exceptions.Count == 1)
        {
            var edi = ExceptionDispatchInfo.Capture(exceptions[0]);
            edi.Throw();
        }

        var aggEx = new AggregateException(exceptions);
        throw aggEx;
    }


    private bool RetryOnException<TContext>(Action<TContext> action, TContext context, ref int failCount)
    {
        Func<TContext, bool> func = (ctx) =>
        {
            action(ctx);
            return true;
        };

        return RetryOnException(func, context, ref failCount);
    }
}