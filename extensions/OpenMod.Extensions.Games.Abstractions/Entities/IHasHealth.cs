using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an object that has health.
    /// </summary>
    public interface IHasHealth
    {
        /// <summary>
        /// Checks if the entity is alive.
        /// </summary>
        bool IsAlive { get; }

        /// <summary>
        /// Gets the max health the entity can have.
        /// </summary>
        double MaxHealth { get; }

        /// <summary>
        /// Gets the health of the entity.
        /// </summary>
        double Health { get; }

        /// <summary>
        /// Sets the health of the entity. Depending on game support, it will not trigger damage effects.
        /// </summary>
        /// <param name="health">The health to set to.</param>
        /// <exception cref="ArgumentException">Thrown when <c>health</c> is invalid, e.g. when it's greater than <see cref="MaxHealth"/> or less than zero.</exception>
        Task SetHealthAsync(double health);

        /// <summary>
        /// This should set the life/health stats to max (this should include food, water, etc if supported)
        /// </summary>
        Task SetFullHealthAsync();

        /// <summary>
        /// Damages the entity. Depending on game support, it will trigger damage effects.
        /// </summary>
        /// <param name="amount">The amount to damage.</param>
        Task DamageAsync(double amount);

        /// <summary>
        /// Kills the entity. Depending on game support, it will trigger death effects.
        /// </summary>
        Task KillAsync();
    }
}