using OpenMod.API.Commands;

namespace OpenMod.API.Console
{
    public interface IConsoleActor : ICommandActor
    {
        bool IsGameConsole { get; }
    }
}
