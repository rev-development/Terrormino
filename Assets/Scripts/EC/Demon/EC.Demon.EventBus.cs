using UnityEngine;
using UnityEngine.Events;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[AddComponentMenu("EC.Demon.EventBus")]
	public class EventBus : MonoBehaviour
	{
		[SerializeField] private Helpers.Events.Channels.GameObjectEC _removeDemon;

		[SerializeField] private Helpers.Events.Channels.VoidEC _gameOver;

		public UnityEvent<GameObject> BanishTriggered = new();

		public UnityEvent<GameObject> JumpscareTriggered = new();

		public UnityEvent JumpscareFxCompleted = new();

		public UnityEvent BanishFxCompleted = new();

		public UnityEvent<bool> Illuminated = new();

		[field: SerializeField] public Config Config { get; private set; } = new();

		[Helpers.DisableInEditorAttribute] [SerializeField] private ControlPanel _controlPanel;

		public void Awake() => _controlPanel = Helpers.Debug.TryFindComponent<ControlPanel>(gameObject);

		private void OnEnable()
		{
			BanishFxCompleted.AddListener(OnBanishFxCompleted);
			JumpscareFxCompleted.AddListener(OnJumpscareFxCompleted);

			if (_controlPanel)
			{
				_controlPanel.ListenerTracker.Add(this, nameof(BanishFxCompleted), nameof(OnBanishFxCompleted));

				_controlPanel.ListenerTracker.Add(this, nameof(JumpscareFxCompleted), nameof(OnJumpscareFxCompleted));
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

		/// <summary>
		///     Not necessary at the moment, but preventing raw Config assignment is good practice.
		///     Ensures any cascading changes are enforced.
		/// </summary>
		/// <param name="config"></param>
		public void ApplyConfig(Config config) => Config = config;

		private void OnBanishFxCompleted() => _removeDemon.RaiseEvent(gameObject);

		private void OnJumpscareFxCompleted() => _gameOver.RaiseEvent();
	}
}