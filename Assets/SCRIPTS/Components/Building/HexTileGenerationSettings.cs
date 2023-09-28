using Codice.CM.WorkspaceServer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="TileGen/GenerationSettings")]
public class HexTileGenerationSettings : ScriptableObject
{

    public enum tileType
    { 
        Standard, 
        Water,
        Cliff
    }

    public GameObject standar;
    public GameObject water;
    public GameObject cliff;

    public GameObject GetTile(tileType tileType)
    {
        switch (tileType)
        {
            case tileType.Standard: return standar;
            case tileType.Water: return water;
            case tileType.Cliff: return cliff;
        }
        return null;
    }
}
