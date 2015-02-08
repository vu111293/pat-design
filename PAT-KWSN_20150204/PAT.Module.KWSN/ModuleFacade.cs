using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using PAT.Common;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.LTS;
using PAT.KWSN.Ultility;

namespace PAT.KWSN
{
    public sealed class ModuleFacade : ModuleFacadeBase
    {
        public ModuleFacade() { }

        public override string ModuleName
        {
            get { return "KWSN Model"; }
        }

        public override Image ModuleIcon
        {
            get
            {
                try
                {
                    Assembly myAssembly = Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream("PAT.KWSN.wireless-icon.jpg");
                    Bitmap image = new Bitmap(myStream);
                    return image;
                }
                catch (Exception)
                {
                    return Common.Ultility.Ultility.GetModuleImage("Error");
                }
            }
        }

        protected override SpecificationBase InstanciateSpecification(string text, string options, string filePath)
        {
            return new Specification(text, options, filePath);
        }

        #region Templates
        public override List<string> GetTemplateTypes()
        {
            List<string> modelTypes = new List<string>();
            modelTypes.Add("Assertions");
            modelTypes.Add("Others");

            return modelTypes;
        }

        public override SortedList<string, string> GetTemplateNames(string type)
        {
            SortedList<string, string> templates = new SortedList<string, string>();
            if (type == "Assertions")
            {
                templates.Add("Deadlock Checking", "Deadlock Checking");
                templates.Add("LTL Checking", "Linear Temparal Logic (LTL) Checking");
                templates.Add("Reachability Checking", "Reachability Checking");
                templates.Add("Refinement Checking", "Refinement Checking");
            }
            else if (type == "Others")
            {
                templates.Add("Linearizability Checking", "Linearizability Checking");
            }

            return templates;
        }

        public override string GetTemplateModel(string templateName)
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
        #endregion
    }
}
