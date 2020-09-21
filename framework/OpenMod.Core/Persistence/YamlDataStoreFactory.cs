using JetBrains.Annotations;
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
        public IDataStore CreateDataStore(DataStoreCreationParameters parameters)
        {
            return new YamlDataStore(parameters);
        }
    }
}