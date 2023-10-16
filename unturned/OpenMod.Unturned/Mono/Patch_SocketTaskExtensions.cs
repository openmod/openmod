using System;
using System.Buffers;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using OpenMod.Unturned.Patching;

namespace OpenMod.Unturned.Mono;

/// <summary>
/// Fixes <see cref="SocketTaskExtensions.ReceiveAsync(Socket, Memory{byte}, SocketFlags, CancellationToken)"/>, that <see cref="Memory{byte}"/> is always empty due
/// to coping the memory array.
/// https://github.com/Unity-Technologies/mono/blob/unity-main/mcs/class/System/System.Net.Sockets/SocketTaskExtensions.cs
/// </summary>
[HarmonyPatch]
internal static class Patch_SocketTaskExtensions
{
    [HarmonyCleanup]
    public static Exception? Cleanup(Exception ex, MethodBase original)
    {
        HarmonyExceptionHandler.ReportCleanupException(typeof(Patch_SocketTaskExtensions), ex, original);
        return null;
    }

    [HarmonyPatch(typeof(SocketTaskExtensions), nameof(SocketTaskExtensions.ReceiveAsync),
    new[] { typeof(Socket), typeof(Memory<byte>), typeof(SocketFlags), typeof(CancellationToken) })]
    [HarmonyPrefix]
    public static bool SocketTaskExtensionsReceiveAsync(Socket socket, Memory<byte> memory, SocketFlags socketFlags, CancellationToken cancellationToken,
        out ValueTask<int> __result)
    {
        var tcs = new TaskCompletionSource<int>(socket);
        if (MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)memory, out var segment))
        {
            // We were able to extract the underlying byte[] from the Memory<byte>. Use it.

            socket.BeginReceive(segment.Array, segment.Offset, segment.Count, socketFlags, static iar =>
            {
                var state = (Tuple<TaskCompletionSource<int>, CancellationToken>)iar.AsyncState;

                state.Item2.ThrowIfCancellationRequested();
                try
                {
                    state.Item1.TrySetResult(((Socket)state.Item1.Task.AsyncState).EndReceive(iar));
                }
                catch (Exception ex)
                {
                    state.Item1.TrySetException(ex);
                }
            }, Tuple.Create(tcs, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }
        else
        {
            // We weren't able to extract an underlying byte[] from the Memory<byte>.
            // Instead read into an ArrayPool array, then copy from that into the memory.

            var poolArray = ArrayPool<byte>.Shared.Rent(memory.Length);
            
            socket.BeginReceive(poolArray, 0, memory.Length, socketFlags, static iar =>
            {
                var state = (Tuple<TaskCompletionSource<int>, Memory<byte>, byte[], CancellationToken>)iar.AsyncState;
                try
                {
                    var bytesCopied = ((Socket)state.Item1.Task.AsyncState).EndReceive(iar);
                    new ReadOnlyMemory<byte>(state.Item3, 0, bytesCopied).Span.CopyTo(state.Item2.Span);
                    state.Item1.TrySetResult(bytesCopied);
                }
                catch (Exception e)
                {
                    state.Item1.TrySetException(e);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(state.Item3);
                }
            }, Tuple.Create(tcs, memory, poolArray, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        __result = new ValueTask<int>(tcs.Task);
        return false;
    }
}
