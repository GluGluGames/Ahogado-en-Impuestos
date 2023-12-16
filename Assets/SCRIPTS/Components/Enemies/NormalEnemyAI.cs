using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class NormalEnemyAI : BehaviourRunner
    {
        public StateTransition DetectPlayer;
        public StateTransition LostPatience;
        public StateTransition Rested;
        public StateTransition Notified;

        public ExitTransition exit;

        public PushPerception playerDetected;
        public PushPerception playerLost;
        public PushPerception RestedPush;
        public PushPerception NotifiedPush;

        public System.Action StartPatrol;
        public System.Func<Status> UpdatePatrol;

        public System.Action StartChase;
        public System.Func<Status> UpdateChase;
        public System.Action StartSleep;

        public float sleepTime = 2.0f;

        public FSM NormalEnemyBehaviour = new FSM();



        #region BT
        public Action StartCheckOnDestination;
        public Func<Status> UpdateCheckOnDestination;

        public Action StartChaseExit;
        public Func<Status> UpdateChaseExit;

        public Action StartMoveClose;
        public Func<Status> UpdateMoveClose;

        public Action StartPatrolExit;
        public Func<Status> UpdatePatrolExit;

        public Action StartWalkToDestination;
        public Func<Status> UpdateWalkToDestination;

        public Func<bool> ConditionSeeNodeCheck;
        public Func<bool> ConditionKeepSearchingCheck;

        public BSRuntimeDebugger _bsRunTimeDebugger;


        #endregion

        protected override BehaviourGraph CreateGraph()
        {
            FunctionalAction Patrol_action = new FunctionalAction();
            Patrol_action.onStarted = StartPatrol;
            Patrol_action.onUpdated = UpdatePatrol;
            State Patrol = NormalEnemyBehaviour.CreateState("patrol", Patrol_action);

            FunctionalAction Chase_action = new FunctionalAction();
            Chase_action.onStarted = StartChase;
            Chase_action.onUpdated = UpdateChase;
            State Chase = NormalEnemyBehaviour.CreateState("chase", Chase_action);

            exit = NormalEnemyBehaviour.CreateExitTransition(Chase, Status.None, statusFlags: StatusFlags.None);

            FunctionalAction Sleep_action = new FunctionalAction();

            Sleep_action.onStarted = StartSleep;
            State Sleep = NormalEnemyBehaviour.CreateState("sleep", Sleep_action);

            DetectPlayer = NormalEnemyBehaviour.CreateTransition(Patrol, Chase, statusFlags: StatusFlags.None);

            LostPatience = NormalEnemyBehaviour.CreateTransition(Chase, Sleep, statusFlags: StatusFlags.None);

            Rested = NormalEnemyBehaviour.CreateTransition(Sleep, Patrol, statusFlags: StatusFlags.None);


            playerDetected = new PushPerception(DetectPlayer);
            playerLost = new PushPerception(LostPatience);
            RestedPush = new PushPerception(Rested);

            #region BT
            BehaviourTree subBehaviourTree = new BehaviourTree();

            FunctionalAction CheckOnDestinationAction = new FunctionalAction();
            CheckOnDestinationAction.onStarted = StartCheckOnDestination;
            CheckOnDestinationAction.onUpdated = UpdateCheckOnDestination;
            LeafNode CheckOnDestination = subBehaviourTree.CreateLeafNode(CheckOnDestinationAction);

            FunctionalAction ChaseExitAction = new FunctionalAction();
            ChaseExitAction.onStarted = StartChaseExit;
            ChaseExitAction.onUpdated = UpdateChaseExit;
            LeafNode ChaseExit = subBehaviourTree.CreateLeafNode(ChaseExitAction);

            ConditionNode ConditionSeeNode = subBehaviourTree.CreateDecorator<ConditionNode>(ChaseExit);
            ConditionSeeNode.Perception = new ConditionPerception(ConditionSeeNodeCheck);

            FunctionalAction MoveCloseAction = new FunctionalAction();
            MoveCloseAction.onStarted = StartMoveClose;
            MoveCloseAction.onUpdated = UpdateMoveClose;
            LeafNode MoveClose = subBehaviourTree.CreateLeafNode(MoveCloseAction);

            ConditionNode ConditionKeepSearching = subBehaviourTree.CreateDecorator<ConditionNode>(MoveClose);
            ConditionKeepSearching.Perception = new ConditionPerception(ConditionKeepSearchingCheck);

            FunctionalAction PatrolExitAction = new FunctionalAction();
            PatrolExitAction.onStarted = StartPatrolExit;
            PatrolExitAction.onUpdated = UpdatePatrolExit;
            LeafNode PatrolExit = subBehaviourTree.CreateLeafNode(PatrolExitAction);

            SelectorNode Selector2 = subBehaviourTree.CreateComposite<SelectorNode>(false, ConditionSeeNode, ConditionKeepSearching, PatrolExit);
            Selector2.IsRandomized = false;

            SequencerNode Sequencer1 = subBehaviourTree.CreateComposite<SequencerNode>(false, CheckOnDestination, Selector2);
            Sequencer1.IsRandomized = false;

            FunctionalAction WalkToDestinationAction = new FunctionalAction();
            WalkToDestinationAction.onStarted = StartWalkToDestination;
            WalkToDestinationAction.onUpdated = UpdateWalkToDestination;
            LeafNode WalkToDestination = subBehaviourTree.CreateLeafNode(WalkToDestinationAction);

            SelectorNode Selector1 = subBehaviourTree.CreateComposite<SelectorNode>(false, Sequencer1, WalkToDestination);
            Selector1.IsRandomized = false;

            LoopNode BasicLoop = subBehaviourTree.CreateDecorator<LoopNode>(Selector1);
            BasicLoop.Iterations = -1;

            subBehaviourTree.SetRootNode(BasicLoop);
            #endregion

            SubsystemAction notifiedAction = new SubsystemAction(subBehaviourTree);
            State NotifiedState= NormalEnemyBehaviour.CreateState("BT", notifiedAction);

            Notified = NormalEnemyBehaviour.CreateTransition(Patrol, NotifiedState, statusFlags: StatusFlags.None);
            NotifiedPush = new PushPerception(Notified);

            _bsRunTimeDebugger.RegisterGraph(NormalEnemyBehaviour, "DETECTADO BT");

            return NormalEnemyBehaviour;
        }
    }
}