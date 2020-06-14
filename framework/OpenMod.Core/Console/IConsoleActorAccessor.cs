using OpenMod.API.Ioc;

namespace OpenMod.Core.Console
{
    [Service]
    public interface IConsoleActorAccessor
    {
        ConsoleActor Actor { get; }
    }
}