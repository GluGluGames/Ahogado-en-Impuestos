using Codice.Client.Common.GameUI;
using GGG.Components.Ticks;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class MovementController : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        public bool gotPath = false;
        private Rigidbody _rigidbody;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            TickManager.OnTick += HandleMovement;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Update line renderer based on current PlayerPosition.CurrentPath
            //UpdateLineRenderer(PlayerPosition.CurrentPath);
        }

        /// <summary>
        /// Update line following the player PlayerPosition.CurrentPath
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
            Debug.Log("Handleo movimiento");    
            if (PlayerPosition.CurrentPath == null || PlayerPosition.CurrentPath.Count <= 1)
            {
                PlayerPosition.NextTile = null;

                if (PlayerPosition.CurrentPath != null && PlayerPosition.CurrentPath.Count > 0)
                {
                    PlayerPosition.CurrentTile = PlayerPosition.CurrentPath[0];
                    PlayerPosition.NextTile = PlayerPosition.CurrentTile;
                }

                gotPath = false;
                UpdateLineRenderer(new List<HexTile>());
            }
            else
            {
                PlayerPosition.CurrentTile = PlayerPosition.CurrentPath[0];

                PlayerPosition.NextTile = PlayerPosition.CurrentPath[1];

                // if the next tile is not traversable, stop moving;
                /*if (PlayerPosition.NextTile.tileType != HexTileGenerationSettings.TileType.Standard)
                {
                    PlayerPosition.CurrentPath.Clear();
                    HandleMovement();
                    return;
                }*/

                PlayerPosition.TargetPosition = PlayerPosition.NextTile.transform.position + new Vector3(0, 1f, 0);
                MoveTo(PlayerPosition.TargetPosition);
                gotPath = true;
                PlayerPosition.CurrentPath.RemoveAt(0);
                PlayerPosition.PlayerPos = PlayerPosition.NextTile.cubeCoordinate;
                UpdateLineRenderer(PlayerPosition.CurrentPath);
            }
        }

        /// <summary>
        /// Moves the player model to the target pos. There is no controll of the target pos.
        /// </summary>
        /// <param name="targetPos"></param>
        private void MoveTo(Vector3 targetPos)
        {
            Quaternion angle = Quaternion.Euler(0, Vector3.Angle(targetPos, transform.forward), 0);
            _rigidbody.Move(targetPos, angle);
        }
    }
}