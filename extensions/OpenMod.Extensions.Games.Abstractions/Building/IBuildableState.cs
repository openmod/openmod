namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents the state of a buildable.
    /// </summary>
    public interface IBuildableState
    {
        /// <summary>
        /// Gets the healh of the buildable.
        /// </summary>
        double Health { get; }

        /// <summary>
        /// Gets the state of the buildable.
        /// </summary>
        byte[]? StateData { get; }
    }
}