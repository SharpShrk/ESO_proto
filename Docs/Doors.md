# Door transitions

`DoorTransition` belongs on the DoorArea prefab root. Trigger references the
existing trigger collider on its Circle child. `PlayerInteractor` belongs on
Player alongside GameInput, Rigidbody2D and CapsuleCollider2D.

Configure each scene instance in the Inspector:

| Field | Entry door | Exit door |
| --- | --- | --- |
| Trigger | Circle's trigger collider | Circle's trigger collider |
| Interior Root | HouseInside | The same HouseInside |
| Enter Interior | Enabled | Disabled |
| Destination | Your inside arrival point | Your outside arrival point |

Keep Player and both arrival points outside the hierarchy of HouseInside.
HouseOutside stays active. Start HouseInside inactive if the player starts outside.
The exit instance is a child of HouseInside, so it activates with the interior.
Its initial position matches the entry door; adjust it to the intended interior doorway.

Arrival points have intentionally not been created or assigned: the scene author
will add them. Until Destination is assigned, attempting a transition logs a clear
error and leaves the player and interior unchanged. Scene references must be set
on prefab instances, not on the prefab asset.

E chooses the nearest available door whose trigger overlaps the player collider.
Left click additionally requires the pointer to be over that door's trigger.
Clicks over EventSystem UI are ignored by GameInput. A door is available only for
its configured interior state. One input frame dispatches at most one transition.

The transition resets velocity and sets Rigidbody2D.position to the arrival
point's X/Y. Player Z is preserved. Frozen Y keeps the new height after arrival.
Camera position and sorting layers are not changed by a transition.
