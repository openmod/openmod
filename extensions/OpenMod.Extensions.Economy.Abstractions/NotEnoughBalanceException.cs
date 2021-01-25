using OpenMod.API.Commands;

namespace OpenMod.Extensions.Economy.Abstractions
{
    /// <summary>
    /// The exception that is thrown when an actor does not have enough balance.
    /// </summary>
    public class NotEnoughBalanceException : UserFriendlyException
    {
        public decimal? Balance;

        public NotEnoughBalanceException(string message) : base(message)
        {
        }

        public NotEnoughBalanceException(string message, decimal balance) : base(message)
        {
            Balance = balance;
        }
    }
}