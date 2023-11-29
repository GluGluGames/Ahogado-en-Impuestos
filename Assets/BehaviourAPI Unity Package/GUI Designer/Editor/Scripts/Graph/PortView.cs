using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    /// <summary>
    /// 
    /// </summary>
    public class PortView : Port
    {
        public EPortOrientation Orientation;

        protected PortView(EPortOrientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation.ToOrientation(), portDirection, portCapacity, type)
        {
            Orientation = portOrientation;
            Decorate();
        }

        public void Decorate()
        {
            bool isOutput = direction == Direction.Output;

            var fixer = new VisualElement();
            fixer.StretchToParentSize();
            Add(fixer);

            if (Orientation == EPortOrientation.Top && isOutput || Orientation == EPortOrientation.Bottom && !isOutput)
            {
                m_ConnectorBox.style.borderTopLeftRadius = 0f;
                m_ConnectorBox.style.borderTopRightRadius = 0f;
            }
            else if (Orientation == EPortOrientation.Left && isOutput || Orientation == EPortOrientation.Right && !isOutput)
            {
                m_ConnectorBox.style.borderTopRightRadius = 0f;
                m_ConnectorBox.style.borderBottomRightRadius = 0f;
            }
            else if (Orientation == EPortOrientation.Right && isOutput || Orientation == EPortOrientation.Left && !isOutput)
            {
                m_ConnectorBox.style.borderTopLeftRadius = 0f;
                m_ConnectorBox.style.borderBottomLeftRadius = 0f;
            }
            else if (Orientation == EPortOrientation.Bottom && isOutput || Orientation == EPortOrientation.Top && !isOutput)
            {
                m_ConnectorBox.style.borderBottomLeftRadius = 0f;
                m_ConnectorBox.style.borderBottomRightRadius = 0f;
            }
        }

        public static PortView Create(EPortOrientation portOrientation, Direction portDirection,
            Capacity portCapacity, Type type, IEdgeConnectorListener edgeConnector = null)
        {
            PortView port = new PortView(portOrientation, portDirection, portCapacity, type)
            {
                m_EdgeConnector = new EdgeConnector<EdgeView>(edgeConnector)
            };
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }

        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private GraphViewChange m_GraphViewChange;

            private List<Edge> m_EdgesToCreate;

            private List<GraphElement> m_EdgesToDelete;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
            }

            public void OnDrop(UnityEditor.Experimental.GraphView.GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                {
                    foreach (Edge connection in edge.input.connections)
                    {
                        if (connection != edge)
                        {
                            m_EdgesToDelete.Add(connection);
                        }
                    }
                }

                if (edge.output.capacity == Capacity.Single)
                {
                    foreach (Edge connection2 in edge.output.connections)
                    {
                        if (connection2 != edge)
                        {
                            m_EdgesToDelete.Add(connection2);
                        }
                    }
                }

                if (m_EdgesToDelete.Count > 0)
                {
                    graphView.DeleteElements(m_EdgesToDelete);
                }

                List<Edge> edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                {
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                }

                foreach (Edge item in edgesToCreate)
                {
                    graphView.AddElement(item);
                    edge.input.Connect(item);
                    edge.output.Connect(item);
                }
            }
        }
    }

    public enum EPortOrientation
    {
        None = 0,
        Top = 1,
        Right = 2,
        Bottom = 3,
        Left = 4
    }
}
