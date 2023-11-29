using System;
using UnityEngine;



namespace GGG.Components.Buildings
{
    [Serializable]
    public enum TileType
    {
        Standard,
        Water,
        Cliff,
        Build
    }

    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject Water;
        [SerializeField] private GameObject Cliff;
        [SerializeField] private GameObject Build;

        public GameObject GetTile(TileType tileType)
        {
            return tileType switch
            {
                TileType.Standard => Standar,
                TileType.Water => Water,
                TileType.Cliff => Cliff,
                TileType.Build => Build,
                _ => null
            };
        }
    }
}




