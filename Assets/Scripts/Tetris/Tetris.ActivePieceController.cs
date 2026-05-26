using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Tetris
{
    public class ActivePieceController : MonoBehaviour
    {

        #region References

        public Board Board;

        #endregion

        public void Initialize(Board board, Vector3Int position, Shape shape)
        {
            Board = board;
            Position = position;
            Shape = shape;

            RotationIndex = 0;

            // Set gravity relative to now so the piece waits exactly one
            // GravityDelay before its first drop no inherited drift
            _gravityTime = Time.time + Board.TetrisConfig.TetrisGravityDelay;
            _moveTime = Time.time;
            _lockTime = 0f;

            // Skip the next Update() so _lockTime starts clean
            _justSpawned = true;

            Cells ??= new Vector3Int[Shape.Cells.Length];
            Cells = Shape.GetCellsAsVec3;
        }

        #region Event Hooks

        public UnityEvent PieceMoved = new();
        public UnityEvent PieceRotated = new();
        public UnityEvent PieceLocked = new();

        #endregion

        #region Input Events

        public UnityEvent<InputAction> MoveInput = new();
        public UnityEvent<InputAction> RotateInput = new();

        #endregion

        #region Input Listeners

        /// <summary>
        ///     Method for handling player initiated movement
        /// </summary>
        /// <param name="inputAction"></param>
        public void OnMove(InputAction inputAction)
        {
            Vector2Int moveInput = new(Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<Vector2>().x, 0.5f), Mathf.Clamp(Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<Vector2>().y, 0.5f), -1, 0));

            Board.UnpaintTiles(this);
            var newPosition = TryMove(moveInput, Cells);

            if (newPosition != null)
            {
                CommitTransform((Vector3Int)newPosition, Cells);
                PieceMoved.Invoke();
            }

            Board.PaintTiles(this);
        }

        /// <summary>
        ///     Method for handling non-player initiated movement (gravity)
        /// </summary>
        /// <param name="moveInput"></param>
        public void OnMove(Vector2Int moveInput)
        {
            Board.UnpaintTiles(this);
            var newPosition = TryMove(moveInput, Cells);

            if (newPosition != null)
            {
                CommitTransform((Vector3Int)newPosition, Cells);
                PieceMoved.Invoke();
            }

            Board.PaintTiles(this);
        }

        public void OnRotate(InputAction inputAction)
        {
            int rotateInput = Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<float>());

            Board.UnpaintTiles(this);

            var newCells = GenerateRotationCells(rotateInput);

            var newPosition = TryRotate(rotateInput, newCells);

            if (newPosition != null)
            {
                CommitTransform((Vector3Int)newPosition, newCells);
                PieceRotated.Invoke();
            }

            Board.PaintTiles(this);
        }

        #endregion

        #region Runtime Values

        [SerializeField] public Shape Shape;
        [SerializeField] public Vector3Int[] Cells;
        [SerializeField] public Vector3Int Position;
        [SerializeField] public int RotationIndex;
        [SerializeField] private float _moveTime;
        [SerializeField] private float _gravityTime;
        [SerializeField] private float _lockTime;
        /// <summary>
        ///     Flag to skip one Update() after a new piece spawns so _lockTime doesn't carry over from the previous piece into the
        ///     new one
        /// </summary>
        [SerializeField] private bool _justSpawned = false;

        #endregion

        #region Subfunctions

        private void LockMovement()
        {
            Board.PaintTiles(this);
            PieceLocked.Invoke();
            Board.ClearLines();
            Board.SpawnPiece();
        }

        public void CommitTransform(Vector3Int position, Vector3Int[] cells)
        {
            Cells = cells;
            Position = position;
            _moveTime = Time.time;
            _lockTime = 0f;
        }

        private Vector3Int? TryMove(Vector2Int moveInput, Vector3Int[] cells)
        {
            var newPosition = Position;

            newPosition.x += moveInput.x;
            newPosition.y += moveInput.y;

            return Board.IsValidPosition(cells, newPosition) ? newPosition : null;
        }

        private Vector3Int? TryRotate(int rotateInput, Vector3Int[] cells)
        {
            int wallKickIndex = Helpers.Math.Wrap(((rotateInput + RotationIndex) * 2) - (rotateInput < 0 ? 1 : 0), 0, Shape.WallKicks.GetLength(0));

            for (int i = 0; i < Shape.WallKicks.GetLength(1); i++)
            {
                var wallKickMoveInput = Shape.WallKicks[wallKickIndex, i];

                if (TryMove(wallKickMoveInput, cells) != null)
                {
                    return TryMove(wallKickMoveInput, cells);
                }
            }

            return null;
        }

        private Vector3Int[] GenerateRotationCells(int rotateInput)
        {
            var newCells = new List<Vector3Int>(Cells).ToArray();

            float[] matrix = ShapeVecs.RotationMatrix;

            for (int i = 0; i < newCells.Length; i++)
            {
                Vector3 cell = newCells[i];

                int x;
                int y;

                switch (Shape.ShapeKey)
                {
                    case ShapeKeys.I:
                    case ShapeKeys.O:
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt((cell.x * matrix[0] * rotateInput) + (cell.y * matrix[1] * rotateInput));
                        y = Mathf.CeilToInt((cell.x * matrix[2] * rotateInput) + (cell.y * matrix[3] * rotateInput));

                        break;
                    default:
                        x = Mathf.RoundToInt((cell.x * matrix[0] * rotateInput) + (cell.y * matrix[1] * rotateInput));
                        y = Mathf.RoundToInt((cell.x * matrix[2] * rotateInput) + (cell.y * matrix[3] * rotateInput));

                        break;
                }

                newCells[i] = new Vector3Int(x, y, 0);
            }

            return newCells;
        }

        #endregion

        #region Lifecycle

        public void OnDisable()
        {
            PieceMoved.RemoveAllListeners();
            PieceRotated.RemoveAllListeners();
            PieceLocked.RemoveAllListeners();
            MoveInput.RemoveAllListeners();
            RotateInput.RemoveAllListeners();
        }

        public void Start()
        {
            Helpers.Debug.CheckIfSetInInspector(gameObject, Board, "Board");

            MoveInput.AddListener(OnMove);
            RotateInput.AddListener(OnRotate);
        }

        public void Update()
        {
            // Skip one frame after spawn so _lockTime is clean
            if (_justSpawned)
            {
                _justSpawned = false;
                Board.PaintTiles(this);

                return;
            }

            Board.UnpaintTiles(this);

            _lockTime += Time.deltaTime;

            if (Time.time > _gravityTime)
            {
                _gravityTime = Time.time + Board.TetrisConfig.TetrisGravityDelay;

                OnMove(Vector2Int.down);

                if (_lockTime >= Board.TetrisConfig.TetrisLockDelay)
                {
                    LockMovement();

                    return;
                }
            }

            Board.PaintTiles(this);
        }

        #endregion

    }
}