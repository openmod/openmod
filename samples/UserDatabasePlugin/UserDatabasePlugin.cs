using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenMod.API.Permissions;
using OpenMod.API.Plugins;
using OpenMod.Core.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;
using UserDatabasePlugin.Database;

[assembly: PluginMetadata("UserDatabasePlugin",
    Author = "OpenMod",
    DisplayName = "User Database Plugin",
    Website = "https://github.com/openmod/openmod/tree/main/samples/UserDatabasePlugin")]

namespace UserDatabasePlugin
{
    public class UserDatabasePlugin : OpenModUniversalPlugin
    {
        private readonly ILogger<UserDatabasePlugin> m_Logger;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly UserDatabaseDbContext m_DbContext;

        public UserDatabasePlugin(
            ILogger<UserDatabasePlugin> logger,
            IServiceProvider serviceProvider,
            IPermissionRegistry permissionRegistry,
            UserDatabaseDbContext dbContext) : base(serviceProvider)
        {
            m_DbContext = dbContext;
            m_Logger = logger;
            m_PermissionRegistry = permissionRegistry;
        }

        protected override async Task OnLoadAsync()
        {
            await m_DbContext.OpenModMigrateAsync();

            m_Logger.LogInformation("UserDatabase has been loaded");

            // this is actually just a silly demo for permission registrations
            // the full permission which the user will see and manage will be "UserDatabasePlugin:anonymous" because RegisterPermission prefixes it with plugin ID
            // when you want to check against this permission you need to call IPermissionChecker.CheckPermissionAsync(actor, "anonymous"), it is again automatically prefixed;
            m_PermissionRegistry.RegisterPermission(this, "anonymous", description: "Prevents a user's name from getting logged.");
        }
    }
}
