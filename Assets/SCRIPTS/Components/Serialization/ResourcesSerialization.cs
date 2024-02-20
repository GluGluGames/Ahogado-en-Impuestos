using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Components.Player;
using GGG.Shared;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class ResourcesSerialization : MonoBehaviour
    {
        [Serializable]
        private class ResourceData
        {
            public string Name;
            public int Count;
        }
        
        public void SaveResourcesCount()
        {
            Dictionary<string, int> resources = PlayerManager.Instance.ResourcesCount();
            ResourceData[] resourceDataList = new ResourceData[resources.Count];
            string filePath = Path.Combine(Application.persistentDataPath, "resources_data.json");
            int i = 0;
            
            foreach (var pair in resources)
            {
                ResourceData data = new()
                {
                    Name = pair.Key,
                    Count = pair.Value
                };
                
                resourceDataList[i] = data;
                i++;
            }
            
            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(resourceDataList));
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadResourcesCount()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "resources_data.json");
            string data;

            if (!File.Exists(filePath))
            {
                if (PlayerManager.Instance.ResourcesCount().Count <= 0) 
                    PlayerManager.Instance.HandleResourceDictionary();
                
                yield break;
            }

            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = File.ReadAllText(filePath);

            ResourceData[] resources = JsonHelper.FromJson<ResourceData>(data);
            PlayerManager.Instance.SetResourcesCount(resources.ToDictionary(item => item.Name, item => item.Count));

            if (PlayerManager.Instance.ResourcesCount().Count <= 0)
                PlayerManager.Instance.HandleResourceDictionary();
        }
    }
}
