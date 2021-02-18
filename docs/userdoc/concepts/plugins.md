# OpenMod plugins
Plugins add new functionality to your server. OpenMod provides commands to download, install, update, and remove plugins at runtime.

## Finding plugins
You can find a list of plugins at [the openmod-plugins page](https://openmod.github.io/openmod-plugins) or on [nuget.org](https://www.nuget.org/packages?q=openmod-plugin).

## Installing and updating plugins
There are two ways to install plugins:

Option 1: **Installing plugins from NuGet**. You can install plugins from NuGet using the `openmod install <package id>` command, e.g. `openmod install Kr4ken.NewEssentials`. To install specific versions, use `openmod install <package id>@<version>`. If you want to enable installation of pre-release versions, add the `-Pre` option: `openmod install <package id> -Pre`. To update plugins, run `openmod install <package id>` again.  

Option 2: **Installing plugins manually**. You can install plugins manually by moving the plugin dll file and all libraries of the plugin to the `openmod/plugins` folder. You can also install libraries with `openmod install <package id>` instead. To update plugins replace the .dll file with the newer one.

> [!NOTE]
> You must reload OpenMod with `openmod reload` to apply changes after installing or updating plugins.

## Removing plugins
To remove  plugins which have been installed using `openmod install`, use `openmod remove <package id>`. If you installed the plugin manually instead, delete the .dll file.

> [!NOTE]
> You must reload OpenMod with `openmod reload` to apply changes after removing plugins.