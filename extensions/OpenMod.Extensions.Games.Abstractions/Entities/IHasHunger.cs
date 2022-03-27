using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IHasHunger
    {
        /// <summary>
        /// Gets the max hunger the entity can have.
        /// </summary>
        double MaxHunger { get; }

        /// <summary>
        /// Gets the hunger of the entity.
        /// </summary>
        double Hunger { get; }

        /// <summary>
        /// Sets the hunger of the entity.
        /// </summary>
        /// <param name="hunger">The hunger to set to.</param>
        /// <exception cref="ArgumentException">Thrown when <c>hunger</c> is invalid, e.g. when it's greater than <see cref="MaxHunger"/> or less than zero.</exception>
        Task SetHungerAsync(double hunger);
    }
}
