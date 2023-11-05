using UnityEngine;



namespace GGG.Components.Buildings
{
    public enum TileType
    {
        Standard,
        Water,
        Cliff,
        UnWalkable
    }

    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject Water;
        [SerializeField] private GameObject Cliff;
        [SerializeField] private GameObject UnWalkable;

        public GameObject GetTile(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Standard: return Standar;
                case TileType.Water: return Water;
                case TileType.Cliff: return Cliff;
                case TileType.UnWalkable: return UnWalkable;
            }
            return null;
        }
    }
}




