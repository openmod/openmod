using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    public sealed class JobTask
    {
        /// <value>
        /// The name of the job.
        /// </value>
        public string JobName { get; set; }

        /// <value>
        /// The task arguments.
        /// </value>
        public Dictionary<string, object> Args { get; set; }

        /// <value>
        /// The task type.
        /// </value>
        public string Task { get; set; }
    }
}