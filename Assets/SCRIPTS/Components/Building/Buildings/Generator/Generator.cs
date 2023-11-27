namespace GGG.Components.Buildings.Generator
{
    public class Generator : BuildingComponent
    {
        private GeneratorUI _ui;

        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<GeneratorUI>();
        }

        public override void Interact()
        {
            _ui.Open(_currentLevel);
        }

        public override void OnBuildDestroy()
        {
            _ui.OnBuildDestroy(Id());
        }
    }
}
