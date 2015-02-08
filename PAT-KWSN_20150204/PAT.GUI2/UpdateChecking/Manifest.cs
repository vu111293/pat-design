using System;
using System.IO;
using System.Net;
using System.Xml;

namespace PAT.GUI.UpdateChecking
{
    public class Manifest
    {
        #region Static Protected Methods

        static protected XmlNamespaceManager GetNamespaceManager(XmlDocument doc)
        {
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace(string.Empty, "urn:schemas-microsoft-com:asm.v2");
            nsmgr.AddNamespace("dsig", "http://www.w3.org/2000/09/xmldsig#");
            nsmgr.AddNamespace("asmv1", "urn:schemas-microsoft-com:asm.v1");
            nsmgr.AddNamespace("asmv2", "urn:schemas-microsoft-com:asm.v2");
            nsmgr.AddNamespace("xrml", "urn:mpeg:mpeg21:2003:01-REL-R-NS");
            nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            return nsmgr;
        }

        #endregion

        #region Private Fields

        private XmlDocument _Document;
        private XmlNamespaceManager _NamespaceManager;

        #endregion

        #region Protected Properties

        protected XmlDocument Document
        {
            get { return _Document; }
            set { _Document = value; }
        }

        protected XmlNamespaceManager NamespaceManager
        {
            get { return _NamespaceManager; }
            set { _NamespaceManager = value; }
        }        

        #endregion

        #region Public Properties


        public Version CurrentPublishedVersion
        {
            get
            {
                if (Document != null)
                {                    
                    // Determine the version of the application from the server
                    XmlElement elm = Document.SelectSingleNode(
                                         "/asmv1:assembly/asmv1:assemblyIdentity",
                                         NamespaceManager) as XmlElement;

                    return new Version(elm.Attributes["version"].Value);
                }
                return new Version(0, 0, 0, 0);
            }
        }


        #endregion

        #region Constructors


        public Manifest(Uri uri) : this(uri, null, null) {}        
        public Manifest(Uri uri, string username, string password)
        {
            LoadFromUri(uri, username, password);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Given a URI downloads the XML Manifest file
        /// </summary>
        /// <returns>An XML Document with the contents of the manifest</returns>
        public XmlDocument LoadFromUri(Uri uri, string username, string password)
        {
            Stream clientStream = null;
            //WebClient client = null;
            WebRequest request = WebRequest.Create(uri);
            try
            {
                
                //client = new WebClient();
                
                
                if (username != null && password != null)
                {
                    request.Credentials = new NetworkCredential(username, password);
                }
                
                // Create an XPath document using the downloaded manifest
               
                //WebRequest request = ;
                
                WebResponse response = request.GetResponse();
                

                //clientStream = client.OpenRead(uri);
                clientStream = response.GetResponseStream();
                Document = new XmlDocument();
                Document.Load(clientStream);
                clientStream.Close();
                clientStream = null;

                // Load a namespace manager for this Manifest
                NamespaceManager = GetNamespaceManager(Document);

                // Record Uri, username, and password
                //Uri = uri;
                //Username = username;
                //Password = password;

                // Load assembly identity
                //XmlNode aiNode = Document.SelectSingleNode(
                //    "/asmv1:assembly/asmv1:assemblyIdentity",
                //    NamespaceManager);
                //if (aiNode != null)
                //{
                //    AssemblyIdentity = new AssemblyIdentity(this, aiNode, NamespaceManager);
                //}


                return Document;
            }
            catch (Exception)
            {
                
                throw;
            } 
            finally
            {
                if (request != null)
                {
                    //request.Dispose();
                    request = null;
                }
                if(clientStream != null)
                {
                    clientStream.Close();
                }
            }
            
        }

        #endregion
    }
}