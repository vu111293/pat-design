using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using PAT.Common.GUI.Drawing;
using Tools.Diagrams;

namespace PAT.Common.GUI.PDDLModule
{
    public sealed class PDDLFile
    {
        public string Name; //file Name
        public string FilePath;//Location of the .pddl file
        public string Content;


        public PDDLFile(string name, string fPath, string content)
        {
            Name = name;
            FilePath = fPath;
            Content = content;
        }
    }

    public class PDDLModel
    {
        public Dictionary<string, PDDLFile> Problems;
        public PDDLFile Domain;
        public string SystemName;
        
        public PDDLModel()
        {
            this.Problems = new Dictionary<string, PDDLFile>();
        }

        public PDDLModel(string systemName, Dictionary<string, PDDLFile> problems, PDDLFile domain )
        {
            SystemName = systemName;
            Problems = problems;
            Domain = domain;
        }

        public void AddProblem(PDDLFile problem)
        {
            if (Problems == null)
                Problems = new Dictionary<string, PDDLFile>();

            Problems.Add(problem.Name, problem);
        }

        private static string ReadFile(string fPath)
        {
            if (!File.Exists(fPath))
                return "";

            string pddlContent;
            StreamReader sReader = null;
            try
            {
                FileStream fileStream = new FileStream(fPath, FileMode.Open, FileAccess.Read);
                sReader = new StreamReader(fileStream);
                pddlContent = sReader.ReadToEnd();
            }
            catch (DirectoryNotFoundException DNF)
            {
                pddlContent = "";
            }
            catch (FileNotFoundException FNF)
            {
                pddlContent = "";
            }
            finally
            {
                if (sReader != null)
                {
                    sReader.Close();
                }
            }

            return pddlContent;
        }

        public static PDDLModel LoadPDDLModelFromXML(string text)
        {
            PDDLModel pddlModel = new PDDLModel();
            XmlDataDocument doc = new XmlDataDocument();
            
            doc.LoadXml(text);
            
            XmlNodeList sitesNodes = doc.GetElementsByTagName(Parsing.PDDL_MODEL_NODE_NAME);
            pddlModel.SystemName = sitesNodes[0].ChildNodes[0].InnerText;
            
            sitesNodes = doc.GetElementsByTagName(Parsing.DOMAIN_NODE_NAME);

            if (sitesNodes[0].HasChildNodes)
            {
                var nameNode = ((XmlElement)sitesNodes[0]).GetElementsByTagName(Parsing.PDDL_FILE_NAME_TAG);
                var pathNode = ((XmlElement)sitesNodes[0]).GetElementsByTagName(Parsing.PDDL_FILE_PATH_TAG);
                var content = ReadFile(pathNode[0].InnerText);
                pddlModel.Domain = new PDDLFile(nameNode[0].InnerText, pathNode[0].InnerText, content);

            }
            //pddlModel.Domain = sitesNodes[0].ChildNodes[0].InnerText;
            
            sitesNodes = doc.GetElementsByTagName(Parsing.PROBLEM_NAME);

            if (sitesNodes.Count > 0)
            {
                int i = 0;
                foreach (XmlElement component in sitesNodes)
                {
                    i++;
                    string fName, fPath;

                    XmlNodeList problemName = component.GetElementsByTagName(Parsing.PDDL_FILE_NAME_TAG);

                    if (problemName == null || problemName.Count < 1)
                    {
                        fName = "problem" + i;
                    }
                    else
                    {
                        fName = problemName[0].InnerText;
                    }

                    XmlNodeList problemPath = component.GetElementsByTagName(Parsing.PDDL_FILE_PATH_TAG);
                    if (problemPath == null || problemPath.Count < 1)
                        fPath = "";
                    else
                    {
                        fPath = problemPath[0].InnerText;
                    }

                    string pddlContent = ReadFile(fPath);
                    pddlModel.AddProblem(new PDDLFile(fName, fPath, pddlContent));
                }
            }

            return pddlModel;
        }

        public XmlDocument GenerateXML()
        {
            XmlDataDocument doc = new XmlDataDocument();

            XmlElement PddlModel = doc.CreateElement(Parsing.PDDL_MODEL_NODE_NAME);
            doc.AppendChild(PddlModel);
            
            XmlElement domain = doc.CreateElement(Parsing.DOMAIN_NODE_NAME);
            XmlElement dFileName = doc.CreateElement(Parsing.PDDL_FILE_NAME_TAG);
            dFileName.InnerText = Domain != null ? this.Domain.Name : " ";
            domain.AppendChild(dFileName);
            
            XmlElement dPath = doc.CreateElement(Parsing.PDDL_FILE_PATH_TAG);
            dPath.InnerText = Domain != null ? this.Domain.FilePath : " ";
            domain.AppendChild(dPath);

            PddlModel.AppendChild(domain);

            XmlElement diagrams = doc.CreateElement(Parsing.PROBLEMS_NODE_NAME);
            foreach (var problem in this.Problems)
            {
                XmlElement problemTag = doc.CreateElement(Parsing.PROBLEM_NAME);

                XmlElement problemName = doc.CreateElement(Parsing.PDDL_FILE_NAME_TAG);
                problemName.InnerText = problem.Key;
                problemTag.AppendChild(problemName);

                XmlElement problemPath = doc.CreateElement(Parsing.PDDL_FILE_PATH_TAG);
                problemPath.InnerText = problem.Value.FilePath;
                problemTag.AppendChild(problemPath);

                diagrams.AppendChild(problemTag);
            }

            PddlModel.AppendChild(diagrams);

            return doc;
        }

        public string ToSpecificationString()
        {
            return GenerateXML().ToString();
        }

        //to change space into '?'
        private static string EncodePath(string path)
        {
            int length = path.Length;

            string newPath = "";
            
            char code = '?';

            for (int i = 0; i < length; i++)
            {
                if(path[i] == ' ')
                {
                    newPath = newPath + code;
                }
                else
                {
                    newPath = newPath + path[i];
                }
            }

            return newPath;
        }
    }
}