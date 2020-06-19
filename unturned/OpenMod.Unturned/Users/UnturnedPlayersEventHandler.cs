using System;
using OpenMod.API.Commands;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    public class UnturnedCommandHandler : IDisposable
    {
        private readonly ICommandExecutor m_CommandExecutor;
        private readonly IUserManager m_UserManager;
        private bool m_Subscribed;

        public UnturnedCommandHandler(
            ICommandExecutor commandExecutor,
            IUserManager userManager)
        {
            m_CommandExecutor = commandExecutor;
            m_UserManager = userManager;
        }

        public virtual void Subscribe()
        {
            if (m_Subscribed)
            {
                return;
            }

            ChatManager.onCheckPermissions += OnCheckCommandPermissions;
            m_Subscribed = true;
        }

        public virtual void Unsubscribe()
        {
            if (!m_Subscribed)
            {
                return;
            }

            // ReSharper disable once DelegateSubtraction
            ChatManager.onCheckPermissions -= OnCheckCommandPermissions;
            m_Subscribed = false;
        }

        protected virtual void OnCheckCommandPermissions(SteamPlayer player, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (!shouldExecuteCommand || !text.StartsWith("/"))
            {
                return;
            }

            shouldExecuteCommand = false;
            shouldList = false;

            AsyncHelper.Schedule("Player command execution", async () =>
            {
                var user = await m_UserManager.FindUserAsync(KnownActorTypes.Player, player.playerID.steamID.ToString(), UserSearchMode.Id);
                if (user == null)
                {
                    return;
                }

                await m_CommandExecutor.ExecuteAsync(user, text.Split(' '), string.Empty);
            });
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}