using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    // Pulled from SnakeEyes
    public class MouseLook : MonoBehaviour
    {
        public Transform PlayerPosition;
        public PlayerInput PlayerInput;

        public float MouseSensitivity = 2f;
        public float CameraVerticalRotation = 0f;
        public bool LockedCursor = true;

        public InputActionReference MousePositionActionRef;

        public void Start()
        {
            if (PlayerInput.currentControlScheme == "Keyboard Mouse")
            {
                // Lock and Hide the Cursor
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        void Update()
        {
            if (PlayerInput.currentControlScheme == "Keyboard Mouse")
            {
                // Collect Mouse Input
                //float inputX = Input.GetAxis("Mouse X") * MouseSensitivity;
                //float inputY = Input.GetAxis("Mouse Y") * MouseSensitivity;
                float inputX = MousePositionActionRef.action.ReadValue<Vector2>().x;
                float inputY = MousePositionActionRef.action.ReadValue<Vector2>().y;

                // Rotate the Camera around its local X axis
                CameraVerticalRotation -= inputY;
                CameraVerticalRotation = Mathf.Clamp(CameraVerticalRotation, -90f, 90f);
                transform.localEulerAngles = Vector3.right * CameraVerticalRotation;

                // Rotate the Player Object and the Camera around its Y axis
                PlayerPosition.Rotate(Vector3.up * inputX);
            }
        }
    }
}
