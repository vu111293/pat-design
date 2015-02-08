using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

using Tools.Diagrams;

using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.KWSNModule
{
    #region Data type
    public enum ChannelType
    {
        Unicast = 0,
        //Broadcast,
        //Multicast,
    }

    public enum ChannelMode
    {
        Normal = 0,
        Lost,
    }
    #endregion

    public class WSNSensorChannel : Route, IWSNBase
    {
        #region XML attribute define
        protected const string TAG_PRO_CHANNEL_TYPE = "CType";
        protected const string TAG_PRO_CHANNEL_MODE = "CMode";
        protected const string TAG_PRO_CHANNEL_ID = "id";
        #endregion

        #region Constructor
        public WSNSensorChannel(IRectangle from, IRectangle to)
            : base(from, to)
        {
            //_docRes = PNDocRes;

            int srcID = -1;
            int dstID = -1;

            try
            {
                srcID = ((WSNSensorItem)from).ID;
            }
            catch { }

            try
            {
                dstID = ((WSNSensorItem)to).ID;
            }
            catch { }

            if (srcID > 0 && dstID > 0)
                ID = "" + srcID + dstID;
        }
        #endregion

        #region Properties
        private ChannelType _channelType = ChannelType.Unicast;
        public virtual ChannelType Type
        {
            get { return _channelType; }
            set { _channelType = value; }
        }

        private ChannelMode _channelMode = ChannelMode.Normal;
        public virtual ChannelMode Mode
        {
            get { return _channelMode; }
            set { _channelMode = value; }
        }

        private string _id = null;
        public virtual string ID
        {
            get { return _id; }
            set { _id = value; }
        }
        #endregion

        #region Data saving/loading
        public override void LoadFromXML(XmlElement xmlElement, LTSCanvas canvas)
        {
            base.LoadFromXML(xmlElement, canvas);

            _channelType = ChannelType.Unicast;
            try
            {
                _channelType = (ChannelType)int.Parse(xmlElement.GetAttribute(TAG_PRO_CHANNEL_TYPE));
            }
            catch { }

            _channelMode = ChannelMode.Normal;
            try
            {
                _channelMode = (ChannelMode)int.Parse(xmlElement.GetAttribute(TAG_PRO_CHANNEL_MODE));
            }
            catch { }

            _id = xmlElement.GetAttribute(TAG_PRO_CHANNEL_ID);
            if (_id != null && _id.Length == 0)
                _id = null;

            //Hashtable _pros = TransitionCondition;
            //WSNUtil.LoadPNTranProperties(xmlElement, ref _pros);
        }

        public override XmlElement WriteToXml(XmlDocument doc)
        {
            XmlElement node = base.WriteToXml(doc);

            node.SetAttribute(TAG_PRO_CHANNEL_TYPE, ((int)_channelType).ToString());
            node.SetAttribute(TAG_PRO_CHANNEL_MODE, ((int)_channelMode).ToString());
            if (_id != null && _id.Length > 0)
                node.SetAttribute(TAG_PRO_CHANNEL_ID, _id);

            //WSNUtil.SavePNTranProperties(doc, node, _transitionConds);

            return node;
        }
        #endregion

        #region PN Export
        protected string GetPNId(ChannelType type, ChannelMode mode)
        {
            string id = null;

            if (type >= 0 && mode >= 0)
                id = type.ToString() + "_" + mode.ToString();

            return id;
        }

        public virtual WSNPNData GeneratePNXml(XmlDocument doc, string id, float xShift, float yShift)
        {
            string pnId = GetPNId(_channelType, _channelMode);
            WSNPNData data = WSNUtil.GetPNXml(doc, id, pnId, _id, xShift, yShift);

            return data;
        }

        //public virtual List<string> GetPNTransitions(string pnId)
        //{
        //    return WSNUtil.GetTransitions(PNRes, pnId);
        //}

        //protected XmlDocument _docRes = null;
        //public virtual XmlDocument PNRes
        //{
        //    get { return _docRes; }
        //}

        //private Hashtable _transitionConds = null;
        //public virtual Hashtable TransitionCondition
        //{
        //    get
        //    {
        //        do
        //        {
        //            if (_transitionConds != null)
        //                break;

        //            // Initial the transition condition map

        //            List<string> transitions = GetPNTransitions(GetPNId(_channelType, _channelMode));
        //            if (transitions == null || transitions.Count == 0)
        //                break;

        //            _transitionConds = new Hashtable();
        //            foreach (string name in transitions)
        //                _transitionConds[name] = null;
        //        } while (false);

        //        return _transitionConds;
        //    }
        //    set { _transitionConds = value; }
        //}
        #endregion
    }
}
