using System;
using System.Threading.Tasks;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Economy.Abstractions
{
    [Service]
    public interface IEconomyProvider
    {
        /// <summary>
        /// Gets the name of the currency, e.g. "Dollar".
        /// </summary>
        string CurrencyName { get; }

        /// <summary>
        /// Gets the symbol of the currency, e.g. "$".
        /// </summary>
        string CurrencySymbol { get; }

        /// <summary>
        /// Gets the balance of an user.
        /// </summary>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <param name="ownerType">The actor type of the owner.</param>
        Task<decimal> GetBalanceAsync(string ownerId, string ownerType);

        /// <summary>
        /// Creates a transaction and updates the balance of a user.
        /// </summary>
        /// <exception cref="NotEnoughBalanceException">if the balance would become negative</exception>
        /// <param name="ownerId">The ID of the owner.</param>
        /// <param name="ownerType">The actor type of the owner.</param>
        /// <param name="changeAmount">The amount to add or remove.</param>
        /// <param name="reason">The optional reason for this transaction.</param>
        Task<decimal> UpdateBalanceAsync(string ownerId, string ownerType, decimal changeAmount, string? reason);

        /// <summary>
        ///   Sets balance for a user.
        /// </summary>
        /// <exception cref="NotSupportedException">may be thrown if setting to negative balance and it's not supported</exception>#
        /// <param name="ownerId">The ID of the owner.</param>
        /// <param name="ownerType">The actor type of the owner.</param>
        /// <param name="balance">The balance to set to.</param>
        Task SetBalanceAsync(string ownerId, string ownerType, decimal balance);
    }
}