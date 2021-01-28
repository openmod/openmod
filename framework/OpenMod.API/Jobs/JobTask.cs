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

        /// <summary>
        /// Gets the name of the job.
        /// </summary>
        public string JobName { get; }

        /// <summary>
        /// Gets the task arguments.
        /// </summary>
        public Dictionary<string, object?> Args { get; }

        /// <summary>
        /// Gets the task type.
        /// </summary>
        public string Task { get; }
    }
}