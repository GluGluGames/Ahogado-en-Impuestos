using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(HexGrid))]
public class customInspectorGUI : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HexGrid hgrid = (HexGrid)target;
        if (GUILayout.Button("Layout"))
        {
            hgrid.LayoutGrid();
        }
    }
}
public class HexGrid : MonoBehaviour
{

    [Header("Grid Settings")]
    public Vector2Int gridSize;
    public float radius = 1f;
    public bool isFlatTopped;

    public HexTileGenerationSettings settings;

    public void Clear()
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            children.Add(child);
        }

        foreach(GameObject child in children)
        {
            DestroyImmediate(child, true);
        }
    }


    public void LayoutGrid()
    {
        Clear();
        for (int y = 0; y < gridSize.y; y++)
        {
            for(int x = 0; x < gridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex C{x},R{y}");
                tile.transform.SetParent(transform, true);
                
                HexTile hexTile = tile.AddComponent<HexTile>();
                hexTile.transform.position = HexUtilities.GetPositionForHexFromCoordinate(new Vector2Int(x, y), radius, isFlatTopped);
                hexTile.settings = settings;
                hexTile.RollTileType();
                hexTile.AddTile();
                
                // Assign its offset coordinates for human parsing (Column, Row)
                hexTile.offsetCoordinate = new Vector2Int(x, y);

                // Assign / convert these to cube coordinates for future navigation
                hexTile.cubeCoordinate = HexUtilities.OffsetToCube(hexTile.offsetCoordinate);

                // Move hex to correct position


            }
        }
    }
}
