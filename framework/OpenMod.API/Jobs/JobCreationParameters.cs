using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Parameters used for scheduling jobs.
    /// </summary>
    public class JobCreationParameters
    {
        /// <summary>
        /// Gets the unique name of the job.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets the job arguments.
        /// </summary>
        public Dictionary<string, object?> Args { get; set; } = null!;

        /// <summary>
        /// Gets the task type of the job. Requires a <see cref="ITaskExecutor"/> that can execute the given type.
        /// </summary>
        public string Task { get; set; } = null!;

        /// <summary>
        /// Gets the schedule expression.
        /// </summary>
        public string Schedule { get; set; } = null!;
    }
}