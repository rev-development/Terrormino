using UnityEngine;
using UnityEngine.AI;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(EventBus))]
    public class Jumpscare : MonoBehaviour
    {

        [Helpers.DisableInEditor] [SerializeField] private EventBus _eventBus;

        [Helpers.DisableInEditor] private ControlPanel _controlPanel;

        private Transform _mainCameraTransform;

        private NavMeshAgent _navMeshAgent;

        public void PositionForJumpscare(GameObject player)
        {
            _eventBus.JumpscareTriggered.RemoveListener(PositionForJumpscare);

            if (_controlPanel)
            {
                _controlPanel.ListenerTracker.Remove(
                        this,
                        nameof(_eventBus.JumpscareTriggered),
                        nameof(PositionForJumpscare)
                    );
            }

            _navMeshAgent.isStopped = true;
            _navMeshAgent.ResetPath();
            _navMeshAgent.velocity = Vector3.zero;
            _navMeshAgent.enabled = false;

            var newPosition = _mainCameraTransform.position + (_mainCameraTransform.forward * 4.5f);

            newPosition.y = Helpers.Bounds.GetComplexCapsuleBounds(gameObject).size.y
                            - _mainCameraTransform.transform.position.y;

            gameObject.transform.position = newPosition;

            var newRotation = Quaternion.LookRotation(-_mainCameraTransform.forward, _mainCameraTransform.up);
            gameObject.transform.rotation = newRotation;
        }

        #region Event Functions

        public void Awake()
        {
            _eventBus = Helpers.Debug.TryFindComponent<EventBus>(gameObject);
            _controlPanel = Helpers.Debug.TryFindComponent<ControlPanel>(gameObject);

            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);

            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }
        }

        private void OnEnable()
        {
            _eventBus.JumpscareTriggered.AddListener(PositionForJumpscare);

            if (_controlPanel)
            {
                _controlPanel.ListenerTracker.Add(
                        this,
                        nameof(_eventBus.JumpscareTriggered),
                        nameof(PositionForJumpscare)
                    );
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _eventBus.JumpscareTriggered.Invoke(other.gameObject);
            }
        }

        #endregion

    }
}