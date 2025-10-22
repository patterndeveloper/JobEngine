using JobEngine.Core.Aggregates;

namespace JobEngine.Core.Services.Contracts;

public interface IRepository
{
    long CreateJobWithExpiration(Job job);
}
