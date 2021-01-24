using System;
using System.Collections.Generic;

namespace OpenMod.API.Jobs
{
    [Serializable]
    public class ScheduledJob : IEquatable<ScheduledJob>
    {
        public string Name { get; set; }

        public Dictionary<string, object> Args { get; set; }

        public string Task { get; set; }

        public string Schedule { get; set; }

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