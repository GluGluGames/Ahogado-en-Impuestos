using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    /// <summary>
    /// Stores a behaviour system data in a asset file.
    /// </summary>
    [CreateAssetMenu(fileName = "newBehaviourSystem", menuName = "BehaviourAPI/BehaviourSystem", order = 0)]
    public class BehaviourSystem : ScriptableObject, IBehaviourSystem
    {
        [SerializeField] SystemData data;
        public SystemData Data => data;
        public Object ObjectReference => this;

        private bool isValid;
        private bool validated;

        /// <summary>
        /// Get a runtime copy of the behaviour system stored in this asset.
        /// </summary>
        /// <returns>A copy of <see cref="Data"/></returns>
        public SystemData GetBehaviourSystemData()
        {
            if(!validated)
            {
                isValid = data.CheckCyclicReferences();
                validated = true;
            }

            if (isValid)
            {
                string json = JsonUtility.ToJson(this);
                BehaviourSystem copy = CreateInstance<BehaviourSystem>();
                JsonUtility.FromJsonOverwrite(json, copy);
                return copy.Data;
            }
            else
            {
                Debug.LogWarning("BUILD ERROR: Cyclic references in asset subgraphs");
                return null;
            }
        }

        public static BehaviourSystem CreateSystem(SystemData data) 
        { 
            var system = CreateInstance<BehaviourSystem>();
            system.data = data;
            return system;
        }
    }
}