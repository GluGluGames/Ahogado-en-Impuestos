using GGG.Components.HexagonalGrid;
using GGG.Components.Ticks;
using System.Diagnostics;

namespace GGG.Components.Enemies
{
    public class ChasingMovement : BasicMovement
    {
        public HexTile targetTile;

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
            if (!gotPath || targetTile == null || targetTile.cubeCoordinate != PlayerPosition.CurrentTile.cubeCoordinate)
            {
                targetTile = PlayerPosition.CurrentTile;
                GoToTargetTile();
            }
        }

        public override void LaunchOnDisable()
        {
            movingAllowed = false;
            TickManager.OnTick -= HandleMovement;
            TickManager.OnTick -= HandleVisibility;
            currentPath = null;
        }

        private void GoToTargetTile()
        {
            targetPosition = targetTile.transform.position;
            currentPath = Pathfinder.FindPath(currentTile, targetTile);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
        }
    }
}