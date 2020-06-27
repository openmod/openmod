using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        protected override Task OnLoadAsync()
        {
            m_Logger.LogInformation("Hello World!");
            return Task.CompletedTask;
        }

        protected override Task OnUnloadAsync()
        {
            m_Logger.LogInformation("Good bye :(");
            return Task.CompletedTask;
        }
    }
}
