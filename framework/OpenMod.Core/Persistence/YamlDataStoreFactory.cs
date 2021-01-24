using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Persistence
{
    [OpenModInternal]
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class YamlDataStoreFactory : IDataStoreFactory
    {
        private readonly IServiceProvider m_ServiceProvider;

        public YamlDataStoreFactory(IServiceProvider serviceProvider)
        {
            m_ServiceProvider = serviceProvider;
        }

        public IDataStore CreateDataStore(DataStoreCreationParameters parameters)
        {
            var lifetime = parameters.Component?.LifetimeScope
                           ?? m_ServiceProvider.GetRequiredService<IRuntime>().LifetimeScope;

            return ActivatorUtilitiesEx.CreateInstance<YamlDataStore>(lifetime, parameters);
        }
    }
}