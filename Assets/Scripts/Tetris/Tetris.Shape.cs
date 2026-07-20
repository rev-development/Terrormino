using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris
{
	public enum ShapeKeys
	{
		I,

		/*   []
			 []
			 []
			 []   */
		J,

		/*   []
			 []
		   [][]   */
		L,

		/*   []
			 []
			 [][] */
		O,

		/* [][]
		   [][]   */
		S,

		/*   [][]
		   [][]   */
		T,

		/* [][][]
			 []   */
		Z,
		/* [][]
			 [][] */
	}

	[Serializable]
	public static class ShapeVecs
	{
		// This looks complicated but it's just matrix math for rotating things
		public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);

		public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);

		public static readonly float[] RotationMatrix =
		{
			cos,
			sin,
			-sin,
			cos,
		};

		public static readonly Dictionary<ShapeKeys, Vector2Int[]> Cells = new()
																		   {
																			   {
																				   ShapeKeys.I, new[]
																					   {
																						   new Vector2Int(-1, 1),
																						   new Vector2Int(0, 1),
																						   new Vector2Int(1, 1),
																						   new Vector2Int(2, 1),
																					   }
																			   },
																			   {
																				   ShapeKeys.J, new[]
																					   {
																						   new Vector2Int(-1, 1),
																						   new Vector2Int(-1, 0),
																						   new Vector2Int(0, 0),
																						   new Vector2Int(1, 0),
																					   }
																			   },
																			   {
																				   ShapeKeys.L, new[]
																					   {
																						   new Vector2Int(1, 1),
																						   new Vector2Int(-1, 0),
																						   new Vector2Int(0, 0),
																						   new Vector2Int(1, 0),
																					   }
																			   },
																			   {
																				   ShapeKeys.O, new[]
																					   {
																						   new Vector2Int(0, 1),
																						   new Vector2Int(1, 1),
																						   new Vector2Int(0, 0),
																						   new Vector2Int(1, 0),
																					   }
																			   },
																			   {
																				   ShapeKeys.S, new[]
																					   {
																						   new Vector2Int(0, 1),
																						   new Vector2Int(1, 1),
																						   new Vector2Int(-1, 0),
																						   new Vector2Int(0, 0),
																					   }
																			   },
																			   {
																				   ShapeKeys.T, new[]
																					   {
																						   new Vector2Int(0, 1),
																						   new Vector2Int(-1, 0),
																						   new Vector2Int(0, 0),
																						   new Vector2Int(1, 0),
																					   }
																			   },
																			   {
																				   ShapeKeys.Z, new[]
																					   {
																						   new Vector2Int(-1, 1),
																						   new Vector2Int(0, 1),
																						   new Vector2Int(0, 0),
																						   new Vector2Int(1, 0),
																					   }
																			   },
																		   };

		private static readonly Vector2Int[,] _wallKicksI =
		{
			{
				new(0, 0),
				new(-2, 0),
				new(1, 0),
				new(-2, -1),
				new(1, 2),
			},
			{
				new(0, 0),
				new(2, 0),
				new(-1, 0),
				new(2, 1),
				new(-1, -2),
			},
			{
				new(0, 0),
				new(-1, 0),
				new(2, 0),
				new(-1, 2),
				new(2, -1),
			},
			{
				new(0, 0),
				new(1, 0),
				new(-2, 0),
				new(1, -2),
				new(-2, 1),
			},
			{
				new(0, 0),
				new(2, 0),
				new(-1, 0),
				new(2, 1),
				new(-1, -2),
			},
			{
				new(0, 0),
				new(-2, 0),
				new(1, 0),
				new(-2, -1),
				new(1, 2),
			},
			{
				new(0, 0),
				new(1, 0),
				new(-2, 0),
				new(1, -2),
				new(-2, 1),
			},
			{
				new(0, 0),
				new(-1, 0),
				new(2, 0),
				new(-1, 2),
				new(2, -1),
			},
		};

		private static readonly Vector2Int[,] _wallKicksJLOSTZ =
		{
			{
				new(0, 0),
				new(-1, 0),
				new(-1, 1),
				new(0, -2),
				new(-1, -2),
			},
			{
				new(0, 0),
				new(1, 0),
				new(1, -1),
				new(0, 2),
				new(1, 2),
			},
			{
				new(0, 0),
				new(1, 0),
				new(1, -1),
				new(0, 2),
				new(1, 2),
			},
			{
				new(0, 0),
				new(-1, 0),
				new(-1, 1),
				new(0, -2),
				new(-1, -2),
			},
			{
				new(0, 0),
				new(1, 0),
				new(1, 1),
				new(0, -2),
				new(1, -2),
			},
			{
				new(0, 0),
				new(-1, 0),
				new(-1, -1),
				new(0, 2),
				new(-1, 2),
			},
			{
				new(0, 0),
				new(-1, 0),
				new(-1, -1),
				new(0, 2),
				new(-1, 2),
			},
			{
				new(0, 0),
				new(1, 0),
				new(1, 1),
				new(0, -2),
				new(1, -2),
			},
		};

		public static readonly Dictionary<ShapeKeys, Vector2Int[,]> WallKicks = new()
			{
				{ ShapeKeys.I, _wallKicksI },
				{ ShapeKeys.J, _wallKicksJLOSTZ },
				{ ShapeKeys.L, _wallKicksJLOSTZ },
				{ ShapeKeys.O, _wallKicksJLOSTZ },
				{ ShapeKeys.S, _wallKicksJLOSTZ },
				{ ShapeKeys.T, _wallKicksJLOSTZ },
				{ ShapeKeys.Z, _wallKicksJLOSTZ },
			};
	}

	[Serializable]
	public struct Shape
	{
		public ShapeKeys ShapeKey;

		public Tile Tile;

		// { get; private set; } is "Others can look but not touch"
		public Vector2Int[] Cells { get; private set; }

		public Vector2Int[,] WallKicks { get; private set; }

		public void Initialize()
		{
			Cells = ShapeVecs.Cells[ShapeKey];
			WallKicks = ShapeVecs.WallKicks[ShapeKey];
		}

		public Vector3Int[] GetCellsAsVec3
		{
			get
			{
				var vector3Ints = new Vector3Int[Cells.Length];

				for (var i = 0; i < Cells.Length; i++) vector3Ints[i] = new Vector3Int(Cells[i].x, Cells[i].y, 0);

				return vector3Ints;
			}
		}
	}

	public interface ITetrisConfig
	{
		float TetrisGravityDelay { get; set; }

		float TetrisMoveDelay { get; set; }

		float TetrisLockDelay { get; set; }
	}

	[Serializable]
	public struct TetrisConfig : ITetrisConfig
	{
		public float TetrisGravityDelay { get; set; }

		public float TetrisMoveDelay { get; set; }

		public float TetrisLockDelay { get; set; }
	}
}