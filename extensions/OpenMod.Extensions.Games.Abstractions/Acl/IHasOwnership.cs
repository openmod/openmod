namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    /// <summary>
    /// Defines that an object can have an owner.
    /// </summary>
    public interface IHasOwnership
    {
        /// <summary>
        /// Gets the ownership of this object.
        /// </summary>
        public IOwnership Ownership { get; }
    }
}