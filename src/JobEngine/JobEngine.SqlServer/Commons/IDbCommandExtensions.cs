using System.Data;

namespace JobEngine.SqlServer.Commons;

public static class IDbCommandExtensions
{
    public static IDbCommand SetCommandText(this IDbCommand dbCommand, string commandText)
    {
        ArgumentNullException.ThrowIfNull(dbCommand);
        ArgumentNullException.ThrowIfNull(commandText);

        dbCommand.CommandText = commandText;

        return dbCommand;
    }

    public static IDbCommand AddParameter(this IDbCommand dbCommand,
                                          string parameterName,
                                          object? value,
                                          DbType dbType)
    {
        ArgumentNullException.ThrowIfNull(dbCommand);
        ArgumentNullException.ThrowIfNull(parameterName);

        var parameter = dbCommand.CreateParameter();

        parameter.ParameterName = parameterName;
        parameter.Value = value ?? DBNull.Value;
        parameter.DbType = dbType;

        dbCommand.Parameters.Add(parameter);

        return dbCommand;
    }
}