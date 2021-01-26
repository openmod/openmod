using OpenMod.Extensions.Games.Abstractions.Vehicles;
using SDG.Unturned;
using System;

namespace OpenMod.Unturned.Vehicles
{
    public class UnturnedVehicleState : IVehicleState
    {
        private readonly InteractableVehicle m_Vehicle;

        public UnturnedVehicleState(InteractableVehicle vehicle)
        {
            m_Vehicle = vehicle;
        }

        public byte[]? StateData
        {
            get
            {
                // SAVEDATA_VERSION = 12

                // cannot use Block because it's singleton
                var buffer = new byte[Block.BUFFER_SIZE];
                var step = 0;

                if (m_Vehicle.isAutoClearable || m_Vehicle.transform == null)
                {
                    return null;
                }

                // Position and rotation can't be saved to the state because it maybe is called from another thread
                // only solution is call ThreadUtil.assertIsGameThread();

                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.id), step);
                step += 2;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.instanceID), step);
                step += 4;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.skinID), step);
                step += 2;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.mythicID), step);
                step += 2;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.roadPosition), step);
                step += 4;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.fuel), step);
                step += 2;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.health), step);
                step += 2;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.batteryCharge), step);
                step += 2;
                buffer[step] = m_Vehicle.tireAliveMask;
                step++;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.lockedOwner.m_SteamID), step);
                step += 8;
                CopyBytes(buffer, BitConverter.GetBytes(m_Vehicle.lockedGroup.m_SteamID), step);
                step += 8;
                buffer[step] = BitConverter.GetBytes(m_Vehicle.isLocked)[0];
                step++;

                if (m_Vehicle.turrets != null)
                {
                    buffer[step] = (byte)m_Vehicle.turrets.Length;
                    step++;

                    foreach (var passenger in m_Vehicle.turrets)
                    {
                        if (passenger?.state != null)
                        {
                            buffer[step] = (byte)passenger.state.Length;
                            step++;
                            CopyBytes(buffer, passenger.state, step);
                        }
                        else
                        {
                            var empty = Array.Empty<byte>();
                            buffer[step] = (byte)empty.Length;
                            step++;
                            CopyBytes(buffer, empty, step);
                        }
                    }
                }
                else
                {
                    buffer[step] = 0;
                    step++;
                }

                if (m_Vehicle.trunkItems?.height > 0)
                {
                    buffer[step] = BitConverter.GetBytes(true)[0];
                    step++;

                    buffer[step] = m_Vehicle.trunkItems.getItemCount();
                    step++;

                    foreach (var item in m_Vehicle.trunkItems.items)
                    {
                        buffer[step] = item?.x ?? 0;
                        step++;

                        buffer[step] = item?.y ?? 0;
                        step++;

                        buffer[step] = item?.rot ?? 0;
                        step++;

                        if (item != null)
                        {
                            CopyBytes(buffer, BitConverter.GetBytes(item.item.id), step);
                        }
                        step += sizeof(ushort);

                        buffer[step] = item?.item.amount ?? 0;
                        step++;

                        buffer[step] = item?.item.quality ?? 0;
                        step++;

                        var state = (item != null) ? item.item.state : Array.Empty<byte>();
                        buffer[step] = (byte)state.Length;

                        step++;
                        CopyBytes(buffer, state, step);
                    }
                }
                else
                {
                    buffer[step] = BitConverter.GetBytes(false)[0];
                }

                return buffer;
            }
        }

        private void CopyBytes(byte[] buffer, byte[] bytes, int step)
        {
            Buffer.BlockCopy(bytes, 0, buffer, step, bytes.Length);
        }
    }
}