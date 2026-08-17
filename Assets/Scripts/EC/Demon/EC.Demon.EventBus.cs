using Helpers.Events.Channels;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Demon
{
	[DisallowMultipleComponent]
	[AddComponentMenu("EC.Demon.EventBus")]
	public class EventBus : MonoBehaviour
	{
		[SerializeField] private GameObjectEC _removeDemon;

		[SerializeField] private VoidEC _gameOver;

		public UnityEvent<GameObject> BanishTriggered = new();

		public UnityEvent<GameObject> JumpscareTriggered = new();

		public UnityEvent JumpscareFxCompleted = new();

		public UnityEvent BanishFxCompleted = new();

		public UnityEvent<bool> Illuminated = new();

		[field: SerializeField] public ConfigSO ConfigSO { get; private set; }

		// [DisableInEditor] [SerializeField] private ControlPanel _controlPanel;

		// [UsedImplicitly] public void Awake() => _controlPanel = gameObject.TryFindComponent<ControlPanel>();

		private void OnEnable()
		{
			BanishFxCompleted.AddListener(OnBanishFxCompleted);
			JumpscareFxCompleted.AddListener(OnJumpscareFxCompleted);

			// if (_controlPanel)
			// {
			// 	_controlPanel.ListenerTracker.Add(this, nameof(BanishFxCompleted), nameof(OnBanishFxCompleted));
			//
			// 	_controlPanel.ListenerTracker.Add(this, nameof(JumpscareFxCompleted), nameof(OnJumpscareFxCompleted));
			// }
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
		/// <param name="configSO"></param>
		public void ApplyConfig(ConfigSO configSO) => ConfigSO = configSO;

		private void OnBanishFxCompleted() => _removeDemon.RaiseEvent(gameObject);

		private void OnJumpscareFxCompleted() => _gameOver.RaiseEvent();
	}
}