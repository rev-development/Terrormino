using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     Authored, static per-tetromino data: which tile to draw and the full set of
	///     rotation states (ShapeState[]). One asset per tetromino shape (I, J, L, O, S,
	///     T, Z); never mutated at runtime — ActivePiece only ever reads through it.
	/// </summary>
	[CreateAssetMenu(fileName = "Shape", menuName = "Terrormino/Tetris/Shape")]
	public class Shape : ScriptableObject, IShape
	{
		[SerializeField] private Tile _tile = null;

		[SerializeField] private ShapeState[] _shapeStates;

		public Tile Tile => _tile;

		public ShapeState[] ShapeStates => _shapeStates;
	}
}