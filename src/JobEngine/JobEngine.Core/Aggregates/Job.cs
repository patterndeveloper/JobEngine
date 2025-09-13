using System.Linq.Expressions;

namespace JobEngine.Core.Aggregates;

public class Job
{
    public long Id { get; set; }
    public string InvocationData { get; set; } = default!;
    public string? Arguments { get; set; }
    public int? CurrentStateId { get; set; }
    public string? CurrentStateName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpireAt { get; set; }


    public Job(string invocationData,
               string? arguments,
               DateTime createdAt,
               DateTime expireAt)
    {
        InvocationData = invocationData;
        Arguments = arguments;
        CreatedAt = createdAt;
        ExpireAt = expireAt;
    }
}


public class InvocationData
{
    public string MethodName { get; set; } = default!;
    public string DeclaringType { get; set; } = default!;
    public List<string> ParameterTypes { get; set; } = [];


    public InvocationData()
    {
    }


    public InvocationData(string methodName, string declaringType, List<string> parameterTypes)
    {
        MethodName = methodName;
        DeclaringType = declaringType;
        ParameterTypes = parameterTypes;
    }
}


public class MethodCallInfo
{
    public string MethodName { get; set; } = default!;
    public string DeclaringType { get; set; } = default!;
    public List<string> ParameterTypes { get; set; } = [];
    public List<string> Arguments { get; set; } = [];


    public static MethodCallInfo FromExpression(MethodCallExpression methodCallExpression)
    {
        var methodCallInfo = new MethodCallInfo();

        if (methodCallExpression is null)
        {
            throw new ArgumentNullException(nameof(methodCallExpression));
        }

        methodCallInfo.MethodName = methodCallExpression.Method.Name;

        methodCallInfo.DeclaringType = methodCallExpression.Method.DeclaringType!.AssemblyQualifiedName!;

        methodCallInfo.ParameterTypes = methodCallExpression.Method.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName).ToList()!;

        methodCallInfo.Arguments = methodCallExpression.Arguments.Select(arg => Expression.Lambda(arg).Compile().DynamicInvoke()!.ToString()).ToList()!;

        return methodCallInfo;
    }
}