using UnityEngine;
using UnityEngine.InputSystem;

namespace Controls
{
    // Attach to the player/XR rig root in the hallway scene only.
    // Since walking isn't grab-gated like InputRouter's other inputs,
    // this listens to the thumbstick action directly.
    //
    // Setup in Inspector:
    //   - MoveAction      -> your thumbstick/move Input Action Reference
    //   - HallwayForward  -> an empty GameObject placed in the scene, rotated
    //                       to face down the hallway. Its forward (blue arrow)
    //                       defines the only direction the player can walk.
    public class HallwayLocomotion : MonoBehaviour
    {
        [Tooltip("The thumbstick/move Input Action Reference (Vector2).")]
        public InputActionReference MoveAction;

        [Tooltip("Empty Transform placed in the scene, rotated to face down the hallway. Only its forward direction is used.")]
        public Transform HallwayForward;

        [Tooltip("Movement speed in units per second.")]
        public float MoveSpeed = 1.5f;

        [Tooltip("Minimum thumbstick push (0-1) required to start moving. Filters out drift/noise.")]
        public float InputDeadzone = 0.15f;

        private CharacterController _characterController;

        private void Awake()
        {
            // Optional - if you're using a CharacterController for collision handling.
            // If not present, falls back to direct transform movement.
            _characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (MoveAction != null)
                MoveAction.action.Enable();
        }

        private void OnDisable()
        {
            if (MoveAction != null)
                MoveAction.action.Disable();
        }

        private void Update()
        {
            if (MoveAction == null || HallwayForward == null) return;

            Vector2 input = MoveAction.action.ReadValue<Vector2>();

            // Only the forward push (positive Y) counts - pulling back does nothing.
            // This enforces "forward only, no backing up."
            float forwardAmount = Mathf.Max(0f, input.y);

            if (forwardAmount < InputDeadzone) return;

            Vector3 direction = HallwayForward.forward;
            direction.y = 0f; // keep movement flat, ignore any tilt on the forward transform
            direction.Normalize();

            Vector3 motion = direction * forwardAmount * MoveSpeed * Time.deltaTime;

            if (_characterController != null)
            {
                _characterController.Move(motion);
            }
            else
            {
                transform.position += motion;
            }
        }
    }
}