using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GGG.Components.Player;
using GGG.Components.UI;
using GGG.Shared;
using UnityEngine;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class HUDSerialization : MonoBehaviour
    {
        [Serializable]
        private class ShownResource
        {
            public string Resource;
            public int Index;
        }
        
        public IEnumerator SaveShownResources()
        {
            List<Resource> shownResource = HUDManager.Instance.ShownResources();
            ShownResource[] resourcesData = new ShownResource[shownResource.Count];
            int i = 0;
            string filePath = Path.Combine(Application.persistentDataPath, "shown_resources.json");

            foreach (Resource resource in shownResource)
            {
                ShownResource resourceData = new()
                {
                    Resource = resource.GetKey(),
                    Index = HUDManager.Instance.GetIndex(resource)
                };

                resourcesData[i] = resourceData;
                i++;
            }

            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(resourcesData));
            File.WriteAllText(filePath, jsonData);
            yield return null;
        }

        public IEnumerator LoadShownResource()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "shown_resources.json");
            string data;

            if (!File.Exists(filePath))
            {
                HUDManager.Instance.SetCurrentIndex(0);
                yield break;
            }

            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = SerializationManager.EncryptDecrypt(File.ReadAllText(filePath));
            

            ShownResource[] resources = JsonHelper.FromJson<ShownResource>(data);

            foreach (ShownResource resource in resources)
            {
                Resource aux = PlayerManager.Instance.GetResource(resource.Resource);
                HUDManager.Instance.AddShownResource(aux);

                HUDManager.Instance.ShowResource(resource.Index);

                HUDManager.Instance.SetResourceIcon(resource.Index, aux.GetSprite());
                HUDManager.Instance.SetResourceText(resource.Index, PlayerManager.Instance.GetResourceCount(resource.Resource).ToString());
                HUDManager.Instance.SetCurrentIndex(resource.Index);
            }

            int current = HUDManager.Instance.ShownResources().Count == 0 ? 
                0 : HUDManager.Instance.GetResourceIcons().FindIndex(x => !x.gameObject.activeInHierarchy);
            HUDManager.Instance.SetCurrentIndex(current);
        }
    }
}
