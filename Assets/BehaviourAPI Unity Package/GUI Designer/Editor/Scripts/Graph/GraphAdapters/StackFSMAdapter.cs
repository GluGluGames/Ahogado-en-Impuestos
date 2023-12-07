using BehaviourAPI.StateMachines.StackFSMs;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    [CustomGraphAdapter(typeof(StackFSM))]
    public class StackFSMAdapter : StateMachineAdapter
    {
        public override string IconPath => BehaviourAPISettings.instance.IconPath + "Graphs/stackfsm.png";
    }
}
