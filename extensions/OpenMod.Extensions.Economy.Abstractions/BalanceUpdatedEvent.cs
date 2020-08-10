using JetBrains.Annotations;
using OpenMod.Core.Eventing;

namespace OpenMod.Extensions.Economy.Abstractions
{
    public class BalanceUpdatedEvent : Event
    {
        public string OwnerId { get; }
        
        public string OwnerType { get; }
        
        public decimal OldAmount { get; }
        
        public decimal NewAmount { get; }

        [CanBeNull]
        public string Reason { get; }

        public BalanceUpdatedEvent(string ownerId, string ownerType, decimal oldAmount, decimal newAmount, string reason)
        {
            OwnerId = ownerId;
            OwnerType = ownerType;
            OldAmount = oldAmount;
            NewAmount = newAmount;
            Reason = reason;
        }
    }
}