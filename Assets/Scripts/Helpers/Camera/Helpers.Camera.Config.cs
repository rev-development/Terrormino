using UnityEngine;

namespace Helpers.Camera
{
    [CreateAssetMenu(fileName = "CameraControlsConfig", menuName = "Helpers/Camera/Config")]
    public class Config : ScriptableObject
    {

        public float CameraPanYBoundsMax = 5f;

        public float CameraPanYBoundsMin = 1f;

        public float CameraRotateXBoundsMax = 90f;

        public float CameraRotateXBoundsMin = 0f;

        public float PanSpeed = 4f;

        public float RotateSpeed = 40f;

        public float ScrollZoomMultiplier = 0.0625f;

        public float ZoomSpeed = 4f;

    }
}