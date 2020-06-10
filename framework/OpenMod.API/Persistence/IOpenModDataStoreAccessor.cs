using OpenMod.API.Ioc;

namespace OpenMod.API.Persistence
{
    [Service]
    public interface IOpenModDataStoreAccessor
    {
        IDataStore DataStore { get; }
    }
}