using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Project.Component.UI.Containers
{
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private GameObject TooltipContainer;
        [SerializeField] private TMP_Text ResourceText;

        private void Start()
        {
            TooltipContainer.SetActive(false);
        }

        public void SetResourceName(string resourceName) => ResourceText.SetText(resourceName);

        private IEnumerator ShowTooltip()
        {
            yield return new WaitForSeconds(0.2f);
            TooltipContainer.SetActive(true);
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            StartCoroutine(ShowTooltip());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            TooltipContainer.SetActive(false);
        }
    }
}
