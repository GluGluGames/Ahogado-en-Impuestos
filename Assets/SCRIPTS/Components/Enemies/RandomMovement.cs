using GGG.Components.Buildings;
using GGG.Components.Ticks;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Enemies
{
    public class RandomMovement : BasicMovement
    {
        public HexTile targetTile;
        public bool imChasing = false;
        public bool imFleeing= false;

        // This has to be called on the start
        public override void LaunchOnStart()
        {
            movingAllowed = true;
            TickManager.OnTick += HandleMovement;
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        // This has to be called on the update
        public override void LaunchOnUpdate()
        {
            if(imChasing && (targetTile != PlayerPosition.CurrentTile || targetTile == null))
            {
                targetTile = PlayerPosition.CurrentTile;
                AddToTargetTile();
            }
            else if (!gotPath)
            {
                GoToRandomTile();
            }
        }

        public override void LaunchOnDisable()
        {
            movingAllowed = false;
            TickManager.OnTick -= HandleMovement;
            TickManager.OnTick -= HandleVisibility;
            currentPath = null;
        }

        private void GoToRandomTile()
        {
            HexTile destination = TileManager.instance.GetRandomHex();
            targetPosition = destination.transform.position;
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
        }

        private void AddToTargetTile()
        {
            targetPosition = targetTile.transform.position;
            List<HexTile> addedPath;
            if (currentPath.Count  > 0)
            {
                addedPath = Pathfinder.FindPath(currentPath.Last(), targetTile);
            }
            else
            {
                addedPath = Pathfinder.FindPath(currentTile, targetTile);
            }
            
            addedPath.Reverse();

            foreach (HexTile tile in addedPath)
            {
                currentPath.Add(tile);
            }

            if (currentPath != null) { gotPath = true; }
        }

        public HexTile Flee(int maxDepth)
        {
            HexTile destination = chooseNeighbourTileAway(maxDepth, 0, currentTile);
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
            return destination;
        }

        private HexTile chooseNeighbourTileAway(int maxDepth, int curDepth, HexTile tile)
        {
            int aux = Random.Range(0, tile.neighbours.Count);
            HexTile auxTile = tile.neighbours[aux];

            if(Vector3.Distance(auxTile.transform.position, currentTile.transform.position) <= Vector3.Distance(tile.transform.position, currentTile.transform.position))
            {
                auxTile = chooseNeighbourTileAway(maxDepth, curDepth, tile);
            }
            else
            {
                curDepth++;
                if(curDepth < maxDepth)
                {
                    auxTile = chooseNeighbourTileAway(maxDepth, curDepth, auxTile);
                }
            }
            return auxTile;
        }
    }
}