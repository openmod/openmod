using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Rust.Oxide.Events;

namespace OpenMod.Rust.Oxide
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModRustOxideHost : BaseOpenModRustHost
    {
        private readonly IServiceProvider m_ServiceProvider;
        private OxideEventsActivator? m_OxideEventsActivator;

        public OpenModRustOxideHost(
            IServiceProvider serviceProvider,
            ILifetimeScope lifetimeScope,
            IRuntime runtime,
            IDataStoreFactory dataStoreFactory) :
            base(lifetimeScope, runtime, dataStoreFactory)
        {
            m_ServiceProvider = serviceProvider;
        }

        protected override Task OnInitAsync()
        {
            m_OxideEventsActivator = ActivatorUtilities.CreateInstance<OxideEventsActivator>(m_ServiceProvider);
            m_OxideEventsActivator.ActivateEventListeners();

            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            m_OxideEventsActivator?.Dispose();
        }
    }
}
