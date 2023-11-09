using UnityEngine;



namespace GGG.Components.Buildings
{
    public enum TileType
    {
        Standard,
        Water,
        Cliff,
        Unwalkable
    }

    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject Water;
        [SerializeField] private GameObject Cliff;
        [SerializeField] private GameObject Unwalkable;

        public GameObject GetTile(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Standard: return Standar;
                case TileType.Water: return Water;
                case TileType.Cliff: return Cliff;
                case TileType.Unwalkable: return Unwalkable;
            }
            return null;
        }
    }
}




