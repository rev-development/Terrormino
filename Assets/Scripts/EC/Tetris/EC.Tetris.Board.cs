using System;
using Helpers.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     An occupancy grid that stores the tile placed at each cell. Doesn't know
	///     about pieces, shapes, or Tetris rules. Not a MonoBehaviour on purpose: no
	///     Update, no Inspector fields, no lifecycle to hook into.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5", "Reviewed by Rev 7-28-26")]
	public class Playfield
	{
		// null = empty, non-null = occupied (stores the tile for rendering).
		private readonly TileBase[,] _grid;

		public Playfield(Vector2Int size)
		{
			Width = size.x;
			Height = size.y;
			_grid = new TileBase[size.x, size.y];
		}

		public int Width { get; }

		public int Height { get; }

		public bool IsOccupied(int x, int y) => _grid[x, y] != null;

		public TileBase GetTile(int x, int y) => _grid[x, y];

		public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

		public void SetCell(int x, int y, TileBase tile) => _grid[x, y] = tile;

		public void Clear() => Array.Clear(_grid, 0, _grid.Length);

		public int ClearFullRows()
		{
			var cleared = 0;
			var row = 0;

			while (row < Height)
			{
				if (IsRowFull(row))
				{
					ClearRow(row);
					ShiftRowsDown(row);
					cleared++;
				}
				else
				{
					row++;
				}
			}

			return cleared;
		}

		private bool IsRowFull(int row)
		{
			for (var x = 0; x < Width; x++)
			{
				if (_grid[x, row] == null) return false;
			}

			return true;
		}

		private void ClearRow(int row)
		{
			for (var x = 0; x < Width; x++) _grid[x, row] = null;
		}

		private void ShiftRowsDown(int fromRow)
		{
			for (var row = fromRow; row < Height - 1; row++)
			{
				for (var x = 0; x < Width; x++) _grid[x, row] = _grid[x, row + 1];
			}

			for (var x = 0; x < Width; x++) _grid[x, Height - 1] = null;
		}
	}
}