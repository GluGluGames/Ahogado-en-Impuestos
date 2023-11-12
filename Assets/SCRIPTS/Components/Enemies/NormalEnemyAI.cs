using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using System;
using UnityEngine;

public class NormalEnemyAI : BehaviourRunner
{

    private StateTransition DetectPlayer;
    private StateTransition LostPatience;
    private StateTransition Rested;
    private ExitTransition exit;

    public PushPerception playerDetected;
    public PushPerception playerLost;

    public System.Action StartPatrol;
    public System.Func<Status> UpdatePatrol;
    
    public System.Action StartChase;
    public System.Func<Status> UpdateChase;
    public System.Action StartSleep;


    protected override BehaviourGraph CreateGraph()
    {
        FSM NormalEnemyBehaviour = new FSM();

        FunctionalAction Patrol_action = new FunctionalAction();
        Patrol_action.onStarted = StartPatrol;
        Patrol_action.onUpdated = UpdatePatrol;
        State Patrol = NormalEnemyBehaviour.CreateState(Patrol_action);

        FunctionalAction Chase_action = new FunctionalAction();
        Chase_action.onStarted = StartChase;
        Chase_action.onUpdated = UpdateChase;
        State Chase = NormalEnemyBehaviour.CreateState(Chase_action);

        exit = NormalEnemyBehaviour.CreateExitTransition(Chase, Status.None, statusFlags: StatusFlags.None);

        FunctionalAction Sleep_action = new FunctionalAction();
        Sleep_action.onStarted = StartSleep;
        Sleep_action.onUpdated = () => Status.Running;
        State Sleep = NormalEnemyBehaviour.CreateState(Sleep_action);

        DetectPlayer = NormalEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

        LostPatience = NormalEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

        Rested = NormalEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);

        playerDetected = new PushPerception(DetectPlayer);
        playerLost = new PushPerception(LostPatience);
        return NormalEnemyBehaviour;
    }
}