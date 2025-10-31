using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Commons;
using Microsoft.Data.SqlClient;
using System.Data;

namespace JobEngine.SqlServer.Services;

public class SqlStorage : IStorage
{
    private readonly SqlServerOption _sqlServerOption;

    public SqlStorage(SqlServerOption sqlServerOption)
    {
        _sqlServerOption = sqlServerOption ?? throw new ArgumentNullException(nameof(sqlServerOption));
    }


    public TResult UseConnection<TContext, TResult>(Func<IDbConnection, TContext, TResult> func, TContext context)
    {
        var sqlConnection = CreateAndOpen();

        try
        {
            return func(sqlConnection, context);
        }
        finally
        {
            sqlConnection?.Dispose();
        }
    }


    public TResult UseTransaction<TContext, TResult>(Func<IDbTransaction, IDbConnection, TContext, TResult> func, TContext context)
    {
        return UseConnection((dbConnection, context) =>
        {
            TResult result = default!;
            using var dbTransaction = dbConnection.BeginTransaction();

            try
            {
                result = func(dbTransaction, dbConnection, context);
                dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                if(dbTransaction.Connection is not null)
                {
                    dbTransaction.Rollback();
                }
                throw;
            }

            return result;
        }, context);
    }


    public void UseTransaction<TContext> (Action<IDbTransaction, IDbConnection, TContext> func, TContext context)
    {
        UseTransaction((dbTransaction, dbConnection, context) =>
        {
            func(dbTransaction, dbConnection, context);
            return true;
        }, context);
    }



    private IDbConnection CreateAndOpen()
    {
        IDbConnection sqlConnection = default!;

        try
        {
            sqlConnection = new SqlConnection(_sqlServerOption.ConnectionString);

            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            return sqlConnection;
        }
        catch (Exception ex)
        {
            sqlConnection?.Dispose();
            throw;
        }
    }
}
