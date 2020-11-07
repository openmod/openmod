using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Entities.Events
{
    public class RustPlayerDeployingEntityEvent : RustPlayerEvent, ICancellableEvent
    {
        public Deployer Deployer { get; }
        public uint EntityId { get; }
        public bool IsCancelled { get; set; }

        public RustPlayerDeployingEntityEvent(
            RustPlayer player,
            Deployer deployer, 
            uint entityId) : base(player)
        {
            Deployer = deployer;
            EntityId = entityId;
        }
    }
}