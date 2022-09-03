using OpenMod.API.Permissions;
using OpenMod.Core.Users;
using OpenMod.Unturned.RocketMod.Commands;
using Rocket.API;

namespace OpenMod.Unturned.RocketMod.Permissions
{
    public class OpenModPlayerPermissionActorWrapper : IPermissionActor
    {
        public IRocketPlayer Player;

        public OpenModPlayerPermissionActorWrapper(IRocketPlayer player)
        {
            Player = player;
            switch (player)
            {
                case RconPlayer rcon:
                    Id = rcon.CommandActor?.Id ?? "root";
                    Type = rcon.CommandActor?.Type ?? KnownActorTypes.Rcon;
                    DisplayName = rcon.CommandActor?.DisplayName ?? "Rcon";
                    FullActorName = rcon.CommandActor?.FullActorName ?? $"rocketmodRcon/{Id} ({DisplayName})";
                    break;

                case ConsolePlayer:
                    Id = player.Id;
                    Type = KnownActorTypes.Console;
                    DisplayName = player.DisplayName;
                    FullActorName = $"rocketmodConsole/{Id} ({DisplayName})";
                    break;

                default:
                    Id = player.Id;
                    Type = KnownActorTypes.Player;
                    DisplayName = player.DisplayName;
                    FullActorName = $"rocketmodPlayer/{Id} ({DisplayName})";
                    break;
            }
        }

        public string Id { get; }
        public string Type { get; }
        public string DisplayName { get; }
        public string FullActorName { get; }
    }
}