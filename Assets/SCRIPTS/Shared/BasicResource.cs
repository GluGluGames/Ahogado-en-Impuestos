using UnityEngine;

namespace GGG.Shared
{
    [CreateAssetMenu(fileName = "Basic Resource", menuName = "Game/Resources/Basic Resource")]
    public class BasicResource : Resource
    {
        [SerializeField] private BasicResources Type;

        public BasicResources GetResource()
        {
            return Type;
        }
    }
}
