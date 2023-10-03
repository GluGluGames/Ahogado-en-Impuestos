using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;
    public GameObject highlightPrefab;
    public GameObject selectorPrefab;
    private Dictionary<Vector3Int, HexTile> _tilesDic;


    private void Awake()
    {
        instance = this;
        _tilesDic = new Dictionary<Vector3Int, HexTile>();

        HexTile[] hexTiles = gameObject.GetComponentsInChildren<HexTile>();

        // Register every hex tile
        foreach (HexTile hexTile in hexTiles)
        {
            RegisterTile(hexTile);
        }

        foreach (HexTile hexTile in hexTiles)
        {
            List<HexTile> neighbours = GetNeighbours(hexTile);
            hexTile.neighbours = neighbours;
        }

        // Instanciate prefabs for better UI / UX
        highlightPrefab = GameObject.Instantiate(highlightPrefab);
        highlightPrefab.transform.position = new Vector3(100000, 100000, 100000);

        selectorPrefab = GameObject.Instantiate(selectorPrefab);
        selectorPrefab.transform.position = new Vector3(100000, 100000, 100000);
    }

    public void RegisterTile(HexTile tile)
    {
        _tilesDic.Add(tile.cubeCoordinate, tile);
    }

    private List<HexTile> GetNeighbours(HexTile tile)
    {
        List<HexTile> neighbours = new List<HexTile>();

        Vector3Int[] neighbourCoords = new Vector3Int[]
        {
            new Vector3Int(1, -1, 0),
            new Vector3Int(1, 0 , -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1),
            new Vector3Int(0, -1, 1),
        };

        foreach (Vector3Int neighbourCoord in neighbourCoords)
        {
            Vector3Int tileCoord = tile.cubeCoordinate;

            if (_tilesDic.TryGetValue(tileCoord + neighbourCoord, out HexTile neighbour))
            {
                neighbours.Add(neighbour);
            }
        }
        return neighbours;
    }

    public void OnHighlightTile(HexTile tile)
    {
        highlightPrefab.transform.position = tile.transform.position;
    }

    public void OnSelectTile(HexTile tile)
    {
        selectorPrefab.transform.position = new Vector3(0, 0.2f, 0) + tile.transform.position;
    }
}