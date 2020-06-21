using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
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
using OpenMod.API.Eventing;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;
using Semver;
using Serilog;
using Serilog.Extensions.Logging;

#pragma warning disable CS4014

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

        // these variables are used for runtime restart
        private List<Assembly> m_OpenModHostAssemblies;
        private Func<IHostBuilder> m_HostBuilderFunc;
        private RuntimeInitParameters m_RuntimeInitParameters; private IHostApplicationLifetime m_AppLifeTime;

        public void Init(List<Assembly> openModAssemblies, RuntimeInitParameters parameters, Func<IHostBuilder> hostBuilderFunc = null)
        {
            AsyncHelper.RunSync(() => InitAsync(openModAssemblies, parameters, hostBuilderFunc));
        }

        public async Task<IHost> InitAsync(
            List<Assembly> openModHostAssemblies,
            RuntimeInitParameters parameters,
            Func<IHostBuilder> hostBuilderFunc = null)
        {
            var openModCoreAssembly = typeof(AsyncHelper).Assembly;
            if (!openModHostAssemblies.Contains(openModCoreAssembly))
            {
                openModHostAssemblies.Insert(0, openModCoreAssembly);
            }

            m_OpenModHostAssemblies = openModHostAssemblies;
            m_HostBuilderFunc = hostBuilderFunc;
            m_RuntimeInitParameters = parameters;

            var hostBuilder = hostBuilderFunc == null ? new HostBuilder() : hostBuilderFunc();

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

            await startup.LoadPluginAssembliesAsync();

            hostBuilder
                .UseContentRoot(parameters.WorkingDirectory)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureHostConfiguration(builder =>
                {
                    ConfigureConfiguration(builder, startup);
                    ((OpenModStartupContext)startup.Context).Configuration = builder.Build();
                })
                .ConfigureAppConfiguration(builder => ConfigureConfiguration(builder, startup))
                .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, startup))
                .ConfigureServices(services => SetupServices(services, startup))
                .UseSerilog();

            m_Host = hostBuilder.Build();
            m_AppLifeTime = m_Host.Services.GetRequiredService<IHostApplicationLifetime>();
            m_AppLifeTime.ApplicationStopping.Register(() => { AsyncHelper.RunSync(ShutdownAsync); });

            Status = RuntimeStatus.Initialized;
            LifetimeScope = m_Host.Services.GetRequiredService<ILifetimeScope>();
            DataStore = m_Host.Services.GetRequiredService<IDataStoreFactory>().CreateDataStore("openmod", WorkingDirectory);

            var eventBus = m_Host.Services.GetRequiredService<IEventBus>();
            foreach (var assembly in openModHostAssemblies)
            {
                eventBus.Subscribe(this, assembly);
            }

            try
            {
                await m_Host.StartAsync();
            }
            catch (Exception ex)
            {
                Status = RuntimeStatus.Crashed;
                m_Logger.LogCritical(ex, "OpenMod has crashed.");
                Log.CloseAndFlush();
            }
            return m_Host;
        }
        
        public async Task ReloadAsync()
        {
            await ShutdownAsync();
            await InitAsync(m_OpenModHostAssemblies, m_RuntimeInitParameters, m_HostBuilderFunc);
        }

        private void ConfigureConfiguration(IConfigurationBuilder builder, OpenModStartup startup)
        {
            builder
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("openmod.yaml", optional: false, reloadOnChange: true)
                .AddCommandLine(CommandlineArgs)
                .AddEnvironmentVariables("OpenMod_");
            startup.ConfigureConfiguration(builder);
        }

        private void SetupServices(IServiceCollection services, OpenModStartup startup)
        {
            services.AddSingleton<IRuntime>(this);
            services.AddHostedService<OpenModHostedService>();
            services.AddOptions();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton(((OpenModStartupContext)startup.Context).NuGetPackageManager);
            startup.SetupServices(services);
        }

        private void SetupContainer(ContainerBuilder containerBuilder, OpenModStartup startup)
        {
            startup.SetupContainer(containerBuilder);
        }

        private void SetupSerilog()
        {
            var loggingPath = Path.Combine(WorkingDirectory, "logging.yaml");
            if (!File.Exists(loggingPath))
            {
                // First run, logging.yaml doesn't exist yet. We can not wait for auto-copy as it would be too late.
                using var stream = typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yaml");
                using var reader = new StreamReader(stream ?? throw new MissingManifestResourceException("Couldn't find resource: OpenMod.Core.logging.yaml"));

                var fileContent = reader.ReadToEnd();
                File.WriteAllText(loggingPath, fileContent);
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("logging.yaml")
                .AddCommandLine(CommandlineArgs)
                .AddEnvironmentVariables()
                .Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            var serilogLogger = Log.Logger = loggerConfiguration.CreateLogger();
            m_LoggerFactory = new SerilogLoggerFactory(serilogLogger);
            m_Logger = m_LoggerFactory.CreateLogger<Runtime>();
        }

        public Task ShutdownAsync()
        {
            m_Logger.LogInformation("OpenMod is shutting down...");
            m_Host.Dispose();
            Status = RuntimeStatus.Unloaded;
            Log.CloseAndFlush();
            return Task.CompletedTask;
        }
    }
}
