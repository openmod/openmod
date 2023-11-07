using OpenMod.API.Commands;
using SDG.Unturned;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OpenMod.Unturned.Locations
{
    public class UnturnedLocationCommandParameterResolveProvider : ICommandParameterResolveProvider
    {
        private readonly IUnturnedLocationDirectory m_LocationDirectory;
        private readonly ILogger<UnturnedLocationCommandParameterResolveProvider> m_Logger;

        public UnturnedLocationCommandParameterResolveProvider(ILogger<UnturnedLocationCommandParameterResolveProvider> logger, IUnturnedLocationDirectory locationDirectory)
        {
            m_LocationDirectory = locationDirectory;
            m_Logger = logger;
        }

        public bool Supports(Type type)
        {
            // ReSharper disable once InvertIf
            if (type == typeof(LocationNode))
            {
                m_Logger.LogWarning("{0} is obsolete, please change to UnturnedLocation instead", typeof(LocationNode));
                return false;
            }
            
            return type == typeof(UnturnedLocation) || type == typeof(LocationDevkitNode);
        }

        public Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                return Task.FromResult((object?) null);
            }

            if (!Supports(type))
            {
                throw new ArgumentException("The given type is not supported", nameof(type));
            }

            var location = m_LocationDirectory.FindLocation(input, exact: false);
            return type == typeof(UnturnedLocation) ? Task.FromResult<object?>(location) : Task.FromResult<object?>(location?.DevkitNode);
        }
    }
}
