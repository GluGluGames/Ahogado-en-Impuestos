using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.Player;
using GGG.Components.Scenes;
using GGG.Shared;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class BuildingSerialization : MonoBehaviour
    {
        [Serializable]
        private class BuildingData
        {
            public int Id;
            public Vector3 Position;
            public string Building;
            public int[] CurrentCost;
            public string[] CurrentResourcesCost;
            public int Level;
            public string FarmResource;
            public bool IsBoost;
        }
        
        public void SaveBuildings()
        {
            if (!SceneManagement.InGameScene()) return;

            List<BuildingComponent> buildings = BuildingManager.Instance.GetBuildings();
            BuildingData[] saveData = new BuildingData[buildings.Count];
            int i = 0;
            string filePath = Path.Combine(Application.persistentDataPath, "buildings_data.json");

            foreach (BuildingComponent build in buildings)
            {
                List<string> aux = new();
                foreach(Resource resource in build.CurrentCost().GetResource())
                    aux.Add(resource.GetKey());
                
                BuildingData data = new()
                {
                    Id = build.Id(),
                    Position = build.Position(),
                    Building = build.BuildData().GetKey(),
                    Level = build.CurrentLevel(),
                    CurrentCost = build.CurrentCost().GetCost(),
                    CurrentResourcesCost = aux.ToArray()
                };
                
                data.IsBoost = build.BuildData().CanBeBoost() && build.IsBoost();

                saveData[i] = data;
                i++;
            }
            
            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(saveData));
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadBuildings()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "buildings_data.json");
            string data;

            if (!File.Exists(filePath)) yield break;
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = SerializationManager.EncryptDecrypt(File.ReadAllText(filePath));
            
            BuildingData[] buildings = JsonHelper.FromJson<BuildingData>(data);
            BuildingComponent[] buildingComponents = new BuildingComponent[buildings.Length];
            List<Building> buildingsSo = Resources.LoadAll<Building>("Buildings").ToList();
            int i = 0;

            foreach (BuildingData buildData in buildings)
            {
                Building build = buildingsSo.Find(x => x.GetKey() == buildData.Building);
                GameObject go = build.Spawn(buildData.Position, transform, buildData.Level, false);
                buildingComponents[i] = go.GetComponent<BuildingComponent>();
                List<Resource> aux = new();
                foreach (string resource in buildData.CurrentResourcesCost)
                    PlayerManager.Instance.GetResource(resource);

                buildingComponents[i].SetId(buildData.Id);
                buildingComponents[i].SetLevel(buildData.Level);
                buildingComponents[i].SetCurrentCost(new ResourceCost(buildData.CurrentCost, aux.ToArray()));

                if (buildingComponents[i].BuildData().CanBeBoost() && buildData.IsBoost)
                    buildingComponents[i].Boost();
                
                BuildingManager.Instance.AddBuilding(buildingComponents[i]);
                i++;
            }

            BuildingManager.Instance.HandleCurrentId();
        }
    }
}
