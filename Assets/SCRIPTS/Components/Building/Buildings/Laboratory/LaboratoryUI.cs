using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GGG.Components.Laboratory
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
        [SerializeField] private Button CloseButton;
        
        public static Action OnLaboratoryOpen;
        
        private Resource[] _seaResources;
        private Resource[] _expeditionResources;
        private Resource[] _fishResources;
        private Building[] _buildings;

        private bool[] _barActive;

        private Button[] _seaButtons;
        private Button[] _expeditionButtons;
        private Button[] _fishButtons;
        private Button[] _buildingsButton;
        
        private GameObject _viewport;
        private int _selected;
        private bool _open;

        private Action _onResourceFinish;

        private void Awake()
        {
            _seaResources = Resources.LoadAll<Resource>("SeaResources");
            _expeditionResources = Resources.LoadAll<Resource>("ExpeditionResources");
            _fishResources = Resources.LoadAll<Resource>("FishResources");
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
            
            CloseButton.onClick.AddListener(Close);
        }

        private void Start()
        {
            if (_seaResources.Length > 0)
                FillResources(ref _seaButtons, _seaResources, 0);
            

            if (_expeditionResources.Length > 0)
                FillResources(ref _expeditionButtons, _expeditionResources, 1);
 

            if (_fishResources.Length > 0)
                FillResources(ref _fishButtons, _fishResources, 2);
            

            if (_buildings.Length > 0)
                FillBuildings();
            
            for (int i = 0; i < ProgressBars.Length; i++)
            {
                // TODO - Check if the player have an active research
                ProgressBars[i].fillAmount = 0;
                CurrentResources[i].enabled = false;
            }

            _barActive = new bool[ProgressBars.Length];
        }

        private void OpenResourceSelection(int idx)
        {
            if (_barActive[idx]) return;
            
            BarsContainer.SetActive(false);
            ResourcesContainer.SetActive(true);
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
                
                StartCoroutine(FillProgressBar(i, resource.GetResearchTime(), resource));
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
                
                StartCoroutine(FillProgressBar(i, building.GetResearchTime(), null, building));
                return;
            }
        }

        private IEnumerator FillProgressBar(int idx, float totalTime, Resource resource = null, Building building = null)
        {
            float deltaTime = totalTime;
            _barActive[idx] = true;
            

            while (deltaTime >= 0f)
            {
                ProgressBars[idx].fillAmount += Time.deltaTime / totalTime;
                
                int minutes = Mathf.FloorToInt(deltaTime / 60);
                int seconds = Mathf.FloorToInt(deltaTime % 60);
                Counters[idx].SetText($"{minutes:00}:{seconds:00}");
                
                deltaTime -= Time.deltaTime;
                yield return null;
            }
            
            if (resource) resource.DiscoverResource();
            if (building) building.Unlock();
            
            ProgressBars[idx].fillAmount = 0f;
            Counters[idx].SetText("--:--");
            _barActive[idx] = false;
            CurrentResources[idx].enabled = false;
        }

        private void FillBuildings()
        {
            _buildingsButton = Containers[3].GetComponentsInChildren<Button>();
            for (int i = 0; i < _buildingsButton.Length; i++)
            {
                if (_buildings.Length <= i)
                {
                    _buildingsButton[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _buildingsButton[i].image.sprite = _buildings[i].GetIcon();
                    SpriteState aux = new();
                    aux.disabledSprite = _buildings[i].GetDisabledIcon();
                    aux.highlightedSprite = _buildings[i].GetSelectedIcon();
                    _buildingsButton[i].spriteState = aux;
                    _buildingsButton[i].interactable = !_buildings[i].IsUnlocked();
                    foreach (Button button in _buildingsButton)
                    {
                        Building building = _buildings[i];
                        button.onClick.AddListener(() => OnBuildingSelected(building));
                    }
                }
            }
        }
        
        private void FillResources(ref Button[] buttons, Resource[] resources, int idx)
        {
            buttons = Containers[idx].GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                if (resources.Length <= i)
                {
                    buttons[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    buttons[i].image.sprite = resources[i].GetSprite();
                    SpriteState aux = new();
                    aux.disabledSprite = resources[i].GetDisabledSprite();
                    aux.highlightedSprite = resources[i].GetSelectedSprite();
                    buttons[i].spriteState = aux;
                    buttons[i].interactable = resources[i].CanResearch();

                    foreach (Button button in buttons)
                    {
                        Resource resource = resources[i];
                        button.onClick.AddListener(() => OnResourceSelected(resource));
                    }
                }
            }
        }

        public void Open()
        {
            if (_open) return;

            _open = true;
            _viewport.SetActive(true);
            GameManager.Instance.OnUIOpen();
            OnLaboratoryOpen?.Invoke();

            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            if (!_open) return;

            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
                OpenBarContainer();
            };

            _selected = 0;
            GameManager.Instance.OnUIClose();
        }
    }
}
