using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API
{
    public class WrongUsageOfCommandException : Exception
    {
        private IRocketCommand command;
        private IRocketPlayer player;
        public WrongUsageOfCommandException(IRocketPlayer player, IRocketCommand command)
        {
            this.command = command;
            this.player = player;
        }
        public override string Message
        {
            get
            {
               return "The player " + player.DisplayName + " did not correctly use the command " + command.Name;
            }
        }
    }
}
