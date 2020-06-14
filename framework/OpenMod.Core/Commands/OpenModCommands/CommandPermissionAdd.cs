using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("add")]
    [CommandAlias("+")]
    [CommandAlias("a")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionAdd : CommandPermissionAction
    {
        private readonly IUsersDataStore m_UsersDataStore;

        public CommandPermissionAdd(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUsersDataStore usersDataStore) : base(serviceProvider, permissionGroupStore, usersDataStore)
        {
            m_UsersDataStore = usersDataStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, string permissionToUpdate)
        {
            var user = await m_UsersDataStore.GetUserAsync(target.Type, target.Id);
            user.Permissions.Add(permissionToUpdate);
            await m_UsersDataStore.SaveChangesAsync();
            await Context.Actor.PrintMessageAsync($"Added \"{permissionToUpdate}\" to \"{target.DisplayName}\".", Color.DarkGreen);
        }
    }
}