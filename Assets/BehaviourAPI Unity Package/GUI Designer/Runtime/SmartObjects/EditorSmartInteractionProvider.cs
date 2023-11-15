using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Framework;

    public class EditorSmartInteractionProvider : DataSmartInteractionProvider, IBehaviourSystem
    {
        [SerializeField] SystemData data;
        public SystemData Data => data;
        public Object ObjectReference => this;

        private BehaviourSystem _bSystem;

        private void Awake()
        {
            _bSystem = BehaviourSystem.CreateSystem(data);
        }

        protected override SystemData GetEditedSystemdata()
        {
            return _bSystem.GetBehaviourSystemData();
        }
    }
}
