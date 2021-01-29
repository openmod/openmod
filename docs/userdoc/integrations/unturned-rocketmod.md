# RocketMod integration
OpenMod supports side by side installation with RocketMod. It will automatically detect RocketMod and apply patches on the fly. In other words, you do not have to chose which plugin framework you want, you can use both.

## Advanced RocketMod configuration
You can adjust the openmod.unturned.yaml file for more advanced scenarios, like making RocketMod use OpenMod's [permission system](../concepts/permissions.md) instead of RocketMod's Permissions.xml or OpenMod's economy system instead of Uconomy:

```yaml
# Configuration for RocketMod integration. Will have no effects if RocketMod is not installed.
# RocketMod must be installed as a separate module for these to work. If using the legacy OpenMod RocketMod Bridge these will not work.
rocketmodIntegration:
  # Sets the permission system to use
  # Available values:
  # - RocketMod: OpenMod will use RocketMod's permission system (default)
  # - OpenMod:   RocketMod will use OpenMod's permission system
  # - Separate:  OpenMod and RocketMod will not have synced permissions
  permissionSystem: RocketMod 

  # Sets the economy system to use
  # Available values:
  # - RocketMod_Uconomy: OpenMod will use Uconomy for its economy system (default)
  # - OpenMod_Uconomy:   RocketMod Uconomy will use OpenMod's economy system
  # - Separate:          OpenMod and RocketMod will not have synced economy
  economySystem: RocketMod_Uconomy
```
 