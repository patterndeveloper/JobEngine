using JobEngine.Core.Services.Concretes;
using JobEngine.Core.Services.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace JobEngine.Core.Commons;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddJobEngine(this IServiceCollection services, Action<JobEngineOption> config)
    {
        services.AddSingleton<IJobClient, JobClient>();

        var jobEngineOption = new JobEngineOption();
        config(jobEngineOption);

        jobEngineOption.ConfigureStore(services);

        return services;
    }
}


public class JobEngineOption
{
    public Action<IServiceCollection> ConfigureStore { get; set; } = default!;
}