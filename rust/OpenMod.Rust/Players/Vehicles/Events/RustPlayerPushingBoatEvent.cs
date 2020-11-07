using OpenMod.API.Eventing;
using OpenMod.Rust.Players.Events;

namespace OpenMod.Rust.Players.Vehicles.Events
{
    public class RustPlayerPushingBoatEvent : RustPlayerEvent, ICancellableEvent
    {
        public MotorRowboat MotorRowboat { get; }
        
        public bool IsCancelled { get; set; }
        
        public RustPlayerPushingBoatEvent(
            RustPlayer player,
            MotorRowboat motorRowboat) : base(player)
        {
            MotorRowboat = motorRowboat;
        }
    }
}