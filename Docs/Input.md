# Input

The project-wide asset is `Assets/Settings/InputSystem_Actions.inputactions`.
Its existing UI map remains available for `InputSystemUIInputModule`.

| Action path | Default bindings |
| --- | --- |
| Player/Move | 1D axis: A/D; alternate: Left/Right arrows |
| Player/Sprint | Left Shift; Right Shift (hold) |
| Player/Interact | E |
| Player/PointerInteract | Left mouse button |
| Panels/Inventory | Q |
| Panels/Quests | T |

Add one `ESO.Input.GameInput` component to the future player/input owner.
Leave Source empty to use the project-wide asset, or assign that same asset explicitly.
The component clones the asset and owns its lifetime; do not also use PlayerInput
to dispatch the same gameplay commands. Scenes have not been changed.

Read Move (float), IsRunning and the *Pressed properties in Update.
PointerInteractPressed rejects clicks over EventSystem UI. PointerPosition is in
screen coordinates. The future interaction system must pick the clicked target and
check its distance; keyboard interaction should select a nearby target.

Call SetGameplayEnabled(false) during dialogue or while a panel is open.
Panels remain enabled so Q/T can close them. The panel owner should derive this
flag from all currently open panels/dialogues. SetPanelsEnabled(false) disables
panel shortcuts too, for example while a text input or modal settings screen owns input.
These methods do not change time scale or pause the world.

## Rebinding from a future settings screen

- GetAction("Player/Move").bindings lists the binding slots. Skip composite roots;
  offer their individual Negative/Positive parts. The two composites provide
  independent main and alternate movement bindings.
- GetBindingLabel(actionPath, bindingIndex) provides the current display label.
- StartRebind(actionPath, bindingIndex) captures a keyboard/mouse button.
  Escape cancels; CancelRebind() also cancels programmatically.
- Gameplay and panel maps are suspended during capture; UI input is independent.
- RebindFinished reports Completed, Cancelled or Conflict. Conflicts with any
  other Player/Panels binding restore the previous bindings. UI bindings are
  excluded from conflict checking because UI and gameplay intentionally share controls.
- BindingsChanged signals a successful change or reset. Refresh labels then.
- Changes save to PlayerPrefs (ESO.Input.BindingOverrides.v1) and load in Awake.
- ResetBindings() resets every gameplay and panel binding to the asset defaults.

The settings screen itself, character motor, interactions, inventory and quest UI
are separate features and are not implemented by this input component.

## Manual integration checks

After attaching the component in the gameplay scene, check A/D and arrows,
simultaneous opposite directions, both Shift keys, E, Q, T, and mouse clicks.
Check that clicking UI does not interact with the world, disabling gameplay stops
Move/Sprint while panel shortcuts still work, and disabling the component stops input.
In settings, check capture, Escape cancellation, duplicate rejection, primary and
alternate bindings, labels, persistence after restart, and reset to defaults.
