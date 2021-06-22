# Databases
The OpenMod.EntityFrameworkCore libraries provide functionality for using Microsoft's Entity Framework Core to load and save information from multiple types of databases.

Currently supported providers:
- OpenMod.EntityFrameworkCore.MySql - MySQL/MariaDB databases.

For more information on Entity Framework Core, [read Microsoft's documentation](https://docs.microsoft.com/en-us/ef/core/). A basic overview of OpenMod's implementation is written here.

## Creating your first DbContext

The DbContext houses all of your database sets which in turn hold all your information.

In this example, we will create a database that will allow us to record every time a user connects and we will be using the MySql provider.

To create your first DbContext, you must create a class which inherits OpenModDbContext and add constructors for both constructors of the base class:

```cs
public class UserConnectionDbContext : OpenModDbContext<UserConnectionDbContext>
{
    public UserConnectionDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public UserConnectionDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
    {
    }
}
```

After creating this class, you must register it as a database context within your plugin. To do this, add a plugin container configurator and register the database context in its configure method.
This class can be placed anywhere but is usually in the root namespace.

```cs
public class PluginContainerConfigurator : IPluginContainerConfigurator
{
    public void ConfigureContainer(IPluginServiceConfigurationContext context)
    {
		    // You can extend how your database context works by using the overloads of this method.
        context.ContainerBuilder.AddMySqlDbContext<UserConnectionDbContext>();
    }
}
```

By default, OpenMod will get the connection string for your MySql database from your config.yaml file. Ensure this is in your config file:

```yaml
database:
  ConnectionStrings:
    default: "Server=127.0.0.1; Database=openmod; Port=3306; User=root; Password=password"
```

## Create your model

For more detail on creating models, refer to [Microsoft's EF Core Documentation on modeling](https://docs.microsoft.com/en-us/ef/core/modeling/).

Simply begin by creating a class with all the fields you want to record.

```cs
public class UserConnection
{
    // The primary key used to identify each connection.
	  public uint ConnectionId { get; set; }
    
	  // The user's ID.
    public string UserId { get; set; } = "";
	
    // The user's type.
    public string UserType { get; set; } = "";

    // The date/time of this connection record.
    public DateTime ConnectionTime { get; set; }
}
```

This class needs a primary key to identify each model and this primary key (ConnectionId) should also be automatically generated. We can configure this in two ways:
- **Data Annotations** - Simply adding the `[Key]` attribute to our `UserConnection` class:
  ```cs
  public uint ConnectionId { get; set; }
  ```
  becomes
  ```cs
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public uint ConnectionId { get; set; }
  ```
- **Fluent API** - Overriding the `OnModelCreating` method within our `UserConnectionDbContext` class and configuring the primary key there:
  ```cs
  protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
			base.OnModelCreating(modelBuilder);
	
	    modelBuilder.Entity<UserConnection>()
			    .HasKey(x => x.ConnectionId);
          
      modelBuilder.Entity<UserConnection>()
          .Property(x => x.ConnectionId)
          .ValueGeneratedOnAdd();
	}
	```
  The purpose of Fluent API, although more complicated, allows greater control over how models are configured.

## Add your model to your DB context

Now that you have your DB context (`UserConnectionDbContext`) and your model (`UserConnection`), linking the two is very easy. All that is needed is to define a new property in your DB context as follows:

```cs
public DbSet<UserConnection> UserConnections => Set<UserConnection>();
```

We will use this property to manage our user connection records.

For a recap, our classes now look like this (assuming Fluent API is being used):

**UserConnection.cs**
```cs
public class UserConnection
{
    // The primary key used to identify each connection.
	  public uint ConnectionId { get; set; }
    
	  // The user's ID.
    public string UserId { get; set; } = "";
	
    // The user's type.
    public string UserType { get; set; } = "";

    // The date/time of this connection record.
    public DateTime ConnectionTime { get; set; }
}
```

**UserConnectionDbContext.cs**
```cs
public class UserConnectionDbContext : OpenModDbContext<UserConnectionDbContext>
{
    public UserConnectionDbContext(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public UserConnectionDbContext(IDbContextConfigurator configurator, IServiceProvider serviceProvider) : base(configurator, serviceProvider)
    {
    }
    
    public DbSet<UserConnection> UserConnections => Set<UserConnection>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserConnection>()
            .HasKey(x => x.ConnectionId);

        modelBuilder.Entity<UserConnection>()
            .Property(x => x.ConnectionId)
            .ValueGeneratedOnAdd();
    }
}
```

## Creating a migration

For more detail on migrations, refer to [Microsoft's EF Core Documentation on migrations](https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/).

Migrations allow EF Core to manage the creation and modifications of the database automatically. An initial migration must be created to allow EF Core to create the database for us.

Creating migrations is very simple:

- Add a NuGet reference to the `Microsoft.EntityFrameworkCore.Tools` package (latest 3.x version - 3.1.16 at the time of writing this).
  In Microsoft Visual Studio's Package Manager: `Install-Package Microsoft.EntityFrameworkCore.Tools -Version 3.1.16`
  In Visual Studio Code's CLI (or command line): `dotnet add package Microsoft.EntityFrameworkCore.Tools --version 3.1.16`
  
- Create a database context factory class (this allows EF core to create an instance of the database context without needing to run the entire OpenMod server):
  ```cs
  public class UserConnectionDbContextFactory : OpenModMySqlDbContextFactory<UserConnectionDbContext>
  {
  }
  ```

- Use the tools package to create the migration (replace MigrationName with your desired migration name, for this sample we'll use `Initial`):
  In Microsoft Visual Studio's Package Manager: `Add-Migration MigrationName`
  In Visual Studio Code's CLI (or command line): `dotnet ef migrations add MigrationName`
  
- Migrate at plugin load by adding this to your plugin's load method (where `m_DbContext` is an instance of your database context):
  ```cs
  await m_DbContext.Database.MigrateAsync();
  ```

To add migrations in the future, you only need to run the `Add-Migration` (or `dotnet ef migrations add`) command.

## Saving data

For more detail on saving, refer to [Microsoft's EF Core Documentation on saving data](https://docs.microsoft.com/en-us/ef/core/saving/).

Every time a user connects, we want to record this to a database. We can subscribe to `IUserConnectionEvent` for this. For more information on events, see the [Events documentation](events.md).

As we registered our DB context in our plugin container configurator, we can simply resolve our DB context from our constructor.

The base code for our event listener is as follows:

```cs
public class UserConnectedEventListener : IEventListener<IUserConnectedEvent>
{
    private readonly UserConnectionDbContext m_DbContext;

    public UserConnectedEventListener(UserConnectionDbContext dbContext)
    {
        m_DbContext = dbContext;
    }

    public async Task HandleEventAsync(object? sender, IUserConnectedEvent @event)
    {
        // database logic
    }
}
```

To insert a record into our database:
- Create an instance of the record.
  ```cs
  UserConnection userConnection = new UserConnection
  {
      UserId = @event.User.Id,
      UserType = @event.User.Type,
      ConnectionTime = DateTime.UtcNow
  };
  ```
  > [!NOTE]
  > We do not set `ConnectionId` as we configured the database to automatically generate this value for us.

- Add this instance to the database set.
  ```cs
  await m_DbContext.UserConnections.AddAsync(userConnection);
  ```

- Save our changes:
  ```cs
  await m_DbContext.SaveChangesAsync();
  ```

Our entire `UserConnectedEventListener` class will now look like:

```cs
public class UserConnectedEventListener : IEventListener<IUserConnectedEvent>
{
    private readonly UserConnectionDbContext m_DbContext;

    public UserConnectedEventListener(UserConnectionDbContext dbContext)
    {
        m_DbContext = dbContext;
    }

    public async Task HandleEventAsync(object? sender, IUserConnectedEvent @event)
    {
        UserConnection userConnection = new UserConnection
        {
            UserId = @event.User.Id,
            UserType = @event.User.Type,
            ConnectionTime = DateTime.UtcNow
        };

        await m_DbContext.UserConnections.AddAsync(userConnection);

        await m_DbContext.SaveChangesAsync();
    }
}
```

## Querying data

For more detail on querying data, refer to [Microsoft's EF Core Documentation on querying](https://docs.microsoft.com/en-us/ef/core/querying/).

For this sample, we'll create a /lastconnect command that responds with the last time a user connected. For more information on commands, see the [Commands documentation](commands.md).

The base code for our command is as follows:
```cs
[Command("lastconnect")]
public class LastConnectCommand : Command
{
    private readonly UserConnectionDbContext m_DbContext;

    public LastConnectCommand(UserConnectionDbContext dbContext,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_DbContext = dbContext;
    }

    protected override async Task OnExecuteAsync()
    {
        string userId = await Context.Parameters.GetAsync<string>(0);
        string userType = await Context.Parameters.GetAsync<string>(1);

        // database logic
    }
}
```

To get the latest connection, we can use the methods of `m_DbContext.UserConnections` to get the connection we're looking for.

```cs
UserConnection? lastConnection = await m_DbContext.UserConnections
    .Where(x => x.UserId == userId && x.UserType == userType) // Filter by only the target user
    .OrderByDescending(x => x.ConnectionTime)                 // Order by descending connection time (latest to earliest connections)
    .FirstOrDefaultAsync();                                   // Execute the query
```

If the user has never connected, `lastConnection` will be equal to `null`.

We can then output the last connection to the command actor, resulting in the command class of:

```cs
[Command("lastconnect")]
[CommandSyntax("<user id> <user type>")]
public class LastConnectCommand : Command
{
    private readonly UserConnectionDbContext m_DbContext;

    public LastConnectCommand(UserConnectionDbContext dbContext,
        IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_DbContext = dbContext;
    }

    protected override async Task OnExecuteAsync()
    {
        var userId = await Context.Parameters.GetAsync<string>(0);
        var userType = await Context.Parameters.GetAsync<string>(1);

        UserConnection? userConnection = await m_DbContext.UserConnections
            .Where(x => x.UserId == userId && x.UserType == userType)
            .OrderByDescending(x => x.ConnectionTime)
            .FirstOrDefaultAsync();

        if (userConnection == null)
        {
            await PrintAsync("This user has never connected.");
        }
        else
        {
            await PrintAsync($"Last connection: {userConnection.ConnectionTime}");
        }
    }
}
```
