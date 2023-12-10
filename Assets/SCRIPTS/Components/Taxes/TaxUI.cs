using GGG.Shared;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;

using TMPro;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Classes.Dialogue;
using GGG.Components.Achievements;
using GGG.Components.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GGG.Components.Taxes
{
    public class TaxUI : MonoBehaviour
    {
        [SerializeField] private GameObject BuildingDestroyParticles;
        [SerializeField] private Sound DestructionSound;
        [Header("UI Fields")]
        [SerializeField] private Image[] ResourcesSprites;
        [SerializeField] private TMP_Text[] ResourcesAmount;
        [SerializeField] private DialogueText PayDialogue;
        [SerializeField] private DialogueText NotPayDialogue;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button PayButton;
        [SerializeField] private Button NotPayButton;

        private PlayerManager _player;
        private DialogueBox _dialogueBox;
        private List<BuildingComponent> _buildings;
        private Resource _taxResource;
        private int _taxAmount;
        private readonly System.Random _random = new();

        private GameObject _viewport;
        private bool _open;

        public Action OnOptionSelected;

        private IEnumerator Start()
        {
            _player = PlayerManager.Instance;
            _dialogueBox = FindObjectOfType<DialogueBox>();
            
            PayButton.onClick.AddListener(PayTaxes);
            NotPayButton.onClick.AddListener(NotPayTaxes);
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.5f);

            yield return null;
            
            GenerateTaxesAmount();
        }

        private void OnDisable()
        {
            PayButton.onClick.RemoveAllListeners();
            NotPayButton.onClick.RemoveAllListeners();
        }

        public Resource GetTaxesResource() => _taxResource;

        public int GetTaxesAmount() => _taxAmount;
        
        private void GenerateTaxesAmount(){
            _taxResource = _player.GetResource("Seaweed");
            _taxAmount = _random.Next(50, 100);
        }

        public void Open()
        {
            if (_open) return;

            _open = true;
            _viewport.SetActive(true);
            GenerateTaxes();
            GameManager.Instance.OnUIOpen();

            _viewport.transform.DOMoveY(Screen.height * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            if (!_open) return;

            _viewport.transform.DOMoveY(Screen.height * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
            };

            OnOptionSelected?.Invoke();
            GameManager.Instance.OnUIClose();
        }

        private void GenerateTaxes()
        {
            // TODO - Algorithm that looks what the player have and generated the resources.
            _buildings = BuildingManager.Instance.GetBuildings();
            
            ResourcesSprites[0].gameObject.SetActive(true);
            ResourcesSprites[0].sprite = _taxResource.GetSprite();
            ResourcesAmount[0].gameObject.SetActive(true);
            ResourcesAmount[0].SetText(_taxAmount.ToString());
            
            PayButton.interactable = _player.GetResourceCount(_taxResource.GetKey()) < _taxAmount;
            PayButton.image.color = _player.GetResourceCount(_taxResource.GetKey()) >= _taxAmount ? 
                new Color(1f, 1f, 1f, 1f) :  new Color(0.81f, 0.84f, 0.81f, 0.9f);
        }

        private void PayTaxes()
        {
            _player.AddResource(_taxResource.GetKey(), _taxAmount);

            StartCoroutine(AchievementsManager.Instance.UnlockAchievement("07"));
            GenerateTaxesAmount();
            
            Close();
            _dialogueBox.AddNewDialogue(PayDialogue);
        }

        private void DestroyBuilding()
        {
            int range;
            
            do range = Random.Range(0, _buildings.Count);
            while(_buildings[range].BuildData().GetKey() == "CityHall");

            if (_buildings.Count > 0)
            {
                GameObject go = Instantiate(BuildingDestroyParticles, _buildings[range].transform.position, Quaternion.identity);
                go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                TileManager.Instance.DestroyBuilding(_buildings[range]);
            }

            SoundManager.Instance.Play(DestructionSound);
            StartCoroutine(AchievementsManager.Instance.UnlockAchievement("08"));
            _dialogueBox.DialogueEnd -= DestroyBuilding;
        }

        private void NotPayTaxes()
        {
            _dialogueBox.AddNewDialogue(NotPayDialogue);
            _dialogueBox.DialogueEnd += DestroyBuilding;
            
            GenerateTaxesAmount();
            Close();
        }
    }
}
