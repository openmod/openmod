using System.Net;
using System.Threading;
using System.Threading.Tasks;
using OpenMod.API.Commands;

namespace OpenMod.Core.Rcon
{
    public interface IRconClient : ICommandActor
    {
        IRconHost Host { get; }

        EndPoint EndPoint { get; }

        bool IsConnected { get; }

        bool IsAuthenticated { get; }

        Task SendMessageAsync(string message, CancellationToken cancellationToken = default);

        Task DisconnectAsync(string? reason = null, CancellationToken cancellationToken = default);

        Task StartAsync(CancellationToken cancellationToken = default);
    }
}