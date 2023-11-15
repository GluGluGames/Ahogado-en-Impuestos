using BehaviourAPI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class RadarDisplay : MonoBehaviour
    {
        [SerializeField] Light _radarLight;
        [SerializeField] Text _speedText;

        bool _intensityIncreasing;

        public Light RadarLight => _radarLight;

        public bool CheckRadar(Func<float, bool> speedCheckFunction)
        {
            Ray ray = new Ray(transform.position, Vector3.left);
            if (Physics.Raycast(ray, out RaycastHit hit, 50) && hit.collider.tag == "Car")
            {
                var carSpeed = hit.collider.gameObject.GetComponent<ICar>().GetSpeed();

                bool trigger = speedCheckFunction?.Invoke(carSpeed) ?? false;
                _speedText.text = $"{Mathf.RoundToInt(carSpeed) + 100}";
                return trigger;
            }
            return false;
        }

        public Status Blink()
        {
            if (_radarLight.intensity >= 2)
            {
                _intensityIncreasing = false;
            }
            else if (_radarLight.intensity <= 0)
            {
                _intensityIncreasing = true;
            }
            _radarLight.intensity += (_intensityIncreasing) ? 0.1f : -0.1f;
            return Status.Running;
        }

        public void Break()
        {
            _radarLight.color = Color.yellow;
            _radarLight.intensity = 1f;
            _intensityIncreasing = true;
            _speedText.text = "---";
        }

        public void Fix()
        {
            if (_radarLight != null) _radarLight.intensity = 1f;
            if (_speedText != null) _speedText.text = "000";
        }
    }

}