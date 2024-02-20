namespace GGG.Components.Buildings.Generator
{
    public class Generator : BuildingComponent
    {
        private GeneratorUI _ui;
        private int[] _boostAmount;
        private int[,] _boostIndex;
        private int[,] _boostBuildings;
            
        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<GeneratorUI>();

            _boostAmount = new int[3];
            _boostIndex = new int[3, 6];
            _boostBuildings = new int[3, 6];
        }

        public override void Interact()
        {
            _ui.Open(this);
        }

        public int CurrentGeneration(int level) => _boostAmount[level - 1];
        public void AddGeneration(int level, int amount) => _boostAmount[level - 1] += amount;
        public void SetIndex(int level, int index, int amount) => _boostIndex[level - 1, index] = amount;
        public int[,] Indexes() => _boostIndex;
        public int Index(int x, int y) => _boostIndex[x, y];
        public void SetBoostBuilding(int level, int idx, int id) => _boostBuildings[level - 1, idx] = id;
        public int[,] BoostBuildings() => _boostBuildings;
        public int BoostBuilding(int x, int y) => _boostBuildings[x, y];
    }
}
