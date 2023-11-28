namespace GGG.Components.Buildings.Generator
{
    public class Generator : BuildingComponent
    {
        private GeneratorUI _ui;
        private int[] _currentGeneration;
        private int[,] _boostIndexes;
        private int[,] _boostBuildings;
        private float[,] _boostingTimes;

        public override void Initialize()
        {
            if(!_ui) _ui = FindObjectOfType<GeneratorUI>();

            _currentGeneration = new int[3];
            _boostIndexes = new int[3, 6];
            _boostBuildings = new int[3, 6];

            for (int i = 0; i < _boostBuildings.GetLength(0); i++)
            {
                for (int j = 0; j < _boostBuildings.GetLength(1); j++)
                    _boostBuildings[i, j] = -1;
            }

            _boostingTimes = new float[3, 6];
        }

        public override void Interact()
        {
            _ui.Open(this);
        }

        public override void OnBuildDestroy()
        {
            _ui.OnBuildDestroy(Id());
        }

        public int CurrentGeneration(int level) => _currentGeneration[level - 1];

        public void AddGeneration(int level, int amount) => _currentGeneration[level - 1] += amount;

        public int[,] BoostIndexes() => _boostIndexes;

        public int BoostIndex(int level, int idx) => _boostIndexes[level - 1, idx];

        public void AddBoostIndex(int level, int idx, int amount) => _boostIndexes[level - 1, idx] += amount;

        public int BoostBuilding(int level, int idx) => _boostBuildings[level - 1, idx];

        public void AddBoostingBuilding(int level, int idx, int id) => _boostBuildings[level - 1, idx] = id;    

        public float BoostingTime(int level, int idx) => _boostingTimes[level - 1, idx];

        public void SetBoostingTime(int level, int idx, float time) => _boostingTimes[level - 1, idx] = time;

        public void AddBoostingTime(int level, int idx, float time) => _boostingTimes[level - 1, idx] += time;
    }
}
