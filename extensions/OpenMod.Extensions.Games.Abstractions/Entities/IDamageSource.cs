namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents a damage source.
    /// </summary>
    public interface IDamageSource
    {
        /// <summary>
        /// Gets the human-readable name of the damage source.
        /// </summary>
        string DamageSourceName { get; }
    }
}
