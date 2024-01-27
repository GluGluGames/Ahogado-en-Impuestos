using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryProgressBar : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;
        [SerializeField] private Image Fill;

        private Laboratory _laboratory;
        private Resource _resource;
        private Building _building;
        
        private float _currentFill;
        private bool _fillBar;
        private float _delta;
        
        private void Awake()
        {
            Fill.fillAmount = 0f;
        }

        private void OnEnable()
        {
            LaboratoryListener.OnResourceResearch += OnResearch;
            LaboratoryListener.OnBuildingResearch += OnResearch;
            
            LaboratoryUI.OnLaboratoryOpen += OnOpen;
            LaboratoryUI.OnLaboratoryClose += OnClose;
        }

        private void Update()
        {
            if (!_fillBar) return;

            _delta -= Time.deltaTime;
            if (_delta <= 0)
            {
                Fill.fillAmount = 0f;
                _fillBar = false;
                return;
            }

            float totalTime = _resource ? _resource.GetResearchTime() : _building.GetResearchTime();
            Fill.fillAmount = Mathf.Clamp01(1 - _laboratory.DeltaTime(Index) / totalTime);
        }

        private void OnResearch(Resource resource, int idx)
        {
            if (Index != idx) return;

            _resource = resource;
            _delta = _laboratory.DeltaTime(Index);
            _fillBar = true;
        }

        private void OnResearch(Building building, int idx)
        {
            if (Index != idx) return;

            _building = building;
            _delta = _laboratory.DeltaTime(Index);
            _fillBar = true;
        }

        private void OnOpen(Laboratory laboratory)
        {
            _laboratory = laboratory;
            if (Index + 1 > laboratory.CurrentLevel())
            {
                gameObject.SetActive(false);
                return;
            }

            if (!_laboratory.IsBarActive(Index)) return;
            
            _delta = _laboratory.DeltaTime(Index);
            
            float totalTime = _laboratory.ActiveResource(Index) 
                ? _laboratory.ActiveResource(Index).GetResearchTime() 
                : _laboratory.ActiveBuild(Index).GetResearchTime();
            Fill.fillAmount = Mathf.Clamp01(1 - _laboratory.DeltaTime(Index) / totalTime);
            _fillBar = true;
        }

        private void OnClose()
        {
            LaboratoryListener.OnResourceResearch -= OnResearch;
            LaboratoryListener.OnBuildingResearch -= OnResearch;
            
            LaboratoryUI.OnLaboratoryClose -= OnClose;

            _laboratory = null;
            _delta = 0;
            _fillBar = false;
        }
    }
}
