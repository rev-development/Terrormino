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

        [Helpers.DisableInEditor] [SerializeField] private Controller _controller;

        [Helpers.DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

        public AudioClip Scream;

        public void Awake()
        {
            _animator = Helpers.Debug.TryFindComponent<Animator>(gameObject);
            _audioSource = Helpers.Debug.TryFindComponent<AudioSource>(gameObject);
            _controller = Helpers.Debug.TryFindComponentInParent<Controller>(gameObject);
            _controlPanel = Helpers.Debug.TryFindComponentInParent<ControlPanel>(gameObject);
        }

        public void OnEnable()
        {
            _controller.JumpscareTriggered.AddListener(OnJumpscare);
            _controller.Illuminated.AddListener(OnIlluminated);
            _controller.BanishTriggered.AddListener(OnBanish);

            if (_controlPanel)
            {
                _controlPanel.AddNonPersistentListener(
                        this,
                        nameof(_controller.JumpscareTriggered),
                        nameof(OnJumpscare)
                    );

                _controlPanel.AddNonPersistentListener(this, nameof(_controller.Illuminated), nameof(OnIlluminated));
                _controlPanel.AddNonPersistentListener(this, nameof(_controller.BanishTriggered), nameof(OnBanish));
            }
        }

        /// <summary>
        ///     This is called from an animation event at the beginning of the "Jumpscare" Animation.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void PlayJumpscareAudio()
        {
            _audioSource.PlayOneShot(Scream);
        }

        public void OnJumpscare(GameObject _)
        {
            _animator.SetTrigger(_jumpscare);
        }

        /// <summary>
        ///     This is called from an animation event at the end of the "Banish" Animation.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void AfterBanishFx()
        {
            _controller.BanishFxCompleted.Invoke();
        }

        /// <summary>
        ///     This is called from an animation event at the end of the "Jumpscare" Animation.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private void AfterJumpscareFx()
        {
            _controller.JumpscareFxCompleted.Invoke();
        }

        public void OnBanish(GameObject _)
        {
            _animator.SetTrigger(_banish);
        }

        public void OnIlluminated(bool isIlluminated)
        {
            _animator.SetBool(_illuminated, isIlluminated);
        }

        /// <summary>
        ///     Only called during testing
        /// </summary>
        public void EndJumpscare()
        {
            _animator.SetTrigger(_endJumpscare);
        }

    }
}