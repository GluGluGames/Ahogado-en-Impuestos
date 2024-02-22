using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    public class BuildingPanel : MonoBehaviour
    {
        [SerializeField] private int Index;

        private List<BuildingArrow> _arrows;
        private GameObject _viewport;
        
        private void Awake()
        {
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(Index == 0);
        }

        public void Initialize()
        {
            if(Index == 0) Show();
            else Hide();
        }

        public void OnArrow(int page)
        {
            if (Index == page) Show();
            else Hide();
        }

        private void Show() => _viewport.SetActive(true);
        private void Hide() => _viewport.SetActive(false);
    }
}
