using OpenMod.Core.Permissions;
using System;
using System.Threading.Tasks;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("reload")]
    [CommandParent(typeof(CommandRole))]
    public class CommandRoleReload : Command
    {
        private readonly IPermissionRolesDataStore m_PermissionRolesDataStore;

        public CommandRoleReload(IServiceProvider serviceProvider, IPermissionRolesDataStore permissionRolesDataStore) : base(serviceProvider)
        {
            m_PermissionRolesDataStore = permissionRolesDataStore;
        }

        protected override async Task OnExecuteAsync()
        {
            await PrintAsync("Reloading roles...");
            await m_PermissionRolesDataStore.ReloadAsync();
            await PrintAsync("Roles have been reloaded.");
        }
    }
}
