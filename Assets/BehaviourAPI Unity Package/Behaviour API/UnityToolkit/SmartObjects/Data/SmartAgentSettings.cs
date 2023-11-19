using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    [CreateAssetMenu(fileName = "NewAgentSettings", menuName = "BehaviourAPI/Smart Objects/Agent Settings")]
    public class SmartAgentSettings : ScriptableObject
    {
        [Header("Needs")]
        [SerializeField] NeedMap _needMap;

        public Dictionary<string, float> GetCapabilityMap() => new Dictionary<string, float>(_needMap);
    }
}
