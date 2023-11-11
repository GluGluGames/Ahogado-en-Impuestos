using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using SmartObjects;
    using Core.Actions;
    using System.Collections.Generic;

    public abstract class SmartInteractionProvider : MonoBehaviour
    {
        [SerializeField] NeedMap capabilities;

        public SmartInteraction CreateInteraction(SmartAgent smartAgent)
        {
            Action action = GetInteractionAction(smartAgent);
            SmartInteraction smartInteraction = new SmartInteraction(action, smartAgent,GetCapabilityMap());
            SetInteractionEvents(smartInteraction);
            return smartInteraction;
        }

        protected abstract Action GetInteractionAction(SmartAgent agent);

        public Dictionary<string, float> GetCapabilityMap() => capabilities;

        protected virtual void SetInteractionEvents(SmartInteraction smartInteraction)
        {
            return;
        }
    }
}
