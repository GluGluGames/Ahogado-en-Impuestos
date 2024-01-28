using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorSerialization : MonoBehaviour
    {
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
        private class GeneratorData
        {
            public int GeneratorId;
            public List<BoostIndexes> Indexes;
            public List<BoostBuildings> Buildings;
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
            /*
            if (_generators.Count <= 0) return;
            
            GeneratorData[] saveData = new GeneratorData[_generators.Count];
            int i = 0;
            string filePath = Path.Combine(Application.persistentDataPath, "generators_boost.json");

            foreach (int id in _generators.Keys)
            {
                int level = _generators[id].CurrentLevel();
                
                GeneratorData data = new()
                {
                    GeneratorId = id,
                    Indexes = new List<BoostIndexes>(),
                    Buildings = new List<BoostBuildings>(),
                };

                for (int y = 1; y <= level; y++) {
                    for (int j = 0; j < _generators[id].BoostIndexes().GetLength(1); j++) {
                        if (_generators[id].BoostIndex(y, j) == 1)
                            data.Indexes.Add(new BoostIndexes(y, j));
                        
                        if (_generators[id].BoostBuilding(y, j) != -1)
                            data.Buildings.Add(new BoostBuildings(y, j, _generators[id].BoostBuilding(y, j)));
                    }

                }

                saveData[i] = data;
                i++;
            }
            
            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
            */
        }
        
        private IEnumerator LoadGeneratorState(Generator[] generators)
        {
            /*
            string filePath = Path.Combine(Application.persistentDataPath, "generators_boost.json");
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
            
            GeneratorData[] generatorData = JsonHelper.FromJson<GeneratorData>(data);

            foreach (GeneratorData generator in generatorData)
            {
                foreach (Generator gen in generators)
                {
                    if (gen.Id() != generator.GeneratorId) continue;
                        
                    _generators.Add(generator.GeneratorId, gen);

                    for (int i = 0; i < generator.Buildings.Count; i++)
                    {
                        gen.AddBoostIndex(generator.Indexes[i].Level, generator.Indexes[i].Index, 1);
                        gen.AddBoostingBuilding(generator.Buildings[i].Level, generator.Buildings[i].Index, generator.Buildings[i].BuildingId);
                        gen.AddGeneration(generator.Indexes[i].Level, 1);

                        BuildingComponent build = BuildingManager.Instance.GetBuildings()
                            .Find(x => x.Id() == generator.Buildings[i].BuildingId);

                        _particles.Add(build, Instantiate(ParticlesPrefab, build.Position(), Quaternion.identity, build.transform));
                        build.Boost();
                    }
                }
            }
            */
            yield break;
        }
    }
}
