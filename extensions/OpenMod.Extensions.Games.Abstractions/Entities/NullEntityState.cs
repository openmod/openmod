namespace OpenMod.Extensions.Games.Abstractions.Entities
{
    public sealed class NullEntityState : IEntityState
    {
        private static NullEntityState m_Instance;
        public static NullEntityState Instance
        {
            get
            {
                return m_Instance ??= new NullEntityState();
            }
        }

        public byte[] StateData { get; } = null;

        private NullEntityState() { }
    }
}