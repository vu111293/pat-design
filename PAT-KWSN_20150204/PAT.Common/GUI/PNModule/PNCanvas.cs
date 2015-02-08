using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.KWSNModule;
using System.Xml;
using Tools.Diagrams;

namespace PAT.Common.GUI.PNModule
{
    public partial class PNCanvas : LTSCanvas
    {

        //testing---------------------------------------------------
        private List<string> sensorIds;
        private List<string> channelIds;

        //end testing-----------------------------------------------

        public int TransitionCounter = 0;
        public int PlaceCounter = 0;

        public const string MODEL_NODE_NAME = "Model";
        public const string TRANSITION_COUNTER = "TransitionCounter";
        public const string PLACE_COUNTER = "PlaceCounter";
        public const string PLACE_NODE_NAME = "Places";
        public const string TRANSITION_NODE_NAME = "Transitions";
        public const string ARC_NODE_NAME = "Arcs";

        private int color;

        public PNCanvas()
        {
            this.sensorIds = new List<string>();
            this.channelIds = new List<string>();
        }

        public PNCanvas(List<string> sensorIds, List<string> channelIds)
        {
            this.sensorIds = sensorIds;
            this.channelIds = channelIds;
        }

        public void LoadFromXmlAlt(XmlElement elem, int color)
        {
            this.color = color;
            LoadFromXml(elem);
        }

        public override void LoadFromXml(XmlElement elem)
        {
            Node.Text = elem.GetAttribute(NAME_PROCESS_NODE_NAME, "");
            Console.Write(Node.Text);
            Parameters = elem.GetAttribute(PARAMETER_NODE_NAME, "");
            Console.Write(Parameters);
            Zoom = float.Parse(elem.GetAttribute(ZOOM_PROCESS_NODE_NAME, ""));
            PlaceCounter = int.Parse(elem.GetAttribute(PLACE_COUNTER));
            TransitionCounter = int.Parse(elem.GetAttribute(TRANSITION_COUNTER));

            int color = 0;
            XmlElement placesElement = (XmlElement)elem.ChildNodes[0];
            foreach (XmlElement element in placesElement.ChildNodes)
            {
                string elementId = element.GetAttribute(PNNode.TAG_REFERENCE_ID);
                if (this.sensorIds.Exists(x => x == elementId)) color = 1;
                if (this.channelIds.Exists(x => x == elementId)) color = -1;
                PNPlace place = new PNPlace(string.Empty, 0, elementId, color);
                place.LoadFromXml(element);
                this.AddSingleCanvasItem(place);
                this.AddSingleCanvasItem(place.labelItems);
            }

            XmlElement transitionsElement = (XmlElement)elem.ChildNodes[1];
            foreach (XmlElement element in transitionsElement.ChildNodes)
            {
                string elementId = element.GetAttribute(PNNode.TAG_REFERENCE_ID);
                if (this.sensorIds.Exists(x => x == elementId)) color = 1;
                if (this.channelIds.Exists(x => x == elementId)) color = -1;
                PNTransition transition = new PNTransition(string.Empty, elementId, color);
                transition.LoadFromXml(element);
                this.AddSingleCanvasItem(transition);
                this.AddSingleCanvasItem(transition.labelItems);
            }

            XmlElement linksElement = (XmlElement)elem.ChildNodes[2];
            foreach (XmlElement element in linksElement.ChildNodes)
            {
                PNArc arc = new PNArc(null, null);
                arc.LoadFromXML(element, this);

                this.AddSingleLink(arc);
                foreach (NailItem nailItem in arc.Nails)
                {
                    this.AddSingleCanvasItem(nailItem);
                }

                this.AddSingleCanvasItem(arc.Label);
            }
        }

        public override XmlElement WriteToXml(XmlDocument doc)
        {
            XmlElement canvasElement = doc.CreateElement(MODEL_NODE_NAME);

            XmlAttribute attributeOfProcess = doc.CreateAttribute(NAME_PROCESS_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = Node.Text;
            canvasElement.Attributes.Append(attributeOfProcess);

            attributeOfProcess = doc.CreateAttribute(PARAMETER_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = this.Parameters.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            attributeOfProcess = doc.CreateAttribute(ZOOM_PROCESS_NODE_NAME); // Non-Standard attribute
            attributeOfProcess.Value = Zoom.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            //place counter
            attributeOfProcess = doc.CreateAttribute(PLACE_COUNTER);
            attributeOfProcess.Value = this.PlaceCounter.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            //transition counter
            attributeOfProcess = doc.CreateAttribute(TRANSITION_COUNTER);
            attributeOfProcess.Value = this.TransitionCounter.ToString(System.Globalization.CultureInfo.InvariantCulture);
            canvasElement.Attributes.Append(attributeOfProcess);

            //places
            XmlElement placesElement = doc.CreateElement(PLACE_NODE_NAME);
            canvasElement.AppendChild(placesElement);
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is PNPlace)
                    placesElement.AppendChild(item.Item.WriteToXml(doc));
            }

            //transitions
            XmlElement transitionsElement = doc.CreateElement(TRANSITION_NODE_NAME);
            canvasElement.AppendChild(transitionsElement);
            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is PNTransition)
                    transitionsElement.AppendChild(item.Item.WriteToXml(doc));
            }

            //links
            XmlElement linksElement = doc.CreateElement(ARC_NODE_NAME);
            canvasElement.AppendChild(linksElement);
            foreach (Route route in diagramRouter.Routes)
            {
                if (route is PNArc)
                    linksElement.AppendChild(route.WriteToXml(doc));
            }

            return canvasElement;
        }

        public override string ToSpecificationString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Process \"" + Node.Text + "\"(" + Parameters + "):");

            List<PNTransition> transitions = GetTransitionList();
            foreach (var transition in transitions)
                sb.AppendLine(GetState(transition));

            sb.AppendLine(";");
            return sb.ToString();
        }

        private string GetState(PNTransition transition)
        {
            StringBuilder sb = new StringBuilder();
            Dictionary<PNPlace, int> ins = new Dictionary<PNPlace, int>();
            Dictionary<PNPlace, int> outs = new Dictionary<PNPlace, int>();
            List<PNArc> arcs = GetArcList();

            foreach (var arc in arcs)
            {
                if (arc.To is PNTransition && (arc.To as PNTransition).Name.Equals(transition.Name))
                    ins.Add(arc.From as PNPlace, arc.Weight);

                if (arc.From is PNTransition && (arc.From as PNTransition).Name.Equals(transition.Name))
                    outs.Add(arc.To as PNPlace, arc.Weight);
            }

            List<string> guards = new List<string>();
            List<string> updates = new List<string>();
            foreach (KeyValuePair<PNPlace, int> kvp in ins)
            {
                guards.Add(string.Format("({0} >= {1})", kvp.Key.Name, kvp.Value));
                if (!string.IsNullOrEmpty(kvp.Key.Guard))
                    guards.Add(kvp.Key.Guard);

                updates.Add(string.Format("{0} = ({0} - {1}); ", kvp.Key.Name, kvp.Value));
            }

            foreach (KeyValuePair<PNPlace, int> kvp in outs)
            {
                if (kvp.Key.Capacity > 0)
                    guards.Add(string.Format("({0} + {1} <= {2})", kvp.Key.Name, kvp.Value, kvp.Key.Capacity));

                updates.Add(string.Format("{0} = ({0} + {1}); ", kvp.Key.Name, kvp.Value));
            }

            if (!string.IsNullOrEmpty(transition.Guard))
                guards.Add(transition.Guard);

            if (!string.IsNullOrEmpty(transition.Program))
                updates.Add(transition.Program);

            ins.Clear();
            outs.Clear();

            string guard = string.Empty;
            string update = string.Empty;
            for (int i = 0; i < guards.Count; i++)
            {
                if (i == (guards.Count - 1))
                {
                    guard += guards[i];
                    continue;
                }

                guard += guards[i] + " && ";
            }

            foreach (var str in updates)
                update += str;

            sb.AppendFormat("\"Start\"--[{0}] ##@@ {1} @@## {{{2}}}-->\"End\"", guard, transition.Name, update);

            return sb.ToString();
        }

        public string GetDeclare()
        {
            StringBuilder sb = new StringBuilder();
            List<PNPlace> places = GetPlaceList();
            string format = "var {0}:{{0..}} = {1};\n";

            //initial marking
            foreach (PNPlace place in places)
                sb.AppendFormat(format, place.Name, place.NumberOfTokens);

            return sb.ToString();
        }

        private List<PNArc> GetArcList()
        {
            List<PNArc> arcs = new List<PNArc>();
            foreach (var item in diagramRouter.Routes)
            {
                if (item is PNArc)
                    arcs.Add(item as PNArc);
            }

            return arcs;
        }

        private List<PNTransition> GetTransitionList()
        {
            List<PNTransition> transitions = new List<PNTransition>();
            foreach (var item in itemsList)
            {
                if (item.Item is PNTransition)
                    transitions.Add(item.Item as PNTransition);
            }

            return transitions;
        }

        private List<PNPlace> GetPlaceList()
        {
            List<PNPlace> places = new List<PNPlace>();
            foreach (var item in itemsList)
            {
                if (item.Item is PNPlace)
                    places.Add(item.Item as PNPlace);
            }

            return places;
        }

        public PNCanvas Dup()
        {
            PNCanvas duplicate = new PNCanvas();
            duplicate.LoadFromXml(this.Clone());
            duplicate.Node.Text = this.Node.Text + "-Copy";

            bool nameExist = true;
            while (nameExist)
            {
                nameExist = false;
                foreach (TreeNode node in this.Node.Parent.Nodes)
                {
                    if (node.Text.Equals(duplicate.Node.Text, StringComparison.CurrentCultureIgnoreCase))
                    {
                        duplicate.Node.Text = duplicate.Node.Text + "-Copy";
                        nameExist = true;
                        break;
                    }
                }
            }

            return duplicate;
        }
    }
}
