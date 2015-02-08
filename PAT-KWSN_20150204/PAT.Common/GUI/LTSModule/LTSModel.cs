using System.Collections.Generic;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.LTSModule
{
    public class LTSModel
    {
        public string Declaration;
        public List<LTSCanvas> Processes;

        public LTSModel()
        {
            Declaration = "";
            this.Processes = new List<LTSCanvas>();
        }

        public LTSModel(string declare, List<LTSCanvas> processes)
        {
            this.Declaration = declare;
            this.Processes = processes;
        }

        public static LTSModel LoadLTSFromXML(string text)
        {
            LTSModel lts = new LTSModel();

            XmlDataDocument doc = new XmlDataDocument();
            doc.LoadXml(text);

            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.DECLARATION_NODE_NAME);
            foreach (XmlElement component in sitesNodes)
                lts.Declaration = component.InnerText;

            sitesNodes = doc.GetElementsByTagName(Parsing.PROCESSES_NODE_NAME);
            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    LTSCanvas canvas = new LTSCanvas();
                    canvas.LoadFromXml(component);

                    lts.Processes.Add(canvas);
                }
            }
            
            return lts;
        }

        public virtual XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();
            XmlElement LTS = doc.CreateElement(Parsing.LTS_NODE_NAME);
            doc.AppendChild(LTS);

            XmlElement declar = doc.CreateElement(Parsing.DECLARATION_NODE_NAME);
            declar.InnerText = Declaration;
            
            LTS.AppendChild(declar);

            XmlElement process = doc.CreateElement(Parsing.PROCESSES_NODE_NAME);
            foreach (LTSCanvas canvas in Processes)
                process.AppendChild(canvas.WriteToXml(doc));

            LTS.AppendChild(process);
            return doc;
        }

        public string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Declaration);
            string file = "";
            foreach (string[] eraCanvase in ParsingException.FileOffset.Values)
                file = eraCanvase[0];

            ParsingException.FileOffset.Clear();
            ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, "Declaration" });
            foreach (LTSCanvas canvas in Processes)
            {
                sb.AppendLine(canvas.ToSpecificationString());
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, canvas.Node.Text });
            }

            return sb.ToString();
        }
    }
}