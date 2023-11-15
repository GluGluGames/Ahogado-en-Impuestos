using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class RadarFSMEditorRunner : EditorBehaviourRunner, IRadar
    {
        RadarDisplay _radarDisplay;

        State m_BrokenState;
        State m_WorkingState;

        protected override void Init()
        {
            _radarDisplay = GetComponent<RadarDisplay>();
            base.Init();
        }

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            var mainGraph = graphMap["Main"];
            m_BrokenState = mainGraph.FindNode<State>("broken");
            m_WorkingState = mainGraph.FindNode<State>("working");
        }

        public State GetBrokenState() => m_BrokenState;

        public State GetWorkingState() => m_WorkingState;

        public bool CheckRadarForOverSpeed()
        {
            return _radarDisplay.CheckRadar((speed) => speed > 20);
        }

        public bool CheckRadarForUnderSpeed()
        {
            return _radarDisplay.CheckRadar((speed) => speed <= 20);
        }
    }

}