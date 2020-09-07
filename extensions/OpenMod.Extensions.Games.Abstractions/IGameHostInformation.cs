using JetBrains.Annotations;
using OpenMod.API;

namespace OpenMod.Extensions.Games.Abstractions
{
    public interface IGameHostInformation : IHostInformation
    {
        /// <summary>
        ///    Checks if the current host is a game client. Keep in mind that a host can be both server and client at the same time.
        /// </summary>
        bool IsClient { get; }

        /// <summary>
        ///    Checks if the current host is a game server. Keep in mind that a host can be both server and client at the same time.
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        ///    Get's the port used to connect to the server. Returns null if the current host is not a server.
        /// </summary>
        [CanBeNull]
        ushort? ServerPort { get; }

        /// <summary>
        ///    Some games support multiple server instances per installation.
        /// </summary>
        public string ServerInstance { get; }

        /// <summary>
        ///    Get's the server's name. Returns null if the current host is not a server.
        /// </summary>
        [CanBeNull]
        string ServerName { get; }

        /// <summary>
        ///     The name of the game.
        /// </summary>
        string GameDisplayName { get; }

        /// <summary>
        ///    The version of the game.
        /// </summary>
        string GameVersion { get; }
    }
}