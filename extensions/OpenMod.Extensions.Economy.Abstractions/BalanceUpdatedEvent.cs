using System;
using OpenMod.Core.Eventing;

namespace OpenMod.Extensions.Economy.Abstractions
{
    /// <summary>
    /// Triggered when the balance of an account updates.
    /// </summary>
    public class BalanceUpdatedEvent : Event
    {
        /// <value>
        /// The ID of the account owner.
        /// </value>
        public string OwnerId { get; }

        /// <value>
        /// The actor type of the account owner.
        /// </value>
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
        /// The reason for the balance update.
        /// </value>
        public string? Reason { get; }

        public BalanceUpdatedEvent(string ownerId, string ownerType, decimal oldBalance, decimal newBalance, string? reason)
        {
            if (string.IsNullOrEmpty(ownerId))
            {
                throw new ArgumentException(nameof(ownerId));
            }

            if (string.IsNullOrEmpty(ownerType))
            {
                throw new ArgumentException(nameof(ownerType));
            }

            OwnerId = ownerId;
            OwnerType = ownerType;
            OldBalance = oldBalance;
            NewBalance = newBalance;
            Reason = reason;
        }
    }
}