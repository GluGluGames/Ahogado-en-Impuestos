using UnityEngine;



namespace GGG.Components.Buildings
{
    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        public enum TileType
        {
            Standard,
            Water,
            Cliff
        }

        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject Water;
        [SerializeField] private GameObject Cliff;

        public GameObject GetTile(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Standard: return Standar;
                case TileType.Water: return Water;
                case TileType.Cliff: return Cliff;
            }
            return null;
        }
    }
}




