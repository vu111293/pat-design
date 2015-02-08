using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;
using Tools.Diagrams;

namespace PAT.Common.GUI.UMLModule
{
    public sealed class StateDiagram
    {
        public string Name; //StateDiagram
        public string XmiContent;//Location of the .xmi file


        public StateDiagram(string name, string xmiContent)
        {
            Name = name;
            XmiContent = xmiContent;
        }
    }

    public class UMLModel
    {
        public Dictionary<string, StateDiagram> Diagrams;
        public string Assertion;
        public string SystemName;
        
        public UMLModel()
        {
            this.Diagrams = new Dictionary<string, StateDiagram>();
            Assertion = "";
        }

        public UMLModel(string systemName, Dictionary<string, StateDiagram> diagrams, string assertion )
        {
            SystemName = systemName;
            Diagrams = diagrams;
            Assertion = assertion;
        }

        public void AddDiagram(StateDiagram diagram)
        {
            if (Diagrams == null)
                Diagrams = new Dictionary<string, StateDiagram>();

            Diagrams.Add(diagram.Name, diagram);
        }

        public static UMLModel LoadUMLModelFromXML(string text)
        {
            UMLModel umlModel = new UMLModel();
            XmlDataDocument doc = new XmlDataDocument();

            
            doc.LoadXml(text);


            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.UML_MODEL_NODE_NAME);
            umlModel.SystemName = sitesNodes[0].ChildNodes[0].InnerText;
            
            sitesNodes = doc.GetElementsByTagName(Parsing.ASSERTION_NODE_NAME);
            
            if (sitesNodes[0].ChildNodes[0] != null)
                umlModel.Assertion = sitesNodes[0].ChildNodes[0].InnerText;
            
            sitesNodes = doc.GetElementsByTagName(Parsing.DIAGRAMS_NODE_NAME);

            if (sitesNodes.Count > 0)
            {
                int i = 0;
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    i++;
                    string dName, dxmiContent;
                    XmlNodeList diagramName = component.GetElementsByTagName(Parsing.DIAGRAM_NAME);

                    if (diagramName == null || diagramName.Count < 1)
                    {
                        dName = "diagram" + i;
                    }
                    else
                    {
                        dName = diagramName[0].InnerText;
                    }

                    XmlNodeList xmiContent = component.GetElementsByTagName(Parsing.DIAGRAM_XMI_CONTENT);
                    if (xmiContent == null || xmiContent.Count < 1)
                        dxmiContent = "";
                    else
                    {
                        dxmiContent = xmiContent[0].InnerText;
                    }


                    umlModel.AddDiagram(new StateDiagram(dName, dxmiContent));
                }
            }

            return umlModel;
        }

        public XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();

            XmlElement UmlModel = doc.CreateElement(Parsing.UML_MODEL_NODE_NAME);
            doc.AppendChild(UmlModel);

            
            XmlElement assertion = doc.CreateElement(Parsing.ASSERTION_NODE_NAME);
            assertion.InnerText = this.Assertion;
            UmlModel.AppendChild(assertion);

            XmlElement diagrams = doc.CreateElement(Parsing.DIAGRAMS_NODE_NAME);
            foreach (var diagram in Diagrams)
            {
                XmlElement diagramTag = doc.CreateElement(Parsing.DIAGRAM_TAG);

                XmlElement diagramName = doc.CreateElement(Parsing.DIAGRAM_NAME);
                diagramName.InnerText = diagram.Key;
                diagramTag.AppendChild(diagramName);

                XmlElement xmiContent = doc.CreateElement(Parsing.DIAGRAM_XMI_CONTENT);
                xmiContent.InnerText = diagram.Value.XmiContent;
                diagramTag.AppendChild(xmiContent);

                diagrams.AppendChild(diagramTag);
            }

            UmlModel.AppendChild(diagrams);

            return doc;
        }

        public string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<UMLModel>");
            sb.AppendLine("<Assertions>");
            sb.AppendLine(Assertion);
            sb.AppendLine("</Assertions>");

            sb.AppendLine("<Diagrams>");
            foreach (var diagram in Diagrams)
            {
                sb.AppendLine("<Diagram>");
                sb.AppendLine("<NAME>" + diagram.Key + "</NAME>");
                
                string xmiContent = "";
                
                if (diagram.Value != null)
                {
                    xmiContent = EncodePath(diagram.Value.XmiContent);
                }
                sb.AppendLine("<PATH>" + xmiContent + "</PATH>");
                sb.AppendLine("</Diagram>");
            }
            sb.AppendLine("</Diagrams>");
            sb.AppendLine("</UMLModel>");

            return sb.ToString();
        }

        //to change space into '?'
        private static string EncodePath(string path)
        {
            int length = path.Length;

            string newPath = "";
            
            char code = '?';

            for (int i = 0; i < length; i++)
            {
                if(path[i] == ' ')
                {
                    newPath = newPath + code;
                }
                else
                {
                    newPath = newPath + path[i];
                }
            }

            return newPath;
        }
    }
}