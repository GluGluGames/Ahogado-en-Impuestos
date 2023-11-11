using System.Linq;
using System.Text;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    public class CodeWriter
    {
        StringBuilder stringBuilder = new StringBuilder();
        StringBuilder currentLineBuilder = new StringBuilder();

        public string IdentationElement = "\t";
        public int IdentationLevel = 0;

        public void Append(string text)
        {
            currentLineBuilder.Append(text);
        }

        public void AppendLine(string line)
        {
            currentLineBuilder.Append(line);
            stringBuilder.AppendLine(string.Concat(Enumerable.Repeat(IdentationElement, IdentationLevel)) + currentLineBuilder.ToString());
            currentLineBuilder.Clear();
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
