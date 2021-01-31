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
        /// <summary>
        /// Gets the unique name of the job.
        /// </summary>
        public string? Name { get; set; } 

        /// <summary>
        /// Gets the job arguments.
        /// </summary>
        public Dictionary<string, object?>? Args { get; set; }

        /// <summary>
        /// Gets the task type of the job.
        /// </summary>
        public string? Task { get; set; }

        /// <summary>
        /// Gets the schedule expression.
        /// </summary>
        public string? Schedule { get; set; }

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