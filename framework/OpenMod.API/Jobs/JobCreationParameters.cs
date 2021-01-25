using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Parameters used for scheduling jobs.
    /// </summary>
    public class JobCreationParameters
    {
        /// <value>
        /// The unique name of the job.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// The job arguments.
        /// </value>
        public Dictionary<string, object> Args { get; set; }

        /// <value>
        /// The task type of the job. Requires a <see cref="ITaskExecutor"/> that can execute the given type.
        /// </value>
        public string Task { get; set; }

        /// <value>
        /// The schedule expression.
        /// </value>
        public string Schedule { get; set; }
    }
}