using UnityEngine;

namespace ESO.Gameplay
{
    [DisallowMultipleComponent]
    public sealed class DoorTransition : MonoBehaviour
    {
        [SerializeField] private Collider2D _trigger;
        [SerializeField] private GameObject _interiorRoot;
        [SerializeField] private bool _enterInterior = true;
        [SerializeField] private Transform _destination;

        public Collider2D Trigger => _trigger;

        public bool IsAvailable => isActiveAndEnabled
            && _trigger != null && _trigger.enabled && _trigger.isTrigger
            && _trigger.gameObject.activeInHierarchy
            && _interiorRoot != null && _interiorRoot.activeSelf != _enterInterior;

        private void Reset()
        {
            _trigger = GetComponentInChildren<Collider2D>(true);
        }

        public bool TryTransition(Rigidbody2D player)
        {
            if (!IsAvailable || player == null)
            {
                return false;
            }

            if (_destination == null)
            {
                Debug.LogError($"Door '{name}' requires a Destination transform before it can be used.", this);
                return false;
            }

            if (player.transform.IsChildOf(_interiorRoot.transform))
            {
                Debug.LogError($"Door '{name}': Player must be outside the Interior Root hierarchy.", this);
                return false;
            }

            Vector2 destination = _destination.position;
            player.linearVelocity = Vector2.zero;
            player.position = destination;
            _interiorRoot.SetActive(_enterInterior);
            Physics2D.SyncTransforms();
            return true;
        }
    }
}
