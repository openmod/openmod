using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Plugins;
using OpenMod.Unturned.Plugins;

[assembly: PluginMetadata(Author = "OpenMod", DisplayName = "Unturned Sample Plugin", Id = "UnturnedSamplePlugin", Version = "1.0.0")]

namespace OpenMod.Unturned.SamplePlugin
{
    [UsedImplicitly]
    public class UnturnedSamplePlugin : OpenModUnturnedPlugin
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UnturnedSamplePlugin> m_Logger;
        private readonly IConfiguration m_Configuration;

        public UnturnedSamplePlugin(
            IStringLocalizer stringLocalizer,
            ILogger<UnturnedSamplePlugin> logger, 
            IConfiguration configuration,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_Configuration = configuration;
        }

        protected override async UniTask OnLoadAsync()
        {
            await UniTask.SwitchToThreadPool();
            m_Logger.LogInformation("SampleUnturnedPlugin has been loaded.");
            m_Logger.LogInformation($"Configuration value: {m_Configuration["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at somevalue: {m_StringLocalizer["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at someothervalue:somenestedvalue: {m_StringLocalizer["someothervalue:somenestedvalue"]}");
        }
    }
}
