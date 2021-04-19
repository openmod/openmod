using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Items
{
    /// <summary>
    /// Represents an item.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Gets the unique instance ID of the item.
        /// </summary>
        string ItemInstanceId { get; }

        /// <summary>
        /// Gets the asset of the item.
        /// </summary>
        IItemAsset Asset { get; }

        /// <summary>
        /// Gets the state of the item.
        /// </summary>
        IItemState State { get; }

        /// <summary>
        /// Sets the quality of the item.
        /// </summary>
        /// <param name="quality">The quality to set to.</param>
        /// <exception cref="ArgumentException">Thrown when the quality is invalid, e.g. non-positive or larger than the maximum quality allowed by the game.</exception>
        Task SetItemQualityAsync(double quality);

        /// <summary>
        /// Sets the amount of the item.
        /// </summary>
        /// <param name="amount">The amount to set to.</param>
        /// <exception cref="ArgumentException">Thrown when the amount is invalid, e.g. non-positive or larger than the maximum amount allowed by the game.</exception>
        Task SetItemAmountAsync(double amount);

        /// <summary>
        /// Sets the durability of the item.
        /// </summary>
        /// <param name="durability">The amount to set to.</param>
        /// <exception cref="ArgumentException">Thrown when the durability is invalid, e.g. non-positive or larger than the maximum durability allowed by the game.</exception>
        Task SetItemDurabilityAsync(double durability);
    }
}