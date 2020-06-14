using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Localization;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    [Command("permission", Priority = Priority.Lowest)]
    [CommandAlias("p")]
    [CommandDescription("Manage permissions")]
    public class CommandPermission : Command
    {
        public CommandPermission(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override Task OnExecuteAsync()
        {
            throw new CommandWrongUsageException(Context);
        }
    }

    public abstract class CommandPermissionUpdate : Command
    {
        private readonly IPermissionGroupStore m_PermissionGroupStore;
        private readonly IUsersDataStore m_UsersDataStore;

        protected CommandPermissionUpdate(IServiceProvider serviceProvider, 
            IPermissionGroupStore permissionGroupStore,
            IUsersDataStore usersDataStore
            ) : base(serviceProvider)
        {
            m_PermissionGroupStore = permissionGroupStore;
            m_UsersDataStore = usersDataStore;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 3)
            {
                throw new CommandWrongUsageException(Context);
            }

            IPermissionActor target;
            string permission;

            string actorType = Context.Parameters[0].ToLower();
            string targetName = Context.Parameters[1];
            string permissionToUpdate = Context.Parameters[2];

            switch (actorType)
            {
                case "g":
                case "group":
                    permission = "Manage.Groups." + targetName;
                    target = await m_PermissionGroupStore.GetGroupAsync(targetName);
                    
                    if (target == null)
                    {
                        await Context.Actor.PrintMessageAsync($"Group \"{targetName}\" was not found.", Color.Red);
                        return;
                    }

                    break;

                case "p":
                case "player":
                    permission = "Manage.Players";
                    var id = await Context.Parameters.GetAsync<string>(1);
                    var user = await m_UsersDataStore.GetUserAsync(actorType, id);
                    
                    if (user == null)
                    {
                        // todo: localizable
                        throw new UserFriendlyException($"User not found: {id}"); 
                    }

                    target = (UserDataPermissionActor)user;
                    break;

                default:
                    throw new CommandWrongUsageException(Context);
            }

            if (await CheckPermissionAsync(permission) != PermissionGrantResult.Grant)
            {
                throw new NotEnoughPermissionException(Context, permission);
            }

            //await UpdatePermissionAsync(context.User, configPermissions, target, permissionToUpdate);
        }
    }
}