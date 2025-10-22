namespace JobEngine.SqlServer.Commons;

public class SqlServerOption
{
    public string ConnectionString { get; private set; } = default!;

    public SqlServerOption()
    {
    }

    public SqlServerOption(string connectionString)
    {
        ConnectionString = connectionString;
    }
}