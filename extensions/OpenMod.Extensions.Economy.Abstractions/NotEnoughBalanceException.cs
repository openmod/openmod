using OpenMod.API.Commands;

namespace OpenMod.Extensions.Economy.Abstractions
{
    public class NotEnoughBalanceException : UserFriendlyException
    {
        public NotEnoughBalanceException(string message) : base(message)
        {
        }
    }
}