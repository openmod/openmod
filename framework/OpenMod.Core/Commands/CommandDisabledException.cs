using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandDisabledException : UserFriendlyException
    {
        public CommandDisabledException(string message) : base(message)
        {
        }
    }
}