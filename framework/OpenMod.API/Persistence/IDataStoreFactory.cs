using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.API.Persistence
{
    [Service]
    public interface IDataStoreFactory
    {
        IDataStore CreateDataStore([CanBeNull] string id, string basePath);
    }
}