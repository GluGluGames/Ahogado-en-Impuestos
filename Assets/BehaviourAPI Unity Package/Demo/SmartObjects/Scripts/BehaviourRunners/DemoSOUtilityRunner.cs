using BehaviourAPI.BehaviourTrees;
using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UnityToolkit;
using BehaviourAPI.UnityToolkit.GUIDesigner.Runtime;
using BehaviourAPI.UtilitySystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSOUtilityRunner : BehaviourRunner
{
    SmartAgent m_Agent;
    BSRuntimeDebugger m_Debugger;

    protected override void Init()
    {
        m_Agent = GetComponent<SmartAgent>();
        m_Debugger = GetComponent<BSRuntimeDebugger>();
        base.Init();
    }

    protected override BehaviourGraph CreateGraph()
    {
        UtilitySystem us = new UtilitySystem(1.2f);

        Factor leisureFactor = us.CreateVariable(() => m_Agent.GetNeed("leisure"), 1, 0);
        Factor restFactor = us.CreateVariable(() => m_Agent.GetNeed("rest"), 1, 0);
        Factor hygieneFactor = us.CreateVariable(() => m_Agent.GetNeed("hygiene"), 1, 0);
        Factor bladderFactor = us.CreateVariable(() => m_Agent.GetNeed("bladder"), 1, 0);
        Factor hungerFactor = us.CreateVariable(() => m_Agent.GetNeed("hunger"), 1, 0);
        Factor thirstFactor = us.CreateVariable(() => m_Agent.GetNeed("thirst"), 1, 0);

        Factor defaultFactor = us.CreateConstant(0.1f);

        us.CreateAction("leisure action", leisureFactor, CreateSmartObjectAction("leisure"));
        us.CreateAction("rest action", restFactor, CreateSmartObjectAction("rest"));
        us.CreateAction("hygiene action", hygieneFactor, CreateSmartObjectAction("hygiene"));
        us.CreateAction("bladder action", bladderFactor, CreateSmartObjectAction("bladder"));
        us.CreateAction("hunger action", hungerFactor, CreateSmartObjectAction("hunger"));
        us.CreateAction("thirst action", thirstFactor, CreateSmartObjectAction("thirst"));

        us.CreateAction("default action", defaultFactor, CreateDefaultAction());

        m_Debugger.RegisterGraph(us, "main");
        return us;
    }

    private Action CreateSmartObjectAction(string needName)
    {
        var bt = new BehaviourTree();
        bt.SetRootNode(
            bt.CreateDecorator<LoopNode>("loop",
                bt.CreateComposite<SelectorNode>("sel",
                    false,
                    bt.CreateLeafNode("request", new NeedRequestAction(needName)),
                    bt.CreateLeafNode("delay", new DelayAction(5f))
                )
            )
        );
        m_Debugger.RegisterGraph(bt, needName);
        return new SubsystemAction(bt);
    }

    private Action CreateDefaultAction()
    {
        var bt = new BehaviourTree();
        bt.SetRootNode(bt.CreateDecorator<LoopNode>(bt.CreateLeafNode(new PatrolAction(10f))));
        return new SubsystemAction(bt);
    }
}
