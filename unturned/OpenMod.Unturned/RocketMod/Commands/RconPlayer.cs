using OpenMod.API.Commands;
using Rocket.API;

namespace OpenMod.Unturned.RocketMod.Commands
{
    public class RconPlayer : ConsolePlayer
    {
        public readonly ICommandActor? CommandActor;

        public RconPlayer()
        { }

        public RconPlayer(ICommandActor actor)
        {
            CommandActor = actor;
        }
    }
}