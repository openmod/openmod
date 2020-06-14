using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandContextBuilder
    {
        ICommandContext CreateContext(ICommandActor actor, string[] args, string prefix, IEnumerable<ICommandRegistration> commandRegistrations);
    }
}