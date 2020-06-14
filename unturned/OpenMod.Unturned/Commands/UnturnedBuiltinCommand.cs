using System;
using Cysharp.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.API.Console;
using OpenMod.Core.Commands;
using OpenMod.Core.Ioc;
using OpenMod.Unturned.API;
using Steamworks;

namespace OpenMod.Unturned.Commands
{
    [DontAutoRegister]
    public class UnturnedBuiltinCommand : UnturnedCommand
    {
        private readonly UnturnedCommandRegistration m_CommandRegistration;

        public UnturnedBuiltinCommand(
            ICurrentCommandContextAccessor contextAccessor,
            UnturnedCommandRegistration commandRegistration) : base(contextAccessor)
        {
            m_CommandRegistration = commandRegistration;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var cmd = m_CommandRegistration.Cmd;
            CSteamID id;

            switch (Context.Actor)
            {
                case IUnturnedPlayerActor unturnedPlayerActor:
                    id = unturnedPlayerActor.SteamId;
                    break;

                case IConsoleActor _:
                    id = CSteamID.Nil;
                    break;

                default:
                    throw new NotSupportedException($"Can not execute unturned commands from actor: {Context.Actor.GetType()}");
            }

            // unturned builtin commands must run on main thread
            await UniTask.SwitchToMainThread();
            cmd.check(id, m_CommandRegistration.Name, Context.GetCommandLine());
        }
    }
}