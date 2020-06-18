using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandCompass : IRocketCommand
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
            get { return "compass"; }
        }

        public string Help
        {
            get { return "Shows the direction you are facing"; }
        }

        public string Syntax
        {
            get { return "[direction]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.compass" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            float currentDirection = player.Rotation;

            if (command.Length == 1)
            {
                switch (command[0])
                {
                    case "north":
                        currentDirection = 0;
                        break;
                    case "east":
                        currentDirection = 90;
                        break;
                    case "south":
                        currentDirection = 180;
                        break;
                    case "west":
                        currentDirection = 270;
                        break;
                    default:
                        UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                        throw new WrongUsageOfCommandException(caller, this);
                }
                player.Teleport(player.Position, currentDirection);
            }
            
            string directionName = "Unknown";

            if (currentDirection > 30 && currentDirection < 60)
            {
                directionName = U.Translate("command_compass_northeast");
            }
            else if (currentDirection > 60 && currentDirection < 120)
            {
                directionName = U.Translate("command_compass_east");
            }
            else if (currentDirection > 120 && currentDirection < 150)
            {
                directionName = U.Translate("command_compass_southeast");
            }
            else if (currentDirection > 150 && currentDirection < 210)
            {
                directionName = U.Translate("command_compass_south");
            }
            else if (currentDirection > 210 && currentDirection < 240)
            {
                directionName = U.Translate("command_compass_southwest");
            }
            else if (currentDirection > 240 && currentDirection < 300)
            {
                directionName = U.Translate("command_compass_west");
            }
            else if (currentDirection > 300 && currentDirection < 330)
            {
                directionName = U.Translate("command_compass_northwest");
            }
            else if (currentDirection > 330 || currentDirection < 30)
            {
                directionName = U.Translate("command_compass_north");
            }

            UnturnedChat.Say(caller, U.Translate("command_compass_facing_private", directionName));
        }
    }
}