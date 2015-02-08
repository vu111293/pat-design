using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace PAT.Common.GUI.ERAModule
{
    public class ERAModel
    {
        public string Declaration;
        public List<ERACanvas> Processes;
        public List<ERACanvas> Properties;

        public ERAModel()
        {
            Declaration = "";
            this.Processes = new List<ERACanvas>();
            this.Properties = new List<ERACanvas>();
        }


        public ERAModel(string declare, List<ERACanvas> processes, List<ERACanvas> properties)
        {
            this.Declaration = declare;
            this.Processes = processes;
            this.Properties = properties;
        }

        public static ERAModel LoadLTSFromXML(string text)
        {
            ERAModel lts = new ERAModel();
            XmlDataDocument doc = new XmlDataDocument();


            doc.LoadXml(text);

            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.DECLARATION_NODE_NAME);

            foreach (XmlElement component in sitesNodes)
            {
                lts.Declaration = component.InnerText;
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.PROCESSES_NODE_NAME);

            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    ERACanvas canvas = new ERACanvas();

                    canvas.LoadFromXml(component);

                    lts.Processes.Add(canvas);
                }
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.PROPERTIES_NODE_NAME.Replace(" ", "_"));

            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    ERACanvas canvas = new ERACanvas();

                    canvas.LoadFromXml(component);

                    lts.Properties.Add(canvas);
                }
            }
            
            return lts;
        }

        public XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();

            XmlElement LTS = doc.CreateElement(Parsing.LTS_NODE_NAME);
            doc.AppendChild(LTS);

            XmlElement declar = doc.CreateElement(Parsing.DECLARATION_NODE_NAME);
            declar.InnerText = Declaration;
            
            LTS.AppendChild(declar);

            XmlElement process = doc.CreateElement(Parsing.PROCESSES_NODE_NAME);
            foreach (ERACanvas canvas in Processes)
            {
                process.AppendChild(canvas.WriteToXml(doc));
               
            }

            LTS.AppendChild(process);

            XmlElement properties = doc.CreateElement(Parsing.PROPERTIES_NODE_NAME.Replace(" ", "_"));
            foreach (ERACanvas canvas in Properties)
            {
                properties.AppendChild(canvas.WriteToXml(doc));

            }

            LTS.AppendChild(properties);

            return doc;
        }

        public string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Declaration);

            string file = "";
            foreach (string[] eraCanvase in ParsingException.FileOffset.Values)
            {
                file = eraCanvase[0];
            }

            ParsingException.FileOffset.Clear();
            ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, "Declaration" });

            foreach (ERACanvas canvas in Processes)
            {
                sb.AppendLine(canvas.ToSpecificationString());
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, canvas.Node.Text });
            }
            foreach (ERACanvas canvas in Properties)
            {
                sb.AppendLine(canvas.ToSpecificationString().Replace("Process ", "Property "));
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, canvas.Node.Text });
            }

            return sb.ToString();
        }
    }
}