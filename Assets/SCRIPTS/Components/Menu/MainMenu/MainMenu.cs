using UnityEngine;

namespace GGG.Components.Menus
{
    public class MainMenu : MonoBehaviour
    {
        private void Start()
        {
            SoundManager.Instance.Play("MainMenu");
        }
    }
}
