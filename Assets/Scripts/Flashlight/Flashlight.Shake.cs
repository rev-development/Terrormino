using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Flashlight
{
	[DisallowMultipleComponent]
	public class Shake : MonoBehaviour
	{
		public Light LightSource;

		public void Charge(Vector3 position)
		{
			// Originally there was an if statement to make sure that cachedMagnitude was not an abysmally small number
			// Instead of doing an if statement, just truncate the float to the precision you want, then it's never a problem
			var cachedMagnitude = Helpers.Math.RoundFloatToDecimalPlaces(_cachedVelocity.magnitude, 2);
			var currentMagnitude = position.magnitude;
			_cachedVelocity = Vector3.Lerp(_cachedVelocity, position, SmoothingFactor);

			var percentageIncrease
				= Mathf.Abs((currentMagnitude - _cachedVelocity.magnitude) / cachedMagnitude)
				  * 100f; //Math for checking the percentage increase between past magnitude and current magnitude

			if (percentageIncrease
				>= MinChargeMagnitudeThreshold) //checking to see if the current magnitude increased by 2.5% (i.e. shaking)
			{
				Battery += Time.deltaTime * 6f;
				FlashlightShaking.Invoke(true);
			}
			else
			{
				FlashlightShaking.Invoke(false);
			}

			_cachedVelocity = position;
		}

#region Input Functions

		public void OnControllerPositionInput(InputAction inputAction)
		{
			var position = inputAction.ReadValue<Vector3>();
			Charge(position);
		}

		public void OnControllerPositionInput(Vector3 position) => Charge(position);

		public void OnControllerTriggerInput(InputAction _) => FlashlightToggled.Invoke(!IsActive);

#endregion

#region Runtime Values

		/// <summary>
		///     Flag for knowing if the flashlight is currently active
		///     Should not be modified directly, use FlashlightToggled to ensure all side effects occur
		/// </summary>
		[field: Header("Runtime Values")]
		[field: SerializeField]
		[Tooltip("Flag for knowing if the flashlight is currently active")]
		public bool IsActive { get; private set; } = false;
		/// <summary>
		///     Battery Life
		/// </summary>
		[Tooltip("Battery Life")]
		[field: SerializeField]
		public float Battery { get; private set; } = 5f;
		/// <summary>
		///     Bool value backing shaking event
		///     Should not be modified directly, use FlashlightShaking to ensure all side effects occur
		/// </summary>
		[Tooltip("Bool value backing shaking event")]
		[field: SerializeField]
		public bool IsShaking { get; private set; } = false;
		/// <summary>
		///     Velocity value saved from last frame
		/// </summary>
		[Tooltip("Velocity value saved from last frame")] [SerializeField]
		private Vector3 _cachedVelocity = new();

#endregion

#region Config Values

		/// <summary>
		///     Minimum velocity magnitude to register charge
		/// </summary>
		[Header("Config Values")]
		[Tooltip("Minimum velocity magnitude to register charge")] public float MinChargeMagnitudeThreshold = 0.25f;
		/// <summary>
		///     Multiplier to help with noise from controller inputs
		/// </summary>
		[Tooltip("Multiplier to help with noise from controller inputs")]
		public float SmoothingFactor = 0.2f;

#endregion

#region Event Hooks

		/// <summary>
		///     Triggered when Flashlight is turned on/off
		/// </summary>
		[Header("Events")]
		[Tooltip("Triggered when Flashlight is turned on/off")] public UnityEvent<bool> FlashlightToggled = new();
		/// <summary>
		///     Triggered when Flashlight begins/stops shaking
		/// </summary>
		[Tooltip("Triggered when Flashlight begins/stops shaking")] public UnityEvent<bool> FlashlightShaking = new();

#endregion

#region Listeners

		private void OnShake(bool isShaking) => IsShaking = isShaking;

		private void OnFlashlightToggle(bool isActive)
		{
			IsActive = isActive;
			LightSource.enabled = isActive;
		}

#endregion

#region Lifecycle

		public void Awake() => LightSource ??= Helpers.Debug.TryFindComponentInChildren<Light>(gameObject);

		private void Start() => FlashlightToggled.Invoke(false);

		private void Update()
		{
			if (IsActive) // Flashlight battery drains when held
				Battery -= Time.deltaTime; //Battery continually loses charge

			if (Battery <= 0) // Battery dies
				FlashlightToggled.Invoke(false);
		}

		public void OnEnable()
		{
			FlashlightToggled.AddListener(OnFlashlightToggle);
			FlashlightShaking.AddListener(OnShake);
		}

		public void OnDisable()
		{
			FlashlightToggled.RemoveAllListeners();
			FlashlightShaking.RemoveAllListeners();
		}

#endregion
	}
}