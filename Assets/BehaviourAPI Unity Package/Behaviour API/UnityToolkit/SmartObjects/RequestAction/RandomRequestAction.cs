using BehaviourAPI.SmartObjects;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Request action that uses a random smart object
    /// </summary>
    public class RandomRequestAction : UnityRequestAction
    {
        /// <summary>
        /// The data sent with the request
        /// </summary>
        public RequestData requestData;

        public RandomRequestAction(SmartAgent agent, RequestData requestData = null) : base(agent)
        {
            this.requestData = requestData;
        }

        protected override RequestData GetRequestData() => requestData;

        protected override SmartObject GetRequestedSmartObject()
        {
            if(SmartObjectManager.Instance == null)
            {
                Debug.LogWarning("This request action need a SmartObjectManager in the scene");
            }

            int random = Random.Range(0, SmartObjectManager.Instance.RegisteredObjects.Count);

            return SmartObjectManager.Instance.RegisteredObjects[random];
        }
    }
}
