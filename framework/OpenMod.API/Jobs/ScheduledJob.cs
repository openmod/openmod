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
        public ScheduledJob()
        {
            
        }

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

        /// <summary>
        /// Gets the unique name of the job.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the job arguments.
        /// </summary>
        public Dictionary<string, object?>? Args { get; }

        /// <summary>
        /// Gets the task type of the job.
        /// </summary>
        public string? Task { get; }

        /// <summary>
        /// Gets the schedule expression.
        /// </summary>
        public string? Schedule { get; }

        /// <summary>
        /// Checks if the job is enabled.
        /// </summary>
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