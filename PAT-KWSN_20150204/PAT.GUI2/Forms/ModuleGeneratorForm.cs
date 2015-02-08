using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PAT.GUI.Forms.GenerateModule;

namespace PAT.GUI.Forms
{
    public partial class ModuleGeneratorForm : Form
    {
        public ModuleGeneratorForm()
        {
            InitializeComponent();
            this.ComboBox_SemanticModel.SelectedIndex = 0;
        }

        private void DisableControls()
        {
            this.Cursor = Cursors.WaitCursor;
            this.Button_Generate.Enabled = false;
            this.Button_Cancel.Enabled = false;
            this.Button_BrowseOutput.Enabled = false;

            this.TextBox_CustomSyntax.Enabled = false;
            this.TextBox_ModuleName.Enabled = false;
            this.TextBox_ModuleCode.Enabled = false;
            this.TextBox_OutputFile.Enabled = false;
            this.CheckBox_BDD.Enabled = false;
            this.ComboBox_SemanticModel.Enabled = false;
            this.LinkLabel_ModuleIcon.Enabled = false;

            GroupBox_Assertions.Enabled = false;
        }

        private void EnableControls()
        {
            this.Cursor = Cursors.Default;
            this.Button_Generate.Enabled = true;
            this.Button_Cancel.Enabled = true;
            this.Button_BrowseOutput.Enabled = true;

            this.TextBox_CustomSyntax.Enabled = true;
            this.TextBox_ModuleName.Enabled = true;
            this.TextBox_ModuleCode.Enabled = true;
            this.TextBox_OutputFile.Enabled = true;
            this.CheckBox_BDD.Enabled = true;
            this.ComboBox_SemanticModel.Enabled = true;
            this.LinkLabel_ModuleIcon.Enabled = true;

            GroupBox_Assertions.Enabled = true;
        }

        /// <summary>
        /// validate the user's inputs
        /// </summary>
        /// <returns></returns>
        private bool ValidateFields(ref GenerateOption option)
        {
            // if the params are null, then return
            //-----------------------------------------------------------------------
            if (TextBox_ModuleName.Text == null || TextBox_ModuleName.Text.Equals(""))
            {
                MessageBox.Show("Module Name CANNOT be empty!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (TextBox_ModuleCode.Text == null || TextBox_ModuleCode.Text.Equals(""))
            {
                MessageBox.Show("Module Code CANNOT be empty!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (TextBox_ModuleCode.Text.Split(' ').Length != 1)
            {
                MessageBox.Show("Module Code must be a single word with pure string!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(string.IsNullOrEmpty(PictureBox_ModuleIcon.ImageLocation))
            {
                MessageBox.Show("Please select a module icon!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
            var customerSyntax = new List<string>();
            customerSyntax.AddRange(TextBox_CustomSyntax.Text.Split(new char[]{',',' '}, StringSplitOptions.RemoveEmptyEntries));
            foreach (var name in customerSyntax)
            {
                if(!System.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(name))
                {
                    MessageBox.Show(string.Format("Invalid class name for C# '{0}'", name),
                                    Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }      
            if (TextBox_OutputFile.Text == null || TextBox_OutputFile.Text.Equals(""))
            {
                MessageBox.Show("Please select an output folder!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            // If all validation passed, assign the option properties
            // --------------------------------------------------------------
            option.ModuleName = TextBox_ModuleName.Text;
            option.ModuleCode = TextBox_ModuleCode.Text;
            option.CustomSyntax = customerSyntax;
            option.Semantics = ComboBox_SemanticModel.Text;
            option.IsBdd = CheckBox_BDD.Checked;
            option.Assertion = new GenerateOption.Assertions()
            {
                AssertionDeadlock = CheckBox_Deadlock.Checked,
                AssertionLTL = CheckBox_LTL.Checked,
                AssertionReachability = CheckBox_Reachabiliity.Checked,
                AssertionRefinement = CheckBox_Refinement.Checked,
                AssertionDeterminism = CheckBox_Deterministic.Checked,
                AssertionDivergence = CheckBox_Divergence.Checked
            };
            option.ModuleIconLocation = PictureBox_ModuleIcon.ImageLocation;
            option.OutputFolder = TextBox_OutputFile.Text;

            return true;
        }

        private void Button_Generate_Click(object sender, System.EventArgs e)
        {
            GenerateOption option = new GenerateOption();
            if(!ValidateFields(ref option))
            {
                return;
            }

            // disabled all controls when generating
            DisableControls();

            GenerateModuleSolution solution = new GenerateModuleSolution();

            // generate the module solution
            solution.GenerateSolution(option);

            EnableControls();

            MessageBox.Show("The target project is generated successfully!", Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);               
        }

        private void Button_BrowseOutput_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Please select a folder to store the solution";
            folderBrowser.ShowDialog();
            TextBox_OutputFile.Text = folderBrowser.SelectedPath;
            folderBrowser.Dispose();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.Filter = "Image Files (JPEG,BMP,PNG,ICO)|*.jpg;*.jpeg;*.bmp;*.png;*.ico";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PictureBox_ModuleIcon.ImageLocation = openFileDialog.FileName;
            }
            openFileDialog.Dispose();
        }
    }
}
