using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public interface IItem
    {
        /// <value>
        /// The unique instance ID of the item.
        /// </value>
        string ItemInstanceId { get; }

        /// <value>
        /// The asset of the item.
        /// </value>
        IItemAsset Asset { get; }

        /// <value>
        /// The state of the item.
        /// </value>
        IItemState State { get; }

        /// <value>
        /// Sets the quality of the item.
        /// </value>
        /// <param name="quality">The quality to set to.</param>
        /// <exception cref="ArgumentException">Thrown when the quality is invalid, e.g. non-positive or larger than the maxmimum quality allowed by the game.</exception>
        Task SetItemQualityAsync(double quality);

        /// <value>
        /// Sets the amount of the item.
        /// </value>
        /// <param name="amount">The amount to set to.</param>
        /// <exception cref="ArgumentException">Thrown when the amount is invalid, e.g. non-positive or larger than the maxmimum amount allowed by the game.</exception>
        Task SetItemAmountAsync(double amount);
    }
}