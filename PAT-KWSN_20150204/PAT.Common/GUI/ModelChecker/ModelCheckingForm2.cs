using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;


namespace PAT.Common.GUI.ModelChecker
{
    public partial class ModelCheckingForm : Form, ResourceFormInterface
    {
        public static ModelCheckingForm ModelCheckingFormInstance = null;
        private const string mystring = "(*)";
        private const string ValidString = "VALID";
        private const string NotValidString = "NOT valid";


        private const int CORRECT_ICON = 0;
        private const int WRONG_ICON= 1;
        private const int UNKNOWN_ICON = 2;
        private const int CORRECT_ICON_PRPB = 3;

        //localized string for the button text display
        private System.ComponentModel.ComponentResourceManager resources;
        private string STOP;
        private string VERIFY;

        protected SpecificationBase Spec;
        protected AssertionBase Assertion;
       
        

         public ModelCheckingForm()
         {
             InitializeComponent();
         }

        public ModelCheckingForm(string Name, SpecificationBase spec)
        {
            ModelCheckingFormInstance = this;
            InitializeComponent();
            
            InitializeResourceText();

            this.Spec = spec;

            int Index = 1;
            foreach (KeyValuePair<string, AssertionBase> entry in Spec.AssertionDatabase)
            {
                ListViewItem item = new ListViewItem(new string[] { "", Index.ToString(), entry.Key });
                
                //if the assertion is LTL, the button of the view BA should be enabled.
                if (entry.Value is AssertionLTL)
                {
                    item.Tag = "LTL";

                    //BuchiAutomata BA = (entry.Value as AssertionLTL).BA;

                    //if (BA != null)
                    //{
                    //    if (BA.HasXOperator)
                    //    {
                    //        item.SubItems[0].Tag = true;
                    //    }
                    //}                                    
                }

                
                //set the question mark image
                item.ImageIndex = 2;

                this.ListView_Assertions.Items.Add(item);
                Index++;
            }

            if (Name != "")
            {
#if DEBUG
                this.Text = this.Text + " (Debug Model) - " + Name; 
#else
            this.Text = this.Text + " - " + Name; 
#endif

            }
            
            this.StatusLabel_Text.Text = Resources.Select_an_assertion_to_start_with;
        }

        public void InitializeResourceText()
        {
            resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelCheckingForm));
            STOP = resources.GetString("Button_Verify_Stop") ?? "Stop";
            VERIFY = resources.GetString("Button_Verify.Text") ?? "Verify";
            
        }


        #region UI Operations

        
        private void TextBox_Output_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.ContextMenu.Show(this.TextBox_Output, e.Location);    
            }            
        }

        private void MenuItem_Copy_Click(object sender, EventArgs e)
        {
            this.TextBox_Output.Copy();
        }

        private void MenuItem_SelectAll_Click(object sender, EventArgs e)
        {
            this.TextBox_Output.SelectedText = this.TextBox_Output.Text;
        }

        private void MenuItem_Clear_Click(object sender, EventArgs e)
        {
            this.TextBox_Output.Text = "";
        }

        private void MenuItem_SaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog svd = new SaveFileDialog();
            svd.Title = "Save Output File";
            svd.Filter = "Text Files|*.txt|All files|*.*";

            if (svd.ShowDialog() == DialogResult.OK)
            {
                TextWriter tr = new StreamWriter(svd.FileName);
                tr.WriteLine(this.TextBox_Output.Text);
                tr.Flush();
                tr.Close();                
            }      
        }

        private void Button_BAGraph_Click(object sender, EventArgs e)
        {

            if (this.ListView_Assertions.SelectedItems.Count == 0)
            {
                MessageBox.Show(Resources.Please_select_an_assertion_first, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            try
            {
                if (this.ListView_Assertions.SelectedItems.Count == 1)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Graph g = Spec.GenerateBAGraph(this.ListView_Assertions.SelectedItems[0].SubItems[2].Text);
                    this.Cursor = Cursors.Default;

                    if (g != null)
                    {
                        LTL2AutoamataConverter v = new LTL2AutoamataConverter(g.UserData.ToString(), "");
                        v.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Common.Ultility.Ultility.LogException(ex, Spec);
            }
        }

        #endregion


        #region Enabled Disable Button

        protected virtual void DisableAllControls()
        {
            this.Cursor = Cursors.WaitCursor;
            this.TextBox_Output.Cursor = Cursors.WaitCursor;
            ListView_Assertions.Enabled = false;
            ComboBox_AdmissibleBehavior.Enabled = false;
            ComboBox_VerificationEngine.Enabled = false;
            this.CheckBox_GenerateWitnessTrace.Enabled = false;

            NumericUpDown_TimeOut.Enabled = false;

            ComboBox_AdmissibleBehavior.Enabled = false;
          

            Button_BAGraph.Enabled = false;            
            Button_Verify.Text = STOP;
            this.ToolTip.SetToolTip(Button_Verify, STOP);

            ProgressBar.Value = 0;
            ProgressBar.Visible = false;
  
            Button_SimulateWitnessTrace.Enabled = false;
        }


        protected virtual void EnableAllControls()
        {
            ListView_Assertions.Enabled = true;
            ComboBox_AdmissibleBehavior.Enabled = true;
            ComboBox_VerificationEngine.Enabled = true;
            this.CheckBox_GenerateWitnessTrace.Enabled = true;

            NumericUpDown_TimeOut.Enabled = true;

            Button_Verify.Text = VERIFY;
            ToolTip.SetToolTip(Button_Verify, VERIFY);
            ProgressBar.Value = 0;
            ProgressBar.Visible = false;
            Cursor = Cursors.Default;
            TextBox_Output.Cursor = Cursors.Default;

            if (ListView_Assertions.SelectedItems.Count == 1)
            {
                ResetUIOptions(ListView_Assertions.SelectedItems[0], false);
            }
            else
            {
                ResetUIOptions(null, false);
            }


            MCTimer.Stop();

            Spec.UnLockSharedData();


            Common.Ultility.Ultility.FlashWindowEx(this);


        }

        #endregion

        private void ListView_Assertions_DoubleClick(object sender, EventArgs e)
        {
            if (ListView_Assertions.SelectedItems.Count == 0)
            {
                MessageBox.Show(Resources.Please_select_an_assertion_first, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            VerificationIndex = 0;
            StartVerification(ListView_Assertions.SelectedItems[VerificationIndex]);
        }

        private int VerificationIndex = -1;
        private void Button_Verify_Click(object sender, EventArgs e)
        {
            try
            {
                if (Button_Verify.Text == STOP)
                {
                    if (Assertion != null)
                    {
                        Button_Verify.Enabled = false;
                        Assertion.Cancel();
                    }
                }
                else
                {
                    if (ListView_Assertions.SelectedItems.Count == 0)
                    {
                        MessageBox.Show(Resources.Please_select_an_assertion_first, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    VerificationIndex = 0;
                    StartVerification(ListView_Assertions.SelectedItems[VerificationIndex]);

                }
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
            }
        }

        /// <summary>
        /// If there is a special checking or user msg, the sub-class of Model Checking Form should override this method.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ModuleSpecificCheckPassed()
        {
            return true;
        }


        protected virtual object[] GetParameters()
        {
            return null;
        }

        private void StartVerification(ListViewItem item)
        {


            if (!ModuleSpecificCheckPassed())
            {
                return;
            }

            if (!Spec.GrabSharedDataLock())
            {
                MessageBox.Show(Resources.Please_wait_for_the_simulation_or_parsing_finished_before_verification, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            
            DisableAllControls();


            try
            {
                Spec.LockSharedData(false);

                StatusLabel_Text.Text = Resources.ModelCheckingForm_StartVerification_Verification_Starts;



                item.ImageIndex = UNKNOWN_ICON;

                Assertion = Spec.AssertionDatabase[item.SubItems[2].Text];                               
                Assertion.UIInitialize(this, ComboBox_AdmissibleBehavior.SelectedIndex == -1 ? 0 : ComboBox_AdmissibleBehavior.SelectedIndex,
                    ComboBox_VerificationEngine.SelectedIndex == -1 ? 0 : ComboBox_VerificationEngine.SelectedIndex);

                Assertion.VerificationOutput.GenerateCounterExample = CheckBox_GenerateWitnessTrace.Checked;

                Assertion.Action += OnAction;
                Assertion.ReturnResult += VerificationFinished;
                Assertion.Cancelled += Verification_Cancelled;
                Assertion.Failed += MC_Failed;

                seconds = 1;
                ProgressBar.Value = 0;
                MCTimer.Start();
                Assertion.Start();
            }
            catch (RuntimeException e)
            {
                Spec.UnLockSharedData();
                Common.Ultility.Ultility.LogRuntimeException(e);
                Button_Verify.Text = VERIFY;
                this.Close();
                return;
            }
            catch (Exception ex)
            {
                Spec.UnLockSharedData();
                Common.Ultility.Ultility.LogException(ex, Spec);
                Button_Verify.Text = VERIFY;
                this.Close();
                return;
            }
        }

        private int seconds = 1;
        private void MCTimer_Tick(object sender, EventArgs e)
        {
            StatusLabel_Text.Text = String.Format(Resources.Verification_has_been_running_for__0__s, seconds);
            seconds++;
            if (NumericUpDown_TimeOut.Value > 0 && seconds > NumericUpDown_TimeOut.Value * 60)
            {
                MCTimer.Stop();
                Button_Verify.PerformClick();
            }
        }

        protected virtual void MC_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                if (e.Exception is RuntimeException)
                {

                    Common.Ultility.Ultility.LogRuntimeException(e.Exception as RuntimeException);

                    //string trace = "";
                    //if (e.Exception.Data.Contains("trace"))
                    //{
                    //    trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + e.Exception.Data["trace"].ToString();
                    //}

                    if(Assertion != null)
                    {
                        TextBox_Output.Text = Assertion.GetVerificationStatistics() + TextBox_Output.Text;
                        TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                        RnderingOutputTextbox();

                        //TextBox_Output.Text = Resources.Total_transitions_visited_before_the_exception + Assertion.VerificationOutput.Transitions + Environment.NewLine + TextBox_Output.Text;
                    }
                    //TextBox_Output.Text += e.Exception.StackTrace + "\r\n" + TextBox_Output.Text;
                    //TextBox_Output.Text = e.Exception.Message + trace + Environment.NewLine + TextBox_Output.Text;
                    //TextBox_Output.Text = Resources.Exception_happened_during_the_verification + ":\r\n" + TextBox_Output.Text;

                    
                    StatusLabel_Text.Text = Resources.Runtime_Exception_Happened;
                }
                else
                {
                    //if the button is enabled -> the failure is not triggered by cancel action.
                    if(Button_Verify.Enabled)
                    {
                        Common.Ultility.Ultility.LogException(e.Exception, Spec);
                        Spec.UnLockSharedData();
                        this.Close();
                    }
                    else
                    {
                        this.StatusLabel_Text.Text = Resources.Verification_Cancelled;
                    }
                    
                }

                if (Assertion != null)
                {
                    Assertion.Clear();
                    //remove the events
                    Assertion.Action -= OnAction;
                    Assertion.ReturnResult -= VerificationFinished;
                    Assertion.Cancelled -= Verification_Cancelled;
                    Assertion.Failed -= MC_Failed;
                    Assertion = null;
                }

                EnableAllControls();
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened: " + ex.Message + "\r\n", // + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        protected virtual void VerificationFinished()
        {
            try
            {
                //timer.Stop();
                //                MCTimer.Stop();

                //                long stopMemorySize = System.GC.GetTotalMemory(false);

                //                TextBox_Output.Text = "Estimated Memory Used:" + (stopMemorySize - startMemroySize) / 1000.0 + "KB\r\n\r\n" + TextBox_Output.Text;
                //                TextBox_Output.Text = "Time Used:" + timer.Elapsed.TotalSeconds + "s\r\n" + TextBox_Output.Text;

                //                //TextBox_Output.Text = "\tSearch Depth:" + Assertion.SearchedDepth + "\r\n" + TextBox_Output.Text;

                //#if Statistics
                //                if(DBM.TimerNumber > 0)
                //                {
                //                    TextBox_Output.Text = "Number of Clocks Used: " + DBM.TimerNumber + "\r\n" + TextBox_Output.Text;
                //                }
                //#endif

                //                if (this.CheckBox_Parallel.Checked && Assertion is AssertionLTL)
                //                {
                //                    AssertionLTL ltl = Assertion as AssertionLTL;
                //                    TextBox_Output.Text = "SCC Ratio: " + Math.Round(((double)ltl.SCCTotalSize / (double)Assertion.VerificationOutput.NoOfStates), 2).ToString() + "\r\n" + TextBox_Output.Text; //DataStore.DataManager.GetNoOfStates()
                //                    if (ltl.SCCCount != 0)
                //                    {
                //                        TextBox_Output.Text = "Average SCC Size: " + (ltl.SCCTotalSize / ltl.SCCCount) + "\r\n" + TextBox_Output.Text;    
                //                    }
                //                    else
                //                    {
                //                        TextBox_Output.Text = "Average SCC Size: 0\r\n" + TextBox_Output.Text;    
                //                    }

                //                    TextBox_Output.Text = "Total SCC states: " + ltl.SCCTotalSize + "\r\n" + TextBox_Output.Text;
                //                    TextBox_Output.Text = "Number of SCC found: " + ltl.SCCCount + "\r\n" + TextBox_Output.Text;
                //                }

                //                TextBox_Output.Text = "Total Transitions:" + Assertion.VerificationOutput.Transitions + "\r\n" + TextBox_Output.Text;
                //                string group = "";
                //                if (Assertion.VerificationOutput.NoOfGroupedStates > 0)
                //                {
                //                    group = " (" + Assertion.VerificationOutput.NoOfGroupedStates + " states are grouped)";
                //                }
                //                TextBox_Output.Text = "Visited States:" + Assertion.VerificationOutput.NoOfStates + group + "\r\n" + TextBox_Output.Text; //DataStore.DataManager.GetNoOfStates() 
                //                TextBox_Output.Text = "********Verification Statistics********\r\n" + TextBox_Output.Text;


                if (Common.Ultility.Ultility.IsValidLicenseAvailable == Ultility.Ultility.LicenseType.Evaluation ||
                    Common.Ultility.Ultility.IsValidLicenseAvailable == Ultility.Ultility.LicenseType.Invalid)
                {
                    if (Assertion.VerificationOutput.NoOfStates > Common.Ultility.Ultility.LicenseBoundedStateNumber)
                    {
                        ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = UNKNOWN_ICON;
                        TextBox_Output.Text =
                            String.Format(
                                "You are running an evaluation version of PAT. The number of searched states are limited to {0}.\r\nPurchase the full version to unlock the full functions of PAT.\r\n\r\n",
                                Common.Ultility.Ultility.LicenseBoundedStateNumber) + TextBox_Output.Text;

                        StatusLabel_Text.Text = Resources.Verification_Completed;

                        Assertion.Clear();

                        //remove the events
                        Assertion.Action -= OnAction;
                        Assertion.Cancelled -= Verification_Cancelled;
                        Assertion.ReturnResult -= VerificationFinished;
                        Assertion.Failed -= MC_Failed;


                        EnableAllControls();
                        return;
                    }
                }

                TextBox_Output.Text =
                    Assertion.GetVerificationStatistics() +
                    TextBox_Output.Text;

                if (Assertion.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
                {
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = UNKNOWN_ICON;
                }
                else if (Assertion.VerificationOutput.VerificationResult == VerificationResultType.VALID)
                {
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = CORRECT_ICON;
                }
                else if (Assertion.VerificationOutput.VerificationResult == VerificationResultType.WITHPROBABILITY)
                {
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = CORRECT_ICON_PRPB;
                }
                else
                {
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = WRONG_ICON;
                }

                StatusLabel_Text.Text = Resources.Generating_Result;

                Assertion.ReturnResult -= VerificationFinished;
                Assertion.ReturnResult += VerificationResult;
                //Assertion.VerificationOutput.GenerateCounterExample = CheckBox_GenerateWitnessTrace.Checked;
                Assertion.VerificationMode = false;
                Assertion.Start();

            }
            catch (Exception ex)
            {
                Assertion.Clear();

                //remove the events
                Assertion.Action -= OnAction;
                Assertion.Cancelled -= Verification_Cancelled;
                Assertion.ReturnResult -= VerificationFinished;
                Assertion.Failed -= MC_Failed;
                Spec.AssertionDatabase[Label_SelectedAssertion.Text] = Assertion;

                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during verification: " + ex.Message + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        protected virtual void VerificationResult()
        {
            try
            {
                TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                StatusLabel_Text.Text = Resources.Verification_Completed;
                //Resources.ModelCheckingForm_VerificationResult_Verification_Completed;
               
                RnderingOutputTextbox();

                EnableAllControls();

            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened during verification: " + ex.Message + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Assertion.Clear();

                //remove the events
                Assertion.Action -= OnAction;
                Assertion.Cancelled -= Verification_Cancelled;
                Assertion.ReturnResult -= VerificationResult;
                Assertion.Failed -= MC_Failed;
                Spec.AssertionDatabase[Label_SelectedAssertion.Text] = Assertion;

                VerificationIndex++;
                if (VerificationIndex < this.ListView_Assertions.SelectedItems.Count)
                {
                    StartVerification(this.ListView_Assertions.SelectedItems[VerificationIndex]);
                }
            }
        }

        private void RnderingOutputTextbox()
        {
            TextBox_Output.SelectAll();
            TextBox_Output.SelectionFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
            TextBox_Output.SelectionColor = System.Drawing.Color.Black;

            TextBox_Output.SelectionStart = 0;
            TextBox_Output.SelectionLength = 0;

            ColorText(0);

            TextBox_Output.Select(0, 0);
            TextBox_Output.ScrollToCaret();
        }

        private static Font fnt = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
        private static Font fntBold = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold, GraphicsUnit.Point);

        private void ColorText(int start)
        {
            int index = int.MaxValue;
            int indexCase = 0;
            int index1 = TextBox_Output.Find(mystring, start, -1, RichTextBoxFinds.WholeWord);

            if (index1 > 0 && index1 < index)
            {
                index = index1;
                indexCase = 0;
            }

            index1 = TextBox_Output.Find("-> (", start, -1, RichTextBoxFinds.None);
            if (index1 > 0 && index1 < index)
            {
                index = index1;
                indexCase = 1;
            }

            index1 = TextBox_Output.Find(ValidString, start, -1, RichTextBoxFinds.WholeWord | RichTextBoxFinds.MatchCase);
            if (index1 > 0 && index1 < index)
            {
                index = index1;
                indexCase = 2;
            }

            index1 = TextBox_Output.Find(NotValidString, start, -1, RichTextBoxFinds.WholeWord | RichTextBoxFinds.MatchCase);
            if (index1 > 0 && index1 < index)
            {
                index = index1;
                indexCase = 3;
            }

            index1 = TextBox_Output.Find(Common.Classes.Ultility.Constants.VERFICATION_RESULT_STRING, start, -1, RichTextBoxFinds.WholeWord | RichTextBoxFinds.MatchCase);
            if (index1 >= 0 && index1 < index)
            {
                index = index1;
                indexCase = 4;
            }

            if (index != int.MaxValue)
            {
                switch (indexCase)
                {
                    case 0:
                        TextBox_Output.SelectionStart = index;
                        TextBox_Output.SelectionLength = mystring.Length;
                        TextBox_Output.SelectionFont = fnt;
                        TextBox_Output.SelectionColor = System.Drawing.Color.Red;
                        TextBox_Output.SelectionBackColor = System.Drawing.Color.Yellow;
                        ColorText(index + mystring.Length);
                        return;
                    case 1:
                        int endIndex = TextBox_Output.Find(")*", start, -1, RichTextBoxFinds.None);
                        if (endIndex > 0)
                        {
                            index = index + 3;
                            if (endIndex - index + 2 > 0)
                            {
                                TextBox_Output.SelectionStart = index;

                                TextBox_Output.SelectionLength = endIndex - index + 2;
                                TextBox_Output.SelectionFont = fnt;
                                TextBox_Output.SelectionColor = System.Drawing.Color.Red;
                                TextBox_Output.SelectionBackColor = System.Drawing.Color.Yellow;
                            }
                            ColorText(endIndex + 2);
                        }
                        return;
                    case 2:
                        TextBox_Output.SelectionStart = index;
                        TextBox_Output.SelectionLength = ValidString.Length;
                        TextBox_Output.SelectionFont = fntBold;
                        TextBox_Output.SelectionColor = System.Drawing.Color.Green;
                        ColorText(index + 2);
                        return;
                    case 3:
                        TextBox_Output.SelectionStart = index;
                        TextBox_Output.SelectionLength = NotValidString.Length;
                        TextBox_Output.SelectionFont = fntBold;
                        TextBox_Output.SelectionColor = System.Drawing.Color.Red;
                        ColorText(index + 2);
                        return;
                    case 4:
                        TextBox_Output.SelectionStart = index;
                        TextBox_Output.SelectionLength = Common.Classes.Ultility.Constants.VERFICATION_RESULT_STRING.Length;
                        TextBox_Output.SelectionFont = fntBold;
                        TextBox_Output.SelectionColor = System.Drawing.Color.Blue;
                        ColorText(index + Common.Classes.Ultility.Constants.VERFICATION_RESULT_STRING.Length);
                        return;
                }
            }
        }


        //void TextBox_DataPane_TextChanged(object sender, EventArgs e)
        //{
        //    TextBox_DataPane.Font = new Font(TextBox_DataPane.Font.FontFamily, 8, FontStyle.Regular);
        //    TextBox_DataPane.SelectAll();
        //    TextBox_DataPane.SelectionFont = TextBox_DataPane.Font;
        //    TextBox_DataPane.SelectionStart = 0;
        //    TextBox_DataPane.SelectionLength = 0;
        //}


        protected virtual void Verification_Cancelled(object sender, EventArgs e)
        {
            try
            {
                //cancel the verification 
                if(Assertion.VerificationMode)
                {
                    TextBox_Output.Text = Assertion.GetVerificationStatistics() + TextBox_Output.Text;
                    TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                    RnderingOutputTextbox();

                    Assertion.Clear();

                    //remove the events
                    Assertion.Action -= OnAction;
                    Assertion.ReturnResult -= VerificationFinished;
                    Assertion.Cancelled -= Verification_Cancelled;
                    Assertion.Failed -= MC_Failed;
                    Assertion.VerificationOutput = null;
                    Assertion = null;


                    StatusLabel_Text.Text = Resources.Verification_Cancelled;

                    //set the verification result to be unknow.
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = UNKNOWN_ICON;

                }
                //cancel the result generation
                else
                {
                    TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                    StatusLabel_Text.Text = Resources.Result_Generation_Cancelled;


                    RnderingOutputTextbox();

                    //ColorText(0);


                    //TextBox_Output.Select(0, 0);
                    //TextBox_Output.ScrollToCaret();

                    Assertion.VerificationOutput = null;
                }
                //TextBox_Output.Text = StatusLabel_Text.Text + "\r\n\r\n" + TextBox_Output.Text; 

                EnableAllControls();
                Cursor = Cursors.Default;

            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened: " + ex.Message + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void OnAction(string action)
        {
            TextBox_Output.Text = action + TextBox_Output.Text;            
        }


        private void Button_SimulateCounterExample_Click(object sender, EventArgs e)
        {
            try
            {
                PAT.Common.GUI.SimulationForm form = new PAT.Common.GUI.SimulationForm();
                form.SetSpec(this.Button_SimulateWitnessTrace.Tag.ToString(), this.Button_SimulateWitnessTrace.Tag.ToString(), Spec, Assertion);
                form.Show();
                //form.Refresh();
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened: " + ex.Message + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void ListView_Assertions_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                

                if (ListView_Assertions.SelectedItems.Count != 1)
                {
                    Assertion = null;

                    if(ListView_Assertions.SelectedItems.Count == 0)
                    {
                        //set text
                        Label_SelectedAssertion.Text = "";
                        Button_BAGraph.Enabled = false;
                        Button_SimulateWitnessTrace.Enabled = false;

                        Button_Verify.Enabled = false;
                        this.StatusLabel_Text.Text = Resources.Select_an_assertion_to_start_with;

                        ComboBox_AdmissibleBehavior.Items.Clear();
                        ComboBox_VerificationEngine.Items.Clear();
                        ComboBox_AdmissibleBehavior.Enabled = false;
                        ComboBox_VerificationEngine.Enabled = false;
                    }
                    else
                    {
                        ResetUIOptions(null, true);

                        Button_Verify.Enabled = true;
                        this.StatusLabel_Text.Text = Resources.Ready;
                    }

                }
                else
                {
                    //only check the selected item
                    //if (e.IsSelected)
                    {
                        ResetUIOptions(ListView_Assertions.SelectedItems[0], true);
                        this.StatusLabel_Text.Text = Resources.Ready;
                    }
                }
            }
            catch (RuntimeException ex)
            {
                Spec.UnLockSharedData();
                Common.Ultility.Ultility.LogRuntimeException(ex);
                this.Close();
            }
            catch (Exception ex)
            {
                Spec.UnLockSharedData();
                Common.Ultility.Ultility.LogException(ex, Spec);
                this.Close();
            }
        }

        protected virtual void ResetUIOptions(ListViewItem item, bool resetUIvalues)
        {
            if(item == null)
            {
                ComboBox_AdmissibleBehavior.Items.Clear();
                ComboBox_AdmissibleBehavior.Enabled = false;

                ComboBox_VerificationEngine.Items.Clear();
                ComboBox_VerificationEngine.Enabled = false;

                Button_BAGraph.Enabled = false;
                

                Button_SimulateWitnessTrace.Enabled = false;

                Button_Verify.Enabled = true;
                Label_SelectedAssertion.Text = "";
                return;
            }

            //set text
            Label_SelectedAssertion.Text = item.SubItems[2].Text;
            Assertion = Spec.AssertionDatabase[Label_SelectedAssertion.Text];           

            if ((string)item.Tag == "LTL") 
            {
                Button_BAGraph.Enabled = true;
            }
            else
            {
                Button_BAGraph.Enabled = false;
            }

            ComboBox_AdmissibleBehavior.Enabled = true;
            ComboBox_VerificationEngine.Enabled = true;

            Button_Verify.Enabled = true;

            if (resetUIvalues)
            {
                //System.Diagnostics.Debug.Assert(Assertion.ModelCheckingOptions.AddimissibleBehaviorsNames.Count > 0);
                ComboBox_AdmissibleBehavior.Items.Clear();

                if (Assertion.ModelCheckingOptions.AddimissibleBehaviorsNames.Count > 0)
                {
                    ComboBox_AdmissibleBehavior.Items.AddRange(Assertion.ModelCheckingOptions.AddimissibleBehaviorsNames.ToArray());
                    ComboBox_AdmissibleBehavior.SelectedIndex = 0;                   
                }
                else
                {
                    Button_Verify.Enabled = false;
                }
            }

            //ComboBox_VerificationEngine.Items.AddRange(Assertion.ModelCheckingOptions.AddimissibleBehaviors[0].VerificationEngines.ToArray());
            //ComboBox_VerificationEngine.SelectedIndex = 0;


            //verification result handling.                
            if (Assertion.VerificationOutput != null && Assertion.VerificationOutput.CounterExampleTrace != null && Assertion.VerificationOutput.CounterExampleTrace.Count > 0)
            {
                Button_SimulateWitnessTrace.Enabled = true;
                Button_SimulateWitnessTrace.Tag = Assertion.StartingProcess.ToString().Replace("()", ""); 
            }
            else
            {
                Button_SimulateWitnessTrace.Enabled = false;
            }
            
        }


        private void ComboBox_AddmissibleBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(Assertion != null)
            {
                //System.Diagnostics.Debug.Assert(Assertion.ModelCheckingOptions.AddimissibleBehaviors[ComboBox_AdmissibleBehavior.SelectedIndex].VerificationEngines.Count > 0);
                ComboBox_VerificationEngine.Items.Clear();
                if (Assertion.ModelCheckingOptions.AddimissibleBehaviors[ComboBox_AdmissibleBehavior.SelectedIndex].VerificationEngines.Count > 0)
                {
                    ComboBox_VerificationEngine.Items.AddRange(Assertion.ModelCheckingOptions.AddimissibleBehaviors[ComboBox_AdmissibleBehavior.SelectedIndex].
                            VerificationEngines.ToArray());
                    ComboBox_VerificationEngine.SelectedIndex = 0;
                    Button_Verify.Enabled = true;
                }
                else
                {
                    Button_Verify.Enabled = false;
                }
            }
        }

        private void ModelCheckingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //may the exception dialog want to force the application to close, in this case, we should not give up.
            if (Button_Verify.Text == STOP && !ExceptionDialog.ForceExit)
            {
                
                if (Assertion != null)
                {
                    Assertion.Cancel();
                }
                e.Cancel = true;
                MessageBox.Show(Resources.Please_wait_for_the_verification_to_stop_first__Otherwise_it_may_cause_exceptional_behaviors, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK);
            }
            
        }

        private void ModelCheckingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ModelCheckingFormInstance = null;
            GC.Collect();
        }
    }
}