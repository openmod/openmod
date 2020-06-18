using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;
using Rocket.Core.RCON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Commands
{
    class CommandRKick : IRocketCommand
    {
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Console; }
        }

        public string Help
        {
            get { return "Kicks a client off of RCON."; }
        }

        public string Name
        {
            get { return "rkick"; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.rkick" }; }
        }

        public string Syntax
        {
            get { return "<ConnectionID>"; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            int? instance = command.GetInt32Parameter(0);
            if (command.Length == 0 || command.Length > 1 || instance == null)
            {
                Logger.Log(R.Translate("command_rkick_help"));
                return;
            }
            foreach (RCONConnection client in RCONServer.Clients)
            {
                if (client.InstanceID == (int)instance)
                {
                    Logger.Log(R.Translate("command_rkick_kicked", instance.ToString(), client.Address));
                    client.Close();
                    return;
                }
            }
            Logger.Log(R.Translate("command_rkick_notfound", instance.ToString()));
        }
    }
}
