using System;
using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Represents a task instance for a job.
    /// </summary>
    public sealed class JobTask
    {
        public JobTask(string jobName, string task, Dictionary<string, object?> args) : this(jobName, task, args, Array.Empty<object>())
        {
        }

        public JobTask(string jobName, string task, Dictionary<string, object?> args, params object[] parameters)
        {
            if (string.IsNullOrEmpty(jobName))
            {
                throw new ArgumentException(nameof(jobName));
            }

            if (string.IsNullOrEmpty(task))
            {
                throw new ArgumentException(nameof(task));
            }

            JobName = jobName;
            Task = task;
            Args = args ?? throw new ArgumentNullException(nameof(args));
            Parameters = parameters;
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

        /// <summary>
        /// Gets the task parameters.
        /// </summary>
        public object[] Parameters { get; }
    }
}