using GGG.Components.Buildings;

namespace GGG.Components.Buildings.Laboratory
{
    public class Laboratory : BuildingComponent
    {
        private LaboratoryUI _ui;

        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<LaboratoryUI>();
        }

        public override void Interact()
        {
            _ui.Open();
        }
    }
}
