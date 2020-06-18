using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandAdmin : IRocketCommand
    {
        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Both;
            }
        }

        public string Name
        {
            get { return "admin"; }
        }

        public string Help
        {
            get { return "Give a player admin privileges";}
        }

        public string Syntax
        {
            get { return ""; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.admin" };
            }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!R.Settings.Instance.WebPermissions.Enabled)
            {
                UnturnedPlayer player = command.GetUnturnedPlayerParameter(0);
                if (player == null)
                {
                    UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller,this);
                }

                if (!player.IsAdmin)
                {
                    UnturnedChat.Say(caller, "Successfully admined "+player.CharacterName);
                    player.Admin(true);
                }
            }   
        }
    }
}