using Helpers.Attributes;
using Helpers.Ext;
using JetBrains.Annotations;
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
	[DisallowMultipleComponent]
	[RequireComponent(typeof(EventBus))]
	[RequireComponent(typeof(Controller))]
	[AddComponentMenu("EC.Tetris.PlayfieldRenderer")]
	public class PlayfieldRenderer : MonoBehaviour
	{
		[DisableInEditor] [SerializeField] private Controller _controller;

		[DisableInEditor] [SerializeField] private EventBus _eventBus;

		[SerializeField] private Tilemap _playfieldTilemap;

		[SerializeField] private Tilemap _activePieceTilemap;

		[SerializeField] private Tilemap _ghostTilemap;

		[SerializeField] private Tilemap _backgroundTilemap;

		private Playfield _playfield => _controller.Playfield;

		[UsedImplicitly]
		public void Awake()
		{
			_controller = gameObject.TryFindComponent<Controller>();
			_eventBus = gameObject.TryFindComponent<EventBus>();
			gameObject.CheckIfSetInInspector(_playfieldTilemap, "Tetris Playfield Tilemap");
			gameObject.CheckIfSetInInspector(_activePieceTilemap, "Tetris Active Piece Tilemap");
			gameObject.CheckIfSetInInspector(_ghostTilemap, "Tetris Ghost Tilemap");
			gameObject.CheckIfSetInInspector(_backgroundTilemap, "Tetris Background Tilemap");
		}

		private void OnEnable()
		{
			_eventBus.GameStart.AddListener(RenderBackground);

			_eventBus.Moved.AddListener(Render);

			_eventBus.Rotated.AddListener(Render);

			_eventBus.HardDropped.AddListener(Render);

			_eventBus.Spawned.AddListener(Render);

			_eventBus.Locked.AddListener(Render);

			_eventBus.LinesCleared.AddListener(Render);
		}

		private void OnDisable()
		{
			_eventBus.GameStart.RemoveListener(RenderBackground);
			_eventBus.Moved.RemoveListener(Render);

			_eventBus.Rotated.RemoveListener(Render);

			_eventBus.Spawned.RemoveListener(Render);

			_eventBus.Locked.RemoveListener(Render);

			_eventBus.LinesCleared.RemoveListener(Render);
		}

		private void RenderBackground()
		{
			_backgroundTilemap.ClearAllTiles();

			for (var x = 0; x < _playfield.Width; x++)
			{
				for (var y = 0; y < _playfield.VisibleHeight; y++)
					_backgroundTilemap.SetTile(new Vector3Int(x, y), _eventBus.Config.BgTile);
			}
		}

		private void RenderPlayfield()
		{
			_playfieldTilemap.ClearAllTiles();

			for (var x = 0; x < _playfield.Width; x++)
			{
				for (var y = 0; y < _playfield.VisibleHeight; y++)
				{
					if (_playfield.IsOccupied(x, y)) _playfieldTilemap.SetTile(new Vector3Int(x, y), _playfield.GetTile(x, y));
				}
			}
		}

		private void RenderActivePiece()
		{
			_activePieceTilemap.ClearAllTiles();

			if (_controller.ActivePiece is not { } piece) return;

			var tile = piece.Shape.Tile;

			foreach (var cell in piece.PlayfieldSpaceCells)
				if (!_playfield.IsInSpawnBuffer(cell.x, cell.y))
					_activePieceTilemap.SetTile(new Vector3Int(cell.x, cell.y), tile);
		}

		private void RenderGhost()
		{
			_ghostTilemap.ClearAllTiles();

			if (!_eventBus.Config.GhostEnabled
					|| _controller.ActivePiece is not { } piece)
				return; // The "_controller.ActivePiece is not { } piece" syntax checks for null and if it is not null, then it unwraps _controller.ActivePice.Value as piece

			var ghostDistance = Rules.GetGhostDistance(_controller.Playfield, piece);
			var tile = piece.Shape.Tile;

			foreach (var cell in piece.PlayfieldSpaceCells)
				_ghostTilemap.SetTile(new Vector3Int(cell.x, cell.y - ghostDistance), tile);
		}

		private void Render()
		{
			// Logger.LogCode(Logger.Code.RenderTriggered);
			RenderActivePiece();
			RenderGhost();
			RenderPlayfield();
		}

		private void Render<T0>(T0 _) => Render(); // Shim for UnityEvents with params
	}
}