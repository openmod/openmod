using JetBrains.Annotations;
using OpenMod.Core.Eventing;

namespace OpenMod.Extensions.Economy.Abstractions
{
    /// <summary>
    /// Triggered when the balance of an account updates.
    /// </summary>
    public class BalanceUpdatedEvent : Event
    {
        /// <value>
        /// The Id of the account owner. Cannot be null.
        /// </value>
        [NotNull]
        public string OwnerId { get; }

        /// <value>
        /// The actor type of the account owner. Cannot be null.
        /// </value>
        [NotNull]
        public string OwnerType { get; }

        /// <value>
        /// The old balance.
        /// </value>
        public decimal OldBalance { get; }

        /// <value>
        /// The new balance.
        /// </value>
        public decimal NewBalance { get; }

        /// <value>
        /// The reason for the balance update. Can be null.
        /// </value>
        [CanBeNull]
        public string Reason { get; }

        public BalanceUpdatedEvent(string ownerId, string ownerType, decimal oldBalance, decimal newBalance, string reason)
        {
            OwnerId = ownerId;
            OwnerType = ownerType;
            OldBalance = oldBalance;
            NewBalance = newBalance;
            Reason = reason;
        }
    }
}