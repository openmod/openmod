using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;

namespace OpenMod.Core.Persistence
{
    [UsedImplicitly]
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class OpenModDataStoreAccessor : IOpenModDataStoreAccessor
    {
        public OpenModDataStoreAccessor(IDataStoreFactory dataStoreFactory, IRuntime runtime)
        {
            DataStore = dataStoreFactory.CreateDataStore(runtime.OpenModComponentId, runtime.WorkingDirectory);
        }

        public IDataStore DataStore { get; }
    }
}