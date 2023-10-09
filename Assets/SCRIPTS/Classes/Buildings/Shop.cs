using UnityEngine;

namespace GGG.Classes.Buildings
{
    [CreateAssetMenu(fileName = "Shop", menuName = "Game/Buildings/Shop")]
    public class Shop : Building
    {
        public override void Interact()
        {
            Debug.Log("Interaction with shop");
        }
    }
}
