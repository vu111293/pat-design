using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;
using Tools.Diagrams;

namespace PAT.Common.GUI.SNModule
{
    public sealed class NodeData
    {
        public string Name; //Name of the node
        public List<string> CodeList;//Application deployed on the node
        public string TopConfiguration;
        public string SensorRanges;
        public string PredefinedVars;
        public string TOS_ID;

        public NodeData(string name, string tosID)
        {
            Name = name;
            CodeList = new List<string>(0);
            SensorRanges = "/* enter the sensor settings here... */";
            TOS_ID = tosID;
        }

        public NodeData(string name, string app, string range, string tosID, string predefVars)
        {
            Name = name;
            CodeList = new List<string>(1) { app };
            TopConfiguration = app;
            SensorRanges = range;
            TOS_ID = tosID;
            PredefinedVars = predefVars;
        }
    }

    public class SNModel
    {
        public LTSCanvas Network;
        //<ID, App>
        public Dictionary<string, NodeData> Sensors;
        public string Assertion;
        public string SystemName;
        public List<string> LinkList;

        public SNModel()
        {
            Network = new LTSCanvas();
            this.Sensors = new Dictionary<string, NodeData>();
            LinkList = new List<string>();
            Assertion = "";
        }


        public SNModel(LTSCanvas declare, string assertion, Dictionary<string, NodeData> sensors, string system)
        {
            this.Network = declare;
            this.Assertion = assertion;
            this.Sensors = sensors;
            this.SystemName = system;
        }

        public void AddLink(string link)
        {
            if (LinkList == null)
                LinkList = new List<string>();

            LinkList.Add(link);
        }

        public static SNModel LoadSensorNetworkFromXML(string text)
        {
            SNModel snModel = new SNModel();
            XmlDataDocument doc = new XmlDataDocument();
            doc.LoadXml(text);

            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.SYSTEM_NODE_NAME);
            snModel.SystemName = sitesNodes[0].ChildNodes[0].InnerText;
            sitesNodes = doc.GetElementsByTagName(Parsing.NETWORK_NODE_NAME);

            foreach (XmlElement component in sitesNodes)
            {
                LTSCanvas canvas = new LTSCanvas();
                canvas.LoadFromXml(component.ChildNodes[0] as XmlElement);
                snModel.Network = canvas;
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.LINK_NODE_NAME);
            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement node in sitesNodes[0].ChildNodes)
                    snModel.AddLink(node.InnerText);
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.ASSERTION_NODE_NAME);
            if (sitesNodes[0].ChildNodes[0] != null)
                snModel.Assertion = sitesNodes[0].ChildNodes[0].InnerText;

            sitesNodes = doc.GetElementsByTagName(Parsing.SENSORS_NODE_NAME);
            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes[0].ChildNodes)
                {
                    //ID
                    //Application
                    XmlNodeList sensorID = component.GetElementsByTagName("ID");
                    XmlNodeList tosId = component.GetElementsByTagName("TOSID");
                    string tosid = "";
                    if (tosId == null || tosId.Count < 1)
                        tosid = "0";
                    else
                        tosid = tosId[0].InnerText;

                    XmlNodeList sensorApp = component.GetElementsByTagName("App");
                    string app = "";
                    if (sensorApp == null || sensorApp.Count < 1)
                        app = "app";
                    else
                        app = sensorApp[0].InnerText;

                    XmlNodeList sensorRange = component.GetElementsByTagName("Range");
                    string range = "";
                    if (sensorRange == null || sensorRange.Count < 1)
                        range = "//enter the sensor settings here...";
                    else
                        range = sensorRange[0].InnerText;

                    string predefVars = "";
                    if (component.HasAttribute("Predefined"))
                    {
                        XmlNodeList sensorPredefVars = component.GetElementsByTagName("Predefined");

                        if (sensorRange.Count < 1)
                            predefVars = "//enter predefined variables here...";
                        else
                            predefVars = sensorPredefVars[0].InnerText;
                    }
                    else
                        predefVars = "//enter predefined variables here...";

                    snModel.Sensors.Add(sensorID[0].InnerText, new NodeData(sensorID[0].InnerText, app, range, tosid, predefVars));
                }
            }

            return snModel;
        }

        public XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();
            XmlElement SensorNetwork = doc.CreateElement(Parsing.SENSORNETWORK_NODE_NAME);
            doc.AppendChild(SensorNetwork);

            XmlElement systemName = doc.CreateElement(Parsing.SYSTEM_NODE_NAME);
            systemName.InnerText = this.SystemName;
            SensorNetwork.AppendChild(systemName);

            XmlElement network = doc.CreateElement(Parsing.NETWORK_NODE_NAME);
            network.AppendChild(Network.WriteToXml(doc));
            SensorNetwork.AppendChild(network);

            if (LinkList != null && LinkList.Count > 0)
            {
                XmlElement linkList = doc.CreateElement(Parsing.LINK_NODE_NAME);
                foreach (string s in LinkList)
                {
                    XmlElement link = doc.CreateElement("LinkElement");
                    link.InnerText = s;
                    linkList.AppendChild(link);
                }

                SensorNetwork.AppendChild(linkList);
            }

            XmlElement assertion = doc.CreateElement(Parsing.ASSERTION_NODE_NAME);
            assertion.InnerText = this.Assertion;
            SensorNetwork.AppendChild(assertion);

            XmlElement sensors = doc.CreateElement(Parsing.SENSORS_NODE_NAME);
            foreach (var sensor in Sensors)
            {
                XmlElement sensorTag = doc.CreateElement("Sensor");
                XmlElement sensorID = doc.CreateElement("ID");
                sensorID.InnerText = sensor.Key;
                sensorTag.AppendChild(sensorID);

                XmlElement tosID = doc.CreateElement("TOSID");
                tosID.InnerText = sensor.Value.TOS_ID;
                sensorTag.AppendChild(tosID);

                XmlElement sensorApp = doc.CreateElement("App");
                sensorApp.InnerText = sensor.Value.TopConfiguration;
                sensorTag.AppendChild(sensorApp);

                XmlElement sensorRange = doc.CreateElement("Range");
                sensorRange.InnerText = sensor.Value.SensorRanges;
                sensorTag.AppendChild(sensorRange);

                if (!string.IsNullOrEmpty(sensor.Value.PredefinedVars))
                {
                    XmlElement sensorPredefVars = doc.CreateElement("Predefined");
                    sensorPredefVars.InnerText = sensor.Value.PredefinedVars;
                    sensorTag.AppendChild(sensorPredefVars);
                }

                sensors.AppendChild(sensorTag);
            }

            SensorNetwork.AppendChild(sensors);

            return doc;
        }

        public string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<SensorNetwork>");
            sb.AppendLine(NetworkToString(Network));
            sb.AppendLine("<Assertions>");
            sb.AppendLine(Assertion);
            sb.AppendLine("</Assertions>");

            sb.AppendLine("<SensorDetails>");
            foreach (var sensor in Sensors)
            {
                sb.AppendLine("<Sensor>");
                sb.AppendLine("<ID>" + sensor.Key + "</ID>");

                string tosid = "0";

                //<App>" + sensor.Value + "</App>"
                string app = "app";
                string range = "/* enter sensor range here... */";
                string predefVars = "/* enter predefined variables here... */";

                if (sensor.Value != null)
                {
                    tosid = sensor.Value.TOS_ID;
                    app = sensor.Value.TopConfiguration;
                    //change space into '?'
                    app = EncodePath(app);
                    range = sensor.Value.SensorRanges;
                    predefVars = sensor.Value.PredefinedVars;
                }

                sb.AppendLine("<TOSID>" + tosid + "</TOSID>");
                sb.AppendLine("<App>" + app + "</App>");
                sb.AppendLine("<Range>" + range + "</Range>");
                sb.AppendLine("<Predefined>" + predefVars + "</Predefined>");
                sb.AppendLine("</Sensor>");
            }
            sb.AppendLine("</SensorDetails>");
            sb.AppendLine("</SensorNetwork>");

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
                if (path[i] == ' ')
                    newPath = newPath + code;
                else
                    newPath = newPath + path[i];
            }

            return newPath;
        }

        public string LinkToString(Transition transition)
        {//Guard: Loss rate
            //Event: Bandwidth
            //Program: Note
            return "[" + transition.Guard + "]  (" + transition.Event + ")  {" + transition.Program + "}";
        }

        public string NetworkToString(LTSCanvas network)
        {
            StringBuilder sb = new StringBuilder();

            StateItem.StateCounterSpec = 0;

            sb.AppendLine("<Network Deployment>");
            sb.AppendLine("<Base Nodes>");
            foreach (LTSCanvas.CanvasItemData item in network.itemsList)
            {//append base station nodes
                if (item.Item is StateItem)
                {
                    if ((item.Item as StateItem).IsInitialState)
                        sb.AppendLine((item.Item as StateItem).GetName());
                }
            }
            sb.AppendLine("</Base Nodes>");

            sb.AppendLine("<Normal Nodes>");
            foreach (LTSCanvas.CanvasItemData item in network.itemsList)
            {//append client nodes
                if (item.Item is StateItem)
                {
                    if (!(item.Item as StateItem).IsInitialState)
                        sb.AppendLine((item.Item as StateItem).GetName());
                }
            }
            sb.AppendLine("</Normal Nodes>");

            sb.AppendLine("<Links>");
            foreach (Route route in network.diagramRouter.Routes)
            {
                sb.AppendLine("\"" + (route.From as StateItem).GetName()
                    + "\"-->\""
                    + (route.To as StateItem).GetName() + "\"");
            }

            sb.AppendLine("</Links>");
            sb.AppendLine("</Network Deployment>");
            return sb.ToString();
        }
    }
}