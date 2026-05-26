using System.Collections.Generic;
using UnityEngine;

namespace Helpers
{
    public static class HexMap
    {

        public static List<Vector3Int> AdjacencyMatrix = new()
        {
            new Vector3Int(-1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, -1, 0)
        };

        public static List<Vector3Int> GetAdjacentCells(Vector3Int hexCords) {
            var adjacentCells = new List<Vector3Int>();

            AdjacencyMatrix.ForEach(adjVec => adjacentCells.Add(adjVec + hexCords));

            return adjacentCells;
        }

    }
}