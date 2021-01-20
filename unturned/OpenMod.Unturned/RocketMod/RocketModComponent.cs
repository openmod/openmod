using System;
using Autofac;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Core.Ioc;
using Rocket.Unturned;

namespace OpenMod.Unturned.RocketMod
{
    public class RocketModComponent : IRocketModComponent
    {
        private readonly IRuntime m_Runtime;

        public RocketModComponent(IRuntime runtime)
        {
            m_Runtime = runtime;
            LifetimeScope = runtime.LifetimeScope.BeginLifetimeScopeEx(builder =>
            {
                builder.Register<IOpenModComponent>(_ => this)
                    .SingleInstance()
                    .OwnedByLifetimeScope();
            });
        }

        public string OpenModComponentId { get; } = "RocketMod";

        public string WorkingDirectory
        {
            get => RocketModIntegration.GetRocketFolder();
        }

        public bool IsComponentAlive
        {
            get => m_Runtime.IsComponentAlive && RocketModIntegration.IsRocketModReady() && U.Instance != null;
        }

        public ILifetimeScope LifetimeScope { get; }

        public IDataStore DataStore { get => throw new NotSupportedException("DataStore is not supported for RocketMod"); }
    }
}