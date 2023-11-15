using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class BerserkerEnemyAI : BehaviourRunner
    {
        public StateTransition detectPlayer;
        public StateTransition lostPatience;
        public StateTransition rested;
        public StateTransition killNonPlayer;
        public StateTransition lostPlayer;

        public PushPerception playerDetectedPush;
        public PushPerception lostPatiencePush;
        public PushPerception lostPlayerPush;
        public PushPerception restedPush;
        public PushPerception enemyKilledPush;

        public FSM berserkerEnemyBehaviour;

        public Action StartPatrol;
        public Func<Status> UpdatePatrol;
        public Action StartChase;
        public Func<Status> UpdateChase;
        public Action SleepMethod;
        public Action StartBerserker;
        public Func<Status> UpdateBerserker;

        public bool berserkerMode = false;

        protected override BehaviourGraph CreateGraph()
        {
            berserkerEnemyBehaviour = new FSM();

            FunctionalAction Patrol_action = new FunctionalAction();
            Patrol_action.onStarted = StartPatrol;
            Patrol_action.onUpdated = UpdatePatrol;
            State Patrol = berserkerEnemyBehaviour.CreateState(Patrol_action);

            FunctionalAction Chase_action = new FunctionalAction();
            Chase_action.onStarted = StartChase;
            Chase_action.onUpdated = UpdateChase;
            State Chase = berserkerEnemyBehaviour.CreateState(Chase_action);

            SimpleAction Sleep_action = new SimpleAction();
            Sleep_action.action = SleepMethod;
            State Sleep = berserkerEnemyBehaviour.CreateState(Sleep_action);

            detectPlayer = berserkerEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

            FunctionalAction Berserker_action = new FunctionalAction();
            Berserker_action.onStarted = StartBerserker;
            Berserker_action.onUpdated = UpdateBerserker;
            State Berserker = berserkerEnemyBehaviour.CreateState(Berserker_action);

            lostPatience = berserkerEnemyBehaviour.CreateTransition(Chase, Berserker, statusFlags: StatusFlags.None);

            lostPlayer = berserkerEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

            rested = berserkerEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

            killNonPlayer = berserkerEnemyBehaviour.CreateTransition(Berserker, Sleep, statusFlags: StatusFlags.None);


            playerDetectedPush = new PushPerception(detectPlayer);
            lostPatiencePush = new PushPerception(lostPatience);
            lostPlayerPush = new PushPerception(lostPlayer);
            restedPush = new PushPerception(rested);
            enemyKilledPush = new PushPerception(killNonPlayer);

            return berserkerEnemyBehaviour;
        }
    }
}