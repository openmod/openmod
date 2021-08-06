using System;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Extensions.Games.Abstractions;
using Rust;

namespace OpenMod.Rust
{
    public abstract class BaseOpenModRustHost : IOpenModHost, IDisposable
    {
        private readonly IRuntime m_Runtime;
        public string OpenModComponentId { get; } = "OpenMod.Rust";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        private bool m_IsDisposing;

        protected BaseOpenModRustHost(
            ILifetimeScope lifetimeScope,
            IRuntime runtime,
            IDataStoreFactory dataStoreFactory
            )
        {
            m_Runtime = runtime;
            LifetimeScope = lifetimeScope;
            WorkingDirectory = runtime.WorkingDirectory;
            DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                Component = this,
                Prefix = "openmod.rust",
                Suffix = null,
                WorkingDirectory = WorkingDirectory
            });
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;

            return OnInitAsync();
        }

        protected abstract Task OnInitAsync();

        public Task ShutdownAsync()
        {
            async UniTask ShutdownTask()
            {
                await UniTask.SwitchToMainThread();
                Application.Quit();
            }

            return ShutdownTask().AsTask();
        }

        public bool HasCapability(string capability)
        {
            if (capability == KnownGameCapabilities.Inventory)
            {
                return true;
            }

            if (capability == KnownGameCapabilities.Health)
            {
                return true;
            }

            return false;
        }

        public Task PerformHardReloadAsync()
        {
            // todo
            return m_Runtime.PerformSoftReloadAsync();
        }

        public void Dispose()
        {
            if (m_IsDisposing)
            {
                return;
            }

            OnDispose();

            m_IsDisposing = true;
            IsComponentAlive = false;
        }

        protected abstract void OnDispose();
    }
}
