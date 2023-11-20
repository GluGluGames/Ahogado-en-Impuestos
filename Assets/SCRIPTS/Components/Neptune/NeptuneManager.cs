using GGG.Components.Buildings;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Neptune
{
    public class NeptuneManager : MonoBehaviour
    {
        #region singleton
        public static NeptuneManager Instance;

        private void Awake() {
            if (Instance) return;

            Instance = this;
        }

        #endregion

        private List<BuildingComponent> _buildings;

        private void Start() {
            _buildings = BuildingManager.Instance.GetBuildings();
            /*
            Instance = this;
            OnEvent += () =>
            {
                if (!checkAlgaes(request))
                {
                    destroyRandomStructure();
                    PlayerManager.Instance.AddResource("Seaweed", Math.Abs(PlayerManager.Instance.GetResourceCount("Seaweed")));
                }
                else
                {
                    PlayerManager.Instance.AddResource("Seaweed", -request);
                }
            };
            TickCoroutine = StartCoroutine(Tick());
            */
        }
        
        /*
        
        private IEnumerator Tick()
        {
            if(secondsPassed == firstEvent)
            {
                OnEvent.Invoke();
            }
            Debug.Log(PlayerManager.Instance.GetResourceCount("Seaweed"));
            yield return new WaitForSeconds(1f);
            secondsPassed++;
            StartCoroutine(Tick());
        }
        

        private void OnDisable()
        {
            StopCoroutine(TickCoroutine);
        }

        private bool checkAlgaes(int amount)
        {
            return PlayerManager.Instance.GetResourceCount("Seaweed") >= request;
        }
        */

        public void DestroyRandomStructure() {
            if (_buildings.Count <= 0) return;
            
            int rand = Random.Range(0, _buildings.Count);
            BuildingComponent building = _buildings[rand];
            // building.GetCurrentTile().DestroyBuilding();
        }
    }
}
