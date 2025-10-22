using JobEngine.Core.Commons;
using JobEngine.Core.Services.Contracts;
using JobEngine.SqlServer.Services;
using Microsoft.Extensions.DependencyInjection;

namespace JobEngine.SqlServer.Commons;

public static class JobEngineOptionExtensions
{
    public static void UseSqlServerStorage(this JobEngineOption option, string connectionString)
    {
        option.ConfigureStore = services =>
        {
            var sqlServerOption = new SqlServerOption(connectionString);

            services.AddSingleton<IStorage>(serviceProvider =>
            {
                var sqlStorage = new SqlStorage(sqlServerOption);
                return sqlStorage;
            });


            services.AddSingleton<IRepository>(serviceProvider =>
            {
                var storage = serviceProvider.GetRequiredService<IStorage>();
                var jobStorage = new SqlRepository(storage);
                return jobStorage;
            });
        };
    }
}