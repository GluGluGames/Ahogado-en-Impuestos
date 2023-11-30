using BehaviourAPI.Core;
using BehaviourAPI.UnityToolkit;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    public class EdgeControl : UnityEditor.Experimental.GraphView.EdgeControl
    {
        public EdgeView edgeView;

        public VisualElement edgeNumberDiv, edgeStatusDiv;
        public Label edgeNumberLabel, edgeStatusLabel;

        public EdgeControl()
        {
            var edgeTagAsset = BehaviourAPISettings.instance.GetLayoutAsset("Elements/edgetag.uxml");
            var edgeTag = edgeTagAsset.CloneTree();
            edgeTag.style.position = Position.Absolute;
            edgeTag.style.left = new StyleLength(new Length(50, LengthUnit.Percent));
            edgeTag.style.top = new StyleLength(new Length(50, LengthUnit.Percent));

            edgeNumberDiv = edgeTag.Q("edge-number-div");
            edgeNumberLabel = edgeTag.Q<Label>("edge-number-label");

            edgeStatusDiv = edgeTag.Q("edge-status-div");
            edgeStatusLabel = edgeTag.Q<Label>("edge-status-label");

            Add(edgeTag);
        }

        protected override void ComputeControlPoints()
        {
            var inputDir = Vector2.zero;
            var outputDir = Vector2.zero;

            if (edgeView.input is PortView inputPortView)
            {
                inputDir = inputPortView.Orientation.ToVector();
            }

            if (edgeView.output is PortView outputPortView)
            {
                outputDir = outputPortView.Orientation.ToVector();
            }


            base.ComputeControlPoints();
            var minDelta = 16f;
            var delta = (controlPoints[3] - controlPoints[0]).magnitude * .25f;

            if (delta < minDelta)
            {
                delta = minDelta;
            }

            if (delta > 30f) delta = 30f;

            controlPoints[1] = controlPoints[0] + delta * outputDir;
            controlPoints[2] = controlPoints[3] + delta * inputDir;
        }

        public void UpdateIndex(int id)
        {
            edgeNumberLabel.text = id.ToString();
            if (id == 0)
            {
                edgeNumberDiv.Disable();
            }
            else
            {
                edgeNumberDiv.Enable();
            }
        }

        public void UpdateStatus(Status status)
        {
            edgeStatusLabel.text = status.ToString();
            edgeStatusLabel.style.color = status.ToColor();
            if (status == Status.None)
            {
                edgeStatusDiv.Disable();
            }
            else
            {
                edgeStatusDiv.Enable();
            }
        }
    }
}
