using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Permissions
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class PermissionChecker : IPermissionChecker, IAsyncDisposable
    {
        private readonly IServiceProvider m_ServiceProvider;
        private readonly IOptions<PermissionCheckerOptions> m_Options;
        private readonly List<IPermissionStore> m_PermissionSources;
        private readonly List<IPermissionCheckProvider> m_PermissionCheckProviders;

        public PermissionChecker(
            IServiceProvider serviceProvider,
            IOptions<PermissionCheckerOptions> options)
        {
            m_ServiceProvider = serviceProvider;
            m_Options = options;
            m_PermissionSources = new List<IPermissionStore>();
            m_PermissionCheckProviders = new List<IPermissionCheckProvider>();
        }

        public IReadOnlyCollection<IPermissionCheckProvider> PermissionCheckProviders
        {
            get { return m_PermissionCheckProviders; }
        }

        public IReadOnlyCollection<IPermissionStore> PermissionStores
        {
            get { return m_PermissionSources; }
        }

        public async Task<PermissionGrantResult> CheckPermissionAsync(IPermissionActor actor, string permission)
        {
            foreach (var provider in m_PermissionCheckProviders.Where(c => c.SupportsActor(actor)))
            {
                var result = await provider.CheckPermissionAsync(actor, permission);
                if (result != PermissionGrantResult.Default)
                {
                    return result;
                }
            }

            return PermissionGrantResult.Default;
        }

        public Task InitAsync()
        {
            foreach (var permissionSourceType in m_Options.Value.PermissionSources)
            {
                m_PermissionSources.Add((IPermissionStore)ActivatorUtilities.CreateInstance(m_ServiceProvider, permissionSourceType));
            }

            foreach (var permissionCheckProviderType in m_Options.Value.PermissionCheckProviders)
            {
                m_PermissionCheckProviders.Add((IPermissionCheckProvider)ActivatorUtilities.CreateInstance(m_ServiceProvider, permissionCheckProviderType));
            }

            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var permissionCheckProvider in m_PermissionCheckProviders)
            {
                if (permissionCheckProvider is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (permissionCheckProvider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            foreach (var permissionSource in m_PermissionSources)
            {
                if (permissionSource is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (permissionSource is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}