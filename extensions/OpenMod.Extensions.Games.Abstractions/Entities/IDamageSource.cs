namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents a damage source.
    /// </summary>
    public interface IDamageSource
    {
        /// <value>
        /// The human readable name of the damage source.
        /// </value>
        string DamageSourceName { get; }
    }
}
