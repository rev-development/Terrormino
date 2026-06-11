using Flashlight;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public List<NonPersistentListenerDisplay> NonPersistentListeners = new();

        [HideInInspector] public Controller Controller;

        [HideInInspector] public FxController FXController;

        [HideInInspector] public Health Health;

        private NavMeshAgent _navMeshAgent;

        private void Awake()
        {
            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);
        }

        public void AddNonPersistentListener(Component component, string unityEvent, string unityAction)
        {
            NonPersistentListeners.Add(new NonPersistentListenerDisplay(component, unityEvent, unityAction));
        }

        public void RemoveNonPersistentListener(Component component, string unityEvent, string unityAction)
        {
            var match = NonPersistentListeners.Find(nonPersistentListener =>
                    nonPersistentListener.Component == component
                    && nonPersistentListener.UnityEvent == unityEvent
                    && nonPersistentListener.UnityAction == unityAction
                );

            if (match != null)
            {
                NonPersistentListeners.Remove(match);
            }
        }

        public Component GetInitializedMainComponent()
        {
            Controller = gameObject.GetComponent<Controller>();

            if (Controller)
            {
                Controller.Awake();
            }

            return Controller;
        }

        public List<Component> GetInitializedSubcomponents()
        {
            var subcomponents = new List<Component>();

            if (!Controller
                || !Controller.FxController
                || !Controller.Health)
            {
                GetInitializedMainComponent();
            }

            if (!Controller)
            {
                return new List<Component>();
            }

            FXController = Controller.FxController;
            Health = Controller.Health;

            if (FXController)
            {
                FXController.Awake();
                subcomponents.Add(FXController);
            }

            if (Health)
            {
                Health.Awake();
                subcomponents.Add(Health);
            }

            return subcomponents;
        }

        public void SpawnAndTestFlashlight()
        {
            if (!FlashlightPrefab)
            {
                return;
            }

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

        private IEnumerator ToggleFlashlightDelay()
        {
            yield return new WaitForSeconds(1);

            if (!SpawnedFlashlight)
            {
                yield break;
            }

            if (SpawnedFlashlight.TryGetComponent<Shake>(out var shake))
            {
                shake.FlashlightToggled.Invoke(true);
            }
            else
            {
                DestroyFlashlight();
            }
        }

        public void DestroyFlashlight()
        {
            if (!SpawnedFlashlight)
            {
                return;
            }

            Destroy(SpawnedFlashlight);
            SpawnedFlashlight = null;
        }

        public void TestJumpscare()
        {
            if (_navMeshAgent
                && Camera.main != null
                && JumpscareTarget)
            {
                _navMeshAgent.SetDestination(JumpscareTarget.transform.position);
            }
        }

        public void ResetJumpscare()
        {
            gameObject.transform.position = Vector3.zero;

            if (_navMeshAgent)
            {
                _navMeshAgent.enabled = true;
            }

            Controller.JumpscareTriggered.AddListener(Controller.PositionForJumpscare);

            AddNonPersistentListener(
                    Controller,
                    nameof(Controller.JumpscareTriggered),
                    nameof(Controller.PositionForJumpscare)
                );

            Controller.FxController.EndJumpscare();
            _navMeshAgent.ResetPath();
        }

        public void TogglePathing()
        {
            _navMeshAgent.isStopped = !_navMeshAgent.isStopped;

            if (_navMeshAgent.isStopped)
            {
                _navMeshAgent.velocity = Vector3.zero;
            }
        }

        [Serializable]
        public class NonPersistentListenerDisplay
        {

            [SerializeField] public Component Component;

            [SerializeField] public string UnityEvent;

            [SerializeField] public string UnityAction;

            public NonPersistentListenerDisplay(Component component, string unityEvent, string unityAction)
            {
                Component = component;
                UnityEvent = unityEvent;
                UnityAction = unityAction;
            }

        }

    }
}