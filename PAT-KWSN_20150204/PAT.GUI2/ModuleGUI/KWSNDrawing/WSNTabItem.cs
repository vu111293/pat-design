using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using Tools.Diagrams;

using PAT.Common.GUI.Drawing;
using PAT.Common.GUI.LTSModule;
using PAT.Common.GUI.KWSNModule;
using PAT.Common.Ultility;

namespace PAT.GUI.ModuleGUI.KWSNDrawing
{
    public class WSNTabItem : LTSDrawing.LTSTabItem
    {
        private const string TAG = "WSNTabItem";

        private const string PNXmlRes = "wsn-pn-based.xml";

        private FormMain mMainForm = null;

        #region GUI controls
        private ToolStripMenuItem mnuItemConvert2PNModel;
        #endregion

        public WSNTabItem(string name, string shortName, FormMain mainForm)
            : base(name)
        {
            _docPNRes = LoadPNRes(shortName);

            mMainForm = mainForm;
            initGUI();
        }

        #region PN XML resource
        private XmlDocument _docPNRes = null;
        public virtual XmlDocument PNRes
        {
            get { return _docPNRes; }
        }

        protected XmlDocument LoadPNRes(string moduleName)
        {
            XmlDocument doc = null;
            string path = Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, moduleName, PNXmlRes);

            try
            {
                doc = new XmlDocument();
                doc.Load(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load PN resource xml\n\n" + "* Path: " + path + "\n\n,* Error: " + ex.Message,
                    "Load resource failed", MessageBoxButtons.OKCancel);
            }

            return doc;
        }
        #endregion

        #region GUI handler
        private void initGUI()
        {
            // Add context menu to convert data to PN Model
            mnuItemConvert2PNModel = new ToolStripMenuItem("Export to Petri Nets Model ...");
            mnuItemConvert2PNModel.Name = "mnuItemConvert2PNModel";
            mnuItemConvert2PNModel.Click += mnuItemConvert2PNModel_Click;
            this.contextMenuStrip1.Items.Add(mnuItemConvert2PNModel);
        }

        private void mnuItemConvert2PNModel_Click(object sender, EventArgs e)
        {
            // Create a file in the system temporary folder
            // Should force the extension to .pn for PAT loading PN model correctly
            string tempPath = Path.GetTempFileName() + ".pn";
            Log.d(TAG, "Save path: " + tempPath);

            PNModelHelper pnHelper = new PNModelHelper(tempPath, Canvas);
            if (pnHelper.generateXML(_docPNRes) == false)
                MessageBox.Show("Failed to export the network to PN model!", "Export failed", MessageBoxButtons.OKCancel);
            else
                mMainForm.OpenFile(tempPath, false);
        }
        #endregion

        protected override LTSModel LoadModel(string text)
        {
            return WSNModel.LoadModelFromXML(text, _docPNRes);
        }

        protected override LTSModel CreateModel(string declare, List<LTSCanvas> canvas)
        {
            return new WSNModel(declare, canvas);
        }

        protected override StateItem CreateItem()
        {
            return new WSNSensorItem(Canvas.StateCounter);
        }

        protected override Form CreateItemEditForm(StateItem item)
        {
            return new SensorEditForm(item);
        }

        protected override Form CreateTransitionEditForm(Route route, List<LTSCanvas.CanvasItemData> items)
        {
            return new SensorChannelEditForm(route, items);
        }

        protected override Route CreateRoute(IRectangle from, IRectangle to)
        {
            return new WSNSensorChannel(from, to);
        }
    }

    class PNModelHelper
    {
        private const string TAG = "PNModelHelper";

        private string _fileName = null;
        private List<WSNSensorItem> _sensors = null;
        private List<WSNSensorChannel> _channels = null;

        private XmlDocument _docOut = null;
        private XmlElement _xRoot = null;

        private const int XPositionShift = 1;
        private const int YPositionShift = 1;
        private float minX = 0;
        private float minY = 0;

        public PNModelHelper(string fileName, LTSCanvas canvas)
        {
            _fileName = fileName;

            // get the sensors list
            _sensors = new List<WSNSensorItem>();
            foreach (LTSCanvas.CanvasItemData item in canvas.itemsList)
            {
                if ((item.Item is WSNSensorItem) == false)
                    continue;
                WSNSensorItem add = (WSNSensorItem)item.Item;
                add.locateX = item.Item.X / 120; //kkk
                add.locateY = item.Item.Y / 120;

                if (minX == 0 || add.locateX < minX)
                    minX = add.locateX;
                if (minY == 0 || add.locateY < minY)
                    minY = add.locateY;
                _sensors.Add(add);//
            }

            // get the channels list
            _channels = new List<WSNSensorChannel>();
            foreach (Route route in canvas.diagramRouter.routes)
            {
                if ((route is WSNSensorChannel) == false)
                    continue;
                _channels.Add((WSNSensorChannel)route);
            }
            initXML();
        }

        private void initXML()
        {
            _docOut = new XmlDocument();
            _xRoot = _docOut.CreateElement(PNNode.TAG_PN);
            _xRoot.AppendChild(_docOut.CreateElement(PNNode.TAG_DECLARATION));
            _docOut.AppendChild(_xRoot);
        }

        public bool generateXML(XmlDocument docRes)
        {
            bool blRet = false;
            bool blError = false;

            XmlElement models = _docOut.CreateElement(PNNode.TAG_MODELS);
            _xRoot.AppendChild(models);

            XmlElement topologyTag = _docOut.CreateElement(PNNode.TAG_TOPOLOGY);
            
            foreach (WSNSensorItem sensor in _sensors)
            {
                XmlElement sensorTag = _docOut.CreateElement(PNNode.TAG_SENSOR);
                sensorTag.SetAttribute(PNNode.TAG_SENSOR_TYPE, sensor.NodeType.ToString());
                sensorTag.SetAttribute(PNNode.TAG_SENSOR_ID, sensor.ID.ToString());
                sensorTag.SetAttribute(PNNode.TAG_SENSOR_MODE, sensor.NodeMode.ToString());
                sensorTag.SetAttribute(PNNode.TAG_SENSOR_X, sensor.locateX.ToString());
                sensorTag.SetAttribute(PNNode.TAG_SENSOR_Y, sensor.locateY.ToString());
                topologyTag.AppendChild(sensorTag);
            }
            foreach (WSNSensorChannel channel in _channels)
            {
                XmlElement channelTag = _docOut.CreateElement(PNNode.TAG_CHANNEL);
                channelTag.SetAttribute(PNNode.TAG_CHANNEL_FROM, ((WSNSensorItem)channel.From).ID.ToString());
                channelTag.SetAttribute(PNNode.TAG_CHANNEL_TO, ((WSNSensorItem)channel.To).ID.ToString());
                channelTag.SetAttribute(PNNode.TAG_CHANNEL_ID, channel.ID);
                channelTag.SetAttribute(PNNode.TAG_CHANNEL_TYPE, channel.Type.ToString());
                channelTag.SetAttribute(PNNode.TAG_CHANNEL_MODE, channel.Mode.ToString());
                topologyTag.AppendChild(channelTag);
            }
            _xRoot.AppendChild(topologyTag);

            XmlElement model = _docOut.CreateElement(PNNode.TAG_MODEL);
            models.AppendChild(model);


            XmlElement places = _docOut.CreateElement(PNNode.TAG_PLACES);
            XmlElement transitions = _docOut.CreateElement(PNNode.TAG_TRANSITIONS);
            XmlElement arcs = _docOut.CreateElement(PNNode.TAG_ARCS);
            
            model.AppendChild(places);
            model.AppendChild(transitions);
            model.AppendChild(arcs);
            

            do
            {
                WSNPNData data = null;

                float xStartPos = 0;
                float yStartPos = 0;

                #region Add Sensor info
                Hashtable mapData = new Hashtable();
                foreach (WSNSensorItem sensor in _sensors)
                {
                    data = sensor.GeneratePNXml(docRes, sensor.ID.ToString(), sensor.locateX - minX, sensor.locateY - minY); //(docRes, xStartPos, 0);
                    if (data == null)
                    {
                        Log.d(TAG, "Failed to generate the sensor PN xml nodes");
                        blError = true;
                        break;
                    }
                    mapData[sensor.ID] = data;
                    xStartPos += XPositionShift;

                    addPNData(data, ref places, ref transitions, ref arcs);

                    // then find the channel connected with this sensor
                    foreach (WSNSensorChannel channel in _channels)
                    {
                        if (((WSNSensorItem)channel.From).ID != sensor.ID)
                            continue;
                        xStartPos = (channel.From.AbsoluteX + channel.To.AbsoluteX) / 240; //kkk

                        yStartPos = (channel.From.AbsoluteY + channel.To.AbsoluteY) / 240;

                        data = channel.GeneratePNXml(docRes, channel.ID, xStartPos - minX, yStartPos - minY);
                        if (data == null)
                        {
                            Log.d(TAG, "Failed to generate the sensor PN xml nodes");
                            blError = true;
                            break;
                        }
                        mapData[channel.ID] = data;
                        //xStartPos += XPositionShift;
                        //yStartPos += YPositionShift;

                        addPNData(data, ref places, ref transitions, ref arcs);
                    }

                    if (blError == true)
                        break;
                }
                if (blError == true)
                    break;

                // connect the model
                int fromId, toId;
                WSNPNData fromData, toData;

                foreach (WSNSensorChannel channel in _channels)
                {
                    fromId = ((WSNSensorItem)channel.From).ID;
                    toId = ((WSNSensorItem)channel.To).ID;

                    // arc from
                    fromData = (WSNPNData)mapData[fromId];
                    toData = (WSNPNData)mapData[channel.ID];
                    addArc(ref arcs, ref fromData, ref toData);

                    // arc to
                    fromData = (WSNPNData)mapData[channel.ID];
                    toData = (WSNPNData)mapData[toId];
                    addArc(ref arcs, ref fromData, ref toData);
                }
                #endregion

                #region Update model properties
                // update the property
                model.SetAttribute(PNNode.TAG_PRO_NAME, "WSN 0");
                model.SetAttribute(PNNode.TAG_MODEL_PRO_PARAM, "");
                model.SetAttribute(PNNode.TAG_MODEL_PRO_ZOOM, "1");
                model.SetAttribute(PNNode.TAG_MODEL_PRO_PCOUNTER, "0");
                model.SetAttribute(PNNode.TAG_MODEL_PRO_TCOUNTER, "0");
                #endregion

                _docOut.Save(_fileName);
                blRet = true;
            } while (false);

            return blRet;
        }

        /// <summary>
        /// Update the PN data to the final output file
        /// </summary>
        /// <param name="data">The input PN data of current item</param>
        /// <param name="places">Places node parent</param>
        /// <param name="transitions">Transitions node parent</param>
        /// <param name="arcs">Arcs node parent</param>
        private void addPNData(WSNPNData data, ref XmlElement places, ref XmlElement transitions, ref XmlElement arcs)
        {
            XmlElement[,] map = new XmlElement[,]
            {
                { data.places, places },
                { data.transitions, transitions },
                { data.arcs, arcs },
            };
            XmlDocumentFragment xFrag = null;

            for (int i = 0; i < map.GetLength(0); i++)
            {
                foreach (XmlElement xml in map[i, 0].ChildNodes)
                {
                    xFrag = _docOut.CreateDocumentFragment();
                    xFrag.InnerXml = xml.OuterXml;
                    map[i, 1].AppendChild(xFrag);
                }
            }

            //Log.d(TAG, string.Format("In: {0} --- Out: {1}", data.inPos, data.outPos));
        }

        /// <summary>
        /// Add an arc between two nodes
        /// </summary>
        /// <param name="arcs">Parent node of all arcs</param>
        /// <param name="from">From node</param>
        /// <param name="to">To node</param>
        private void addArc(ref XmlElement arcs, ref WSNPNData from, ref WSNPNData to)
        {
            XmlElement arc;
            XmlElement label;
            XmlElement position;

            float xPos, yPos, fTmp;

            arc = _docOut.CreateElement(PNNode.TAG_ARC);
            arc.SetAttribute(PNNode.TAG_ARC_PRO_FROM, from.outNode.name + from.nodeId);
            arc.SetAttribute(PNNode.TAG_ARC_PRO_TO, to.inNode.name + to.nodeId);
            arc.SetAttribute(PNNode.TAG_ARC_PRO_WEIGHT, "1");

            label = _docOut.CreateElement(PNNode.TAG_LABEL);
            position = _docOut.CreateElement(PNNode.TAG_POSITION);
            fTmp = Math.Abs(from.outNode.pos.x - to.inNode.pos.x) / 2;
            xPos = Math.Min(from.outNode.pos.x, to.inNode.pos.x) + fTmp;
            position.SetAttribute(PNNode.TAG_POSITION_X, xPos.ToString());

            fTmp = Math.Abs(from.outNode.pos.y - to.inNode.pos.y) / 2;
            yPos = Math.Min(from.outNode.pos.y, to.inNode.pos.y) + fTmp;
            position.SetAttribute(PNNode.TAG_POSITION_Y, yPos.ToString());
            position.SetAttribute(PNNode.TAG_POSITION_WIDTH, "0.25");
            label.AppendChild(position);
            arc.AppendChild(label);
            arcs.AppendChild(arc);
        }
    }
}
