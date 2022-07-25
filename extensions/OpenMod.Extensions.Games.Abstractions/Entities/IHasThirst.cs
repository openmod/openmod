using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IHasThirst
    {
        /// <summary>
        /// Gets the max thirst the entity can have.
        /// </summary>
        double MaxThirst { get; }

        /// <summary>
        /// Gets the thirst of the entity.
        /// </summary>
        double Thirst { get; }

        /// <summary>
        /// Sets the thirst of the entity.
        /// </summary>
        /// <param name="thirst">The thirst to set to.</param>
        /// <exception cref="ArgumentException">Thrown when <c>thirst</c> is invalid, e.g. when it's greater than <see cref="MaxThirst"/> or less than zero.</exception>
        Task SetThirstAsync(double thirst);
    }
}
