# Services and dependency injection

OpenMod, like other modern .NET based frameworks, uses the [dependency injection pattern](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection). This guide aims to simplify it and explain what it means for plugin developers using OpenMod.

Plugins, commands, event listeners, and services can automatically get references to plugins or any other services provided by OpenMod just by adding their interfaces to the constructor. See the example below.

## Dependency injection example
Let's see this in action:

This is how your plugin looks like normally:
```c#
public class MyPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
{

}
```

If you wanted to access the `IStringLocalizer` service, you could it like this:
```c#
public class MyPlugin(IStringLocalizer stringLocalizer, IServiceProvider serviceProvider) : base(serviceProvider)
{
   // do something with stringLocalizer
}
```

Assume you want to access your plugins instance and configuration from a command. Here is how you could do it:
```c#
private readonly IConfiguration m_Configuration;
private readonly MyPlugin m_MyPlugin;

public EchoCommand(
    IServiceProvider serviceProvider, 
    MyPlugin myPlugin,
    IConfiguration configuration) : base(serviceProvider)
    m_MyPlugin = myPlugin;
    m_Configuration = configuration;
}
```

## Registering your own services
There are two ways to register a service:

1. Registering by using the`[Service]` attribute for the interface and `[PluginServiceImplementation]` for the concrete class.
2. Registering manually by implementing the `IServiceConfigurator` or `IContainerConfigurator` interfaces. Classes which implement these interfaces are automatically instantiated when the IoC container is configured and can be used to configure the container directly. 

You can implement the `IDisposable` or the `IAsyncDisposable` interface for cleaning up resources when OpenMod or your plugin unloads. 

Here is an example service to clear vehicles:
```c#
[Service]
public interface IVehicleClearingService
{
    Task ClearVehiclesAsync();
}

[PluginServiceImplementation(Lifetime = ServiceLifetime.Transient)]
public class VehicleClearingService : IVehicleClearingService, IAsyncDisposable
{
    private readonly IStringLocalizer m_StringLocalizer;
    private readonly ILogger<VehicleClearingService> m_Logger;
    public VehicleClearingService(
        ILogger<VehicleClearingService> logger, 
        IPluginAccessor<VehicleClearerPlugin> pluginAccessor)
    {
        VehicleClearerPlugin plugin = pluginAccessor.Instance;

        // Services live in the global OpenMod scope, which does not provide an IStringLocalizer.
        // Since IStringLocalizer does not exist in this scope, we have to use the plugins scope.
        m_StringLocalizer = plugin.Lifetime.Resolve<IStringLocalizer>();
        m_Logger = logger;
    }

    public async Task ClearVehiclesAsync() 
    {
        m_Logger.LogInformation(m_StringLocalizer["messages:clearing_vehicles"]); // translation is read from the plugins translation
        // call game apis to clear vehicles...
    }

    // Service dispose methods are called on OpenMod reload or server shutdown 
    public async ValueTask DisposeAsync()
    {
        await ClearVehiclesAsync(); // ensure vehicles get cleared on reload or shutdown
    }
}
```

You can now access this service by injecting `IVehicleClearingService` in e.g. commands, event listeners, your plugin class, or other services. 

### Service lifetime
You might have noticed that the VehicleClearingService's Lifetime was set to `Lifetime = ServiceLifetime.Transient`.  

The following lifetimes are available:

* **Transient** - The service gets recreated every time it gets resolved. Every resolution and injection of this service will have a unique instance. This is the default lifetime.
* **Scoped** - The service will share the same instance across the same command or event.
* **Singleton** - The service will only have one instance that lives as long as the OpenMod IoC container does (until OpenMod gets reloaded or the server shuts down).  

> [!WARNING]
> **Avoid** making singleton services. This may cause problems if your service has transient or scoped dependencies.

# Service scope
Services can have to scopes: plugin scope and global scope.

- **Plugin scope** services only used within the defining plugin itself and can not be accessed by other plugins. They can also not override global scope services for OpenMod itself or for other plugins. Plugin scope services are implemented using the [PluginServiceImplementation] attribute. Plugin scope services are constructed when your plugin loads and are disposed when your plugin shuts down.  

- **Global scope** services are used for all plugins and OpenMod itself. Global scope services are implemented using the [ServiceImplementation] attribute. If you want to replace e.g. the IStringLocalizer for everyone, you need to implement it in the global scope. Global scope services are disposed when OpenMod reloads or the server shuts down. Global scope services are constructed when your OpenMod loads and are disposed your OpenMod shuts down.

> [!WARNING]
> **Avoid** depending on your plugin when implementing global scope services. Only create dependencies to other global scope services. As global scope services have a different lifecycle than plugins, you can not inject your plugin instance. A workaround is to inject Lazy<IPluginAccessor<YourPlugin\>\> and resolve at a later time. Keep in mind that even if your plugin fails to load or unloads, your global scope service will still stay alive.

## Built-in OpenMod services

| **Service**                                     | **Description**                                        |
|-------------------------------------------------|--------------------------------------------------------|
| IConfiguration                                  | Provides values from configuration files               |
| ICommandContextBuilder                          | Builds command contexts                                |
| ICommandExecutor                                | Execute command                                        |
| ICommandStore                                   | Provides command registrations                         |
| ICommandPermissionBuilder                       | Defines a command's permission                         |
| ICommandParameterResolver                       | Parses command parameters from strings                 |
| ICurrentCommandContextAccessor                  | Accesses the current command context                   |
| IDataStore                                      | Serializes and deserializes persistent data            |
| IDataStoreFactory                               | Creates data stores                                    |
| IEventBus                                       | Subscribes to events and emits them                    |
| IJobScheduler                                   | Schedules and manages jobs                             |
| ILogger<T\>                                     | Provides logging for the T class                       |
| ILoggerFactory                                  | Creaters loggers                                       |
| IOpenModStringLocalizer                         | Localizes messages from OpenMod's own translation file |
| IOpenModDataStoreAccessor                       | Accesses OpenMod's own data store                      |
| IOpenModHost                                    | OpenMod host platform abstractions                     | 
| IPermissionChecker                              | Checks permissions                                     |
| IPermissionRoleStore                            | Stores permission roles                                |
| IPermissionRegistry                             | Registers and stores permissions                       |
| IPluginAccessor<TPlugin\>                       | Accesses a plugins instance                           |
| IPluginActivator                                | Loads and activates plugins                            |
| IRuntime                                        | Creates and hosts the OpenMod runtime                  |
| IStringLocalizer                                | Localizes messages from plugin translation files       |
| IUserDataSeeder                                 | Seeds user data on first connect                       |
| IUserDataStore                                  | Stores and manages user data                           |
| IUserManager                                    | Manages users                                          |
