using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Events;
using OpenMod.Core.Ioc;
using OpenMod.Unturned.Configuration;
using OpenMod.Unturned.RocketMod.Events;
using Rocket.Core;

namespace OpenMod.Unturned.RocketMod.Rcon
{
    public class RconStartListener : IEventListener<OpenModInitializedEvent>, IEventListener<RocketModInitializedEvent>
    {
        private readonly IEventBus m_EventBus;
        private readonly IRuntime m_Runtime;
        private readonly IConfiguration m_OpenModConfiguration;
        private readonly IConfiguration m_UnturnedConfiguration;
        private static bool s_RocketInitialized;
        public RconStartListener(
            IEventBus eventBus,
            IRuntime runtime,
            IConfiguration openModConfiguration,
            IOpenModUnturnedConfiguration unturnedConfiguration)
        {
            m_EventBus = eventBus;
            m_Runtime = runtime;
            m_OpenModConfiguration = openModConfiguration;
            m_UnturnedConfiguration = unturnedConfiguration.Configuration;
        }

        public Task HandleEventAsync(object? sender, OpenModInitializedEvent @event)
        {
            if (RocketModIntegration.IsRocketModUnturnedLoaded(out _))
            {
                if (s_RocketInitialized)
                {
                    StartRocketModRconWithRocketModSettings();
                }

                // Use RocketModInitializedEvent instead
                return Task.CompletedTask;
            }

            var rocketRconEnabled = m_UnturnedConfiguration.GetSection("rcon:rocketmod:enabled").Get<bool>();
            if (!rocketRconEnabled)
            {
                return Task.CompletedTask;
            }

            var bind = m_OpenModConfiguration.GetSection("rcon:bind").Get<string>() ?? "127.0.0.1";;
            var port = m_UnturnedConfiguration.GetSection("rcon:rocketmod:port").Get<int>();

            StartRocketModRcon(bind, port);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object? sender, RocketModInitializedEvent @event)
        {
            s_RocketInitialized = true;
            StartRocketModRconWithRocketModSettings();
            return Task.CompletedTask;
        }

        private void StartRocketModRconWithRocketModSettings()
        {
            var settings = R.Settings.Instance.RCON;
            if (!settings.Enabled)
            {
                return;
            }

            var bind = !settings.EnableMaxGlobalConnections || settings.MaxGlobalConnections == 0
                ? "127.0.0.1"
                : "0.0.0.0";
            var port = settings.Port;

            StartRocketModRcon(bind, port);
            s_RocketInitialized = true;
        }

        private void StartRocketModRcon(string bind, int port)
        {
            var cancellationToken = GetCancellationToken();
            var endpoint = new IPEndPoint(IPAddress.Parse(bind), port);
            var rocketRconListener = ActivatorUtilitiesEx.CreateInstance<RocketModRconHost>(m_Runtime.LifetimeScope);
            Task.Run(() => rocketRconListener.StartListeningAsync(endpoint, cancellationToken), cancellationToken);
        }

        private CancellationToken GetCancellationToken()
        {
            var cts = new CancellationTokenSource(); // lgtm [cs/local-not-disposed]
            m_EventBus.Subscribe<OpenModShutdownEvent>(m_Runtime, (_, _, _) =>
            {
                cts.Cancel();
                cts.Dispose();
                return Task.CompletedTask;
            });

            return cts.Token;
        }
    }
}