using OpenMod.API.Console;
using OpenMod.API.Ioc;

namespace OpenMod.Core.Console
{
    [Service]
    public interface IConsoleActorAccessor
    {
        IConsoleActor Actor { get; }
    }
}