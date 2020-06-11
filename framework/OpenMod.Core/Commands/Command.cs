using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    [IgnoreCommand]
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