using UnityEngine;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class FxController : MonoBehaviour
    {

        private static readonly int _illuminated = Animator.StringToHash("Illuminated");

        private static readonly int _banish = Animator.StringToHash("Banish");

        private static readonly int _jumpscare = Animator.StringToHash("Jumpscare");

        private static readonly int _endJumpscare = Animator.StringToHash("EndJumpscare");

        [Helpers.DisableInEditor] [SerializeField] private Animator _animator;

        [Helpers.DisableInEditor] [SerializeField] private AudioSource _audioSource;

        [Helpers.DisableInEditor] [SerializeField] private EventBus _eventBus;

        [Helpers.DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

        public AudioClip Scream;

        public void Awake() {
            _animator = Helpers.Debug.TryFindComponent<Animator>(gameObject);
            _audioSource = Helpers.Debug.TryFindComponent<AudioSource>(gameObject);
            _eventBus = Helpers.Debug.TryFindComponentInParent<EventBus>(gameObject);
            _controlPanel = Helpers.Debug.TryFindComponentInParent<ControlPanel>(gameObject);
        }

        public void OnEnable() {
            _eventBus.JumpscareTriggered.AddListener(OnJumpscare);
            _eventBus.Illuminated.AddListener(OnIlluminated);
            _eventBus.BanishTriggered.AddListener(OnBanish);

            if (_controlPanel)
            {
                _controlPanel.ListenerTracker.Add(this, nameof(_eventBus.JumpscareTriggered), nameof(OnJumpscare));
                _controlPanel.ListenerTracker.Add(this, nameof(_eventBus.Illuminated), nameof(OnIlluminated));
                _controlPanel.ListenerTracker.Add(this, nameof(_eventBus.BanishTriggered), nameof(OnBanish));
            }
        }

        public void OnJumpscare(GameObject _) {
            _animator.SetTrigger(_jumpscare);
        }

        public void OnBanish(GameObject _) {
            _animator.SetTrigger(_banish);
        }

        public void OnIlluminated(bool isIlluminated) {
            if (_animator.GetBool(_illuminated) != isIlluminated) _animator.SetBool(_illuminated, isIlluminated);
        }

        /// <summary>
        ///     Only called during testing
        /// </summary>
        public void EndJumpscare() {
            _animator.SetTrigger(_endJumpscare);
        }

    }
}