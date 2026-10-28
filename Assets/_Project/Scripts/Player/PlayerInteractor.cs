using System.Collections.Generic;
using ESO.Gameplay;
using ESO.Input;
using UnityEngine;

namespace ESO.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameInput), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public sealed class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Camera _interactionCamera;

        private readonly List<Collider2D> _overlaps = new List<Collider2D>();
        private GameInput _input;
        private Rigidbody2D _body;
        private CapsuleCollider2D _collider;
        private ContactFilter2D _filter;

        private void Awake()
        {
            _input = GetComponent<GameInput>();
            _body = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _filter = new ContactFilter2D().NoFilter();
            _filter.useTriggers = true;
        }

        private void Update()
        {
            if (!_input.isActiveAndEnabled || !_collider.enabled)
            {
                return;
            }

            bool keyboardPressed = _input.InteractPressed;
            bool pointerPressed = _input.PointerInteractPressed;
            if (!keyboardPressed && !pointerPressed)
            {
                return;
            }

            Camera camera = _interactionCamera != null ? _interactionCamera : Camera.main;
            if (!keyboardPressed && camera == null)
            {
                Debug.LogError("PlayerInteractor requires an Interaction Camera or a camera tagged MainCamera for mouse interaction.", this);
                return;
            }

            // Query on demand so teleports and disabled interiors cannot leave stale door candidates.
            Physics2D.SyncTransforms();
            _collider.Overlap(_filter, _overlaps);
            DoorTransition selected = null;
            float nearestDistance = float.PositiveInfinity;

            foreach (Collider2D overlap in _overlaps)
            {
                if (!overlap.isTrigger)
                {
                    continue;
                }

                DoorTransition door = overlap.GetComponentInParent<DoorTransition>();
                if (door == null || door.Trigger != overlap || !door.IsAvailable)
                {
                    continue;
                }

                if (!keyboardPressed && !IsUnderPointer(camera, overlap))
                {
                    continue;
                }

                float distance = ((Vector2)overlap.bounds.center - _body.position).sqrMagnitude;
                if (distance < nearestDistance)
                {
                    selected = door;
                    nearestDistance = distance;
                }
            }

            // Dispatch once, even when both inputs were pressed or another door becomes active.
            if (selected != null)
            {
                selected.TryTransition(_body);
            }
        }

        private bool IsUnderPointer(Camera camera, Collider2D trigger)
        {
            Ray ray = camera.ScreenPointToRay(_input.PointerPosition);
            Plane plane = new Plane(Vector3.forward, trigger.transform.position);
            return plane.Raycast(ray, out float distance) && trigger.OverlapPoint(ray.GetPoint(distance));
        }
    }
}
