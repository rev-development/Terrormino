using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Controller : MonoBehaviour
    {

        [Helpers.DisableInEditor] public FxController FxController;

        [Helpers.DisableInEditor] public Health Health;

        [SerializeField] private Helpers.Events.Channels.GameObjectEC _removeDemon;

        [SerializeField] private Helpers.Events.Channels.VoidEC _gameOver;

        public UnityEvent<GameObject> BanishTriggered = new();

        public UnityEvent<GameObject> JumpscareTriggered = new();

        public UnityEvent JumpscareFxCompleted = new();

        public UnityEvent BanishFxCompleted = new();

        public UnityEvent<bool> Illuminated = new();

        [Helpers.DisableInEditor] private ControlPanel _controlPanel;

        private Transform _mainCameraTransform;

        private NavMeshAgent _navMeshAgent;

        private Rigidbody _rb;

        public void Awake()
        {
            FxController = Helpers.Debug.TryFindComponentInChildren<FxController>(gameObject);

            Health = Helpers.Debug.TryFindComponent<Health>(gameObject);

            _navMeshAgent = Helpers.Debug.TryFindComponent<NavMeshAgent>(gameObject);
            _rb = Helpers.Debug.TryFindComponent<Rigidbody>(gameObject);

            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }

            _controlPanel = Helpers.Debug.TryFindComponent<ControlPanel>(gameObject);
        }

        private void OnEnable()
        {
            JumpscareTriggered.AddListener(PositionForJumpscare);
            BanishFxCompleted.AddListener(OnBanishFxCompleted);
            JumpscareFxCompleted.AddListener(OnJumpscareFxCompleted);

            if (_controlPanel)
            {
                _controlPanel.AddNonPersistentListener(this, nameof(JumpscareTriggered), nameof(PositionForJumpscare));
                _controlPanel.AddNonPersistentListener(this, nameof(BanishFxCompleted), nameof(OnBanishFxCompleted));

                _controlPanel.AddNonPersistentListener(
                        this,
                        nameof(JumpscareFxCompleted),
                        nameof(OnJumpscareFxCompleted)
                    );
            }
        }

        private void OnDisable()
        {
            BanishTriggered.RemoveAllListeners();
            JumpscareTriggered.RemoveAllListeners();
            JumpscareFxCompleted.RemoveAllListeners();
            BanishFxCompleted.RemoveAllListeners();
            Illuminated.RemoveAllListeners();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                JumpscareTriggered.Invoke(other.gameObject);
            }
        }

        public void PositionForJumpscare(GameObject player)
        {
            JumpscareTriggered.RemoveListener(PositionForJumpscare);

            if (_controlPanel)
            {
                _controlPanel.RemoveNonPersistentListener(
                        this,
                        nameof(JumpscareTriggered),
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

        private void OnJumpscareFxCompleted()
        {
            _gameOver.RaiseEvent();
        }

        private void OnBanishFxCompleted()
        {
            _removeDemon.RaiseEvent(gameObject);
        }

    }
}