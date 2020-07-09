using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata("Rocket.Unturned", Author = "OpenMod", DisplayName = "OpenMod RocketMod Plugin")]

namespace Rocket.Unturned
{
    public class RocketUnturnedOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly ILoggerFactory m_LoggerFactory;
        private U m_RocketComponent;

        public RocketUnturnedOpenModPlugin(IServiceProvider serviceProvider, ILoggerFactory loggerFactory) : base(serviceProvider)
        {
            m_LoggerFactory = loggerFactory;
        }

        protected override async UniTask OnLoadAsync()
        {
            await base.OnLoadAsync();
            await UniTask.SwitchToMainThread();

            U.Logger = m_LoggerFactory.CreateLogger("RocketMod");
            U.PluginInstance = this;

            m_RocketComponent = new U();
            m_RocketComponent.initialize();
        }

        protected override async UniTask OnUnloadAsync()
        {
            await base.OnUnloadAsync();
            await UniTask.SwitchToMainThread();
            
            U.Logger = NullLogger.Instance;
            U.Instance?.shutdown();
            U.PluginInstance = null;

            if (m_RocketComponent != null) // can not use ? null conditional operator on Unity components
            {
                m_RocketComponent.shutdown();
            }
        }
    }
}
