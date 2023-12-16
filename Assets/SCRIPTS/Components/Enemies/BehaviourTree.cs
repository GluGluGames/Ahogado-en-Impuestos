using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.Core.Perceptions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using System;
using System.Net.Http;
using Action = System.Action;

namespace GGG.Components.Enemies
{
    public class BehaviourTreeOnNotified : BehaviourRunner
    {

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


        protected override BehaviourGraph CreateGraph()
        {
            BehaviourTree newbehaviourgraph = new BehaviourTree();

            FunctionalAction CheckOnDestinationAction = new FunctionalAction();
            CheckOnDestinationAction.onStarted = StartCheckOnDestination;
            CheckOnDestinationAction.onUpdated = UpdateCheckOnDestination;
            LeafNode CheckOnDestination = newbehaviourgraph.CreateLeafNode(CheckOnDestinationAction);

            FunctionalAction ChaseExitAction = new FunctionalAction();
            ChaseExitAction.onStarted = StartChaseExit;
            ChaseExitAction.onUpdated = UpdateChaseExit;
            LeafNode ChaseExit = newbehaviourgraph.CreateLeafNode(ChaseExitAction);

            ConditionNode ConditionSeeNode = newbehaviourgraph.CreateDecorator<ConditionNode>(ChaseExit);
            ConditionSeeNode.Perception = new ConditionPerception(ConditionSeeNodeCheck);

            FunctionalAction MoveCloseAction = new FunctionalAction();
            MoveCloseAction.onStarted = StartMoveClose;
            MoveCloseAction.onUpdated = UpdateMoveClose;
            LeafNode MoveClose = newbehaviourgraph.CreateLeafNode(MoveCloseAction);

            ConditionNode ConditionKeepSearching = newbehaviourgraph.CreateDecorator<ConditionNode>(MoveClose);
            ConditionKeepSearching.Perception = new ConditionPerception(ConditionKeepSearchingCheck);

            FunctionalAction PatrolExitAction = new FunctionalAction();
            PatrolExitAction.onStarted = StartPatrolExit;
            PatrolExitAction.onUpdated = UpdatePatrolExit;
            LeafNode PatrolExit = newbehaviourgraph.CreateLeafNode(PatrolExitAction);

            SelectorNode Selector2 = newbehaviourgraph.CreateComposite<SelectorNode>(false, ConditionSeeNode, ConditionKeepSearching, PatrolExit);
            Selector2.IsRandomized = false;

            SequencerNode Sequencer1 = newbehaviourgraph.CreateComposite<SequencerNode>(false, CheckOnDestination, Selector2);
            Sequencer1.IsRandomized = false;

            FunctionalAction WalkToDestinationAction = new FunctionalAction();
            WalkToDestinationAction.onStarted = StartWalkToDestination;
            WalkToDestinationAction.onUpdated = UpdateWalkToDestination;
            LeafNode WalkToDestination = newbehaviourgraph.CreateLeafNode(WalkToDestinationAction);

            SelectorNode Selector1 = newbehaviourgraph.CreateComposite<SelectorNode>(false, Sequencer1, WalkToDestination);
            Selector1.IsRandomized = false;

            LoopNode BasicLoop = newbehaviourgraph.CreateDecorator<LoopNode>(Selector1);
            BasicLoop.Iterations = -1;

            newbehaviourgraph.SetRootNode(BasicLoop);

            _bsRunTimeDebugger.RegisterGraph(newbehaviourgraph, "DETECTADO BT");

            return newbehaviourgraph;
        }
    }
}