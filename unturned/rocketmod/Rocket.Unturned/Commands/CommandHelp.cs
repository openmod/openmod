using Rocket.API;
using Rocket.Core;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rocket.Unturned.Commands
{
    public class CommandHelp : IRocketCommand
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
            get { return "help"; }
        }

        public string Help
        {
            get { return "Shows you a specific help";}
        }

        public string Syntax
        {
            get { return "[command]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public List<string> Permissions
        {
                get { return new List<string>() { "rocket.help" }; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("[Vanilla]");
                System.Console.ForegroundColor = ConsoleColor.White;
                Commander.commands.OrderBy(c => c.command).All(c => { System.Console.WriteLine(c.command.ToLower().PadRight(20, ' ') + " " + c.info.Replace(c.command, "").TrimStart().ToLower()); return true; });

                System.Console.WriteLine();

                System.Console.ForegroundColor = ConsoleColor.Cyan;
                System.Console.WriteLine("[Rocket]");
                System.Console.ForegroundColor = ConsoleColor.White;
                R.Commands.Commands.Where(c => c.GetType().Assembly == Assembly.GetExecutingAssembly()).OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });

                System.Console.WriteLine();
                
                foreach (IRocketPlugin plugin in R.Plugins.GetPlugins())
                {
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    System.Console.WriteLine("[" + plugin.GetType().Assembly.GetName().Name + "]");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    R.Commands.Commands.Where(c => c.GetType().Assembly == plugin.GetType().Assembly).OrderBy(c => c.Name).All(c => { System.Console.WriteLine(c.Name.ToLower().PadRight(20, ' ') + " " + c.Syntax.ToLower()); return true; });
                    System.Console.WriteLine();
                }
            }
            else
            {
                IRocketCommand cmd = R.Commands.Commands.Where(c => (String.Compare(c.Name, command[0], true) == 0)).FirstOrDefault();
                if (cmd != null)
                {
                    string commandName = cmd.GetType().Assembly.GetName().Name + " / " + cmd.Name;
                   
                    System.Console.ForegroundColor = ConsoleColor.Cyan;
                    System.Console.WriteLine("[" + commandName + "]");
                    System.Console.ForegroundColor = ConsoleColor.White;
                    System.Console.WriteLine(cmd.Name + "\t\t" + cmd.Syntax);
                    System.Console.WriteLine(cmd.Help);
                }
            }
        }
    }
}