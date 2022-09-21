using Cysharp.Threading.Tasks;
using OpenMod.API;
using OpenMod.Core.Ioc;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Steamworks;
using System;

namespace OpenMod.Unturned.Commands
{
    [DontAutoRegister]
    [OpenModInternal]
    public class UnturnedBuiltinCommand : UnturnedCommand
    {
        private readonly UnturnedCommandRegistration m_CommandRegistration;

        public UnturnedBuiltinCommand(
            IServiceProvider serviceProvider,
            UnturnedCommandRegistration commandRegistration) : base(serviceProvider)
        {
            m_CommandRegistration = commandRegistration;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var cmd = m_CommandRegistration.Cmd;

            var id = Context.Actor.Type switch
            {
                KnownActorTypes.Player => ((UnturnedUser)Context.Actor).SteamId,
                KnownActorTypes.Console => CSteamID.Nil,
                KnownActorTypes.Rcon => CSteamID.Nil,
                _ => throw new NotSupportedException($"Cannot execute unturned commands from actor: {Context.Actor.GetType()}.")
            };

            // unturned builtin commands must run on main thread
            await UniTask.SwitchToMainThread();
            var argsLine = string.Join(" ", Context.Parameters);
            cmd.check(id, m_CommandRegistration.Name, argsLine);
        }
    }
}