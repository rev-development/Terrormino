using Helpers.Attributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controls {

    [AiGenerated("Claude", "claude-sonnet-4-6")]
    public class HallwayLocomotion : MonoBehaviour {
        [Header("Movement")]
        [Tooltip("Left thumbstick/move Input Action Reference (Vector2).")]
        public InputActionReference MoveAction;

        [Tooltip("Empty Transform rotated to face down the hallway. Only its forward direction is used.")]
        public Transform HallwayForward;

        [Tooltip("Movement speed in units per second.")]
        public float MoveSpeed = 1.5f;

        [Tooltip("Minimum thumbstick push (0-1) required to start moving. Filters out drift/noise.")]
        public float MoveDeadzone = 0.15f;

        // The fixed X position on the rail -- locked in at Start()
        // and enforced every LateUpdate regardless of what moved the rig.
        // Hallway runs along world Z so X is always the lateral axis to lock.
        private float _railX;

        private void Start() {
            _railX = transform.position.x;
        }

        private void OnEnable() {
            if (MoveAction != null)
                MoveAction.action.Enable();
        }

        private void OnDisable() {
            if (MoveAction != null)
                MoveAction.action.Disable();
        }

        private void Update() {
            HandleMovement();
        }

        // LateUpdate runs after XR Toolkit's turn provider has already moved the rig,
        // so snapping back here is guaranteed to be the last word on position this frame
        private void LateUpdate() {
            EnforceRail();
        }

        private void HandleMovement() {
            if (MoveAction == null || HallwayForward == null)
                return;

            Vector2 input = MoveAction.action.ReadValue<Vector2>();

            float forwardAmount = Mathf.Max(0f, input.y);
            if (forwardAmount < MoveDeadzone)
                return;

            Vector3 direction = HallwayForward.forward;
            direction.y = 0f;
            direction.Normalize();

            transform.position += direction * forwardAmount * MoveSpeed * Time.deltaTime;
        }

        private void EnforceRail() {
            Vector3 pos = transform.position;
            pos.x = _railX;
            transform.position = pos;
        }
    }
}