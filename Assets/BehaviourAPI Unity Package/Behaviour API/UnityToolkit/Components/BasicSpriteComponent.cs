using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public class BasicSpriteComponent : MonoBehaviour, IRendererComponent
    {
        [SerializeField] SpriteRenderer spriteRenderer;

        public Sprite Sprite { get => spriteRenderer.sprite; set => spriteRenderer.sprite = value; }
        public Color Tint { get => spriteRenderer.color; set => spriteRenderer.color = value; }
        public bool FlipX { get => spriteRenderer.flipX; set => spriteRenderer.flipX = value; }
        public bool FlipY { get => spriteRenderer.flipY; set => spriteRenderer.flipY = value; }
    }
}
