using UnityEngine;
using Random = UnityEngine.Random;


namespace GGG.Components.HexagonalGrid
{
    public enum TileType
    {
        Standard,
        Water,
        Cliff,
        Build,
        Mountain
    }

    [CreateAssetMenu(menuName = "TileGen/GenerationSettings")]
    public class HexTileGenerationSettings : ScriptableObject
    {
        [SerializeField] private GameObject Standar;
        [SerializeField] private GameObject[] Water;
        [SerializeField] private GameObject[] Cliff;
        [SerializeField] private GameObject Build;
        [SerializeField] private GameObject Mountain;

        public GameObject GetTile(TileType tileType)
        {
            return tileType switch
            {
                TileType.Standard => Standar,
                TileType.Water => Water[Random.Range(0, Water.Length)],
                TileType.Cliff => Cliff[Random.Range(0, Cliff.Length)],
                TileType.Build => Build,
                TileType.Mountain => Mountain,
                _ => null
            };
        }
    }
}




