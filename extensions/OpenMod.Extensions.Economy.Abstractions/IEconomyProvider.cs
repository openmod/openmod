using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMod.API.Ioc;

namespace OpenMod.Extensions.Economy.Abstractions
{
    [Service]
    public interface IEconomyProvider
    {
        /// <value>
        /// Name of the currency, e.g. "Dollar". Cannot be null.
        /// </value>
        [NotNull]
        string CurrencyName { get; }

        /// <value>
        /// Symbol of the currency, e.g. "$". Cannot be null.
        /// </value>
        [NotNull]
        string CurrencySymbol { get; }

        /// <summary>
        /// Get's the balance of an user.
        /// </summary>
        Task<decimal> GetBalanceAsync(string ownerId, string ownerType);

        /// <summary>
        /// Updates balance for a user.
        /// </summary>
        /// <exception cref="NotEnoughBalanceException">if the balance would become negative</exception>
        Task<decimal> UpdateBalanceAsync(string ownerId, string ownerType, decimal changeAmount, string reason);

        /// <summary>
        ///   Sets balance for a user.
        /// </summary>
        /// <exception cref="NotSupportedException">may be thrown if setting to negative balance and it's not supported</exception>
        Task SetBalanceAsync(string ownerId, string ownerType, decimal balance);
    }
}