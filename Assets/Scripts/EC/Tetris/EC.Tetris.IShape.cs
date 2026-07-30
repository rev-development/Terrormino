using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     One orientation of a Shape: its cell layout plus the CW/CCW wall-kick tables
	///     to try when rotating out of this orientation. Pure authored data, no behavior
	///     — Rules.TryRotate reads CW/CCW directly rather than deriving them at runtime.
	/// </summary>
	[Serializable]
	public class RotationState
	{
		/// <summary>
		///     The counter-clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		public Vector2Int[] CCW;
		/// <summary>
		///     The coordinates for the base shape in a single orientation.
		/// </summary>
		public Vector2Int[] Cells;

		/// <summary>
		///     The clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		public Vector2Int[] CW;
	}

	/// <summary>
	///     Contract for piece shape data (tile + rotation states) so both the
	///     ScriptableObject-backed Shape and any future non-asset source can be used
	///     interchangeably wherever a piece's shape is needed.
	/// </summary>
	public interface IShape
	{
		TileBase Tile { get; }

		RotationState[] ShapeStates { get; }
	}
}