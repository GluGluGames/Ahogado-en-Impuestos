using GGG.Components.HexagonalGrid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Enemies
{
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
        public bool movingAllowed = true;
        public int enemyLayer = 0;
        public Action onMove = ()=> { };
        public GameObject model1;
        public GameObject model2;

        public List<HexTile> currentPath = new List<HexTile>();

        public void HandleMovement()
        {
            if (movingAllowed)
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

                    nextTile = currentPath[1];

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
        }

        public void HandleVisibility()
        {
            if (model1 == null || model2 == null) return;
            // This should be better done
            if (nextTile != null && !alwaysVisible)
            {
                if (nextTile.gameObject.layer == 0 || (currentTile != null && currentTile.gameObject.layer == 0))
                {
                    gameObject.layer = enemyLayer;
                    model1.layer = enemyLayer;
                    model2.layer = enemyLayer;
                }
                else
                {
                    gameObject.layer = 7;
                    model1.layer = 7;
                    model2.layer = 7;
                }
            }
            else if (currentTile != null && !alwaysVisible)
            {
                if (currentTile.gameObject.layer == 0)
                {
                    gameObject.layer = enemyLayer;
                    model1.layer = enemyLayer;
                    model2.layer = enemyLayer;
                }
                else
                {
                    gameObject.layer = 7;
                    model1.layer = 7;
                    model2.layer = 7;
                }
            }
            else if (alwaysVisible)
            {
                gameObject.layer = enemyLayer;
                model1.layer = enemyLayer;
                model2.layer = enemyLayer;
            }
        }

        public abstract void LaunchOnDisable();

        public abstract void LaunchOnStart();

        public abstract void LaunchOnUpdate();

        public void MoveTo(Vector3 targetPos)
        {
            //Quaternion angle = Quaternion.Euler(0, Vector3.Angle(targetPos, rigidbody.transform.forward), 0);
            gameObject.transform.LookAt(new Vector3(targetPos.x, targetPos.y + 0.5f, targetPos.z));
            //rigidbody.Move(targetPos, angle);
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPos, 1f);
            onMove.Invoke();
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
}