using JobEngine.Core.Aggregates;
using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Commons;
using System.Data;

namespace JobEngine.SqlServer.Services;

public class WriteOnlyTransaction
{
    private readonly SortedDictionary<long, List<Action<IDbCommand>>> _jobCommands = [];
    private readonly IStorage _storage;

    public WriteOnlyTransaction(IStorage storage)
    {
        ArgumentNullException.ThrowIfNull(storage);
        
        _storage = storage;
    }


    public void SetJobState(long jobId, State state)
    {
        Action<IDbCommand> insertState = dbCommand => dbCommand
                                                    .SetCommandText(SqlHelper.InsertState)
                                                    .AddParameter("@JobId", state.JobId, DbType.Int64)
                                                    .AddParameter("@Name", state.JobState.ToString(), DbType.String)
                                                    .AddParameter("@Reason", state.Reason, DbType.String)
                                                    .AddParameter("@Data", state.Data, DbType.String)
                                                    .AddParameter("@CreatedAt", state.CreatedAt, DbType.DateTime);

        AddCommand(_jobCommands, jobId,  insertState);
    }

    private void AddCommand<TKey>(SortedDictionary<TKey, List<Action<IDbCommand>>> sortedCommandsDic,
                                  TKey id,
                                  Action<IDbCommand> action) where TKey : notnull
    {
        ArgumentNullException.ThrowIfNull(sortedCommandsDic);
        ArgumentNullException.ThrowIfNull(action);

        var hasBucket = sortedCommandsDic.TryGetValue(id, out var bucket);

        if(!hasBucket || bucket is null)
        {
            bucket = [];
            sortedCommandsDic.Add(id, bucket);
        }

        bucket.Add(action);
    }
}