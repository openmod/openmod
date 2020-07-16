using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using UserDatabasePlugin.Database;

[assembly: PluginMetadata("UserDatabasePlugin", Author = "OpenMod", DisplayName = "User Database Plugin")]

namespace UserDatabasePlugin
{
    public class UserDatabasePlugin : OpenModUniversalPlugin
    {
        private readonly ILogger<UserDatabasePlugin> m_Logger;
        private readonly UserDatabaseDbContext m_DbContext;

        public UserDatabasePlugin(
            ILogger<UserDatabasePlugin> logger,
            IServiceProvider serviceProvider,
            UserDatabaseDbContext dbContext) : base(serviceProvider)
        {
            m_DbContext = dbContext;
            m_Logger = logger;
        }

        protected override async Task OnLoadAsync()
        {
            await m_DbContext.OpenModMigrateAsync();

            m_Logger.LogInformation("UserDatabase has been loaded.");
        }
    }
}
