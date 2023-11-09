using UnityEngine;



namespace GGG.Components.Buildings
{
    public enum TileType
    {
        Standard,
        Water,
        Cliff,
        Walkable
    }

    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject Water;
        [SerializeField] private GameObject Cliff;
        [SerializeField] private GameObject Walkable;

        public GameObject GetTile(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Standard: return Standar;
                case TileType.Water: return Water;
                case TileType.Cliff: return Cliff;
                case TileType.Walkable: return Walkable;
            }
            return null;
        }
    }
}




