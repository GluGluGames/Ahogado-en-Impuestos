using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;
using System.Runtime.CompilerServices;
using Action = System.Action;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;

namespace GGG.Components.Enemies
{
	public class BerserkerEnemyAI : BehaviourRunner
	{
		private StateTransition detectPlayer;
		private StateTransition lostPatience;
		private StateTransition killedNonPlayer;
		private StateTransition rested;

        public PushPerception detectPlayerPush;
        public PushPerception lostPatiencePush;
        public PushPerception killedNonPlayerPush;
        public PushPerception restedPush;

        public FSM berserkerEnemyBehaviour;

        public Action StartPatrol;
        public Func<Status> UpdatePatrol;
        public Action StartChase;
        public Func<Status> UpdateChase;
        public Action StartBerserker;
        public Func<Status> UpdateBerserker;
		public Action SleepMethod;

        [SerializeField] public BSRuntimeDebugger bSRuntimeDebugger;

        protected override BehaviourGraph CreateGraph()
		{
			berserkerEnemyBehaviour = new FSM();
			
			FunctionalAction Patrol_action = new FunctionalAction();
			Patrol_action.onStarted = StartPatrol;
			Patrol_action.onUpdated = UpdatePatrol;
			State Patrol = berserkerEnemyBehaviour.CreateState("patrol", Patrol_action);
			
			FunctionalAction Chase_action = new FunctionalAction();
			Chase_action.onStarted = StartChase;
			Chase_action.onUpdated = UpdateChase;
			State Chase = berserkerEnemyBehaviour.CreateState("chase", Chase_action);
			
			FunctionalAction Berserker_action = new FunctionalAction();
			Berserker_action.onStarted = StartBerserker;
			Berserker_action.onUpdated = UpdateBerserker;
			State Berserker = berserkerEnemyBehaviour.CreateState("berserker", Berserker_action);
			
			SimpleAction Sleep_action = new SimpleAction(SleepMethod);
			State Sleep = berserkerEnemyBehaviour.CreateState("sleep", Sleep_action);
			
			 detectPlayer = berserkerEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);
			 lostPatience = berserkerEnemyBehaviour.CreateTransition(Chase, Berserker, statusFlags: StatusFlags.None);
			 killedNonPlayer = berserkerEnemyBehaviour.CreateTransition(Berserker, Sleep, statusFlags: StatusFlags.None);
			 rested = berserkerEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

            detectPlayerPush = new PushPerception(detectPlayer);
            lostPatiencePush = new PushPerception(lostPatience);
            killedNonPlayerPush = new PushPerception(killedNonPlayer);
            restedPush = new PushPerception(rested);

            bSRuntimeDebugger.RegisterGraph(berserkerEnemyBehaviour, "berserker fsm");

            return berserkerEnemyBehaviour;
		}
	}
}
