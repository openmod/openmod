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
The configs for RM4 are yet to be decided to be seperate, or to be proxied.

## Status
OpenMod is currently work-in-progress, hence a downloadable version does not exist.

Development will be fast since most of the code already exists in RocketMod 5. However, RocketMod 5 code is not ported 1:1, instead it gets refactored.

framework | standalone | unityengine | unturned 
----------|------------|-------------|---------
![OpenMod.Bootstrapper Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Bootstrapper/badge.svg) | ![OpenMod.Standalone Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Standalone/badge.svg) | ![OpenMod.UnityEngine Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.UnityEngine/badge.svg) | ![OpenMod.Unturned Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Unturned/badge.svg)
![OpenMod.API Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.API/badge.svg) | | | ![OpenMod.Unturned.Module Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Unturned.Module/badge.svg)
![OpenMod.Core Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Core/badge.svg)
![OpenMod.NuGet Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.NuGet/badge.svg)
![OpenMod.Runtime Build Status](https://github.com/openmod/OpenMod/workflows/OpenMod.Runtime/badge.svg)

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
