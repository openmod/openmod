using OpenMod.API.Ioc;

namespace OpenMod.Standalone
{
    [Service]
    public interface IConsoleActorAccessor
    {
        ConsoleActor Actor { get; }
    }
}