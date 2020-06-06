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
using OpenMod.API;
using OpenMod.Core.Helpers;
using Semver;
using Serilog;

namespace OpenMod.Runtime
{
    [UsedImplicitly]
    public sealed class Runtime : IRuntime, IOpenModComponent
    {
        public string WorkingDirectory { get; private set; }
        public string[] CommandlineArgs { get; private set; }

        private IHost m_Host;

        public void Init(ICollection<Assembly> openModAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            var thread = new Thread(o => { InitAsync(openModAssemblies, hostBuilder, parameters).Forget(); });
            thread.Start();
        }

        public async Task InitAsync(ICollection<Assembly> openModHostAssemblies, IHostBuilder hostBuilder, RuntimeInitParameters parameters)
        {
            WorkingDirectory = parameters.WorkingDirectory;
            CommandlineArgs = parameters.CommandlineArgs;

            SetupSerilog();
            Log.Information("OpenMod is starting...");

            try
            {

                if (!Directory.Exists(parameters.WorkingDirectory))
                {
                    Directory.CreateDirectory(parameters.WorkingDirectory);
                }


                hostBuilder
                    .UseContentRoot(parameters.WorkingDirectory)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureAppConfiguration(builder =>
                    {
                        SetupConfiguration(builder, parameters.CommandlineArgs, "openmod.yml", parameters.WorkingDirectory);
                    })
                    .ConfigureContainer<ContainerBuilder>(builder => SetupContainer(builder, openModHostAssemblies))
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<OpenModHostedService>();
                        services.AddOptions();
                    })
                    .UseSerilog();

                m_Host = hostBuilder.Build();
                await m_Host.RunAsync();
                Log.Information("OpenMod has shut down.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "OpenMod has crashed.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private void SetupContainer(ContainerBuilder containerBuilder, ICollection<Assembly> openModHostAssemblies)
        {
            containerBuilder.Register(context => this)
                .As<IRuntime>()
                .SingleInstance();

            var registrator = new ContainerRegistrator(this, containerBuilder);

            // register from OpenMod.Core, should prob. use a different class than AsyncHelper
            registrator.RegisterServicesFromAssembly(typeof(AsyncHelper).Assembly);

            // register from assemblies such as OpenMod.UnityEngine, OpenMod.Unturned, etc.
            foreach (var assembly in openModHostAssemblies)
            {
                registrator.RegisterServicesFromAssembly(assembly);
            }
            
            AsyncHelper.RunSync(async () =>
            {
                await registrator.BootstrapAndRegisterPluginsAsync();
            });

            registrator.Complete();
        }

        private void SetupSerilog()
        {
            var loggingPath = Path.Combine(WorkingDirectory, "logging.yml");
            if (!File.Exists(loggingPath))
            {
                // First run, logging.yml doesn't exist yet. We can not wait for auto-copy as it would be too late.
                using (Stream stream = typeof(AsyncHelper).Assembly.GetManifestResourceStream("OpenMod.Core.logging.yml"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string fileContent = reader.ReadToEnd();
                    File.WriteAllText(loggingPath, fileContent);
                }
            }
            
            var builder = new ConfigurationBuilder();
            SetupConfiguration(builder, CommandlineArgs, "logging.yml", WorkingDirectory);
            var configuration = builder.Build();

            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration);

            Log.Logger = loggerConfiguration.CreateLogger();
        }

        private void SetupConfiguration(IConfigurationBuilder configBuilder, string[] commandlineArgs, string file, string basePath)
        {
            configBuilder
                .SetBasePath(basePath)
                .AddYamlFile(file, true)
                .AddCommandLine(commandlineArgs)
                .AddEnvironmentVariables();
        }

        public async Task ShutdownAsync()
        {
            await m_Host.StopAsync();
        }

        public SemVersion Version { get; } = new SemVersion(0, 1, 0);

        public string OpenModComponentId { get; } = "OpenMod.Runtime";

        public bool IsComponentAlive { get; } = true;
    }
}
