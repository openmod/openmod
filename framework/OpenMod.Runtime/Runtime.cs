using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Permissions;
using OpenMod.API.Persistence;
using OpenMod.Common.Hotloading;
using OpenMod.Core.Helpers;
using OpenMod.Core.Ioc;
using OpenMod.Core.Permissions;
using OpenMod.Core.Persistence;
using OpenMod.Core.Plugins;
using OpenMod.Core.Plugins.NuGet;
using OpenMod.NuGet;
using Semver;
using Serilog;
using Serilog.Extensions.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using AssemblyExtensions = OpenMod.Common.Helpers.AssemblyExtensions;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    [OpenModInternal]
    public sealed class Runtime : IRuntime
    {
        public Runtime()
        {
            Version = VersionHelper.ParseAssemblyVersion(GetType().Assembly);
            HostAssemblies = new List<Assembly>();
        }

        public SemVersion Version { get; }

        public string OpenModComponentId { get; } = "OpenMod.Core";

        public bool IsComponentAlive => Status != RuntimeStatus.Unloaded && Status != RuntimeStatus.Crashed;

        public ILifetimeScope LifetimeScope { get; private set; } = null!;

        public RuntimeStatus Status { get; private set; } = RuntimeStatus.Unloaded;

        public string WorkingDirectory { get; private set; } = null!;

        public string[] CommandlineArgs { get; private set; } = new string[0];

        public IDataStore DataStore { get; private set; } = null!;

        public IHostInformation? HostInformation { get; private set; }

        public IReadOnlyCollection<Assembly> HostAssemblies { get; private set; }

        public IHost? Host { get; private set; }

        public bool IsDisposing { get; private set; }

        private SerilogLoggerFactory? m_LoggerFactory;
        private ILogger<Runtime>? m_Logger;
        private IHostApplicationLifetime? m_AppLifeTime;

        // these variables are used for soft reloads
        private List<Assembly>? m_OpenModHostAssemblies;
        private Func<IHostBuilder>? m_HostBuilderFunc;
        private RuntimeInitParameters? m_RuntimeInitParameters;

        public void Init(List<Assembly> openModAssemblies, RuntimeInitParameters parameters, Func<IHostBuilder>? hostBuilderFunc = null)
        {
            AsyncHelper.RunSync(() => InitAsync(openModAssemblies, parameters, hostBuilderFunc));
        }

        public async Task<IHost> InitAsync(
            List<Assembly> openModHostAssemblies,
            RuntimeInitParameters parameters,
            Func<IHostBuilder>? hostBuilderFunc = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            HostAssemblies = openModHostAssemblies ?? throw new ArgumentNullException(nameof(openModHostAssemblies));

            try
            {
                IsDisposing = false;

                var openModCoreAssembly = typeof(AsyncHelper).Assembly;
                if (!openModHostAssemblies.Contains(openModCoreAssembly))
                {
                    openModHostAssemblies.Insert(0, openModCoreAssembly);
                }

                var hostInformationType = openModHostAssemblies
                    .Select(asm =>
                        AssemblyExtensions.GetLoadableTypes(asm)
                            .FirstOrDefault(t => typeof(IHostInformation).IsAssignableFrom(t)))
                    .LastOrDefault(d => d != null);

                if (hostInformationType == null)
                {
                    throw new Exception("Failed to find IHostInformation in host assemblies.");
                }

                HostInformation = (IHostInformation)Activator.CreateInstance(hostInformationType);
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

                m_Logger.LogInformation("OpenMod v{Version} is starting...", Version);

                if (parameters.PackageManager is not NuGetPackageManager nugetPackageManager)
                {
                    var packagesDirectory = Path.Combine(WorkingDirectory, "packages");
                    nugetPackageManager = new NuGetPackageManager(packagesDirectory)
                    {
                        Logger = new OpenModNuGetLogger(m_LoggerFactory!.CreateLogger("NuGet"))
                    };
                }

                nugetPackageManager.Logger = new OpenModNuGetLogger(m_LoggerFactory!.CreateLogger("NuGet"));

                await nugetPackageManager.RemoveOutdatedPackagesAsync();
                nugetPackageManager.InstallAssemblyResolver();
                nugetPackageManager.SetAssemblyLoader(Hotloader.LoadAssembly);

                var startupContext = new OpenModStartupContext
                {
                    Runtime = this,
                    LoggerFactory = m_LoggerFactory!,
                    NuGetPackageManager = nugetPackageManager,
                    DataStore = new Dictionary<string, object>()
                };

                var startup = new OpenModStartup(startupContext);
                startupContext.OpenModStartup = startup;

                foreach (var assembly in openModHostAssemblies)
                {
                    startup.RegisterIocAssemblyAndCopyResources(assembly, string.Empty);
                }

                var configFile = Path.Combine(WorkingDirectory, "openmod.yaml");
                if (File.Exists(configFile))
                {
                    var yaml = File.ReadAllText(configFile);
                    var deserializer = new DeserializerBuilder()
                        .WithTypeConverter(new YamlNullableEnumTypeConverter())
                        .WithNamingConvention(CamelCaseNamingConvention.Instance)
                        .Build();

                    var config = deserializer.Deserialize<Dictionary<string, object>>(yaml);

                    var hotReloadingEnabled = true;
                    if (config.TryGetValue("hotreloading", out var unparsed))
                    {
                        switch (unparsed)
                        {
                            case bool value:
                                hotReloadingEnabled = value;
                                break;
                            case string strValue when bool.TryParse(strValue, out var parsed):
                                hotReloadingEnabled = parsed;
                                break;
                            default:
                                m_Logger.LogWarning(
                                    "Unknown config for 'hotreloading' in OpenMod configuration: {UnparsedConfig}",
                                    unparsed);
                                break;
                        }
                    }
                    Hotloader.Enabled = hotReloadingEnabled;

                    var tryInstallMissingDependencies = false;
                    if (config.TryGetValue("nuget", out unparsed) && unparsed is Dictionary<object, object> nugetConfig)
                    {
                        if (nugetConfig.TryGetValue("tryAutoInstallMissingDependencies", out unparsed))
                        {
                            switch (unparsed)
                            {
                                case bool value:
                                    tryInstallMissingDependencies = value;
                                    break;
                                case string strValue when bool.TryParse(strValue, out var parsed):
                                    tryInstallMissingDependencies = parsed;
                                    break;
                                default:
                                    m_Logger.LogWarning(
                                        "Unknown config for 'tryAutoInstallMissingDependencies' in OpenMod configuration: {UnparsedConfig}",
                                        unparsed);
                                    break;
                            }
                        }
                    }

                    PluginAssemblyStore.TryInstallMissingDependencies = tryInstallMissingDependencies;
                }

                await nugetPackageManager.InstallMissingPackagesAsync(updateExisting: true);
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

                Host = hostBuilder.Build();

                m_AppLifeTime = Host.Services.GetRequiredService<IHostApplicationLifetime>();
                m_AppLifeTime.ApplicationStopping.Register(() => { AsyncHelper.RunSync(ShutdownAsync); });

                Status = RuntimeStatus.Initialized;
                LifetimeScope = Host.Services.GetRequiredService<ILifetimeScope>().BeginLifetimeScopeEx(
                    containerBuilder =>
                    {
                        containerBuilder.Register(_ => this)
                            .As<IOpenModComponent>()
                            .SingleInstance()
                            .ExternallyOwned();

                        containerBuilder.RegisterType<ScopedPermissionChecker>()
                            .As<IPermissionChecker>()
                            .InstancePerLifetimeScope()
                            .OwnedByLifetimeScope();
                    });

                DataStore = Host.Services.GetRequiredService<IDataStoreFactory>().CreateDataStore(
                    new DataStoreCreationParameters
                    {
                        Component = this,
                        Prefix = "openmod",
                        Suffix = null,
                        WorkingDirectory = WorkingDirectory
                    });

                try
                {
                    await Host.StartAsync();
                }
                catch (Exception ex)
                {
                    Status = RuntimeStatus.Crashed;
                    m_Logger.LogCritical(ex, "OpenMod has crashed");
                    Log.CloseAndFlush();
                }

                return Host;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task PerformSoftReloadAsync()
        {
            await ShutdownAsync();
            await InitAsync(m_OpenModHostAssemblies!, m_RuntimeInitParameters!, m_HostBuilderFunc);
        }

        private void ConfigureConfiguration(IConfigurationBuilder builder, OpenModStartup startup)
        {
            builder
                .SetBasePath(WorkingDirectory)
                .AddYamlFile("openmod.yaml", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables("OpenMod_");
            startup.ConfigureConfiguration(builder);
        }

        private void SetupServices(IServiceCollection services, OpenModStartup startup)
        {
            services.AddSingleton<IRuntime>(this);
            services.AddSingleton(HostInformation);
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
            LoggerConfiguration loggerConfiguration;
            try
            {
                var loggingPath = Path.Combine(WorkingDirectory, "logging.yaml");
                if (!File.Exists(loggingPath))
                {
                    // First run, logging.yaml doesn't exist yet.
                    // We can't wait for auto-copy from the assembly as it would be too late.
                    using var stream =
                        typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yaml");
                    using var reader =
                        new StreamReader(
                            stream ?? throw new MissingManifestResourceException(
                                "Couldn't find resource: OpenMod.Core.logging.yaml"));

                    var fileContent = reader.ReadToEnd();
                    File.WriteAllText(loggingPath, fileContent);
                }

                var configuration = new ConfigurationBuilder()
                    .SetBasePath(WorkingDirectory)
                    .AddYamlFileEx(s =>
                    {
                        s.Path = "logging.yaml";
                        s.Optional = false;
                        s.Variables = new Dictionary<string, string>
                        {
                            {"workingDirectory", WorkingDirectory.Replace(@"\", @"/")},
                            {"date", DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}
                        };
                        s.ResolveFileProvider();
                    })
                    .AddEnvironmentVariables()
                    .Build();

                loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration);

            }
            catch (Exception ex)
            {
                var previousColor = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Failed to setup Serilog; logging will not work correctly.");
                Console.WriteLine("Setting up console only logging as workaround.");
                Console.WriteLine("Please fix your logging.yaml file or delete it to restore the default one.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);

                Console.ForegroundColor = previousColor;

                loggerConfiguration = new LoggerConfiguration()
                    .WriteTo.Async(c => c.Console());
            }

            var serilogLogger = Log.Logger = loggerConfiguration.CreateLogger();
            m_LoggerFactory = new SerilogLoggerFactory(serilogLogger);
            m_Logger = m_LoggerFactory.CreateLogger<Runtime>();
        }

        public async Task ShutdownAsync()
        {
            IsDisposing = true;
            m_Logger.LogInformation("OpenMod is shutting down...");
            await LifetimeScope.DisposeAsync();
            Host?.Dispose();
            Status = RuntimeStatus.Unloaded;
            Log.CloseAndFlush();
        }
    }
}
