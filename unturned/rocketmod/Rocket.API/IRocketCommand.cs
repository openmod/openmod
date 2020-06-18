using System.Collections.Generic;

namespace Rocket.API
{
    public enum AllowedCaller { Console, Player, Both }

    public interface IRocketCommand
    {
        AllowedCaller AllowedCaller { get; }
        string Name { get; }
        string Help { get; }
        string Syntax { get; }
        List<string> Aliases { get; }
        List<string> Permissions { get; }

        void Execute(IRocketPlayer caller, string[] command); 
    }
}
