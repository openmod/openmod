using OpenMod.API.Ioc;

namespace OpenMod.Unturned.Console
{
    [Service]
    public interface IConsoleActorAccessor
    {
        ConsoleActor Actor { get; }
    }
}