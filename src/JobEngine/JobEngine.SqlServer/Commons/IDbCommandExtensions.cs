using System.Data;

namespace JobEngine.SqlServer.Commons;

public static class IDbCommandExtensions
{
    public static IDbCommand AddParameter(this IDbCommand dbCommand,
                                          string parameterName,
                                          object? value,
                                          DbType dbType)
    {
        ArgumentNullException.ThrowIfNull(dbCommand);
        ArgumentNullException.ThrowIfNull(parameterName);

        var parameter = dbCommand.CreateParameter();

        parameter.ParameterName = parameterName;
        parameter.Value = value;
        parameter.DbType = dbType;

        dbCommand.Parameters.Add(parameter);

        return dbCommand;
    }
}