using GGG.Shared;
using GGG.Components.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Project.Component.UI.Containers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings
{
    public class FarmUI : MonoBehaviour
    {
        [Header("GameObjects")]
        [SerializeField] private GameObject[] Panels;
        [Space(5),Header("Images")]
        [SerializeField] private Image SelectedImage;
        [Space(5), Header("Texts")]
        [SerializeField] private TMP_Text ResourceName;
        [SerializeField] private TMP_Text GenerationAmount;
        [Space(5), Header("Buttons")]
        [SerializeField] private Button CloseButton;
        
        private GameManager _gameManager;
        
        private readonly Dictionary<int, List<Resource>> _resources = new();
        private readonly Dictionary<Farm, GameObject> _models = new();
        private readonly List<Button> _buttons = new();
        private readonly List<Image> _resourcesImages;
        private Farm _currentFarm;
        private GameObject _viewport;

        private Button _buttonActive;
        private bool _open;

        public static Action OnFarmUIOpen;
        
        private void Awake()
        {
            _resources.Add(0, Resources.LoadAll<Resource>("SeaResources").ToList());
            _resources.Add(1, Resources.LoadAll<Resource>("FishResources").ToList());
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }
        
        private void Start()
        {
            Initialize();
        }

        private void OnDisable()
        {
            CloseButton.onClick.RemoveAllListeners();
        }

        private void Initialize()
        {
            _gameManager = GameManager.Instance;
            
            ResourceName.SetText("");
            GenerationAmount.SetText("");
            
            List<Button[]> auxButtons = new ();
            foreach (GameObject panel in Panels) auxButtons.Add(panel.GetComponentsInChildren<Button>());
            
            foreach (Button[] buttons in auxButtons)
            {
                foreach (Button button in buttons)
                {
                    _buttons.Add(button);
                    HandleImageTransparency(button, false);
                }
            }
            
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void InitializeButtons(int resource, Resource currentResource)
        {
            int a = _buttons.Count / 2 - 1;
            
            for (int i = 0; i < _buttons.Count; i++)
            {
                int idx = i % 2 == 0 ? i / 2 : i + a--;
                
                if(_resources[resource].Count <= i) _buttons[idx].transform.parent.gameObject.SetActive(false);
                else
                {
                    Tooltip tooltip = _buttons[idx].transform.parent.GetComponent<Tooltip>();
                    SpriteState aux = new()
                    {
                        selectedSprite = _resources[resource][i].GetSelectedSprite(),
                        highlightedSprite = _resources[resource][i].GetSelectedSprite(),
                        disabledSprite = _resources[resource][i].GetSprite()
                    };

                    _buttons[idx].spriteState = aux;
                    _buttons[idx].image.sprite = _resources[resource][i].GetSprite();
                    _buttons[idx].interactable = _resources[resource][i].Unlocked();
                    _buttons[idx].image.color = _resources[resource][i].Unlocked() ? Color.white : Color.black;

                    int button = idx;
                    int res = i;
                    _buttons[idx].onClick.AddListener(() => SelectResource(button, _resources[resource][res]));
                    tooltip.SetResourceName(_resources[resource][i].GetName());
                    HandleImageTransparency(_buttons[idx], false);

                    if (_resources[resource][i] != currentResource) continue;
                    
                    _buttons[idx].image.sprite = currentResource.GetSelectedSprite();
                    HandleImageTransparency(_buttons[idx], true);
                    _buttonActive = _buttons[idx];
                }
            }
        }

        private void HandleImageTransparency(Button button, bool visible)
        {
            Image auxImage = button.gameObject.transform.parent.GetComponent<Image>();;
            Color aux = auxImage.color;

            aux.a = visible ? 1 : 0;
            auxImage.color = aux;
        }

        public void RemoveModel(Farm farm)
        {
            _models.Remove(farm);
        }
        
        private void SelectResource(int index, Resource resource)
        {
            if (_buttons[index] == _buttonActive) return;

            if (_buttonActive)
            {
                _buttonActive.image.sprite = _buttonActive.spriteState.disabledSprite;
                HandleImageTransparency(_buttonActive, false);
                GameObject aux = _models[_currentFarm];
                _models.Remove(_currentFarm);
                Destroy(aux);
            }
            
            _buttons[index].image.sprite = resource.GetSelectedSprite();
            HandleImageTransparency(_buttons[index], true);
            _buttonActive = _buttons[index];

            SelectedImage.sprite = resource.GetSprite();
            SelectedImage.color = Color.white;
            
            ResourceName.SetText(resource.GetName());
            GenerationAmount.SetText($"{1/_currentFarm.GetGeneration():0.00}/s");
            
            if (_currentFarm.GetResource() == resource) return;
            _currentFarm.Resource(resource);
            _models.Add(_currentFarm, Instantiate(resource.GetModel(), _currentFarm.transform.position + new Vector3(0, 2.5f), Quaternion.identity,
                _currentFarm.transform));
            _models[_currentFarm].transform.localScale = resource.GetModelScale();
            _currentFarm.SetResourceModel(_models[_currentFarm]);
            BuildingManager.Instance.SaveBuildings();
        }
        
        public void Open(FarmTypes type, Resource currentResource, Farm farm)
        {
            if(_open) return;

            _viewport.SetActive(true);

            _currentFarm = farm;
            if(currentResource && !_models.ContainsKey(farm)) _models.Add(farm, farm.ResourceMode());
            ResourceName.SetText(currentResource ? currentResource.GetName() : "");
            GenerationAmount.SetText(currentResource ? $"{1/_currentFarm.GetGeneration():0.00}/s" : "");
            SelectedImage.sprite = currentResource ? currentResource.GetSprite() : _resources[0][0].GetSprite();
            SelectedImage.color = currentResource ? Color.white : new Color(1f, 1f, 1f, 0f);
            
            InitializeButtons((int) type, currentResource);
            OnFarmUIOpen?.Invoke();
            
            _gameManager.OnUIOpen();
            _open = true;
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f, true).SetEase(Ease.OutCubic).onComplete += () => { 
                _viewport.SetActive(false);
                _open = false;
                
                ResourceName.SetText("");
                GenerationAmount.SetText("");
                
                _gameManager.OnUIClose();
            };

            _buttonActive = null;
            _currentFarm = null;
            _buttons.ForEach(x => x.onClick.RemoveAllListeners());
        }
    }
}
