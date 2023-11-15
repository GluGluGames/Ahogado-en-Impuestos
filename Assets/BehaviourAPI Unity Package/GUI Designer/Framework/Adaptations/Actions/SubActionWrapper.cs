using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core.Actions;

    [System.Serializable]
    public class SubActionWrapper
    {
        [SerializeReference] public Action action;

        public SubActionWrapper(Action action)
        {
            this.action = action;
        }

        public SubActionWrapper()
        {
        }
    }
}
