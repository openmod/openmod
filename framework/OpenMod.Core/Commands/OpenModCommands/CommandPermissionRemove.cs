using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("remove")]
    [CommandAlias("-")]
    [CommandAlias("r")]
    [CommandSyntax("<[p]layer/[g]roup> [target] [permission]")]
    [CommandParent(typeof(CommandPermission))]
    public class CommandPermissionRemove : CommandPermissionAction
    {
        private readonly IUsersDataStore m_UsersDataStore;

        public CommandPermissionRemove(IServiceProvider serviceProvider, IPermissionGroupStore permissionGroupStore, IUsersDataStore usersDataStore) : base(serviceProvider, permissionGroupStore, usersDataStore)
        {
            m_UsersDataStore = usersDataStore;
        }

        protected override async Task ExecuteUpdateAsync(IPermissionActor target, string permissionToUpdate)
        {
            var user = await m_UsersDataStore.GetUserAsync(target.Type, target.Id);
            if (user.Permissions.Remove(permissionToUpdate))
            {
                await m_UsersDataStore.SaveChangesAsync();
                await Context.Actor.PrintMessageAsync($"Removed \"{permissionToUpdate}\" from \"{target.DisplayName}\".", Color.DarkGreen);
            }
            else
            {
                await Context.Actor.PrintMessageAsync($"\"{target.DisplayName}\" does not have permission \"{permissionToUpdate}\".", Color.DarkRed);
            }
        }
    }
}