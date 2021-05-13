using System;
using OpenMod.Extensions.Games.Abstractions.Building;
using OpenMod.Rust.Serialization;

namespace OpenMod.Rust.Building
{
    public class RustBuildingBlockState : IBuildableState
    {
        private readonly BuildingBlock? m_BuildingBlock;
        private readonly byte[]? m_StateData;

        public RustBuildingBlockState(BuildingBlock buildingBlock)
        {
            m_BuildingBlock = buildingBlock;
        }

        public double Health => m_BuildingBlock != null ? m_BuildingBlock.Health() : throw new NotImplementedException("Health from raw state is not supported yet.");

        public RustBuildingBlockState(byte[] data)
        {
            m_StateData = data;
        }

        public byte[]? StateData
        {
            get
            {
                if (m_BuildingBlock != null)
                {
                    return BaseNetworkableSerializer.Serialize(m_BuildingBlock, out _);
                }

                return m_StateData;
            }
        }
    }
}