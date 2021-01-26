namespace OpenMod.Extensions.Games.Abstractions.Acl
{
    /// <summary>
    /// Defines that an object can have an owner.
    /// </summary>
    public interface IHasOwnership
    {
        /// <value>
        /// The owner of this object.
        /// </value>
        public IOwnership Ownership { get; }
    }
}