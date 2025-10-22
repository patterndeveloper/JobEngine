using JobEngine.Core.Aggregates;
using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Commons;
using System.Data;

namespace JobEngine.SqlServer.Services;

public class SqlRepository : IRepository
{
    private readonly IStorage _sqlStorage;


    public SqlRepository(IStorage sqlStorage)
    {
        ArgumentNullException.ThrowIfNull(sqlStorage);

        _sqlStorage = sqlStorage;
    }


    public long CreateJobWithExpiration(Job job)
    {
        return _sqlStorage.UseConnection<Job, long>((dbConnection, context) =>
        {
            using var dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = SqlHelper.InsertJob;

            dbCommand.AddParameter("@InvocationData", context.InvocationData, DbType.String);
            dbCommand.AddParameter("@Arguments", context.Arguments, DbType.String);
            dbCommand.AddParameter("@CreatedAt", context.CreatedAt, DbType.DateTime);
            dbCommand.AddParameter("@ExpireAt", context.ExpireAt, DbType.DateTime);

            var result = (long)dbCommand.ExecuteScalar()!;

            return result;
        }, job);
    }
}