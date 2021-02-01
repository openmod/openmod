# Commands
To create a command, create a class that inherits from one of these:

* Command (for universal plugins)
* UnityEngineCommand (for UnityEngine plugins)
* UnturnedCommand (for Unturned plugins)

In this example, we will make a universal command that works on any game or platform:
```c#
public class CommandAwesome : Command
{
    public CommandAwesome(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
```

Add metadata to describe the command:
```c#
[Command("awesome")] // The primary name for the command. Usually, it is defined as lowercase. 
[CommandAlias("awsm")] // Add "awsm" as alias.
[CommandAlias("aw")] // Add "aw" as alias.
[CommandDescription("My awesome command")] // Description. Try to keep it short and simple.
public class CommandAwesome : Command
{
    public CommandAwesome(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
```

Implement OnExecuteAsync:
```c#
[Command("awesome")] 
[CommandAlias("awsm")]
[CommandAlias("aw")]
[CommandDescription("My awesome command")]
public class CommandAwesome : Command
{
    public CommandAwesome(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    // use UniTask instead of Task if derivered from UnityEngineCommand or UnturnedCommand
    public async Task OnExecuteAsync() 
    {
        await PrintAsync("OpenMod is awesome!");
    }
}
```

> [!WARNING]
> **Avoid** hardcoding messages like in the example above. Instead, use [localization](localization.md) so users can customize and translate the messages.

## Parameters
Most commands usually require parameters. The command context provides a Parameter property for this:  
```c#
public async Task OnExecuteAsync()
{
    // assume we want the command to be called like this: /awesome <player> <amount>
    // Parameters start from 0, so <player> index is 0, <amount> index is 1.
    var player = await Context.Parameters.GetAsync<string>(0);
    var amount = await Context.Parameters.GetAsync<int>(1);
    await PrintAsync($"{player} is {amount}x awesome!");
}
```

After that, you need to describe how users can use the command. Add the syntax metadata to the command class:
```c#
//...
[CommandSyntax("<player> [amount]")] // Describe the syntax/usage. Use <> for required arguments and [] for optional arguments.
public class CommandAwesome : Command
//...
```

## Restricting Command Actors
If you are not developing universal plugins, you may want to limit who can execute commands.  
The `[CommandActor(Type)]` attribute allows you to specify such restrictions.

For example, if you want to restrict a commands usage to UnturnedUser and ConsoleActor, you can add the following:
```c#
//...
[CommandActor(typeof(UnturnedUser))]
[CommandActor(typeof(ConsoleActor))]
public class CommandAwesome : Command
//...
```

> [!WARNING]
>  By default, any user type can execute your command. Plugins can dynamically add new users such as DiscordUser, PluginUser, RconUser etc. Write your commands in a way that works with all users. If that is not possible (e.g. a /god command that requires UnturnedUser), restrict the allowed actors like mentioned above.

> [!CAUTION]
>  **Do not** manually check if an actor is allowed to execute a command (e.g. `if(!(actor is UnturnedUser))`). Always use `[CommandActor(Type)]` for such restrictions. It will automatically hide the command from actors who cannot execute them and give a consistent error message. 

## Exceptions
Exceptions derived from `UserFriendlyException` are automatically caught by OpenMod and the message is displayed to the user in a user-friendly way.  

These built-in user-friendly exceptions available: 

* **NotEnoughPermissionException**, can be thrown if the user does not have enough permission to execute an action.
* **CommandWrongUsageException**, can be thrown on wrong command usage. Displays correct usage based on command syntax.
* **CommandIndexOutOfRangeException**, thrown automatically by Parameters.Get if the given index is bigger than the arguments length.
* **CommandParameterParseException**, thrown automatically by Parameters.Get if the parameter could not be parsed to the expected type.
* **CommandNotFoundException**, thrown automatically if a command was not found.

```c#
public async Task OnExecuteAsync()
{
    var player = await Context.Parameters.GetAsync<string>(0);
    var amount = await Context.Parameters.GetAsync<int>(1);

    if(amount < 1) 
    {
        throw new UserFriendlyException("Amount cannot be negative!");
    }

    await PrintAsync($"{player} is {amount}x awesome!");
}
```

## Command Permissions
By design and for consistency reasons, you cannot define a command permission manually. OpenMod will automatically assign a permission to the command instead. You can use the `help <yourcommand>` command to figure out what the base permission for your command is.

Assume you want to restrict the `awesome` command if the amount is more than 10. This is how you would do it:
```c#
public async Task OnExecuteAsync()
{
    var player = await Context.Parameters.GetAsync<string>(0);
    var amount = await Context.Parameters.GetAsync<int>(1);

    if(amount > 10 && await CheckPermissionAsync("MoreThan10") != PermissionGrantResult.Grant) 
    {
        throw new NotEnoughPermissionException(this, "MoreThan10"); // displays an error message to the user 
    }

    await PrintAsync($"{player} is {amount}x awesome!");
}
```

> [!CAUTION]
> **Do not** manually check for general command usage permissions. OpenMod automatically defines permissions for commands.
> A bad example would be:
> ```c#
> public async Task OnExecuteAsync()
> {
>     if (!await CheckPermissionAsync("MyCommand")) 
>     {
>        //throw ... or even worse await PrintAsync(...) 
>     }
> }
> ```

> [!CAUTION]
> **Do not** manually send missing permission messages. *Always* throw NotEnoughPermissionException instead. 

## Adding child commands
You can add child commands to a command by using the `[CommandParent]` attribute. This allows OpenMod to discover your child commands and provide additional help and tab autocompletion.

The following command will execute when a user types "/awesome more". The `CommandAwesome.OnExecuteAsync` method will not execute in this case.
```c#
[Command("more")] 
[CommandDescription("My more awesome command")]
[CommandParent(typeof(CommandAwesome))] // set "awesome" as parent.
public class CommandAwesomeMore : Command
{
    public CommandAwesomeMore(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task OnExecuteAsync()
    {
        // Note: Parameters do not include "more".
        // If you type "/awesome more a", Context.Parameters[0] will be equal to "a". 
        await PrintAsync("You are even more awesome!");
    }
}
```

> [!CAUTION] 
> **Do not** handle child commands yourself (e.g. `if(Context.Parameters[0] == "add")`). When manually handling child commands, OpenMod can't discover your child commands and provide additional help, permissions or tab autocompletion. 

## Adding child permissions
You can add child permissions for your commands, they need to be registered using `[RegisterCommandPermission("child.permission")]`,
to check if the user has the permission you will just need to call `await CheckPermissionAsync("child.permission")`and check the result.

Example:
```c#
[Command("more")] 
[RegisterCommandPermission("child.permission", Description = "My awesome permission description", DefaultGrant = PermissionGrantResult.Default)]
//Description and DefaultGrant are optional

public class CommandAwesomeMore : Command
{
    public CommandAwesomeMore(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public async Task OnExecuteAsync()
    {
        //MyPlugin:AwesomeMore.child.permission
        if (await CheckPermissionAsync("child.permission") == PermissionGrantResult.Grant)
        {
            //User have the child permission
        }
        else
        {
            //User do not have the child permission
        }
    }
}
```