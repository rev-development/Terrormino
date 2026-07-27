using Helpers.Attributes;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace EC.Tetris
{
	/// <summary>
	///     The only class allowed to touch Tilemaps for Tetris. Purely visual: reads
	///     state off EventBus/Controller and draws it, never mutates gameplay state.
	///     Also owns the ghost-piece decision (gated by Config.GhostEnabled) since
	///     ghost is visual-only with no effect on the actual game — rendering is the
	///     correct place for that call, not Controller.
	/// </summary>
	[AiGenerated("Claude", "claude-sonnet-5")]
	[RequireComponent(typeof(EventBus))]
	public class BoardRenderer : MonoBehaviour
	{
		[SerializeField] private Controller _controller;

		[SerializeField] private EventBus _eventBus;

		[SerializeField] private Tilemap _boardTilemap;

		[SerializeField] private Tilemap _activePieceTilemap;

		[SerializeField] private Tilemap _ghostTilemap;

		// Single tile used for all locked cells.
		// If per-piece tile color on locked cells is required, Board
		// would need to store Tile references per cell instead of bool — deferred decision.
		[SerializeField] private TileBase _lockedTile;

		private void OnEnable()
		{
			_eventBus.OnBoardChanged.AddListener(RenderBoard);
			_eventBus.OnPieceMoved.AddListener(OnPieceMoved);
			_eventBus.OnPieceRotated.AddListener(OnPieceRotated);
			_eventBus.OnPieceSpawned.AddListener(OnPieceSpawned);
			_eventBus.OnPieceLocked.AddListener(OnPieceLocked);
		}

		private void OnDisable()
		{
			_eventBus.OnBoardChanged.RemoveListener(RenderBoard);
			_eventBus.OnPieceMoved.RemoveListener(OnPieceMoved);
			_eventBus.OnPieceRotated.RemoveListener(OnPieceRotated);
			_eventBus.OnPieceSpawned.RemoveListener(OnPieceSpawned);
			_eventBus.OnPieceLocked.RemoveListener(OnPieceLocked);
		}

		private void RenderBoard(Board board)
		{
			_boardTilemap.ClearAllTiles();

			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					if (board.IsOccupied(x, y)) _boardTilemap.SetTile(new Vector3Int(x, y), _lockedTile);
				}
			}
		}

		private void RenderActivePiece()
		{
			_activePieceTilemap.ClearAllTiles();
			_ghostTilemap.ClearAllTiles();

			var piece = _controller.ActivePiece;
			var tile = piece.Shape.Tile;

			foreach (var cell in piece.BoardSpaceCells)
				_activePieceTilemap.SetTile(new Vector3Int(cell.x, cell.y), tile);

			if (_eventBus.Config.GhostEnabled) RenderGhost(piece);
		}

		private void RenderGhost(ActivePiece piece)
		{
			var ghostDistance = Rules.GetGhostDistance(_controller.Board, piece);
			var tile = piece.Shape.Tile;

			foreach (var cell in piece.BoardSpaceCells)
				_ghostTilemap.SetTile(new Vector3Int(cell.x, cell.y - ghostDistance), tile);
		}

		private void ClearPieceLayers()
		{
			_activePieceTilemap.ClearAllTiles();
			_ghostTilemap.ClearAllTiles();
		}

		private void OnPieceLocked(Board _) => ClearPieceLayers();

		private void OnPieceMoved(Vector2Int _) => RenderActivePiece();

		private void OnPieceRotated(int _) => RenderActivePiece();

		private void OnPieceSpawned(ActivePiece _) => RenderActivePiece();
	}
}