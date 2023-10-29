using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System;
using System.Buffers;
using System.IO;
using System.Reflection;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleState : IVehicleState
    {
        internal const byte NewestSaveDataVersion = SaveDataVersionBatteryGuid;
        internal const byte InitialSaveDataVersion = 12;
        internal const byte SaveDataVersionBatteryGuid = VehicleManager.SAVEDATA_VERSION_BATTERY_GUID;

        internal static readonly FieldInfo? BatteryItemGuidField;

        static UnturnedVehicleState()
        {
            BatteryItemGuidField = typeof(InteractableVehicle).GetField("batteryItemGuid", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private readonly InteractableVehicle m_Vehicle;

        public UnturnedVehicleState(InteractableVehicle vehicle)
        {
            m_Vehicle = vehicle;
        }

        public byte[]? StateData
        {
            get
            {
                if (m_Vehicle.isAutoClearable || m_Vehicle.transform == null)
                {
                    return null;
                }

                var buffer = ArrayPool<byte>.Shared.Rent(Block.BUFFER_SIZE);

                using var stream = new MemoryStream(buffer, true);
                using var writer = new BinaryWriter(stream);

                // Position and rotation can't be saved to the state because it maybe is called from another thread
                // only solution is call ThreadUtil.assertIsGameThread();

                writer.Write(NewestSaveDataVersion);
                // removed: vehicle guid
                // removed: instanceID
                writer.Write(m_Vehicle.skinID);
                writer.Write(m_Vehicle.mythicID);
                writer.Write(m_Vehicle.roadPosition);
                writer.Write(m_Vehicle.fuel);
                writer.Write(m_Vehicle.health);
                writer.Write(m_Vehicle.batteryCharge);

                var batteryItemGuid = Guid.Empty;
                if (BatteryItemGuidField != null)
                {
                    batteryItemGuid = (Guid)BatteryItemGuidField.GetValue(m_Vehicle);
                }
                writer.Write(batteryItemGuid.ToByteArray());
                writer.Write(m_Vehicle.tireAliveMask);
                writer.Write(m_Vehicle.lockedOwner.m_SteamID);
                writer.Write(m_Vehicle.lockedGroup.m_SteamID);
                writer.Write(m_Vehicle.isLocked);

                if (m_Vehicle.turrets != null)
                {
                    writer.Write((byte)m_Vehicle.turrets.Length);

                    foreach (var passenger in m_Vehicle.turrets)
                    {
                        var state = passenger?.state ?? Array.Empty<byte>();
                        writer.Write((byte)state.Length);
                        writer.Write(state);
                    }
                }
                else
                {
                    writer.Write((byte)0);
                }

                if (m_Vehicle.trunkItems?.height > 0)
                {
                    writer.Write(true);

                    writer.Write(m_Vehicle.trunkItems.getItemCount());

                    foreach (var item in m_Vehicle.trunkItems.items)
                    {
                        writer.Write(item?.x ?? 0);
                        writer.Write(item?.y ?? 0);
                        writer.Write(item?.rot ?? 0);

                        writer.Write(item?.item.id ?? 0);
                        writer.Write(item?.item.amount ?? 0);
                        writer.Write(item?.item.quality ?? 0);

                        var state = item?.item.state ?? Array.Empty<byte>();
                        writer.Write((byte)state.Length);
                        writer.Write(state);
                    }
                }
                else
                {
                    writer.Write(false);
                }

                // decayTimer - missing, due to field protection level

                // set the last value to 255, to specify that it's the new version of state data
                writer.Write((byte)255);
                writer.Flush();

                stream.SetLength(stream.Position);
                var stateData = stream.ToArray();

                ArrayPool<byte>.Shared.Return(buffer);

                return stateData;
            }
        }
    }
}