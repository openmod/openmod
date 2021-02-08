using System.Net;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Eventing;
using OpenMod.Core.Events;
using OpenMod.Core.Rcon.Minecraft;
using OpenMod.Core.Rcon.Source;

namespace OpenMod.Core.Rcon
{
    [UsedImplicitly]
    public class RconStartListener : IEventListener<OpenModInitializedEvent>
    {
        private readonly IRuntime m_Runtime;
        private readonly IConfiguration m_OpenModConfiguration;

        public RconStartListener(
            IRuntime runtime,
            IConfiguration openModConfiguration)
        {
            m_Runtime = runtime;
            m_OpenModConfiguration = openModConfiguration;
        }

        [EventListener(Priority = EventListenerPriority.Lowest)] // Lowest so it starts up as early as possible
        public Task HandleEventAsync(object sender, OpenModInitializedEvent @event)
        {
            var bind = m_OpenModConfiguration.GetSection("rcon:bind").Get<string>();

            var sourceRconEnabled = m_OpenModConfiguration.GetSection("rcon:srcds:enabled").Get<bool>();
            if (sourceRconEnabled)
            {
                var port = m_OpenModConfiguration.GetSection("rcon:srcds:port").Get<int>();
                var endpoint = new IPEndPoint(IPAddress.Parse(bind), port);
                var sourceRconListener = new SourceRconHost(m_Runtime.Host!.Services);

                Task.Run(() => sourceRconListener.StartListeningAsync(endpoint));
            }

            var minecraftRconEnabled = m_OpenModConfiguration.GetSection("rcon:minecraft:enabled").Get<bool>();
            if(minecraftRconEnabled)
            {
                var port = m_OpenModConfiguration.GetSection("rcon:minecraft:port").Get<int>();
                var endpoint = new IPEndPoint(IPAddress.Parse(bind), port);
                var minecraftRconListener = new MinecraftRconHost(m_Runtime.Host!.Services);

                Task.Run(() => minecraftRconListener.StartListeningAsync(endpoint));
            }

            return Task.CompletedTask;
        }
    }
}