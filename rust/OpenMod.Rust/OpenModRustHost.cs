using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Persistence;
using OpenMod.API.Prioritization;
using Rust;
using UnityEngine.LowLevel;

namespace OpenMod.Rust
{
    [ServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class OpenModRustHost : IOpenModHost, IDisposable
    {
        public string OpenModComponentId { get; } = "OpenMod.Rust";

        public string WorkingDirectory { get; }

        public bool IsComponentAlive { get; private set; }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get; }

        private bool m_IsDisposing;
        private static bool s_UniTaskInited;

        public OpenModRustHost(
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

        public async Task InitAsync()
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
        }

        public Task ShutdownAsync()
        {
            Application.Quit();
            return Task.CompletedTask;
        }

        public bool HasCapability(string capability)
        {
            // todo: implement universal API
            return false;
        }

        public void Dispose()
        {
            if (m_IsDisposing)
            {
                return;
            }

            m_IsDisposing = true;
            IsComponentAlive = false;
        }
    }
}