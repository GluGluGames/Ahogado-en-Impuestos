using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class PlayerFSMRunner : BehaviourRunner
    {
        #region variables

        [SerializeField] private float minDistanceToChicken = 5;
        [SerializeField] private Transform chicken;
        [SerializeField] private Transform restartPoint;

        private NavMeshAgent meshAgent;
        private PushPerception _click;

        #endregion variables

        BSRuntimeDebugger _debugger;

        protected override void Init()
        {
            meshAgent = GetComponent<NavMeshAgent>();
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }

        protected override BehaviourGraph CreateGraph()
        {
            var fsm = new BehaviourAPI.StateMachines.FSM();

            // Percepciones
            var chickenNear = new ConditionPerception(CheckDistanceToChicken);

            // Estados
            var idle = fsm.CreateState("Idle");
            var moving = fsm.CreateState("Moving", new MoveToMousePosAction());
            var flee = fsm.CreateState("Flee", new FleeAction(chicken, 2.5f, 15f, 5f));

            // Las transiciones que pasan al estado "moving" se activan con percepciones Push.
            var idleToMoving = fsm.CreateTransition("idle to moving", idle, moving, statusFlags: StatusFlags.None);
            var movingToMoving = fsm.CreateTransition("moving to moving", moving, moving, statusFlags: StatusFlags.None);
            
            // El orden de los elementos es importante: la transición moving-moving se activará antes que la transición idle-moving
            _click = new PushPerception(movingToMoving, idleToMoving);

            // La transición que pasan al estado "idle" se lanzan cuando la acción del estado anterior termine.
            fsm.CreateTransition("moving to idle", moving, idle, statusFlags: StatusFlags.Finished);
            fsm.CreateTransition("runaway to idle", flee, idle, statusFlags: StatusFlags.Finished);

            // Las transiciones que pasan al estado "flee" se activan con la percepción "chicken near"
            fsm.CreateTransition("idle to runaway", idle, flee, chickenNear);
            fsm.CreateTransition("moving to runaway", moving, flee, chickenNear);

            _debugger.RegisterGraph(fsm);
            return fsm;
        }

        protected override void OnUpdated()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _click.Fire();
            }
            base.OnUpdated();
        }

        private bool CheckDistanceToChicken()
        {
            return Vector3.Distance(transform.position, chicken.transform.position) < minDistanceToChicken;
        }

        public void Restart()
        {
            transform.position = restartPoint.position;
        }
    }
}