using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using System;
    using UtilitySystems;
    [CustomNodeDrawer(typeof(UtilityNode))]
    public class UtilityNodeDrawer : NodeDrawer
    {
        PortView InputPort, OutputPort;

        private FunctionDisplay m_FunctionDisplay;

        ProgressBar m_UtilityProgressBar;

        public override void DrawNodeDetails()
        {
            switch (node)
            {
                case VariableFactor:
                    view.SetColor(BehaviourAPISettings.instance.LeafFactorColor);
                    var functionTaskView = view.AddExtensionView("$VariableFactor");
                    break;
                case ConstantFactor:
                    view.SetColor(BehaviourAPISettings.instance.LeafFactorColor);
                    functionTaskView = view.AddExtensionView("$ConstantFactor");
                    break;
                case FusionFactor:
                    view.SetColor(BehaviourAPISettings.instance.FusionFactorColor);
                    view.SetIconText(node.TypeName().RemoveTermination("Factor").CamelCaseToSpaced());
                    break;

                case CurveFactor curve:
                    view.SetColor(BehaviourAPISettings.instance.CurveFactorColor);
                    view.SetIconText(node.TypeName().RemoveTermination("Factor").CamelCaseToSpaced());
                    functionTaskView = view.AddExtensionView("$CurveFactor");
                    functionTaskView.Label.Disable();
                    m_FunctionDisplay = new FunctionDisplay(curve.TestEvaluate);
                    functionTaskView.AddElement(m_FunctionDisplay);
                    break;

                case UtilityBucket:
                    view.SetColor(BehaviourAPISettings.instance.BucketColor);
                    break;

                case UtilityExecutableNode:
                    view.SetColor(BehaviourAPISettings.instance.SelectableNodeColor);
                    break;
            }

            if (view.graphView.IsRuntime && node is UtilityNode utilityHandler)
            {
                m_UtilityProgressBar = new ProgressBar()
                {
                    title = " ",
                    lowValue = 0,
                    highValue = 1,
                };

                m_UtilityProgressBar.value = utilityHandler.Utility;
                m_UtilityProgressBar.title = utilityHandler.Utility.ToString("0.000");

                utilityHandler.UtilityChanged += OnUtilityChanged;
                view.Details.Add(m_UtilityProgressBar);

                OnUtilityChanged(utilityHandler.Utility);
            }
        }

        private void OnUtilityChanged(float utility)
        {
            m_UtilityProgressBar.value = utility;
            m_UtilityProgressBar.title = utility.ToString("0.000");
        }

        public override void OnDestroy()
        {
            if (view.graphView.IsRuntime && node is UtilityNode utilityHandler)
            {
                utilityHandler.UtilityChanged -= OnUtilityChanged;
            }
        }

        public override PortView GetPort(NodeView nodeView, Direction direction)
        {
            if (direction == Direction.Input) return InputPort;
            else return OutputPort;
        }

        public override void SetUpPorts()
        {
            var portDiv = view.Q("node-ports");
            portDiv.style.flexDirection = FlexDirection.RowReverse;

            if (node == null || node.MaxInputConnections != 0)
            {
                InputPort = view.InstantiatePort(Direction.Input, EPortOrientation.Right);
            }
            else view.inputContainer.Disable();

            if (node == null || node.MaxOutputConnections != 0)
            {
                OutputPort = view.InstantiatePort(Direction.Output, EPortOrientation.Left);
            }
            else view.outputContainer.Disable();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);

            if (view.graphView.IsRuntime) return;

            evt.menu.AppendAction("Order childs by y position", _ => view.OrderChildNodes(n => n.position.y),
                view.data.childIds.Count > 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }

        public override void OnRefreshDisplay()
        {
            switch (node)
            {
                case VariableFactor variableFactor:
                    var functionTaskView = view.GetTaskView("$VariableFactor");
                    functionTaskView.Update($"[{variableFactor.min} - {variableFactor.max}]");
                    break;

                case ConstantFactor constant:
                    functionTaskView = view.GetTaskView("$ConstantFactor");
                    functionTaskView.Update(constant.value.ToString());
                    break;

                case CustomCurveFactor:
                    m_FunctionDisplay?.Disable();
                    break;

                case CurveFactor:
                    m_FunctionDisplay?.Update();
                    break;
            }
        }

        private class FunctionDisplay : VisualElement
        {
            private static readonly int k_FunctionIntervals = 50;
            private static readonly int k_FunctionThickness = 2;
            private static readonly Color k_FunctionColor = Color.red;

            private VisualElement k_Display;

            public FunctionDisplay(Func<float, float> evaluationMethod)
            {
                var asset = BehaviourAPISettings.instance.GetLayoutAsset("Elements/functiondisplay.uxml");
                asset.CloneTree(this);

                k_Display = this.Q("fd-main");
                k_Display.generateVisualContent += (mgc) => OnGenerateVisualContent(evaluationMethod, mgc);
            }

            public void Update() => k_Display.MarkDirtyRepaint();

            private void OnGenerateVisualContent(Func<float, float> evaluationMethod, MeshGenerationContext mgc)
            {
                //var t = DateTime.Now;
                var width = k_Display.resolvedStyle.width;
                var height = k_Display.resolvedStyle.height - k_FunctionThickness;

                float delta = 1f / k_FunctionIntervals;

                var mesh = mgc.Allocate(k_FunctionIntervals * 2 + 2, k_FunctionIntervals * 6);
                for (int i = 0; i <= k_FunctionIntervals; i++)
                {
                    var x = delta * i;
                    var value = evaluationMethod(x);
                    value = Mathf.Clamp01(value);

                    mesh.SetNextVertex(new Vertex() { position = new Vector3(x * width, (1 - value) * height, Vertex.nearZ), tint = k_FunctionColor });
                    mesh.SetNextVertex(new Vertex() { position = new Vector3(x * width, (1 - value) * height + k_FunctionThickness, Vertex.nearZ), tint = k_FunctionColor });
                }

                for (ushort i = 0; i < k_FunctionIntervals * 2; i++)
                {
                    if (i % 2 == 0)
                    {
                        mesh.SetNextIndex(i);
                        mesh.SetNextIndex((ushort)(i + 2));
                        mesh.SetNextIndex((ushort)(i + 1));
                    }
                    else
                    {
                        mesh.SetNextIndex(i);
                        mesh.SetNextIndex((ushort)(i + 1));
                        mesh.SetNextIndex((ushort)(i + 2));
                    }
                }
                //Debug.Log((DateTime.Now - t).TotalMilliseconds);
            }
        }
    }
}
