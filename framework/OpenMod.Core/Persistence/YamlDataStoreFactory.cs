using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Persistence
{
    [ServiceImplementation(Priority = Priority.Lowest)]
    public class YamlDataStoreFactory : IDataStoreFactory
    {
        /* id is ignored on purpose */
        public IDataStore CreateDataStore(string id, string basePath)
        {
            return new YamlDataStore(basePath);
        }
    }
}