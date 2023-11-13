using GGG.Components.Buildings;
using GGG.Components.Ticks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace GGG.Components.Enemies
{
    public class RandomMovement : BasicMovement
    {
        public HexTile targetTile;
        public bool imChasing = false;

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
    }
}