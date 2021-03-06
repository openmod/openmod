using System.Net;
using OpenMod.Core.Helpers;
using OpenMod.Extensions.Games.Abstractions;
using Rust;
using Semver;

namespace OpenMod.Rust
{
    public class RustHostInformation : IGameHostInformation
    {
        public RustHostInformation()
        {
            HostVersion = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        public SemVersion HostVersion { get; }

        public string HostName { get; } = "OpenMod for Rust";

        public bool IsClient { get; } = false;

        public bool IsServer { get; } = true;

        public IPAddress? ServerAddress { get; } = IPAddress.Parse(ConVar.Server.ip);

        public ushort? ServerPort { get; } = (ushort) ConVar.Server.port;

        public string ServerInstance { get; } = ConVar.Server.identity;

        public string ServerName { get; } = ConVar.Server.hostname;

        public string GameDisplayName { get; } = "Rust";

        public string GameVersion { get; } = Protocol.printable;
    }
}