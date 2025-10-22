using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobEngine.SqlServer.Commons;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJobEngineFromDb(this IServiceCollection services, Action<SqlServerOption> configure)
    {
        services.AddSingleton<SqlServerOption>(provider =>
        {
            var sqlServerOption = new SqlServerOption();
            configure(sqlServerOption);
            return sqlServerOption;
        });

        services.AddSingleton<IStorage, SqlStorage>();

        return services;
    }
}