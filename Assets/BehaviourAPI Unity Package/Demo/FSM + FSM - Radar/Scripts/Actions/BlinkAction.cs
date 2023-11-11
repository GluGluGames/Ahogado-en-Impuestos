using UnityEngine;
using UnityEngine.UI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core;

    /// <summary>
    /// Custom action that makes a light blink.
    /// </summary>
    /// 
    [SelectionGroup("DEMO - Radar")]
    public class BlinkAction : UnityAction
    {
        public Light Light;
        public Color Color;
        public Text Marker;

        bool _intensityIncreasing;

        public BlinkAction()
        {
        }


        public BlinkAction(Light light, Text marker, Color color)
        {
            Light = light;
            Color = color;
            Marker = marker;
        }

        public override void Start()
        {
            Light.color = Color;
            Light.intensity = 1f;
            _intensityIncreasing = true;
            Marker.text = "---";
        }

        public override void Stop()
        {
            if (Light != null) Light.intensity = 1f;
            if (Marker != null) Marker.text = "000";
        }

        public override Status Update()
        {
            if (Light.intensity >= 2)
            {
                _intensityIncreasing = false;
            }
            else if (Light.intensity <= 0)
            {
                _intensityIncreasing = true;
            }
            Light.intensity += (_intensityIncreasing) ? 0.1f : -0.1f;
            return Status.Running;
        }

        public override string ToString()
        {
            var colorTag = $"#{ColorUtility.ToHtmlStringRGB(Color)}";
            return $"Make light blink in <color={colorTag}>color</color> and disable {Marker.name}"; 
        }
    }
}
