using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.Core.Plugins;

[assembly: PluginMetadata("UniversalSamplePlugin", Author = "OpenMod", DisplayName = "Universal Sample Plugin")]

namespace UniversalSamplePlugin
{
    public class UniversalSamplePlugin : OpenModUniversalPlugin
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UniversalSamplePlugin> m_Logger;
        private readonly IConfiguration m_Configuration;

        public UniversalSamplePlugin(
            IStringLocalizer stringLocalizer,
            ILogger<UniversalSamplePlugin> logger, 
            IConfiguration configuration,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_Configuration = configuration;
        }

        protected override async Task OnLoadAsync()
        {
            m_Logger.LogInformation("UniversalSamplePlugin has been loaded.");
            m_Logger.LogInformation($"Configuration value: {m_Configuration["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at somevalue: {m_StringLocalizer["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at someothervalue:somenestedvalue: {m_StringLocalizer["someothervalue:somenestedvalue"]}");
        }
    }
}
