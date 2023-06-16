using System;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using SDG.Unturned;

namespace OpenMod.Unturned.Users
{
    [OpenModInternal]
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

            ChatManager.onCheckPermissions -= OnCheckCommandPermissions;
            m_Subscribed = false;
        }

        protected virtual void OnCheckCommandPermissions(SteamPlayer steamPlayer, string text, ref bool shouldExecuteCommand, ref bool shouldList)
        {
            if (!text.StartsWith("/", StringComparison.Ordinal))
            {
                return;
            }

            shouldList = false;
            shouldExecuteCommand = false;

            var args = ArgumentsParser.ParseArguments(text[1..]);
            if (args.Length == 0)
            {
                return;
            }

            AsyncHelper.Schedule("Player command execution", async () =>
            {
                var player = await m_UserManager.FindUserAsync(KnownActorTypes.Player, steamPlayer.playerID.steamID.ToString(), UserSearchMode.FindById);
                if (player == null)
                {
                    return;
                }

                await m_CommandExecutor.ExecuteAsync(player, args, string.Empty);
            });
        }

        public void Dispose()
        {
            Unsubscribe();
        }
    }
}