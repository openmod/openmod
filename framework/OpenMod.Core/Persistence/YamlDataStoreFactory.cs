using System;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using OpenMod.Core.Plugins;

namespace OpenMod.Core.Persistence
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class YamlDataStoreFactory : IDataStoreFactory
    {
        private readonly IRuntime m_Runtime;

        public YamlDataStoreFactory(IRuntime runtime)
        {
            m_Runtime = runtime;
        }

        public IDataStore CreateDataStore(string basePath)
        {
            return new YamlDataStore(basePath);
        }
    }
}