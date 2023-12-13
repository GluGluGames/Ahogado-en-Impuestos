using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.StateMachines.StackFSMs;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class ChatarreroEnemyAI : BehaviourRunner
    {
        private StateTransition DetectedBetterResource;
        private StateTransition ResourceCollected;
        private StateTransition ResourceNotOnSight;
        private StateTransition DetectedBetterResourceOnMap;
        private PushTransition LostStaminaChasing;
        private PushTransition LostStaminaPatrolling;
        private PushTransition LostStaminaTaking;
        private PopTransition Rested;
        private StateTransition DetectedBetterResourceOnPlayer;
        private StateTransition StoleResource;

        public PushPerception DetectedBetterResourcePush;
        public PushPerception ResourceCollectedPush;
        public PushPerception ResourceNotOnSightPush;
        public PushPerception DetectedBetterResourceOnMapPush;
        public PushPerception LostStaminaPatrollingPush;
        public PushPerception LostStaminaChasingPush;
        public PushPerception LostStaminaTakingPush;
        public PushPerception RestedPush;
        public PushPerception DetectedBetterResourceOnPlayerPush;
        public PushPerception StoleResourcePush;

        public Action StartPatrol;
        public Func<Status> UpdatePatrol;
        public Action StartTaking;
        public Func<Status> UpdateTaking;
        public Action StartStealing;
        public Func<Status> UpdateStealing;
        public Func<Status> UpdateSleep;

        public BSRuntimeDebugger _bsRunTimeDebugger;

        protected override BehaviourGraph CreateGraph()
        {
            StackFSM ChatarreroFSMStack = new StackFSM();

            FunctionalAction PatrolAction = new FunctionalAction();
            PatrolAction.onStarted = StartPatrol;
            PatrolAction.onUpdated = UpdatePatrol;
            State Patrol = ChatarreroFSMStack.CreateState("Patrol", PatrolAction);

            FunctionalAction TakeAction = new FunctionalAction();
            TakeAction.onStarted = StartTaking;
            TakeAction.onUpdated = UpdateTaking;
            State Take = ChatarreroFSMStack.CreateState("Take", TakeAction);

            FunctionalAction StealAction = new FunctionalAction();
            StealAction.onStarted = StartStealing;
            StealAction.onUpdated = UpdateStealing;
            State Steal = ChatarreroFSMStack.CreateState("Steal", StealAction);

            FunctionalAction SleepAction = new FunctionalAction();
            SleepAction.onUpdated = UpdateSleep;
            State Sleep = ChatarreroFSMStack.CreateState("Sleep", SleepAction);

            DetectedBetterResource = ChatarreroFSMStack.CreateTransition(Patrol, Take, statusFlags: StatusFlags.Failure);
            ResourceCollected = ChatarreroFSMStack.CreateTransition(Take, Patrol, statusFlags: StatusFlags.None);
            ResourceNotOnSight = ChatarreroFSMStack.CreateTransition(Take, Patrol, statusFlags: StatusFlags.None);
            DetectedBetterResourceOnMap = ChatarreroFSMStack.CreateTransition(Steal, Take, statusFlags: StatusFlags.None);
            LostStaminaChasing = ChatarreroFSMStack.CreatePushTransition(Steal, Sleep, statusFlags: StatusFlags.None);
            LostStaminaPatrolling = ChatarreroFSMStack.CreatePushTransition(Patrol, Sleep, statusFlags: StatusFlags.Success);
            LostStaminaTaking = ChatarreroFSMStack.CreatePushTransition(Take, Sleep, statusFlags: StatusFlags.None);
            Rested = ChatarreroFSMStack.CreatePopTransition(Sleep, statusFlags: StatusFlags.Success);
            DetectedBetterResourceOnPlayer = ChatarreroFSMStack.CreateTransition(Patrol, Steal, statusFlags: StatusFlags.None);
            StoleResource = ChatarreroFSMStack.CreateTransition(Steal, Patrol, statusFlags: StatusFlags.None);

            DetectedBetterResourcePush = new PushPerception(DetectedBetterResource);
            ResourceCollectedPush = new PushPerception(ResourceCollected);
            ResourceNotOnSightPush = new PushPerception(ResourceNotOnSight);
            DetectedBetterResourceOnMapPush = new PushPerception(DetectedBetterResourceOnMap);
            LostStaminaPatrollingPush = new PushPerception(LostStaminaPatrolling);
            LostStaminaChasingPush = new PushPerception(LostStaminaChasing);
            LostStaminaTakingPush = new PushPerception(LostStaminaTaking);
            RestedPush = new PushPerception(Rested);
            DetectedBetterResourceOnPlayerPush = new PushPerception(DetectedBetterResourceOnPlayer);
            StoleResourcePush = new PushPerception(StoleResource);


            _bsRunTimeDebugger.RegisterGraph(ChatarreroFSMStack, "Chatarrrero FSM STACK");

            return ChatarreroFSMStack;
        }
    }
}