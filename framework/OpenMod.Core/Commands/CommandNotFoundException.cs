using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandNotFoundException : UserFriendlyException
    {
        public CommandNotFoundException(string message) : base(message)
        {
        }
    }
}