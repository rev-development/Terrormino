using Helpers.Attributes;
using Helpers.Events.Channels;
using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Tetris
{
	/// <summary>
	///     Single instance in the scene — the event surface for internal listeners
	///     (renderer, audio, animation) and the config holder Controller, InputAdapter,
	///     and PlayfieldRenderer all read through, rather than through each other. Also
	///     raises EventChannels for the two events external systems care about (lines
	///     cleared, game over). Future mechanics that need to intercept or react to
	///     these events hook directly into this bus — there is no separate stage.
	/// </summary>
	[DisallowMultipleComponent]
	[AiGenerated("Claude", "claude-sonnet-5", "Reviewed by Rev 7-28-26")]
	[AddComponentMenu("EC.Tetris.EventBus")]
	public class EventBus : MonoBehaviour
	{
		// EventChannels for the outward-facing audience of these two events
		// (NightManager, Player.Manager). The UnityEvents below cover the
		// local audience (audio, animation, renderer).
		[SerializeField] private IntEC _linesClearedEC;

		[SerializeField] private VoidEC _gameOverEC;

		[SerializeField] private VoidEC _gameStartEC;

		[field: SerializeField] public ConfigSO Config { get; private set; }

		public UnityEvent<Vector2Int> HorizontalMoveInput = new();

		public UnityEvent HorizontalMoveInputCancel = new();

		public UnityEvent<Vector2Int> DownMoveInput = new();

		public UnityEvent DownMoveInputCancel = new();

		public UnityEvent HardDropInput = new();

		public UnityEvent<int> RotateInput = new();

		public UnityEvent<Vector2Int> Moved = new();

		public UnityEvent<int> Rotated = new();

		public UnityEvent HardDropped = new();

		public UnityEvent<ActivePiece> Spawned = new();

		public UnityEvent Locked = new();

		public UnityEvent<int> LinesCleared = new();

		public UnityEvent GameStart = new();

		public UnityEvent GameStarted = new();

		public UnityEvent GameOver = new();

		[UsedImplicitly]
		public void Awake()
		{
			gameObject.CheckIfSetInInspector(_linesClearedEC, "Lines Cleared Event Channel");
			gameObject.CheckIfSetInInspector(_gameOverEC, "Game Over Event Channel");
			gameObject.CheckIfSetInInspector(_gameStartEC, "Game Start Event Channel");
		}

		private void OnEnable()
		{
			LinesCleared.AddListener(RaiseLinesClearedEC);
			GameOver.AddListener(RaiseGameOverEC);

			if (_gameStartEC) _gameStartEC.OnEventRaised += GameStart.Invoke;
		}

		private void OnDisable()
		{
			_gameStartEC.OnEventRaised -= GameStart.Invoke;
			Moved.RemoveAllListeners();
			HorizontalMoveInput.RemoveAllListeners();
			HorizontalMoveInputCancel.RemoveAllListeners();
			Rotated.RemoveAllListeners();
			RotateInput.RemoveAllListeners();
			HardDropped.RemoveAllListeners();
			HardDropInput.RemoveAllListeners();
			Spawned.RemoveAllListeners();
			Locked.RemoveAllListeners();
			LinesCleared.RemoveAllListeners();
			GameStart.RemoveAllListeners();
			GameStarted.RemoveAllListeners();
			GameOver.RemoveAllListeners();
		}

		public void ApplyConfig(ConfigSO config) => Config = config;

		private void RaiseLinesClearedEC(int lines)
		{
			if (_linesClearedEC) _linesClearedEC.RaiseEvent(lines);
		}

		private void RaiseGameOverEC()
		{
			if (_gameOverEC) _gameOverEC.RaiseEvent();
		}
	}
}