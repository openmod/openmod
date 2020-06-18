using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;

namespace Rocket.Unturned.Commands
{
    public class CommandHeal : IRocketCommand
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
            get { return "heal"; }
        }

        public string Help
        {
            get { return "Heals yourself or somebody else";}
        }

        public string Syntax
        {
            get { return "[player]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "rocket.heal" };
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer && command.Length != 1)
            {
                UnturnedPlayer player = (UnturnedPlayer)caller;
                player.Heal(100);
                player.Bleeding = false;
                player.Broken = false;
                player.Infection = 0;
                player.Hunger = 0;
                player.Thirst = 0;
                UnturnedChat.Say(player, U.Translate("command_heal_success"));
            }
            else
            {
                UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                if (otherPlayer != null)
                {
                    otherPlayer.Heal(100);
                    otherPlayer.Bleeding = false;
                    otherPlayer.Broken = false;
                    otherPlayer.Infection = 0;
                    otherPlayer.Hunger = 0;
                    otherPlayer.Thirst = 0;
                    UnturnedChat.Say(caller, U.Translate("command_heal_success_me", otherPlayer.CharacterName));
                    
                    if(caller != null)
                        UnturnedChat.Say(otherPlayer, U.Translate("command_heal_success_other", caller.DisplayName));
                }
                else
                {
                    UnturnedChat.Say(caller, U.Translate("command_generic_target_player_not_found"));
                    throw new WrongUsageOfCommandException(caller, this);
                }
            }
        }
    }
}