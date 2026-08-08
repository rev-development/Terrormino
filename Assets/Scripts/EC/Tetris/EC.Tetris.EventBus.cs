using Helpers.Attributes;
using Helpers.Events.Channels;
using Helpers.Ext;
using UnityEngine;
using UnityEngine.Events;

namespace EC.Tetris
{
	/// <summary>
	///     Single instance in the scene — the event surface for internal listeners
	///     (renderer, audio, animation) and the config holder Controller, InputAdapter,
	///     and BoardRenderer all read through, rather than through each other. Also
	///     raises EventChannels for the two events external systems care about (lines
	///     cleared, game over). Future mechanics that need to intercept or react to
	///     these events hook directly into this bus — there is no separate stage.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5", "Reviewed by Rev 7-28-26")]
	public class EventBus : MonoBehaviour
	{
		// EventChannels for the outward-facing audience of these two events
		// (NightManager, Player.Manager). The UnityEvents below cover the
		// local audience (audio, animation, renderer).
		[SerializeField] private IntEC _linesClearedEC;

		[SerializeField] private VoidEC _gameOverEC;

		[field: SerializeField] public ConfigSO Config { get; private set; }

		public UnityEvent<Vector2Int> OnPieceMoved = new();

		public UnityEvent<int> OnPieceRotated = new();

		public UnityEvent OnHardDrop = new();

		public UnityEvent<ActivePiece> OnPieceSpawned = new();

		public UnityEvent<Playfield> OnPieceLocked = new();

		public UnityEvent<Playfield, int> OnLinesCleared = new();

		public UnityEvent OnGameOver = new();

		private void Awake()
		{
			gameObject.CheckIfSetInInspector(_linesClearedEC, "Lines Cleared Event Channel");
			gameObject.CheckIfSetInInspector(_gameOverEC, "Game Over Event Channel");
		}

		private void OnEnable()
		{
			OnLinesCleared.AddListener(RaiseLinesClearedEC);
			OnGameOver.AddListener(RaiseGameOverEC);
		}

		private void OnDisable()
		{
			OnPieceMoved.RemoveAllListeners();
			OnPieceRotated.RemoveAllListeners();
			OnHardDrop.RemoveAllListeners();
			OnPieceSpawned.RemoveAllListeners();
			OnPieceLocked.RemoveAllListeners();
			OnLinesCleared.RemoveAllListeners();
			OnGameOver.RemoveAllListeners();
		}

		public void ApplyConfig(ConfigSO config) => Config = config;

		private void RaiseLinesClearedEC(Playfield playfield, int lines) => _linesClearedEC.RaiseEvent(lines);

		private void RaiseGameOverEC() => _gameOverEC.RaiseEvent();
	}
}