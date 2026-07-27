using Helpers;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     Owns Tetris gameplay state (Board, ActivePiece, Level, LinesCleared) and the
	///     per-frame gravity/lock timing loop. Executes whatever action it's told to
	///     (Move/Rotate/HardDrop) without deciding whether that action should currently
	///     be allowed — gating input (DAS, HardDropEnabled) is InputAdapter's job, not
	///     this class's. The component itself is alive from scene load like any other
	///     (Awake builds the piece bag), but gameplay stays idle — Board is null and
	///     IsRunning is false — until some other system calls StartGame(), typically
	///     after EventBus.ApplyConfig().
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	[RequireComponent(typeof(EventBus))]
	public class Controller : MonoBehaviour
	{
		[SerializeField] private Shape[] _shapes;

		[SerializeField] private EventBus _eventBus;

		private RandomBag<Shape> _bag;

		private float _gravityAccumulator;

		private bool _isGrounded;

		private float _lockAccumulator;

		// Manually controlled rather than auto-derived from LinesCleared — real
		// Tetris Guideline gravity ramps too hard to let lines-cleared drive it
		// unattended for this game's needs.
		public int Level { get; private set; } = 1;

		public int LinesCleared { get; private set; }

		public Board Board { get; private set; }

		public ActivePiece ActivePiece { get; private set; }

		// Gameplay stays idle until some other system (e.g. NightManager) calls
		// StartGame() — the component itself is active from scene load as normal.
		public bool IsRunning { get; private set; }

		private void Awake() => _bag = new RandomBag<Shape>(_shapes);

		private void Update()
		{
			if (!IsRunning) return;

			HandleGravity();
			HandleLock();
		}

		public void SetLevel(int level) => Level = Mathf.Max(1, level);

		public void StartGame()
		{
			Board = new Board(_eventBus.Config.BoardWidth, _eventBus.Config.BoardHeight);
			IsRunning = true;

			SpawnNext();
		}

		public void Move(Vector2Int direction)
		{
			if (!IsRunning) return;

			var piece = ActivePiece;

			if (!Rules.TryMove(Board, ref piece, direction)) return;

			ActivePiece = piece;
			_isGrounded = false;
			_lockAccumulator = 0f;

			_eventBus.OnPieceMoved.Invoke(direction);
			_eventBus.OnBoardChanged.Invoke(Board);
		}

		public void Rotate(int direction)
		{
			if (!IsRunning) return;

			var piece = ActivePiece;

			if (!Rules.TryRotate(Board, ref piece, direction)) return;

			ActivePiece = piece;
			_lockAccumulator = 0f;

			_eventBus.OnPieceRotated.Invoke(direction);
			_eventBus.OnBoardChanged.Invoke(Board);
		}

		public void HardDrop()
		{
			if (!IsRunning) return;

			var piece = ActivePiece;

			Rules.DropToBottom(Board, ref piece);

			ActivePiece = piece;

			_eventBus.OnHardDrop.Invoke();
			LockPiece();
		}

		private void HandleGravity()
		{
			var gravityDelay = Rules.GetGravityDelay(Level);

			_gravityAccumulator += Time.deltaTime;

			if (_gravityAccumulator < gravityDelay) return;

			_gravityAccumulator -= gravityDelay;

			var piece = ActivePiece;
			var moved = Rules.TryMove(Board, ref piece, Vector2Int.down);

			if (moved)
			{
				ActivePiece = piece;
				_isGrounded = false;
				_eventBus.OnPieceMoved.Invoke(Vector2Int.down);
				_eventBus.OnBoardChanged.Invoke(Board);
			}
			else
			{
				_isGrounded = true;
			}
		}

		private void HandleLock()
		{
			if (!_isGrounded) return;

			_lockAccumulator += Time.deltaTime;

			if (_lockAccumulator >= _eventBus.Config.LockDelay) LockPiece();
		}

		private void LockPiece()
		{
			foreach (var cell in ActivePiece.BoardSpaceCells) Board.SetCell(cell.x, cell.y);

			_eventBus.OnPieceLocked.Invoke(Board);

			var linesCleared = Board.ClearFullRows();

			if (linesCleared > 0)
			{
				LinesCleared += linesCleared;
				_eventBus.OnLinesCleared.Invoke(Board, linesCleared);
				_eventBus.OnBoardChanged.Invoke(Board);
			}

			SpawnNext();
		}

		private void SpawnNext()
		{
			var next = new ActivePiece
			{
				Shape = _bag.Next(),
				Position = _eventBus.Config.SpawnPosition,
				RotationIndex = 0,
			};

			if (!Rules.IsValidPosition(Board, next))
			{
				Board.Clear();
				IsRunning = false;
				_eventBus.OnGameOver.Invoke();

				return;
			}

			ActivePiece = next;
			_gravityAccumulator = 0f;
			_lockAccumulator = 0f;
			_isGrounded = false;

			_eventBus.OnPieceSpawned.Invoke(ActivePiece);
			_eventBus.OnBoardChanged.Invoke(Board);
		}
	}
}