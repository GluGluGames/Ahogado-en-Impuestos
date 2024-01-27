using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratorySerialization : MonoBehaviour
    {
        [Serializable]
        private static class LaboratoryData
        {
            public static int LaboratoryId;
            public static string[] Resource = new string[3];
            public static string[] Building = new string[3];
            public static float[] RemainingTime = new float[3];
        }

        public static void SaveResearchProgress()
        {
            /*
            if (_laboratories.Count <= 0) return;
            
            LaboratoryData[] saveData = new LaboratoryData[_laboratories.Count];
            string filePath = Path.Combine(Application.persistentDataPath, "laboratory_progress.json");
            int i = 0;

            foreach (Laboratory lab in _laboratories.Values)
            {
                LaboratoryData data = new() { LaboratoryId = lab.Id() };

                for (int j = 0; j < 3; j++)
                {
                    if (!lab.IsBarActive(j)) continue;
                    
                    if (lab.ActiveResource(j))
                        data.Resource[j] = lab.ActiveResource(j).GetKey();

                    if (lab.ActiveBuilding(j))
                        data.Building[j] = lab.ActiveBuilding(j).GetKey();
                    
                    data.RemainingTime[j] = lab.DeltaTime(j);
                }
                
                saveData[i++] = data;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
            */
        }

        public static IEnumerator LoadResearchProgress(List<Laboratory> laboratories)
        {
            yield break;
            
            /*
            string filePath = Path.Combine(Application.persistentDataPath, "laboratory_progress.json");
            string data;
            
            if (!File.Exists(filePath)) yield break;
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }
            
            LaboratoryData[] researchProgress = JsonHelper.FromJson<LaboratoryData>(data);
            TimeSpan time = DateTime.Now.Subtract(DateTime.Parse(PlayerPrefs.GetString("ExitTime")));
            foreach (LaboratoryData laboratoryData in researchProgress)
            {
                foreach (Laboratory lab in laboratories)
                {
                    if(lab.Id() != laboratoryData.LaboratoryId) continue;
                        
                    _laboratories.Add(lab.Id(), lab);

                    for (int i = 0; i < ProgressBarsFills.Length; i++)
                    {
                        if (laboratoryData.RemainingTime[i] <= 0.0f) continue;
                            
                        lab.SetDeltaTime(i, laboratoryData.RemainingTime[i] - time.Seconds);
                        lab.ActiveBar(i, true);
                        
                        if (!string.IsNullOrEmpty(laboratoryData.Resource[i]))
                        {
                            lab.SetActiveResource(i, PlayerManager.Instance.GetResource(laboratoryData.Resource[i]));
                            StartCoroutine(Research(lab.Id(), i));
                        } else if (!string.IsNullOrEmpty(laboratoryData.Building[i]))
                        {
                            lab.SetActiveBuild(i, Array.Find(_buildings, x => x.GetKey() == laboratoryData.Building[i]));
                            StartCoroutine(Research(lab.Id(), i));
                        }
                    }
                }
            }*/
            
        }
    }
}
