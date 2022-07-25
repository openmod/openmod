using System;
using System.Linq;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Extensions.Games.Abstractions.Players;
using SDG.Unturned;

namespace OpenMod.Unturned.Players
{
    [Priority(Priority = Priority.Low)]
    public class UnturnedPlayerCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        public bool Supports(Type type)
        {
            return typeof(IPlayer).IsAssignableFrom(type);
        }

        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (!Supports(type))
            {
                var ex = new ArgumentException("The given type is not supported", nameof(type));
                return Task.FromException<object?>(ex);
            }

            var steamPlayer = Provider.clients
                .FirstOrDefault(x => input == x.playerID.steamID.ToString() || input.Equals(x.playerID.playerName, StringComparison.OrdinalIgnoreCase) || input.Equals(x.playerID.characterName, StringComparison.OrdinalIgnoreCase));

            var player = steamPlayer == null ? null : new UnturnedPlayer(steamPlayer.player);

            return Task.FromResult<object?>(player);
        }
    }
}
