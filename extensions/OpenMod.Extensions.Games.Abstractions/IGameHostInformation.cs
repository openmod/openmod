using JetBrains.Annotations;
using OpenMod.API;

namespace OpenMod.Extensions.Games.Abstractions
{
    /// <summary>
    /// Provides information about the current game.
    /// </summary>
    public interface IGameHostInformation : IHostInformation
    {
        /// <value>
        ///<b>True</b> if the current host is a game client; otherwsie, <b>false</b>. Keep in mind that a host can be both server and client at the same time.
        /// </value>
        bool IsClient { get; }

        /// <value>
        /// <b>True</b> if the current host is a game server; otherwise, <b>false</b>. Keep in mind that a host can be both server and client at the same time.
        /// </value>
        bool IsServer { get; }

        /// <value>
        /// The port used to connect to the server. <b>Null</b> if the current host is not a server or is not listening yet.
        /// </value>
        [CanBeNull]
        ushort? ServerPort { get; }

        /// <value>
        /// The current server instance for games that support multiple server instances per installation.
        /// <b>default</b> for games that do not support multiple instances. <b>Null</b> if the host is a client.
        /// </value>
        [NotNull]
        public string ServerInstance { get; }

        /// <value>
        /// The name of the server. <b>Null </b> if the host is a client.
        /// </value>
        [CanBeNull]
        string ServerName { get; }

        /// <value>
        /// The name of the game.
        /// </value>
        [NotNull]
        string GameDisplayName { get; }

        /// <value>
        /// The version of the running game.
        /// </value>
        [NotNull]
        string GameVersion { get; }
    }
}