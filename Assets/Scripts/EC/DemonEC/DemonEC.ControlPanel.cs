using System.Collections;
using System.Collections.Generic;
using Flashlight;
using UnityEngine;
using UnityEngine.AI;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    public class ControlPanel : MonoBehaviour
    {

        public GameObject FlashlightPrefab;

        [Helpers.DisableInEditor] public GameObject SpawnedFlashlight;

        public GameObject JumpscareTarget;

        [HideInInspector] public EventBus EventBus;

        [HideInInspector] public FxController FXController;

        [HideInInspector] public Jumpscare Jumpscare;

        [HideInInspector] public Health Health;

        [HideInInspector] public Pathing Pathing;

        public Helpers.Events.Channels.GameObjectEC NavBeaconEC;

        public List<GameObject> NavBeacons = new();

        public Helpers.NonPersistentListenerTracker ListenerTracker = new();

        private NavMeshAgent _navMeshAgent;

        private void Awake() {
            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);
        }

        public void TogglePathing() {
            Helpers.Nav.TogglePathing(_navMeshAgent);
        }

        public List<Component> GetInitializedComponents() {
            var components = new List<Component>();

            if (string.IsNullOrEmpty(gameObject.scene.name)) return components;

            EventBus = gameObject.GetComponent<EventBus>();
            FXController = gameObject.GetComponentInChildren<FxController>();
            Health = gameObject.GetComponent<Health>();
            Jumpscare = gameObject.GetComponent<Jumpscare>();
            Pathing = gameObject.GetComponent<Pathing>();

            if (EventBus)
            {
                EventBus.Awake();
                components.Add(EventBus);
            }

            if (FXController)
            {
                FXController.Awake();
                components.Add(FXController);
            }

            if (Health)
            {
                Health.Awake();
                components.Add(Health);
            }

            if (Jumpscare)
            {
                Jumpscare.Awake();
                components.Add(Jumpscare);
            }

            if (Pathing)
            {
                Pathing.Awake();
                components.Add(Pathing);
            }

            NavBeacons = NavBeaconEC.CollectedParams;

            return components;
        }

        public void SpawnAndTestFlashlight() {
            if (!FlashlightPrefab) return;

            DestroyFlashlight();

            var flashlightSpawnPoint = new Vector3(0, 3, -5);

            SpawnedFlashlight = Instantiate(
                    FlashlightPrefab,
                    transform.position + flashlightSpawnPoint,
                    Quaternion.identity
                );

            if (SpawnedFlashlight.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.useGravity = false;

                SpawnedFlashlight.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

                StartCoroutine(ToggleFlashlightDelay());
            }
            else
            {
                DestroyFlashlight();
            }
        }

        private IEnumerator ToggleFlashlightDelay() {
            yield return new WaitForSeconds(1);

            if (!SpawnedFlashlight) yield break;

            if (SpawnedFlashlight.TryGetComponent<Shake>(out var shake))
                shake.FlashlightToggled.Invoke(true);
            else
                DestroyFlashlight();
        }

        public void DestroyFlashlight() {
            if (!SpawnedFlashlight) return;

            Destroy(SpawnedFlashlight);
            SpawnedFlashlight = null;
        }

        public void TestJumpscare() {
            if (_navMeshAgent
                && Camera.main != null
                && JumpscareTarget)
                Pathing.GoTo(JumpscareTarget.gameObject);
        }

        public void ResetJumpscare() {
            gameObject.transform.position = Vector3.zero;

            if (_navMeshAgent) _navMeshAgent.enabled = true;

            EventBus.JumpscareTriggered.AddListener(Jumpscare.PositionForJumpscare);

            ListenerTracker.Add(EventBus, nameof(EventBus.JumpscareTriggered), nameof(Jumpscare.PositionForJumpscare));

            FXController.EndJumpscare();
            _navMeshAgent.ResetPath();
        }

    }
}