using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Singleton class to manage all Smart objects in the scene
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class SmartObjectManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static SmartObjectManager Instance { get; private set; }

        /// <summary>
        /// List of registered objects.
        /// </summary>
        public List<SmartObject> RegisteredObjects { get; private set; } = new List<SmartObject>();


        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// Add an object to the registered list.
        /// </summary>
        /// <param name="smartObject">The smart object added</param>
        public void RegisterSmartObject(SmartObject smartObject)
        {
            RegisteredObjects.Add(smartObject);
        }

        /// <summary>
        /// Remove an objet to the registered list.
        /// </summary>
        /// <param name="smartObject"></param>
        public void UnregisterSmartObject(SmartObject smartObject)
        {
            RegisteredObjects.Remove(smartObject);
        }
    }
}
