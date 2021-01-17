using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;

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
            return ActivatorUtilities.CreateInstance<YamlDataStore>(m_ServiceProvider, parameters);
        }
    }
}