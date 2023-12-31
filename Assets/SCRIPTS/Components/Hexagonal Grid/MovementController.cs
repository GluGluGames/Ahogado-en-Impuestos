using DG.Tweening;
using GGG.Components.Ticks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

namespace GGG.Components.HexagonalGrid
{
    public class MovementController : MonoBehaviour
    {
        public int visionRange;
        private LineRenderer _lineRenderer;
        public bool gotPath = false;
        private Rigidbody _rigidbody;

        [SerializeField] private Ticker _ticker;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
            _ticker.onTick += HandleMovement;
        }

        private void OnDisable()
        {
            _ticker.onTick -= HandleMovement;
        }

        /// <summary>
        /// Update line following the player PlayerPosition.currentPath
        /// </summary>
        /// <param name="path"></param>
        protected void UpdateLineRenderer(List<HexTile> path)
        {
            if (_lineRenderer == null || path.Count == 0) { }

            List<Vector3> points = new List<Vector3>();
            foreach (HexTile tile in path)
            {
                points.Add(tile.transform.position + new Vector3(0, 0.5f, 0));
            }
            _lineRenderer.positionCount = points.Count;
            _lineRenderer.SetPositions(points.ToArray());
        }

        /// <summary>
        /// Function that updates the player position on relation with his path. If there is no path, then this will not work. It also moves the player rigidbody.
        /// </summary>
        public void HandleMovement()
        {
            try
            {
                if (PlayerPosition.currentPath == null || PlayerPosition.currentPath.Count <= 1)
                {
                    PlayerPosition.NextTile = null;

                    if (PlayerPosition.currentPath != null && PlayerPosition.currentPath.Count > 0)
                    {
                        PlayerPosition.CurrentTile = PlayerPosition.currentPath[0];
                        PlayerPosition.NextTile = PlayerPosition.CurrentTile;
                    }

                    gotPath = false;
                    UpdateLineRenderer(new List<HexTile>());
                }
                else
                {
                    PlayerPosition.CurrentTile = PlayerPosition.currentPath[0];

                    PlayerPosition.NextTile = PlayerPosition.currentPath[1];

                    // if the next tile is not traversable, stop moving;
                    /*if (PlayerPosition.NextTile.tileType != HexTileGenerationSettings.TileType.Standard)
                    {
                        PlayerPosition.currentPath.Clear();
                        HandleMovement();
                        return;
                    }*/

                    PlayerPosition.TargetPosition = PlayerPosition.NextTile.transform.position + new Vector3(0, PlayerPosition.heightOffset, 0);
                    gotPath = true;
                    PlayerPosition.currentPath.RemoveAt(0);
                    PlayerPosition.PlayerPos = PlayerPosition.NextTile.cubeCoordinate;
                    MoveTo(PlayerPosition.TargetPosition);

                    UpdateLineRenderer(PlayerPosition.currentPath);
                }
            }
            catch (System.Exception e)
            {
                Debug.Log("Proccess probably stopped while executing");
                Debug.Log(e);
            }
        }

        /// <summary>
        /// Moves the player model to the target pos. There is no controll of the target pos.
        /// </summary>
        /// <param name="targetPos"></param>
        private void MoveTo(Vector3 targetPos)
        {
            Quaternion angle = Quaternion.Euler(0, 0, 0);
            _rigidbody.transform.LookAt(targetPos);
            //_rigidbody.transform.position = targetPos;
            _rigidbody.transform.DOMove(targetPos, 0.75f);
            TileManager.Instance.RevealTile(PlayerPosition.NextTile, visionRange);
        }
    }
}