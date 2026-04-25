using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;

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

        private float _moveTime;
        public float _lockTime;

        //Audio sources and audio clips for rotating tetris blocks & when tetris blocks lock in place
        public AudioSource RotateSound;
        public AudioClip[] RotateClips;
        public AudioSource TouchSound;
        public AudioClip[] TouchClips;

        // Flag to skip one Update() after a new piece spawns so _lockTime
        // doesn't carry over from the previous piece into the new one
        private bool _justSpawned = false;

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
                PlayRotateSound();
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
                PlayRotateSound();
                CommitPlayerTransform((Vector3Int)newPosition, newCells);
            }
            Board.PaintTiles(this);
        }

        private Vector3Int? TryMove(Vector2Int moveInput, Vector3Int[] cells)
        {
            Vector3Int newPosition = Position;

            newPosition.x += moveInput.x;
            newPosition.y += moveInput.y;

            return Board.IsValidPosition(cells, newPosition) ? newPosition : null;
        }

        private Vector3Int? TryRotate(int rotateInput, Vector3Int[] cells)
        {
            int wallKickIndex = Helpers.Math.Wrap(
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

            // Set gravity relative to now so the piece waits exactly one
            // GravityDelay before its first drop — no inherited drift
            _gravityTime = Time.time + Board.Config.GravityDelay;
            _moveTime = Time.time;
            _lockTime = 0f;

            // Skip the next Update() so _lockTime starts clean
            _justSpawned = true;

            Cells ??= new Vector3Int[Shape.Cells.Length];
            Cells = Shape.GetCellsAsVec3;
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
            PlayTouchSound();
        }

        public void PlayRotateSound()
        {
            if (RotateClips.Length == 0) return;

            int index = Random.Range(0, RotateClips.Length);
            RotateSound.PlayOneShot(RotateClips[index]);
        }

        public void PlayTouchSound()
        {
            if (TouchClips.Length == 0) return;

            int index = Random.Range(0, TouchClips.Length);
            TouchSound.PlayOneShot(TouchClips[index]);
        }
    }
}