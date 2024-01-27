using System;
using System.Collections.Generic;
using GGG.Classes.Buildings;
using GGG.Shared;

namespace GGG.Components.Buildings.Laboratory
{
    public class Laboratory : BuildingComponent
    {
        private LaboratoryUI _ui;
        
        private Resource[] _activeResources;
        private Building[] _activeBuildings;
        private float[] _deltaTimes;

        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<LaboratoryUI>();
            
            _activeResources = new Resource[3];
            _activeBuildings = new Building[3];
            _deltaTimes = new float[3];
        }

        public override void Interact()
        {
            _ui.Open(this);
        }

        public bool IsBarActive(int idx) => _activeBuildings[idx] || _activeResources[idx];
        public Resource ActiveResource(int idx) => _activeResources[idx];
        public void SetActiveResource(int idx, Resource resource) => _activeResources[idx] = resource;
        public Building ActiveBuild(int idx) => _activeBuildings[idx];
        public void SetActiveBuild(int idx, Building building) => _activeBuildings[idx] = building;
        public float DeltaTime(int idx) => _deltaTimes[idx];
        public void SetDeltaTime(int idx, float time) => _deltaTimes[idx] = time;
        public void AddDeltaTime(int idx, float amount) => _deltaTimes[idx] += amount;
    }
}
