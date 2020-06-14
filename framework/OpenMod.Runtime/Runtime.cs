using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;
using Semver;
using Serilog;
using Serilog.Extensions.Logging;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    public sealed class Runtime : IRuntime
    {
        public Runtime()
        {
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
        }

        public SemVersion Version { get; }

        public string OpenModComponentId { get; } = "OpenMod.Runtime";

        public bool IsComponentAlive => Status != RuntimeStatus.Unloaded && Status != RuntimeStatus.Crashed;

        public ILifetimeScope LifetimeScope { get; private set; }

        public RuntimeStatus Status { get; private set; } = RuntimeStatus.Unloaded;

        public string WorkingDirectory { get; private set; }

        public string[] CommandlineArgs { get; private set; }

        public IDataStore DataStore { get; private set; }

        private IHost m_Host;
        private SerilogLoggerFactory m_LoggerFactory;
        private ILogger<Runtime> m_Logger;
        private readonly EventWaitHandle m_HostReadyEventHandle = new ManualResetEvent(false);

        public void Init(List<Assembly> openModAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            AsyncHelper.RunSync(() => InitAsync(openModAssemblies, hostBuilder, parameters));
        }

        public Task<IHost> InitAsync(
            List<Assembly> openModHostAssemblies,
            [CanBeNull] IHostBuilder hostBuilder,
            RuntimeInitParameters parameters)
        {
            var openModCoreAssembly = typeof(AsyncHelper).Assembly;
            if (!openModHostAssemblies.Contains(openModCoreAssembly))
            {
                openModHostAssemblies.Insert(0, openModCoreAssembly);
            }

            hostBuilder ??= new HostBuilder();

            if (!Directory.Exists(parameters.WorkingDirectory))
            {
                Directory.CreateDirectory(parameters.WorkingDirectory);
            }

            Status = RuntimeStatus.Initializing;
            WorkingDirectory = parameters.WorkingDirectory;
            CommandlineArgs = parameters.CommandlineArgs;

            SetupSerilog();

            m_Logger.LogInformation($"OpenMod v{Version} is starting...");

            var packagesDirectory = Path.Combine(WorkingDirectory, "packages");
            var nuGetPackageManager = new NuGetPackageManager(packagesDirectory)
            {
                Logger = new OpenModNuGetLogger(m_LoggerFactory.CreateLogger<OpenModNuGetLogger>())
            };
            nuGetPackageManager.InstallAssemblyResolver();

            var startupContext = new OpenModStartupContext
            {
                Runtime = this,
                LoggerFactory = m_LoggerFactory,
                NuGetPackageManager = nuGetPackageManager,
                DataStore = new Dictionary<string, object>()
            };

            var startup = new OpenModStartup(startupContext);
            foreach (var assembly in openModHostAssemblies)
            {
                startup.RegisterIocAssemblyAndCopyResources(assembly, string.Empty);
            }

            hostBuilder
                .UseContentRoot(parameters.WorkingDirectory)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(builder =>
                {
                    ConfigureConfiguration(builder);
                    ((OpenModStartupContext)startup.Context).Configuration = builder.Build();
                })
                .ConfigureAppConfiguration(ConfigureConfiguration)
                .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, startup))
                .ConfigureServices(services => SetupServices(services, startup))
                .UseSerilog();

            m_Host = hostBuilder.Build();
            Status = RuntimeStatus.Initialized;
            LifetimeScope = m_Host.Services.GetRequiredService<ILifetimeScope>();
            DataStore = m_Host.Services.GetRequiredService<IDataStoreFactory>().CreateDataStore("openmod", WorkingDirectory);

            Exception exception = null;
            AsyncHelper.Schedule("OpenMod start", StartHostAsync, ex =>
            {
                Status = RuntimeStatus.Crashed;
                m_Logger.LogCritical(ex, "OpenMod has crashed.");
                exception = ex;
                m_HostReadyEventHandle.Set();
            });

            m_HostReadyEventHandle.WaitOne();
            return exception == null 
                ? Task.FromResult(m_Host) 
                : Task.FromException<IHost>(exception);
        }

        private async Task StartHostAsync()
        {
            try
            {
                await m_Host.RunAsync();
                m_Logger.LogInformation("OpenMod has shut down.");
                Status = RuntimeStatus.Unloaded;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void ConfigureConfiguration(IConfigurationBuilder builder)
        {
            builder
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("openmod.yml", optional: false, reloadOnChange: true)
                .AddCommandLine(CommandlineArgs)
                .AddEnvironmentVariables("OpenMod_");
        }

        private void SetupServices(IServiceCollection services, OpenModStartup startup)
        {
            services.AddSingleton<IRuntime>(this);
            services.AddHostedService<OpenModHostedService>();
            services.AddOptions();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton(((OpenModStartupContext)startup.Context).NuGetPackageManager);

            AsyncHelper.RunSync(() => startup.CompleteServiceRegistrationsAsync(services));
        }

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void SetupContainer(ContainerBuilder containerBuilder, OpenModStartup startup)
        {
            AsyncHelper.RunSync(() => startup.CompleteContainerRegistrationAsync(containerBuilder));
        }

        private void SetupSerilog()
        {
            var loggingPath = Path.Combine(WorkingDirectory, "logging.yml");
            if (!File.Exists(loggingPath))
            {
                // First run, logging.yml doesn't exist yet. We can not wait for auto-copy as it would be too late.
                using var stream = typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yml");
                using var reader = new StreamReader(stream ?? throw new InvalidOperationException());

                var fileContent = reader.ReadToEnd();
                File.WriteAllText(loggingPath, fileContent);
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("logging.yml")
                .AddCommandLine(CommandlineArgs)
                .AddEnvironmentVariables()
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            var serilogLogger = Log.Logger = loggerConfiguration.CreateLogger();
            m_LoggerFactory = new SerilogLoggerFactory(serilogLogger);
            m_Logger = m_LoggerFactory.CreateLogger<Runtime>();
        }

        public async Task ShutdownAsync()
        {
            await m_Host.StopAsync();
            Status = RuntimeStatus.Unloaded;
        }

        internal void NotifyHostReady()
        {
            m_HostReadyEventHandle.Set();
        }
    }
}
