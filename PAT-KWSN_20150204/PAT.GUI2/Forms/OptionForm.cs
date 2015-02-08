using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using PAT.Common;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Ultility;
using PAT.Common.GUI;

namespace PAT.GUI.Forms
{
    public partial class OptionForm : Form
    {
        private List<OptionPanelInterface> Tabs = new List<OptionPanelInterface>();
        public OptionForm()
        {
            InitializeComponent();

            this.NUD_MC_Initial.Value = Ultility.MC_INITIAL_SIZE;
            this.NUD_SimulationBound.Value = Ultility.SIMULATION_BOUND;
            this.NUD_CutNumber.Value = Ultility.ABSTRACT_CUT_NUMBER;
            this.NUD_CutNumberBound.Value = Ultility.ABSTRACT_CUT_NUMBER_BOUND;
            

            CheckBox_CheckUpdate.Checked = GUIUltility.CHECK_UPDATE_AT_START;
            CheckBox_EmailSSL.Checked = Common.Ultility.Ultility.SEND_EMAIL_USE_SSL;

            CheckBox_LinkCSP.Checked = GUIUltility.LINK_CSP;
            

            CheckBox_PerformDeterminization.Checked = Ultility.PERFORM_DETERMINIZATION;

            CheckBox_AutoSave.Checked = GUIUltility.AUTO_SAVE;

            this.ComboBox_DefaultModelingLanguage.Items.Add("");
            this.ComboBox_DefaultModelingLanguage.Items.AddRange(Common.Ultility.Ultility.ModuleNames.ToArray());

            this.ComboBox_DefaultModelingLanguage.Text = GUIUltility.DEFAULT_MODELING_LANGUAGE;


            //BDD
            this.DisplayBDDSettings();

            foreach (KeyValuePair<string, ModuleFacadeBase> pair in PAT.Common.Ultility.Ultility.ModuleDictionary)
            {
                OptionPanelInterface tab = pair.Value.GetOptionPanel();
                if(tab != null)
                {
                    tab.Dock = DockStyle.Fill;
                    tab.BackColor = Color.Transparent;
                    tab.LoadData();
                    Tabs.Add(tab);
                    TabPage tagpage = new TabPage(pair.Key);
                    tagpage.Controls.Add(tab);
                    tagpage.BackColor = Color.Transparent;
                    tagpage.UseVisualStyleBackColor = true;
                    this.tabControl1.TabPages.Add(tagpage);


                }                
            }


        }

        private void Button_OK_Click(object sender, EventArgs e)
        {
            
            try
            {
                Ultility.MC_INITIAL_SIZE = (int)this.NUD_MC_Initial.Value;
                Ultility.SIMULATION_BOUND = (int)this.NUD_SimulationBound.Value;
                GUIUltility.CHECK_UPDATE_AT_START = CheckBox_CheckUpdate.Checked;
                Ultility.ABSTRACT_CUT_NUMBER = (int)this.NUD_CutNumber.Value;
                Ultility.ABSTRACT_CUT_NUMBER_BOUND = (int)this.NUD_CutNumberBound.Value;
                GUIUltility.DEFAULT_MODELING_LANGUAGE = this.ComboBox_DefaultModelingLanguage.Text;
                Ultility.PERFORM_DETERMINIZATION = CheckBox_PerformDeterminization.Checked;

                Common.Ultility.Ultility.SEND_EMAIL_USE_SSL = CheckBox_EmailSSL.Checked;

                GUIUltility.AUTO_SAVE = CheckBox_AutoSave.Checked;

                GUIUltility.LINK_CSP = CheckBox_LinkCSP.Checked;

                if (CheckBox_LinkCSP.Checked)
                {                    
                    if (Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.csp") != null)
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.csp");
                    }
                    Registry.ClassesRoot.CreateSubKey(".csp").SetValue("", "CSP", Microsoft.Win32.RegistryValueKind.String);
                    Registry.ClassesRoot.CreateSubKey(@".csp\shell\open\command").SetValue("", Application.ExecutablePath + " \"%1\"", Microsoft.Win32.RegistryValueKind.String);                    
                }
                else
                {
                    if (Registry.ClassesRoot.OpenSubKey(".csp") != null)
                    {
                        Registry.ClassesRoot.DeleteSubKeyTree(".csp");
                    }
                }

                //BDD
                this.WriteBDDData();

                foreach (OptionPanelInterface tab in Tabs)
                {
                    tab.WriteData();
                }
            }
            catch (Exception ex)
            {
                //Common.Ultility.Ultility.LogException(ex, null);
            }

            GUIUltility.SaveSettingValue();
        }

        private void Button_Default_Click(object sender, EventArgs e)
        {
            this.CheckBox_LinkCSP.Checked = true;

            this.CheckBox_AutoSave.Checked = true;
            this.CheckBox_CheckUpdate.Checked = true;
            this.CheckBox_EmailSSL.Checked = true;

            this.CheckBox_PerformDeterminization.Checked = true;

            this.NUD_MC_Initial.Value = 1048576;
            this.NUD_SimulationBound.Value = 300;
            this.NUD_CutNumber.Value = 2;
            this.NUD_CutNumberBound.Value = 50;
            this.ComboBox_DefaultModelingLanguage.SelectedIndex = 0;

            //BD
            Model.ResetDefaultValue();
            this.DisplayBDDSettings();

            foreach (OptionPanelInterface tab in Tabs)
            {
                tab.ResetDefaultValue();
            }
        }

        public void WriteBDDData()
        {
            //Variable range
            if (this.NUD_VarLowerBound.Value <= this.NUD_VarUpperBound.Value)
            {
                Model.BDD_INT_LOWER_BOUND = (int)this.NUD_VarLowerBound.Value;
                Model.BDD_INT_UPPER_BOUND = (int)this.NUD_VarUpperBound.Value;
            }
            else
            {
                MessageBox.Show("Variable upper bound value must be greater than variable lower bound!", "Incorrect Variable Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }

            //Channel message length and value
            if (this.nudMessageLower.Value <= this.nudMessageUpper.Value)
            {
                Model.MAX_MESSAGE_LENGTH = (int)this.nudMessageLength.Value;
                Model.MIN_ELEMENT_BUFFER = (int)this.nudMessageLower.Value;
                Model.MAX_ELEMENT_BUFFER = (int)this.nudMessageUpper.Value;
            }
            else
            {
                MessageBox.Show("Channel message upper bound value must be greater than channel message lower bound!", "Incorrect Channel Message Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }


            string[] eventParaMax = this.textBoxEventParaMax.Text.Split(',', ' ', '[', ']');
            List<int> intEventParaMax = new List<int>();
            foreach (string max in eventParaMax)
            {
                int a = 0;
                bool isInt = int.TryParse(max, out a);
                if (isInt)
                {
                    intEventParaMax.Add(a);
                }
            }

            string[] eventParaMin = this.textBoxEventParaMin.Text.Split(',', ' ', '[', ']');
            List<int> intEventParaMin = new List<int>();
            foreach (string min in eventParaMin)
            {
                int a = 0;
                bool isInt = int.TryParse(min, out a);
                if (isInt)
                {
                    intEventParaMin.Add(a);
                }
            }
            if (intEventParaMax.Count == intEventParaMin.Count)
            {
                bool correctFormat = true;
                for (int i = 0; i < intEventParaMax.Count; i++)
                {
                    if (intEventParaMax[i] < intEventParaMin[i])
                    {
                        correctFormat = false;
                    }
                }
                if (correctFormat)
                {
                    Model.MAX_NUMBER_EVENT_PARAMETERS = intEventParaMax.Count;
                    Model.MAX_EVENT_INDEX = intEventParaMax.ToArray();
                    Model.MIN_EVENT_INDEX = intEventParaMin.ToArray();
                    return;
                }
            }

            //
            MessageBox.Show("Event parameter maximum/minimum values are not in correct format!", "Incorrect Input Format", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        private void DisplayBDDSettings()
        {
            this.textBoxEventParaMax.Text = ArrayToString(Model.MAX_EVENT_INDEX);
            this.textBoxEventParaMin.Text = ArrayToString(Model.MIN_EVENT_INDEX);
            
            this.NUD_VarLowerBound.Value = Model.BDD_INT_LOWER_BOUND;
            this.NUD_VarUpperBound.Value = Model.BDD_INT_UPPER_BOUND;

            this.nudMessageLength.Value = Model.MAX_MESSAGE_LENGTH;
            this.nudMessageLower.Value = Model.MIN_ELEMENT_BUFFER;
            this.nudMessageUpper.Value = Model.MAX_ELEMENT_BUFFER;
        }

        private string ArrayToString(int[] intValues)
        {
            if (Model.MAX_NUMBER_EVENT_PARAMETERS == 0)
            {
                return "[]";
            }
            else
            {
                string result = "[";
                for (int i = 0; i < Model.MAX_NUMBER_EVENT_PARAMETERS - 1; i++)
                {
                    result += (intValues[i] + ", ");
                }
                result += (intValues[Model.MAX_NUMBER_EVENT_PARAMETERS - 1] + "]");

                //
                return result;
            }
        }
    }
}