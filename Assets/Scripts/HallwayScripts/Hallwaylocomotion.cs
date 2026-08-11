using UnityEngine;
using UnityEngine.InputSystem;

namespace Controls {

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

        // The hallway rail origin -- set once on Start() from the HallwayForward position.
        // All lateral correction is projected back onto the line through this point.
        private Vector3 _railOrigin;

        private void Start() {
            if (HallwayForward != null)
                _railOrigin = HallwayForward.position;
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

        private void LateUpdate() {
            ConstrainToRail();
        }

        private void HandleMovement() {
            if (MoveAction == null || HallwayForward == null)
                return;

            Vector2 input = MoveAction.action.ReadValue<Vector2>();

            // Clamp out any backward push -- forward only
            float forwardAmount = Mathf.Max(0f, input.y);
            if (forwardAmount < MoveDeadzone)
                return;

            Vector3 direction = HallwayForward.forward;
            direction.y = 0f;
            direction.Normalize();

            transform.position += direction * forwardAmount * MoveSpeed * Time.deltaTime;
        }

        private void ConstrainToRail() {
            if (HallwayForward == null)
                return;

            // Project current position onto the hallway axis line.
            // This corrects any lateral drift introduced by snap turn pivot offset
            // without affecting Y (height) or forward progress.
            Vector3 railDirection = HallwayForward.forward;
            railDirection.y = 0f;
            railDirection.Normalize();

            Vector3 toPlayer = transform.position - _railOrigin;

            // Scalar distance along the rail
            float distanceAlongRail = Vector3.Dot(toPlayer, railDirection);

            // Reconstruct position strictly on the rail, keeping current Y
            Vector3 constrainedPosition = _railOrigin + railDirection * distanceAlongRail;
            constrainedPosition.y = transform.position.y;



            transform.position = constrainedPosition;
        }
    }
}