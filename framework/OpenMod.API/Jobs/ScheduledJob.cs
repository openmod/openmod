using System;
using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    /// <summary>
    /// Represents a job.
    /// </summary>
    [Serializable]
    public sealed class ScheduledJob : IEquatable<ScheduledJob>
    {
        public ScheduledJob(string name, string task, Dictionary<string, object?> args, string schedule)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            if (string.IsNullOrEmpty(task))
            {
                throw new ArgumentException(nameof(task));
            }

            if (string.IsNullOrEmpty(schedule))
            {
                throw new ArgumentException(nameof(schedule));
            }

            Name = name;
            Schedule = schedule;
            Task = task;
            Args = args ?? throw new ArgumentNullException(nameof(schedule));
            Enabled = true;
        }

        /// <value>
        /// The unique name of the job.
        /// </value>
        public string? Name { get; }

        /// <value>
        /// The job arguments.
        /// </value>
        public Dictionary<string, object?>? Args { get; }

        /// <value>
        /// The task type of the job.
        /// </value>
        public string? Task { get; }

        /// <value>
        /// The schedule expression.
        /// </value>
        public string? Schedule { get; }

        /// <value>
        /// <b>True</b> if the job is enabled; otherwise, <b>false</b>.
        /// </value>
        public bool? Enabled { get; set; }

        public bool Equals(ScheduledJob other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScheduledJob)obj);
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? base.GetHashCode();
        }
    }
}