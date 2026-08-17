using Helpers.Attributes;
using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;

namespace EC.Demon.Fx
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(AudioSource))]
	[AddComponentMenu("EC.Demon.Fx.Controller")]
	public class Controller : MonoBehaviour
	{
		private static readonly int _illuminated = Animator.StringToHash("Illuminated");

		private static readonly int _banish = Animator.StringToHash("Banish");

		private static readonly int _jumpscare = Animator.StringToHash("Jumpscare");

		private static readonly int _endJumpscare = Animator.StringToHash("EndJumpscare");

		[DisableInEditor] [SerializeField] private Animator _animator;

		[DisableInEditor] [SerializeField] [UsedImplicitly] private AudioSource _audioSource;

		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		// [DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

		[UsedImplicitly]
		public void Awake()
		{
			_animator = gameObject.TryFindComponent<Animator>();
			_audioSource = gameObject.TryFindComponent<AudioSource>();

			if (gameObject.transform.parent.gameObject.activeInHierarchy)
				_eventBus = gameObject.TryFindComponentInParent<EventBus>();
			// _controlPanel = gameObject.TryFindComponentInParent<ControlPanel>();
		}

		public void OnEnable()
		{
			_eventBus.JumpscareTriggered.AddListener(OnJumpscare);
			_eventBus.Illuminated.AddListener(OnIlluminated);
			_eventBus.BanishTriggered.AddListener(OnBanish);

			// if (_controlPanel)
			// {
			// 	_controlPanel.ListenerTracker.Add(this, nameof(_eventBus.JumpscareTriggered), nameof(OnJumpscare));
			// 	_controlPanel.ListenerTracker.Add(this, nameof(_eventBus.Illuminated), nameof(OnIlluminated));
			// 	_controlPanel.ListenerTracker.Add(this, nameof(_eventBus.BanishTriggered), nameof(OnBanish));
			// }
		}

		/// <summary>
		///     Only called during testing
		/// </summary>
		public void StopJumpscare() => _animator.SetTrigger(_endJumpscare);

		public void OnBanish(GameObject _) => _animator.SetTrigger(_banish);

		public void OnIlluminated(bool isIlluminated)
		{
			if (_animator.GetBool(_illuminated) != isIlluminated) _animator.SetBool(_illuminated, isIlluminated);
		}

		public void OnJumpscare(GameObject _) => _animator.SetTrigger(_jumpscare);
	}
}