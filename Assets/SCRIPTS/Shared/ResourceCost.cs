using UnityEngine;

namespace GGG.Shared
{
    [System.Serializable]
    public class ResourceCost
    {
        [SerializeField] private int[] Costs;
        [SerializeField] private Resource[] Resources;

        public int GetCost(int index) => Costs[index];
        public Resource GetResource(int index) => index < Resources.Length ? Resources[index] : null;
        public int GetCostsAmount() => Costs.Length;
        public int AddCost(int index, int amount) => Costs[index] += amount;
    }
}
