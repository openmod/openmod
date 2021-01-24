using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Runs jobs from autoexec.yaml.
    /// </summary>
    [Service]
    public interface IJobScheduler
    {
        Task StartAsync();

        Task AddJobAsync(ScheduledJob job);

        Task RemoveJobAsync(ScheduledJob job);

        Task<ICollection<ScheduledJob>> GetScheduledJobsAsync();
    }
}