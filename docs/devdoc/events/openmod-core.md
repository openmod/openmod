# OpenMod.Core events

| **Event**                                       | **Fired when**                                    |
|-------------------------------------------------|---------------------------------------------------|
| UserConnectingEvent                             | A user attempts to connect to the server          |
| UserConnectedEvent                              | A user has connected to the server                |
| UserDisconnectedEvent                           | A user has disconnected from the server           |
| CommandExecutingEvent                           | A command is going to be executed                 |
| CommandExecutedEvent                            | A command has been executed                       |
| OpenModInitializedEvent                         | OpenMod has been initialized                      |
| OpenModShutdownEvent                            | OpenMod is shutting down                          |
| PluginActivatingEvent                           | A plugin is being activated                       |
| PluginContainerConfiguringEvent                 | A plugins IoC container is getting created        |
| PluginConfigurationChangedEvent                 | A plugins configuration has changed               |
| PluginLoadEvent                                 | A plugin is loading                               |
| PluginLoadedEvent                               | A plugin has been loaded                          |
| PluginUnloadEvent                               | A plugin is unloading                             |
| PluginUnloadedEvent                             | A plugin has been unloaded                        |