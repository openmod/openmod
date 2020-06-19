using System;
using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;
using OpenMod.API.Users;
using OpenMod.Core.Permissions;

namespace OpenMod.Core.Commands.OpenModCommands
{
    public abstract class CommandPermissionAction : Command
    {
        private readonly IPermissionGroupStore m_PermissionGroupStore;
        private readonly IUserDataStore m_UserDataStore;

        protected CommandPermissionAction(IServiceProvider serviceProvider,
            IPermissionGroupStore permissionGroupStore,
            IUserDataStore userDataStore) : base(serviceProvider)
        {
            m_PermissionGroupStore = permissionGroupStore;
            m_UserDataStore = userDataStore;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 3)
            {
                throw new CommandWrongUsageException(Context);
            }

            IPermissionActor target;
            string permission;

            var actorType = Context.Parameters[0].ToLower();
            var targetName = Context.Parameters[1];
            var permissionToUpdate = Context.Parameters[2];

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
                    var user = await m_UserDataStore.GetUserDataAsync(actorType, id);

                    if (user == null)
                    {
                        // todo: localizable
                        throw new UserFriendlyException($"User not found: {id}");
                    }

                    target = (UserDataPermissionActor) user;
                    break;

                default:
                    throw new CommandWrongUsageException(Context);
            }

            if (await CheckPermissionAsync(permission) != PermissionGrantResult.Grant)
            {
                throw new NotEnoughPermissionException(Context, permission);
            }

            await ExecuteUpdateAsync(target, permissionToUpdate);
        }

        protected abstract Task ExecuteUpdateAsync(IPermissionActor target, string param);
    }
}