using System.Collections;
using GGG.Shared;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Input;

namespace GGG.Components.UI
{
    public class InventoryUI : MonoBehaviour
    {
        #region Public variables

        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject SeaContainer;
        [SerializeField] private GameObject ExpeditionContainer;
        [SerializeField] private GameObject FishContainer;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button CloseButton;
        [SerializeField] private Button SeaButton;
        [SerializeField] private Button ExpeditionButton;
        [SerializeField] private Button FishButton;

        #endregion

        #region Private variables

        private Resource[] _seaResources;
        private Resource[] _expeditionResources;
        private Resource[] _fishResources;

        private Button[] _seaButtons;
        private Button[] _expeditionButtons;
        private Button[] _fishButtons;

        private Dictionary<string, TextMeshProUGUI> _resourcesCountText;

        private const int _INITIAL_POSITION = -1100;

        private PlayerManager _player;
        private InputManager _input;
        private GameObject _viewport;
        private int _active = 0;
        private bool _open;

        #endregion

        #region Unity functions

        void Awake()
        {
            CloseButton.onClick.AddListener(CloseInventory);
        }

        private IEnumerator Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _viewport = transform.GetChild(0).gameObject;
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f + _INITIAL_POSITION);

            _resourcesCountText = new Dictionary<string, TextMeshProUGUI>(_player.GetResourceNumber());
            
            _seaResources = Resources.LoadAll<Resource>("RESOURCES/SeaResources");
            _expeditionResources = Resources.LoadAll<Resource>("RESOURCES/ExpeditionResources");
            _fishResources = Resources.LoadAll<Resource>("RESOURCES/FishResources");

            while (_seaResources.Length <= 0 || _expeditionResources.Length <= 0 || _fishResources.Length <= 0)
                yield return null;
            
            FillSeaResources();
            FillExpeditionResources();
            FillFishResources();

            SeaButton.onClick.AddListener(() => HandleSeaToggle());
            ExpeditionButton.onClick.AddListener(() => HandleExpeditionToggle());
            FishButton.onClick.AddListener(() => HandleFishToggle());

            SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;

            SeaContainer.SetActive(true);
            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(false);
        }

        private void Update()
        {
            if (!_open || !_input.Escape()) return;
            
            CloseInventory();
        }

        #endregion

        #region Methods


        private void FillSeaResources()
        {
            _seaButtons = SeaContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _seaButtons.Length; i++)
            {
                if (_seaResources.Length <= i)
                {
                    _seaButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _seaButtons[i].image.sprite = _seaResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.highlightedSprite = _seaResources[i].GetSelectedSprite();
                    aux.selectedSprite = _seaResources[i].GetSelectedSprite();
                    aux.disabledSprite = _seaResources[i].GetSprite();
                    _seaButtons[i].spriteState = aux;
                    int index = i;
                    _seaButtons[i].onClick.AddListener(() => AddListener(_seaResources, _seaButtons, 0, index));

                    _resourcesCountText[_seaResources[i].GetKey()] =
                        _seaButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>(true);
                }
            }
        }

        private void FillExpeditionResources()
        {
            _expeditionButtons = ExpeditionContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _expeditionButtons.Length; i++)
            {
                if (_expeditionResources.Length <= i)
                {
                    _expeditionButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _expeditionButtons[i].image.sprite = _expeditionResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.highlightedSprite = _expeditionResources[i].GetSelectedSprite();
                    aux.selectedSprite = _expeditionResources[i].GetSelectedSprite();
                    aux.disabledSprite = _expeditionResources[i].GetSprite();
                    _expeditionButtons[i].spriteState = aux;
                    int index = i;
                    _expeditionButtons[i].onClick
                        .AddListener(() => AddListener(_expeditionResources, _expeditionButtons, 1, index));

                    _resourcesCountText[_expeditionResources[i].GetKey()] =
                        _expeditionButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void FillFishResources()
        {
            _fishButtons = FishContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < _fishButtons.Length; i++)
            {
                if (_fishResources.Length <= i)
                {
                    _fishButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    _fishButtons[i].image.sprite = _fishResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.highlightedSprite = _fishResources[i].GetSelectedSprite();
                    aux.disabledSprite = _fishResources[i].GetSprite();
                    aux.selectedSprite = _fishResources[i].GetSelectedSprite();
                    _fishButtons[i].spriteState = aux;
                    int index = i;
                    _fishButtons[i].onClick.AddListener(() => AddListener(_fishResources, _fishButtons, 2, index));

                    _resourcesCountText[_fishResources[i].GetKey()] =
                        _fishButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void AddListener(Resource[] resources, Button[] buttons, int type, int i)
        {
            //
        }

        public void HandleSeaToggle()
        {
            if (_active == 0) return;

            SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaContainer.SetActive(true);
            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(false);

            _active = 0;
        }

        public void HandleExpeditionToggle()
        {
            if (_active == 1) return;

            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.selectedSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaContainer.SetActive(false);
            ExpeditionContainer.SetActive(true);
            FishContainer.SetActive(false);

            _active = 1;
        }

        public void HandleFishToggle()
        {
            if (_active == 2) return;

            FishButton.image.sprite = FishButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;

            SeaContainer.SetActive(false);
            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(true);

            _active = 2;
        }

        public void OpenInventory()
        {
            if (_open) return;

            _viewport.SetActive(true);
            _open = true;
            GameManager.Instance.OnUIOpen();
            
            foreach (string key in _resourcesCountText.Keys)
            {
                _resourcesCountText[key].SetText(_player.GetResourceCount(key).ToString());
            }
            
            transform.DOMoveY(Screen.height * 0.5f, 0.5f).SetEase(Ease.OutBounce);
        }

        public void CloseInventory()
        {
            if (!_open) return;

            transform.DOMoveY(Screen.height * 0.5f + _INITIAL_POSITION, 0.5f).SetEase(Ease.OutBounce).onComplete += () =>
            {
                _viewport.SetActive(false);
                _open = false;
            };
            
            GameManager.Instance.OnUIClose();
        }

        #endregion
    }
}
