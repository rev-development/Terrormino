using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Tetris
{
    [DefaultExecutionOrder(-1)]
    public class Board : MonoBehaviour
    {
        public Tilemap BoardTilemap;
        public Shape[] Tetrominoes;
        public ActivePieceController ActivePiece;
        public Config Config = new()
        {
            GravityDelay = 1f,
            MoveDelay = 0.1f,
            LockDelay = 0.5f,
        };
        public Vector2Int BoardSize = new(10, 14);
        public RectInt BoardBounds
        {
            get
            {
                return new RectInt(new Vector2Int(-BoardSize.x / 2, -BoardSize.y / 2), BoardSize);
            }
        }
        public Vector3Int SpawnPosition = new(-1, 6, 0);
        public Player.Manager PlayerManager;

        // Assign in Inspector — drag your NightManager GameObject here
        public Game.NightManager NightManager;

        public void Awake()
        {
            for (int i = 0; i < Tetrominoes.Length; i++)
            {
                Tetrominoes[i].Initialize();
            }
        }

        public void Start()
        {
            BoardTilemap = Helpers.Debug.TryFindComponentOnGameObjectByName<Tilemap>(
                "BoardTilemap"
            );
            ActivePiece = Helpers.Debug.TryFindComponent<ActivePieceController>(gameObject);
            PlayerManager = Helpers.Debug.TryFindComponentOnGameObjectByTag<Player.Manager>(
                "Player"
            );
            SpawnPiece();
        }

        public void SpawnPiece()
        {
            // Currently picks randomly from all potential shapes, could use weighting
            int random = Random.Range(0, Tetrominoes.Length);
            Shape shape = Tetrominoes[random];

            // Pass to the PieceController component
            ActivePiece.Initialize(this, SpawnPosition, shape);

            // If the stack is too high that the new piece can't be legally spawned, game ends
            if (IsValidPosition(ActivePiece.Cells, SpawnPosition))
            {
                PaintTiles(ActivePiece);
            }
            else
            {
                Debug.Log(ActivePiece.Shape.ShapeKey);
                Debug.Log("You suck! Tetris");
                BoardTilemap.ClearAllTiles();
                if (SceneManager.GetActiveScene().name == "Gameplay")
                {
                    PlayerManager.GameOver.Invoke();
                }
            }
        }

        public void PaintTiles(ActivePieceController tetromino)
        {
            for (int i = 0; i < tetromino.Cells.Length; i++)
            {
                Vector3Int tilePosition = tetromino.Cells[i] + tetromino.Position;
                BoardTilemap.SetTile(tilePosition, tetromino.Shape.Tile);
            }
        }

        public void UnpaintTiles(ActivePieceController tetromino)
        {
            for (int i = 0; i < tetromino.Cells.Length; i++)
            {
                Vector3Int tilePosition = tetromino.Cells[i] + tetromino.Position;
                BoardTilemap.SetTile(tilePosition, null);
            }
        }

        public bool IsValidPosition(Vector3Int[] cells, Vector3Int position)
        {
            // Validate each cell position
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int tilePosition = cells[i] + position;

                if (!BoardBounds.Contains(new(tilePosition.x, tilePosition.y)))
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
            RectInt bounds = BoardBounds;

            // Iterate through each column, if any are missing then is not full
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row, 0);

                if (!BoardTilemap.HasTile(position))
                {
                    return false;
                }
            }

            return true;
        }

        public void ClearLines()
        {
            RectInt bounds = BoardBounds;
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
                        Vector3Int position = new Vector3Int(col, row, 0);
                        BoardTilemap.SetTile(position, null);
                    }

                    // Shift every row above down one
                    for (int shiftRow = row; shiftRow < bounds.yMax; shiftRow++)
                    {
                        for (int col = bounds.xMin; col < bounds.xMax; col++)
                        {
                            Vector3Int above = new Vector3Int(col, shiftRow + 1, 0);
                            Vector3Int current = new Vector3Int(col, shiftRow, 0);
                            BoardTilemap.SetTile(current, BoardTilemap.GetTile(above));
                        }
                    }
                    // Don't increment row — the row that shifted down needs checking too
                }
                else
                {
                    row++;
                }
            }

            // Report cleared lines to NightManager to track win condition
            if (linesCleared > 0 && NightManager != null)
            {
                NightManager.RegisterLineCleared(linesCleared);
            }
        }
    }
}