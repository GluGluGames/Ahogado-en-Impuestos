using System;
using System.Collections.Generic;
using UnityEngine;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.StateMachines;

public class NormalEnemyBehaviour : BehaviourRunner
{
	private void StartPatrol()
	{
		throw new System.NotImplementedException();
	}
	
	private void StartChase()
	{
		throw new System.NotImplementedException();
	}
	
	private Status UpdateChase()
	{
		throw new System.NotImplementedException();
	}
	
	private void Sleep()
	{
		throw new System.NotImplementedException();
	}

    protected override BehaviourGraph CreateGraph()
    {
        FSM NormalEnemyBehaviour = new FSM();

        FunctionalAction Patrol_action = new FunctionalAction();
        Patrol_action.onStarted = StartPatrol;
        Patrol_action.onUpdated = () => Status.Running;
        State Patrol = NormalEnemyBehaviour.CreateState(Patrol_action);

        FunctionalAction Chase_action = new FunctionalAction();
        Chase_action.onStarted = StartChase;
        Chase_action.onUpdated = UpdateChase;
        State Chase = NormalEnemyBehaviour.CreateState(Chase_action);

        ExitTransition unnamed = NormalEnemyBehaviour.CreateExitTransition(Chase, Status.None, statusFlags: StatusFlags.None);

        FunctionalAction Sleep_action = new FunctionalAction();
        Sleep_action.onStarted = this.Sleep;
        Sleep_action.onUpdated = () => Status.Running;
        State Sleep = NormalEnemyBehaviour.CreateState(Sleep_action);

        StateTransition DetectarJugador = NormalEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

        StateTransition LostPatience = NormalEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

        StateTransition Rested = NormalEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

        return NormalEnemyBehaviour;
    }
}
