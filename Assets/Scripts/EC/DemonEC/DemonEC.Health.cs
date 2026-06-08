using Flashlight;
using UnityEngine;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {

        public UnityEvent<bool> Illuminated = new();

        public UnityEvent<GameObject> LocalBanished = new();

        private readonly Helpers.ClampedFloat _health = new(3f, 3f);

        private void OnDisable()
        {
            Illuminated.RemoveAllListeners();
            LocalBanished.RemoveAllListeners();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                Illuminated.Invoke(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                var shake = other.GetComponentInParent<Shake>();

                if (shake.IsActive)
                {
                    Illuminated.Invoke(true);
                    _health.Value -= Time.deltaTime;

                    if (_health.Value <= 0)
                    {
                        LocalBanished.Invoke(gameObject);
                    }
                }
                else
                {
                    Illuminated.Invoke(false);
                }
            }
        }

    }
}