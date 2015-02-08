using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using PAT.Common.GUI.LTSModule;
using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.KWSNModule
{
    /// <summary>
    /// Model data definition
    /// </summary>
    public static class Data
    {
        public const string NODE_NAME = "WSN";

        public const string TAG_DECLARATION = "Declaration";

        public const string TAG_NETWORK = "Network";
        public const string TAG_SENSORS = "Sensors";
    }

    #region PN Model define
    public class PNNode
    {
        public const string TAG_PN = "PN";
        public const string TAG_DECLARATION = "Declaration";
        public const string TAG_MODELS = "Models";
        public const string TAG_MODEL = "Model";

        public const string TAG_PRO_NAME = "Name";

        public const string TAG_MODEL_PRO_PARAM = "Parameter";
        public const string TAG_MODEL_PRO_ZOOM = "Zoom";
        public const string TAG_MODEL_PRO_PCOUNTER = "PlaceCounter";
        public const string TAG_MODEL_PRO_TCOUNTER = "TransitionCounter";
        // for WSN database only
        public const string TAG_MODEL_PRO_IN = "In"; // Input node name
        public const string TAG_MODEL_PRO_OUT = "Out"; // Output node name

        public const string TAG_PLACES = "Places";
        public const string TAG_PLACE = "Place";

        public const string TAG_TRANSITIONS = "Transitions";
        public const string TAG_TRANSITION = "Transition";

        public const string TAG_ARCS = "Arcs";
        public const string TAG_ARC = "Arc";
        public const string TAG_ARC_PRO_FROM = "From";
        public const string TAG_ARC_PRO_TO = "To";
        public const string TAG_ARC_PRO_WEIGHT = "Weight";

        public const string TAG_POSITION = "Position";
        public const string TAG_POSITION_X = "X";
        public const string TAG_POSITION_Y = "Y";
        public const string TAG_POSITION_WIDTH = "Width";

        public const string TAG_LABEL = "Label";

        public const string TAG_GUARD = "Guard";

        //kwsn expande element
        public const string TAG_PN_LEVEL = "Level";
        public const string TAG_TOPOLOGY = "Topololy";
        public const string TAG_SENSOR = "Sensor";
        public const string TAG_CHANNEL = "Channel";
        public const string TAG_SENSOR_TYPE = "Type";
        public const string TAG_SENSOR_ID = "Id";
        public const string TAG_SENSOR_MODE = "Mode";
        public const string TAG_SENSOR_X = "X";
        public const string TAG_SENSOR_Y = "Y";
        public const string TAG_CHANNEL_FROM = "From";
        public const string TAG_CHANNEL_TO = "To";
        public const string TAG_CHANNEL_ID = "Id";
        public const string TAG_CHANNEL_TYPE = "Type";
        public const string TAG_CHANNEL_MODE = "Mode";
        public const string TAG_REFERENCE_ID = "Id";
    }

    public class WSNPNData
    {
        public XmlElement places = null;
        public XmlElement transitions = null;
        public XmlElement arcs = null;

        public NodeInfo inNode;
        public NodeInfo outNode;

        public string nodeId = null;

        public void clear()
        {
            places = null;
            transitions = null;
            arcs = null;

            inNode = new NodeInfo();
            outNode = new NodeInfo();

            nodeId = null;
        }
    }

    public class Position
    {
        public float x = -1;
        public float y = -1;

        public override string ToString()
        {
            return string.Format("[{0}, {1}]", x, y);
        }
    }

    public class NodeInfo
    {
        public string name;
        public Position pos;
    }
    #endregion

    /// <summary>
    /// Wireless Sensor Network model
    /// </summary>
    public class WSNModel : LTSModel
    {
        #region Constructor
        public WSNModel() : base() { }
        public WSNModel(string declare, List<LTSCanvas> canvas) : base(declare, canvas) { }
        #endregion

        /// <summary>
        /// Load model from XML string
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="PNDocRes"></param>
        /// <returns></returns>
        public static WSNModel LoadModelFromXML(string xml, XmlDocument PNDocRes)
        {
            WSNModel model = new WSNModel();

            do
            {
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.LoadXml(xml);
                }
                catch { }

                XmlNodeList nodes = null;

                do
                {
                    nodes = doc.GetElementsByTagName(Data.TAG_DECLARATION);
                    if (nodes == null || nodes.Count != 1)
                        break;

                    model.Declaration = nodes.Item(0).InnerText;
                } while (false);

                do
                {
                    nodes = doc.GetElementsByTagName(Data.TAG_NETWORK);
                    if (nodes == null)
                        break;

                    LTSCanvas canvas = null;
                    foreach (XmlElement node in nodes[0].ChildNodes)
                    {
                        canvas = new WSNCanvas(PNDocRes);
                        canvas.LoadFromXml(node);
                        model.Processes.Add(canvas);
                    }
                } while (false);
            } while (false);

            return model;
        }

        /// <summary>
        /// Generate the XML data
        /// </summary>
        /// <returns></returns>
        public override XmlDocument GenerateXML()
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element = null;

            XmlElement root = doc.CreateElement(Data.NODE_NAME);
            doc.AppendChild(root);

            element = doc.CreateElement(Data.TAG_DECLARATION);
            element.InnerText = Declaration;
            root.AppendChild(element);

            element = doc.CreateElement(Data.TAG_NETWORK);
            foreach (LTSCanvas canvas in Processes)
                element.AppendChild(canvas.WriteToXml(doc));

            root.AppendChild(element);

            return doc;
        }
    }
}
