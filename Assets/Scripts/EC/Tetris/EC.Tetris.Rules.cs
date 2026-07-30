using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     The only place Tetris movement/rotation/collision/gravity math lives. Pure
	///     and stateless — every method takes the Board/ActivePiece it needs and either
	///     returns a result or mutates via `ref`; nothing here fires events, touches
	///     Unity lifecycle, or holds state of its own. If a calculation belongs to
	///     "the rules of Tetris" rather than "this specific game's state," it goes here.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	public static class Rules
	{
		public static bool IsValidPosition(Board board, ActivePiece piece)
		{
			foreach (var cell in piece.BoardSpaceCells)
			{
				if (!board.IsInBounds(cell.x, cell.y)) return false;

				if (board.IsOccupied(cell.x, cell.y)) return false;
			}

			return true;
		}

		public static bool TryMove(Board board, ref ActivePiece piece, Vector2Int delta)
		{
			var candidate = piece;
			candidate.Position += delta;

			if (!IsValidPosition(board, candidate)) return false;

			piece = candidate;

			return true;
		}

		public static bool TryRotate(Board board, ref ActivePiece piece, int direction)
		{
			var count = piece.Shape.ShapeStates.Length;
			var nextRotation = (piece.RotationIndex + direction + count) % count;

			var kicks = direction > 0 ? piece.CurrentState.CW : piece.CurrentState.CCW;

			foreach (var kick in kicks)
			{
				var candidate = piece;
				candidate.RotationIndex = nextRotation;
				candidate.Position += kick;

				if (IsValidPosition(board, candidate))
				{
					piece = candidate;

					return true;
				}
			}

			return false;
		}

		public static void DropToBottom(Board board, ref ActivePiece piece)
		{
			var candidate = piece;
			candidate.Position += Vector2Int.down;

			while (IsValidPosition(board, candidate))
			{
				piece = candidate;
				candidate.Position += Vector2Int.down;
			}
		}

		public static int GetGhostDistance(Board board, ActivePiece piece)
		{
			var distance = 0;
			var candidate = piece;

			while (TryMove(board, ref candidate, Vector2Int.down))
			{
				distance++;
			}

			return distance;
		}

		// Matches Tetris Guideline: center horizontally (anchor one left of true center
		// so a 4-wide piece spans columns Width/2-1 through Width/2+2), two rows below
		// the top so pieces spawn fully visible. Reproduces (4,18) on a standard 10×20.
		[AiGenerated("Claude", "claude-sonnet-4-6", "Reviewed by Rev 7-29-26")]
		public static Vector2Int GetSpawnPosition(int boardWidth, int boardHeight) =>
			new(boardWidth / 2 - 1, boardHeight - 2);

		public static Vector2Int GetSpawnPosition(Board board) => GetSpawnPosition(board.Width, board.Height);

		// Tetris Guideline gravity formula. Always evaluates to exactly 1s at level 1
		// (anything^0 == 1), so there's no separate base value to configure. Level is
		// driven manually rather than auto-scaled from lines cleared, so nothing here
		// clamps or caps how far this can ramp — that's on whoever calls SetLevel.
		public static float GetGravityDelay(int level) => Mathf.Pow(0.8f - (level - 1) * 0.007f, level - 1);
	}
}