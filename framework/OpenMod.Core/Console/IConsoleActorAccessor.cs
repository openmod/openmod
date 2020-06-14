using OpenMod.API.Commands;
using OpenMod.API.Ioc;

namespace OpenMod.Core.Console
{
    [Service]
    public interface IConsoleActorAccessor
    {
        ICommandActor Actor { get; }
    }
}