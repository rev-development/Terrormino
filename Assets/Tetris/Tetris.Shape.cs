using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Tetris
{
    public enum ShapeKeys
    {
        I,

        /*   []
             []
             []
             []   */
        J,

        /*   []
             []
           [][]   */
        L,

        /*   []
             []
             [][] */
        O,

        /* [][]
           [][]   */
        S,

        /*   [][]
           [][]   */
        T,

        /* [][][]
             []   */
        Z,
        /* [][]
             [][] */
    }

    [System.Serializable]
    public static class ShapeVecs
    {
        // This looks complicated but it's just matrix math for rotating things
        public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
        public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
        public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

        public static readonly Dictionary<ShapeKeys, Vector2Int[]> Cells = new Dictionary<
            ShapeKeys,
            Vector2Int[]
        >()
        {
            {
                ShapeKeys.I,
                new Vector2Int[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(2, 1),
                }
            },
            {
                ShapeKeys.J,
                new Vector2Int[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }
            },
            {
                ShapeKeys.L,
                new Vector2Int[]
                {
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }
            },
            {
                ShapeKeys.O,
                new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }
            },
            {
                ShapeKeys.S,
                new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                }
            },
            {
                ShapeKeys.T,
                new Vector2Int[]
                {
                    new Vector2Int(0, 1),
                    new Vector2Int(-1, 0),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }
            },
            {
                ShapeKeys.Z,
                new Vector2Int[]
                {
                    new Vector2Int(-1, 1),
                    new Vector2Int(0, 1),
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                }
            },
        };

        private static readonly Vector2Int[,] _wallKicksI = new Vector2Int[,]
        {
            {
                new Vector2Int(0, 0),
                new Vector2Int(-2, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-2, -1),
                new Vector2Int(1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(2, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(2, 1),
                new Vector2Int(-1, -2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(-1, 2),
                new Vector2Int(2, -1),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-2, 0),
                new Vector2Int(1, -2),
                new Vector2Int(-2, 1),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(2, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(2, 1),
                new Vector2Int(-1, -2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-2, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-2, -1),
                new Vector2Int(1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(-2, 0),
                new Vector2Int(1, -2),
                new Vector2Int(-2, 1),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(-1, 2),
                new Vector2Int(2, -1),
            },
        };

        private static readonly Vector2Int[,] _wallKicksJLOSTZ = new Vector2Int[,]
        {
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, -2),
                new Vector2Int(-1, -2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(1, -1),
                new Vector2Int(0, 2),
                new Vector2Int(1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(0, -2),
                new Vector2Int(-1, -2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(0, -2),
                new Vector2Int(1, -2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, -1),
                new Vector2Int(0, 2),
                new Vector2Int(-1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(-1, -1),
                new Vector2Int(0, 2),
                new Vector2Int(-1, 2),
            },
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(1, 1),
                new Vector2Int(0, -2),
                new Vector2Int(1, -2),
            },
        };

        public static readonly Dictionary<ShapeKeys, Vector2Int[,]> WallKicks = new Dictionary<
            ShapeKeys,
            Vector2Int[,]
        >()
        {
            { ShapeKeys.I, _wallKicksI },
            { ShapeKeys.J, _wallKicksJLOSTZ },
            { ShapeKeys.L, _wallKicksJLOSTZ },
            { ShapeKeys.O, _wallKicksJLOSTZ },
            { ShapeKeys.S, _wallKicksJLOSTZ },
            { ShapeKeys.T, _wallKicksJLOSTZ },
            { ShapeKeys.Z, _wallKicksJLOSTZ },
        };
    };

    [System.Serializable]
    public struct Shape
    {
        public ShapeKeys ShapeKey;
        public Tile Tile;

        // { get; private set; } is "Others can look but not touch"
        public Vector2Int[] Cells { get; private set; }
        public Vector2Int[,] WallKicks { get; private set; }

        public void Initialize()
        {
            Cells = ShapeVecs.Cells[ShapeKey];
            WallKicks = ShapeVecs.WallKicks[ShapeKey];
        }

        public Vector3Int[] GetCellsAsVec3
        {
            get
            {
                Vector3Int[] vector3Ints = new Vector3Int[Cells.Length];

                for (int i = 0; i < Cells.Length; i++)
                {
                    vector3Ints[i] = new Vector3Int(Cells[i].x, Cells[i].y, 0);
                }

                return vector3Ints;
            }
        }
    }

    [System.Serializable]
    public struct Config
    {
        public float GravityDelay;
        public float MoveDelay;
        public float LockDelay;
    }
}
