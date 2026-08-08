using Helpers;
using Helpers.Attributes;
using Helpers.Ext;
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
	[AiGenerated("Claude", "claude-sonnet-4-6", "Reviewed by Rev 7-28-26")]
	[RequireComponent(typeof(EventBus))]
	[RequireComponent(typeof(Controller))]
	public class BoardRenderer : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private Controller _controller;

		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[SerializeField] private Tilemap _boardTilemap;

		[SerializeField] private Tilemap _activePieceTilemap;

		[SerializeField] private Tilemap _ghostTilemap;

		private void Awake()
		{
			_controller = gameObject.TryFindComponent<Controller>();
			_eventBus = gameObject.TryFindComponent<EventBus>();
			gameObject.CheckIfSetInInspector(_boardTilemap, "Board Tilemap");
			gameObject.CheckIfSetInInspector(_activePieceTilemap, "Active Piece Tilemap");
			gameObject.CheckIfSetInInspector(_ghostTilemap, "Ghost Tilemap");
		}

		private void OnEnable()
		{
			_eventBus.OnPieceMoved.AddListener(OnPieceMoved);
			_eventBus.OnPieceMoved.AddListener(RenderPlayfield);
			_eventBus.OnPieceRotated.AddListener(OnPieceRotated);
			_eventBus.OnPieceRotated.AddListener(RenderPlayfield);
			_eventBus.OnPieceSpawned.AddListener(OnPieceSpawned);
			_eventBus.OnPieceSpawned.AddListener(RenderPlayfield);
			_eventBus.OnPieceLocked.AddListener(OnPieceLocked);
			_eventBus.OnPieceLocked.AddListener(RenderPlayfield);
			_eventBus.OnLinesCleared.AddListener(RenderPlayfield);
		}

		private void OnDisable()
		{
			_eventBus.OnPieceMoved.RemoveListener(OnPieceMoved);
			_eventBus.OnPieceMoved.RemoveListener(RenderPlayfield);
			_eventBus.OnPieceRotated.RemoveListener(OnPieceRotated);
			_eventBus.OnPieceRotated.RemoveListener(RenderPlayfield);
			_eventBus.OnPieceSpawned.RemoveListener(OnPieceSpawned);
			_eventBus.OnPieceSpawned.RemoveListener(RenderPlayfield);
			_eventBus.OnPieceLocked.RemoveListener(OnPieceLocked);
			_eventBus.OnPieceLocked.RemoveListener(RenderPlayfield);
			_eventBus.OnLinesCleared.RemoveListener(RenderPlayfield);
		}

		private void RenderPlayfield()
		{
			var board = _controller.Playfield;
			_boardTilemap.ClearAllTiles();

			for (var x = 0; x < board.Width; x++)
			{
				for (var y = 0; y < board.Height; y++)
				{
					if (board.IsOccupied(x, y)) _boardTilemap.SetTile(new Vector3Int(x, y), board.GetTile(x, y));
				}
			}
		}

		// Typed listener shims — C# method group resolution picks the right overload
		// per event so RenderBoard can be added directly without lambdas.
		private void RenderPlayfield(Vector2Int _) => RenderPlayfield();
		private void RenderPlayfield(int _) => RenderPlayfield();
		private void RenderPlayfield(ActivePiece _) => RenderPlayfield();
		private void RenderPlayfield(Playfield _) => RenderPlayfield();
		private void RenderPlayfield(Playfield _, int __) => RenderPlayfield();

		private void RenderActivePiece()
		{
			_activePieceTilemap.ClearAllTiles();

			if (_controller.ActivePiece is not { } piece) return;

			var tile = piece.Shape.Tile;

			foreach (var cell in piece.BoardSpaceCells) _activePieceTilemap.SetTile(new Vector3Int(cell.x, cell.y), tile);

			RenderGhost();
		}

		private void RenderGhost()
		{
			_ghostTilemap.ClearAllTiles();

			if (!_eventBus.Config.GhostEnabled
					|| _controller.ActivePiece is not { } piece)
				return; // The "_controller.ActivePiece is not { } piece" syntax checks for null and if it is not null, then it unwraps _controller.ActivePice.Value as piece

			var ghostDistance = Rules.GetGhostDistance(_controller.Playfield, piece);
			var tile = piece.Shape.Tile;

			foreach (var cell in piece.BoardSpaceCells)
				_ghostTilemap.SetTile(new Vector3Int(cell.x, cell.y - ghostDistance), tile);
		}

		private void ClearActivePieceLayers()
		{
			RenderActivePiece();
			RenderGhost();
		}

		private void OnPieceLocked(Playfield _) => ClearActivePieceLayers();
		private void OnPieceMoved(Vector2Int _) => RenderActivePiece();
		private void OnPieceRotated(int _) => RenderActivePiece();
		private void OnPieceSpawned(ActivePiece _) => RenderActivePiece();
	}
}