# OpenMod
OpenMod is a modding framework for .NET. It is the successor of RocketMod 5.

It's main purpose is to add plugin functionality to Unity games, but it could be used with any .NET host.

## Features
OpenMod is based on modern C# code and best practices.
- Modern API for plugin development with C# and Unity best practices
- Plugin installation with [NuGet](https://nuget.org)
- Based on [.NET Generic Host](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/generic-host)
- IoC and Dependency Injection using Autofac
- [UniTask](https://github.com/Cysharp/UniTask) integration for Unity based games
- Configure OpenMod and plugins with yaml configurations, environment variables, commandline arguments, etc.
- Serilog for logging, including rich configuration options via logging.yml
- Scripting (todo)
- RCON (todo)

## Supported Games
Currently Unturned is the only supported game. More games might follow in the future.

A RocketMod 4 bridge plugin is also planned which allows to run legacy rm4 plugins.

## Status
OpenMod is currently work-in-progress, hence a downloadable version does not exist.

Development will be fast since most of the code already exists in RocketMod 5. However, RocketMod 5 code is not ported 1:1, instead it gets refactored.

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
- [ ] Scheduling
- [ ] Eventing
- [ ] Economy interfaces
- [ ] Permissions implementations
- [ ] Command framework (CommandContext, CommandActor, etc.)
- [ ] Players
- [ ] RM 4 Bridge
- [ ] RCON
- [ ] Scripting
- [ ] Standalone console implementation