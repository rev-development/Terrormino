using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     The only place Tetris movement/rotation/collision/gravity math lives. Pure
	///     and stateless — every method takes the Playfield/ActivePiece it needs and either
	///     returns a result or mutates via `ref`; nothing here fires events, touches
	///     Unity lifecycle, or holds state of its own. If a calculation belongs to
	///     "the rules of Tetris" rather than "this specific game's state," it goes here.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	public static class Rules
	{
		public static bool IsValidPosition(Playfield playfield, ActivePiece piece)
		{
			foreach (var cell in piece.PlayfieldSpaceCells)
			{
				if (!playfield.IsInBounds(cell.x, cell.y)) return false;

				if (playfield.IsOccupied(cell.x, cell.y)) return false;
			}

			return true;
		}

		public static bool TryMove(Playfield playfield, ref ActivePiece piece, Vector2Int delta)
		{
			var candidate = piece;
			candidate.PlayfieldPosition += delta;

			if (!IsValidPosition(playfield, candidate)) return false;

			piece = candidate;

			return true;
		}

		public static bool TryRotate(Playfield playfield, ref ActivePiece piece, int direction)
		{
			var count = piece.Shape.RotationStates.Length;
			var nextRotation = (piece.RotationIndex + direction + count) % count;

			var kicks = direction > 0 ? piece.CurrentState.CW : piece.CurrentState.CCW;

			foreach (var kick in kicks)
			{
				var candidate = piece;
				candidate.RotationIndex = nextRotation;
				candidate.PlayfieldPosition += kick;

				if (IsValidPosition(playfield, candidate))
				{
					piece = candidate;

					return true;
				}
			}

			return false;
		}

		public static void DropToBottom(Playfield playfield, ref ActivePiece piece)
		{
			var candidate = piece;
			candidate.PlayfieldPosition += Vector2Int.down;

			while (IsValidPosition(playfield, candidate))
			{
				piece = candidate;
				candidate.PlayfieldPosition += Vector2Int.down;
			}
		}

		public static int GetGhostDistance(Playfield playfield, ActivePiece piece)
		{
			var distance = 0;
			var candidate = piece;

			while (TryMove(playfield, ref candidate, Vector2Int.down))
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

		public static Vector2Int GetSpawnPosition(Playfield playfield) =>
			GetSpawnPosition(playfield.Width, playfield.Height);

		// Tetris Guideline gravity formula. Always evaluates to exactly 1s at level 1
		// (anything^0 == 1), so there's no separate base value to configure. Level is
		// driven manually rather than auto-scaled from lines cleared, so nothing here
		// clamps or caps how far this can ramp — that's on whoever calls SetLevel.
		public static float GetGravityDelay(int level) => Mathf.Pow(0.8f - (level - 1) * 0.007f, level - 1);
	}
}