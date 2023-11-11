using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class CarFSMRunner : BehaviourRunner, ICar
    {
        Rigidbody _rb;
        IRadar _radar;

        float _speed;

        BSRuntimeDebugger _debugger;

        protected override void Init()
        {
            _rb = GetComponent<Rigidbody>();
            _radar = FindObjectOfType<RadarFSMRunner>();
            _speed = Random.Range(10f, 30f);
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }
        protected override BehaviourGraph CreateGraph()
        {
            var carFSM = new BehaviourAPI.StateMachines.FSM();

            // Esta percepción se activará cuando el estado del radar sea "broken"
            var radarIsBroken = new ExecutionStatusPerception(_radar.GetBrokenState());

            // Esta percepción se activará cuando el estado del radar sea "working"
            var radarIsWorking = new ExecutionStatusPerception(_radar.GetWorkingState());

            var lowSpeedState = carFSM.CreateState("low speed", new FunctionalAction(() => _rb.velocity = transform.forward * _speed));
            var highSpeedState = carFSM.CreateState("high speed", new FunctionalAction(() => _rb.velocity = transform.forward * (_speed + 10f)));

            // La FSM cambia de un estado a otro con el paso del tiempo:
            carFSM.CreateTransition("speed up", lowSpeedState, highSpeedState, radarIsBroken);
            carFSM.CreateTransition("slow down", highSpeedState, lowSpeedState, radarIsWorking);

            _debugger.RegisterGraph(carFSM);
            return carFSM;
        }

        public float GetSpeed() => _rb.velocity.magnitude;
    }

}