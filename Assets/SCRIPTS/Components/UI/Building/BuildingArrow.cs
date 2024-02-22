using System;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    public class BuildingArrow : MonoBehaviour
    {
        private enum Direction
        {
            Right,
            Left
        }

        [SerializeField] private Direction ArrowDirection;
        
        public Action<int> OnArrowPress;

        public void OnArrow()
        {
            OnArrowPress?.Invoke(ArrowDirection == Direction.Left ? -1 : 1);
        }
    }
}
