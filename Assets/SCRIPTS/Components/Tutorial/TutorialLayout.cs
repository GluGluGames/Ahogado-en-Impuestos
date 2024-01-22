using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Tutorial
{
    public class TutorialLayout : MonoBehaviour
    {
        [SerializeField] private TMP_Text TutorialText;
        [SerializeField] private Image TutorialImage;

        public bool WithImage() => TutorialImage;

        public void SetText(string text) => TutorialText.SetText(text);

        public void SetImage(Sprite image)
        {
            if (!WithImage()) return;

            TutorialImage.sprite = image;
        }

        public void SetActive(Sprite image)
        {
            gameObject.SetActive((WithImage() && image) || (!WithImage() && !image));
        }
    }
}
