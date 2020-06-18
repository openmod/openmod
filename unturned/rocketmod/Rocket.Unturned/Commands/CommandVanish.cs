using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandVanish : IRocketCommand
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
            get { return "vanish"; }
        }

        public string Help
        {
            get { return "Are we rushing in or are we goin' sneaky beaky like?"; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.vanish" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (player.Features.VanishMode)
            {
                Logger.Log(U.Translate("command_vanish_disable_console", player.CharacterName));
                UnturnedChat.Say(caller, U.Translate("command_vanish_disable_private"));
                player.Features.VanishMode = false;
            }
            else
            {
                Logger.Log(U.Translate("command_vanish_enable_console", player.CharacterName));
                UnturnedChat.Say(caller, U.Translate("command_vanish_enable_private"));
                player.Features.VanishMode = true;
            }
        }
    }
}