using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    [RequireComponent(typeof(Button))]
    public class LaboratoryResearchButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Range(0, 2)] private int Index;
        [SerializeField] private Image Icon;

        private Laboratory _laboratory;
        private Resource _resource;
        private Building _building;
        
        public Action<int> OnBarSelected;

        private void OnEnable()
        {
            LaboratoryUI.OnLaboratoryOpen += OnOpen;
            LaboratoryUI.OnLaboratoryClose += OnClose;
            
            LaboratoryListener.OnBuildingResearch += OnResearch;
            LaboratoryListener.OnResourceResearch += OnResearch;
            LaboratoryListener.OnResearchFinish += OnFinishResearch;
        }

        private void OnOpen(Laboratory laboratory)
        {
            _laboratory = laboratory;
            Icon.color = laboratory.IsBarActive(Index) ? Color.white : new Color(1, 1, 1, 0);
            if (!laboratory.IsBarActive(Index)) return;
            
            Icon.sprite = laboratory.ActiveResource(Index)
                ? laboratory.ActiveResource(Index).GetSprite()
                : laboratory.ActiveBuild(Index).GetResearchIcon();

            _resource = laboratory.ActiveResource(Index);
            _building = laboratory.ActiveBuild(Index);
        }

        private void OnClose()
        {
            LaboratoryUI.OnLaboratoryClose -= OnClose;
            
            LaboratoryListener.OnBuildingResearch -= OnResearch;
            LaboratoryListener.OnResourceResearch -= OnResearch;

            _building = null;
            _resource = null;
            Icon.color = new Color(1, 1, 1, 0);
        }

        private void OnResearch(Resource resource, int i)
        {
            if (i != Index) return;
            
            Icon.sprite = resource.GetSprite();
            Icon.color = Color.white;
            _resource = resource;
            _resource.BeingResearch();
        }

        private void OnResearch(Building building, int i)
        {
            if (i != Index) return;
            
            Icon.sprite = building.GetResearchIcon();
            Icon.color = Color.white;
            _building = building;
            _building.Research();
        }

        private void OnFinishResearch(int idx)
        {
            if (Index != idx) return;
            
            Icon.color = new Color(1, 1, 1, 0);

            if (_building) _building.Unlock();
            if (_resource) _resource.Unlock();
        }

        public void OnSelect()
        {
            if (_laboratory.IsBarActive(Index)) return;
            
            OnBarSelected?.Invoke(Index);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_laboratory.IsBarActive(Index)) return;

            Icon.sprite = _building ? _building.GetSelectedIcon() : _resource.GetSelectedSprite();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_laboratory.IsBarActive(Index)) return;

            Icon.sprite = _building ? _building.GetResearchIcon() : _resource.GetSprite();
        }
    }
}
