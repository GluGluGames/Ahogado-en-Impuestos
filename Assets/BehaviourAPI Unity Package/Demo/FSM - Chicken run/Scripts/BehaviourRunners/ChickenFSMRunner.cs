using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using BehaviourAPI.UnityToolkit;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class ChickenFSMRunner : BehaviourRunner
    {
        #region variables

        [SerializeField] Transform _target;
        [SerializeField] Collider _visionCollider;
        NavMeshAgent _agent;

        #endregion variables

        BSRuntimeDebugger _debugger;
        protected override void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _debugger = GetComponent<BSRuntimeDebugger>();
            base.Init();
        }


        protected override BehaviourGraph CreateGraph()
        {
            var fsm = new BehaviourAPI.StateMachines.FSM();

            // Percepciones pull
            var watchPlayer = new ConditionPerception(CheckWatchTarget);
            var timeToStartMoving = new UnityTimePerception(3f);

            // Estados
            var idle = fsm.CreateState("Idle");
            var moving = fsm.CreateState("Moving", new PatrolAction(13f));
            var chasing = fsm.CreateState("Chasing", new ChaseAction(_target, 1.2f, 1f, 2f));

            // Las transiciones que pasan al estado moving se lanzan con un temporizador:
            var idleToMoving = fsm.CreateTransition("idle to moving", idle, moving, timeToStartMoving);

            // Las transiciones que pasan al estado "idle" se lanzan cuando la acción "moving" o "chase" termine.
            fsm.CreateTransition("moving to idle", moving, idle, statusFlags: StatusFlags.Finished);
            fsm.CreateTransition("runaway to idle", chasing, idle, statusFlags: StatusFlags.Finished);

            // Las transiciones que pasan al estado "chasing" se activan con la percepción "watchPlayer".
            fsm.CreateTransition("idle to runaway", idle, chasing, watchPlayer);
            fsm.CreateTransition("moving to runaway", moving, chasing, watchPlayer);

            _debugger.RegisterGraph(fsm);
            return fsm;
        }

        private bool CheckWatchTarget()
        {
            if (_visionCollider.bounds.Contains(_target.position))
            {
                Vector3 direction = (_target.position - transform.position).normalized;
                Ray ray = new Ray(transform.position + transform.up, direction * 20);

                bool watchPlayer = Physics.Raycast(ray, out RaycastHit hit, 20) && hit.collider.gameObject.transform == _target;

                return watchPlayer;
            }
            return false;
        }
    }
}