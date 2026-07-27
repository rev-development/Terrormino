using System;
using Helpers.Attributes;

namespace EC.Tetris
{
	/// <summary>
	///     A plain occupancy grid — true/false per cell, nothing else. Doesn't know
	///     about pieces, shapes, tiles, or Tetris rules; tile identity for rendering
	///     belongs to the piece, not the board. Not a MonoBehaviour on purpose: no
	///     Update, no Inspector fields, no lifecycle to hook into.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	public class Board
	{
		// true = occupied, false = empty.
		// Tile identity for rendering is owned by the piece, not the board.
		private readonly bool[,] _grid;

		public int Width { get; }

		public int Height { get; }

		public Board(int width, int height)
		{
			Width = width;
			Height = height;
			_grid = new bool[width, height];
		}

		public bool IsOccupied(int x, int y) => _grid[x, y];

		public bool IsInBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

		public void SetCell(int x, int y) => _grid[x, y] = true;

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
			for (var x = 0; x < Width; x++) if (!_grid[x, row]) return false;

			return true;
		}

		private void ClearRow(int row)
		{
			for (var x = 0; x < Width; x++) _grid[x, row] = false;
		}

		private void ShiftRowsDown(int fromRow)
		{
			for (var row = fromRow; row < Height - 1; row++)
				for (var x = 0; x < Width; x++)
					_grid[x, row] = _grid[x, row + 1];

			for (var x = 0; x < Width; x++) _grid[x, Height - 1] = false;
		}
	}
}
