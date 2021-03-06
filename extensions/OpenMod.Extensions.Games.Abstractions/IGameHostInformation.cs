using System.Net;
using OpenMod.API;

namespace OpenMod.Extensions.Games.Abstractions
{
    /// <summary>
    /// Provides information about the current game.
    /// </summary>
    public interface IGameHostInformation : IHostInformation
    {
        /// <summary>
        /// Checks if the current host is a client.
        /// </summary>
        /// <remarks>
        /// A host can be both server and client at the same time.
        /// </remarks>
        bool IsClient { get; }

        /// <summary>
        /// Checks if the current host is a server.
        /// </summary>
        /// <remarks>
        /// A host can be both server and client at the same time.
        /// </remarks>
        bool IsServer { get; }

        /// <summary>
        /// Gets the address used to connect to the server. <b>Null</b> if the current host is not a server or is not listening yet.
        /// </summary>
        IPAddress? ServerAddress { get; }

        /// <summary>
        /// Gets the port used to connect to the server. <b>Null</b> if the current host is not a server or is not listening yet.
        /// </summary>
        ushort? ServerPort { get; }

        /// <summary>
        /// Gets the current server instance for games that support multiple server instances per installation. Returns
        /// <b>default</b> for games that do not support multiple instances. Returns <b>null</b> if the host is not a server.
        /// </summary>
        public string ServerInstance { get; }

        /// <summary>
        /// Gets the name of the server. Returns <b>null</b> if the host is not a server.
        /// </summary>
        string? ServerName { get; }

        /// <summary>
        /// Gets the name of the game.
        /// </summary>
        string GameDisplayName { get; }

        /// <summary>
        /// Gets the version of the running game.
        /// </summary>
        string GameVersion { get; }
    }
}