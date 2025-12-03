using JobEngine.Core.Aggregates;

namespace JobEngine.SqlServer.Services
{
    public interface IWriteOnlyTransaction : IDisposable
    {
        void SetJobState(long jobId, State state);

        void AddToSet(string key, string value, int score);
        void RemoveFromSet(string key, string value);

        void Commit();
    }
}