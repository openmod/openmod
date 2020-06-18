using Rocket.API;
using Rocket.Core.Logging;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;

namespace Rocket.Unturned.Commands
{
    public class CommandGod : IRocketCommand
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
            get { return "god"; }
        }

        public string Help
        {
            get { return "Cause you ain't givin a shit";}
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
            get
            {
                return new List<string>() { "rocket.god" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (player.Features.GodMode)
            {
                Logger.Log(U.Translate("command_god_disable_console", player.CharacterName));
                UnturnedChat.Say(caller, U.Translate("command_god_disable_private"));
                player.Features.GodMode = false;
            }
            else
            {
                Logger.Log(U.Translate("command_god_enable_console", player.CharacterName));
                UnturnedChat.Say(caller, U.Translate("command_god_enable_private"));
                player.Features.GodMode = true;
            }
        }
    }
}