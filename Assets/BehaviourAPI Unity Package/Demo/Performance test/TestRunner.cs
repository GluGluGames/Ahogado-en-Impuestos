using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRunner : BehaviourRunner
{
    protected override BehaviourGraph CreateGraph()
    {
        float delay = Random.Range(0f, 5f);
        BehaviourTree tree = new BehaviourTree();
        var leaf1 = tree.CreateLeafNode(new PatrolAction(10f));
        var leaf2 = tree.CreateLeafNode(new DelayAction(delay));
        var seq = tree.CreateComposite<SequencerNode>(false, leaf1, leaf2);
        var loop = tree.CreateDecorator<LoopNode>(seq);
        tree.SetRootNode(loop);
        return tree;
    }
}
