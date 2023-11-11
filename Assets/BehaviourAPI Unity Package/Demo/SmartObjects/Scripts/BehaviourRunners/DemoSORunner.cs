using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using BehaviourAPI.UnityToolkit;

public class DemoSORunner : BehaviourRunner
{
    private SmartAgent _agent;

    BSRuntimeDebugger _debugger;

    protected override void Init()
    {
        _agent = GetComponent<SmartAgent>();
        _debugger = GetComponent<BSRuntimeDebugger>();
        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
    {
        BehaviourTree bt = new BehaviourTree();
        RandomRequestAction randomRequestAction = new RandomRequestAction(_agent);
        LeafNode leaf = bt.CreateLeafNode(randomRequestAction);
        LoopNode root = bt.CreateDecorator<LoopNode>(leaf);
        bt.SetRootNode(root);

        _debugger.RegisterGraph(bt, "main");
        return bt;
    }
}
