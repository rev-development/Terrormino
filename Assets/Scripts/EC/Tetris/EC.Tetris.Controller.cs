using Helpers;
using Helpers.Attributes;
using Helpers.Ext;
using JetBrains.Annotations;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     Owns Tetris gameplay state (Playfield, ActivePiece, Level, LinesCleared) and the
	///     per-frame gravity/lock timing loop. Executes whatever action it's told to
	///     (Move/Rotate/HardDrop) without deciding whether that action should currently
	///     be allowed — gating input (DAS, HardDropEnabled) is InputAdapter's job, not
	///     this class's. The component itself is alive from scene load like any other
	///     (Awake builds the piece bag), but gameplay stays idle — Playfield is null and
	///     IsRunning is false — until some other system calls StartGame(), typically
	///     after EventBus.ApplyConfig().
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[AiGenerated("Claude", "claude-sonnet-4-6", "Reviewed by Rev 7-28-26")]
	public class Controller : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		private readonly RandomBag<Shape> _bag = new();

		// Manually controlled rather than auto-derived from LinesCleared — real
		// Tetris Guideline gravity ramps too hard to let lines-cleared drive it
		// unattended for this game's needs.
		public int Level { get; private set; } = 1;

		private float _gravityDelay => Rules.GetGravityDelay(Level);

		public Playfield Playfield { get; private set; }

		// Gameplay stays idle until some other system (e.g. NightManager) calls
		// StartGame() — the component itself is active from scene load as normal.
		public bool IsRunning { get; private set; } = false;

		[UsedImplicitly] public void Awake() => _eventBus = gameObject.TryFindComponent<EventBus>();

		private void Start()
		{
			if (_eventBus != null) _bag.Init(_eventBus.Config.Shapes);
		}

		private void Update()
		{
			if (!IsRunning) return;

			HandleGravity();
		}

		public void SetLevel(int level) => Level = Mathf.Max(1, level);

		private void ClearActivePiece() => ActivePiece = null;

		public void StartGame()
		{
			Playfield = new Playfield(_eventBus.Config.PlayfieldSize);
			_eventBus.OnGameStart.Invoke();

			IsRunning = true;

			SpawnNext();
		}

		private void SpawnNext()
		{
			var next = new ActivePiece
			{
				Shape = _bag.Next(),
				PlayfieldPosition = Rules.GetSpawnPosition(Playfield),
				RotationIndex = 0,
			};

			if (!Rules.IsValidPosition(Playfield, next))
			{
				Playfield.Clear();
				IsRunning = false;
				_eventBus.OnGameOver.Invoke();

				return;
			}

			ActivePiece = next;
			_gravityAccumulator = 0f;
			ResetGrounding();

			_eventBus.OnPieceSpawned.Invoke(next);
		}

#region Locking

		// Locking Workflow
		// 1. Gravity attempts to move piece downward
		// 2. If that attempt fails, then the piece is considered grounded and the time is stamped
		// 3. A grounded piece becomes locked after a delay. The delay can be extended with successful transformations, up to a limit of 15 per OG Tetris

		private void HandleGravity()
		{
			_gravityAccumulator += Time.deltaTime;

			if (_gravityAccumulator < _gravityDelay) return;

			_gravityAccumulator
				-= _gravityDelay; // Overshoot preservation, instead of resetting the timer it subtracts the delay value and keeps the clock running

			if (!Move(
						Vector2Int.down
					)) // Don't convert to TryMove, the side effect of Move (actually translating the piece) is important here
			{
				UpdateGrounding();
				HandleLock();
			}
		}

		private void UpdateGrounding()
		{
			if (ActivePiece is not { } candidate) return;

			var canFall = Rules.TryMove(
				Playfield,
				ref candidate,
				Vector2Int.down
			); // A failed down move means the space below the piece is occupied or out of bounds

			if (!canFall
					&& !_isGrounded) // If it hasn't been marked as grounded yet, timestamp when it becomes grounded
			{
				_isGrounded = true;
				_groundedAt = Time.time;
			}
			else if (canFall)
			{
				ResetGrounding();
			}
		}

		/// <summary>
		///     Resets the lock timer when the player successfully moves or rotates a grounded
		///     piece, giving them more time to maneuver before it locks. Per Tetris Guideline
		///     this is called "Extended Placement Lockdown" — any successful move or rotation
		///     while grounded restarts the delay, up to <see cref="IConfig.LockResetLimit" />
		///     times per piece. Once the cap is hit the lock timer runs to completion,
		///     preventing indefinite extension via side-to-side sliding.
		///     <see href="https://tetris.wiki/Extended_Placement_Lockdown" />
		/// </summary>
		private void ExtendLockWindow()
		{
			if (!_isGrounded) return;
			if (_lockResetCount >= _eventBus.Config.LockResetLimit) return;

			_lockResetCount++;
			_groundedAt = Time.time;
		}

		private void HandleLock()
		{
			if (!_isGrounded) return;

			if (Time.time - _groundedAt >= _eventBus.Config.LockDelay) LockPiece();
		}

		private void LockPiece()
		{
			var piece = ActivePiece!.Value;
			foreach (var cell in piece.PlayfieldSpaceCells) Playfield.SetCell(cell.x, cell.y, piece.Shape.Tile);

			ClearActivePiece();
			_eventBus.OnPieceLocked.Invoke();

			var linesCleared = Playfield.ClearFullRows();

			if (linesCleared > 0) _eventBus.OnLinesCleared.Invoke(linesCleared);

			SpawnNext();
		}

#endregion

#region Per Piece State

		// These values should reset with each new ActivePiece

		public ActivePiece? ActivePiece { get; private set; }

		private float _gravityAccumulator;

		private float _groundedAt;

		private bool _isGrounded;

		private int _lockResetCount;

		private void ResetGrounding()
		{
			_isGrounded = false;
			_groundedAt = 0f;
			_lockResetCount = 0; // This is intentional, the lock count is meant to reset per grounding
		}

#endregion

#region Transformations

		// These functions all use the Try pattern, which is:
		// Make a local copy > Mutate object > Check validity > If valid, apply changes to original, otherwise discard

		public bool Move(Vector2Int direction)
		{
			if (ActivePiece is not { } candidate) return false;

			if (!Rules.TryMove(Playfield, ref candidate, direction)) return false;

			ActivePiece = candidate;

			UpdateGrounding();

			ExtendLockWindow();

			_eventBus.OnPieceMoved.Invoke(direction);
			HandleLock();

			return true;
		}

		public void Rotate(int direction)
		{
			if (ActivePiece is not { } candidate) return;

			if (!Rules.TryRotate(Playfield, ref candidate, direction)) return;

			ActivePiece = candidate;

			UpdateGrounding();
			ExtendLockWindow();

			_eventBus.OnPieceRotated.Invoke(direction);
			HandleLock();
		}

		/// <summary>
		///     Drops the active piece to the bottom and locks it immediately, bypassing
		///     the lock delay. Per Tetris Guideline, hard drop always locks instantly —
		///     skipping HandleLock is intentional, not an oversight.
		///     <see href="https://tetris.wiki/Hard_drop" />
		/// </summary>
		public void HardDrop()
		{
			if (ActivePiece is not { } candidate) return;

			Rules.DropToBottom(Playfield, ref candidate);

			ActivePiece = candidate;

			_eventBus.OnHardDrop.Invoke();
			LockPiece(); // Calls LockPiece instead of HandleLock because HardDrops skip delay
		}

#endregion
	}
}