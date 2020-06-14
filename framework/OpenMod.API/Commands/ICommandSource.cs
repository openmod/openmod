using System.Collections.Generic;

namespace OpenMod.API.Commands
{
    public interface ICommandSource
    {
        ICollection<ICommandRegistration> Commands { get; }
    }
}