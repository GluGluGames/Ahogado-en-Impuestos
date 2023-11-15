using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class CarFSMAssetRunner : AssetBehaviourRunner, ICar
    {
        Rigidbody _rb;

        IRadar m_Radar;
        protected override void Init()
        {
            _rb = GetComponent<Rigidbody>();
            m_Radar = GameObject.FindGameObjectWithTag("Radar").GetComponent<IRadar>();
            base.Init();
        }

        protected override void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            var mainGraph = graphMap["main"];
            var speedUp = mainGraph.FindNode<StateTransition>("speed up");
            var speedDown = mainGraph.FindNode<StateTransition>("speed down");

            speedUp.Perception = new ExecutionStatusPerception(m_Radar.GetBrokenState(), StatusFlags.Running);
            speedDown.Perception = new ExecutionStatusPerception(m_Radar.GetWorkingState(), StatusFlags.Running);
        }

        public float GetSpeed() => _rb.velocity.magnitude;
    }

}