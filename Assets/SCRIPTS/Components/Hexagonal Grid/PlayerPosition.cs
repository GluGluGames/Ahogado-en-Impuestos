using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.HexagonalGrid
{
    public static class PlayerPosition
    {
        /// <summary>
        /// Player position on cube coordinates
        /// </summary>
        public static Vector3Int PlayerPos = Vector3Int.zero;
        public static Vector3 TargetPosition;
        public static HexTile CurrentTile;
        public static HexTile NextTile;
        public static List<HexTile> currentPath = new List<HexTile>();
        public static float heightOffset = 0.6f;
    }
}
