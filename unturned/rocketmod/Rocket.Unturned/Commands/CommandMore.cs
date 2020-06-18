using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Unturned.Commands
{
    public class CommandMore : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public string Help
        {
            get { return "Gives more of an item that you have in your hands."; }
        }

        public string Name
        {
            get{ return "more"; }
        }

        public List<string> Permissions
        {
            get{ return new List<string>() { "rocket.more" }; }
        }

        public string Syntax
        {
            get{ return "<amount>"; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            byte? amount = command.GetByteParameter(0);
            if (command.Length == 0 || command.Length > 1 || amount == null || amount == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_more_usage"));
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;
            ushort itemId = player.Player.equipment.itemID;
            if (itemId == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_more_dequipped"));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_more_give", amount, itemId));
                player.GiveItem(itemId, (byte)amount);
            }
        }
    }
}
