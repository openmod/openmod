using System;
using System.Threading.Tasks;

namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents an object that has health.
    /// </summary>
    public interface IHasHealth
    {
        /// <value>
        /// <b>True</b> if the entity is alive; otherwise, <b>false</b>.
        /// </value>
        bool IsAlive { get; }

        /// <value>
        /// The max health the entity can have.
        /// </value>
        double MaxHealth { get; }

        /// <value>
        /// The health of the entity.
        /// </value>
        double Health { get; }

        /// <summary>
        /// Sets the health of the entity. Depending on game support, it will not trigger damage effects.
        /// </summary>
        /// <param name="health">The health to set to.</param>
        /// <exception cref="ArgumentException">Thrown when <c>health</c> is invalid, e.g. when it's greater than <see cref="MaxHealth"/> or less than zero.</exception>
        Task SetHealthAsync(double health);

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