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
		[SerializeField] private TileBase _tile;

		[SerializeField] private RotationState _orientationZero = new();

		[SerializeField] private RotationState _orientationRight = new();

		[SerializeField] private RotationState _orientationTwo = new();

		[SerializeField] private RotationState _orientationLeft = new();

		public RotationState[] RotationStates =>
			new[]
			{
				_orientationZero,
				_orientationRight,
				_orientationTwo,
				_orientationLeft,
			};

		public TileBase Tile => _tile;
	}
}