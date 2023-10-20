using UnityEngine;
using GGG.Shared;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class MuseumUI : MonoBehaviour
{
    #region Public variables

    [Space(5)]
    [Header("Containers")]
    [SerializeField] private GameObject SeaResourcesContainer;
    [SerializeField] private GameObject ExpeditionResourcesContainer;
    [SerializeField] private GameObject FishResourcesContainer;

    [Space(5)]
    [Header("Information")]
    [SerializeField] private TextMeshProUGUI Name;
    [SerializeField] private TextMeshProUGUI Description;

    [Space(5)]
    [Header("Buttons")]
    [SerializeField] private Button SeaButton;
    [SerializeField] private Button ExpeditionButton;
    [SerializeField] private Button FishButton;

    [Space(5)]
    [Header("Backgrounds")]
    [SerializeField] private GameObject SeaBackground;
    [SerializeField] private GameObject ExpeditionBackground;
    [SerializeField] private GameObject FishBackground;

    [SerializeField] private float ButtonScale = 1;
    [SerializeField] private LocalizedString LockedString;

    #endregion

    #region Private variables

    private Resource[] SeaResources;
    private Resource[] ExpeditionResources;
    private Resource[] FishResources;

    private Button[] SeaButtons;
    private Button[] ExpeditionButtons;
    private Button[] FishButtons;

    private bool[] SeaActive;
    private bool[] ExpeditionActive;
    private bool[] FishActive;

    private int _active = 0;
    private int[] _activeResource = { 0, 0, 0 };


    #endregion

    #region Unity functions

    void Awake()
    {
        SeaResources = Resources.LoadAll<Resource>("SeaResources");
        ExpeditionResources = Resources.LoadAll<Resource>("ExpeditionResources");
        FishResources = Resources.LoadAll<Resource>("FishResources");
    }

    private void Start()
    {
        //_lockedString = new LocalizedString(LocalizationKey, EntryReference);

        if (SeaResources.Length > 0) { FillSeaResources(); }
        if(ExpeditionResources.Length > 0) { FillExpeditionResources(); }
        if(FishResources.Length > 0) { FillFishResources(); }

        SeaActive = new bool[SeaResources.Length];
        ExpeditionActive = new bool[ExpeditionResources.Length];
        FishActive = new bool[FishResources.Length];

        InitArray(SeaActive, SeaButtons);
        InitArray(ExpeditionActive, ExpeditionButtons);
        InitArray(FishActive, FishButtons);

        for(int i=0; i<SeaResources.Length; i++)
        {
            UnlockResource(SeaActive, SeaButtons, i);
        }
        UnlockResource(ExpeditionActive, ExpeditionButtons, 0);
        UnlockResource(FishActive, FishButtons, 0);

        SeaButton.onClick.AddListener(() => HandleSeaToggle());
        ExpeditionButton.onClick.AddListener(() => HandleExpeditionToggle());
        FishButton.onClick.AddListener(() => HandleFishToggle());

        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
    }

    #endregion

    #region Methods

    private void FillSeaResources()
    {
        SeaButtons = SeaResourcesContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < SeaButtons.Length; i++)
        {
            if (SeaResources.Length <= i)
            {
                SeaButtons[i].gameObject.SetActive(false);
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
                SeaButtons[i].onClick.AddListener(() => AddListener(SeaResources, SeaButtons, SeaActive, 0, index));
            }
        }
    }
    private void FillExpeditionResources()
    {
        ExpeditionButtons = ExpeditionResourcesContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < ExpeditionButtons.Length; i++)
        {
            if (ExpeditionResources.Length <= i)
            {
                ExpeditionButtons[i].gameObject.SetActive(false);
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
                ExpeditionButtons[i].onClick.AddListener(() => AddListener(ExpeditionResources, ExpeditionButtons, ExpeditionActive, 1, index));
            }
        }
    }

    private void FillFishResources()
    {
        FishButtons = FishResourcesContainer.GetComponentsInChildren<Button>();
        for (int i = 0; i < FishButtons.Length; i++)
        {
            if (FishResources.Length <= i)
            {
                FishButtons[i].gameObject.SetActive(false);
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
                FishButtons[i].onClick.AddListener(() => AddListener(FishResources, FishButtons, FishActive, 2, index));
            }
        }
    }

    private void AddListener(Resource[] resources, Button[] buttons, bool[] isActive, int type, int i)
    {
        SelectResource(resources, buttons, isActive, type, i);
    }

    public void HandleSeaToggle() {
        if (_active == 0) return;
        
        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        SeaBackground.SetActive(true);
        FishBackground.SetActive(false);
        ExpeditionBackground.SetActive(false);

        SelectResource(SeaResources, SeaButtons, SeaActive, 0, 0);

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

        SelectResource(ExpeditionResources, ExpeditionButtons, ExpeditionActive, 1, 0);

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


        SelectResource(FishResources, FishButtons, FishActive, 2, 0);

        _active = 2;
    }

    private void InitArray(bool[] array, Button[] buttons)
    {
        for(int i=0; i<array.Length; i++)
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
        if (activeRes == i && activeRes!=0) return;
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

    #endregion
}
