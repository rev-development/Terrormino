using Helpers.Attributes;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     Contract for piece shape data (tile + rotation states) so both the
	///     ScriptableObject-backed Shape and any future non-asset source can be used
	///     interchangeably wherever a piece's shape is needed.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-4-6")]
	public interface IShape
	{
		Tile Tile { get; }

		ShapeState[] ShapeStates { get; }
	}
}