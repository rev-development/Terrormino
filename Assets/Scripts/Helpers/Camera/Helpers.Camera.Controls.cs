using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// ReSharper disable MemberCanBePrivate.Global

namespace Helpers.Camera
{
    [AddComponentMenu("Helpers.Camera.Controls")]
    public class Controls : MonoBehaviour
    {

        public Config Config;

        public UnityEvent EscapeStarted = new();

        public UnityEvent<Ray> MouseRaycasted = new();

        public UnityEvent RightClickStarted = new();

        private DefaultControls.DevCameraActions _actions;

        private DefaultControls _defaultControls;

        private bool _isRotating;

        private UnityEngine.Camera _mainCamera;

        private Vector2 _panInput;

        private Vector2 _rotateInput;

        private float _zoomInput;

        private void Awake() {
            if (!Config) Config = ScriptableObject.CreateInstance<Config>();

            _defaultControls = new DefaultControls();
            _actions = _defaultControls.DevCamera;
            _mainCamera = UnityEngine.Camera.main;
        }

        private void Update() {
            if (_panInput != Vector2.zero) Pan();

            if (!Mathf.Approximately(_zoomInput, 0f)) Zoom();

            if (_isRotating) Rotate();
        }

        private void OnEnable() {
            _defaultControls.Enable();

            _actions.Pan.performed += OnPanPerformed;
            _actions.Pan.canceled += OnPanCanceled;

            _actions.Zoom.performed += OnZoomPerformed;
            _actions.Zoom.canceled += OnZoomCanceled;

            _actions.ScrollZoom.performed += OnScrollZoomPerformed;

            _actions.RightClick.started += OnRightClickStarted;
            _actions.RightClick.canceled += OnRightClickCanceled;

            _actions.MouseDelta.performed += OnMouseDeltaPerformed;

            _actions.LeftClick.started += MouseRaycast;

            _actions.Escape.started += OnEscapeStarted;
        }

        private void OnDisable() {
            _actions.Pan.performed -= OnPanPerformed;
            _actions.Pan.canceled -= OnPanCanceled;

            _actions.Zoom.performed -= OnZoomPerformed;
            _actions.Zoom.canceled -= OnZoomCanceled;

            _actions.ScrollZoom.performed -= OnScrollZoomPerformed;

            _actions.RightClick.started -= OnRightClickStarted;
            _actions.RightClick.canceled -= OnRightClickCanceled;

            _actions.MouseDelta.performed -= OnMouseDeltaPerformed;

            _actions.LeftClick.started -= MouseRaycast;

            _actions.Escape.started -= OnEscapeStarted;

            _defaultControls.Disable();

            MouseRaycasted.RemoveAllListeners();
            EscapeStarted.RemoveAllListeners();
            RightClickStarted.RemoveAllListeners();
        }

        private void OnDestroy() {
            OnDisable();
            _defaultControls.Dispose();
        }

        public void Pan() {
            var panMoveVec = _mainCamera.transform.rotation.normalized
                             * new Vector3(_panInput.x, 0, _panInput.y)
                             * (Config.PanSpeed * Time.deltaTime);

            panMoveVec.y = 0;
            _mainCamera.transform.Translate(panMoveVec, Space.World);
        }

        public void Rotate() {
            _mainCamera.transform.localEulerAngles = new Vector3(
                    Mathf.Clamp(
                            _mainCamera.transform.localEulerAngles.x
                            - _rotateInput.y * Config.RotateSpeed * Time.deltaTime,
                            Config.CameraRotateXBoundsMin,
                            Config.CameraRotateXBoundsMax
                        ),
                    _mainCamera.transform.localEulerAngles.y + _rotateInput.x * Config.RotateSpeed * Time.deltaTime,
                    0
                );

            _rotateInput = Vector2.zero;
        }

        public void Zoom(float zoomInput) {
            // To keep the camera within bounds without stuttering, need to calculate the position it will end up in before allowing it to move there

            // Get the camera's forward vector as a world-space vector and scale it based on our input
            var intendedZoomVec = _mainCamera.transform.forward * (zoomInput * Config.ZoomSpeed);

            // Predict the camera's ending position by applying the intended zoom vector to its current position
            var predictedCamPos = _mainCamera.transform.position + intendedZoomVec;

            // Clamp the value of the y-axis to our min/max
            predictedCamPos.y = Mathf.Clamp(predictedCamPos.y, Config.CameraPanYBoundsMin, Config.CameraPanYBoundsMax);

            // Calc the difference in the predicted position and the current position
            // If the intended movement does not cause the camera to hit either bounds, this should be equal to the intended zoom vector
            var predictedCamPosDelta = predictedCamPos - _mainCamera.transform.position;

            // This vector can be applied to the camera's position without violating intended bounds
            var safeMoveVec = predictedCamPosDelta;

            // This is to prevent 'skating' when the camera is fully zoomed in or out 
            if (Mathf.Approximately(safeMoveVec.y, 0))
            {
                safeMoveVec.z = 0;
                safeMoveVec.x = 0;
            }

            _mainCamera.transform.Translate(safeMoveVec, Space.World);
        }

        public void Zoom() {
            Zoom(_zoomInput * Time.deltaTime);
        }

        public void MouseRaycast(InputAction.CallbackContext _) {
            var ray = _mainCamera.ScreenPointToRay(_actions.MousePosition.ReadValue<Vector2>());

            MouseRaycasted.Invoke(ray);
        }

        protected void OnMouseDeltaPerformed(InputAction.CallbackContext context) {
            if (!_isRotating) return;

            _rotateInput = context.ReadValue<Vector2>();
        }

        protected void OnRightClickCanceled(InputAction.CallbackContext _) {
            _isRotating = false;
            _rotateInput = Vector2.zero;
        }

        protected void OnRightClickStarted(InputAction.CallbackContext _) {
            _isRotating = true;
            RightClickStarted.Invoke();
        }

        protected void OnScrollZoomPerformed(InputAction.CallbackContext context) {
            Zoom(context.ReadValue<float>() * Config.ScrollZoomMultiplier);
        }

        protected void OnPanPerformed(InputAction.CallbackContext context) {
            _panInput = context.ReadValue<Vector2>();
        }

        protected void OnPanCanceled(InputAction.CallbackContext context) {
            _panInput = Vector2.zero;
        }

        protected void OnZoomPerformed(InputAction.CallbackContext context) {
            _zoomInput = context.ReadValue<float>();
        }

        protected void OnZoomCanceled(InputAction.CallbackContext _) {
            _zoomInput = 0f;
        }

        protected void OnEscapeStarted(InputAction.CallbackContext _) {
            EscapeStarted.Invoke();
        }

    }
}