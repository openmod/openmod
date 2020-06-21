using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using OpenMod.Core.Commands;
using OpenMod.Core.Ioc;
using OpenMod.Core.Users;
using OpenMod.Unturned.Users;
using Steamworks;

namespace OpenMod.Unturned.Commands
{
    [DontAutoRegister]
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
            CSteamID id;

            switch (Context.Actor.Type)
            {
                case KnownActorTypes.Player:
                    id = ((UnturnedUser) Context.Actor).SteamId;
                    break;

                case KnownActorTypes.Console:
                    id = CSteamID.Nil;
                    break;

                default:
                    throw new NotSupportedException($"Can not execute unturned commands from actor: {Context.Actor.GetType()}");
            }

            // unturned builtin commands must run on main thread
            await UniTask.SwitchToMainThread();
            cmd.check(id, m_CommandRegistration.Name, string.Join(" ",Context.Parameters.Skip(1)));
        }
    }
}