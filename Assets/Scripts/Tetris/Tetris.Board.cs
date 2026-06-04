using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

namespace Tetris
{
    [DefaultExecutionOrder(-1)]
    public class Board : MonoBehaviour
    {

        public void SpawnPiece()
        {
            // Currently picks randomly from all potential shapes, could use weighting
            int random = Random.Range(0, Tetrominoes.Length);
            var shape = Tetrominoes[random];

            // Pass to the PieceController component
            ActivePiece.Initialize(this, SpawnPosition, shape);

            // If the stack is too high that the new piece can't be legally spawned, game ends
            if (IsValidPosition(ActivePiece.Cells, SpawnPosition))
            {
                PaintTiles(ActivePiece);
                PieceSpawned.Invoke();
            }
            else
            {
                BoardTilemap.ClearAllTiles();
                TetrisLose.Invoke();
            }
        }

        public void ClearLines()
        {
            var bounds = BoardBounds;
            int row = bounds.yMin;
            int linesCleared = 0; // Track how many rows we clear this piece

            // Clear from bottom to top
            while (row < bounds.yMax)
            {
                // Only advance to the next row if the current is not cleared
                // because the tiles above will fall down when a row is cleared
                if (IsLineFull(row))
                {
                    linesCleared++;

                    // Clear all tiles in the row
                    for (int col = bounds.xMin; col < bounds.xMax; col++)
                    {
                        var position = new Vector3Int(col, row, 0);
                        BoardTilemap.SetTile(position, null);
                    }

                    // Shift every row above down one
                    for (int shiftRow = row; shiftRow < bounds.yMax; shiftRow++)
                    {
                        for (int col = bounds.xMin; col < bounds.xMax; col++)
                        {
                            var above = new Vector3Int(col, shiftRow + 1, 0);
                            var current = new Vector3Int(col, shiftRow, 0);
                            BoardTilemap.SetTile(current, BoardTilemap.GetTile(above));
                        }
                    }
                    // Don't increment row the row that shifted down needs checking too
                }
                else
                {
                    row++;
                }
            }

            // Report cleared lines to NightManager to track win condition
            LineCleared.Invoke(linesCleared);
        }

        #region Sub-Functions

        /// <summary>
        ///     Draws all the current tiles
        ///     NOTE: Pieces do not 'stay together' once they are locked in place, the only time tiles are grouped together is when
        ///     the player is controlling them
        ///     Called each frame by Tetris.ActivePieceController
        /// </summary>
        /// <param name="tetromino"></param>
        public void PaintTiles(ActivePieceController tetromino)
        {
            for (int i = 0; i < tetromino.Cells.Length; i++)
            {
                var tilePosition = tetromino.Cells[i] + tetromino.Position;
                BoardTilemap.SetTile(tilePosition, tetromino.Shape.Tile);
            }
        }

        /// <summary>
        ///     Cleans the tilemap to be redrawn later
        /// </summary>
        /// <param name="tetromino"></param>
        public void UnpaintTiles(ActivePieceController tetromino)
        {
            for (int i = 0; i < tetromino.Cells.Length; i++)
            {
                var tilePosition = tetromino.Cells[i] + tetromino.Position;
                BoardTilemap.SetTile(tilePosition, null);
            }
        }

        public bool IsValidPosition(Vector3Int[] cells, Vector3Int position)
        {
            // Validate each cell position
            for (int i = 0; i < cells.Length; i++)
            {
                var tilePosition = cells[i] + position;

                if (!BoardBounds.Contains(new Vector2Int(tilePosition.x, tilePosition.y)))
                {
                    return false;
                }

                if (BoardTilemap.HasTile(tilePosition))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsLineFull(int row)
        {
            var bounds = BoardBounds;

            // Iterate through each column, if any are missing then is not full
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                var position = new Vector3Int(col, row, 0);

                if (!BoardTilemap.HasTile(position))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region Lifecycle

        public void Awake()
        {
            if (Instance != null
                && Instance != this)
            {
                Destroy(gameObject);

                return;
            }

            Instance = this;

            for (int i = 0; i < Tetrominoes.Length; i++)
            {
                Tetrominoes[i].Initialize();
            }
        }

        public void Start()
        {
            BoardTilemap = Helpers.Debug.TryFindComponentOnGameObjectByName<Tilemap>("BoardTilemap");
            ActivePiece = Helpers.Debug.TryFindComponent<ActivePieceController>(gameObject);

            SpawnPiece();
        }

        public void OnDisable()
        {
            LineCleared.RemoveAllListeners();
            TetrisLose.RemoveAllListeners();
            PieceSpawned.RemoveAllListeners();
        }

        #endregion

        #region Events

        /// <summary>
        ///     Triggered when a Tetris line is cleared with the amount of lines cleared
        ///     GameLoop.NightManager listens to this event and responds with RegisterLineCleared
        /// </summary>
        public UnityEvent<int> LineCleared = new();
        
        /// <summary>
        ///     Triggered when Player loses Tetris
        ///     Player.Manager listens to this event and responds with GameOver
        /// </summary>
        public UnityEvent TetrisLose = new();

        /// <summary>
        /// </summary>
        public UnityEvent PieceSpawned = new();

        #endregion

        #region Config Values

        // ReSharper disable once ConvertToAutoProperty
        public ITetrisConfig TetrisConfig => _tetrisConfig;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private ITetrisConfig _tetrisConfig = new TetrisConfig { TetrisGravityDelay = 1f, TetrisMoveDelay = 0.1f, TetrisLockDelay = 0.5f };

        public void ApplyTetrisConfig(ITetrisConfig tetrisConfig)
        {
            TetrisConfig.TetrisGravityDelay = tetrisConfig.TetrisGravityDelay;
            TetrisConfig.TetrisMoveDelay = tetrisConfig.TetrisMoveDelay;
            TetrisConfig.TetrisLockDelay = tetrisConfig.TetrisLockDelay;
        }

        public Vector2Int BoardSize = new(10, 14);

        public Vector3Int SpawnPosition = new(-1, 6, 0);

        public RectInt BoardBounds => new(new Vector2Int(-BoardSize.x / 2, -BoardSize.y / 2), BoardSize);

        #endregion

        #region References

        /// <summary>
        ///     Allows global access through Tetris.Board.Instance and enables Singleton behavior
        /// </summary>
        public static Board Instance { get; private set; }

        public ActivePieceController ActivePiece;

        public Tilemap BoardTilemap;

        public Shape[] Tetrominoes;

        #endregion

    }
}