using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    public abstract class Command : CommandBase
    {
        protected Command(ICurrentCommandContextAccessor contextAccessor) : base(contextAccessor)
        {
        }

        public sealed override Task ExecuteAsync()
        {
            return OnExecuteAsync();
        }

        protected abstract Task OnExecuteAsync();
    }
}