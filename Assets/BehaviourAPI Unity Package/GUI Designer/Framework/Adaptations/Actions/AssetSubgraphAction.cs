using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core.Actions;

    public class AssetSubgraphAction : SubsystemAction, IBuildable
    {
        [SerializeField] BehaviourSystem subgraph;

        [SerializeField] string name;

        public AssetSubgraphAction() : base(null)
        {
        }

        public void Build(BSBuildingInfo data)
        {
            if (subgraph == null) return;

            var runtimeData = subgraph.GetBehaviourSystemData();
            SubSystem = runtimeData.BuildSystem(data, name);
        }

        public override string ToString()
        {
            return $"Subgraph \"{name}\"";
        }

        public bool Validate(BSValidationInfo validationInfo)
        {
            if (!validationInfo.systemStack.Contains(subgraph.Data))
            {
                return subgraph.Data.CheckCyclicReferences(validationInfo);
            }
            else
            {
                return false; 
            }
        }
    }
}
