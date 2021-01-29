# Logging

OpenMod uses the `Microsoft.Extensions.Logging` package for logging abstractions and [Serilog](https://serilog.net/) as the logging implementation for it.

For more, check out the [ILogger Interface documentation on docs.microsoft.com](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.ilogger?view=dotnet-plat-ext-3.1). 

You can get a logger instance by injecting it:
```c#
public class MyPlugin : OpenModUniversalPlugin
{
    private readonly ILogger<MyPlugin> m_Logger; 

    public MyPlugin(ILogger<MyPlugin> logger, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_Logger = logger;
        m_Logger.LogInformation("Hello world!");
    }
}
```

The generic part (`T` in `ILogger<T>`) must be the class that is using the logger.

## Implementing your own logger
To implement your own logger, you must implement the `ILoggerFactory` and `ILogger<>` services.  
After that you must register them via a [ServiceConfigurator](services.md):
```c#
public class ServiceConfigurator : IServiceConfigurator
{
    public void ConfigureServices(IOpenModStartupContext openModStartupContext, IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ILoggerFactory, MyLoggerFactory>();
        serviceCollection.AddTransient(typeof(ILogger<>), typeof(MyLogger<>)(); // must be transient
    }
}
```

> [!WARNING]
> Custom logger are used after OpenMod has built the IoC container. Early boot messages will not show on custom loggers.