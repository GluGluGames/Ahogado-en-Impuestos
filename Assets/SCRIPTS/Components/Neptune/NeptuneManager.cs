using GGG.Components.Buildings;
using GGG.Components.Player;
using PlasticGui.WebApi.Responses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Neptune
{
    public class NeptuneManager : MonoBehaviour
    {
        #region singleton
        private NeptuneManager Instance;
        
        #endregion

        private int secondsPassed = 0;
        [SerializeField] private int firstEvent = 5;
        [SerializeField] private int request = 10;
        private Coroutine TickCoroutine;
        private Action OnEvent;

        private void Start()
        {
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
        }
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

        private void destroyRandomStructure()
        {
            BuildingComponent[] buildings = BuildingManager.Instance.GetComponentsInChildren<BuildingComponent>();
            int rand = Random.Range(0, buildings.Length);
            BuildingComponent building = buildings[rand];
            building.GetCurrentTile().DestroyBuilding();
            Destroy(building);
        }
    }
}
