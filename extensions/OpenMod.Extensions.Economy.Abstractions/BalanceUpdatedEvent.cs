using System;
using OpenMod.Core.Eventing;

namespace OpenMod.Extensions.Economy.Abstractions
{
    /// <summary>
    /// Triggered when the balance of an account updates.
    /// </summary>
    public class BalanceUpdatedEvent : Event
    {
        /// <summary>
        /// Gets the ID of the account owner.
        /// </summary>
        public string OwnerId { get; }

        /// <summary>
        /// Gets the actor type of the account owner.
        /// </summary>
        public string OwnerType { get; }

        /// <summary>
        /// Gets the old balance.
        /// </summary>
        public decimal OldBalance { get; }

        /// <summary>
        /// Gets the new balance.
        /// </summary>
        public decimal NewBalance { get; }

        /// <summary>
        /// Gets the reason for the balance update.
        /// </summary>
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