using OpenMod.API.Commands;
using Steamworks;
using System;
using System.Threading.Tasks;

namespace OpenMod.Unturned.Steam
{
    public class CSteamIDCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return Task.FromResult((object?)CSteamID.Nil);
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            return Task.FromResult((object?)new CSteamID(ulong.Parse(input)));
        }

        public bool Supports(Type type)
        {
            return type == typeof(CSteamID);
        }
    }
}
