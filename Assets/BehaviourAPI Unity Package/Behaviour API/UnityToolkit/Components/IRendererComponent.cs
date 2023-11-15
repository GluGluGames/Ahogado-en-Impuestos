using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public interface IRendererComponent
    {
        public Sprite Sprite { get; set; }
        public Color Tint { get; set; }
        public bool FlipX { get; set; }
        public bool FlipY { get; set; } 
    }
}
