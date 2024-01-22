using TMPro;
using UnityEngine;

namespace GGG.Components.Tutorial
{
    public class TutorialTitle : MonoBehaviour
    {
        [SerializeField] private TMP_Text Title;
        
        public void SetTitle(string title)
        {
            Title.gameObject.SetActive(!string.IsNullOrEmpty(title));
            Title.SetText(title);
        }
    }
}
