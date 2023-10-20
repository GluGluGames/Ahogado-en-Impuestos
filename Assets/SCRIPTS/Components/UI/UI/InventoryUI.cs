using UnityEngine;
using GGG.Shared;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    #region Public variables

    [Space(5)]
    [Header("Containers")]
    [SerializeField] private GameObject SeaContainer;
    [SerializeField] private GameObject ExpeditionContainer;
    [SerializeField] private GameObject FishContainer;

    [Space(5)]
    [Header("Buttons")]
    [SerializeField] private Button SeaButton;
    [SerializeField] private Button ExpeditionButton;
    [SerializeField] private Button FishButton;


    [SerializeField] private float ButtonScale = 1;

    #endregion

    #region Private variables

    private BasicResource[] SeaResources;
    private BasicResource[] ExpeditionResources;
    private BasicResource[] FishResources;

    private Button[] SeaButtons;
    private Button[] ExpeditionButtons;
    private Button[] FishButtons;

    private List<TextMeshProUGUI> SeaText;
    private List<TextMeshProUGUI> ExpeditionText;
    private List<TextMeshProUGUI> FishText;

    private int active = 0;

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
        SeaText = new List<TextMeshProUGUI>(SeaResources.Length);
        ExpeditionText = new List<TextMeshProUGUI>(ExpeditionResources.Length);
        FishText = new List<TextMeshProUGUI>(FishResources.Length);

        if (SeaResources.Length > 0) { FillSeaResources(); }
        if (ExpeditionResources.Length > 0) { FillExpeditionResources(); }
        if (FishResources.Length > 0) { FillFishResources(); }

        SeaButton.onClick.AddListener(() => HandleSeaToggle());
        ExpeditionButton.onClick.AddListener(() => HandleExpeditionToggle());
        FishButton.onClick.AddListener(() => HandleFishToggle());

        SeaButton.image.sprite = SeaButton.spriteState.selectedSprite;
        FishButton.image.sprite = FishButton.spriteState.disabledSprite;
        ExpeditionButton.image.sprite = ExpeditionButton.spriteState.disabledSprite;

        ExpeditionContainer.SetActive(false);
        FishContainer.SetActive(false);
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
                SeaButtons[i].transform.parent.GetComponentInParent<Image>()?.gameObject.SetActive(false);
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

                SeaText.Add(SeaButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>());
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
                ExpeditionButtons[i].transform.parent.GetComponentInParent<Image>()?.gameObject.SetActive(false);
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
                ExpeditionButtons[i].onClick.AddListener(() => AddListener(ExpeditionResources, ExpeditionButtons, 1, index));

                ExpeditionText.Add(ExpeditionButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>());
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
                FishButtons[i].transform.parent.GetComponentInParent<Image>()?.gameObject.SetActive(false);
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

                FishText.Add(FishButtons[i].transform.GetComponentInChildren<TextMeshProUGUI>());
            }
        }
    }

    private void AddListener(BasicResource[] resources, Button[] buttons, int type, int i)
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

    #endregion
}
