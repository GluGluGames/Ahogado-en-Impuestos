using GGG.Components.Core;
using GGG.Components.Scenes;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class ExitButton : MonoBehaviour
    {
        private void OnEnable()
        {
            gameObject.SetActive(!SceneManagement.InMiniGameScene());
        }

        public void OnExitButton()
        {
            Application.Quit();
        }
    }
}
