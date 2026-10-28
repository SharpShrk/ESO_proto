using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ESO.Input
{
    public enum RebindResult
    {
        Completed,
        Cancelled,
        Conflict
    }

    [DisallowMultipleComponent]
    public sealed class GameInput : MonoBehaviour
    {
        private const string OverridesKey = "ESO.Input.BindingOverrides.v1";

        [SerializeField] private InputActionAsset _source;

        private InputActionAsset _actions;
        private InputActionMap _player;
        private InputActionMap _panels;
        private InputAction _move;
        private InputAction _sprint;
        private InputAction _interact;
        private InputAction _pointerInteract;
        private InputAction _inventory;
        private InputAction _quests;
        private InputActionRebindingExtensions.RebindingOperation _rebinding;
        private bool _gameplayEnabled = true;
        private bool _panelsEnabled = true;

        public event Action BindingsChanged;
        public event Action<RebindResult> RebindFinished;

        public float Move => _move.ReadValue<float>();
        public bool IsRunning => _sprint.IsPressed();
        public bool InteractPressed => _interact.WasPressedThisFrame();
        public bool PointerInteractPressed => _pointerInteract.WasPressedThisFrame()
            && (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject());
        public Vector2 PointerPosition => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        public bool InventoryPressed => _inventory.WasPressedThisFrame();
        public bool QuestsPressed => _quests.WasPressedThisFrame();
        public bool IsRebinding => _rebinding != null;

        private void Awake()
        {
            InputActionAsset source = _source != null ? _source : InputSystem.actions;
            if (source == null)
            {
                Debug.LogError("GameInput requires the project's Input Actions asset.", this);
                enabled = false;
                return;
            }

            // Each owner has its own map state; UI input remains independent.
            _actions = Instantiate(source);
            _actions.Disable();
            _player = _actions.FindActionMap("Player", true);
            _panels = _actions.FindActionMap("Panels", true);
            _move = _player.FindAction("Move", true);
            _sprint = _player.FindAction("Sprint", true);
            _interact = _player.FindAction("Interact", true);
            _pointerInteract = _player.FindAction("PointerInteract", true);
            _inventory = _panels.FindAction("Inventory", true);
            _quests = _panels.FindAction("Quests", true);

            string overrides = PlayerPrefs.GetString(OverridesKey, string.Empty);
            if (!string.IsNullOrEmpty(overrides))
            {
                try
                {
                    _actions.LoadBindingOverridesFromJson(overrides);
                }
                catch (ArgumentException exception)
                {
                    Debug.LogWarning($"Cannot load input bindings; using defaults. {exception.Message}", this);
                    _actions.RemoveAllBindingOverrides();
                }
            }
        }

        private void OnEnable()
        {
            UpdateMaps();
        }

        private void OnDisable()
        {
            CancelRebind();
            if (_actions != null)
            {
                _actions.Disable();
            }
        }

        private void OnDestroy()
        {
            if (_actions != null)
            {
                Destroy(_actions);
            }
        }

        public void SetGameplayEnabled(bool enabled)
        {
            _gameplayEnabled = enabled;
            UpdateMaps();
        }

        public void SetPanelsEnabled(bool enabled)
        {
            _panelsEnabled = enabled;
            UpdateMaps();
        }

        public InputAction GetAction(string actionPath)
        {
            InputAction action = _actions.FindAction(actionPath, true);
            if (action.actionMap != _player && action.actionMap != _panels)
            {
                throw new ArgumentException("Only Player and Panels actions can be rebound.", nameof(actionPath));
            }

            return action;
        }

        public string GetBindingLabel(string actionPath, int bindingIndex)
        {
            return GetAction(actionPath).GetBindingDisplayString(bindingIndex);
        }

        public void StartRebind(string actionPath, int bindingIndex)
        {
            if (!isActiveAndEnabled)
            {
                throw new InvalidOperationException("Enable GameInput before rebinding.");
            }

            InputAction action = GetAction(actionPath);
            if (bindingIndex < 0 || bindingIndex >= action.bindings.Count || action.bindings[bindingIndex].isComposite)
            {
                throw new ArgumentOutOfRangeException(nameof(bindingIndex), "Select a button or a composite part.");
            }

            CancelRebind();
            string previousOverrides = _actions.SaveBindingOverridesAsJson();
            _player.Disable();
            _panels.Disable();

            _rebinding = action.PerformInteractiveRebinding(bindingIndex)
                .WithExpectedControlType("Button")
                .WithControlsHavingToMatchPath("<Keyboard>/*")
                .WithControlsHavingToMatchPath("<Mouse>/*")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnCancel(operation => FinishRebind(RebindResult.Cancelled))
                .OnComplete(operation =>
                {
                    if (HasConflict(action, bindingIndex))
                    {
                        _actions.LoadBindingOverridesFromJson(previousOverrides);
                        FinishRebind(RebindResult.Conflict);
                        return;
                    }

                    SaveBindings();
                    FinishRebind(RebindResult.Completed);
                });
            _rebinding.Start();
        }

        public void CancelRebind()
        {
            _rebinding?.Cancel();
        }

        public void ResetBindings()
        {
            CancelRebind();
            _actions.RemoveAllBindingOverrides();
            SaveBindings();
        }

        private bool HasConflict(InputAction action, int bindingIndex)
        {
            string path = action.bindings[bindingIndex].effectivePath;
            foreach (InputActionMap map in new[] { _player, _panels })
            {
                foreach (InputAction candidate in map.actions)
                {
                    for (int index = 0; index < candidate.bindings.Count; index++)
                    {
                        if (candidate == action && index == bindingIndex)
                        {
                            continue;
                        }

                        InputBinding binding = candidate.bindings[index];
                        if (!binding.isComposite && string.Equals(binding.effectivePath, path, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void FinishRebind(RebindResult result)
        {
            _rebinding.Dispose();
            _rebinding = null;
            UpdateMaps();
            RebindFinished?.Invoke(result);
        }

        private void SaveBindings()
        {
            PlayerPrefs.SetString(OverridesKey, _actions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
            BindingsChanged?.Invoke();
        }

        private void UpdateMaps()
        {
            if (_actions == null || IsRebinding)
            {
                return;
            }

            if (isActiveAndEnabled && _gameplayEnabled)
            {
                _player.Enable();
            }
            else
            {
                _player.Disable();
            }

            if (isActiveAndEnabled && _panelsEnabled)
            {
                _panels.Enable();
            }
            else
            {
                _panels.Disable();
            }
        }
    }
}
