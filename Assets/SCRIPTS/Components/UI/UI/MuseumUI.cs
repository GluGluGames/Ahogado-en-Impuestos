using UnityEngine;
using GGG.Shared;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;

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
        if(SeaResources.Length > 0) { FillSeaResources(); }
        if(ExpeditionResources.Length > 0) { FillExpeditionResources(); }
        if(FishResources.Length > 0) { FillFishResources(); }

        SeaActive = new bool[SeaResources.Length];
        ExpeditionActive = new bool[ExpeditionResources.Length];
        FishActive = new bool[FishResources.Length];

        initArray(SeaActive, SeaButtons);
        initArray(ExpeditionActive, ExpeditionButtons);
        initArray(FishActive, FishButtons);

        unlockResource(SeaActive, SeaButtons, 0);
        unlockResource(ExpeditionActive, ExpeditionButtons, 0);
        unlockResource(FishActive, FishButtons, 0);

        SeaButton.onClick.AddListener(() => HandleSeaToggle());
        ExpeditionButton.onClick.AddListener(() => HandleExpeditionToggle());
        FishButton.onClick.AddListener(() => HandleFishToggle());
        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
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
                SeaButtons[i].image.sprite = SeaResources[i].GetSprite();
                int index = i;
                SeaButtons[i].onClick.AddListener(() => AddListener(SeaResources, SeaButtons, 0, index));
   
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
                ExpeditionButtons[i].image.sprite = ExpeditionResources[i].GetSprite();
                int index = i;
                ExpeditionButtons[i].onClick.AddListener(() => AddListener(ExpeditionResources, ExpeditionButtons, 1, index));
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
                FishButtons[i].image.sprite = FishResources[i].GetSprite();
                int index = i;
                FishButtons[i].onClick.AddListener(() => AddListener(FishResources, FishButtons, 2, index));
            }
        }
    }

    private void AddListener(BasicResource[] resources, Button[] buttons, int type, int i)
    {
        SelectResource(resources, buttons, type, i);
    }

    public void HandleSeaToggle() {
        if (active == 0) return;
        
        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        SeaBackground.active = true;
        FishBackground.active = false;
        ExpeditionBackground.active = false;

        SelectResource(SeaResources, SeaButtons, 0, 0);

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

        SelectResource(ExpeditionResources, ExpeditionButtons, 1, 0);

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


        SelectResource(FishResources, FishButtons, 2, 0);

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

    private void SelectResource(BasicResource[] resources, Button[] buttons, int type, int i)
    {
        int activeRes = activeResource[type];
        if (activeRes == i && activeRes!=0) return;
        buttons[activeRes].image.sprite = buttons[i].spriteState.disabledSprite;

        Name.text = resources[i].GetName().GetLocalizedString();
        Description.text = resources[i].GetDescription().GetLocalizedString();
        buttons[i].image.sprite = buttons[i].spriteState.selectedSprite;
        activeResource[type] = i;
    }

    #endregion
}
