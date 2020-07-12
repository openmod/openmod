using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;
using OpenMod.Core.Users;

namespace OpenMod.Core.Commands.OpenModCommands
{
    public abstract class CommandPermissionAction : Command
    {
        private readonly IPermissionRoleStore m_PermissionRoleStore;
        private readonly IUserDataStore m_UserDataStore;
        private readonly IUserManager m_UserManager;

        protected CommandPermissionAction(IServiceProvider serviceProvider,
            IPermissionRoleStore permissionRoleStore,
            IUserDataStore userDataStore,
            IUserManager userManager) : base(serviceProvider)
        {
            m_PermissionRoleStore = permissionRoleStore;
            m_UserDataStore = userDataStore;
            m_UserManager = userManager;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 3)
            {
                throw new CommandWrongUsageException(Context);
            }

            IPermissionActor target;

            var actorType = Context.Parameters[0].ToLower();
            var permission = "Manage." + actorType;
            var targetName = Context.Parameters[1];
            var permissionToUpdate = Context.Parameters[2];

            switch (actorType)
            {
                case "r":
                case "role":
                    permission = "Manage.Roles." + targetName;
                    target = await m_PermissionRoleStore.GetRoleAsync(targetName);

                    if (target == null)
                    {
                        await Context.Actor.PrintMessageAsync($"Role \"{targetName}\" was not found.", Color.Red);
                        return;
                    }

                    break;

                case "p":
                case "player":
                    permission = "Manage.Players";
                    actorType = KnownActorTypes.Player;
                    goto default;

                default:
                    var idOrName = await Context.Parameters.GetAsync<string>(1);
                    var user = await m_UserManager.FindUserAsync(actorType, idOrName, UserSearchMode.NameOrId);

                    if (user == null)
                    {
                        // todo: make localizable
                        throw new UserFriendlyException($"Player not found: {idOrName}");
                    }

                    var userData = await m_UserDataStore.GetUserDataAsync(user.Id, actorType);
                    target = (UserDataPermissionActor)userData;
                    break;
            }

            if (await CheckPermissionAsync(permission) != PermissionGrantResult.Grant)
            {
                throw new NotEnoughPermissionException(Context, permission);
            }

            await ExecuteUpdateAsync(target, permissionToUpdate);
        }

        protected abstract Task ExecuteUpdateAsync(IPermissionActor target, string roleId);
    }
}