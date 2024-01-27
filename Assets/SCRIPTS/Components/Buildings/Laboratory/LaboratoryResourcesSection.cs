using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryResourcesSection : MonoBehaviour
    {
        private List<LaboratoryResearchButton> _researchButtons = new();
        private LaboratoryBackButton _backButton;
        private GameObject _viewport;
        
        private void Awake()
        {
            _backButton = FindObjectOfType<LaboratoryBackButton>(true);
            _researchButtons = FindObjectsOfType<LaboratoryResearchButton>(true).ToList();
            
            _viewport = transform.GetChild(0).gameObject;
        }

        private void OnEnable()
        {
            Hide();

            _researchButtons.ForEach(x => x.OnBarSelected += Show);
            _backButton.OnBack += Hide;
            
            LaboratoryListener.OnBuildingResearch += AuxHide;
            LaboratoryListener.OnResourceResearch += AuxHide;
        }

        private void OnDisable()
        {
            _researchButtons.ForEach(x => x.OnBarSelected -= Show);
            _backButton.OnBack -= Hide;
            
            LaboratoryListener.OnBuildingResearch -= AuxHide;
            LaboratoryListener.OnResourceResearch -= AuxHide;
        }

        private void AuxHide(Resource resource, int i) => Hide();
        private void AuxHide(Building resource, int i) => Hide();

        private void Show(int i)
        {
            _viewport.SetActive(true);
            LaboratoryListener.InitializeContainerButtons();
        }
        private void Hide() => _viewport.SetActive(false);
    }
}
