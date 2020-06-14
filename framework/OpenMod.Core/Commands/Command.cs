using System;
using System.Threading.Tasks;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    public abstract class Command : CommandBase
    {
        protected Command(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public sealed override Task ExecuteAsync()
        {
            return OnExecuteAsync();
        }

        protected abstract Task OnExecuteAsync();
    }
}