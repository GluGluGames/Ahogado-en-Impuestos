using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class MiniEnemyAI : BehaviourRunner
    {
        private StateTransition detectPlayer;
        private StateTransition detectBiggerEnemy;
        private StateTransition lostPatience;
        private StateTransition rested;
        private StateTransition distant;

        public PushPerception playerDetectedPush;
        public PushPerception detectBiggerEnemyPush;
        public PushPerception lostPatiencePush;
        public PushPerception restedPush;
        public PushPerception distantPush;

        public FSM miniEnemyBehaviour;


        public Action StartPatrol;
        public Func<Status> UpdatePatrol;
        public Action StartChase;
        public Func<Status> UpdateChase;
        public Action SleepMethod;
        public Action FleeMethod;

        protected override BehaviourGraph CreateGraph()
        {
            miniEnemyBehaviour = new FSM();

            FunctionalAction Patrol_action = new FunctionalAction();
            Patrol_action.onStarted = StartPatrol;
            Patrol_action.onUpdated = UpdatePatrol;
            State Patrol = miniEnemyBehaviour.CreateState(Patrol_action);

            FunctionalAction Chase_action = new FunctionalAction();
            Chase_action.onStarted = StartChase;
            Chase_action.onUpdated = UpdateChase;
            State Chase = miniEnemyBehaviour.CreateState(Chase_action);

            SimpleAction Sleep_action = new SimpleAction();
            Sleep_action.action = SleepMethod;
            State Sleep = miniEnemyBehaviour.CreateState(Sleep_action);

            SimpleAction Flee_action = new SimpleAction();
            Flee_action.action = FleeMethod;
            State Flee = miniEnemyBehaviour.CreateState(Flee_action);

            detectPlayer = miniEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

            detectBiggerEnemy = miniEnemyBehaviour.CreateTransition(Chase, Flee, statusFlags: StatusFlags.None);

            lostPatience = miniEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

            rested = miniEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

            distant = miniEnemyBehaviour.CreateTransition(Flee, Patrol, statusFlags: StatusFlags.None);


            playerDetectedPush = new PushPerception(detectPlayer);
            detectBiggerEnemyPush = new PushPerception(detectBiggerEnemy);
            lostPatiencePush = new PushPerception(lostPatience);
            restedPush= new PushPerception(rested);
            distantPush = new PushPerception(distant);

            return miniEnemyBehaviour;
        }
    }
}