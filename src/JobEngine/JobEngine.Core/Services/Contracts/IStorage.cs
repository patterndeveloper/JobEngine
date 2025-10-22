using System.Data;

namespace JobEngine.Core.Services.Contracts;

public interface IStorage
{
    TResult UseConnection<TContext, TResult>(Func<IDbConnection, TContext, TResult> func, TContext context);
}