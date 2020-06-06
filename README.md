# OpenMod
OpenMod is a modding framework for .NET. It is the successor of RocketMod 5.

It's main purpose is to add plugin functionality to Unity games, but it could be used with any .NET host.

## Features
OpenMod is based on modern C# code and best practices.
- Modern API for plugin development
- Plugin installation with NuGet
- IoC and Dependency Injection using Autofac
- Yml configurations
- Based on .NET Generic Host
- Serilog for logging, with rich configuration options
- Scripting (todo)
- Proper RCON implementation (todo)

## Supported Games
Currently Unturned is the only supported game. More games might follow in the future.

A RocketMod 4 bridge is also planned that allows to run legacy plugins.

## Status
OpenMod is currently work-in-progress, hence a downloadable version does not exist.

## ToDo
- [x] IoC
- [x] Logging
- [x] Yaml Configurations
- [x] Runtime
- [x] Dynamic bootstrapping
- [x] Projects
- [x] Unturned Module
- [x] Plugins (dll / NuGet)
- [ ] Plugin configurations & translations
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