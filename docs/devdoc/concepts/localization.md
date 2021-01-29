# Localization
Localization allows users to customize and translate your plugins messages.  
This is achieved through the translations.yaml file and the IStringLocalizer service.

> [!WARNING]
> The **<RootNamespace\>** and **<AssemblyName\>** properties in the plugins .csproj file must be equal, otherwise the `IStringLocalizer` service will not work.
    
## Adding the translations.yaml
Create a new file called "translations.yaml" inside your project's root directory.  
After that, add the following to your .csproj file: 
```xml
<ItemGroup>
  <EmbeddedResource Include="translations.yaml" />
</ItemGroup>
```

> [!NOTE]
> If you created your plugin project using one of the templates, the translations.yaml file will be already set up.

## Using IStringLocalizer for localization
Assume you want to localize the following command:
```c#
[Command("awesome")]
public class AwesomeCommand : Command
{
    public AwesomeCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {

    }

    public async Task OnExecuteAsync()
    {
        var amount = Context.Parameters.Get<int>(0);

        await PrintAsync($"{Actor.DisplayName} is {amount}x awesome!");
    }
}
```

Adjust the command to use the IStringLocalizer service:
```c#
[Command("awesome")]
public class AwesomeCommand : Command
{
    private readonly IStringLocalizer m_StringLocalizer;

    public AwesomeCommand(
        IStringLocalizer stringLocalizer, // Injects the IStringLocalizer service 
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_StringLocalizer = stringLocalizer;
    }

    public async Task OnExecuteAsync()
    {
        var count = Context.Parameters.Get<int>(0);

        // Prints the localized messages.
        await PrintAsync(m_StringLocalizer["commands:awesome", new { Actor = Actor, Amount = amount }]);
    }
}
```

`commands:awesome` defines the key for the translation. The IStringLocalizer services uses the key as the path to the value in the yaml file. You can use any valid path as key, such as `messages:awesome`, just `awesome`.

`new { Actor = Actor, Amount = amount }` sets the arguments for the message.

Add the translation to the `translations.yaml`:
```yaml
commands: 
  awesome: "{Actor.DisplayName} is {Amount}x awesome!"
```

Notice how we can access the properties of the Actor parameter by calling `Actor.DisplayName`. The IStringLocalizer uses SmartFormat.NET for parsing arguments. See the [SmartFormat.NET wiki](https://github.com/axuno/SmartFormat/wiki) for more information. 