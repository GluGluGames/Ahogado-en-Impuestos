using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UnityToolkit;
using System.Net.Http;

namespace GGG.Components.Enemies
{
    public class BehaviourTreeOnNotified : BehaviourRunner
    {
        protected override BehaviourGraph CreateGraph()
        {
            BehaviourTree newbehaviourgraph = new BehaviourTree();

            FunctionalAction action = new FunctionalAction();
            LeafNode CheckOnDestination = newbehaviourgraph.CreateLeafNode(null /* missing action */);

            LeafNode ChaseExit = newbehaviourgraph.CreateLeafNode(null /* missing action */);

            ConditionNode ConditionSeeNode = newbehaviourgraph.CreateDecorator<ConditionNode>(ChaseExit);

            LeafNode MoveClose = newbehaviourgraph.CreateLeafNode(null /* missing action */);

            ConditionNode ConditionKeepSearching = newbehaviourgraph.CreateDecorator<ConditionNode>(MoveClose);

            LeafNode PatrolExit = newbehaviourgraph.CreateLeafNode(null /* missing action */);

            SelectorNode Selector2 = newbehaviourgraph.CreateComposite<SelectorNode>(false, ConditionSeeNode, ConditionKeepSearching, PatrolExit);
            Selector2.IsRandomized = false;

            SequencerNode Sequencer1 = newbehaviourgraph.CreateComposite<SequencerNode>(false, CheckOnDestination, Selector2);
            Sequencer1.IsRandomized = false;

            LeafNode WalkToDestination = newbehaviourgraph.CreateLeafNode(null /* missing action */);

            SelectorNode Selector1 = newbehaviourgraph.CreateComposite<SelectorNode>(false, Sequencer1, WalkToDestination);
            Selector1.IsRandomized = false;

            LoopNode BasicLoop = newbehaviourgraph.CreateDecorator<LoopNode>(Selector1);
            BasicLoop.Iterations = -1;

            return newbehaviourgraph;
        }
    }
}