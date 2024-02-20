using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Components.UI.Buttons;
using GGG.Shared;
using UnityEngine;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryListener : MonoBehaviour
    {
        private List<LaboratoryContainer> _containers = new ();
        private static List<ContainerButton> _containerButtons = new ();
        private List<LaboratoryResearchButton> _researchButtons = new();
        
        private Laboratory _selectedLaboratory;
        private static ContainerButton _containerButton;
        private int _researchIndex;

        public static Action<Resource, int> OnResourceResearch;
        public static Action<Building, int> OnBuildingResearch;
        public static Action<int> OnResearchFinish;

        private void OnEnable()
        {
            if (_containers.Count <= 0) 
                _containers = GetComponentsInChildren<LaboratoryContainer>(true).ToList();
            if (_containerButtons.Count <= 0)
                _containerButtons = GetComponentsInChildren<ContainerButton>(true).ToList();
            if (_researchButtons.Count <= 0)
                _researchButtons = GetComponentsInChildren<LaboratoryResearchButton>(true).ToList();

            LaboratoryUI.OnLaboratoryOpen += Initialize;
            LaboratoryUI.OnLaboratoryClose += Deinitialize;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            
            LaboratoryUI.OnLaboratoryOpen -= Initialize;
            LaboratoryUI.OnLaboratoryClose -= Deinitialize;
        }

        private void Initialize(Laboratory laboratory)
        {
            InitializeResearchButtons();
            InitializeButtons();
            _selectedLaboratory = laboratory;
        }

        public static void InitializeContainerButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                if (button.GetIndex() == 0)
                {
                    _containerButton = button;
                    _containerButton.SelectButton();
                }

                button.OnSelect += OnContainerSelect;
            }
        }

        private void InitializeResearchButtons()
        {
            foreach (LaboratoryResearchButton button in _researchButtons)
                button.OnBarSelected += SelectBar;
        }

        private void InitializeButtons()
        {
            foreach (LaboratoryContainer container in _containers)
            {
                foreach (LaboratoryButton button in container.Buttons())
                    button.OnSelect += Research;
            }
        }

        private void SelectBar(int idx) => _researchIndex = idx;

        private void Research(Resource resource, Building building)
        {
            if (resource)
            {
                _selectedLaboratory.SetActiveResource(_researchIndex, resource);
                _selectedLaboratory.SetDeltaTime(_researchIndex, resource.GetResearchTime());
                StartCoroutine(StartBar(_researchIndex));
                OnResourceResearch?.Invoke(resource, _researchIndex);
                return;
            }
            
            _selectedLaboratory.SetActiveBuild(_researchIndex, building);
            _selectedLaboratory.SetDeltaTime(_researchIndex, building.GetResearchTime());
            StartCoroutine(StartBar(_researchIndex));
            OnBuildingResearch?.Invoke(building, _researchIndex);
        }

        public void Research(int bar) => StartCoroutine(StartBar(bar));

        private IEnumerator StartBar(int idx)
        {
            float delta = _selectedLaboratory.DeltaTime(idx);
            Laboratory laboratory = _selectedLaboratory;

            while (delta >= 0)
            {
                delta -= Time.deltaTime;
                laboratory.AddDeltaTime(idx, -Time.deltaTime);
                yield return null;
            }
            
            if (laboratory.ActiveBuild(idx))
            {
                laboratory.ActiveBuild(idx).Unlock();
                laboratory.SetActiveBuild(idx, null);
            }
            else
            {
                laboratory.ActiveResource(idx).Unlock();
                laboratory.SetActiveResource(idx, null);
            }
            
            OnResearchFinish?.Invoke(idx);
        }

        private void Deinitialize()
        {
            DeinitializeResearchButtons();
            DeinitializeContainerButtons();
            DeinitializeButtons();
            
            _containerButton = null;
            _selectedLaboratory = null;
        }

        private void DeinitializeResearchButtons()
        {
            foreach (LaboratoryResearchButton button in _researchButtons)
                button.OnBarSelected -= SelectBar;
        }

        private void DeinitializeContainerButtons()
        {
            foreach (ContainerButton button in _containerButtons)
            {
                button.DeselectButton();
                button.OnSelect -= OnContainerSelect;
            }
        }

        private void DeinitializeButtons()
        {
            foreach (LaboratoryContainer container in _containers)
            {
                foreach (LaboratoryButton button in container.Buttons())
                    button.OnSelect -= Research;
            }
        }

        private static void OnContainerSelect(ContainerButton button)
        {
            if (_containerButton) _containerButton.DeselectButton();
            _containerButton = button;
        }
    }
}
