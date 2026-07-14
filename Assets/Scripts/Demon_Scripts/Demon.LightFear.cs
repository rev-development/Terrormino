using Flashlight;
using UnityEngine;
using UnityEngine.Events;

namespace Demon
{
    public class LightFear : MonoBehaviour
    {

        public Helpers.ClampedFloat Health = new(3f, 3f);

        public UnityEvent<bool> Illuminate = new();

        // In case other things want to respond, the Demon being destroyed is wrapped in an event
        // When Banish is invoked, it sets a marker to destroy it in LateUpdate() which is the same as Update() except it runs after everything
        // EventListeners are executed in the order they're added, this basically ensure that the actual destroy runs after everything else
        public UnityEvent<GameObject> Banish = new();

        public Helpers.ClampedFloat InspectorHealth = new(3f, 3f);

        private bool _destroyInLateUpdate = false;

        public void LateUpdate()
        {
            if (_destroyInLateUpdate)
            {
                Destroy(gameObject);
            }
        }

        public void OnEnable()
        {
            Banish.AddListener(StartDelayedDestroy);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player.Manager playerManager))
            {
                playerManager.GameOver.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                Illuminate.Invoke(false);
            }
        }

        // This happens every frame the Flashlight is intersecting the Demon
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                var shake = other.GetComponentInParent<Shake>();

                if (shake.IsActive)
                {
                    Health.Value -= Time.deltaTime;
                    Illuminate.Invoke(true);

                    if (Health.Value <= 0)
                    {
                        Banish.Invoke(gameObject);
                    }
                }
                else
                {
                    Illuminate.Invoke(false);
                }
            }
        }

        public void OnValidate()
        {
            InspectorHealth.Value = Health.Value;
        }

        public void StartDelayedDestroy(GameObject _)
        {
            _destroyInLateUpdate = true;
        }

    }
}