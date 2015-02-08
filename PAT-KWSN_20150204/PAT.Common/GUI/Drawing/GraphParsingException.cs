using Antlr.Runtime;

namespace PAT.Common.GUI.Drawing
{
    public class GraphParsingException : ParsingException
    {
        public string ProcessName;
        public GraphParsingException(string processName, string message, IToken token) : base(message, token)
        {
            ProcessName = processName;
        }

        public GraphParsingException(string processName, string message, int line, int position, string text)
            : base(message, line, position, text)
        {
            ProcessName = processName;
        }
    }
}