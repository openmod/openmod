using System.Threading.Tasks;

namespace OpenMod.API.Jobs
{
    public interface IJobExecutor
    {
        bool SupportsType(string jobType);
        Task ExecuteAsync(ScheduledJob job);
    }
}