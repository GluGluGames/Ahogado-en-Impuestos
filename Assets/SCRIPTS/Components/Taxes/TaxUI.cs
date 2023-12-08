using GGG.Shared;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;

using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using GGG.Components.Achievements;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GGG.Components.Taxes
{
    public class TaxUI : MonoBehaviour
    {
        [Header("UI Fields")]
        [SerializeField] private Image[] ResourcesSprites;
        [SerializeField] private TMP_Text[] ResourcesAmount;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button PayButton;
        [SerializeField] private Button NotPayButton;

        private PlayerManager _player;
        private List<BuildingComponent> _buildings;
        private readonly Dictionary<Resource, int> _taxResources = new();
        private readonly System.Random _random = new();

        private GameObject _viewport;
        private bool _open;

        public Action OnOptionSelected;

        private void Start()
        {
            _player = PlayerManager.Instance;
            
            PayButton.onClick.AddListener(PayTaxes);
            NotPayButton.onClick.AddListener(NotPayTaxes);

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.5f);
        }

        private void OnDisable()
        {
            PayButton.onClick.RemoveAllListeners();
            NotPayButton.onClick.RemoveAllListeners();
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
            _taxResources.Clear();
            GameManager.Instance.OnUIClose();
        }

        private void GenerateTaxes()
        {
            // TODO - Algorithm that looks what the player have and generated the resources.
            _taxResources.Add(_player.GetResource("Seaweed"), _random.Next(50, 100));
            _buildings = BuildingManager.Instance.GetBuildings();

            bool found = false;
            int i = 0;

            foreach (Resource resource in _taxResources.Keys)
            {
                ResourcesSprites[i].gameObject.SetActive(true);
                ResourcesSprites[i].sprite = resource.GetSprite();
                ResourcesAmount[i].gameObject.SetActive(true);
                ResourcesAmount[i].SetText(_taxResources[resource].ToString());

                if (_player.GetResourceCount(resource.GetKey()) < _taxResources[resource])
                {
                    PayButton.interactable = false;
                    PayButton.image.color = new Color(0.81f, 0.84f, 0.81f, 0.9f);
                    found = true;
                }

                i++;
            }

            if (found) return;
            
            PayButton.interactable = true;
            PayButton.image.color = new Color(1f, 1f, 1f, 1f);
        }

        private void PayTaxes()
        {
            foreach (Resource resource in _taxResources.Keys)
                _player.AddResource(resource.GetKey(), -_taxResources[resource]);

            StartCoroutine(AchievementsManager.Instance.UnlockAchievement("07"));
            
            Close();
            // TODO - Maybe trigger a dialogue with Poseidon?
        }

        private void NotPayTaxes()
        {
            if (_buildings.Count > 0)
                TileManager.Instance.DestroyBuilding(_buildings[Random.Range(0, _buildings.Count)]);
            
            StartCoroutine(AchievementsManager.Instance.UnlockAchievement("08"));
            
            Close();
        }
    }
}
