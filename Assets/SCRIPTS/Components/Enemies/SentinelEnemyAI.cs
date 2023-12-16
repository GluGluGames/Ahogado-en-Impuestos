using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class SentinelEnemyAI : BehaviourRunner
    {
        public FSM SentinelFSM;

        private StateTransition DetectClosePlayer;
        private StateTransition NotifyEmited;
        private StateTransition Rested;

        public PushPerception DetectClosePlayerPush;
        public PushPerception NotifyEmitedPush;
        public PushPerception RestedPush;

        public Action StartPatrol;
        public Func<Status> UpdatePatrol;
        public Action StartNotify;
        public Func<Status> UpdateNotify;
        public Action StartSleep;
        public Func<Status> UpdateSleep;

        protected override BehaviourGraph CreateGraph()
        {
            SentinelFSM = new FSM();

            FunctionalAction Patrol_action = new FunctionalAction();
            Patrol_action.onStarted = StartPatrol;
            Patrol_action.onUpdated = UpdatePatrol;
            State Patrol = SentinelFSM.CreateState(Patrol_action);

            FunctionalAction Notify_action = new FunctionalAction();
            Notify_action.onStarted = StartNotify;
            Notify_action.onUpdated = UpdateNotify;
            State Notify = SentinelFSM.CreateState(Notify_action);

            FunctionalAction Sleep_action = new FunctionalAction();
            Sleep_action.onStarted = StartSleep;
            Sleep_action.onUpdated = UpdateSleep;
            State Sleep = SentinelFSM.CreateState(Sleep_action);

             DetectClosePlayer = SentinelFSM.CreateTransition(Patrol, Notify, statusFlags: StatusFlags.Success);
             NotifyEmited = SentinelFSM.CreateTransition(Notify, Sleep, statusFlags: StatusFlags.Success);
             Rested = SentinelFSM.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.Success);

            DetectClosePlayerPush = new PushPerception(DetectClosePlayer);
            NotifyEmitedPush = new PushPerception(NotifyEmited);
            RestedPush = new PushPerception(Rested);

            return SentinelFSM;
        }
    }
}