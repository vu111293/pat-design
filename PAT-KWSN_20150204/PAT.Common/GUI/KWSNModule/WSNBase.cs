using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Web;

using PAT.Common.Ultility;

namespace PAT.Common.GUI.KWSNModule
{
    public interface IWSNBase
    {
        //Hashtable TransitionCondition
        //{
        //    set;
        //    get;
        //}

        //XmlDocument PNRes
        //{
        //    get;
        //}

        WSNPNData GeneratePNXml(XmlDocument doc, string id, float xShift, float yShift);
        //List<string> GetPNTransitions(string pnId);
    }

    public class WSNUtil
    {
        private const string TAG = "WSNUtil";

        protected const string TAG_PNTRANS_GUARDS = "PNTrProp";

        /// <summary>
        /// Get the PN xml data
        /// </summary>
        /// <param name="PNRes">PN XML resource document</param>
        /// <param name="pnId">Id (Name) of the PN model</param>
        /// <param name="itemId">Sensor/Channel item Id</param>
        /// <param name="xShift"></param>
        /// <param name="yShift"></param>
        /// <returns></returns>
        public static WSNPNData GetPNXml(XmlDocument PNRes, string id, string pnId, string itemId, float xShift, float yShift)
        {
            WSNPNData data = null;
            string tmp;

            do
            {
                if (PNRes == null)
                    break;

                if (pnId == null || pnId.Length == 0)
                    break;

                // find the sensor PN model
                string query = string.Format("//{0}[@{1}='{2}']", PNNode.TAG_MODEL, PNNode.TAG_PRO_NAME, pnId);

                XmlElement pnModel = null;
                try
                {
                    pnModel = (XmlElement)PNRes.SelectSingleNode(query).CloneNode(true);
                }
                catch
                {
                    Log.d(TAG, "Can not find the sensor model node in resource!");
                    break;
                }
                XmlNodeList nodeList = null;

                //#region Update the Transition Guards
                //do
                //{
                //    if (pnTrProp == null || pnTrProp.Count == 0)
                //        break;

                //    XmlElement xGuard = null;
                //    string tranCondition = null;
                //    foreach (string key in pnTrProp.Keys)
                //    {
                //        tranCondition = (string)pnTrProp[key];
                //        if (tranCondition == null || tranCondition.Length == 0)
                //            continue;

                //        xGuard = (XmlElement)pnModel.SelectSingleNode(string.Format("./{0}/{1}[@{2}='{3}']/{4}",
                //            PNNode.TAG_TRANSITIONS, PNNode.TAG_TRANSITION, PNNode.TAG_PRO_NAME, key, PNNode.TAG_GUARD));
                //        if (xGuard == null)
                //            continue;

                //        xGuard.InnerText = HttpUtility.HtmlEncode(tranCondition);
                //    }
                //} while (false);
                //#endregion

                #region Update node name
                // update the name of these nodes
                if (itemId == null || itemId.Length == 0)
                {
                    Log.d(TAG, "Sensor ID is invalid: " + itemId);
                    break;
                }

                // node Place, Transition
                {
                    string[] nodes = new[]
                    {
                        PNNode.TAG_PLACE,
                        PNNode.TAG_TRANSITION,
                    };
                    foreach (string node in nodes)
                    {
                        nodeList = pnModel.GetElementsByTagName(node);
                        if (nodeList == null || nodeList.Count == 0)
                            continue;

                        foreach (XmlElement xml in nodeList)
                        {
                            tmp = xml.GetAttribute(PNNode.TAG_PRO_NAME);
                            xml.SetAttribute(PNNode.TAG_PRO_NAME, tmp + itemId);
                            xml.SetAttribute(PNNode.TAG_REFERENCE_ID, id);
                        }
                    }
                }

                // node Arc
                {
                    nodeList = pnModel.GetElementsByTagName(PNNode.TAG_ARC);
                    foreach (XmlElement xml in nodeList)
                    {
                        tmp = xml.GetAttribute(PNNode.TAG_ARC_PRO_FROM);
                        xml.SetAttribute(PNNode.TAG_ARC_PRO_FROM, tmp + itemId);

                        tmp = xml.GetAttribute(PNNode.TAG_ARC_PRO_TO);
                        xml.SetAttribute(PNNode.TAG_ARC_PRO_TO, tmp + itemId);
                    }
                }
                #endregion

                #region Combine the returned data
                data = new WSNPNData();
                data.nodeId = itemId;
                data.places = (XmlElement)pnModel.GetElementsByTagName(PNNode.TAG_PLACES)[0];
                data.transitions = (XmlElement)pnModel.GetElementsByTagName(PNNode.TAG_TRANSITIONS)[0];
                data.arcs = (XmlElement)pnModel.GetElementsByTagName(PNNode.TAG_ARCS)[0];
                #endregion

                #region Update node position
                do
                {
                    if (xShift <= 0 && yShift <= 0)
                        break;

                    XmlElement[] elements = new XmlElement[]
                    {
                        data.places,
                        data.transitions,
                        data.arcs,
                    };

                    query = string.Format("//{0}", PNNode.TAG_POSITION);

                    float xPos;
                    float yPos;
                    foreach (XmlElement node in elements)
                    {
                        nodeList = node.SelectNodes(query);

                        foreach (XmlElement ele in nodeList)
                        {
                            xPos = -1;
                            yPos = -1;
                            try
                            {
                                xPos = float.Parse(ele.GetAttribute(PNNode.TAG_POSITION_X));
                            }
                            catch { }
                            try
                            {
                                yPos = float.Parse(ele.GetAttribute(PNNode.TAG_POSITION_Y));
                            }
                            catch { }
                            if (xPos < 0 && yPos < 0)
                                continue;

                            xPos += xShift;
                            yPos += yShift;

                            ele.SetAttribute(PNNode.TAG_POSITION_X, xPos.ToString());
                            ele.SetAttribute(PNNode.TAG_POSITION_Y, yPos.ToString());
                        }
                    }
                } while (false);
                #endregion

                #region Save the position
                XmlElement xNode;

                data.inNode = new NodeInfo();

                string inName = pnModel.GetAttribute(PNNode.TAG_MODEL_PRO_IN);
                string outName = pnModel.GetAttribute(PNNode.TAG_MODEL_PRO_OUT);

                if (inName == null || inName.Length == 0)
                    inName = "Input";
                query = string.Format("//*[@{0}='{1}{2}']/{3}", PNNode.TAG_PRO_NAME, inName, itemId, PNNode.TAG_POSITION);
                xNode = (XmlElement)pnModel.SelectSingleNode(query);
                if (xNode != null)
                {
                    data.inNode.name = inName;
                    data.inNode.pos = new Position();
                    try
                    {
                        data.inNode.pos.x = float.Parse(xNode.GetAttribute(PNNode.TAG_POSITION_X));
                        data.inNode.pos.y = float.Parse(xNode.GetAttribute(PNNode.TAG_POSITION_Y));
                    }
                    catch { }
                }

                data.outNode = new NodeInfo();
                if (outName == null || outName.Length == 0)
                    outName = "Output";
                query = string.Format("//*[@{0}='{1}{2}']/{3}", PNNode.TAG_PRO_NAME, outName, itemId, PNNode.TAG_POSITION);
                xNode = (XmlElement)pnModel.SelectSingleNode(query);
                if (xNode != null)
                {
                    data.outNode.name = outName;
                    data.outNode.pos = new Position();
                    try
                    {
                        data.outNode.pos.x = float.Parse(xNode.GetAttribute(PNNode.TAG_POSITION_X));
                        data.outNode.pos.y = float.Parse(xNode.GetAttribute(PNNode.TAG_POSITION_Y));
                    }
                    catch { }
                }
                #endregion
            } while (false);

            return data;
        }

        ///// <summary>
        ///// Load the transition names of a model
        ///// </summary>
        ///// <param name="PNRes">PN Xml resource</param>
        ///// <param name="pnId">Model name</param>
        ///// <returns></returns>
        //public static List<string> GetTransitions(XmlDocument PNRes, string pnId)
        //{
        //    List<string> transitions = null;

        //    do
        //    {
        //        if (PNRes == null || pnId == null || pnId.Length == 0)
        //            break;

        //        string query = string.Format("//{0}[@{1}='{2}']", PNNode.TAG_MODEL, PNNode.TAG_PRO_NAME, pnId);
        //        XmlElement model = null;
        //        try
        //        {
        //            model = (XmlElement)PNRes.SelectSingleNode(query);
        //        }
        //        catch { }
        //        if (model == null)
        //            break;

        //        query = string.Format("./{0}/{1}", PNNode.TAG_TRANSITIONS, PNNode.TAG_TRANSITION);
        //        XmlNodeList xmlTrans = model.SelectNodes(query);
        //        if (xmlTrans == null || xmlTrans.Count == 0)
        //            break;

        //        transitions = new List<string>();
        //        foreach (XmlElement xml in xmlTrans)
        //            transitions.Add(xml.GetAttribute(PNNode.TAG_PRO_NAME));
        //    } while (false);

        //    return transitions;
        //}

        ///// <summary>
        ///// Save the PN Transition properties
        ///// </summary>
        ///// <param name="doc">The output XML doc</param>
        ///// <param name="parent">Parent node</param>
        ///// <param name="tranPros">Transition conditions data</param>
        ///// <returns></returns>
        //public static bool SavePNTranProperties(XmlDocument doc, XmlElement parent, Hashtable tranPros)
        //{
        //    bool blRet = false;
        //    XmlElement xmlTransPro = null;

        //    do
        //    {
        //        if (tranPros == null || tranPros.Count == 0)
        //            break;
        //        if (doc == null || parent == null)
        //            break;

        //        xmlTransPro = doc.CreateElement(TAG_PNTRANS_GUARDS);
        //        bool hasValue = false;
        //        foreach (string key in tranPros.Keys)
        //        {
        //            if (tranPros[key] == null)
        //                continue;

        //            hasValue = true;
        //            xmlTransPro.SetAttribute(key, HttpUtility.HtmlEncode(tranPros[key]));
        //        }

        //        if (hasValue == false)
        //            break;

        //        parent.AppendChild(xmlTransPro);
        //        blRet = true;
        //    } while (false);

        //    return blRet;
        //}

        ///// <summary>
        ///// Load the PN Transition properties
        ///// </summary>
        ///// <param name="parent">Parent node</param>
        ///// <param name="transPros">Transition conditions data [out]</param>
        ///// <returns></returns>
        //public static bool LoadPNTranProperties(XmlElement parent, ref Hashtable transPros)
        //{
        //    bool blRet = false;

        //    do
        //    {
        //        if (parent == null || transPros == null)
        //            break;

        //        string query = string.Format("./{0}", TAG_PNTRANS_GUARDS);
        //        XmlElement pnTrans = (XmlElement)parent.SelectSingleNode(query);
        //        if (pnTrans == null)
        //            break;

        //        if (pnTrans.Attributes == null || pnTrans.Attributes.Count == 0)
        //            break;

        //        foreach (XmlAttribute att in pnTrans.Attributes)
        //            transPros[att.Name] = HttpUtility.HtmlDecode(att.InnerText);

        //        blRet = true;
        //    } while (false);

        //    return blRet;
        //}
    }
}
