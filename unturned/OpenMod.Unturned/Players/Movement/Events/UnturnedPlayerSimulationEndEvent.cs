using System.Numerics;
using OpenMod.Unturned.Events;

namespace OpenMod.Unturned.Players.Movement.Events
{
    public class UnturnedPlayerSimulationEndEvent : UnturnedPlayerEvent
    {
        public UnturnedPlayerSimulationEndEvent(UnturnedPlayer player, uint simulation, Vector2 input, Vector2 look, bool inputJump,
            bool inputSprint, float deltaTime) : base(player)
        {
            Simulation = simulation;
            Input = input;
            Look = look;
            InputJump = inputJump;
            InputSprint = inputSprint;
            DeltaTime = deltaTime;
        }

        public uint Simulation { get; set; }
        public Vector2 Input { get; set; }
        public Vector2 Look { get; set; }
        public bool InputJump { get; set; }
        public bool InputSprint { get; set; }
        public float DeltaTime { get; set; }
    }
}