using UnityEditor.Experimental.GraphView;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    public class EdgeView : Edge
    {
        public EdgeControl control;
        protected override UnityEditor.Experimental.GraphView.EdgeControl CreateEdgeControl()
        {
            control = new EdgeControl()
            {
                capRadius = 4.0f,
                interceptWidth = 6.0f,
                edgeView = this
            };
            return control;
        }
    }
}