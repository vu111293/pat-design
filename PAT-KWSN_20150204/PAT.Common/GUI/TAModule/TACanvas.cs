using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.ERAModule;
using Tools.Diagrams;

namespace PAT.Common.GUI.TAModule
{
    public class TACanvas : LTSCanvas
    {
        public override void LoadFromXml(XmlElement elem)
        {
            Node.Text = elem.GetAttribute(NAME_PROCESS_NODE_NAME, "");
            Parameters = elem.GetAttribute(PARAMETER_NODE_NAME, "");
            Zoom = float.Parse(elem.GetAttribute(ZOOM_PROCESS_NODE_NAME, ""));
            try
            {
                StateCounter = int.Parse(elem.GetAttribute(STATE_COUNTER));
            }
            catch
            {
                StateCounter = elem.ChildNodes[0].ChildNodes.Count + 1;
            }

            XmlElement statesElement = (XmlElement)elem.ChildNodes[0];
            foreach (XmlElement element in statesElement.ChildNodes)
            {
                TAState canvasitem = new TAState(false, "");
                canvasitem.LoadFromXml(element);
                this.AddSingleCanvasItem(canvasitem);
                this.AddSingleCanvasItem(canvasitem.labelItems);
            }

            XmlElement linksElement = (XmlElement)elem.ChildNodes[1];
            foreach (XmlElement element in linksElement.ChildNodes)
            {
                Route route = new Route(null, null);
                route.LoadFromXML(element, this);

                this.AddSingleLink(route);
                foreach (NailItem nailItem in route.Nails)
                {
                    this.AddSingleCanvasItem(nailItem);
                }
                //
                this.AddSingleCanvasItem(route.Transition);
            }
        }

        public TACanvas Duplicate()
        {
            TACanvas duplicate = new TACanvas();
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
