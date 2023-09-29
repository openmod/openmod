namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    /// <summary>
    /// Represents the null entity state for entities that do not have a state.
    /// </summary>
    public sealed class NullEntityState : IEntityState
    {
        private static NullEntityState? s_Instance;
        public static NullEntityState Instance
        {
            get
            {
                return s_Instance ??= new NullEntityState();
            }
        }

        public byte[]? StateData { get; } = null;

        private NullEntityState() { }
    }
}