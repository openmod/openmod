using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins;

[assembly: PluginMetadata("UniversalSamplePlugin", Author = "OpenMod", DisplayName = "Universal Sample Plugin")]

namespace UniversalSamplePlugin
{
    public class UniversalSamplePlugin : OpenModUniversalPlugin
    {
        private readonly IStringLocalizer m_StringLocalizer;
        private readonly ILogger<UniversalSamplePlugin> m_Logger;
        private readonly IConfiguration m_Configuration;
        private readonly IEventBus m_EventBus;

        public UniversalSamplePlugin(
            IStringLocalizer stringLocalizer,
            ILogger<UniversalSamplePlugin> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IEventBus eventBus) : base(serviceProvider)
        {
            m_StringLocalizer = stringLocalizer;
            m_Logger = logger;
            m_Configuration = configuration;
            m_EventBus = eventBus;
        }

        protected override Task OnLoadAsync()
        {
            m_Logger.LogInformation("UniversalSamplePlugin has been loaded.");
            m_Logger.LogInformation($"Configuration value: {m_Configuration["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at somevalue: {m_StringLocalizer["somevalue"]}");
            m_Logger.LogInformation($"String localizer value at someothervalue:somenestedvalue: {m_StringLocalizer["someothervalue:somenestedvalue"]}");

            AsyncHelper.Schedule("SampleEvent emit", async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    var @event = new SampleEvent();
                    await m_EventBus.EmitAsync(this, this, @event);
                }
            });

            return Task.CompletedTask;
        }
    }
}
