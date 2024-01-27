using System;
using GGG.Classes.Buildings;
using GGG.Shared;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    [RequireComponent(typeof(TMP_Text))]
    public class LaboratoryTimer : MonoBehaviour
    {
        [SerializeField, Range(0, 2)] private int Index;
        
        private TMP_Text _timer;
        private Laboratory _laboratory;

        private bool _researching;
        
        private void Awake()
        {
            _timer = GetComponent<TMP_Text>();
            SetTimer(-1);
        }

        private void OnEnable()
        {
            LaboratoryUI.OnLaboratoryOpen += InitializeTimer;
            LaboratoryUI.OnLaboratoryClose += OnClose;
            
            LaboratoryListener.OnResourceResearch += SetTimer;
            LaboratoryListener.OnBuildingResearch += SetTimer;
        }

        private void Update()
        {
            if (!_researching) return;

            if (_laboratory.DeltaTime(Index) <= 0)
            {
                SetTimer(-1);
                _researching = false;
                return;
            }
            
            SetTimer(_laboratory.DeltaTime(Index));
        }

        private void OnClose()
        {
            LaboratoryUI.OnLaboratoryClose -= OnClose;
            LaboratoryListener.OnResourceResearch -= SetTimer;
            LaboratoryListener.OnBuildingResearch -= SetTimer;
            
            _researching = false;
        }

        private void InitializeTimer(Laboratory laboratory)
        {
            _laboratory = laboratory;
            if (Index + 1 > laboratory.CurrentLevel())
            {
                gameObject.SetActive(false);
                return;
            }

            if (!_laboratory.IsBarActive(Index))
            {
                SetTimer(-1);
                return;
            }
            
            SetTimer(laboratory.DeltaTime(Index));
        }

        private void SetTimer(float time)
        {
            if (time < 0)
            {
                _timer.SetText("--:--");
                return;
            }
            
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            _timer.SetText($"{minutes:00}:{seconds:00}");
            _researching = true;
        }

        private void SetTimer(Resource resource, int i)
        {
            if (Index != i) return;
            
            int minutes = Mathf.FloorToInt((float) resource.GetResearchTime() / 60);
            int seconds = Mathf.FloorToInt((float) resource.GetResearchTime() % 60);
            _timer.SetText($"{minutes:00}:{seconds:00}");
            _researching = true;
        }

        private void SetTimer(Building building, int i)
        {
            if (Index != i) return;
            
            int minutes = Mathf.FloorToInt((float) building.GetResearchTime() / 60);
            int seconds = Mathf.FloorToInt((float) building.GetResearchTime() % 60);
            _timer.SetText($"{minutes:00}:{seconds:00}");
            _researching = true;
        }
    }
}
