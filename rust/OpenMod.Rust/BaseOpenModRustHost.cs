using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Extensions.Games.Abstractions;
using Rust;
using UnityEngine.LowLevel;

namespace OpenMod.Rust
{
    public abstract class BaseOpenModRustHost : IOpenModHost, IDisposable
    {
        public string OpenModComponentId { get; } = "OpenMod.Rust";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        private bool m_IsDisposing;
        private static bool s_UniTaskInited;

        protected BaseOpenModRustHost(
            ILifetimeScope lifetimeScope,
            IRuntime runtime,
            IDataStoreFactory dataStoreFactory
            )
        {
            LifetimeScope = lifetimeScope;
            WorkingDirectory = runtime.WorkingDirectory;
            DataStore = dataStoreFactory.CreateDataStore(new DataStoreCreationParameters
            {
                ComponentId = OpenModComponentId,
                Prefix = "openmod.rust",
                Suffix = null,
                WorkingDirectory = WorkingDirectory
            });
        }

        public Task InitAsync()
        {
            IsComponentAlive = true;

            if (!s_UniTaskInited)
            {
                var unitySynchronizationContetextField =
                    typeof(PlayerLoopHelper).GetField("unitySynchronizationContetext",
                        BindingFlags.Static | BindingFlags.NonPublic);
                unitySynchronizationContetextField.SetValue(null, SynchronizationContext.Current);

                var mainThreadIdField =
                    typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic);
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetDefaultPlayerLoop();
             
                PlayerLoopHelper.Initialize(ref playerLoop);
                s_UniTaskInited = true;
            }

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
