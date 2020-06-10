using System.Collections.Generic;

namespace OpenMod.API.Commands
{
    public interface ICommandSource
    {
        string Id { get; }
        ICollection<ICommandRegistration> Commands { get; }
    }
}