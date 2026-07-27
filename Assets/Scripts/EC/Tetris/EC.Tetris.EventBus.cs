using Helpers.Attributes;
using Helpers.Events.Channels;
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
	[AiGenerated("Claude", "claude-sonnet-5")]
	public class EventBus : MonoBehaviour
	{
		// EventChannels for the outward-facing audience of these two events
		// (NightManager, Player.Manager). The UnityEvents below cover the
		// local audience (audio, animation, renderer).
		[SerializeField] private IntEC _linesClearedEC;

		[SerializeField] private VoidEC _gameOverEC;

		public UnityEvent<Vector2Int> OnPieceMoved = new();

		public UnityEvent<int> OnPieceRotated = new();

		public UnityEvent OnHardDrop = new();

		public UnityEvent<ActivePiece> OnPieceSpawned = new();

		public UnityEvent<Board> OnPieceLocked = new();

		public UnityEvent<Board, int> OnLinesCleared = new();

		public UnityEvent<Board> OnBoardChanged = new();

		public UnityEvent OnGameOver = new();

		// Gravity is computed from Level via Rules.GetGravityDelay (Guideline formula),
		// not stored here. Lock delay and DAS aren't level-scaled — 0.5s lock is the
		// Guideline constant across all levels, and 0.1s reuses the classic NES repeat
		// rate for MoveDelay since this config has no separate initial-DAS-charge field.
		public IConfig Config { get; private set; } = new Config
		{
			LockDelay = 0.5f,
			MoveDelay = 0.1f,
			HardDropEnabled = true,
			GhostEnabled = true,
			BoardWidth = 10,
			BoardHeight = 20,
			SpawnPosition = new Vector2Int(4, 18),
		};

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
			OnBoardChanged.RemoveAllListeners();
			OnGameOver.RemoveAllListeners();
		}

		public void ApplyConfig(IConfig config) => Config = config;

		private void RaiseLinesClearedEC(Board board, int lines) => _linesClearedEC.RaiseEvent(lines);

		private void RaiseGameOverEC() => _gameOverEC.RaiseEvent();
	}
}