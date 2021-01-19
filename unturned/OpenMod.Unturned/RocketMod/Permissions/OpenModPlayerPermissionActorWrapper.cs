using OpenMod.API.Permissions;
using OpenMod.Core.Users;
using Rocket.API;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class OpenModPlayerPermissionActorWrapper : IPermissionActor
    {
        public IRocketPlayer Player;

        public OpenModPlayerPermissionActorWrapper(IRocketPlayer player)
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