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
        GameObject father;
        [SerializeField] private List<GameObject> visibilityListy;


        private StateIcon currentState;

        private void Start()
        {
            father = transform.parent.gameObject;
        }

        private void Update()
        {
            if (father.layer == 7)
            {
                transform.gameObject.layer = 7;
                foreach (GameObject go in visibilityListy)
                {
                    go.layer = 7;
                }
            }
            else
            {
                transform.gameObject.layer = 8;
                foreach (GameObject go in visibilityListy)
                {
                    go.layer = 8;
                }
            }
            
        }

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