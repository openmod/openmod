using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata("PLUGIN-ID", DisplayName = "PLUGIN-NAME")]
namespace MyOpenModPlugin
{
    public class MyOpenModPlugin : OpenModUnturnedPlugin
    {
        private readonly ILogger<MyOpenModPlugin> m_Logger;

        public MyOpenModPlugin(ILogger<MyOpenModPlugin> logger, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_Logger = logger;
        }

        protected override async UniTask OnLoadAsync()
        {
			// await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation("Hello World!");
            return UniTask.CompletedTask;
        }

        protected override async UniTask OnUnloadAsync()
        {
            // await UniTask.SwitchToMainThread(); uncomment if you have to access Unturned or UnityEngine APIs
            m_Logger.LogInformation("Good bye :(");
            return UniTask.CompletedTask;
        }
    }
}
