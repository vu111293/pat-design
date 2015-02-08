using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;
using Tools.Diagrams;

namespace PAT.Common.GUI.TAModule
{
    public class TAModel
    {
        public string Declaration;
        public List<TACanvas> Processes;
        public List<TACanvas> Properties;

        public TAModel()
        {
            Declaration = "";
            this.Processes = new List<TACanvas>();
            this.Properties = new List<TACanvas>();
        }


        public TAModel(string declare, List<TACanvas> processes, List<TACanvas> properties)
        {
            this.Declaration = declare;
            this.Processes = processes;
            this.Properties = properties;
        }

        public static TAModel LoadLTSFromXML(string text)
        {
            TAModel lts = new TAModel();
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
                    TACanvas canvas = new TACanvas();

                    canvas.LoadFromXml(component);

                    lts.Processes.Add(canvas);
                }
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.PROPERTIES_NODE_NAME.Replace(" ", "_"));

            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    TACanvas canvas = new TACanvas();

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
            foreach (TACanvas canvas in Processes)
            {
                process.AppendChild(canvas.WriteToXml(doc));
               
            }

            LTS.AppendChild(process);

            XmlElement properties = doc.CreateElement(Parsing.PROPERTIES_NODE_NAME.Replace(" ", "_"));
            foreach (TACanvas canvas in Properties)
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

            foreach (TACanvas canvas in Processes)
            {
                sb.AppendLine(canvas.ToSpecificationString());
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file,canvas.Node.Text});
            }
            foreach (TACanvas canvas in Properties)
            {
                sb.AppendLine(canvas.ToSpecificationString().Replace("Process ", "Property "));
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file,canvas.Node.Text});
            }
            return sb.ToString();
        }

        public string ToSpectNewFormat()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(Declaration);
            foreach (TACanvas canvas in Processes)
            {
                string[] strings = canvas.Parameters.Trim().Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);
                string clocks = string.Empty;
                string para = string.Empty;

                if (strings.Length == 1 && canvas.Parameters.Trim().StartsWith("$"))
                {
                    clocks = strings[0];
                }
                else if (strings.Length == 1 && !canvas.Parameters.Trim().StartsWith("$"))
                {
                    para = strings[0];
                }
                else if (strings.Length == 2)
                {
                    para = strings[0];
                    clocks = strings[1];
                }

                if(para != string.Empty)
                {
                    para = "(" + para + ")";
                }
                sb.AppendLine("TimedAutomaton " + canvas.Node.Text + para);
                sb.AppendLine("{");
                if(clocks != string.Empty)
                {
                    sb.AppendLine("\tclock: " + clocks + ";");
                }

                sb.AppendLine();

                StringBuilder otherStates = new StringBuilder();

                foreach (var item in canvas.itemsList)
                {
                    if (item.Item is TAState)
                    {
                        if ((item.Item as TAState).IsInitialState)
                        {
                            sb.AppendLine(StateToNewFormat(item.Item as TAState, canvas.diagramRouter.Routes));
                        }
                        else
                        {
                            otherStates.AppendLine(StateToNewFormat(item.Item as TAState, canvas.diagramRouter.Routes));
                        }
                    }
                }

                sb.Append(otherStates);
                sb.AppendLine("}");
                sb.AppendLine();

            }

            return sb.ToString();
        }

        private string StateToNewFormat(TAState item, Route[] routes)
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("\tstate " + item.GetName());

            if (item.Invariant != string.Empty)
            {
                result.AppendLine("\tinv: " + item.Invariant + ";");
            }

            string label = string.Empty;
            if(item.IsUrgent)
            {
                label += "urgent ";
            }
            if(item.IsCommitted)
            {
                label += "committed";
            }
            if (item.IsError)
            {
                label += "error";
            }
            if(label != string.Empty)
            {
                result.AppendLine("\t" + label + ";");
            }

            foreach (var route in routes)
            {
                if ((route.From as TAState).GetName() == item.GetName())
                {
                    string clockGuard = (route.Transition.ClockGuard == string.Empty) ? string.Empty : "[[" + route.Transition.ClockGuard + "]]";
                    string guard = (route.Transition.Guard == string.Empty) ? string.Empty : "[" + route.Transition.Guard + "]";

                    string program = (route.Transition.Program == string.Empty) ? string.Empty : "{" + route.Transition.Program.Replace("\r\n", "") + "}";
                    string clockReset = (route.Transition.ClockReset == string.Empty) ? string.Empty : "<" + route.Transition.ClockReset + ">";



                    result.AppendLine("\ttrans:" + clockGuard + guard + route.Transition.GetEventPart() + program + clockReset + "->" + (route.To as TAState).GetName() + ";");
                }
            }

            result.AppendLine("\tendstate");

            return result.ToString();
        }

    }
}