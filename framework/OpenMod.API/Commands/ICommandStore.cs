using System.Collections.Generic;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandStore
    {
        Task<IReadOnlyCollection<ICommandRegistration>> GetCommandsAsync();

        Task InvalidateAsync();
    }
}