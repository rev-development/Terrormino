using Helpers.Ext;
using UnityEngine;

namespace Flashlight
{
	public class AudioController : MonoBehaviour
	{
		public AudioSource ShakeSound;

		public AudioSource NoChargeSound;

		public Shake Shake;

		public void OnEnable()
		{
			if (!Shake) return;

			Shake.FlashlightToggled.AddListener(OnFlashlightToggle);
			Shake.FlashlightShaking.AddListener(OnFlashlightShaking);
		}

		public void Start() => Shake ??= gameObject.TryFindComponent<Shake>();

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