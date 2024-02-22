using System;
using TMPro;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    [RequireComponent(typeof(TMP_Text))]
    public class BuildingPageText : MonoBehaviour
    {
        private TMP_Text _pageText;

        private void Awake()
        {
            _pageText = GetComponent<TMP_Text>();
        }
 
        public void UpdateText(int page, int maxPage) => _pageText.SetText($"{page + 1}/{maxPage}");
        
    }
}
