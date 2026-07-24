# MyroP's Player Teleporter System

This is a system that allows players to teleport to other players in the instance. The system also features a checkpoint system, allowing players to teleport to checkpoints instead.

## Installation

- Download and import the Unity package file, which can be found on the Releases page.
- Drag and drop the `PlayerTeleporter` prefab into your scene.
- If you want to add checkpoints to your world, drag and drop the `Checkpoint` prefab into your scene, select the GameObject, and on the `PlayerCheckpoint` component, make sure it references the `PlayerTeleporter` prefab.
- If you want players to become reachable or unreachable in specific areas of your world, drag and drop the `PlayerReachableSetter` prefab into your scene and configure it.
- All prefabs and modules you can add to your scene are described below under the "Settings" chapter.

**Note:**
- If you need to toggle the system on and off, do not toggle the entire `PlayerTeleporter` prefab. Instead, toggle the `Canvas` child GameObject of `PlayerTeleporter`. Disabling the entire GameObject will also disable important scripts.
- If you do not want the checkpoint data to be persisted, select the `POPlayerTeleporter` GameObject and remove the `VRC Enable Persistence` component.
- Since the data is persisted using PlayerObjects, do not regenerate Network IDs if you encounter Network ID-related issues, as this will clear the persistent data. Network IDs have a tendency to break due to a long-standing VRC SDK bug.
- Adding more checkpoints may invalidate the currently saved persistent data.

## Settings

The prefab can be customized using the settings and prefabs below.

### PlayerTeleporter

- **Teleportation Mode**: If this setting is set to `Player`, players will teleport directly to other players. If it is set to `Checkpoint`, players will teleport to the last checkpoint reached by the target player.
- **Event sender when the player respawns**: If needed, an event defined in `Event Name Respawn` can be sent to the GameObject referenced in `Event Behaviour Respawn` whenever the local player respawns. This can be useful if, for example, you want to enable the spawn area when the player respawns. This setting is rarely needed, but it can be useful in some cases.
- Players that cannot currently be teleported to will have their teleport button grayed out.

### POPlayerTeleporter

This prefab represents a single entry in the player list. It displays the name and the location of the player and provides the button used to teleport to them. The UI can be freely customized to match your world's style.

- **Player Teleporter reference**: This parameter must reference the main prefab.
- **Show player location**: If you want to show the player's location on the panel, locations can be defined using checkpoints. If you are not using any checkpoints, you can leave this setting disabled.

### Checkpoint

- **Player Teleporter reference**: This parameter must reference the main prefab.
- **Name**: The name of the checkpoint, which will be shown on the panel. For instance, if the player entered the checkpoint `BEACH`, then `BEACH` will be shown next to the player's name on the main panel.
- **Teleport location**: The location where the player will be teleported if they choose to teleport to that checkpoint. If this field is empty, the player will be teleported to the checkpoint's transform.
- **Can teleport here**: If unchecked, the checkpoint cannot be teleported to, but it can still be used to display the player's location on the panel.
- **Event senders...**: A set of event sender settings in case you need to send an event when a player enters a checkpoint. For instance, the `local player enters the checkpoint` event can be useful if you need to trigger an animation or script when the local player enters the checkpoint. The `... teleport to that location` event is similar, except that it is triggered when the local player teleports to that location.

### PlayerReachableSetter

If your world contains spoiler-heavy areas, you may not want newly joined players to teleport directly to someone who has already reached them. This prefab allows you to make players reachable or unreachable when they enter the trigger.

- **Player Teleporter reference**: This parameter must reference the main prefab.
- **Is Reachable**: When a player enters the trigger, if this parameter is set to `true`, other players can teleport to them again. If set to `false`, they cannot (this will disable the "Teleport" button).

### TeleportToCheckpoint

This prefab allows players to teleport to a specific checkpoint by interacting with it.

- **Checkpoint**: The checkpoint players will be teleported to when they interact with this prefab.

### TeleportToLastCheckpointPersistence

This prefab automatically teleports the local player to their last saved checkpoint when they join the instance.

> **Note:** This prefab only works when checkpoint persistence is enabled (that is, when the `POPlayerTeleporter` GameObject still has the `VRC Enable Persistence` component).

- **Player Teleporter reference**: This parameter must reference the main prefab.

## API

> [!NOTE]
> The API below is intended for developers who want to extend the system by creating custom modules. If you only want to use the prefab in your world, you can safely ignore this section.

I started working on an API so modules can be implemented, but it is still very bare-bones.

Create a class that inherits from `IPlayerTeleporterModule`. You can then override the following methods:

#### public virtual bool OnLocalPlayerTeleportToPlayer(VRCPlayerApi playerToTeleportTo, PlayerCheckpoint checkpoint)

Called when the local player wants to teleport to `playerToTeleportTo`. `checkpoint` contains the last checkpoint reached by the other player.

Note that `checkpoint` can be `null` in two cases: when there are no checkpoints, or when the other player is at the spawn point. The spawn point is treated as a virtual checkpoint and therefore has no associated `PlayerCheckpoint` instance.

This callback is only executed when `Teleportation Mode` is set to `Player`.

**Return:** `true` to let the prefab handle the teleportation, or `false` if you want to teleport the player yourself.

#### public virtual bool OnLocalPlayerTeleportToPlayerCheckpoint(VRCPlayerApi playerToTeleportTo, PlayerCheckpoint checkpointToTeleportTo)

Similar to `OnLocalPlayerTeleportToPlayer`, except this callback is only executed when `Teleportation Mode` is set to `Checkpoint`.

#### public virtual void OnPlayerEnteredCheckpoint(VRCPlayerApi player, PlayerCheckpoint checkpoint)

Called when player `player` enters checkpoint `checkpoint`.

## Public methods

The following public methods can safely be called from the `PlayerTeleporter` prefab.

- `PlayerCheckpoint _GetCheckpoint(int id)`: Returns the checkpoint with the specified ID. Note that ID `0` represents the spawn point, in which case the method returns `null`.
- `void TeleportLocalPlayerToPlayer(VRCPlayerApi player)`
- `void TeleportLocalPlayerToCheckpoint(PlayerCheckpoint checkpoint)`
- `void ExecuteCheckpointEvent(PlayerCheckpoint checkpoint)`: Executes the **Event senders...** events of the specified checkpoint. If `checkpoint` is `null`, the respawn event defined on the main prefab is triggered.
- `_TeleportLocalPlayerToLastKnownCheckpoint()`
- `_GetPlayerObjectOfPlayer(VRCPlayerApi player)`: Returns the `PlayerObject` associated with the specified player. This is useful if you need to access their persistent data.

## Credits

Crediting this project is **not required**, but it is greatly appreciated. If you would like to give credit, please use my VRChat name **MyroP**.

You may also include a link to this repository.

# License

This project is licensed under the **MIT License**. See the `LICENSE` file for details.