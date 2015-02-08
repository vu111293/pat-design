using System.Collections.Generic;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;

namespace PAT.Common.GUI.BPELModule
{
    public class BPELModel
    {
        public string Assertion;

        //Added it temporarily to let the code runs
        //public List<string> Filenames;
     

        public Dictionary<string, string> PathContentMap=new Dictionary<string,string>();

        public BPELModel()
        {
            Assertion = "";
            this.PathContentMap = new Dictionary<string, string>();
        }


        //public BPELModel(string declare, List<string> Filenames)
        //{
        //    this.Assertion = declare;
        //    this.Filenames = Filenames;
        //}
        public BPELModel(string declare, List<string> Paths)
        {
            Dictionary<string,string> pathContentMap=new Dictionary<string, string>();
            foreach(string s in Paths)
            {
                pathContentMap.Add(s, "");
            }
            SetValue(declare, pathContentMap);
        }
        public BPELModel(string declare,  Dictionary<string,string> pathContentMap)
        {
            SetValue(declare, pathContentMap);
        }

        public List<string> getFileName()
        {
            List<string> keys = new List<string>();

            foreach (KeyValuePair<string, string> pair in PathContentMap)
            {
                keys.Add(pair.Key);
            }

            return keys;
        }
        public void AddPath(string s)
        {
            if(!PathContentMap.ContainsKey(s))
            {
                PathContentMap.Add(s,"");
            }
        }
        public void SetValue(string declare,  Dictionary<string,string> pathContentMap)
        {
            this.Assertion = declare;
            this.PathContentMap = pathContentMap;
        }
        public static BPELModel LoadLTSFromXML(string text)
        {
            BPELModel lts = new BPELModel();
            XmlDataDocument doc = new XmlDataDocument();


            doc.LoadXml(text);

            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.ASSERTION_NODE_NAME);

            foreach (XmlElement component in sitesNodes)
            {
                lts.Assertion = component.InnerText;
            }

            sitesNodes = doc.GetElementsByTagName(Parsing.FILE_NODE_NAME);

            if (sitesNodes.Count > 0)
            {
                foreach (XmlElement component in sitesNodes)
                {
                    lts.AddPath(component.GetAttribute(Parsing.PATH_ATTR_NODE_NAME));
                }
            }
            
            return lts;
        }

        public XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();

            XmlElement BPEL = doc.CreateElement(Parsing.BPEL_NODE_NAME);
            doc.AppendChild(BPEL);

            XmlElement assertion = doc.CreateElement(Parsing.ASSERTION_NODE_NAME);
            assertion.InnerText = Assertion;

            BPEL.AppendChild(assertion);
            if (PathContentMap.Count > 0)
            {
                XmlElement files = doc.CreateElement(Parsing.FILES_NODE_NAME);
                foreach (KeyValuePair<string, string> pair in PathContentMap)
                {
                        XmlElement file = doc.CreateElement(Parsing.FILE_NODE_NAME);
                        file.SetAttribute(Parsing.PATH_ATTR_NODE_NAME, pair.Key);
                        if (!string.IsNullOrEmpty(pair.Value.Trim()))
                        {
                            file.InnerText = pair.Value;
                        }
                        files.AppendChild(file);
                }
                BPEL.AppendChild(files);
            }

            return doc;
        }

        //public string ToSpecificationString()
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine(Assertion);
        //    foreach (KeyValuePair<string, string> pair in pathContentMap)
        //    {
        //        sb.AppendLine(pair.Key);
        //    }

        //    return sb.ToString();
        //}
    }
}