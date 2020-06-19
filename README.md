# OpenMod [![Discord](https://img.shields.io/discord/666327627124047872?label=Discord )](https://discord.com/invite/jRrCJVm)

OpenMod is a modding framework for .NET. It is the successor of RocketMod 5.

It's main purpose is to add plugin functionality to Unity games, but it could be used with any .NET host.

## Features
OpenMod is based on modern C# code and best practices.
- Modern API for plugin development with C# and Unity best practices
- Plugin installation with [NuGet](https://nuget.org)
- Can self update with NuGet
- Based on [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host)
- IoC and Dependency Injection using Autofac
- [UniTask](https://github.com/Cysharp/UniTask) integration for Unity based games
- Configure OpenMod and plugins with yaml configurations, environment variables, commandline arguments, etc.
- Serilog for logging, including rich configuration options via logging.yml
- Scripting (todo)
- RCON (todo)

## Supported Games
Currently Unturned is the only supported game. More games might follow in the future.

A RocketMod 4 bridge has been made, which allows to run legacy RM4 plugins.
The configs for RM4 are yet to be decided to be separate, or to be proxied.

## Build Status
| **framework**                                                                                                                                                                          | standalone                                                                                                                                                                       | unityengine                                                                                                                                                                         | unturned                                                                                                                                                                                        |
|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [![OpenMod.Bootstrapper](https://github.com/openmod/OpenMod/workflows/OpenMod.Bootstrapper/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Bootstrapper) | [![OpenMod.Standalone](https://github.com/openmod/OpenMod/workflows/OpenMod.Standalone/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Standalone) | [![OpenMod.UnityEngine](https://github.com/openmod/OpenMod/workflows/OpenMod.UnityEngine/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.UnityEngine) | [![OpenMod.Unturned](https://github.com/openmod/OpenMod/workflows/OpenMod.Unturned/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Unturned)                      |
| [![OpenMod.API](https://github.com/openmod/OpenMod/workflows/OpenMod.API/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.API)                            |                                                                                                                                                                                  |                                                                                                                                                                                     | [![OpenMod.Unturned.Module](https://github.com/openmod/OpenMod/workflows/OpenMod.Unturned.Module/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Unturned.Module) |
| [![OpenMod.Core](https://github.com/openmod/OpenMod/workflows/OpenMod.Core/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Core)                         |                                                                                                                                                                                  |                                                                                                                                                                                     | [![OpenMod.Rocket.API](https://github.com/openmod/OpenMod/workflows/OpenMod.Rocket.API/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Rocket.API)                |
| [![OpenMod.NuGet](https://github.com/openmod/OpenMod/workflows/OpenMod.NuGet/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.NuGet)                      |                                                                                                                                                                                  |                                                                                                                                                                                     | [![OpenMod.Rocket.Core](https://github.com/openmod/OpenMod/workflows/OpenMod.Rocket.Core/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Rocket.Core)             |
| [![OpenMod.Runtime](https://github.com/openmod/OpenMod/workflows/OpenMod.Runtime/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Runtime)                |                                                                                                                                                                                  |                                                                                                                                                                                     | [![OpenMod.Rocket.Unturned](https://github.com/openmod/OpenMod/workflows/OpenMod.Rocket.Unturned/badge.svg)](https://github.com/openmod/OpenMod/actions?query=workflow%3AOpenMod.Rocket.Unturned) |

## ToDo
- [x] IoC
- [x] Logging
- [x] Yaml Configurations
- [x] Runtime
- [x] Dynamic bootstrapping
- [x] Projects
- [x] Unturned Module
- [x] Unitask integration
- [x] Plugins (dll / NuGet)
- [x] Plugin configurations & translations
- [x] Standalone console implementation
- [x] GitHub actions (auto releases & nuget deployments)
- [x] Eventing
- [x] Persistent Datastore
- [x] Permissions implementations
- [x] Command framework
- [x] Unturned Command Wrappers
- [x] Built-in OpenMod Commands
- [x] Framework events implementation
- [x] RM 4 Bridge
- [ ] Documentation
- [ ] Release
- [ ] RCON Plugin
- [ ] Scripting Plugin
