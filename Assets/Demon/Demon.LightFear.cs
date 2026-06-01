using Helpers;
using UnityEngine;
using UnityEngine.Events;

namespace Demon
{
    public class LightFear : MonoBehaviour
    {
        public ClampedFloat Health = new(3f, 3f);

        public UnityEvent<bool> Illuminate = new();

        // This happens every frame the Flashlight is intersecting the Demon
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                var shake = other.GetComponentInParent<FlashlightShake>();
                if (shake.FlashlightActive)
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

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Flashlight"))
            {
                Illuminate.Invoke(false);
            }
        }

        // In case other things want to respond, the Demon being destroyed is wrapped in an event
        // When Banish is invoked, it sets a marker to destroy it in LateUpdate() which is the same as Update() except it runs after everything
        // EventListeners are executed in the order they're added, this basically ensure that the actual destroy runs after everything else
        public UnityEvent<GameObject> Banish = new();

        public void StartDelayedDestroy(GameObject _)
        {
            _destroyInLateUpdate = true;
        }

        private bool _destroyInLateUpdate = false;

        public void LateUpdate()
        {
            if (_destroyInLateUpdate)
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            Banish.AddListener(StartDelayedDestroy);
        }

        // For seeing values in the inspector, can remove for production if desired
        public float InspectorHealth;

        public void OnValidate()
        {
            InspectorHealth = Health.Value;
        }
    }
}
