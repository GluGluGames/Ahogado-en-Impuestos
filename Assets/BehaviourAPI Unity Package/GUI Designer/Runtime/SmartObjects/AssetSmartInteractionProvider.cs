using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Core;
    using Core.Actions;
    using Framework;

    public class AssetSmartInteractionProvider : DataSmartInteractionProvider
    {
        [SerializeField] BehaviourSystem _behaviourSystem;

        protected override Action GetInteractionAction(SmartAgent agent)
        {
            SystemData data = _behaviourSystem.GetBehaviourSystemData();
            BehaviourGraph graph = data.BuildSystem(agent).MainGraph;
            return new SubsystemAction(graph);
        }

        protected override SystemData GetEditedSystemdata() => _behaviourSystem.GetBehaviourSystemData();
    }
}
