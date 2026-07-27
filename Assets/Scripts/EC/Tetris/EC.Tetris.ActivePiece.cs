using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     A snapshot of one in-play piece: which Shape, where, and which rotation.
	///     Pure value data with no identity — passed by value through Controller,
	///     Rules, and events so listeners get what they need without reaching back
	///     into the controller. Computed properties (CurrentState, BoardSpaceCells)
	///     derive only from these three fields, nothing external.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-4-6")]
	public struct ActivePiece
	{
		public IShape Shape;

		public Vector2Int Position;

		public int RotationIndex;

		public ShapeState CurrentState => Shape.ShapeStates[RotationIndex];

		public Vector2Int[] BoardSpaceCells
		{
			get
			{
				var cells = CurrentState.Cells;
				var result = new Vector2Int[cells.Length];
				for (var i = 0; i < cells.Length; i++) result[i] = cells[i] + Position;

				return result;
			}
		}
	}
}