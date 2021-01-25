using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Building
{
    /// <summary>
    /// Represents the state of a buildable.
    /// </summary>
    public interface IBuildableState
    {
        /// <value>
        /// The heal of the buildable.
        /// </value>
        double Health { get; }

        /// <value>
        /// State of the buildable. Can be null.
        /// </value>
        [CanBeNull]
        byte[] StateData { get; }
    }
}