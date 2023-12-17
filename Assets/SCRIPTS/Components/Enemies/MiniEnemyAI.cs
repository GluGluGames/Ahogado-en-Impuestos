using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.StateMachines;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using static Codice.Client.BaseCommands.Import.Commit;
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

        #region BT

        public StateTransition Notified;
        public StateTransition EnemyFoundWhileBT;
        public StateTransition EnemyNotFoundWhileBT;

        public PushPerception EnemyFoundWhileBTPush;
        public PushPerception EnemyNotFoundWhileBTPush;
        public PushPerception NotifiedPush;

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

        public Func<bool> ConditionSeePlayerCheck;
        public Func<bool> ConditionKeepSearchingCheck;

        public BSRuntimeDebugger _bsRunTimeDebugger;

        #endregion BT

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
            ConditionSeeNode.Perception = new ConditionPerception(ConditionSeePlayerCheck);

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
            State NotifiedState = miniEnemyBehaviour.CreateState("BT", notifiedAction);

            Notified = miniEnemyBehaviour.CreateTransition(Patrol, NotifiedState, statusFlags: StatusFlags.Success);
            NotifiedPush = new PushPerception(Notified);

            EnemyFoundWhileBT = miniEnemyBehaviour.CreateTransition(NotifiedState, Chase, statusFlags: StatusFlags.None);
            EnemyNotFoundWhileBT = miniEnemyBehaviour.CreateTransition(NotifiedState, Patrol, statusFlags: StatusFlags.None);

            EnemyFoundWhileBTPush = new PushPerception(EnemyFoundWhileBT);
            EnemyNotFoundWhileBTPush = new PushPerception(EnemyNotFoundWhileBT);

            _bsRunTimeDebugger.RegisterGraph(miniEnemyBehaviour, "DETECTADO BT");



            return miniEnemyBehaviour;
        }
    }
}