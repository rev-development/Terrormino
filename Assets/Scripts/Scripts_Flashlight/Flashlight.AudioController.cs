using UnityEngine;

namespace Flashlight
{
    public class AudioController : MonoBehaviour
    {

        public AudioSource ShakeSound;

        public AudioSource NoChargeSound;

        public Shake Shake;

        public void Start()
        {
            Shake ??= Helpers.Debug.TryFindComponent<Shake>(gameObject);
        }

        public void OnEnable()
        {
            if (!Shake)
            {
                return;
            }

            Shake.FlashlightToggled.AddListener(OnFlashlightToggle);
            Shake.FlashlightShaking.AddListener(OnFlashlightShaking);
        }

        private void OnFlashlightToggle(bool isActive)
        {
            if (!isActive)
            {
                NoChargeSound.Play();
            }
        }

        private void OnFlashlightShaking(bool isShaking)
        {
            if (isShaking)
            {
                ShakeSound.Play();
            }
            else
            {
                ShakeSound.Stop();
            }
        }

    }
}