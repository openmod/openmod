using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.API.Commands
{
    [Service]
    public interface ICommandExecutor
    {
        Task<ICommandContext> ExecuteAsync(ICommandActor actor, string[] args, string prefix);
    }
}