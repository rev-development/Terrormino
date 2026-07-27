using System;
using Helpers.Attributes;
using UnityEngine;

namespace EC.Tetris
{
	/// <summary>
	///     One orientation of a Shape: its cell layout plus the CW/CCW wall-kick tables
	///     to try when rotating out of this orientation. Pure authored data, no behavior
	///     — Rules.TryRotate reads CW/CCW directly rather than deriving them at runtime.
	/// </summary>
	[Serializable]
	[AiGenerated("Claude", "claude-sonnet-4-6")]
	public class ShapeState
	{
		/// <summary>
		///     The coordinates for the base shape in a single orientation.
		/// </summary>
		public Vector2Int[] Cells;

		/// <summary>
		///     The clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		public Vector2Int[] CW;

		/// <summary>
		///     The counter-clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		public Vector2Int[] CCW;
	}
}