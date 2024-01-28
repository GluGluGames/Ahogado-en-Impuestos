namespace GGG.Components.Buildings.Generator
{
    public class Generator : BuildingComponent
    {
        private GeneratorUI _ui;
        private int[] _boostAmount;

        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<GeneratorUI>();

            _boostAmount = new int[3];
        }

        public override void Interact()
        {
            _ui.Open(this);
        }

        public int CurrentGeneration(int level) => _boostAmount[level - 1];

        public void AddGeneration(int level, int amount) => _boostAmount[level - 1] += amount;
    }
}
