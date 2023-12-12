using System;
using UnityEditor.Experimental.GraphView;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using Core;
    using UnityEngine.UIElements;

    public abstract class NodeDrawer
    {
        protected NodeView view;
        protected Node node;

        /// <summary>
        /// Set the references of the drawer
        /// </summary>
        /// <param name="view">The view that the drawer uses.</param>
        /// <param name="node">The node that the drawer represents.</param>
        public void SetView(NodeView view, Node node)
        {
            this.view = view;
            this.node = node;
        }

        /// <summary>
        /// Set up the node ports
        /// </summary>
        public abstract void SetUpPorts();

        /// <summary>
        /// Draw the specified elements of the node drawed.
        /// </summary>
        public abstract void DrawNodeDetails();

        public abstract PortView GetPort(NodeView nodeView, Direction direction);

        #region --------------------------------------------- Events ---------------------------------------------

        /// <summary>
        /// Add options to the node contextual menu.
        /// </summary>
        /// <param name="evt">The contextual menu creation event.</param>
        public virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt) { }
        /// <summary>
        /// Method called when the node needs to be repainted.
        /// </summary>
        public virtual void OnRepaint() { }

        /// <summary>
        /// Method called when the node position changes
        /// </summary>
        public virtual void OnMoved() { }

        /// <summary>
        /// Method called when the node is selected.
        /// </summary>
        public virtual void OnSelected() { }

        /// <summary>
        /// Method called when the node is unselected.
        /// </summary>
        public virtual void OnUnselected() { }

        /// <summary>
        /// Method called when a new connection is created in the node.
        /// </summary>
        public virtual void OnConnected(EdgeView edgeView) { }

        /// <summary>
        /// Method called when a new connection is deleted in the node.
        /// </summary>
        public virtual void OnDisconnected(EdgeView edgeView) { }

        /// <summary>
        /// Method called when the node is being removed from the graph.
        /// </summary>
        public virtual void OnDeleted() { }

        /// <summary>
        /// Method called when the node properties changed and must be reflected in the view        
        /// </summary>
        public virtual void OnRefreshDisplay() { }

        /// <summary>
        /// Method called when the node is destroyed in runtime mode, when the selected graph changes.
        /// </summary>
        public virtual void OnDestroy() { }

        #endregion

        /// <summary>
        /// Create a node drawer according to the node type.
        /// </summary>
        /// <param name="node">The node that the created drawer will render.</param>
        /// <returns>The drawer created.</returns>
        public static NodeDrawer Create(Node node)
        {
            if (node != null && BehaviourAPISettings.instance.Metadata.NodeDrawerTypeMap.TryGetValue(node.GetType(), out Type drawerType))
            {
                return (NodeDrawer)Activator.CreateInstance(drawerType);
            }
            else
            {
                return null;
            }

        }
    }
}
