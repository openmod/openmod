using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;

namespace OpenMod.Rust.Oxide
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModRustOxideHost : BaseOpenModRustHost
    {
        public OpenModRustOxideHost(
            ILifetimeScope lifetimeScope,
            IRuntime runtime,
            IDataStoreFactory dataStoreFactory) :
            base(lifetimeScope, runtime, dataStoreFactory)
        {
        }

        protected override Task OnInitAsync()
        {
            // Rust todo: listen to Oxide hooks and translate to OpenMod events
            return Task.CompletedTask;
        }

        protected override void OnDispose()
        {
            // Rust todo: stop listening to Oxide hooks
        }
    }
}