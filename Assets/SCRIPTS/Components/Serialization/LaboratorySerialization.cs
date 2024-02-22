using GGG.Components.Buildings.Laboratory;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Components.Buildings;
using GGG.Components.Player;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class LaboratorySerialization : MonoBehaviour
    {
        [Serializable]
        private class LaboratoryData
        {
            public int LaboratoryId;
            public string[] Resource = new string[3];
            public string[] Building = new string[3];
            public float[] RemainingTime = new float[3];
        }

        public IEnumerator SaveResearchProgress()
        {
            List<Laboratory> laboratories = FindObjectsOfType<Laboratory>().ToList();
            if (laboratories.Count <= 0) yield break;
            
            LaboratoryData[] saveData = new LaboratoryData[laboratories.Count];
            string filePath = Path.Combine(Application.persistentDataPath, "laboratory_progress.json");
            int i = 0;

            foreach (Laboratory lab in laboratories)
            {
                
                LaboratoryData data = new() { LaboratoryId = lab.Id() };

                for (int j = 0; j < 3; j++)
                {
                    if (!lab.IsBarActive(j)) continue;
                    
                    if (lab.ActiveResource(j))
                        data.Resource[j] = lab.ActiveResource(j).GetKey();

                    if (lab.ActiveBuild(j))
                        data.Building[j] = lab.ActiveBuild(j).GetKey();
                    
                    data.RemainingTime[j] = lab.DeltaTime(j);
                }
                
                saveData[i++] = data;
            }

            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(saveData, true));
            File.WriteAllText(filePath, jsonData);
            yield return null;
        }

        public IEnumerator LoadResearchProgress()
        {
            List<Laboratory> laboratories = FindObjectsOfType<Laboratory>().ToList();
            if (laboratories.Count <= 0) yield break;
            
            string filePath = Path.Combine(Application.persistentDataPath, "laboratory_progress.json");
            string data;
            
            if (!File.Exists(filePath)) yield break;
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = SerializationManager.EncryptDecrypt(File.ReadAllText(filePath));
            
            LaboratoryData[] researchProgress = JsonHelper.FromJson<LaboratoryData>(data);
            TimeSpan time = DateTime.Now.Subtract(DateTime.Parse(PlayerPrefs.GetString("ExitTime")));
            foreach (LaboratoryData laboratoryData in researchProgress)
            {
                foreach (Laboratory lab in laboratories)
                {
                    if(lab.Id() != laboratoryData.LaboratoryId) continue;

                    for (int i = 0; i < 3; i++)
                    {
                        if (laboratoryData.RemainingTime[i] <= 0.0f) continue;
                            
                        lab.SetDeltaTime(i, laboratoryData.RemainingTime[i] - time.Seconds);
                        
                        if (!string.IsNullOrEmpty(laboratoryData.Resource[i]))
                        {
                            lab.SetActiveResource(i, PlayerManager.Instance.GetResource(laboratoryData.Resource[i]));
                        } else if (!string.IsNullOrEmpty(laboratoryData.Building[i]))
                        {
                            lab.SetActiveBuild(i, 
                                BuildingManager.Instance.GetBuildings().Find
                                    (x => x.BuildData().GetKey() == laboratoryData.Building[i]).BuildData());
                        }
                        
                        FindObjectOfType<LaboratoryListener>().Research(i);
                    }
                }
            }
            
        }
    }
}
