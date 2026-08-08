using System;
using Helpers.Attributes;
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
		///     The coordinates for the base shape in a single orientation.
		///     Origin (0,0) is bottom-left; Y increases upward.
		/// </summary>
		[CellGrid(5, 5)] public Vector2Int[] Cells =
			{ };
		/// <summary>
		///     The clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		[SerializeField] public Vector2Int[] CW =
			{ };
		/// <summary>
		///     The counter-clockwise set of 'wall kick' vectors. (Should be 5)
		/// </summary>
		[SerializeField] public Vector2Int[] CCW =
			{ };
	}

	/// <summary>
	///     Contract for piece shape data (tile + rotation states) so both the
	///     ScriptableObject-backed Shape and any future non-asset source can be used
	///     interchangeably wherever a piece's shape is needed.
	/// </summary>
	public interface IShape
	{
		TileBase Tile { get; }

		RotationState[] RotationStates { get; }
	}
}