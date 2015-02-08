using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Tools.Diagrams;

using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.KWSNModule
{
    /// <summary>
    /// Wireless Sensor Network canvas
    /// </summary>
    public class WSNCanvas : LTSCanvas
    {
        public WSNCanvas(XmlDocument PNDoc)
            : base()
        {
            PNDocResource = PNDoc;
        }

        #region Model Save/Load
        #region Model Loading
        protected override void LoadStates(XmlElement procNode)
        {
            XmlElement sensors = (XmlElement)procNode.ChildNodes[0];
            WSNSensorItem canvasSensor = null;
            foreach (XmlElement xmlSen in sensors.ChildNodes)
            {
                canvasSensor = new WSNSensorItem(StateCounter);
                canvasSensor.LoadFromXml(xmlSen);
                AddSingleCanvasItem(canvasSensor);
                AddSingleCanvasItem(canvasSensor.labelItems);
            }
        }

        protected override Route LoadRoute(XmlElement element)
        {
            WSNSensorChannel route = new WSNSensorChannel(null, null);
            route.LoadFromXML(element, this);

            return route;
        }
        #endregion

        #region Model Saving
        protected override void WriteStates(XmlDocument doc, XmlElement canvasElement)
        {
            XmlElement sensors = doc.CreateElement(Data.TAG_SENSORS);
            canvasElement.AppendChild(sensors);

            foreach (CanvasItemData item in itemsList)
            {
                if (item.Item is StateItem)
                    sensors.AppendChild(item.Item.WriteToXml(doc));
            }
        }
        #endregion
        #endregion

        #region PN Export
        protected XmlDocument _docRes = null;
        public virtual XmlDocument PNDocResource
        {
            get { return _docRes; }
            set { _docRes = value; }
        }
        #endregion
    }
}
