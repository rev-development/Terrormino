using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Flashlight
{
    public class Shake : MonoBehaviour
    {
        public Light LightSource;
        public Collider LightCollider;
        public AudioSource ShakingSound;
        public AudioSource NoChargeSound;
        public XRGrabInteractable GrabInteractable;

        // Changed name to avoid confusion with gameObject.Active
        public bool IsOn = false;

        // Made a simple helper class just to enforce a Mathf.Clamp() setter
        public Helpers.ClampedFloat Battery = new(5f, 5f);

        // Pulled out any constants so they are modifiable in the editor
        public float SmoothingFactor = 0.2f;
        public float MinThreshold = 2.5f;
        public float ChargeMultiplier = 6f;

        private Vector3 _cachedVelocity = Vector3.zero;

        public void Charge(Vector3 currentVelocity)
        {
            // Originally there was an if statement to make sure that cachedMagnitude was not an abysmally small number
            // Instead of doing an if statement, just truncate the float to the precision you want, then its never a problem
            float cachedMagnitude = Helpers.Math.RoundFloatToDecimalPlaces(
                _cachedVelocity.magnitude,
                2
            );
            _cachedVelocity = Vector3.Lerp(_cachedVelocity, currentVelocity, SmoothingFactor);

            float percentageIncrease =
                Mathf.Abs((currentVelocity.magnitude - _cachedVelocity.magnitude) / cachedMagnitude)
                * 100;

            if (percentageIncrease >= MinThreshold)
            {
                Battery.Value += Time.deltaTime * 6f;
                ShakingSound.Play();
            }
            else
            {
                ShakingSound.Stop();
            }

            _cachedVelocity = currentVelocity;
        }

        public void OnShake(InputAction inputAction)
        {
            Charge(inputAction.ReadValue<Vector3>());
        }

        public void OnShake(Vector3 velocity)
        {
            Charge(velocity);
        }

        // The reason to do something like this instead of just flipping a boolean is to manage side effects
        // This ensures every time the flashlight is toggle off, the light source also always turns off
        public UnityEvent<InputAction> TogglePower = new();

        // For when power is toggled off by the player via InputRouter
        public void OnTogglePower(InputAction _)
        {
            IsOn = !IsOn;
            LightSource.enabled = IsOn;
        }

        // For when power is toggled off by the game
        public void OnTogglePower(bool value)
        {
            IsOn = value;
            LightSource.enabled = IsOn;
        }

        public void Start()
        {
            LightSource = Helpers.Debug.TryFindComponentInChildren<Light>(gameObject);
            GrabInteractable = Helpers.Debug.TryFindComponent<XRGrabInteractable>(gameObject);
            TogglePower.AddListener(OnTogglePower);
            OnTogglePower(false);
        }

        public void Update()
        {
            if (IsOn)
            {
                Battery.Value -= Time.deltaTime;
            }

            if (Battery.Value <= 0)
            {
                OnTogglePower(false);
            }
        }
    }
}
