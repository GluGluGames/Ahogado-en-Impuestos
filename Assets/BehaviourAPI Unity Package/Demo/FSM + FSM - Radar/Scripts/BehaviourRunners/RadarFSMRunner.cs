using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class RadarFSMRunner : BehaviourRunner, IRadar
    {
        State _brokenState, _workingState;

        RadarDisplay _radarDisplay;

        BSRuntimeDebugger _debugger;
        protected override void Init()
        {
            _radarDisplay = GetComponent<RadarDisplay>();
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }

        protected override BehaviourGraph CreateGraph()
        {
            var radarFSM = new FSM();

            var fix = new UnityTimePerception(10f);
            var @break = new UnityTimePerception(20f);

            var subFSM = CreateLightSubFSM();
            var workingState = radarFSM.CreateState("working state", new SubsystemAction(subFSM));
            var brokenState = radarFSM.CreateState("broken state", new FunctionalAction(_radarDisplay.Break, _radarDisplay.Blink, _radarDisplay.Fix));

            // La FSM cambia de un estado a otro con el paso del tiempo:
            var @fixed = radarFSM.CreateTransition("fixed", brokenState, workingState, fix);
            var broken = radarFSM.CreateTransition("broken", workingState, brokenState, @break);

            _brokenState = brokenState;
            _workingState = workingState;

            _debugger.RegisterGraph(radarFSM, "Radar");
            _debugger.RegisterGraph(subFSM, "Lights");

            return radarFSM;
        }

        private FSM CreateLightSubFSM()
        {
            var lightSubFSM = new FSM();
            var overSpeedPerception = new ConditionPerception(() => _radarDisplay.CheckRadar((speed) => speed > 20));
            var underSpeedPerception = new ConditionPerception(() => _radarDisplay.CheckRadar((speed) => speed <= 20));

            var waitingState = lightSubFSM.CreateState("waiting", new LightAction(_radarDisplay.RadarLight, Color.blue));
            var overSpeedState = lightSubFSM.CreateState("over", new LightAction(_radarDisplay.RadarLight, Color.red, 1f));
            var underSpeedState = lightSubFSM.CreateState("under", new LightAction(_radarDisplay.RadarLight, Color.green, 1f));

            // Pasa al estado "over" o "under" si pasa un coche, según la velocidad que lleve
            lightSubFSM.CreateTransition("car over speed", waitingState, overSpeedState, overSpeedPerception);
            lightSubFSM.CreateTransition("car under speed", waitingState, underSpeedState, underSpeedPerception);

            // Vuelve al estado de espera al acabar la acción
            lightSubFSM.CreateTransition("over speed to waiting", overSpeedState, waitingState, statusFlags: StatusFlags.Finished);
            lightSubFSM.CreateTransition("under speed to waiting", underSpeedState, waitingState, statusFlags: StatusFlags.Finished);

            return lightSubFSM;
        }

        public State GetBrokenState() => _brokenState;

        public State GetWorkingState() => _workingState;


    }
}