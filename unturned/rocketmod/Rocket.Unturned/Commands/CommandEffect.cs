using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.API.Extensions;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Name
        {
            get { return "effect"; }
        }

        public string Help
        {
            get { return "Triggers an effect at your position";}
        }

        public string Syntax
        {
            get { return "<id>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.effect" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            ushort? id = command.GetUInt16Parameter(0);
            if (id == null)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            player.TriggerEffect(id.Value);
        }
    }
}