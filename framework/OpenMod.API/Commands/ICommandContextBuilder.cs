using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandContextBuilder
    {
        ICommandContext CreateContext(ICommandActor actor, string[] args, string prefix, ICollection<ICommandRegistration> commandRegistrations);
    }
}