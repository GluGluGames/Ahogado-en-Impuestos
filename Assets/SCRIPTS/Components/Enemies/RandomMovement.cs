using GGG.Components.HexagonalGrid;
using GGG.Components.Ticks;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGG.Components.Enemies
{
    public class RandomMovement : BasicMovement
    {
        public HexTile targetTile;
        public bool imChasing = false;
        public bool imFleeing = false;
        public bool chasingPlayer = false;
        public Ticker ticker;

        // This has to be called on the start
        public override void LaunchOnStart()
        {
            movingAllowed = true;
            ticker.onTick += HandleMovement;
            ticker.onTick += HandleVisibility;
            HandleVisibility();
        }

        // This has to be called on the update
        public override void LaunchOnUpdate()
        {
            if (imChasing && (targetTile != PlayerPosition.CurrentTile || targetTile == null))
            {
                targetTile = PlayerPosition.CurrentTile;
                AddToTargetTile();
            }
            else if (!gotPath)
            {
                GoToRandomTile();
            }
        }

        public void LaunchOnUpdateBerserker()
        {
            if (imChasing)
            {
                if (currentPath.Count == 0 || targetTile != currentPath.Last())
                {
                    AddToTargetTile();
                }
            }
        }

        public void LaunchOnUpdateJunkDealer()
        {
            if (imChasing)
            {
                if (currentPath.Count == 0 || targetTile != currentPath.Last())
                {
                    if (chasingPlayer)
                    {
                        AddToTargetTile();
                    }
                    else
                    {
                        currentPath.Clear();
                        AddToTargetTile();
                    }
                }
            }
        }

        public override void LaunchOnDisable()
        {
            movingAllowed = false;
            ticker.onTick -= HandleMovement;
            ticker.onTick -= HandleVisibility;
            currentPath = null;
        }

        private void GoToRandomTile()
        {
            HexTile destination = TileManager.Instance.GetRandomHex();
            targetPosition = destination.transform.position;
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
        }

        private void AddToTargetTile()
        {
            targetPosition = targetTile.transform.position;
            List<HexTile> addedPath;
            if (currentPath.Count > 0)
            {
                addedPath = Pathfinder.FindPath(currentPath.Last(), targetTile);
            }
            else
            {
                addedPath = Pathfinder.FindPath(currentTile, targetTile);
            }

            addedPath.Reverse();

            if (addedPath.Count != 0) { addedPath.RemoveAt(0); } // remove the first tile since it is already on the path

            foreach (HexTile tile in addedPath)
            {
                currentPath.Add(tile);
            }

            if (currentPath != null) { gotPath = true; }
        }

        public HexTile Flee(int maxDepth)
        {
            HexTile destination = ChooseNeighbourTileAway(maxDepth, 0, currentTile);
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
            return destination;
        }

        public HexTile ChooseNeighbourTileAway(int maxDepth, int curDepth, HexTile tile)
        {
            int aux = Random.Range(0, tile.neighbours.Count);
            HexTile auxTile = tile.neighbours[aux];

            if (Vector3.Distance(auxTile.transform.position, currentTile.transform.position) <= Vector3.Distance(tile.transform.position, currentTile.transform.position))
            {
                auxTile = ChooseNeighbourTileAway(maxDepth, curDepth, tile);
            }
            else
            {
                curDepth++;
                if (curDepth < maxDepth)
                {
                    auxTile = ChooseNeighbourTileAway(maxDepth, curDepth, auxTile);
                }
            }
            return auxTile;
        }
    }
}