using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PAT.GUI.Forms
{
    public partial class NewWizardForm : Form
    {
        private string selectedModule;

        public string SelectedModule
        {
            get
            {
                return selectedModule;
            }
        }

        private string selectedTemplateName;

        public string SelectedTemplate
        {
            get
            {
                return selectedTemplateName; // this.ListView_Templates.SelectedItems[0].Text;
            }
        }

        public bool SetDefaultLanguage
        {
            get
            {
                return this.CheckBox_DefaultLanguage.Checked;
            }
        }

        public string TemplateModel
        {
            get
            {
                return Common.Ultility.Ultility.ModuleDictionary[selectedModule].GetTemplateModel(SelectedTemplate); //Common.Examples.Templates.GetTemplateModel(SelectedModel, SelectedTemplate);
            }
        }

        public NewWizardForm()
        {
            InitializeComponent();

            try
            {
                
                TreeView_Language.ImageList = Common.Ultility.Ultility.Images.ImageList;

                for (int i = 0; i < Common.Ultility.Ultility.ModuleNames.Count; i++)
                {
                    string name = Common.Ultility.Ultility.ModuleNames[i];

                    TreeNode node = this.TreeView_Language.Nodes.Add(name);
                    node.ImageKey = name;
                    node.SelectedImageKey = name;
                }

                if (this.TreeView_Language.Nodes.Count > 0)
                {
                    this.TreeView_Language.SelectedNode = this.TreeView_Language.Nodes[0];
                }
            }
            catch (Exception)
            {
                
            }            
        }

        private void TreeView_Language_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ListView_Templates.Groups.Clear();
            ListView_Templates.Items.Clear();

            this.Button_OK.Enabled = false;
            this.TextBox_Explanation.Text = "";

            if(e.Node.IsSelected)
            {
                selectedModule = this.TreeView_Language.SelectedNode.Text;
            }
            else
            {
                selectedModule = null;
            }

            try
            {
                List<string> groups = Common.Ultility.Ultility.ModuleDictionary[e.Node.Text].GetTemplateTypes();

                foreach (string name in groups)
                {
                    ListViewGroup group = this.ListView_Templates.Groups.Add(name, name);

                    SortedList<string, string> templates = Common.Ultility.Ultility.ModuleDictionary[e.Node.Text].GetTemplateNames(name);
                    foreach (KeyValuePair<string, string> list in templates)
                    {
                        ListViewItem item = this.ListView_Templates.Items.Add(list.Key);
                        item.Tag = list.Value;
                        item.Group = group;
                    }
                }
            }
            catch (Exception)
            {
        
            }


            if (this.ListView_Templates.Items.Count > 0)
            {
                this.ListView_Templates.Items[0].Selected = true;
            }
        }

        private void ListView_Templates_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                if (e.IsSelected)
                {
                    this.TextBox_Explanation.Text = e.Item.Tag.ToString();
                    selectedTemplateName = e.Item.Text;
                    this.Button_OK.Enabled = true;
                }
                else
                {
                    this.TextBox_Explanation.Text = "";
                    this.Button_OK.Enabled = false;
                    selectedTemplateName = "";
                }
            }
            catch (Exception)
            {
                
            }
            
        }

        private void Button_OK_Click(object sender, EventArgs e)
        {

        }
    }
}
