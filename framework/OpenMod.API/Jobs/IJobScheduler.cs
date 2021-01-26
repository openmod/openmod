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
        /// <summary>
        /// Notifies the job scheduler that the server has loaded.<br/>
        /// </summary>
        /// <remarks>
        /// <b>This method is for internal usage only and should not be called by plugins.</b>
        /// </remarks>
        [OpenModInternal]
        Task StartAsync();

        /// <summary>
        /// Schedules a new job.
        /// </summary>
        /// <param name="parameters">The parameters for the job creation.</param>
        Task<ScheduledJob> ScheduleJobAsync(JobCreationParameters parameters);

        /// <summary>
        /// Finds a job based on its name.
        /// </summary>
        /// <returns><b>The scheduled job</b> if found; otherwise, <b>null.</b></returns>
        Task<ScheduledJob?> FindJobAsync(string name);

        /// <summary>
        /// Removes a scheduled job. Will unschedule the job and prevent further execution.
        /// </summary>
        /// <returns><b>True</b> if the job was found and removed; otherwise, <b>false</b>.</returns>
        Task<bool> RemoveJobAsync(string name);

        /// <inheritdoc cref="RemoveJobAsync(string)"/>
        Task<bool> RemoveJobAsync(ScheduledJob job);

        /// <summary>
        /// Gets all scheduled jobs.
        /// </summary>
        /// <param name="includeDisabled">Sets if disabled jobs should be included.</param>
        /// <returns><b>All jobs</b> if <c>includeDisabled</c> is set to true; otherwise, <b>only enabled jobs.</b></returns>
        Task<ICollection<ScheduledJob>> GetScheduledJobsAsync(bool includeDisabled = false);
    }
}