using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Tetris
{
    public class ActivePieceController : MonoBehaviour
    {
        public Board Board;
        public Shape Shape;
        public Vector3Int[] Cells;
        public Vector3Int Position;
        public int RotationIndex;

        public float _gravityTime;

        // TODO: Reimplement repeated movement handling, potentially use hold action on action map
        private float _moveTime;
        public float _lockTime;

        public UnityEvent<InputAction> Move = new();
        public UnityEvent<InputAction> Rotate = new();

        public void CommitPlayerTransform(Vector3Int position, Vector3Int[] cells)
        {
            Cells = cells;
            Position = position;
            _moveTime = Time.time;
            _lockTime = 0f;
        }

        public void OnMove(InputAction inputAction)
        {
            Vector2Int moveInput = new(
                Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<Vector2>().x, 0.5f),
                Mathf.Clamp(
                    Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<Vector2>().y, 0.5f),
                    -1,
                    0
                )
            );

            Board.UnpaintTiles(this);
            var newPosition = TryMove(moveInput, Cells);

            if (newPosition != null)
            {
                CommitPlayerTransform((Vector3Int)newPosition, Cells);
            }
            Board.PaintTiles(this);
        }

        public void OnMove(Vector2Int moveInput)
        {
            Board.UnpaintTiles(this);
            var newPosition = TryMove(moveInput, Cells);

            if (newPosition != null)
            {
                CommitPlayerTransform((Vector3Int)newPosition, Cells);
            }
            Board.PaintTiles(this);
        }

        public void OnRotate(InputAction inputAction)
        {
            int rotateInput = Helpers.Math.RoundNearestNonZeroInt(inputAction.ReadValue<float>());

            Board.UnpaintTiles(this);

            Vector3Int[] newCells = GenerateRotationCells(rotateInput);

            var newPosition = TryRotate(rotateInput, newCells);

            if (newPosition != null)
            {
                CommitPlayerTransform((Vector3Int)newPosition, newCells);
            }
            Board.PaintTiles(this);
        }

        private Vector3Int? TryMove(Vector2Int moveInput, Vector3Int[] cells)
        {
            Vector3Int newPosition = Position;

            newPosition.x += moveInput.x;
            newPosition.y += moveInput.y;

            // Return the newPosition if valid, otherwise just pass back original
            // Removes the weird bool check in original and avoids a null return
            return Board.IsValidPosition(cells, newPosition) ? newPosition : null;
        }

        private Vector3Int? TryRotate(int rotateInput, Vector3Int[] cells)
        {
            // See Wall Kick: https://tetris.wiki/Super_Rotation_System#Wall_Kicks
            // Fetches an index to a presaved transformation of the shape vector

            int wallKickIndex = Helpers.Math.Wrap(
                // Add input to existing and multiply by 2, if rotateInput is negative then subtract 1
                // The array for wall kicks basically has 2 vector arrays for each orientation, clockwise and counterclockwise
                ((rotateInput + RotationIndex) * 2) - (rotateInput < 0 ? 1 : 0),
                0,
                Shape.WallKicks.GetLength(0)
            );

            for (int i = 0; i < Shape.WallKicks.GetLength(1); i++)
            {
                Vector2Int wallKickMoveInput = Shape.WallKicks[wallKickIndex, i];

                if (TryMove(wallKickMoveInput, cells) != null)
                {
                    return TryMove(wallKickMoveInput, cells);
                }
            }

            return null;
        }

        private Vector3Int[] GenerateRotationCells(int rotateInput)
        {
            // Makes an non-reference copy of the array
            Vector3Int[] newCells = new List<Vector3Int>(Cells).ToArray();

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
                        // "I" and "O" are rotated from an offset center point
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        x = Mathf.CeilToInt(
                            (cell.x * matrix[0] * rotateInput) + (cell.y * matrix[1] * rotateInput)
                        );
                        y = Mathf.CeilToInt(
                            (cell.x * matrix[2] * rotateInput) + (cell.y * matrix[3] * rotateInput)
                        );
                        break;
                    default:
                        x = Mathf.RoundToInt(
                            (cell.x * matrix[0] * rotateInput) + (cell.y * matrix[1] * rotateInput)
                        );
                        y = Mathf.RoundToInt(
                            (cell.x * matrix[2] * rotateInput) + (cell.y * matrix[3] * rotateInput)
                        );
                        break;
                }

                newCells[i] = new Vector3Int(x, y, 0);
            }

            return newCells;
        }

        public void Start()
        {
            Helpers.Debug.CheckIfSetInInspector(gameObject, Board, "Board");

            Move.AddListener(OnMove);
            Rotate.AddListener(OnRotate);
        }

        public void Initialize(Board board, Vector3Int position, Shape shape)
        {
            Board = board;
            Position = position;
            Shape = shape;

            RotationIndex = 0;

            _gravityTime = Time.time + Board.Config.GravityDelay;
            _moveTime = Time.time;
            _lockTime = 0f;

            // This is a Null-coalescing assignment, if the value on the left is null then it assigns the value on the right
            // It's the same as:
            // if (Cells == null) { Cells = new Vector3Int[Shape.Cells.Length]; }
            Cells ??= new Vector3Int[Shape.Cells.Length];

            Cells = Shape.GetCellsAsVec3;
        }

        public void Update()
        {
            Board.UnpaintTiles(this);

            // Timer before piece can no longer be moved
            _lockTime += Time.deltaTime;

            if (Time.time > _gravityTime)
            {
                _gravityTime = Time.time + Board.Config.GravityDelay;

                OnMove(Vector2Int.down);

                if (_lockTime >= Board.Config.LockDelay)
                {
                    LockMovement();
                    return;
                }
            }

            Board.PaintTiles(this);
        }

        private void LockMovement()
        {
            Board.PaintTiles(this);
            Board.ClearLines();
            Board.SpawnPiece();
        }
    }
}
