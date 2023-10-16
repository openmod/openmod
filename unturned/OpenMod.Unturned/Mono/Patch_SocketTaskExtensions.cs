using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;

namespace OpenMod.Unturned.Mono;
[HarmonyPatch]
internal static class Patch_SocketTaskExtensions
{
    [HarmonyPatch(typeof(SocketTaskExtensions), nameof(SocketTaskExtensions.ReceiveAsync),
    new[] { typeof(Socket), typeof(Memory<byte>), typeof(SocketFlags), typeof(CancellationToken) })]
    [HarmonyPrefix]
    public static bool SocketTaskExtensionsReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags, CancellationToken cancellationToken,
        out ValueTask<int> __result)
    {
        if (MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)memory, out var segment))
        {
            var taskCompletionSource = new TaskCompletionSource<int>(socket);
            socket.BeginReceive(segment.Array, segment.Offset, segment.Count, socketFlags, iar =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                var taskCompletionSource2 = (TaskCompletionSource<int>)iar.AsyncState;
                var socket2 = (Socket)taskCompletionSource2.Task.AsyncState;
                try
                {
                    taskCompletionSource2.TrySetResult(socket2.EndReceive(iar));
                }
                catch (Exception ex)
                {
                    taskCompletionSource2.TrySetException(ex);
                }
            }, taskCompletionSource);

            cancellationToken.ThrowIfCancellationRequested();
            __result = new ValueTask<int>(taskCompletionSource.Task);
        }
        else
        {
            __result = new(0);
        }

        return false;
    }
}
