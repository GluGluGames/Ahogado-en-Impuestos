using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexTile : MonoBehaviour
{
    public HexTileGenerationSettings settings;
    public HexTileGenerationSettings.TileType tileType;

    public GameObject tilePrefab;
    public GameObject fow;
    public Vector2Int offsetCoordinate;
    public Vector3Int cubeCoordinate;
    public List<HexTile> neighbours;
    private bool isDirty = false;

    // summary:
    //  Generate random type of tile
    public void RollTileType()
    {
        tileType = (HexTileGenerationSettings.TileType)Random.Range(0, 3);
    }

    public void AddTile()
    {
        tilePrefab = Instantiate(settings.GetTile(tileType));
        tilePrefab.transform.SetParent(transform, false);
        

        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        transform.rotation = Quaternion.Euler(-90f, 0f ,0f);
    }

    private void OnValidate()
    {
        if (tilePrefab == null) { return; }
        isDirty = true;
    }

    private void Update()
    {
        if (isDirty)
        {

            if (Application.isPlaying)
            {
                GameObject.Destroy(tilePrefab);
            }
            else
            {
                GameObject.DestroyImmediate(tilePrefab);
            }
            tilePrefab = null;

            AddTile();
            isDirty = false;
        }
    }

    public void OnDrawGizmosSelected()
    {
        foreach (HexTile neighbour in neighbours)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(transform.position, 0.2f);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, neighbour.transform.position);
        }
    }

    public void OnHighlightTile()
    {
        Debug.Log(TileManager.instance);
        TileManager.instance.OnHighlightTile(this);
    }

    public void OnSelectTile()
    {
        TileManager.instance.OnSelectTile(this);

    }
}


