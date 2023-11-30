using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using Core;
    using BehaviourTrees;
    using Framework;

    [CustomNodeDrawer(typeof(BTNode))]
    public class BTNodeDrawer : NodeDrawer
    {
        PortView InputPort, OutputPort;

        VisualElement rootIcon;

        public override void DrawNodeDetails()
        {
            rootIcon = view.Q("node-root");
            switch (node)
            {
                case LeafNode:
                    view.SetColor(BehaviourAPISettings.instance.LeafNodeColor);
                    break;
                case CompositeNode:
                    view.SetColor(BehaviourAPISettings.instance.CompositeColor);
                    if (node is SequencerNode) view.SetIconText("-->");
                    else if (node is SelectorNode) view.SetIconText("?");
                    break;
                case DecoratorNode:
                    view.SetColor(BehaviourAPISettings.instance.DecoratorColor);
                    break;
            }

            OnRepaint();
        }

        public override void OnConnected(EdgeView edgeView)
        {
            if (edgeView.input.node != view || !view.graphView.IsRuntime) return;

            if (view.InputConnectionViews.Count == 1 && node is BTNode btNode)
            {
                btNode.LastExecutionStatusChanged += OnLastExecutionStatusChanged;
                OnLastExecutionStatusChanged(btNode.LastExecutionStatus);
            }
        }

        public override void OnDestroy()
        {
            if (view.graphView.IsRuntime && node is BTNode btNode && view.InputConnectionViews.Count == 1)
            {
                btNode.LastExecutionStatusChanged -= OnLastExecutionStatusChanged;
            }
        }

        private void OnLastExecutionStatusChanged(Status status)
        {
            var edgeView = view.InputConnectionViews[0];
            edgeView.control.UpdateStatus(status);
        }


        public override void OnRepaint()
        {
            if (view.graphView.graphData.nodes.First() == view.data && IsValidRootNode(view.data))
            {
                if (view.data.parentIds.Count > 0)
                {

                }
                else
                {
                    view.inputContainer.Hide();
                    rootIcon.Enable();
                }
            }
            else
            {
                view.inputContainer.Show();
                rootIcon.Disable();
            }
        }

        public override void SetUpPorts()
        {
            if (node == null || node.MaxInputConnections != 0)
            {
                InputPort = view.InstantiatePort(Direction.Input, EPortOrientation.Bottom);
            }
            else view.inputContainer.Disable();

            if (node == null || node.MaxOutputConnections != 0)
            {
                OutputPort = view.InstantiatePort(Direction.Output, EPortOrientation.Top);
            }
            else view.outputContainer.Disable();
        }

        public override PortView GetPort(NodeView nodeView, Direction direction)
        {
            if (direction == Direction.Input) return InputPort;
            else return OutputPort;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (view.graphView.IsRuntime) return;

            evt.menu.AppendAction("Convert to root node",
                _ => ConvertToRootNode(),
                (view.GetDataIndex() != 0) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            evt.menu.AppendSeparator();
            evt.menu.AppendAction("Order childs by x position", _ => view.OrderChildNodes(n => n.position.x),
                view.data.childIds.Count > 1 ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
        }

        private void ConvertToRootNode()
        {
            view.DisconnectAllPorts(Direction.Input);
            view.ConvertToFirstNode();
        }

        public override void OnDeleted()
        {
            RecomputeRootNode();
        }

        private static bool IsValidRootNode(NodeData data)
        {
            return data.parentIds.Count == 0;
        }

        private void RecomputeRootNode()
        {
            var nodes = view.graphView.graphData.nodes;
            if (nodes.Count > 0 && !IsValidRootNode(nodes.First()))
            {
                var newRootNode = nodes.FirstOrDefault(IsValidRootNode);

                if (newRootNode != null) nodes.MoveAtFirst(newRootNode);
            }
        }
    }
}
