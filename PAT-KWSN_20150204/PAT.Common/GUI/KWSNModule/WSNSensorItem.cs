using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

using PAT.Common.Ultility;
using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.KWSNModule
{
    #region Sensor type definition
    public enum SensorType
    {
        Source = 0,
        Sink,
        Intermediate,
    }

    public enum SensorMode
    {
        Normal = 0,
        Congested,
        Drop,
    }
    #endregion

    public class WSNSensorItem : StateItem, IWSNBase
    {
        public float locateX;
        public float locateY;
        private const string TAG = "WSNSensor";

        #region XML node definition
        protected const string TAG_SENSOR = "Sensor";
        protected const string TAG_PRO_NODE_TYPE = "SType";
        protected const string TAG_PRO_NODE_MODE = "SMode";
        protected const string TAG_PRO_NODE_ID = "id";
        #endregion

        #region Property define
        protected SensorType mSensorType = SensorType.Intermediate;
        public SensorType NodeType
        {
            get { return mSensorType; }
            set { mSensorType = value; }
        }

        protected SensorMode mSensorMode = SensorMode.Normal;
        public SensorMode NodeMode
        {
            get { return mSensorMode; }
            set { mSensorMode = value; }
        }

        public override bool IsInitialState
        {
            get { return initialState; }
            set
            {
                initialState = value;

                if (initialState == true)
                    NodeType = SensorType.Source;
            }
        }

        protected int _id = -1;
        public virtual int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        #endregion

        public WSNSensorItem(int id)
            : base(false, "Sensor " + id)
        {
            _id = id;
            //_docRes = PNDocRes;
        }

        #region Model Save/Load
        public override void LoadFromXml(XmlElement element)
        {
            // base load first
            base.LoadFromXml(element);

            mSensorType = SensorType.Intermediate;
            try
            {
                mSensorType = (SensorType)int.Parse(element.GetAttribute(TAG_PRO_NODE_TYPE));
            }
            catch { }
            if (mSensorType == SensorType.Source)
                initialState = true;

            mSensorMode = SensorMode.Normal;
            try
            {
                mSensorMode = (SensorMode)int.Parse(element.GetAttribute(TAG_PRO_NODE_MODE));
            }
            catch { }

            try
            {
                _id = int.Parse(element.GetAttribute(TAG_PRO_NODE_ID));
            }
            catch { }

            //Hashtable _pros = TransitionCondition;
            //WSNUtil.LoadPNTranProperties(element, ref _pros);
        }

        #region Model Saving
        protected override XmlElement CreateXmlElement(XmlDocument doc)
        {
            return doc.CreateElement(TAG_SENSOR);
        }

        protected override void FillXmlElement(XmlElement element, XmlDocument document)
        {
            base.FillXmlElement(element, document);

            element.SetAttribute(TAG_PRO_NODE_TYPE, ((int)mSensorType).ToString());
            element.SetAttribute(TAG_PRO_NODE_MODE, ((int)mSensorMode).ToString());
            if (_id > 0)
                element.SetAttribute(TAG_PRO_NODE_ID, _id.ToString());

            //WSNUtil.SavePNTranProperties(document, element, TransitionCondition);
        }
        #endregion
        #endregion

        public override void DrawToGraphics(Graphics graphics)
        {
            base.DrawToGraphics(graphics);

            // Special draw for the sink node
            if (mSensorType == SensorType.Sink)
            {
                RectangleF EllipseFill = new RectangleF(AbsoluteX + 1, AbsoluteY + 1, 18, 18);
                graphics.FillEllipse(Brushes.Black, EllipseFill);
            }
        }

        #region PN Exported
        //protected XmlDocument _docRes = null;
        //public virtual XmlDocument PNRes
        //{
        //    get { return _docRes; }
        //}

        /// <summary>
        /// Generate the PN model of this sensor
        /// </summary>
        /// <param name="PNRes">PN model resource reference document</param>
        /// <param name="xShift"></param>
        /// <param name="yShift"></param>
        /// <returns></returns>
        public virtual WSNPNData GeneratePNXml(XmlDocument doc, string id, float xShift, float yShift)
        {
            WSNPNData data = null;

            do
            {
                if (_id < 0)
                    break;

                string pnId = GetPNId(mSensorType, mSensorMode);
                if (pnId == null)
                    break;

                data = WSNUtil.GetPNXml(doc, id, pnId, _id.ToString(), xShift, yShift);
            } while (false);

            return data;
        }

        protected string GetPNId(SensorType type, SensorMode mode)
        {
            string pnId = null;

            do
            {
                if (type < 0 || mode < 0)
                    break;

                switch (type)
                {
                    case SensorType.Source:
                    case SensorType.Sink:
                        pnId = type.ToString();
                        break;

                    default:
                        pnId = mode.ToString();
                        break;
                } // switch
            } while (false);

            return pnId;
        }

        //public virtual List<string> GetPNTransitions(string pnId)
        //{
        //    return WSNUtil.GetTransitions(PNRes, pnId);
        //}

        //protected Hashtable _transitionCons = null;
        //public virtual Hashtable TransitionCondition
        //{
        //    get
        //    {
        //        do
        //        {
        //            if (_transitionCons != null)
        //                break;

        //            List<string> trans = GetPNTransitions(GetPNId(mSensorType, mSensorMode));
        //            if (trans == null || trans.Count == 0)
        //                break;

        //            _transitionCons = new Hashtable();
        //            foreach (string t in trans)
        //                _transitionCons[t] = null;
        //        } while (false);
        //        return _transitionCons;
        //    }
            
        //    set { _transitionCons = value; }
        //}
        #endregion
    }
}
