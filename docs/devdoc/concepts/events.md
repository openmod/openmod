# Events and event listeners
Events are used to notify components that something is happening, like a user disconnecting.  

There are two types of events:

* OpenMod Events
* C# Events

This guide will be about OpenMod events.

## Subscribing to events
There are two ways to subscribe to events:

1. Implement the `IEventListener` interface:
```c#
public class UserConnectListener : IEventListener<UserConnectedEvent>
{
    [EventListener(Priority = EventListenerPriority.Lowest)]
    public async Task HandleEventAsync(object sender, UserConnectEvent @event)
    {
        // do something
    }
}
```
2. Use the subscribe methods from the `IEventBus` service:
```c#
public class MyPlugin : OpenModUniversalPlugin
{
    private readonly IEventBus m_EventBus;

    public MyPlugin(IEventBus eventBus, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        m_EventBus = eventBus;
    }

    public async Task OnLoadAsync()
    {
        m_EventBus.Subscribe(this, (sender, @event) => {
            // do something
        });
    }
}
```

> [!Note]
> All OpenMod event listeners are automatically unsubscribed when your plugin unloads. You do not have to unsubscribe them manually on unload.

## Event listener priority and execution order
OpenMod allows you to control in which order your event listeners are executed. Execution order is based on priority. 

You can set an event listeners priority by using the `[EventListener]` attribute.    
Execution order is from lowest priority to highest. In other words, lowest priority is called first.

```c#
public class UserConnectListener1 : IEventListener<UserConnectedEvent>
{
    [EventListener(Priority = EventListenerPriority.Lowest)]
    public async Task HandleEventAsync(object sender, UserConnectEvent @event)
    {

    }
}

public class UserConnectListener2 : IEventListener<UserConnectedEvent>
{
    [EventListener(Priority = EventListenerPriority.Low)]
    public async Task HandleEventAsync(object sender, UserConnectEvent @event)
    {

    }
}

public class UserConnectListener3 : IEventListener<UserConnectedEvent>
{
    [EventListener(Priority = EventListenerPriority.High)]
    public async Task HandleEventAsync(object sender, UserConnectEvent @event)
    {

    }
}
```

In the example above, `UserConnectListener1` is called first, then `UserConnectListener2` and finally `UserConnectListener3`.

## Cancelling events and ignoring cancelled events
An event has to implement the `ICancellableEvent` interface to be cancellable. If an event gets cancelled, event listeners which do not have the `IgnoreCancelled` property in the `[EventListener]` attribute set to true will not be notified. 

`UserConnectingEvent` is such a cancellable event. It will disconnect the connecting user if the event gets cancelled.

```c#
public class UserConnectingListener1 : IEventListener<UserConnectingEvent>
{
    [EventListener(Priority = EventListenerPriority.Lowest)]
    public async Task HandleEventAsync(object sender, UserConnectingEvent @event)
    {
        if(user.DisplayName.Equals("Trojaner"))
        {
            @event.IsCancelled = true;
        }
    }
}

public class UserConnectingListener2 : IEventListener<UserConnectingEvent>
{
    [EventListener(Priority = EventListenerPriority.Low)]
    public async Task HandleEventAsync(object sender, UserConnectingEvent @event)
    {
        // this event listener will not be called because it does not ignore cancellation
    }
}

public class UserConnectingListener3 : IEventListener<UserConnectingEvent>
{
    [EventListener(Priority = EventListenerPriority.High, IgnoreCancelled = true)]
    public async Task HandleEventAsync(object sender, UserConnectingEvent @event)
    {
        // this event listener will be called even if the event gets cancelled
    }
}
```

In the example above, if a user named "Trojaner" connects, `UserConnectingListener1` will cancel the event. `UserConnectingListener2` will not be called in this case because it does not ignore cancelled events like `UserConnectingListener3` does. 

## Event listener lifetime
Event listeners can have three types of lifetime:

* **Transient** - The event listener is always be recreated on every event. If you have multiple IEventListeners, all of them will have their own instances. This is the default lifetime.
* **Scoped** - If you implement multiple IEventListeners in one class, all of them will share the same instance. Otherwise same as transient.
* **Singleton** - The event listener will have only one shared lifetime that lives until the plugin gets unloaded.

You can set the event listener lifetime by adding the `[EventListenerLifetime(ServiceLifetime)]` attribute:
```c#
[EventListenerLifetime(ServiceLifetime.Transient)]
public class UserConnectBroadcaster : IEventListener<UserConnectedEvent>
// ...
```

## Custom events
Creating a custom event is simple: just create a new class that inherits from `Event`.

Here is an example:
```c#
public class SampleEvent : Event
{
    public int MyValue { get; set; } 
    // you can also add other properties 
}
```

You can then emit it by using the event bus:
```c#
MyPlugin myPlugin = ...;
IEventBus eventBus = ...;
ILogger<xxx> logger = ...;

var @event = new SampleEvent
{
    MyValue = 20
};
   
await m_EventBus.EmitAsync(myPlugin, this /* sender */, @event);
logger.LogInformation($"Event value: {@event.MyValue}");
```

If you want your event to be cancellable, you must implement the `ICancellableEvent` interface:
```c#
public class SampleEvent : Event, ICancellableEvent
{
    public int MyValue { get; set; }
    public bool IsCancelled { get; set; }
}
``` 

```c#
MyPlugin myPlugin = ...;
IEventBus eventBus = ...;
ILogger<xxx> logger = ...;
   
var @event = new SampleEvent
{
    MyValue = 20
};
   
await m_EventBus.EmitAsync(myPlugin, this /* sender */, @event);

if(@event.IsCancelled)
{
    logger.LogInformation($"Event has been cancelled!");
    return;
}

logger.LogInformation($"Event value: {@event.MyValue}");
```

> [!CAUTION]
> **Do not** forget to unsubscribe from C# events and delegates when your plugin unloads or a service gets disposed. For example, if you want to subscribe to the `onEnemyConnected` event from Unturned when your plugin loads, you must also unsubscribe from it like this:
> ```c#
> public async UniTask OnLoadAsync()
> {
>     Provider.onEnemyConnected += OnPlayerConnected;
> }
> 
> public async UniTask OnUnloadAsync()
> {
>     // this is very important, otherwise your plugin will not properly support reloads and unloads.
>     Provider.onEnemyConnected -= OnPlayerConnected;
> }
> ```

> [!WARNING] 
> **Avoid** writing singleton event listeners. This may cause problems if the event listener has transient or scoped dependencies. 
