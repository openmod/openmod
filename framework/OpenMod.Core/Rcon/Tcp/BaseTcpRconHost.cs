using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Eventing;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Rcon.Events;

namespace OpenMod.Core.Rcon.Tcp
{
    public abstract class BaseTcpRconHost<TClient> : IRconHost, IAsyncDisposable where TClient : class, IRconClient
    {
        public IPEndPoint? Bind { get; private set; }
        public bool IsListening
        {
            get { return m_Listener != null && (!m_ListenerTask?.IsCompleted ?? false) && !m_IsStopped; }
        }

        private readonly IEventBus m_EventBus;
        private readonly IRuntime m_Runtime;
        private readonly ILifetimeScope m_LifetimeScope;
        private readonly ILogger<BaseTcpRconHost<TClient>> m_Logger;

        private volatile bool m_IsStopped;

        private TcpListener? m_Listener;
        private Task? m_ListenerTask;

        protected BaseTcpRconHost(IServiceProvider serviceProvider)
        {
            m_LifetimeScope = serviceProvider.GetRequiredService<ILifetimeScope>();
            m_EventBus = serviceProvider.GetRequiredService<IEventBus>();
            m_Runtime = serviceProvider.GetRequiredService<IRuntime>();
            m_Logger = serviceProvider.GetRequiredService<ILogger<BaseTcpRconHost<TClient>>>();
        }

        public virtual Task StartListeningAsync(IPEndPoint bind, CancellationToken cancellationToken = default)
        {
            if (m_Listener != null)
            {
                throw new InvalidOperationException("The listener is already running.");
            }

            Bind = bind;

            m_IsStopped = false;
            m_Listener = new TcpListener(Bind);

            cancellationToken.Register(() => { m_IsStopped = true; });

            var task = Task.Run(async () =>
            {
                var clients = new List<TClient>();

                try
                {
                    m_Listener.Start();
                    m_Logger.LogInformation("{Type} started listening on {IPAddress}:{Port}",
                        GetType().Name, Bind.Address, Bind.Port);

                    while (true)
                    {
                        try
                        {
                            var tcpClient = await m_Listener.AcceptTcpClientAsync();
                            cancellationToken.ThrowIfCancellationRequested();

                            var client = await OnClientConnected(tcpClient);
                            if (client == null)
                            {
                                // Connection was rejected
                                continue;
                            }

                            var eventSubs = new List<IDisposable>();
                            var sub = m_EventBus.Subscribe<RconClientAuthenticatingEvent>(m_Runtime, async (_, _, e) =>
                            {
                                if (e.Client != client)
                                {
                                    return;
                                }

                                // Ensure clients get disconnected if authentication failed
                                if (!e.IsAuthenticated)
                                {
                                    if (client.IsConnected)
                                    {
                                        await client.DisconnectAsync("Authorization failed.", cancellationToken);
                                    }
                                }
                            });

                            eventSubs.Add(sub);

                            sub = m_EventBus.Subscribe<RconClientDisconnectedEvent>(m_Runtime, async (_, _, e) =>
                            {
                                if (e.Client != client)
                                {
                                    return;
                                }

                                foreach (var s in eventSubs)
                                {
                                    s.Dispose();
                                }

                                await client.DisposeSyncOrAsync();
                                clients.Remove(client);
                            });

                            eventSubs.Add(sub);
                            clients.Add(client);

                        }
                        catch (ObjectDisposedException) when (m_IsStopped)
                        {
                            // Listener was stopped
                            break;
                        }
                        catch (TaskCanceledException)
                        {
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Logger.LogError(ex, "Exception occured in RCON host task");
                }
                finally
                {
                    foreach (var client in clients)
                    {
                        if (client.IsConnected)
                        {
                            await client.DisconnectAsync(cancellationToken: cancellationToken);
                        }

                        await client.DisposeSyncOrAsync();
                    }

                    clients.Clear();
                    m_Listener = null;
                    m_IsStopped = true;
                }

                m_Logger.LogInformation("{Type} stopped", GetType().Name);
            }, cancellationToken);

            m_ListenerTask = task;
            return Task.CompletedTask;
        }

        protected virtual async Task<TClient?> OnClientConnected(TcpClient tcpClient)
        {
            var client = await CreateRconClientAsync(tcpClient);
            await client.StartAsync();

            var @event = new RconClientConnectingEvent(client);

            await m_EventBus.EmitAsync(m_Runtime, this, @event);
            if (@event.IsCancelled)
            {
                return null;
            }

            await m_EventBus.EmitAsync(m_Runtime, this, new RconClientConnectedEvent(client));
            return client;
        }

        public virtual Task StopListeningAsync()
        {
            if (!IsListening)
            {
                return Task.CompletedTask;
            }

            m_IsStopped = true;
            m_Listener?.Stop();
            m_Listener = null;
            return Task.CompletedTask;
        }

        protected virtual Task<TClient> CreateRconClientAsync(TcpClient tcpClient)
        {
            return Task.FromResult(ActivatorUtilitiesEx.CreateInstance<TClient>(m_LifetimeScope, tcpClient, this));
        }

        public virtual async ValueTask DisposeAsync()
        {
            await StopListeningAsync();
        }
    }
}