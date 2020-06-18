using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    internal class CommandTphere : IRocketCommand
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
            get { return "tphere"; }
        }

        public string Help
        {
            get { return "Teleports another player to you";}
        }

        public string Syntax
        {
            get { return "<player>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.tphere", "rocket.teleporthere" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
            UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
            if (otherPlayer!=null && otherPlayer != caller)
            {
            	if(otherPlayer.IsInVehicle)
            	{
            		UnturnedChat.Say(caller, U.Translate("command_tphere_vehicle"));
            		return;
            	}
                otherPlayer.Teleport(player);
                Logger.Log(U.Translate("command_tphere_teleport_console", otherPlayer.CharacterName, player.CharacterName));
                UnturnedChat.Say(caller, U.Translate("command_tphere_teleport_from_private", otherPlayer.CharacterName));
                UnturnedChat.Say(otherPlayer, U.Translate("command_tphere_teleport_to_private", player.CharacterName));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
                throw new WrongUsageOfCommandException(caller, this);
            }
        }
    }
}
