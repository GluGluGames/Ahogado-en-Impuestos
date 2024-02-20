using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using GGG.Components.Buildings;
using GGG.Components.Buildings.Generator;
using GGG.Components.Scenes;
using UnityEngine;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class GeneratorSerialization : MonoBehaviour
    {
        private static string _filePath;
        
        private void Awake()
        { 
            _filePath = Path.Combine(Application.persistentDataPath, "generators_boost");
        }

        private void Start()
        {
            SceneManagement.Instance.OnGameSceneLoaded += Initialize;
            SceneManagement.Instance.OnGameSceneUnloaded += Deinitialize;
        }

        private void OnDisable()
        {
            Deinitialize();
        }

        private void Deinitialize()
        {
            FindObjectsOfType<GeneratorBoostButton>(true).ToList().ForEach(
                x => x.OnBoost -= SaveGeneratorState);
        }

        private void Initialize()
        {
            FindObjectsOfType<GeneratorBoostButton>(true).ToList().ForEach(
                x => x.OnBoost += SaveGeneratorState);
        }

        [Serializable]
        private class GeneratorData
        {
            public int GeneratorId;
            public List<BoostIndexes> BoostIndex;
            public List<BoostBuildings> BoostedBuildings;
        }
        
        [Serializable]
        private class BoostIndexes {
            public int Level;
            public int Index;

            public BoostIndexes(int level, int index)
            {
                Level = level;
                Index = index;
            }
        }
        
        [Serializable]
        private class BoostBuildings
        {
            public int Level;
            public int Index;
            public int BuildingId;

            public BoostBuildings(int level, int idx, int buildingId)
            {
                Level = level;
                Index = idx;
                BuildingId = buildingId;
            }
        }

        public void SaveGeneratorState()
        {
            List<Generator> generators = FindObjectsOfType<Generator>().ToList();
            if (generators.Count <= 0) return;

            GeneratorData[] saveData = new GeneratorData[generators.Count];
            int i = 0;
            string filePath = Path.Combine(Application.persistentDataPath, "generators_boost.json");

            foreach (Generator generator in generators)
            {
                int level = generator.CurrentLevel();

                GeneratorData data = new()
                {
                    GeneratorId = generator.Id(),
                    BoostIndex = new List<BoostIndexes>(),
                    BoostedBuildings = new List<BoostBuildings>(),
                };
                
                for (int y = 1; y <= level; y++) { 
                    for (int j = 0; j < generator.Indexes().GetLength(1); j++) {
                        if (generator.Index(y, j) == 1)
                            data.BoostIndex.Add(new BoostIndexes(y, j));

                        if (generator.BoostBuilding(y, j) != -1)
                            data.BoostedBuildings.Add(new BoostBuildings(y, j, generator.BoostBuilding(y, j)));
                    }

                }
                
                saveData[i] = data;
                i++;
            }

            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(saveData));
            File.WriteAllText(filePath, jsonData);
        }
        
        public IEnumerator LoadGeneratorState()
        {
            List<Generator> generators = FindObjectsOfType<Generator>().ToList();
            if (generators.Count <= 0) yield break;
            
            string filePath = Path.Combine(Application.persistentDataPath, "generators_boost.json");
            string data;

            if (!File.Exists(filePath)) yield break;

            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = SerializationManager.EncryptDecrypt(File.ReadAllText(filePath));
            

            GeneratorData[] generatorData = JsonHelper.FromJson<GeneratorData>(data);

            foreach (GeneratorData generator in generatorData)
            {
                foreach (Generator gen in generators)
                {
                    if (gen.Id() != generator.GeneratorId) continue;

                    for (int i = 0; i < generators.Count; i++)
                    {
                        gen.SetIndex(generator.BoostIndex[i].Level, generator.BoostIndex[i].Index, 1);
                        gen.SetBoostBuilding(generator.BoostedBuildings[i].Level, generator.BoostedBuildings[i].Index, generator.BoostedBuildings[i].BuildingId);
                        gen.AddGeneration(generator.BoostIndex[i].Level, 1);

                        BuildingComponent build = BuildingManager.Instance.GetBuildings()
                            .Find(x => x.Id() == generator.BoostedBuildings[i].BuildingId);

                        // _particles.Add(build, Instantiate(ParticlesPrefab, build.Position(), Quaternion.identity, build.transform));
                        build.Boost();
                        
                    }
                }
            }
        }
    }
}
