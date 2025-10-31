using System.Data;

namespace JobEngine.Core.Services.Contracts;

public interface IStorage
{
    TResult UseConnection<TContext, TResult>(Func<IDbConnection, TContext, TResult> func, TContext context);
    TResult UseTransaction<TContext, TResult>(Func<IDbTransaction, IDbConnection, TContext, TResult> func, TContext context);
    void UseTransaction<TContext>(Action<IDbTransaction, IDbConnection, TContext> func, TContext context);
}