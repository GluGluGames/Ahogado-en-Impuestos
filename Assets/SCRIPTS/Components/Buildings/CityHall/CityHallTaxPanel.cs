using System;
using System.Collections;
using GGG.Components.Taxes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallTaxPanel : MonoBehaviour
    {
        [SerializeField] private Image TaxResource;
        [SerializeField] private TMP_Text TaxAmount;
        [SerializeField] private TMP_Text TaxCounter;

        private TaxUI _taxUI;
        private float _timeDelta;

        private void OnEnable()
        {
            if(!_taxUI) _taxUI = FindObjectOfType<TaxUI>(true);
            
            Initialize();
            StartCoroutine(StartCounter());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void Initialize()
        {
            _timeDelta = TaxManager.GetRemainingTime();
            int minutes = Mathf.FloorToInt(_timeDelta / 60);
            int seconds = Mathf.FloorToInt(_timeDelta % 60);
            TaxCounter.SetText($"{minutes:00}:{seconds:00}");

            TaxAmount.SetText(_taxUI.GetTaxesAmount().ToString());
            if(!_taxUI.GetTaxesResource()) return;
            TaxResource.sprite = _taxUI.GetTaxesResource().GetSprite();
        }

        private IEnumerator StartCounter()
        {
            int minutes, seconds;

            while (true)
            {
                _timeDelta = TaxManager.GetRemainingTime();
                minutes = Mathf.FloorToInt(_timeDelta / 60);
                seconds = Mathf.FloorToInt(_timeDelta % 60);
                TaxCounter.SetText($"{minutes:00}:{seconds:00}");
                yield return null;
            }
        }
    }
}
