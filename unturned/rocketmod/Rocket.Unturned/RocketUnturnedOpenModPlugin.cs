using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMod.Core.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata("Rocket.Unturned", Author = "OpenMod", DisplayName = "OpenMod RocketMod Plugin")]

namespace Rocket.Unturned
{
    public class RocketUnturnedOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly ILoggerFactory m_LoggerFactory;
        private U uComp;

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

            uComp = new U();
            uComp.initialize();
        }

        protected override async UniTask OnUnloadAsync()
        {
            await base.OnUnloadAsync();
            await UniTask.SwitchToMainThread();
            
            U.Logger = NullLogger.Instance;
            U.Instance.shutdown();
            U.PluginInstance = null;
            uComp.shutdown();
        }
    }
}
