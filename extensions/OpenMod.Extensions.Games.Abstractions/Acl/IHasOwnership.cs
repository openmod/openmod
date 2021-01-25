using JetBrains.Annotations;

namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    /// <summary>
    /// Defines that an object can have an owner.
    /// </summary>
    public interface IHasOwnership
    {
        /// <value>
        /// The owner of this object. Cannot be null.
        /// </value>
        [NotNull]
        public IOwnership Ownership { get; }
    }
}