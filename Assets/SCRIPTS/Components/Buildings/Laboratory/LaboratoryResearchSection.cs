using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryResearchSection : MonoBehaviour
    {
        private List<LaboratoryResearchButton> _researchButtons;
        private LaboratoryBackButton _backButton;
        private GameObject _viewport;

        private void Awake()
        {
            _researchButtons = FindObjectsOfType<LaboratoryResearchButton>(true).ToList();
            _backButton = FindObjectOfType<LaboratoryBackButton>(true);
            
            _viewport = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            Show();
            
            _backButton.OnBack += Show;
            _researchButtons.ForEach(x => x.OnBarSelected += Hide);
            
            LaboratoryListener.OnBuildingResearch += AuxShow;
            LaboratoryListener.OnResourceResearch += AuxShow;
        }

        private void OnDisable()
        {
            _researchButtons.ForEach(x => x.OnBarSelected -= Hide);
            _backButton.OnBack -= Show;
            
            LaboratoryListener.OnBuildingResearch -= AuxShow;
            LaboratoryListener.OnResourceResearch -= AuxShow;
        }

        private void AuxShow(Resource resource, int i) => Show();
        private void AuxShow(Building resource, int i) => Show();

        private void Show() => _viewport.SetActive(true);
        private void Hide(int i) => _viewport.SetActive(false);
    }
}
