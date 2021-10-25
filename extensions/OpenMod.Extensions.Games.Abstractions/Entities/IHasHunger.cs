using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public interface IHasHunger
    {
        /// <summary>
        /// Checks if hunger is zero.
        /// </summary>

        bool IsHungery { get; }

        /// <summary>
        /// Gets the max hunger the entity can have.
        /// </summary>
        double MaxHunger { get; }

        /// <summary>
        /// Gets the health of the entity.
        /// </summary>
        double Hunger { get; }

        /// <summary>
        /// Sets the hunger of the entity.
        /// </summary>
        /// <param name="hunger">The health to set to.</param>
        /// <exception cref="ArgumentException">Thrown when <c>hunger</c> is invalid, e.g. when it's greater than <see cref="MaxHunger"/> or less than zero.</exception>
        Task SetHungerAsync(double hunger);

        /// <summary>
        /// Sets the hunger to zero of the entity.
        /// </summary>
        Task HungeryAsync();

        /// <summary>
        /// Increase hunger of the entity.
        /// </summary>
        /// <param name="hunger">Amount to be increased.</param>
        /// <exception cref="ArgumentException">Thrown when <c>hunger</c> is invalid, e.g. when it's less than zero.</exception>

        Task HungeryAsync(double hunger);

    }
}
