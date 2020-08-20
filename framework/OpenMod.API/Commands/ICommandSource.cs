using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenMod.API.Commands
{
    public interface ICommandSource
    {
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();
    }
}