using OpenMod.API.Commands;

namespace OpenMod.Extensions.Economy.Abstractions
{
    public class NotEnoughBalanceException : UserFriendlyException
    {
        public decimal? Balance = null;

        public NotEnoughBalanceException(string message) : base(message)
        {
        }

        public NotEnoughBalanceException(string message, decimal balance) : base(message)
        {
            Balance = balance;
        }
    }
}