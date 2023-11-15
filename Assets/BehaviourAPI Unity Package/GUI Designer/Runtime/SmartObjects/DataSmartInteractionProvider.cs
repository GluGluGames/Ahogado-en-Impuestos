using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Core;
    using Core.Actions;
    using Framework;

    public abstract class DataSmartInteractionProvider : SmartInteractionProvider
    {
        protected override Action GetInteractionAction(SmartAgent agent)
        {
            SystemData data = GetEditedSystemdata();
            BSBuildingResults buildedData = data.BuildSystem(agent);
            ModifyGraphs(buildedData.GraphMap, buildedData.PushPerceptionMap);
            return new SubsystemAction(buildedData.MainGraph);
        }

        protected abstract SystemData GetEditedSystemdata();

        protected virtual void ModifyGraphs(Dictionary<string, BehaviourGraph> graphs, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            return;
        }

    }
}
