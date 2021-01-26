using OpenMod.Extensions.Games.Abstractions.Entities;
using OpenMod.Rust.Serialization;

namespace OpenMod.Rust.Entities
{
    public class RustEntityState : IEntityState
    {
        private readonly BaseEntity? m_Entity;
        private readonly byte[]? m_StateData;

        public RustEntityState(BaseEntity entity)
        {
            m_Entity = entity;
        }

        public RustEntityState(byte[] data)
        {
            m_StateData = data;
        }

        public byte[]? StateData
        {
            get
            {
                if (m_Entity != null)
                {
                    return BaseNetworkableSerializer.Serialize(m_Entity, out _);
                }

                return m_StateData;
            }
        }
    }
}