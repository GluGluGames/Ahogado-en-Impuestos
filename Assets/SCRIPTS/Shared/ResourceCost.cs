using UnityEngine;

namespace GGG.Shared
{
    [System.Serializable]
    public class ResourceCost
    {
        [SerializeField] private int[] Costs;
        [SerializeField] private Resource[] Resources;

        public ResourceCost(ResourceCost other)
        {
            Costs = (int[])other.Costs.Clone();
            Resources = (Resource[])other.Resources.Clone();
        }

        public ResourceCost(int[] costs, Resource[] resources)
        {
            Costs = costs;
            Resources = resources;
        }

        public int GetCost(int index) => Costs[index];
        public int[] GetCost() => Costs;
        public Resource GetResource(int index) => index < Resources.Length ? Resources[index] : null;
        public Resource[] GetResource() => Resources;
        public int GetCostsAmount() => Costs.Length;
        public int AddCost(int index, int amount) => Costs[index] += amount;
    }
}
