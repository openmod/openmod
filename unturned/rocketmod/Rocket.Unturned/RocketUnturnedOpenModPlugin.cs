using System;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenMod.Core.Plugins;
using OpenMod.Unturned.Plugins;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: PluginMetadata(Author = "OpenMod", DisplayName = "OpenMod RocketMod Plugin", Id = "Rocket.Unturned")]

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

        protected override UniTask OnLoadAsync()
        {
            U.Logger = m_LoggerFactory.CreateLogger("RocketMod");
            U.PluginInstance = this;

            uComp = new U();
            uComp.initialize();
            
            return base.OnLoadAsync();
        }

        protected override UniTask OnUnloadAsync()
        {
            U.Logger = NullLogger.Instance;
            U.Instance.shutdown();
            U.PluginInstance = null;
            uComp.shutdown();

            return base.OnUnloadAsync();
        }
    }
}
