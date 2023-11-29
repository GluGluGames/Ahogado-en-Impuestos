using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using BehaviourAPI.Core;
    using BehaviourAPI.UnityToolkit;
    using Framework;

    /// <summary>
    /// Class used to represent a <see cref="NodeData"/> element in a <see cref="GraphView"/>.
    /// </summary>
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        #region UI Elements 
        private static readonly string k_NameField = "title-input-field";
        private static readonly string k_Status = "node-status";
        private static readonly string k_Border = "node-border";

        private static readonly string k_DetailsDiv = "node-details";
        private static readonly string k_ExtensionToggle = "node-extension-toggle";
        private static readonly string k_ExtensionContainer = "node-extension-content";

        private static readonly string k_PortCover = "node-port-cover";
        #endregion

        #region ------------------------------- Public fields -------------------------------

        /// <summary>
        /// The node data represented by the view.
        /// </summary>
        public NodeData data;

        /// <summary>
        /// The list of edges connected to input ports in the node.
        /// </summary>
        public List<EdgeView> InputConnectionViews { get; private set; } = new List<EdgeView>();

        /// <summary>
        /// The list of edges connected to output ports in the node.
        /// </summary>
        public List<EdgeView> OutputConnectionViews { get; private set; } = new List<EdgeView>();

        /// <summary>
        /// The graphview that contains the node.
        /// </summary>
        public BehaviourGraphView graphView => m_graphView;

        /// <summary>
        /// The element that display node runtime information.
        /// </summary>
        public VisualElement Details => m_Details;

        #endregion

        #region ------------------------------- Private fields -------------------------------

        NodeDrawer drawer;

        SerializedProperty nodeProperty;

        VisualElement m_BorderElement;
        VisualElement m_Warning;
        VisualElement m_Details;
        VisualElement m_ExtensionContainer;
        VisualElement m_StatusBorder;

        TextField m_NameInputField;

        Toggle m_ExtensionToggle;

        Dictionary<string, ExtensionView> m_taskViews = new Dictionary<string, ExtensionView>();

        BehaviourGraphView m_graphView;

        #endregion

        /// <summary>
        /// Create a new view for an specified node
        /// </summary>
        /// <param name="data">The node data.</param>
        /// <param name="drawer">The drawer used to render the node details.</param>
        /// <param name="graphView"></param>
        /// <param name="property">The serialized property of the node.</param>
        public NodeView(NodeData data, NodeDrawer drawer, BehaviourGraphView graphView, SerializedProperty property = null) 
            : base(BehaviourAPISettings.instance.EditorLayoutsPath + "Elements/node.uxml")
        {
            this.data = data;
            this.drawer = drawer;

            m_graphView = graphView;

            drawer.SetView(this, data.node);
            SetPosition(new Rect(data.position, Vector2.zero));

            m_NameInputField = this.Q<TextField>(k_NameField);
            m_BorderElement = this.Q(k_Border);
            m_ExtensionToggle = this.Q<Toggle>(k_ExtensionToggle);
            m_ExtensionContainer = this.Q(k_ExtensionContainer);
            m_StatusBorder = this.Q(k_Status);
            m_Details = this.Q(k_DetailsDiv);

            m_ExtensionToggle.RegisterValueChangedCallback(OnChangeExtensionToggle);

            nodeProperty = property;

            drawer.SetUpPorts();

            m_NameInputField.value = data.name;

            if (nodeProperty != null)
            {
                m_NameInputField.bindingPath = nodeProperty.FindPropertyRelative("name").propertyPath;
                m_NameInputField.Bind(nodeProperty.serializedObject);
                m_NameInputField.isReadOnly = false;
            }
            else
            {
                m_NameInputField.isReadOnly = true;
            }

            DrawNodeDetails();
            OnPropertyChanged();
        }

        /// <summary>
        /// Call this method when the node is selected.
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            m_BorderElement.ChangeBackgroundColor(new Color(.5f, .5f, .5f, .5f));
            drawer.OnSelected();
        }

        /// <summary>
        /// Call this method when the node is unselected.
        /// </summary>
        public override void OnUnselected()
        {
            base.OnSelected();
            m_BorderElement.ChangeBackgroundColor(new Color(0f, 0f, 0f, 0f));
            drawer.OnUnselected();
        }

        /// <summary>
        /// Call this when the node is dragged in the editor
        /// </summary>
        public void OnMoved()
        {
            data.position = GetPosition().position;
            drawer.OnMoved();
        }


        private void DrawNodeDetails()
        {
            m_NameInputField.value = data.name;

            SetIconText(data.node.TypeName().CamelCaseToSpaced());

            if (nodeProperty != null)
            {
                m_NameInputField.bindingPath = nodeProperty.FindPropertyRelative("name").propertyPath;
                m_NameInputField.Bind(nodeProperty.serializedObject);
                m_NameInputField.isReadOnly = false;
            }
            else
            {
                m_NameInputField.isReadOnly = true;
            }

            if (graphView.IsRuntime)
            {
                var portCover = this.Q(k_PortCover);
                portCover.Enable();

                if (data.node is IStatusHandler statusHandler)
                {
                    statusHandler.StatusChanged += OnStatusChanged;
                    OnStatusChanged(statusHandler.Status);
                }
            }

            foreach(var referencedData in data.references)
            {
                AddExtensionView(referencedData.FieldName);
            }

            drawer.DrawNodeDetails();

            if (m_taskViews.Count == 0) m_ExtensionToggle.Disable();
        }

        #region --------------------------------------- Modify elements ---------------------------------------

        /// <summary>
        /// Create a new port in the node.
        /// </summary>
        /// <param name="direction">The direction of the port.</param>
        /// <param name="orientation">The orientation of the port.</param>
        /// <returns>The port created.</returns>
        public PortView InstantiatePort(Direction direction, EPortOrientation orientation)
        {
            var isInput = direction == Direction.Input;

            Port.Capacity portCapacity;
            Type portType;

            if (data.node != null)
            {
                if (isInput) portCapacity = data.node.MaxInputConnections == -1 ? Port.Capacity.Multi : Port.Capacity.Single;
                else portCapacity = data.node.MaxOutputConnections == -1 ? Port.Capacity.Multi : Port.Capacity.Single;
                portType = isInput ? data.node.GetType() : data.node.ChildType;
            }
            else
            {
                portCapacity = Port.Capacity.Multi;
                portType = typeof(NodeView); // Any invalid type
            }

            var port = PortView.Create(orientation, direction, portCapacity, portType, graphView.Connector);
            (isInput ? inputContainer : outputContainer).Add(port);

            if (graphView.IsRuntime)
            {
                port.capabilities -= Capabilities.Selectable;
            }

            port.portName = "";
            port.style.flexDirection = orientation.ToFlexDirection();

            var bg = new VisualElement();
            bg.style.position = Position.Absolute;
            bg.style.top = 0; bg.style.left = 0; bg.style.bottom = 0; bg.style.right = 0;
            port.Add(bg);

            return port;
        }

        /// <summary>
        /// Add a new label to the extension container in the node.
        /// </summary>
        /// <param name="name">The name of the extension element.</param>
        /// <returns>The element created.</returns>
        public ExtensionView AddExtensionView(string name)
        {
            ExtensionView taskView = new ExtensionView();

            if (m_taskViews.TryAdd(name, taskView))
            {
                m_ExtensionContainer.Add(taskView);
                return taskView;
            }
            else
            {
                Debug.LogWarning("Error creating a taskview. Other with the same name already exists");
                return null;
            }
        }

        /// <summary>
        /// Find a extension element by its name.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <returns>The element found.</returns>
        public ExtensionView GetTaskView(string name)
        {
            if (m_taskViews.TryGetValue(name, out var taskView))
            {
                return taskView;
            }
            else
            {
                Debug.LogWarning("Error creating a taskview. Other with the same name already exists");
                return null;
            }
        }

        /// <summary>
        /// Change the color of the horizontal strips in the node. 
        /// </summary>
        /// <param name="color">The new color.</param>
        public void SetColor(Color color)
        {
            var iconElement = this.Q("node-icon");
            iconElement.ChangeBackgroundColor(color);
            elementTypeColor = color;
        }

        /// <summary>
        /// Displays and change the text of the icon element.
        /// </summary>
        /// <param name="text">The new text.</param>
        public void SetIconText(string text)
        {
            var iconElement = this.Q<Label>("node-icon-label");
            iconElement.text = text;
        }

        #endregion

        #region -------------------------------------- Editor Events --------------------------------------

        /// <summary>
        /// Call this method when a new edge is connected to the node.
        /// </summary>
        /// <param name="edgeView">The edge connected.</param>
        /// <param name="updateData">True if the edge was created by the user and the node connection data must be updated.</param>
        public void OnConnected(EdgeView edgeView, bool updateData = true)
        {
            if (edgeView.output.node == this)
            {
                if (updateData)
                {
                    var other = edgeView.input.node as NodeView;
                    data.childIds.Add(other.data.id);
                }
                OutputConnectionViews.Add(edgeView);
                UpdateChildConnectionViews();
            }
            else
            {
                if (updateData)
                {
                    var other = edgeView.output.node as NodeView;
                    data.parentIds.Add(other.data.id);
                }
                InputConnectionViews.Add(edgeView);
                UpdateParentConnectionViews();
            }
            drawer.OnConnected(edgeView);
        }

        /// <summary>
        /// Call when an edge is disconnected to the node.
        /// </summary>
        /// <param name="edgeView">The edge disconnected.</param>
        /// <param name="updateData">True if the edge was removed by the user and the node connection data must be updated.</param>
        public void OnDisconnected(EdgeView edgeView, bool updateData = true)
        {
            if (edgeView.output.node == this)
            {
                if (updateData)
                {
                    var other = edgeView.input.node as NodeView;
                    data.childIds.Remove(other.data.id);
                }
                OutputConnectionViews.Remove(edgeView);
                UpdateChildConnectionViews();
            }
            else
            {
                if (updateData)
                {
                    var other = edgeView.output.node as NodeView;
                    data.parentIds.Remove(other.data.id);
                }
                InputConnectionViews.Remove(edgeView);
                UpdateParentConnectionViews();
            }
            drawer.OnDisconnected(edgeView);
        }


        /// <summary>
        /// Called when the node index changes and the serialized property must be updated, after deleting a node or changing the start node.
        /// </summary>
        /// <param name="prop">The new property associated with the node.</param>
        public void UpdateSerializedProperty(SerializedProperty prop)
        {
            nodeProperty = prop;
            m_NameInputField.value = data.name;
            if (nodeProperty == null)
            {
                m_NameInputField.isReadOnly = true;
            }
            else
            {
                // The binding path needs to be assigned before the object:
                m_NameInputField.bindingPath = nodeProperty.FindPropertyRelative("name").propertyPath;
                m_NameInputField.Bind(nodeProperty.serializedObject);
                m_NameInputField.isReadOnly = false;
            }
        }

        /// <summary>
        /// Called every time the node serialized property changed in editor:
        /// </summary>
        public void OnPropertyChanged()
        {
            foreach(var referencedData in data.references)
            {
                var display = m_taskViews[referencedData.FieldName];
                display.Update(referencedData.GetInfo());
            }
            drawer.OnRefreshDisplay();
        }

        /// <summary>
        /// Called when the node is deleted.
        /// Is used to remove the event handlers in runtime mode.
        /// </summary>
        public void OnDestroy()
        {
            if (graphView.IsRuntime && data.node is IStatusHandler statusHandler)
            {
                statusHandler.StatusChanged -= OnStatusChanged;
            }
            drawer.OnDestroy();
        }

        /// <summary>
        /// Called when the element will be deleted
        /// </summary>
        public void OnDeleted()
        {
            drawer.OnDeleted();
        }

        #endregion

        #region ------------------------------------- Runtime Events -------------------------------------

        private void OnStatusChanged(Status status)
        {
            m_StatusBorder.ChangeBorderColor(status.ToColor());
        }

        #endregion      

        #region ------------------------------- Contextual menu -------------------------------

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (!graphView.IsRuntime)
            {
                evt.menu.AppendAction("Disconnect all.", _ => DisconnectAll(),
                (InputConnectionViews.Count > 0 || OutputConnectionViews.Count > 0) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Disconnect all input edges.", _ => DisconnectAllInput(),
                    (InputConnectionViews.Count > 0) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction("Disconnect all output edges.", _ => DisconnectAllOutput(),
                    (OutputConnectionViews.Count > 0) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator();
            }

            evt.menu.AppendAction("Debug", _ => DebugNode());

            drawer.BuildContextualMenu(evt);
            evt.StopPropagation();
        }

        /// <summary>
        /// Change the order of the child elements of the node.
        /// </summary>
        /// <param name="orderFunction">The function used to sort the elements, usually using its position.</param>
        public void OrderChildNodes(Func<NodeData, float> orderFunction)
        {
            m_graphView.RegisterUndo("Order child nodes");
            graphView.graphData.OrderChildNodes(data, (n) => n.position.x);
            UpdateChildConnectionViews();
            graphView.UpdateProperties();
        }

        private void DisconnectAll()
        {
            DisconnectAllInput();
            DisconnectAllOutput();
        }

        private void DisconnectAllInput()
        {
            graphView.DeleteElements(InputConnectionViews);
            InputConnectionViews.Clear();
        }

        private void DisconnectAllOutput()
        {
            graphView.DeleteElements(OutputConnectionViews);
            OutputConnectionViews.Clear();
        }

        private void DebugNode()
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Name: " + data.name);
            sb.AppendLine("Id: " + data.id);
            sb.AppendLine("Parents (" + data.parentIds.Count + ") :");

            for (int i = 0; i < data.parentIds.Count; i++)
            {
                sb.AppendLine("\t- " + data.parentIds[i]);
            }

            sb.AppendLine("Children (" + data.childIds.Count + ") :");

            for (int i = 0; i < data.childIds.Count; i++)
            {
                sb.AppendLine("\t- " + data.childIds[i]);
            }

            sb.AppendLine("Index: " + graphView.graphData.nodes.IndexOf(data).ToString());
            Debug.Log(sb.ToString());
        }

        #endregion

        /// <summary>
        /// Get the best port to connect a new edge, according to the position of the other node.
        /// </summary>
        /// <param name="targetNodeView">The other node view.</param>
        /// <param name="direction">The direction of the conection in the local node.</param>
        /// <returns>The selected port for the connection.</returns>
        public Port GetBestPort(NodeView targetNodeView, Direction direction) => drawer.GetPort(targetNodeView, direction);

        /// <summary>
        /// Get the index of the node represented by this view in the graph.
        /// </summary>
        /// <returns>The index of the node.</returns>
        public int GetDataIndex()
        {
            var graphData = graphView.graphData;
            return graphData.nodes.IndexOf(data);
        }

        /// <summary>
        /// Move the represented node to the first position in the node list.
        /// </summary>
        public void ConvertToFirstNode()
        {
            graphView.SetNodeAsFirst(this);
        }

        /// <summary>
        /// Remove all edges connected to input ports in the node.
        /// </summary>
        public void DisconnectAllPorts(Direction direction)
        {
            if (direction == Direction.Input) DisconnectAllInput();
            else DisconnectAllInput();
        }

        /// <summary>
        /// Force the update of the view.
        /// </summary>
        public void RefreshView()
        {
            SetPosition(new Rect(data.position, Vector2.zero));
            drawer.OnMoved();
            drawer.OnRepaint();
        }



        private void UpdateChildConnectionViews()
        {
            var childs = data.childIds;
            if (childs.Count <= 1)
            {
                foreach (var edgeView in OutputConnectionViews)
                {
                    edgeView.control.UpdateIndex(0);
                }
            }
            else
            {
                foreach (var edgeView in OutputConnectionViews)
                {
                    var target = (edgeView.input.node as NodeView).data;
                    int idx = childs.IndexOf(target.id);
                    edgeView.control.UpdateIndex(idx + 1);
                }
            }
        }

        private void UpdateParentConnectionViews()
        {

        }

        private void OnChangeExtensionToggle(ChangeEvent<bool> evt)
        {
            m_ExtensionContainer.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    public class ExtensionView : VisualElement
    {
        public Label Label { get; private set; }
        public VisualElement Container { get; private set; }

        public ExtensionView()
        {
            var asset = BehaviourAPISettings.instance.GetLayoutAsset("Elements/extensionview.uxml");
            asset.CloneTree(this);

            Label = this.Q<Label>("tv-label");
            Container = this.Q("tv-container");
        }

        public void Update(string text)
        {
            Label.text = text;
        }

        public void AddElement(VisualElement visualElement)
        {
            Container.Add(visualElement);
        }
    }
}
