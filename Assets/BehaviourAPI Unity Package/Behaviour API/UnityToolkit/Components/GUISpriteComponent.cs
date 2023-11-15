using UnityEngine;
using UnityEngine.UI;

namespace BehaviourAPI.UnityToolkit
{
    public class GUISpriteComponent : MonoBehaviour, IRendererComponent
    {
        [SerializeField] Image image;

        public Sprite Sprite { get => image.sprite; set => image.sprite = value; }
        public Color Tint { get => image.color; set => image.color = value; }
        public bool FlipX
        {
            get => isFlipX; 
            set
            {
                if(isFlipX == value) return;
                Vector3 scale = image.transform.localScale;
                scale.x *= -1f;
                image.transform.localScale = scale;
            }
        }
        public bool FlipY
        {
            get => isFlipY;
            set
            {
                if (isFlipY == value) return;
                Vector3 scale = image.transform.localScale;
                scale.y *= -1f;
                image.transform.localScale = scale;
            }
        }

        bool isFlipX, isFlipY;
    }
}
