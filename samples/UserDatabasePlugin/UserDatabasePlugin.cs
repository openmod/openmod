using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using UserDatabasePlugin.Database;

[assembly: PluginMetadata("UserDatabasePlugin", Author = "OpenMod", DisplayName = "User Database Plugin")]

namespace UserDatabasePlugin
{
    public class UserDatabasePlugin : OpenModUniversalPlugin
    {
        private readonly ILogger<UserDatabasePlugin> m_Logger;
        private readonly UserDatabaseDbContext m_DbContext;

        public UserDatabasePlugin(
            UserDatabaseDbContext dbContext,
            ILogger<UserDatabasePlugin> logger,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            m_DbContext = dbContext;
            m_Logger = logger;
        }

        protected override async Task OnLoadAsync()
        {
            await m_DbContext.Database.MigrateAsync();

            m_Logger.LogInformation("UserDatabase has been loaded.");
        }

        protected override async Task OnUnloadAsync()
        {
            if (m_DbContext != null)
            {
                await m_DbContext.DisposeAsync();
            }
        }
    }
}
