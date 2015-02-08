using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using PAT.Common.GUI.KWSNModule;
using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.PNModule
{
    public class PNModel
    {
        // Save the assertion
        private string _declaration;
        public string Declaration
        {
            get { return _declaration; }
            set { _declaration = value; }
        }

        private List<PNCanvas> _canvases;
        public List<PNCanvas> Canvases
        {
            get { return _canvases; }
            set { _canvases = value; }
        }

        //testing----------------------------------------------------------
        private XmlElement _topology;

        public XmlElement Topology
        {
            get { return _topology; }
            set { _topology = value; }
        }
        //end testing------------------------------------------------------

        public PNModel(string declaration, List<PNCanvas> canvases, XmlElement topology)
        {
            _declaration = declaration;
            _canvases = canvases;
            _topology = topology;
        }

        public XmlDocument GenerateXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement PN = doc.CreateElement(Parsing.PN_NODE_NAME);
            doc.AppendChild(PN);

            XmlNode _topo = _topology.CloneNode(true);
            XmlNode topo = PN.OwnerDocument.ImportNode(_topo, true);
            PN.AppendChild(topo);

            XmlElement decl = doc.CreateElement(Parsing.ASSERTION_NODE_NAME);
            decl.InnerText = _declaration;
            PN.AppendChild(decl);

            XmlElement model = doc.CreateElement(Parsing.MODELS_NODE_NAME);
            foreach (PNCanvas canvas in _canvases)
                model.AppendChild(canvas.WriteToXml(doc));

            PN.AppendChild(model);
            return doc;
        }

        public static PNModel LoadLTSFromXML(string text)
        {
            string assertion = string.Empty;
            List<PNCanvas> canvases = new List<PNCanvas>();

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(text);

            // testing----------------------------------------------------

            XmlElement topology;

            List<string> sensorIds = new List<string>();
            List<string> channelIds = new List<string>();

            XmlNodeList sitesNodes = doc.GetElementsByTagName(PNNode.TAG_TOPOLOGY);
            topology = (XmlElement)sitesNodes[0];

            sitesNodes = doc.GetElementsByTagName(PNNode.TAG_SENSOR);
            foreach (XmlElement element in sitesNodes)
                sensorIds.Add(element.GetAttribute(PNNode.TAG_SENSOR_ID));

            sitesNodes = doc.GetElementsByTagName(PNNode.TAG_CHANNEL);
            foreach (XmlElement element in sitesNodes)
                channelIds.Add(element.GetAttribute(PNNode.TAG_CHANNEL_ID));          

            //------------------------------------------------------------

            sitesNodes = doc.GetElementsByTagName(Parsing.ASSERTION_NODE_NAME);

            //TODO? What is this for?
            foreach (XmlElement component in sitesNodes)
                assertion = component.InnerText;

            sitesNodes = doc.GetElementsByTagName(Parsing.MODELS_NODE_NAME);

            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    PNCanvas canvas = new PNCanvas(sensorIds, channelIds);
                    canvas.LoadFromXmlAlt(component, 0);
                    canvases.Add(canvas);
                }
            }

            return new PNModel(assertion, canvases, topology);
        }

        public string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (PNCanvas canvas in _canvases)
                sb.AppendLine(canvas.GetDeclare());

            sb.AppendLine(Declaration);
            string file = "";
            foreach (string[] eraCanvase in ParsingException.FileOffset.Values)
                file = eraCanvase[0];

            ParsingException.FileOffset.Clear();
            ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, "Declaration" });
            foreach (PNCanvas canvas in _canvases)
            {
                sb.AppendLine(canvas.ToSpecificationString());
                ParsingException.FileOffset.Add(ParsingException.CountLinesInFile(sb.ToString()), new string[] { file, canvas.Node.Text });
            }

            return sb.ToString();
        }
    }
}
