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


    public TResult UseTransaction<TContext, TResult>(Func<IDbConnection, IDbTransaction, TContext, TResult> func, TContext context)
    {
        var sqlConnection = CreateAndOpen();
        using var sqlTransaction = sqlConnection.BeginTransaction();

        try
        {
            return func(sqlConnection, sqlTransaction, context);
        }
        finally
        {
            sqlConnection?.Close();
        }
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
