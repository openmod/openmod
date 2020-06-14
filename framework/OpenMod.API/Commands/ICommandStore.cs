using System.Collections.Generic;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandStore
    {
        IReadOnlyCollection<ICommandRegistration> Commands { get; }
    }
}