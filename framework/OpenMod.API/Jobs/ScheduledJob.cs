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
        /// <value>
        /// The unique name of the job.
        /// </value>
        public string Name { get; set; }

        /// <value>
        /// The job arguments.
        /// </value>
        public Dictionary<string, object> Args { get; set; }

        /// <value>
        /// The task type of the job.
        /// </value>
        public string Task { get; set; }

        /// <value>
        /// The schedule expression.
        /// </value>
        public string Schedule { get; set; }

        /// <value>
        /// <b>True</b> if the job is enabled; otherwise, <b>false</b>.
        /// </value>
        public bool Enabled { get; set; }

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
            return Equals((ScheduledJob) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}