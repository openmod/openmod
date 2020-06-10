using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Permissions;

namespace OpenMod.API.Commands
{
    public interface ICommandActor : IPermissionActor
    {
        Task PrintMessageAsync(Color color, string message);
    }
}