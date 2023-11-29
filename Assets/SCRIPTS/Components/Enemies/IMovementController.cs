using GGG.Components.HexagonalGrid;

using UnityEngine;

namespace GGG.Components.Enemies
{
    public interface IMovementController
    {
        // { } = Default implementation a.k.a Optional

        public abstract void LaunchOnStart();

        public abstract void LaunchOnDisable();

        public abstract void LaunchOnUpdate();

        /// <summary>
        /// Will make the rb move from currentPos to b
        /// </summary>
        /// <param name="targetPos"></param>
        void MoveTo(Vector3 targetPos);

        /// <summary>
        /// Makes the player move each second. DANGER -> Allways running! DONT DESTROY WHILE EXECUTING OR ASSIGNED TO TICKMANAGER ACTION
        /// </summary>
        public void HandleMovement();

        /// <summary>
        /// Handles enemy visibility
        /// </summary>
        public void HandleVisibility();

        public abstract void SetGameObject(GameObject gameObject);

        public abstract void SetAlwaysVisible(bool alwaysVisible);

        public abstract void SetCurrentTile(HexTile current);

        public abstract HexTile GetCurrentTile();
    }
}