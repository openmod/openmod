using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [OpenModInternal]
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class CommandParameterResolver : ICommandParameterResolver, IAsyncDisposable
    {
        private bool m_IsDisposing;

        private readonly List<ICommandParameterResolveProvider> m_CommandParameterResolveProviders;

        public CommandParameterResolver(
            ILifetimeScope lifetimeScope,
            IOptions<CommandParameterResolverOptions> options)
        {
            m_CommandParameterResolveProviders = options.Value.CommandParameterResolverTypes
                .Select(type => (ICommandParameterResolveProvider)ActivatorUtilitiesEx.CreateInstance(lifetimeScope, type))
                .ToList();
        }

        public async Task<object?> ResolveAsync(Type type, string input)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException($"'{nameof(input)}' cannot be null or empty", nameof(input));
            }

            foreach (var resolver in m_CommandParameterResolveProviders)
            {
                if (!resolver.Supports(type))
                {
                    continue;
                }

                var value = await resolver.ResolveAsync(type, input);
                if (value != null)
                {
                    return value;
                }
            }

            return null;
        }

        public async ValueTask DisposeAsync()
        {
            if (m_IsDisposing)
            {
                return;
            }

            m_IsDisposing = true;
            await m_CommandParameterResolveProviders.DisposeAllAsync();
        }
    }
}
