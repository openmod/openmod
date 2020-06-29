# OpenMod RocketMod Bridge
Based on https://github.com/SmartlyDressedGames/Legally-Distinct-Missile/commit/1b2615260aa1425f0443e90db75f51c007b7715f


## How it works
This repository includes a patched RocketMod that has been converted to an OpenMod plugin. 


The following modifications have been done to add support for OpenMod
- RocketMod is now a OpenMod plugin instead of an Unturned nexus module
- Changed project style to .NET SDK project with net461 target
- Command handling has been patched so that it does not conflict with OpenMod. OpenMod will fallback to RocketMod commands when a command was not found, hence OpenMod commands will always have priority over RocketMod commands.


The following are *not* planned:
- Loading RocketMod plugins as OpenMod plugins
- Loading RocketMod commands as OpenMod commands
- Adding any kind of new features to RocketMod except for upstream changes from LDM
- Integrating OpenMod services such as permissions. RocketMod will keep its own permission system. See [Rocket.PermissionLink](https://github.com/openmod/openmod/tree/master/unturned/rocketmod/Rocket.PermissionLink).


RocketMod keeps its own plugin system, config system, permission system etc.

## Contribution Guidelines
In addition to the [OpenMod Contribution Guidelines](https://github.com/openmod/OpenMod/blob/master/CONTRIBUTING.md), the follow rules also apply:
- Removing of any members or types (public or private) is not allowed to avoid breaking legacy plugins
- Do not add new features except for LDM upstream merges or OpenMod related bug fixes
- Document every change with `// OPENMOD PATCH: <description>` at the beginning and `// END OPENMOD PATCH: <description>` at the end

## Copyright
[RocketMod License](https://github.com/RocketMod/Rocket/blob/legacy/LICENSE)
