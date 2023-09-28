using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexTile : MonoBehaviour
{
    public HexTileGenerationSettings settings;
    public HexTileGenerationSettings.tileType tileType;

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
        tileType = (HexTileGenerationSettings.tileType)Random.Range(0,3);
    }

    public void AddTile()
    {
        tilePrefab = Instantiate(settings.GetTile(tileType));
        tilePrefab.transform.SetParent(transform, false);
        

        if(gameObject.GetComponent<MeshCollider>() == null )
        {
            // !BUG: Collider appears aligned with y axis instead of z (vertical instead of horizontal)
            //MeshCollider collider = gameObject.AddComponent<MeshCollider>();
            //collider.sharedMesh = GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        Debug.Log(tilePrefab.transform.position);
    }

    private void OnValidate()
    {
        if(tilePrefab == null) { return; }
        isDirty = true;
    }

    private void Update()
    {
        if(isDirty)
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
}
