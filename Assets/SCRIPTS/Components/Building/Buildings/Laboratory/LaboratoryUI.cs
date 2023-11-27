using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryUI : MonoBehaviour
    {
        [Header("Images")] 
        [SerializeField] private Image[] ProgressBars;
        [SerializeField] private Image[] CurrentResources;
        [Space(5), Header("Texts")] 
        [SerializeField] private TMP_Text[] Counters;
        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject BarsContainer;
        [SerializeField] private GameObject ResourcesContainer;
        [SerializeField] private GameObject[] Containers;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button[] ResearchButtons;
        [SerializeField] private Button[] TabButtons;
        [SerializeField] private Button BackButton;
        [SerializeField] private Button CloseButton;
        
        public static Action OnLaboratoryOpen;

        private GameManager _gameManager;

        private readonly Dictionary<int, Resource[]> _resources = new();
        private readonly Dictionary<int, Button[]> _buttons = new();
        private Building[] _buildings;

        private Resource[] _activeResource;
        private Building[] _activeBuilding;
        private bool[] _barActive;
        private bool _wasOpen;
        private float[] _deltaTimes;
        
        private GameObject _viewport;
        private int _selected;
        private bool _open;

        [Serializable]
        private class LaboratoryData
        {
            public float FillAmount;
            public Resource Resource;
            public Building Building;
            public float RemainingTime;
        }

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

            for (int i = 0; i < TabButtons.Length; i++)
            {
                int idx = i;
                TabButtons[i].onClick.AddListener(() => ChangeTab(idx));
            }
            
            BackButton.onClick.AddListener(OpenBarContainer);
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private IEnumerator Start()
        {
            _gameManager = GameManager.Instance;

            for (int i = 0; i < _resources.Count; i++)
            {
                _buttons.Add(i, Containers[i].GetComponentsInChildren<Button>());
                FillResources(_buttons[i], _resources[i]);
            }
            
            _buttons.Add(3, Containers[3].GetComponentsInChildren<Button>());
            FillBuildings();
            
            _barActive = new bool[ProgressBars.Length];
            _deltaTimes = new float[ProgressBars.Length];
            _activeResource = new Resource[ProgressBars.Length];
            _activeBuilding = new Building[ProgressBars.Length];

            yield return LoadResearchProgress();
        }

        private void OpenResourceSelection(int idx)
        {
            if (_barActive[idx]) return;
            
            BarsContainer.SetActive(false);
            ResourcesContainer.SetActive(true);
            CheckResources();
        }

        private void ChangeTab(int idx)
        {
            if (_selected == idx) return;
            
            for (int i = 0; i < Containers.Length; i++)
            {
                Containers[i].SetActive(idx == i);
            }

            _selected = idx;
        }

        private void OpenBarContainer()
        {
            BarsContainer.SetActive(true);
            ResourcesContainer.SetActive(false);
        }

        private void OnResourceSelected(Resource resource)
        {
            OpenBarContainer();
            
            for (int i = 0; i < _barActive.Length; i++)
            {
                if (_barActive[i]) continue;

                CurrentResources[i].enabled = true;
                CurrentResources[i].sprite = resource.GetSprite();
                _activeResource[i] = resource;
                _deltaTimes[i] = resource.GetResearchTime();
                _barActive[i] = true;
                
                StartCoroutine(FillProgressBar(i, resource.GetResearchTime()));
                return;
            }
            
        }

        private void OnBuildingSelected(Building building)
        {
            OpenBarContainer();
            
            for (int i = 0; i < _barActive.Length; i++)
            {
                if (_barActive[i]) continue;

                CurrentResources[i].enabled = true;
                CurrentResources[i].sprite = building.GetIcon();
                _activeBuilding[i] = building;
                _deltaTimes[i] = building.GetResearchTime();
                _barActive[i] = true;
                
                StartCoroutine(FillProgressBar(i, building.GetResearchTime()));
                return;
            }
        }

        private IEnumerator FillProgressBar(int idx, float totalTime)
        {
            while (_deltaTimes[idx] >= 0f)
            {
                ProgressBars[idx].fillAmount += Time.deltaTime / totalTime;
                
                int minutes = Mathf.FloorToInt(_deltaTimes[idx] / 60);
                int seconds = Mathf.FloorToInt(_deltaTimes[idx] % 60);
                Counters[idx].SetText($"{minutes:00}:{seconds:00}");
                
                _deltaTimes[idx] -= Time.deltaTime;
                yield return null;
            }

            if (_activeResource[idx])
            {
                _activeResource[idx].Unlock();
                _activeResource[idx] = null;
            }

            if (_activeBuilding[idx])
            {
                _activeBuilding[idx].Unlock();
                _activeBuilding[idx] = null;
            }
            
            ProgressBars[idx].fillAmount = 0f;
            Counters[idx].SetText("--:--");
            _barActive[idx] = false;
            _deltaTimes[idx] = 0f;
            CurrentResources[idx].enabled = false;
        }

        private void FillBuildings()
        {
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
                    SpriteState aux = new();
                    aux.disabledSprite = _buildings[i].GetResearchIcon();
                    aux.highlightedSprite = _buildings[i].GetSelectedIcon();
                    _buttons[3][i].spriteState = aux;
                    _buttons[3][i].interactable = !_buildings[i].IsUnlocked();
                    
                    Building building = _buildings[i];
                    _buttons[3][i].onClick.AddListener(() => OnBuildingSelected(building));
                }
            }
        }
        
        private void FillResources(Button[] buttons, Resource[] resources)
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
                    SpriteState aux = new();
                    aux.disabledSprite = resources[i].GetSprite();
                    aux.highlightedSprite = resources[i].GetSelectedSprite();
                    buttons[i].spriteState = aux;
                    buttons[i].interactable = resources[i].CanResearch();
                    
                    Resource resource = resources[i];
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
            if (_activeResource.Any(r => r == resource))
                button.transform.parent.gameObject.SetActive(false);
        }

        private void CheckButton(Button button, Building building)
        {
            button.interactable = !building.IsUnlocked();
            button.image.color = !building.IsUnlocked() ? Color.white : Color.black;
            button.transform.parent.gameObject.SetActive(!building.IsUnlocked());
            if (_activeBuilding.Any(b => b == building))
                button.transform.parent.gameObject.SetActive(false);
        }

        private void SaveResearchProgress()
        {
            LaboratoryData[] saveData = new LaboratoryData[ProgressBars.Length];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "laboratory_progress.json");

            for (int i = 0; i < ProgressBars.Length; i++)
            {
                LaboratoryData data = new()
                {
                    FillAmount = ProgressBars[i].fillAmount,
                    Resource = _activeResource[i],
                    Building = _activeBuilding[i],
                    RemainingTime =  _deltaTimes[i]
                };

                saveData[i] = data;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadResearchProgress()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "laboratory_progress.json");
#if UNITY_EDITOR
            filePath = "file://" + filePath;
#endif
            string data;
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }

            if (!string.IsNullOrEmpty(data))
            {
                LaboratoryData[] researchProgress = JsonHelper.FromJson<LaboratoryData>(data);
                TimeSpan time = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString("ExitTime"));
                for (int i = 0; i < ProgressBars.Length; i++)
                {
                    if (researchProgress[i].FillAmount <= 0.0f)
                    {
                        CurrentResources[i].enabled = false;
                        continue;
                    }
                    
                    _deltaTimes[i] = researchProgress[i].RemainingTime - time.Seconds;
                    ProgressBars[i].fillAmount = researchProgress[i].FillAmount + time.Seconds / _deltaTimes[i];
                    
                    CurrentResources[i].enabled = researchProgress[i].FillAmount > 0.0f;
                    _barActive[i] = researchProgress[i].FillAmount > 0.0f;
                    
                    if (researchProgress[i].Resource)
                    {
                        _activeResource[i] = researchProgress[i].Resource;
                        CurrentResources[i].sprite = researchProgress[i].Resource.GetSprite();
                        StartCoroutine(FillProgressBar(i, researchProgress[i].Resource.GetResearchTime()));
                    }

                    if (researchProgress[i].Building)
                    {
                        _activeBuilding[i] = researchProgress[i].Building;
                        CurrentResources[i].sprite = researchProgress[i].Building.GetIcon();
                        StartCoroutine(FillProgressBar(i, researchProgress[i].Building.GetResearchTime()));
                    }
                }
            }
            else
            {
                for (int i = 0; i < ProgressBars.Length; i++)
                    CurrentResources[i].enabled = false;
            }
        }

        private void OnDisable()
        {
            if (!SceneManagement.InGameScene() || _gameManager.TutorialOpen() || !_wasOpen) return;
            
            SaveResearchProgress();
        }

        public void Open()
        {
            if (_open) return;

            _open = true;
            _wasOpen = true;
            _viewport.SetActive(true);
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
                _gameManager.OnUIClose();
            };

            _selected = 0;
        }
    }
}
