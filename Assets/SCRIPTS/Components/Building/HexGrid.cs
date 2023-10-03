using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;


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
    [SerializeField] private Vector2Int GridSize;
    [SerializeField] private float Radius = 1f;
    [SerializeField] private bool IsFlatTopped;

    [SerializeField] private HexTileGenerationSettings Settings;

    public void Clear()
    {
        List<GameObject> children = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            children.Add(child);
        }

        foreach (GameObject child in children)
        {
            DestroyImmediate(child, true);
        }
    }


    public void LayoutGrid()
    {
        Clear();
        for (int y = 0; y < GridSize.y; y++)
        {
            for (int x = 0; x < GridSize.x; x++)
            {
                GameObject tile = new GameObject($"Hex C{x},R{y}");
                tile.transform.SetParent(transform, true);

                HexTile hexTile = tile.AddComponent<HexTile>();
                hexTile.transform.position = HexUtilities.GetPositionForHexFromCoordinate(new Vector2Int(x, y), Radius, IsFlatTopped);
                hexTile.settings = Settings;
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


