using System;
using System.Collections.Generic;
using System.Threading;
using OpenMod.API.Persistence;

namespace OpenMod.API.Jobs
{
    /// <summary>
    ///     Represents a job.
    /// </summary>
    [Serializable]
    public sealed class ScheduledJob : IEquatable<ScheduledJob>
    {
        /// <summary>
        ///     Gets the unique name of the job.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        ///     Gets the job arguments.
        /// </summary>
        public Dictionary<string, object?>? Args { get; set; }

        /// <summary>
        ///     Gets the task type of the job.
        /// </summary>
        public string? Task { get; set; }

        /// <summary>
        ///     Gets the schedule time.
        /// </summary>
        public string? Schedule { get; set; }

        /// <summary>
        ///     Checks if the job is enabled.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        ///     Cancelling a job will always dispose the token source and remove it from schedule list
        /// </summary>
        [SerializeIgnore]
        public CancellationTokenSource? CancellationTokenSource { get; set; }

        public override int GetHashCode()
        {
            // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Name?.GetHashCode() ?? base.GetHashCode();
        }

        public bool Equals(ScheduledJob? other)
        {
#pragma warning disable IDE0041 // Use '== null' or 'is null'
            if (ReferenceEquals(objA: null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(objA: null, obj))
            {
                return false;
            }
#pragma warning restore IDE0041 // Use '== null' or 'is null'

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((ScheduledJob)obj);
        }
    }
}