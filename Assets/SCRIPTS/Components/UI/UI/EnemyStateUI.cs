using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum StateIcon
{
    PatrolState,
    ChasingState,
    BerserkerState,
    FleeState,
    SleepState,
    RestState
}

namespace GGG.Components.UI
{
    public class EnemyStateUI : MonoBehaviour
    {
        [SerializeField] private List<Image> ImageList = new List<Image>();

        private StateIcon currentState;

        public StateIcon GetCurrentState()
        {
            return currentState;
        }

        public Image GetCurrentImageState()
        {
            return ImageList[(int)currentState];
        }

        public bool ChangeState(StateIcon newState)
        {
            ImageList[(int)currentState].enabled = false;
            currentState = newState;
            ImageList[(int)currentState].enabled = true;

            return true;
        }
    }
}