using System;
using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    public sealed class JobTask
    {
        public JobTask(string jobName, string task, Dictionary<string, object?> args)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                throw new ArgumentException(nameof(jobName));
            }

            if (string.IsNullOrEmpty(nameof(task)))
            {
                throw new ArgumentException(nameof(task));
            }

            JobName = jobName;
            Task = task;
            Args = args ?? throw new ArgumentNullException(nameof(args));
        }

        /// <value>
        /// The name of the job.
        /// </value>
        public string JobName { get; }

        /// <value>
        /// The task arguments.
        /// </value>
        public Dictionary<string, object?> Args { get; }

        /// <value>
        /// The task type.
        /// </value>
        public string Task { get; }
    }
}