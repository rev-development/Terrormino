using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ambient
{
    public class Manager : MonoBehaviour
    {
        public List<Effect> Effects;
        public List<GameObject> TrackedObjects;

        public void Update()
        {
            TriggerEffects();
        }

        public void Start()
        {
            Effects.AddRange(gameObject.GetComponents<Effect>());
        }

        public List<GameObject> UnwatchedObjects
        {
            get
            {
                var camera = Camera.main;
                var planes = GeometryUtility.CalculateFrustumPlanes(camera);

                return TrackedObjects
                    .Where(trackedObject =>
                    {
                        if (trackedObject.TryGetComponent<Collider>(out Collider collider))
                        {
                            return !GeometryUtility.TestPlanesAABB(planes, collider.bounds);
                        }
                        else
                        {
                            Debug.Log(
                                $"No Collider Component found on {trackedObject.name}",
                                trackedObject
                            );
                            return false;
                        }
                    })
                    .ToList();
            }
        }

        void TriggerEffects()
        {
            foreach (Effect effect in Effects)
            {
                effect.TriggerEffect.Invoke(UnwatchedObjects);
            }
        }
    }
}
