using System;
using GGG.Components.Core;
using UnityEngine;
using GGG.Shared;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace GGG.Components.Museum
{
    public class MuseumUI : MonoBehaviour
    {
        #region Public variables

        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject SeaResourcesContainer;
        [SerializeField] private GameObject ExpeditionResourcesContainer;
        [SerializeField] private GameObject FishResourcesContainer;

        [Space(5), Header("Information")] 
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI Description;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button SeaButton;
        [SerializeField] private Button ExpeditionButton;
        [SerializeField] private Button FishButton;
        [SerializeField] private Button CloseButton;

        [Space(5), Header("Backgrounds")] 
        [SerializeField] private GameObject SeaBackground;
        [SerializeField] private GameObject ExpeditionBackground;
        [SerializeField] private GameObject FishBackground;
        
        [SerializeField] private LocalizedString LockedString;

        public static Action OnMuseumOpen;

        #endregion

        #region Private variables
        
        private const int _INITIAL_POSITION = -1100;

        private Resource[] _seaResources;
        private Resource[] _expeditionResources;
        private Resource[] _fishResources;

        private Button[] _seaButtons;
        private Button[] _expeditionButtons;
        private Button[] _fishButtons;

        private GameObject _viewport;

        private bool[] _seaActive;
        private bool[] _expeditionActive;
        private bool[] _fishActive;
        private bool _open;

        private int _active = 0;
        private int[] _activeResource = { 0, 0, 0 };


        #endregion

        #region Unity functions

        void Awake()
        {
            _seaResources = Resources.LoadAll<Resource>("SeaResources");
            _expeditionResources = Resources.LoadAll<Resource>("ExpeditionResources");
            _fishResources = Resources.LoadAll<Resource>("FishResources");

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f + _INITIAL_POSITION);
        }

        private void Start()
        {
            //_lockedString = new LocalizedString(LocalizationKey, EntryReference);

            if (_seaResources.Length > 0)
            {
                FillSeaResources();
            }

            if (_expeditionResources.Length > 0)
            {
                FillExpeditionResources();
            }

            if (_fishResources.Length > 0)
            {
                FillFishResources();
            }

            _seaActive = new bool[_seaResources.Length];
            _expeditionActive = new bool[_expeditionResources.Length];
            _fishActive = new bool[_fishResources.Length];

            InitArray(_seaActive, _seaButtons);
            InitArray(_expeditionActive, _expeditionButtons);
            InitArray(_fishActive, _fishButtons);

            CheckResources();

            SeaButton.onClick.AddListener(HandleSeaToggle);
            ExpeditionButton.onClick.AddListener(HandleExpeditionToggle);
            FishButton.onClick.AddListener(HandleFishToggle);
            CloseButton.onClick.AddListener(Close);

            SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            
            SelectResource(_seaResources, _seaButtons, _seaActive, 0, 0);
        }

        #endregion

        #region Methods

        private void FillSeaResources()
        {
            _seaButtons = SeaResourcesContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _seaButtons.Length; i++)
            {
                if (_seaResources.Length <= i)
                {
                    _seaButtons[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _seaButtons[i].image.sprite = _seaResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.selectedSprite = _seaResources[i].GetSelectedSprite();
                    aux.highlightedSprite = _seaResources[i].GetSelectedSprite();
                    aux.disabledSprite = _seaResources[i].GetSprite();
                    _seaButtons[i].spriteState = aux;
                    int index = i;
                    _seaButtons[i].onClick
                        .AddListener(() => AddListener(_seaResources, _seaButtons, _seaActive, 0, index));
                }
            }
        }

        private void FillExpeditionResources()
        {
            _expeditionButtons = ExpeditionResourcesContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _expeditionButtons.Length; i++)
            {
                if (_expeditionResources.Length <= i)
                {
                    _expeditionButtons[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _expeditionButtons[i].image.sprite = _expeditionResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.selectedSprite = _expeditionResources[i].GetSelectedSprite();
                    aux.highlightedSprite = _expeditionResources[i].GetSelectedSprite();
                    aux.disabledSprite = _expeditionResources[i].GetSprite();
                    _expeditionButtons[i].spriteState = aux;
                    int index = i;
                    _expeditionButtons[i].onClick.AddListener(() =>
                        AddListener(_expeditionResources, _expeditionButtons, _expeditionActive, 1, index));
                }
            }
        }

        private void FillFishResources()
        {
            _fishButtons = FishResourcesContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _fishButtons.Length; i++)
            {
                if (_fishResources.Length <= i)
                {
                    _fishButtons[i].transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _fishButtons[i].image.sprite = _fishResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.disabledSprite = _fishResources[i].GetSprite();
                    aux.selectedSprite = _fishResources[i].GetSelectedSprite();
                    aux.highlightedSprite = _fishResources[i].GetSelectedSprite();
                    _fishButtons[i].spriteState = aux;
                    int index = i;
                    _fishButtons[i].onClick
                        .AddListener(() => AddListener(_fishResources, _fishButtons, _fishActive, 2, index));
                }
            }
        }

        private void AddListener(Resource[] resources, Button[] buttons, bool[] isActive, int type, int i)
        {
            SelectResource(resources, buttons, isActive, type, i);
        }

        public void HandleSeaToggle()
        {
            if (_active == 0) return;

            SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaBackground.SetActive(true);
            FishBackground.SetActive(false);
            ExpeditionBackground.SetActive(false);

            SelectResource(_seaResources, _seaButtons, _seaActive, 0, 0);

            _active = 0;
        }

        public void HandleExpeditionToggle()
        {
            if (_active == 1) return;

            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.selectedSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaBackground.SetActive(false);
            FishBackground.SetActive(false);
            ExpeditionBackground.SetActive(true);

            SelectResource(_expeditionResources, _expeditionButtons, _expeditionActive, 1, 0);

            _active = 1;
        }

        public void HandleFishToggle()
        {
            if (_active == 2) return;

            FishButton.image.sprite = FishButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;

            SeaBackground.SetActive(false);
            FishBackground.SetActive(true);
            ExpeditionBackground.SetActive(false);


            SelectResource(_fishResources, _fishButtons, _fishActive, 2, 0);

            _active = 2;
        }

        private void InitArray(bool[] array, Button[] buttons)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = false;
                buttons[i].image.color = Color.black;
            }
        }

        private void UnlockResource(bool[] array, Button[] buttons, int i)
        {
            array[i] = true;
            buttons[i].image.color = Color.white;
        }

        private void SelectResource(Resource[] resources, Button[] buttons, bool[] isActive, int type, int i)
        {
            int activeRes = _activeResource[type];
            if (activeRes == i && activeRes != 0) return;
            buttons[activeRes].image.sprite = buttons[activeRes].spriteState.disabledSprite;

            if (isActive[i])
            {
                Name.text = resources[i].GetName();
                Description.text = resources[i].GetDescription();
            }
            else
            {
                Name.text = LockedString.GetLocalizedString();
                Description.text = LockedString.GetLocalizedString();
            }

            buttons[i].image.sprite = buttons[i].spriteState.selectedSprite;
            _activeResource[type] = i;
        }

        private void CheckResources()
        {
            for (int i = 0; i < _seaResources.Length; i++)
                if(_seaResources[i].Unlocked()) UnlockResource(_seaActive, _seaButtons, i);
            
            for (int i = 0; i < _expeditionResources.Length; i++)
                if(_expeditionResources[i].Unlocked()) UnlockResource(_expeditionActive, _expeditionButtons, i);
            
            for (int i = 0; i < _fishResources.Length; i++)
                if(_fishResources[i].Unlocked()) UnlockResource(_fishActive, _fishButtons, i);
        }

        public void Open()
        {
            if (_open) return;
            
            _open = true;
            _viewport.SetActive(true);
            CheckResources();
            GameManager.Instance.OnUIOpen();
            OnMuseumOpen?.Invoke();
            
            transform.DOMoveY(Screen.height * 0.5f, 0.75f).SetEase(Ease.OutBounce);
        }

        private void Close()
        {
            if (!_open) return;

            transform.DOMoveY(Screen.height * 0.5f + _INITIAL_POSITION, 0.75f).SetEase(Ease.OutBounce).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
            };
            
            GameManager.Instance.OnUIClose();
        }

        #endregion
    }
}
