using GGG.Components.Buildings;
using GGG.Components.Ticks;

namespace GGG.Components.Enemies
{
    public class RandomMovement : BasicMovement
    {
        // This has to be called on the start
        public override void LaunchOnStart()
        {
            TickManager.OnTick += HandleMovement;
            TickManager.OnTick += HandleVisibility;
            HandleVisibility();
        }

        // This has to be called on the update
        public override void LaunchOnUpdate()
        {
            if (!gotPath)
            {
                GoToRandomTile();
            }
        }

        public override void LaunchOnDisable()
        {
            TickManager.OnTick -= HandleMovement;
            TickManager.OnTick -= HandleVisibility;
        }

        private void GoToRandomTile()
        {
            HexTile destination = TileManager.instance.GetRandomHex();
            targetPosition = destination.transform.position;
            currentPath = Pathfinder.FindPath(currentTile, destination);
            currentPath.Reverse();
            if (currentPath != null) { gotPath = true; }
        }
    }
}