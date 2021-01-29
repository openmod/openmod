# Game events
These events are provided by the `OpenMod.Extensions.Games.Abstractions` package and are not part of the core OpenMod API.  
They may not be available on all supported OpenMod platforms.

| **Event**                   | **Fired when**                                  | **Cancellable** | **Namespace**                                 |
|-----------------------------|-------------------------------------------------|-----------------|-----------------------------------------------|
| IPlayerChattingEvent        | Before a player chats                           | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerSpawnedEvent         | After a player spawns                           | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerDamagingEvent        | Before a player is damaged                      | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerDamagedEvent         | After a player is damaged                       | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerDyingEvent           | Before a player takes a fatal  amount of damage | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerDeadEvent            | After a player dies                             | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerEnteringVehicleEvent | Before a player enters a vehicle                | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerEnteredVehicleEvent  | After a player enter a vehicle                  | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerExitingVehicleEvent  | Before a player exits a vehicle                 | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerExitedVehicleEvent   | After a player exit a vehicle                   | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerItemEquippingEvent   | Before a player equips an item                  | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerItemEquippedEvent    | After a player equips an item                   | No              | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerUnequippingEvent     | Before a player unequips an item                | Yes             | OpenMod.Extensions.Games.Abstractions.Players |
| IPlayerUnequippedEvent      | After a player unequips an item                 | No              | OpenMod.Extensions.Games.Abstractions.Players |
