using UnityEngine;
using UnityEngine.Events;

namespace EC.DemonEC
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {

        private static readonly int _takingDamage = Animator.StringToHash("Illuminated");

        private static readonly int _banish = Animator.StringToHash("Banish");

        private static readonly int _jumpscare = Animator.StringToHash("Jumpscare");

        [Helpers.DisableInEditorAttribute] [SerializeField] private Health _health;

        [Helpers.DisableInEditorAttribute] [SerializeField] private Animator _animator;

        public UnityEvent BanishAnimationEnded = new();

        [Helpers.DisableInEditorAttribute] [SerializeField] private Controller _demonController;

        public UnityEvent JumpscareAnimationEnded = new();

        private void Awake()
        {
            _demonController = Helpers.Debug.TryFindComponentInParent<Controller>(gameObject);

            if (_demonController)
            {
                _health = _demonController.Health;
            }

            _animator = Helpers.Debug.TryFindComponent<Animator>(gameObject);
        }

        private void OnEnable()
        {
            if (_demonController)
            {
                _demonController.JumpscareTriggered.AddListener(OnJumpscareTriggered);
            }

            if (_health)
            {
                _health.Illuminated.AddListener(OnIlluminated);
                _health.LocalBanished.AddListener(OnLocalBanished);
            }
        }

        private void OnDisable()
        {
            BanishAnimationEnded.RemoveAllListeners();
            JumpscareAnimationEnded.RemoveAllListeners();
        }

        private void OnJumpscareTriggered()
        {
            _animator.SetTrigger(_jumpscare);
        }

        /// <summary>
        ///     This is called from an animation event at the end of the dissolve animation clip.
        /// </summary>
        private void OnBanishAnimationEnd()
        {
            BanishAnimationEnded.Invoke();
        }

        /// <summary>
        ///     This is called from an animation event at the end of the jumpscare animation clip.
        /// </summary>
        private void OnJumpscareAnimationEnd()
        {
            JumpscareAnimationEnded.Invoke();
        }

        private void OnLocalBanished(GameObject _)
        {
            _animator.SetTrigger(_banish);
        }

        private void OnIlluminated(bool isIlluminated)
        {
            _animator.SetBool(_takingDamage, isIlluminated);
        }

    }
}