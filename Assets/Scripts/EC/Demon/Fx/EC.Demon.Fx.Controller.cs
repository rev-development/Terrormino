using UnityEngine;

namespace EC.Demon.Fx
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("EC.Demon.Fx.Controller")]
	public class Controller : MonoBehaviour
	{
		private static readonly int Illuminated = Animator.StringToHash("Illuminated");

		private static readonly int Banish = Animator.StringToHash("Banish");

		private static readonly int Jumpscare = Animator.StringToHash("Jumpscare");

		private static readonly int EndJumpscare = Animator.StringToHash("EndJumpscare");

		[Helpers.DisableInEditorAttribute] [SerializeField] private Animator _animator;

		[Helpers.DisableInEditorAttribute] [SerializeField] private AudioSource _audioSource;

		[Helpers.DisableInEditorAttribute] [SerializeField] private EventBus _eventBus;

		[Helpers.DisableInEditorAttribute] [SerializeField] private ControlPanel _controlPanel;

		public AudioClip Scream;

		public void Awake()
		{
			_animator = Helpers.Debug.TryFindComponent<Animator>(gameObject);
			_audioSource = Helpers.Debug.TryFindComponent<AudioSource>(gameObject);
			_eventBus = Helpers.Debug.TryFindComponentInParent<EventBus>(gameObject);
			_controlPanel = Helpers.Debug.TryFindComponentInParent<ControlPanel>(gameObject);
		}

		public void OnEnable()
		{
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

		/// <summary>
		///     Only called during testing
		/// </summary>
		public void StopJumpscare()
		{
			_animator.SetTrigger(EndJumpscare);
		}

		public void OnBanish(GameObject _)
		{
			_animator.SetTrigger(Banish);
		}

		public void OnIlluminated(bool isIlluminated)
		{
			if (_animator.GetBool(Illuminated) != isIlluminated) _animator.SetBool(Illuminated, isIlluminated);
		}

		public void OnJumpscare(GameObject _)
		{
			_animator.SetTrigger(Jumpscare);
		}
	}
}