using Helpers.Ext;
using UnityEngine;

namespace EC.Flashlight
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Shake))]
	[AddComponentMenu("EC.Flashlight.AudioController")]
	public class AudioController : MonoBehaviour
	{
		public AudioSource ShakeSound;

		public AudioSource NoChargeSound;

		private Shake _shake;

		public void Awake() => _shake = gameObject.TryFindComponent<Shake>();

		public void OnEnable()
		{
			_shake.FlashlightToggled.AddListener(OnFlashlightToggle);
			_shake.FlashlightShaking.AddListener(OnFlashlightShaking);
		}

		public void OnDisable()
		{
			_shake.FlashlightToggled.RemoveListener(OnFlashlightToggle);
			_shake.FlashlightShaking.RemoveListener(OnFlashlightShaking);
		}

		private void OnFlashlightShaking(bool isShaking)
		{
			if (isShaking)
				ShakeSound.Play();
			else
				ShakeSound.Stop();
		}

		private void OnFlashlightToggle(bool isActive)
		{
			if (!isActive) NoChargeSound.Play();
		}
	}
}