using BehaviourAPI.Core;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    [SelectionGroup("DEMO - Radar")]
    public class LightAction : UnityAction
    {
        public Light Light;
        public Color Color;
        public float TimeToEnd;

        float _currentTime;

        public LightAction()
        {
        }

        public LightAction(Light light, Color color, float timeToEnd = -1f)
        {
            Light = light;
            Color = color;
            TimeToEnd = timeToEnd;
        }

        public override void Start()
        {
            _currentTime = 0f;
            Light.color = Color;
        }

        public override void Stop()
        {
            _currentTime = 0f;
        }

        public override Status Update()
        {
            if (TimeToEnd >= 0f)
            {
                _currentTime += Time.deltaTime;
                if (_currentTime > TimeToEnd)
                {
                    return Status.Success;
                }
            }
            return Status.Running;
        }

        public override string ToString()
        {
            var colorTag = $"#{ColorUtility.ToHtmlStringRGB(Color)}";
            return $"Change light ({Light}) color to <color={colorTag}>color</color>";
        }
    }

}