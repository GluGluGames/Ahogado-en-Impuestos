using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance;

        [Serializable]
        private class BuildingData
        {
            public Vector3 Position;
            public int Level;

            public BuildingData(Vector3 pos)
            {
                Position = pos;
            }
        }

        private void Awake()
        {
            if (Instance != null) return;

            Instance = this;
        }

        

        public void AddBuilding(BuildingComponent build)
        {
            
        }

        private void SaveBuildingData(BuildingData data)
        {
            
        }
    }
}
