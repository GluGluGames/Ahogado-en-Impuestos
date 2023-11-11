using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using BehaviourAPI.SmartObjects;

    /// <summary>
    /// Request action for objects that covers a specific need  
    /// </summary>
    public class NeedRequestAction : UnityRequestAction
    {
        /// <summary>
        /// The covered need
        /// </summary>
        public string need;

        public NeedRequestAction(string need)
        {
            this.need = need;
        }

        public NeedRequestAction()
        {
        }

        protected override RequestData GetRequestData() => need;

        protected override SmartObject GetRequestedSmartObject()
        {
            var objects = SmartObjectManager.Instance.RegisteredObjects.FindAll(s => s.GetCapabilityValue(need) > 0);
            if (objects.Count > 0)
            {
                return objects[Random.Range(0, objects.Count)];
            }
            else
            {
                Debug.LogWarning("No objects found");
                return null;
            }
        }
    }

}