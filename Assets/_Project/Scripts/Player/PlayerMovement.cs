using ESO.Input;
using UnityEngine;

namespace ESO.Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameInput), typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _walkSpeed = 4f;
        [SerializeField, Min(0f)] private float _runSpeed = 7f;

        private GameInput _input;
        private Rigidbody2D _body;

        private void Awake()
        {
            _input = GetComponent<GameInput>();
            _body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            float direction = _input.isActiveAndEnabled ? _input.Move : 0f;
            float speed = _input.isActiveAndEnabled && _input.IsRunning ? _runSpeed : _walkSpeed;
            Vector2 velocity = _body.linearVelocity;
            velocity.x = direction * speed;
            _body.linearVelocity = velocity;
        }

        private void OnDisable()
        {
            if (_body != null)
            {
                Vector2 velocity = _body.linearVelocity;
                velocity.x = 0f;
                _body.linearVelocity = velocity;
            }
        }

        private void OnValidate()
        {
            _walkSpeed = Mathf.Max(0f, _walkSpeed);
            _runSpeed = Mathf.Max(_walkSpeed, _runSpeed);
        }
    }
}
