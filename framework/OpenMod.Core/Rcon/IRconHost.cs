using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace OpenMod.Core.Rcon
{
    public interface IRconHost
    {
        bool IsListening { get; }

        Task StartListeningAsync(IPEndPoint bind, CancellationToken cancellationToken = default);

        Task StopListeningAsync();
    }
}