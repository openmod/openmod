using OpenMod.API.Permissions;
using OpenMod.Core.Users;
using Rocket.API;

namespace Rocket.PermissionLink
{
    public class RocketPlayerPermissionActorWrapper : IPermissionActor
    {
        public IRocketPlayer Player;

        public RocketPlayerPermissionActorWrapper(IRocketPlayer player)
        {
            Player = player;
            Id = player.Id;
            Type = KnownActorTypes.Player;
            DisplayName = player.DisplayName;
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName { get; }
    }
}