using GGG.Shared;
using GGG.Components.Core;
using GGG.Components.Player;

using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Taxes
{
    public class TaxUI : MonoBehaviour
    {
        [Header("UI Fields")]
        [SerializeField] private GameObject Viewport;
        [SerializeField] private Image[] ResourcesSprites;
        [SerializeField] private TMP_Text[] ResourcesAmount;

        [Space(5), Header("Buttons")] 
        [SerializeField] private Button PayButton;
        [SerializeField] private Button NotPayButton;

        private const int _INITIAL_POSITION = -1100;

        private PlayerManager _player;
        private readonly Dictionary<Resource, int> _taxResources = new();
        private readonly System.Random _random = new();

        private bool _open;

        public Action OnOptionSelected;

        private void Start()
        {
            _player = PlayerManager.Instance;
            
            PayButton.onClick.AddListener(PayTaxes);
            NotPayButton.onClick.AddListener(NotPayTaxes);
        }

        public void Open()
        {
            if (_open) return;

            _open = true;
            Viewport.SetActive(true);
            GameManager.Instance.OnUIOpen();
            
            GenerateTaxes();

            transform.DOMoveY(Screen.height * 0.5f, 0.75f).SetEase(Ease.OutBounce);
        }

        private void Close()
        {
            if (!_open) return;

            transform.DOMoveY(Screen.height * 0.5f + _INITIAL_POSITION, 0.75f).SetEase(Ease.OutBounce).onComplete +=
            () =>
            {
                _open = false;
                Viewport.SetActive(false);
            };

            OnOptionSelected?.Invoke();
            _taxResources.Clear();
            GameManager.Instance.OnUIClose();
        }

        private void GenerateTaxes()
        {
            // TODO - Algorithm that looks what the player have and generated the resources.
            _taxResources.Add(_player.GetMainResource(), _random.Next(50, 100));

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
            
            Close();
            // TODO - Maybe trigger a dialogue with Poseidon?
        }

        private void NotPayTaxes()
        {
            // TODO - DESTROY RANDOM BUILDING
            Close();
        }
    }
}
