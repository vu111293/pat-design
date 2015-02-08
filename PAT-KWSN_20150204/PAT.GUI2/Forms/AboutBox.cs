using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PAT.Common;


namespace PAT.GUI.Forms
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();

            //  Initialize the AboutBox to display the product information from the assembly information.
            //  Change assembly information settings for your application through either:
            //  - Project->Properties->Application->Assembly Information
            //  - AssemblyInfo.cs
            this.Text = this.Text + " " + AssemblyTitle(Assembly.GetExecutingAssembly());
            this.Label_ProductName.Text = this.Label_ProductName.Text + ": " + AssemblyProduct;
            this.Label_Version.Text = this.Label_Version.Text + ": " + Common.Ultility.Ultility.AssemblyVersion(Assembly.GetExecutingAssembly());
            this.Label_Copyright.Text = this.Label_Copyright.Text + ": " + AssemblyCopyright;
            this.Label_CompanyName.Text = this.Label_CompanyName.Text + ": " + AssemblyCompany;
            this.Label_URL.Text = "http://www.patroot.com/";

            ListView_Modules.SmallImageList = Common.Ultility.Ultility.Images.ImageList;

            for (int i = 0; i < Common.Ultility.Ultility.ModuleNames.Count; i++)
            {
                string shortName = Common.Ultility.Ultility.ModuleFolderNames[i];
                string fullName = Common.Ultility.Ultility.ModuleNames[i];

                //Assembly assembly1 = Assembly.LoadFrom(Application.StartupPath + "\\PAT.Module." + shortName + ".dll");
                string file = Path.Combine(Path.Combine(Common.Ultility.Ultility.ModuleFolderPath, shortName), "PAT.Module." + shortName + ".dll");
                Assembly assembly1 = Assembly.LoadFrom(file);

                ListViewItem item = new ListViewItem(new string[] { "", AssemblyTitle(assembly1), Common.Ultility.Ultility.AssemblyVersion(assembly1) });
                item.Tag = AssemblyDescription(assembly1);
                item.ImageKey = fullName;
                ListView_Modules.Items.Add(item);

            }                  
        }
        
        #region Assembly Attribute Accessors

        public string AssemblyTitle(Assembly assembly)
        {
                // Get all Title attributes on this assembly
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // If there is at least one Title attribute
                if (attributes.Length > 0)
                {
                    // Select the first one
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // If it is not an empty string, return it
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
                return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);

        }


        public string AssemblyDescription(Assembly assembly)
        {
            // Get all Description attributes on this assembly
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            // If there aren't any Description attributes, return an empty string
            if (attributes.Length == 0)
                return "";
            // If there is a Description attribute, return its value
            return ((AssemblyDescriptionAttribute) attributes[0]).Description;
        }

        public string AssemblyProduct
        {
            get
            {
                // Get all Product attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // If there aren't any Product attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Product attribute, return its value
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                // Get all Copyright attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // If there aren't any Copyright attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Copyright attribute, return its value
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                // Get all Company attributes on this assembly
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // If there aren't any Company attributes, return an empty string
                if (attributes.Length == 0)
                    return "";
                // If there is a Company attribute, return its value
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void Label_URL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start("www.patroot.com/");
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, null);                
            }
        }

        private void ListView_Modules_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            this.textBoxDescription.Text = e.Item.Tag.ToString();
        }
    }
}