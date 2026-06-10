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

        private Controller _controller;

        private FxController _fxController;

        private Health _health;

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
            _controller = gameObject.GetComponent<Controller>();

            if (_controller)
            {
                _controller.Awake();
            }

            return _controller;
        }

        public List<Component> GetInitializedSubcomponents()
        {
            var subcomponents = new List<Component>();


            if (!_controller
                || !_controller.FxController
                || !_controller.Health)
            {
                GetInitializedMainComponent();
            }

            if (!_controller)
            {
                return new List<Component>();
            }

            _fxController = _controller.FxController;
            _health = _controller.Health;

            if (_fxController)
            {
                _fxController.Awake();
                subcomponents.Add(_fxController);
            }

            if (_health)
            {
                _health.Awake();
                subcomponents.Add(_health);
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
            _controller.ModelRoot.transform.localPosition = Vector3.zero;

            if (_navMeshAgent)
            {
                _navMeshAgent.enabled = true;
            }

            _controller.JumpscareTriggered.AddListener(_controller.PositionForJumpscare);

            AddNonPersistentListener(
                    _controller,
                    nameof(_controller.JumpscareTriggered),
                    nameof(_controller.PositionForJumpscare)
                );

            _controller.FxController.EndJumpscare();
            _navMeshAgent.ResetPath();
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