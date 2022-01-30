# OpenMod permissions
OpenMod has a simple role-based permission system. Permissions define which actions a user can execute and which they cannot.

You can manage permissions by editing the openmod.roles.yaml file inside the OpenMod directory or by using the `permission` and `permissionrole` commands. You can use the `help permission` and `help permissionrole` commands for more information.

>[!NOTE]
> Changes to the openmod.roles.yaml file are applied instantly. Reloading is not necessary.

## Permission roles
Roles are basically a group of permissions and other attributes. If you assign a role to a user, they will automatically inherit all permissions of the role. You can also add parent roles to a role for inheriting permissions.

A role has the following attributes:

- **Parents**: The parent roles, whose permissions are inherited.
- **Permissions**: List of permissions the role has.
- **Display Name**: Human-readable name of the role.
- **Is Auto Assigned**: Automatically assigns the role to new users. **Does not assign to existing users**.
- **Data**: Data that can be attached to the role by plugins. 
- **Priority**: In case of conflicting permissions, this attribute will define which role gets preferred.

## Permission syntax
Permissions are always made up of two components: PluginId and Permission. If the plugin ID is Kr4ken.NewEssentials and the permission is commands.home, the full form of the permission we would assign would be Kr4ken.NewEssentials:commands.home. 

### Creating roles
To create a new role, open the `openmod.roles.yaml` file. You will see something similar to this:
```yaml
roles:
- id: default
  parents: []
  permissions:
  - OpenMod.Core:help
  displayName: Default
  data: {}
  isAutoAssigned: true
- id: vip
  priority: 1
  parents:
  - default
  permissions:
  - Kr4ken.NewEssentials:commands.home
  - Kr4ken.NewEssentials:commands.tp  
  - Kr4ken.NewEssentials:kits.vip
  data: {}
```

This list contains 2 roles: default and vip. Notice how the `-` starts a new role.

To add a new role, simply copy the default role and add it like this:
```yaml
roles:
- id: default
  parents: []
  permissions:
  - OpenMod.Core:help
  displayName: Default
  data: {}
  isAutoAssigned: true
- id: vip
  displayName: VIP
  priority: 1
  parents:
  - default
  permissions:
  - Kr4ken.NewEssentials:commands.home
  - Kr4ken.NewEssentials:commands.tp  
  - Kr4ken.NewEssentials:kits.vip
  data: {}
- id: megavip
  displayName: Mega VIP
  priority: 1
  parents:
  - vip
  permissions:
  - Kr4ken.NewEssentials:kits.megavip
  data: {}  
```

### Removing roles
To remove a role, remove from the `openmod.roles.yaml` file or comment it out.

### Managing role permissions
From the earlier example, the megavip role has the following permissions:
- OpenMod.Core:commands.help (inherited from default, which is a parent of vip)
- Kr4ken.NewEssentials:kits.vip (inherited from vip)
- Kr4ken.NewEssentials:kits.megavip

What if we want it to have vip as parent, but we do not want it to inherit the `Kr4ken.NewEssentials:kits.vip` permission?  
In that case, we can negate the permission by adding it prefixed with a "!":

```
- id: megavip
  displayName: Mega VIP
  priority: 1
  parents:
  - default
  - vip
  permissions:
  - '!Kr4ken.NewEssentials:kits.vip' # Forcefully removes the permission, even if inherited
  - Kr4ken.NewEssentials:kits.megavip
  data: {} 
```

The ! prefix will forcefully remove any permission. 

> [!NOTE]
> You can also add and remove permissions with the `om p` command: `om p add role megavip !Kr4ken.NewEssentials:kit.vip`

> [!Note]
> If you are managing RocketMod permissions from OpenMod, you must add `RocketMod:` to the beginning of the permission.
> For example, if the RocketMod permission is `help`, the equivalent in OpenMod is `RocketMod:help`.

## User permissions and assigning roles
You can assign roles to users by using the `om r add player <player> <role>` command, e.g. `om r add player Trojaner megavip`. Similarly, you can remove users from a role by typing `om r remove player <player> <role>`

Permissions can be directly attached to users: `om p add player Trojaner Kr4ken.NewEssentials:kits.vip`. User permissions always override any conflicting role permissions. You can use `om p remove Trojaner Kr4ken.NewEssentials:kits.vip` to remove the permission again.

## Wildcards
Assume a teleport plugin has the following permissions:

* TeleportPlugin:teleport
* TeleportPlugin:teleport.bring
* TeleportPlugin:teleport.bring.request
* TeleportPlugin:teleport.request

Instead of adding all of these one by one, you can use the * wildcard to add all of them:

* TeleportPlugin:teleport.*

This will grant all permissions from above. You can also use TeleportPlugin:* to grant full access to the plugin. 

> [!NOTE]
> Just adding the TeleportPlugin:teleport permission will **not** grant the child permissions like TeleportPlugin:teleport.bring.

## Finding command permissions
If you do not know what permission a command requires, you can use `help <command>` to find it. Permissions for child commands are not granted automatically and must be given either by using wildcards on the parent command permission or by specifying them directly.

You can also look up permissions by checking out the help.md file generated in each plugins folder.