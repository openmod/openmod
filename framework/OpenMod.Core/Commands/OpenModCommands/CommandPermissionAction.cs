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
        private readonly ICommandPermissionBuilder m_PermissionBuilder;
        private readonly IPermissionChecker m_PermissionChecker;
        private readonly IUserDataStore m_UserDataStore;
        private readonly IPermissionRegistry m_PermissionRegistry;
        private readonly IUserManager m_UserManager;

        protected CommandPermissionAction(IServiceProvider serviceProvider,
            IPermissionRoleStore permissionRoleStore,
            ICommandPermissionBuilder permissionBuilder,
            IPermissionChecker permissionChecker,
            IUserDataStore userDataStore,
            IUserManager userManager, 
            IPermissionRegistry permissionRegistry) : base(serviceProvider)
        {
            m_PermissionRoleStore = permissionRoleStore;
            m_PermissionBuilder = permissionBuilder;
            m_PermissionChecker = permissionChecker;
            m_UserDataStore = userDataStore;
            m_UserManager = userManager;
            m_PermissionRegistry = permissionRegistry;
        }

        protected override async Task OnExecuteAsync()
        {
            if (Context.Parameters.Length != 3)
            {
                throw new CommandWrongUsageException(Context);
            }

            IPermissionActor? target;

            var actorType = Context.Parameters[0].ToLower();
            var permission = "permissions.manage." + actorType;
            var targetName = Context.Parameters[1];
            var permissionToUpdate = Context.Parameters[2];

            switch (actorType)
            {
                case "r":
                case "role":
                    permission = "permissions.manage.roles." + targetName;
                    target = await m_PermissionRoleStore.GetRoleAsync(targetName);
                    
                    // todo: register on startup instead of here so it can get written to a help file
                    m_PermissionRegistry.RegisterPermission(Context.CommandRegistration!.Component, permission, description: $"Manage role: {targetName}");

                    if (target == null)
                    {
                        await Context.Actor.PrintMessageAsync($"Role \"{targetName}\" was not found.", Color.Red);
                        return;
                    }

                    break;

                case "p":
                case "player":
                    permission = "permissions.manage.players";
                    actorType = KnownActorTypes.Player;
                    goto default;

                default:
                    var idOrName = await Context.Parameters.GetAsync<string>(1);
                    var user = await m_UserManager.FindUserAsync(actorType, idOrName, UserSearchMode.FindByNameOrId);
                    m_PermissionRegistry.RegisterPermission(Context.CommandRegistration!.Component, permission, description: $"Manage actor: {actorType}");

                    if (user == null)
                    {
                        // todo: make localizable
                        throw new UserFriendlyException($"Player not found: {idOrName}");
                    }

                    var userData = await m_UserDataStore.GetUserDataAsync(user.Id, actorType) ?? new UserData();
                    target = (UserDataPermissionActor)userData;
                    break;
            }

            // we call m_PermissionChecker from here so the permission will become OpenMod.Core.manage.players instead of 
            if (await m_PermissionChecker.CheckPermissionAsync(Context.Actor, permission) != PermissionGrantResult.Grant)
            {
                throw new NotEnoughPermissionException(Context, permission);
            }

            await ExecuteUpdateAsync(target, permissionToUpdate);
        }

        protected abstract Task ExecuteUpdateAsync(IPermissionActor target, string roleId);
    }
}