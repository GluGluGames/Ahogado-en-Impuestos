using System.Collections;
using GGG.Shared;
using GGG.Components.Player;

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using GGG.Components.Core;

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
        [SerializeField] private float ButtonScale = 1;

        #endregion

        #region Private variables

        private Resource[] SeaResources;
        private Resource[] ExpeditionResources;
        private Resource[] FishResources;

        private Button[] SeaButtons;
        private Button[] ExpeditionButtons;
        private Button[] FishButtons;

        private Dictionary<string, TextMeshProUGUI> ResourcesCountText;

        private const int _INITIAL_POSITION = -1100;

        private PlayerManager _player;
        private GameObject _viewport;
        private int active = 0;
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
            _viewport = transform.GetChild(0).gameObject;
            transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f + _INITIAL_POSITION);

            ResourcesCountText = new Dictionary<string, TextMeshProUGUI>(_player.GetResourceNumber());
            
            SeaResources = Resources.LoadAll<Resource>("RESOURCES/SeaResources");
            ExpeditionResources = Resources.LoadAll<Resource>("RESOURCES/ExpeditionResources");
            FishResources = Resources.LoadAll<Resource>("RESOURCES/FishResources");

            while (SeaResources.Length <= 0 || ExpeditionResources.Length <= 0 || FishResources.Length <= 0)
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

            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(false);
        }

        private void Update()
        {
            if (!_open) return;

            foreach (string key in ResourcesCountText.Keys)
            {
                ResourcesCountText[key].SetText(_player.GetResourceCount(key).ToString());
            }
        }

        #endregion

        #region Methods


        private void FillSeaResources()
        {
            SeaButtons = SeaContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < SeaButtons.Length; i++)
            {
                if (SeaResources.Length <= i)
                {
                    SeaButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    SeaButtons[i].gameObject.transform.localScale = new Vector3(ButtonScale, ButtonScale, 1);
                    SeaButtons[i].image.sprite = SeaResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.selectedSprite = SeaResources[i].GetSelectedSprite();
                    aux.disabledSprite = SeaResources[i].GetSprite();
                    SeaButtons[i].spriteState = aux;
                    int index = i;
                    SeaButtons[i].onClick.AddListener(() => AddListener(SeaResources, SeaButtons, 0, index));

                    ResourcesCountText[SeaResources[i].GetKey()] =
                        SeaButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void FillExpeditionResources()
        {
            ExpeditionButtons = ExpeditionContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < ExpeditionButtons.Length; i++)
            {
                if (ExpeditionResources.Length <= i)
                {
                    ExpeditionButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    ExpeditionButtons[i].gameObject.transform.localScale = new Vector3(ButtonScale, ButtonScale, 1);
                    ExpeditionButtons[i].image.sprite = ExpeditionResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.selectedSprite = ExpeditionResources[i].GetSelectedSprite();
                    aux.disabledSprite = ExpeditionResources[i].GetSprite();
                    ExpeditionButtons[i].spriteState = aux;
                    int index = i;
                    ExpeditionButtons[i].onClick
                        .AddListener(() => AddListener(ExpeditionResources, ExpeditionButtons, 1, index));

                    ResourcesCountText[ExpeditionResources[i].GetKey()] =
                        ExpeditionButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void FillFishResources()
        {
            FishButtons = FishContainer.GetComponentsInChildren<Button>();
            for (int i = 0; i < FishButtons.Length; i++)
            {
                if (FishResources.Length <= i)
                {
                    FishButtons[i].transform.parent.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    FishButtons[i].gameObject.transform.localScale = new Vector3(ButtonScale, ButtonScale, 1);
                    FishButtons[i].image.sprite = FishResources[i].GetSprite();
                    SpriteState aux = new SpriteState();
                    aux.disabledSprite = FishResources[i].GetSprite();
                    aux.selectedSprite = FishResources[i].GetSelectedSprite();
                    FishButtons[i].spriteState = aux;
                    int index = i;
                    FishButtons[i].onClick.AddListener(() => AddListener(FishResources, FishButtons, 2, index));

                    ResourcesCountText[FishResources[i].GetKey()] =
                        FishButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>();
                }
            }
        }

        private void AddListener(Resource[] resources, Button[] buttons, int type, int i)
        {
            //
        }

        public void HandleSeaToggle()
        {
            if (active == 0) return;

            SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaContainer.SetActive(true);
            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(false);

            active = 0;
        }

        public void HandleExpeditionToggle()
        {
            if (active == 1) return;

            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.selectedSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;
            FishButton.image.sprite = FishButton.spriteState.disabledSprite;
            SeaContainer.SetActive(false);
            ExpeditionContainer.SetActive(true);
            FishContainer.SetActive(false);

            active = 1;
        }

        public void HandleFishToggle()
        {
            if (active == 2) return;

            FishButton.image.sprite = FishButton.spriteState.selectedSprite;
            ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
            SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;

            SeaContainer.SetActive(false);
            ExpeditionContainer.SetActive(false);
            FishContainer.SetActive(true);

            active = 2;
        }

        public void OpenInventory()
        {
            if (_open) return;
            
            _viewport.SetActive(true);
            _open = true;
            GameManager.Instance.OnUIOpen();

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
