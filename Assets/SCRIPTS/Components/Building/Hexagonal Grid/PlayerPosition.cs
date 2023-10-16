using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Buildings
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
        public static List<HexTile> CurrentPath = new List<HexTile>();
    }
}
