# Scheduling Tasks
OpenMod provides the `AsyncHelper.Schedule` method for fire-and-forget tasks.  
It will enqueue the given task on a thread pool. This can be used to run tasks delayed or periodically.  

## Universal
The following examples work on all OpenMod platforms.

### Running a delayed Task
You can delay a Task like this:

```c#
public async Task MyTask()
{
    m_Logger.LogInformation("Waiting 5 seconds...");
    await Task.Delay(TimeSpan.FromSeconds(5));
    m_Logger.LogInformation("Done!");
}
```
Then call AsyncHelper.Schedule like this:
```c#
AsyncHelper.Schedule("My Task", () => MyTask());
```

### Running a Task periodically
If you want to run a Task periodically, all you have to do is to surround your task with a while loop:

```c#
public async Task MyPeriodicTask(IOpenModPlugin myPlugin)
{
    while(myPlugin.IsComponentAlive) // ensure this task runs only as long as the plugin is loaded 
    {
        m_Logger.LogInformation("Waiting 5 seconds...");
        await Task.Delay(TimeSpan.FromSeconds(5));
        m_Logger.LogInformation("Done!");
    }
}
```
Then call AsyncHelper.Schedule like earlier, but passing the plugin instance:
```c#
AsyncHelper.Schedule("My Task", () => MyPeriodicTask(myPlugin));
```

> [!CAUTION]
> **Do not** keep your tasks running after your plugin gets unloaded. Make sure your tasks stop running when your plugin unloads. You can use your plugins IsComponentAlive property or [cancellation tokens](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken) to do this.   

> [!CAUTION]
> **Do not** use Thread.Sleep or similar blocking methods like non-async I/O methods in Tasks. Such methods will block the worker thread and prevent other tasks from running. Always use the async alternatives instead, such as `Task.Delay` instead of Thread.Sleep or `Stream.ReadAsync` instead of Stream.Read.

## UnityEngine
The following examples only work with games using the UnityEngine such as Unturned.

### Running a Task on every Update
Like in the `Running a Task Periodically` example, we will use a while loop again.  
Notice how the return type has changed to `UniTask` and how the call to the `AsyncHelper.Schedule` method has changed.

```c#
public async UniTask MyUpdateTask(IOpenModPlugin myPlugin)
{
    await UniTask.SwitchToMainThread(); // ensure this runs on main thread first.
    int i = 0;
    while(myPlugin.IsComponentAlive) // ensure this task runs only as long as the plugin is loaded 
    {
        await UniTask.DelayFrame(1, PlayerLoopTiming.Update);
        m_Logger.LogInformation($"Frame update: {++i}");
    }
}
```

Let's break this down.  
Inspect the following line:
`await UniTask.DelayFrame(1, PlayerLoopTiming.Update)`  

The first parameter, the 1, defines how many frames to wait. So this example will always wait for one frame and hence runs on every frame update.  
The second parameter, PlayerLoopTiming.Update, sets which type of update it should wait for. In this example, it is a normal frame update. You can use other update types such as FixedUpdate too.

The following update types are available:

* EarlyUpdate
* LastEarlyUpdate
* FixedUpdate
* LastFixedUpdate
* PreUpdate
* LastPreUpdate
* Update
* LastUpdate
* PreLateUpdate
* LastPreLateUpdate
* PostLateUpdate
* LastPostLateUpdate

To schedule your task, call the AsyncHelper like this: 
```c#
AsyncHelper.Schedule("My Task", () => MyUpdateTask(myPlugin).AsTask() /* for UniTask, you will have to use .AsTask() */);
```
