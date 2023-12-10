using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using GGG.Components.Achievements;
using GGG.Components.UI.Buttons;
using Project.Component.UI.Containers;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryUI : MonoBehaviour
    {
        [FormerlySerializedAs("ProgressBars")]
        [Header("Images")] 
        [SerializeField] private Image[] ProgressBarsFills;
        [SerializeField] private Image[] ProgressBarsBackground;
        [SerializeField] private Image[] CurrentResources;
        [Space(5), Header("Texts")] 
        [SerializeField] private TMP_Text[] Counters;
        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject BarsContainer;
        [SerializeField] private GameObject ResourcesContainer;
        [SerializeField] private GameObject[] Containers;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button[] ResearchButtons;
        [SerializeField] private Button BackButton;
        [SerializeField] private Button CloseButton;
        
        public static Action OnLaboratoryOpen;

        private GameManager _gameManager;
        private Laboratory _currentLaboratory;

        private readonly Dictionary<int, Laboratory> _laboratories = new();
        private readonly Dictionary<int, Resource[]> _resources = new();
        private readonly Dictionary<int, Button[]> _buttons = new();
        private List<ContainerButton> _containerButtons;
        private Building[] _buildings;
        
        private GameObject _viewport;
        private int _selected;
        private int _currentBar;
        private int _researchDone;
        private bool _open;

        private Action _onResourceFinish;

        private void Awake()
        {
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources"));
            _resources.Add(1, Resources.LoadAll<Resource>("ExpeditionResources"));
            _resources.Add(2, Resources.LoadAll<Resource>("FishResources"));
            _buildings = Resources.LoadAll<Building>("Buildings");

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);


            for(int i = 0; i < ResearchButtons.Length; i++)
            {
                int idx = i;
                ResearchButtons[i].onClick.AddListener(() => OpenResourceSelection(idx));
            }

            _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();

            for (int i = 0; i < _containerButtons.Count; i++)
            {
                int idx = i;
                for (int j = 0; j < _containerButtons.Count - 1; j++)
                {
                    _containerButtons[i].OnButtonClick +=
                        _containerButtons[(idx + 1) % _containerButtons.Count].DeselectButton;
                    idx++;
                }
            }
            
            BackButton.onClick.AddListener(OpenBarContainer);
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;

            for (int i = 0; i < _resources.Count; i++)
            {
                _buttons.Add(i, Containers[i].GetComponentsInChildren<Button>());
                FillResources(_buttons[i], _resources[i], Containers[i].GetComponentsInChildren<Tooltip>(true));
            }
            
            _buttons.Add(3, Containers[3].GetComponentsInChildren<Button>());
            
            FillBuildings();

            BuildingManager.OnBuildsLoad += OnBuildsLoad;
        }

        private void Update()
        {
            if (!_open) return;

            FillBars(_currentLaboratory);
        }

        private void OnDisable()
        {
            BackButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
            BuildingManager.OnBuildsLoad -= OnBuildsLoad;
            SaveResearchProgress();
        }
        
        private void ResetContainers()
        {
            for (int i = 0; i < _containerButtons.Count; i++)
            {
                _containerButtons[i].Initialize();
                Containers[i].SetActive(i == 0);
            }
        }

        private void InitializeLaboratories(Laboratory laboratory)
        {
            _currentLaboratory = laboratory;
            if (!_laboratories.ContainsKey(laboratory.Id()))
            {
                _laboratories.Add(laboratory.Id(), laboratory);
            }

            for (int i = 0; i < ProgressBarsBackground.Length; i++)
            {
                ProgressBarsBackground[i].color = i + 1 <= _currentLaboratory.CurrentLevel()
                    ? Color.white
                    : new Color(0.47f, 0.47f, 0.47f);

                ResearchButtons[i].interactable = i + 1 <= _currentLaboratory.CurrentLevel();
            }

            FillBars(laboratory);
        }

        private void FillBars(Laboratory laboratory)
        {
            for (int i = 0; i < ProgressBarsFills.Length; i++)
            {
                CurrentResources[i].enabled = laboratory.IsBarActive(i);
                
                if(!laboratory.IsBarActive(i)) continue;

                float totalTime = 0;

                if (laboratory.ActiveResource(i))
                {
                    CurrentResources[i].sprite = laboratory.ActiveResource(i).GetSprite();
                    totalTime = laboratory.ActiveResource(i).GetResearchTime();
                }

                if (laboratory.ActiveBuilding(i))
                {
                    CurrentResources[i].sprite = laboratory.ActiveBuilding(i).GetIcon();
                    totalTime = laboratory.ActiveBuilding(i).GetResearchTime();
                }

                int minutes = Mathf.FloorToInt(laboratory.DeltaTime(i) / 60);
                int seconds = Mathf.FloorToInt(laboratory.DeltaTime(i) % 60);
                Counters[i].SetText($"{minutes:00}:{seconds:00}");
                ProgressBarsFills[i].fillAmount = Mathf.Clamp01(1 - laboratory.DeltaTime(i) / totalTime);
            }
        }

        private void OnBuildsLoad(BuildingComponent[] builds)
        {
            if (builds == null) return;

            List<Laboratory> laboratories = new();

            foreach (BuildingComponent build in builds)
            {
                if(build.GetType() != typeof(Laboratory)) continue;
                
                laboratories.Add((Laboratory) build);
            }

            StartCoroutine(LoadResearchProgress(laboratories));
        }

        private void OpenResourceSelection(int idx)
        {
            if (_currentLaboratory.IsBarActive(idx)) return;
            
            BarsContainer.SetActive(false);
            ResourcesContainer.SetActive(true);
            _currentBar = idx;
            CheckResources();
        }

        private void OpenBarContainer()
        {
            BarsContainer.SetActive(true);
            ResourcesContainer.SetActive(false);
        }

        private void OnResourceSelected(Resource resource)
        {
            OpenBarContainer();
            
            CurrentResources[_currentBar].enabled = true;
            CurrentResources[_currentBar].sprite = resource.GetSprite();
            _currentLaboratory.SetActiveResource(_currentBar, resource);
            _currentLaboratory.SetDeltaTime(_currentBar, resource.GetResearchTime());
            _currentLaboratory.ActiveBar(_currentBar, true);
                
            StartCoroutine(Research(_currentLaboratory.Id(), _currentBar));
            
        }

        private void OnBuildingSelected(Building building)
        {
            OpenBarContainer();
            
            CurrentResources[_currentBar].enabled = true;
            CurrentResources[_currentBar].sprite = building.GetIcon();
            _currentLaboratory.SetActiveBuild(_currentBar, building);
            _currentLaboratory.SetDeltaTime(_currentBar, building.GetResearchTime());
            _currentLaboratory.ActiveBar(_currentBar, true);
                
            StartCoroutine(Research(_currentLaboratory.Id(), _currentBar));
            return;
        }

        private IEnumerator Research(int id, int idx)
        {
            while (_laboratories[id].DeltaTime(idx) >= 0f)
            {
                _laboratories[id].AddDeltaTime(idx, -Time.deltaTime);
                yield return null;
            }

            if (_laboratories[id].ActiveResource(idx))
            {
                _laboratories[id].ActiveResource(idx).Unlock();
                _laboratories[id].SetActiveResource(idx, null);
            }

            if (_laboratories[id].ActiveBuilding(idx))
            {
                _laboratories[id].ActiveBuilding(idx).Unlock();
                _laboratories[id].SetActiveBuild(idx, null);
            }
            
            _laboratories[id].ActiveBar(idx, false);
            _laboratories[id].SetDeltaTime(idx, 0f);
            _researchDone++;

            if (_researchDone >= 6)
                StartCoroutine(AchievementsManager.Instance.UnlockAchievement("05"));
            
            if (!_open) yield break;
            
            Counters[idx].SetText("--:--");
            ProgressBarsFills[idx].fillAmount = 0f;
            CurrentResources[idx].enabled = false;
        }

        private void FillBuildings()
        {
            Tooltip[] tooltips = Containers[3].GetComponentsInChildren<Tooltip>();
            for (int i = 0; i < _buttons[3].Length; i++)
            {
                if (_buildings.Length <= i)
                {
                    _buttons[3][i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    if (_buildings[i].IsUnlocked())
                    {
                        _buttons[3][i].transform.parent.gameObject.SetActive(false);
                        continue;
                    }
                    
                    _buttons[3][i].image.sprite = _buildings[i].GetResearchIcon();
                    SpriteState aux = new()
                    {
                        disabledSprite = _buildings[i].GetResearchIcon(),
                        highlightedSprite = _buildings[i].GetSelectedIcon()
                    };
                    _buttons[3][i].spriteState = aux;
                    _buttons[3][i].interactable = !_buildings[i].IsUnlocked();
                    
                    Building building = _buildings[i];
                    _buttons[3][i].onClick.AddListener(() => OnBuildingSelected(building));
                    tooltips[i].SetResourceName(_buildings[i].GetName());
                }
            }
        }
        
        private void FillResources(Button[] buttons, Resource[] resources, Tooltip[] tooltips)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (resources.Length <= i)
                {
                    buttons[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    buttons[i].image.sprite = resources[i].GetSprite();
                    buttons[i].image.color = resources[i].CanResearch() ? Color.white : Color.black;
                    SpriteState aux = new()
                    {
                        disabledSprite = resources[i].GetSprite(),
                        highlightedSprite = resources[i].GetSelectedSprite()
                    };
                    buttons[i].spriteState = aux;
                    buttons[i].interactable = resources[i].CanResearch();
                    
                    Resource resource = resources[i];
                    tooltips[i].SetResourceName(resource.GetName());
                    buttons[i].onClick.AddListener(() => OnResourceSelected(resource));
                }
            }
        }

        private void CheckResources()
        {
            for (int i = 0; i < _resources.Count; i++)
            {
                for (int j = 0; j < _resources[i].Length; j++)
                {
                    if (_buttons[i].Length <= j) break;
                    CheckButton(_buttons[i][j], _resources[i][j]);
                }
            }

            for (int i = 0; i < _buildings.Length; i++)
            {
                if (_buttons[3].Length <= i) break;
                CheckButton(_buttons[3][i], _buildings[i]);
            }
        }

        private void CheckButton(Button button, Resource resource)
        {
            button.interactable = resource.CanResearch();
            button.image.color = resource.CanResearch() ? Color.white : Color.black;
            button.transform.parent.gameObject.SetActive(!resource.Unlocked());
            if (_currentLaboratory.ActiveResources().Any(r => r == resource))
                button.transform.parent.gameObject.SetActive(false);
        }

        private void CheckButton(Button button, Building building)
        {
            button.interactable = !building.IsUnlocked();
            button.image.color = !building.IsUnlocked() ? Color.white : Color.black;
            button.transform.parent.gameObject.SetActive(!building.IsUnlocked());
            if (_currentLaboratory.ActiveBuildings().Any(b => b == building))
                button.transform.parent.gameObject.SetActive(false);
        }
        
        [Serializable]
        private class LaboratoryData
        {
            public int LaboratoryId;
            public Resource[] Resource = new Resource[3];
            public Building[] Building = new Building[3];
            public float[] RemainingTime = new float[3];
        }

        public void SaveResearchProgress()
        {
            if (_laboratories.Count <= 0) return;
            
            LaboratoryData[] saveData = new LaboratoryData[_laboratories.Count];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "laboratory_progress.json");
            int i = 0;

            foreach (Laboratory lab in _laboratories.Values)
            {
                LaboratoryData data = new() { LaboratoryId = lab.Id() };

                for (int j = 0; j < 3; j++)
                {
                    if (!lab.IsBarActive(j)) continue;
                    
                    if (lab.ActiveResource(j))
                        data.Resource[j] = lab.ActiveResource(j);

                    if (lab.ActiveBuilding(j))
                        data.Building[j] = lab.ActiveBuilding(j);
                    
                    data.RemainingTime[j] = lab.DeltaTime(j);
                }
                
                saveData[i++] = data;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadResearchProgress(List<Laboratory> laboratories)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "laboratory_progress.json");
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
                        
                        if (laboratoryData.Resource[i])
                        {
                            lab.SetActiveResource(i, laboratoryData.Resource[i]);
                            StartCoroutine(Research(lab.Id(), i));
                        } else if (laboratoryData.Building[i])
                        {
                            lab.SetActiveBuild(i, laboratoryData.Building[i]);
                            StartCoroutine(Research(lab.Id(), i));
                        }
                    }
                }
            }
        }

        public void Open(Laboratory laboratory)
        {
            if (_open) return;

            _open = true;
            _viewport.SetActive(true);
            ResetContainers();
            InitializeLaboratories(laboratory);
            _gameManager.OnUIOpen();
            OnLaboratoryOpen?.Invoke();

            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
                OpenBarContainer();

                for (int i = 0; i < ProgressBarsFills.Length; i++)
                {
                    ProgressBarsFills[i].fillAmount = 0f;
                    Counters[i].SetText("--:--");
                    CurrentResources[i].enabled = false;
                }
                
                _gameManager.OnUIClose();
            };

            _selected = 0;
            
        }
    }
}
