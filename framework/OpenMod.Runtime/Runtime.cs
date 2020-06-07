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
using OpenMod.Core.Helpers;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;
using Semver;
using Serilog;
using Serilog.Extensions.Logging;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    public sealed class Runtime : IRuntime, IOpenModComponent
    {
        public string WorkingDirectory { get; private set; }
        public string[] CommandlineArgs { get; private set; }
        public IConfigurationRoot Configuration { get; private set; }

        private IHost m_Host;
        private OpenModStartup m_Startup;
        private SerilogLoggerFactory m_LoggerFactory;
        private ILogger<Runtime> m_Logger;

        public void Init(ICollection<Assembly> openModAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            var thread = new Thread(o => { InitAsync(openModAssemblies, hostBuilder, parameters).Forget(); });
            thread.Start();
        }

        public async Task InitAsync(ICollection<Assembly> openModHostAssemblies, IHostBuilder hostBuilder,
            RuntimeInitParameters parameters)
        {
            Status = RuntimeStatus.Initializing;
            WorkingDirectory = parameters.WorkingDirectory;
            CommandlineArgs = parameters.CommandlineArgs;

            SetupSerilog();


            m_Logger.LogInformation("OpenMod is starting...");

            try
            {
                if (!Directory.Exists(parameters.WorkingDirectory))
                {
                    Directory.CreateDirectory(parameters.WorkingDirectory);
                }

                hostBuilder
                    .UseContentRoot(parameters.WorkingDirectory)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, openModHostAssemblies))
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<OpenModHostedService>();
                        services.AddOptions();
                        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
                        // m_Startup.ConfigureServices(services);
                    })
                    .UseSerilog();

                m_Host = hostBuilder.Build();
                Status = RuntimeStatus.Initialized;
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

        private void SetupContainer(ContainerBuilder containerBuilder, ICollection<Assembly> openModHostAssemblies)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("openmod.yml")
                .AddCommandLine(CommandlineArgs)
                .AddEnvironmentVariables("OpenMod_");

            containerBuilder.Register(context => this)
                .As<IRuntime>()
                .SingleInstance()
                .OwnedByLifetimeScope();

            var packagesDirectory = Path.Combine(WorkingDirectory, "packages");
            var nuGetPackageManager = new NuGetPackageManager(packagesDirectory);
            nuGetPackageManager.Logger = new OpenModNuGetLogger(m_LoggerFactory.CreateLogger<OpenModNuGetLogger>());
            nuGetPackageManager.InstallAssemblyResolver();

            containerBuilder.Register(context => nuGetPackageManager)
                .AsSelf()
                .SingleInstance()
                .OwnedByLifetimeScope();

            var startupContext = new OpenModStartupContext
            {
                Runtime = this,
                Configuration = Configuration,
                LoggerFactory = m_LoggerFactory
            };

            startupContext.DataStore.Add("nugetPackageManager", nuGetPackageManager);

            m_Startup = new OpenModStartup(startupContext, nuGetPackageManager, containerBuilder);
            startupContext.OpenModStartup = m_Startup;

            // register from OpenMod.Core, should prob. use a different class than AsyncHelper
            m_Startup.RegisterServiceFromAssemblyWithResources(typeof(AsyncHelper).Assembly, string.Empty);

            // register from assemblies such as OpenMod.UnityEngine, OpenMod.Unturned, etc.
            foreach (var assembly in openModHostAssemblies)
            {
                m_Startup.RegisterServiceFromAssemblyWithResources(assembly, string.Empty);
            }

            AsyncHelper.RunSync(async () => { await m_Startup.BootstrapAndRegisterPluginsAsync(); });

            Configuration = configurationBuilder.Build();
            containerBuilder.Register(context => Configuration)
                .As<IConfiguration>()
                .As<IConfigurationRoot>()
                .SingleInstance()
                .OwnedByLifetimeScope();

            m_Startup.Complete();
        }

        private void SetupSerilog()
        {
            var loggingPath = Path.Combine(WorkingDirectory, "logging.yml");
            if (!File.Exists(loggingPath))
            {
                // First run, logging.yml doesn't exist yet. We can not wait for auto-copy as it would be too late.
                using (Stream stream =
                    typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yml"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    File.WriteAllText(loggingPath, fileContent);
                }
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

        public SemVersion Version { get; } = new SemVersion(0, 1, 0);

        public string OpenModComponentId { get; } = "OpenMod.Runtime";

        public bool IsComponentAlive
        {
            get { return Status != RuntimeStatus.Unloaded && Status != RuntimeStatus.Crashed; }
        }

        public RuntimeStatus Status { get; private set; } = RuntimeStatus.Unloaded;
    }
}
