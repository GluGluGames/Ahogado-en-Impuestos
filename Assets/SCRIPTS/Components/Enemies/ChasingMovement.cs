using GGG.Components.Buildings;
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
            TickManager.OnTick += () => UnityEngine.Debug.Log(currentPath.ToArray());
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
            TickManager.OnTick -= HandleMovement;
            TickManager.OnTick -= HandleVisibility;
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