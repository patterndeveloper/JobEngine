using JobEngine.Core.Aggregates;
using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Commons;
using System.Data;

namespace JobEngine.SqlServer.Services;

public class WriteOnlyTransaction : IWriteOnlyTransaction
{
    private readonly SortedDictionary<long, List<Action<IDbCommand>>> _jobCommands = [];
    private readonly SortedDictionary<string, List<Action<IDbCommand>>> _setCommands = [];
    private readonly IStorage _storage;

    public WriteOnlyTransaction(IStorage storage)
    {
        ArgumentNullException.ThrowIfNull(storage);

        _storage = storage;
    }


    public void SetJobState(long jobId, State state)
    {
        Action<IDbCommand> commandAction = dbCommand => dbCommand.SetCommandText(SqlHelper.InsertState)
                                                               .AddParameter("@JobId", state.JobId, DbType.Int64)
                                                               .AddParameter("@Name", state.JobState.ToString(), DbType.String)
                                                               .AddParameter("@Reason", state.Reason, DbType.String)
                                                               .AddParameter("@Data", state.Data, DbType.String)
                                                               .AddParameter("@CreatedAt", state.CreatedAt, DbType.DateTime);

        AddCommand(_jobCommands, jobId, commandAction);
    }


    public void AddToSet(string key, string value, int score)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        Action<IDbCommand> commandAction = dbCommand => dbCommand.SetCommandText(SqlHelper.UpsertSet)
                                                                 .AddParameter("@key", key, DbType.String)
                                                                 .AddParameter("@value", value, DbType.String)
                                                                 .AddParameter("@score", score, DbType.Int32);

        AddCommand(_setCommands, key, commandAction);
    }


    public void RemoveFromSet(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        Action<IDbCommand> commandAction = dbCommand => dbCommand.SetCommandText(SqlHelper.RemoveFromSet)
                                                                 .AddParameter("@key", key, DbType.String)
                                                                 .AddParameter("@value", value, DbType.String);

        AddCommand(_setCommands, key, commandAction);
    }


    private void AddCommand<TKey>(SortedDictionary<TKey, List<Action<IDbCommand>>> sortedCommandsDic,
                                  TKey id,
                                  Action<IDbCommand> action) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(sortedCommandsDic);
        ArgumentNullException.ThrowIfNull(action);

        var hasBucket = sortedCommandsDic.TryGetValue(id, out var bucket);

        if (!hasBucket || bucket is null)
        {
            bucket = [];
            sortedCommandsDic.Add(id, bucket);
        }

        bucket.Add(action);
    }


    public void Commit()
    {
        _storage.UseTransaction(static (dbTransaction, dbConnection, context) =>
        {
            using var sqlCommandBatch = new SqlCommandBatch(dbTransaction, dbConnection);

            foreach (var jobCommands in context._jobCommands.Values)
            {
                foreach (var jobCommandAction in jobCommands)
                {
                    using var command = dbConnection.CreateCommand();
                    jobCommandAction(command);

                    sqlCommandBatch.AddDbCommand(command);
                }
            }

            sqlCommandBatch.ExecuteNonQuery();
        }, this);
    }
}