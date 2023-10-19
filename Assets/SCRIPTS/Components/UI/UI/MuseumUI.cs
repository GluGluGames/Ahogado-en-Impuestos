using UnityEngine;
using GGG.Shared;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

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

    #endregion

    #region Private variables

    private BasicResource[] SeaResources;
    private BasicResource[] ExpeditionResources;
    private BasicResource[] FishResources;

    private Button[] SeaButtons;
    private Button[] ExpeditionButtons;
    private Button[] FishButtons;

    private bool[] SeaActive;
    private bool[] ExpeditionActive;
    private bool[] FishActive;

    private int active = 0;
    private int[] activeResource = new int[] { 0, 0, 0 };

    public LocalizedString _lockedString;

    #endregion

    #region Unity functions

    void Awake()
    {
        SeaResources = Resources.LoadAll<BasicResource>("RESOURCES/SeaResources");
        ExpeditionResources = Resources.LoadAll<BasicResource>("RESOURCES/ExpeditionResources");
        FishResources = Resources.LoadAll<BasicResource>("RESOURCES/FishResources");
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

        initArray(SeaActive, SeaButtons);
        initArray(ExpeditionActive, ExpeditionButtons);
        initArray(FishActive, FishButtons);

        for(int i=0; i<SeaResources.Length; i++)
        {
            unlockResource(SeaActive, SeaButtons, i);
        }
        unlockResource(ExpeditionActive, ExpeditionButtons, 0);
        unlockResource(FishActive, FishButtons, 0);

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

    private void AddListener(BasicResource[] resources, Button[] buttons, bool[] active, int type, int i)
    {
        SelectResource(resources, buttons, active, type, i);
    }

    public void HandleSeaToggle() {
        if (active == 0) return;
        
        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        SeaBackground.active = true;
        FishBackground.active = false;
        ExpeditionBackground.active = false;

        SelectResource(SeaResources, SeaButtons, SeaActive, 0, 0);

        active = 0;
    }

    public void HandleExpeditionToggle()
    {
        if (active == 1) return;
        
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.selectedSprite;
        SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        SeaBackground.active = false;
        FishBackground.active = false;
        ExpeditionBackground.active = true;

        SelectResource(ExpeditionResources, ExpeditionButtons, ExpeditionActive, 1, 0);

        active = 1;
    }

    public void HandleFishToggle()
    {
        if (active == 2) return;
        
        FishButton.image.sprite = FishButton.spriteState.selectedSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
        SeaButton.image.sprite = SeaButton.spriteState.disabledSprite;
        
        SeaBackground.active = false;
        FishBackground.active = true;
        ExpeditionBackground.active = false;


        SelectResource(FishResources, FishButtons, FishActive, 2, 0);

        active = 2;
    }

    private void initArray(bool[] array, Button[] buttons)
    {
        for(int i=0; i<array.Length; i++)
        {
            array[i] = false;
            buttons[i].image.color = Color.black;
        }
    }

    private void unlockResource(bool[] array, Button[] buttons, int i)
    {
        array[i] = true;
        buttons[i].image.color = Color.white;
    }

    private void SelectResource(BasicResource[] resources, Button[] buttons, bool[] active, int type, int i)
    {
        int activeRes = activeResource[type];
        if (activeRes == i && activeRes!=0) return;
        buttons[activeRes].image.sprite = buttons[activeRes].spriteState.disabledSprite;

        if (active[i] == true)
        {
            Name.text = resources[i].GetName().GetLocalizedString();
            Description.text = resources[i].GetDescription().GetLocalizedString();
        }
        else
        {
            Name.text = _lockedString.GetLocalizedString();
            Description.text = _lockedString.GetLocalizedString();
        }
        buttons[i].image.sprite = buttons[i].spriteState.selectedSprite;
        activeResource[type] = i;
    }

    #endregion
}
