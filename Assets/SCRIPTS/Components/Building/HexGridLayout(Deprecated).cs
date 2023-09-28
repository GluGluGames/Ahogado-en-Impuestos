using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class HexGridLayout : MonoBehaviour
{

    [Header("Grid Settings")]
    public Vector2Int gridSize;

    [Header("Tile Settings")]
    public float outerSize = 1f;
    public float height = 1f;
    public bool isFlatTopped;
    public Material material;
    public GameObject tilePrefab;


    private void OnEnable()
    {
        LayoutGrid();
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            LayoutGrid();
        }
    }

    private void LayoutGrid()
    {
        for(int y = 0; y < gridSize.y; y++) 
        {
            for(int x = 0; x < gridSize.x; x++) 
            {
                GameObject tile = Instantiate(tilePrefab);
                tile.transform.position = GetPositionForHexFromCoordinate(new Vector2Int(x, y));
            }
        }
    }

    private Vector3 GetPositionForHexFromCoordinate(Vector2Int coord)
    {
        int column = coord.x;
        int row = coord.y;

        float width;
        float height;
        float xPosition;
        float yPosition;
        bool shouldOffset;
        float horizontalDistance;
        float verticalDistance;
        float offset;
        float radious = outerSize;

        if(!isFlatTopped)
        {
            shouldOffset = (row % 2) == 0;
            width = Mathf.Sqrt(3) * radious;
            height = 2f * radious;

            horizontalDistance = width;
            verticalDistance = height * (3f / 4f);

            offset = (shouldOffset) ? width/2 : 0;

            xPosition = (column * (horizontalDistance)) + offset;
            yPosition = (row * verticalDistance);
        }
        else
        {
            shouldOffset = (column % 2) == 0;

            width = 2f * radious;
            height = Mathf.Sqrt(3) * radious;

            horizontalDistance = width * (3f / 4f);
            verticalDistance = height;

            offset = shouldOffset ? height/2 : 0;
            xPosition = (column * (horizontalDistance));
            yPosition = (row * verticalDistance) - offset;
        }

        return new Vector3(xPosition, 0, -yPosition);
    }

}
