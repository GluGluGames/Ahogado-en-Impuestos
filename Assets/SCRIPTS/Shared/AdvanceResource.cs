using UnityEngine;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Advance Resource", menuName = "Game/Resources/Advance Resource")]
    public class AdvanceResource : Resource
    {
        [SerializeField] private AdvanceResources Type;

        public AdvanceResources GetResource()
        {
            return Type;
        }
    }
}
