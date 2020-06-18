using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.RCON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Commands
{
    class CommandRWho : IRocketCommand
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
            get { return "Returns a list of clients connected to RCON."; }
        }

        public string Name
        {
            get { return "rwho"; }
        }

        public List<string> Permissions
        {
            get { return new List<string>() { "rocket.rwho" }; }
        }

        public string Syntax
        {
            get { return ""; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            for (int i = 0; i < RCONServer.Clients.Count; i++)
            {
                RCONConnection client = RCONServer.Clients[i];
                int timeTotal = (int)((DateTime.Now - client.ConnectedTime).TotalSeconds);
                string connectedTimeFormat = "";

                // Format days, hours minutes and seconds since the client connected to RCON.
                if (timeTotal >= (60 * 60 * 24))
                {
                    connectedTimeFormat = ((int)(timeTotal / (60 * 60 * 24))).ToString() + "d ";
                }
                if (timeTotal >= (60 * 60))
                {
                    connectedTimeFormat += ((int)((timeTotal / (60 * 60)) % 24)).ToString() + "h ";
                }
                if (timeTotal >= 60)
                {
                    connectedTimeFormat += ((int)((timeTotal / 60) % 60)).ToString() + "m ";
                }
                connectedTimeFormat += ((int)(timeTotal % 60)).ToString() + "s";
                Logger.Log(R.Translate("command_rwho_line", i + 1, client.InstanceID, client.Authenticated, client.Address, client.ConnectedTime.ToString(), connectedTimeFormat));
            }
        }
    }
}
