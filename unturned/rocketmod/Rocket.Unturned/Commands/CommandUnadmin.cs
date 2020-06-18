using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandUnadmin : IRocketCommand
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
            get { return "unadmin"; }
        }

        public string Help
        {
            get { return "Revoke a players admin privileges"; }
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
            get { return new List<string>() { "rocket.unadmin" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (!R.Settings.Instance.WebPermissions.Enabled)
            {
                UnturnedPlayer player = command.GetUnturnedPlayerParameter(0);
                if (player == null)
                {
                    UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                    throw new WrongUsageOfCommandException(caller, this);
                }

                if (player.IsAdmin)
                {
                    UnturnedChat.Say(caller, "Successfully unadmined " + player.CharacterName);
                    player.Admin(false);
                }
            }
        }
    }
}