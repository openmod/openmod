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
    [ServiceImplementation(Priority = Priority.Lowest, Lifetime = ServiceLifetime.Singleton)]
    public class OpenModDataStoreAccessor : IOpenModDataStoreAccessor
    {
        private readonly IRuntime m_Runtime;

        public OpenModDataStoreAccessor(IRuntime runtime)
        {
            m_Runtime = runtime;
        }

        public IDataStore DataStore
        {
            get { return m_Runtime.DataStore!; }
        }
    }
}