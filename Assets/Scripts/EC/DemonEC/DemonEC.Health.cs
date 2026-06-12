using Flashlight;
using UnityEngine;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EventBus))]
    public class Health : MonoBehaviour
    {

        [Helpers.DisableInEditor] [SerializeField] private EventBus _eventBus;

        [field: SerializeField] public Helpers.ClampedFloat HP = new(3f, 3f);

        public void Awake()
        {
            _eventBus = Helpers.Debug.TryFindComponentInParent<EventBus>(gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                _eventBus.Illuminated.Invoke(false);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                var shake = other.GetComponentInParent<Shake>();

                if (shake.IsActive)
                {
                    _eventBus.Illuminated.Invoke(true);
                    HP.Value -= Time.deltaTime;

                    if (HP.Value <= 0)
                    {
                        _eventBus.BanishTriggered.Invoke(gameObject);
                    }
                }
                else
                {
                    _eventBus.Illuminated.Invoke(false);
                }
            }
        }

    }
}