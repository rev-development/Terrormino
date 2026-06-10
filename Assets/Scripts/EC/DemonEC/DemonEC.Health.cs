using Flashlight;
using UnityEngine;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {

        [Helpers.DisableInEditor] [SerializeField] private Controller _controller;

        [SerializeField] private Helpers.ClampedFloat _hp = new(3f, 3f);

        public void Awake()
        {
            _controller = Helpers.Debug.TryFindComponentInParent<Controller>(gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                _controller.Illuminated.Invoke(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                var shake = other.GetComponentInParent<Shake>();

                if (shake.IsActive)
                {
                    _controller.Illuminated.Invoke(true);
                    _hp.Value -= Time.deltaTime;

                    if (_hp.Value <= 0)
                    {
                        _controller.BanishTriggered.Invoke(gameObject);
                    }
                }
                else
                {
                    _controller.Illuminated.Invoke(false);
                }
            }
        }

    }
}