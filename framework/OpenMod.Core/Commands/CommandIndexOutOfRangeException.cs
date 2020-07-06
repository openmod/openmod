using OpenMod.API.Commands;

namespace OpenMod.Core.Commands
{
    public class CommandIndexOutOfRangeException : UserFriendlyException
    {
        public int Index { get; }
        public int Length { get; }

        public CommandIndexOutOfRangeException(string message, int index, int length) : base(message)
        {
            Index = index;
            Length = length;
        }
    }
}