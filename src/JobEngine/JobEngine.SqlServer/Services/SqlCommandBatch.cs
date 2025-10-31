using System.Data;

namespace JobEngine.SqlServer.Services;

public class SqlCommandBatch : IDisposable
{
    private readonly List<IDbCommand> _dbCommands = [];
    private readonly IDbTransaction _dbTransaction;
    private readonly IDbConnection _dbConnection;


    public SqlCommandBatch(IDbTransaction dbTransaction, IDbConnection dbConnection)
    {
        ArgumentNullException.ThrowIfNull(dbTransaction);
        ArgumentNullException.ThrowIfNull(dbConnection);

        _dbTransaction = dbTransaction;
        _dbConnection = dbConnection;
    }


    public void AddDbCommand(IDbCommand dbCommand)
    {
        ArgumentNullException.ThrowIfNull(dbCommand);

        _dbCommands.Add(dbCommand);
    }


    public void ExecuteNonQuery()
    {
        foreach (var dbCommand in _dbCommands)
        {
            dbCommand.Connection = _dbConnection;
            dbCommand.Transaction = _dbTransaction;

            dbCommand.ExecuteNonQuery();
        }
    }


    public void Dispose()
    {
        foreach(var  dbCommand in _dbCommands)
        {
            dbCommand.Dispose();
        }
    }
}