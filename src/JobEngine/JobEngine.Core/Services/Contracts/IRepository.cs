using JobEngine.Core.Aggregates;
using JobEngine.SqlServer.Services;

namespace JobEngine.Core.Services.Contracts;

public interface IRepository
{
    long CreateJobWithExpiration(Job job);
    IWriteOnlyTransaction CreateWriteOnlyTransaction();
}
