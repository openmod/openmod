using System.Drawing;
using System.Threading.Tasks;
using OpenMod.API.Commands;
using OpenMod.Core.Ioc;

namespace OpenMod.Core.Commands
{
    [DontAutoRegister]
    public abstract class CommandBase: ICommand
    {
        public ICommandContext Context { get; }

        protected CommandBase(ICurrentCommandContextAccessor contextAccessor)
        {
            Context = contextAccessor.Context;
        }

        public abstract Task ExecuteAsync();

        public virtual Task PrintAsync(string message)
        {
            return Context.Actor.PrintMessageAsync(message, Color.White);
        }

        public virtual Task PrintAsync(string message, Color color)
        {
            return Context.Actor.PrintMessageAsync(message, color);
        }
    }
}