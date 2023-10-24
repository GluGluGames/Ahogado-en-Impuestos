using GGG.Components.Buildings;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicMovement : ScriptableObject, IMovementController
{
    public bool gotPath = false;
    public HexTile currentTile;
    public HexTile nextTile;
    public Vector3 targetPosition;
    public Vector3Int cubeCoordPos;
    public Rigidbody rigidbody;
    public GameObject gameObject;
    public bool alwaysVisible;

    public List<HexTile> currentPath = new List<HexTile>();

    public void HandleMovement()
    {
        if (currentTile == null)
        {
            return;
        }
        if (currentPath == null || currentPath.Count <= 1)
        {
            nextTile = null;

            if (currentPath != null && currentPath.Count > 0)
            {
                currentTile = currentPath[0];
                nextTile = currentTile;
            }

            gotPath = false;
        }
        else
        {
            currentTile = currentPath[0];

            nextTile = currentPath[0];

            // if the next tile is not traversable, stop moving;
            /*if (PlayerPosition.NextTile.tileType != HexTileGenerationSettings.TileType.Standard)
            {
                PlayerPosition.currentPath.Clear();
                HandleMovement();
                return;
            }*/

            targetPosition = nextTile.transform.position + new Vector3(0, 1f, 0);
            MoveTo(targetPosition);
            gotPath = true;
            currentPath.RemoveAt(0);
            cubeCoordPos = nextTile.cubeCoordinate;
        }
    }

    public void HandleVisibility()
    {
        // Prob this should be a FSM
        if (nextTile != null && !alwaysVisible)
        {
            if (nextTile.gameObject.layer == 0 || (currentTile != null && currentTile.gameObject.layer == 0))
            {
                gameObject.layer = 8;
            }
            else
            {
                gameObject.layer = 7;
            }
        }
        else if (currentTile != null && !alwaysVisible)
        {
            if (currentTile.gameObject.layer == 0)
            {
                gameObject.layer = 8;
            }
            else
            {
                gameObject.layer = 7;
            }
        }
        else if (alwaysVisible)
        {
            gameObject.layer = 8;
        }
    }

    public abstract void LaunchOnDisable();
    public abstract void LaunchOnStart();
    public abstract void LaunchOnUpdate();

    public void MoveTo(Vector3 targetPos)
    {
        Quaternion angle = Quaternion.Euler(0, Vector3.Angle(targetPos, rigidbody.transform.forward), 0);
        rigidbody.Move(targetPos, angle);
    }

    public void SetAlwaysVisible(bool alwaysVisible)
    {
        this.alwaysVisible = alwaysVisible;
    }

    public void SetGameObject(GameObject gameObject)
    {
        this.gameObject = gameObject;
        this.rigidbody = gameObject.GetComponent<Rigidbody>();
    }
    
    public void SetCurrentTile(HexTile current)
    {
        this.currentTile = current;
    }

    public HexTile GetCurrentTile()
    {
        return this.currentTile;
    }

}