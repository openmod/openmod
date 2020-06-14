using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            Assembly runtimeAssembly = typeof(Runtime).Assembly;
            var version = runtimeAssembly.GetName().Version;
            Version = new SemVersion(version.Major, version.Minor, version.Build, build: version.Revision != 0 ? version.Revision.ToString() : "");
        }

        public string WorkingDirectory { get; private set; }
        public string[] CommandlineArgs { get; private set; }
        public IDataStore DataStore { get; private set; }

        private IHost m_Host;
        private SerilogLoggerFactory m_LoggerFactory;
        private ILogger<Runtime> m_Logger;

        public void Init(List<Assembly> openModAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            AsyncHelper.RunSync(() => InitAsync(openModAssemblies, hostBuilder, parameters));
        }

        public async Task InitAsync(
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

            try
            {
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
                await m_Host.RunAsync();
                m_Logger.LogInformation("OpenMod has shut down.");
                Status = RuntimeStatus.Unloaded;
            }
            catch (Exception ex)
            {
                Status = RuntimeStatus.Crashed;
                m_Logger.LogCritical(ex, "OpenMod has crashed.");
                throw;
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
                .AddYamlFile("openmod.yml")
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

            AsyncHelper.RunSync(async () => { await startup.CompleteServiceRegistrationsAsync(services); });
        }

        private void SetupContainer(ContainerBuilder containerBuilder, OpenModStartup startup)
        {
            AsyncHelper.RunSync(async () => { await startup.CompleteContainerRegistrationAsync(containerBuilder); });
        }

        private void SetupSerilog()
        {
            var loggingPath = Path.Combine(WorkingDirectory, "logging.yml");
            if (!File.Exists(loggingPath))
            {
                // First run, logging.yml doesn't exist yet. We can not wait for auto-copy as it would be too late.
                using Stream stream = typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yml");
                using StreamReader reader = new StreamReader(stream);
                
                string fileContent = reader.ReadToEnd();
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

        public SemVersion Version { get; }

        public string OpenModComponentId { get; } = "OpenMod.Runtime";

        public bool IsComponentAlive
        {
            get { return Status != RuntimeStatus.Unloaded && Status != RuntimeStatus.Crashed; }
        }

        public ILifetimeScope LifetimeScope { get; private set; }

        public RuntimeStatus Status { get; private set; } = RuntimeStatus.Unloaded;
    }
}
