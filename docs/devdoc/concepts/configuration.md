# Configurations
Configurations allow users to configure and customize your plugins behavior.  
Assume your plugin gives XP when killing a player. By using configs, a user can dynamically set how much XP will be given. 

OpenMod configurations are based on [Microsoft.Extensions.Configuration](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.configuration?view=dotnet-plat-ext-3.1) which is also used in ASP.NET Core.

> [!WARNING]
> The **<RootNamespace\>** and **<AssemblyName\>** properties in the plugins .csproj file must be equal, otherwise the `IConfiguration` service will not work.

## Adding the config.yaml
Create a new file called "config.yaml" inside your project's root directory.  
After that, add the following to your .csproj file: 
```xml
<ItemGroup>
  <EmbeddedResource Include="config.yaml" />
</ItemGroup>
```

> [!NOTE]
> If you created your plugin project using one of the templates, the comfig.yaml file will be already set up.

## Reading from the configuration file
You can use the `IConfiguration` service to read the config.yaml file. You can inject it like this:  
```c#
public class MyPlugin : OpenModUniversalPlugin
{
    private readonly IConfiguration m_Configuration;

    public MyPlugin(IConfiguration configuration, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_Configuration = configuration;
    }

    public async Task OnLoadAsync()
    {
        // ...
    }
}
```

Assume your configuration looks like this:
```yaml
plugin_load_delay: 500
```

Then this is how you would read the value:
```c#
public async Task OnLoadAsync()
{
  var delay = m_Configuration.GetSection("plugin_load_delay").Get<int>();
  await Task.Delay(delay);
}
```

You can also have nested values:
```yaml
plugin_load:
  actions:
    wait_delay: 500
```

```c#
public async Task OnLoadAsync()
{
  // notice how ":" is used to access nested values
  var delay = m_Configuration.GetSection("plugin_load:actions:wait_delay").Get<int>();
  await Task.Delay(delay);
}
```

If you want to access strings, you can also use the indexer:
```yaml
players:
  owner: "Trojaner"
```

```c#
public async Task OnLoadAsync()
{
  // reading strings is even easier
  string owner = m_Configuration["players:owner"];
}
```

> [!WARNING]
> Configuration files are not meant store data, that's why there are no setter or save methods. See [persistence](persistence.md) if you need to store and load data.

> [!CAUTION]
> **Do not** cache data read from configurations. This will break dynamic reloading of configuration files.

## Converting configurations to C# classes
```c#
public async Task OnLoadAsync()
{
  var config = new MyConfigClass();  
  m_Configuration.Bind(config);
  // or if you only want to bind a subset:
  // m_Configuration.GetSection("something").Bind(config);
}
```

> [!NOTE]
> You can cache the config variable here, bind will automatically update the values if the config reloads.

# Listening to configuration changes
To listen to changes, you can use [change tokens](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/change-tokens).

Get the reload token of the configuration:
```c#
var changeToken = m_Configuration.GetReloadToken();
```

Add a new event listener:
```c#
var listener = ChangeToken.OnChange(() => reloadToken, () =>
{
    m_Logger.LogInformation("Configuration file has been reloaded!");
})
```

You can use listener.Dispose() to stop listening.

> [!CAUTION]
> **Do not** use config change listeners to fetch latest configuration values. OpenMod does this automatically. IConfiguration.Get will always return the current configuration values. 

## Adding your own configuration sources
You can add additional configuration sources by implementing the `IConfigurationConfigurator` interface.  
This interface allows you to add additional sources to the `IConfigurationBuilder` used when building the OpenMod config.

> [!WARNING]
> Custom configuration sources for plugins [are not supported yet](https://github.com/openmod/openmod/issues/90).

## Best Practices
> [!CAUTION]
> **Do not** hardcode important values. If you are making a plugin that gives players XP when killing other players, make sure the XP amount is configurable, as it is something many users may want to adjust.  

> [!CAUTION]
> **Do not** overcomplicate your configurations. Only add values that users are likely going to change. Having a simple configuration is preferred to a complex one.  
  
> [!CAUTION]
> **Do not** use configurations for customizable messages. Use [localization](localization.md) instead. Unlike configurations, localization supports formatting and passing arguments.