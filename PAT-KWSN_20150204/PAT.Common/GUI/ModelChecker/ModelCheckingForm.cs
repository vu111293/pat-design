using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;


namespace PAT.Common.GUI
{
    public partial class ModelCheckingForm1 : Form, ResourceFormInterface
    {
        public static ModelCheckingForm1 ModelCheckingFormInstance = null;
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
       
        
        private FairnessType Fairness
        {
            get
            {
                switch (this.ComboBox_Fairness.Text)
                {
                    case "No Fairness":
                        return FairnessType.NO_FAIRNESS;
                    //case "Fairness Labels Only":
                    //    return FairnessType.FAIRNESS_LABEL_ONLY;
                    case "Event Level Weak Fairness":
                        return FairnessType.EVENT_LEVEL_WEAK_FAIRNESS;
                    case "Event Level Strong Fairness":
                        return FairnessType.EVENT_LEVEL_STRONG_FAIRNESS;
                    case "Process Level Weak Fairness":
                        return FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS;
                    case "Process Level Strong Fairness":
                        return FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS;
                    case "Strong Global Fairness":
                        return FairnessType.GLOBAL_FAIRNESS;

                }
                return FairnessType.NO_FAIRNESS;
            }            
        }

        private bool ShortestPath
        {
            get
            {
                return this.CheckBox_ShortestTrace.Checked;
            }
        }

         public ModelCheckingForm1()
         {
             InitializeComponent();
         }

        public ModelCheckingForm1(string Name, SpecificationBase spec, bool UseFairness)
        {
            ModelCheckingFormInstance = this;
            InitializeComponent();
            
            InitializeResourceText();

            this.Spec = spec;
            //timer = new Stopwatch();

            int Index = 1;
            foreach (KeyValuePair<string, AssertionBase> entry in Spec.AssertionDatabase)
            {
                ListViewItem item = new ListViewItem(new string[] { "", Index.ToString(), entry.Key });
                
                //remember the type of the assertion
                item.Tag = entry.Value.AssertionType;

                //if the assertion is LTL, the button of the view BA should be enabled.
                if (entry.Value is AssertionLTL)
                {
                    BuchiAutomata BA = (entry.Value as AssertionLTL).BA;

                    if (BA != null)
                    {
                        if (BA.HasXOperator)
                        {
                            item.SubItems[0].Tag = true;
                        }
                    }                                    
                }

                //if (entry.Value is AssertionLTLSafety)
                //{
                //    BuchiAutomata BA = (entry.Value as AssertionLTLSafety).BA;

                //    if (BA != null)
                //    {
                //        if (BA.HasXOperator)
                //        {
                //            item.SubItems[0].Tag = true;
                //        }
                //    }
                //}

                
                //set the question mark image
                item.ImageIndex = 2;

                this.ListView_Assertions.Items.Add(item);
                Index++;
            }

            ////select the first assertion
            //if (this.ListView_Assertions.Items.Count > 0)
            //{
            //    this.ListView_Assertions.Items[0].Selected = true;
            //}


            if (Name != "")
            {
                this.Text = this.Text + " - " + Name; 
            }
            
            this.StatusLabel_Text.Text = Resources.Select_an_assertion_to_start_with;

            ComboBox_Fairness.SelectedIndex = 0;

            //if there is no fair events in the system, then remove the "Fairness Label Only" entry.
            //if(!spec.HasFairEvent)
            //{
            //    ComboBox_Fairness.Items.RemoveAt(1);
            //}

            if(!UseFairness)
            {
                this.ComboBox_Fairness.Visible = false;
                this.Label_Fairness.Visible = false;
            }

            this.CheckBox_BDD.Visible = false;

        }

        public void InitializeResourceText()
        {
            resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelCheckingForm1));
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
                        //v.gViewer.Graph = g;
                        //v.TextBox_Prop.Text = g.UserData.ToString();
                        //v.gViewer.AutoSize = true;

                        v.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                Common.Ultility.Ultility.LogException(ex, Spec);
                //MessageBox.Show("Exception happened: " + ex.Message + "\r\n" + ex.StackTrace, "PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        #endregion


        #region Enabled Disable Button

        protected virtual void DisableAllControls()
        {
            this.Cursor = Cursors.WaitCursor;
            this.TextBox_Output.Cursor = Cursors.WaitCursor;
            ListView_Assertions.Enabled = false;
            this.CheckBox_Verbose.Enabled = false;
            this.CheckBox_PartialOrderReduction.Enabled = false;
            this.CheckBox_Parallel.Enabled = false;
            this.CheckBox_BDD.Enabled = false;
            this.CheckBox_CheckNonZenoness.Enabled = false;
            NumericUpDown_TimeOut.Enabled = false;

            //Panel_EMC.Enabled = false;
            ComboBox_Fairness.Enabled = false;
          

            Button_BAGraph.Enabled = false;            
            Button_Verify.Text = STOP;
            this.ToolTip.SetToolTip(Button_Verify, STOP);

            ProgressBar.Value = 0;
            ProgressBar.Visible = false;
  
            Button_SimulateWitnessTrace.Enabled = false;
            this.CheckBox_ShortestTrace.Enabled = false;
        }


        protected virtual void EnableAllControls()
        {            
            ListView_Assertions.Enabled = true;

            this.CheckBox_Parallel.Enabled = true;
            this.CheckBox_Verbose.Enabled = true;
            this.CheckBox_BDD.Enabled = true;
            this.CheckBox_CheckNonZenoness.Enabled = true;

            //the following three values 
            //ComboBox_Fairness.Enabled = true;
            this.CheckBox_PartialOrderReduction.Enabled = true;
            //this.NUD_Depth.Enabled = true;

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

                //these data will not change for SpecProcess.SharedData, so once it is created after parsing, there is no need to set them again.
                //SpecProcess.SharedData.VariableLowerBound = Valuation.VariableLowerBound;
                //SpecProcess.SharedData.VariableUpperLowerBound = Valuation.VariableUpperLowerBound;
                //SpecProcess.SharedData.AlphaDatabase = Specification.AlphaDatabase;
                //SpecProcess.SharedData.HasSyncrhonousChannel = Specification.HasSyncrhonousChannel;
                //SpecProcess.SharedData.SyncrhonousChannelNames = Specification.SyncrhonousChannelNames;


                //SpecProcess.SharedData.FairnessType = AssertionBase.FairnessType;
                //SpecProcess.SharedData.CalculateParticipatingProcess = AssertionBase.FairnessType == FairnessType.PROCESS_LEVEL_WEAK_FAIRNESS || AssertionBase.FairnessType == FairnessType.PROCESS_LEVEL_STRONG_FAIRNESS || Assertion.MustAbstract;
                //SpecProcess.SharedData.CalculateCreatedProcess = Assertion.MustAbstract;
                //SpecProcess.SharedData.DataManager = DataStore.DataManager;


                item.ImageIndex = UNKNOWN_ICON;

                Assertion = Spec.AssertionDatabase[item.SubItems[2].Text];
                Assertion.UIInitialize(this, Fairness, this.CheckBox_PartialOrderReduction.Checked, this.CheckBox_Verbose.Checked, this.CheckBox_Parallel.Checked, this.ShortestPath, this.CheckBox_BDD.Checked, this.CheckBox_CheckNonZenoness.Checked, GetParameters());


                Assertion.Action += OnAction;
                Assertion.ReturnResult += VerificationFinished;
                Assertion.Cancelled += Verification_Cancelled;
                Assertion.Failed += MC_Failed;

                seconds = 1;
                ProgressBar.Value = 0;
                //timer.Reset();
                //startMemroySize = GC.GetTotalMemory(true);
                MCTimer.Start();
                //timer.Start();
                Assertion.Start();
            }
            catch (RuntimeException e)
            {
                Common.Ultility.Ultility.LogRuntimeException(e);
                this.Close();
                return;
            }
            catch (Exception ex)
            {
                //TextBox_Output.Text += ex.Message + "\r\n" + ex.StackTrace;
                //EnableAllControls();
                Common.Ultility.Ultility.LogException(ex, Spec);
                this.Close();
                return;
            }
        }

        private int seconds = 1;
        private void MCTimer_Tick(object sender, EventArgs e)
        {
            StatusLabel_Text.Text = String.Format(Resources.Verification_has_been_running_for__0__s, seconds);
            seconds++;
            if (NumericUpDown_TimeOut.Value > 0 && seconds > NumericUpDown_TimeOut.Value)
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

                    string trace = "";
                    if (e.Exception.Data.Contains("trace"))
                    {
                        trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + e.Exception.Data["trace"].ToString();
                    }

                    if(Assertion != null)
                    {
                        TextBox_Output.Text = Resources.Total_transitions_visited_before_the_exception + Assertion.VerificationOutput.Transitions + Environment.NewLine + TextBox_Output.Text;
                    }
                    //TextBox_Output.Text += e.Exception.StackTrace + "\r\n" + TextBox_Output.Text;
                    TextBox_Output.Text = e.Exception.Message + trace + Environment.NewLine + TextBox_Output.Text;
                    TextBox_Output.Text = Resources.Exception_happened_during_the_verification + ":\r\n" + TextBox_Output.Text;

                    
                    StatusLabel_Text.Text = Resources.Runtime_Exception_Happened;

                    //MessageBox.Show("Runtime exception!\r\n" +e.Exception.Message,// + "\r\n" + ex.StackTrace,"PAT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //this.Close();
                    //return;
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
                Assertion.Clear();
                //remove the events
                Assertion.Action -= OnAction;
                Assertion.ReturnResult -= VerificationFinished;
                Assertion.Cancelled -= Verification_Cancelled;
                Assertion.Failed -= MC_Failed;
                Assertion = null;

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


                if (Common.Ultility.Ultility.IsValidLicenseAvailable == Ultility.Ultility.LicenseType.Evaluation || Common.Ultility.Ultility.IsValidLicenseAvailable == Ultility.Ultility.LicenseType.Invalid)
                {
                    if (Assertion.VerificationOutput.NoOfStates > Common.Ultility.Ultility.LicenseBoundedStateNumber)
                    {
                        ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = UNKNOWN_ICON;
                        TextBox_Output.Text = String.Format("You are running an evaluation version of PAT. The number of searched states are limited to {0}.\r\nPurchase the full version to unlock the full functions of PAT.\r\n\r\n", Common.Ultility.Ultility.LicenseBoundedStateNumber) + TextBox_Output.Text;
                        
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

                TextBox_Output.Text = Assertion.VerificationOutput.GetVerificationStatistics(this.CheckBox_Parallel.Checked && Assertion is AssertionLTL) + TextBox_Output.Text;

                if (Assertion.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
                {
                    ListView_Assertions.SelectedItems[VerificationIndex].ImageIndex = UNKNOWN_ICON;
                }
                //else if ((Assertion.VerificationResult == VerificationResultType.VALID && Assertion.AssertionType != AssertionType.Reachability) || (Assertion.VerificationResult == VerificationResultType.VALID && Assertion.AssertionType == AssertionType.Reachability && Assertion.BA != null) || (Assertion.VerificationResult == VerificationResultType.INVALID && Assertion.AssertionType == AssertionType.Reachability && Assertion.BA == null))
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
               
                TextBox_Output.SelectAll();
                TextBox_Output.SelectionFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point);
                TextBox_Output.SelectionStart = 0;
                TextBox_Output.SelectionLength = 0;
                
                ColorText(0);

                TextBox_Output.Select(0, 0);
                TextBox_Output.ScrollToCaret();

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

        private static Font fnt = new Font("Verdana", 8F, FontStyle.Regular, GraphicsUnit.Point);
        private static Font fntBold = new Font("Verdana", 8F, FontStyle.Bold, GraphicsUnit.Point);

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
                    Assertion.VerificationOutput = null;

                    TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                    StatusLabel_Text.Text = Resources.Result_Generation_Cancelled;
                    ColorText(0);
                    TextBox_Output.Select(0, 0);
                    TextBox_Output.ScrollToCaret();
                    
                }

                TextBox_Output.Text = StatusLabel_Text.Text + "\r\n\r\n" + TextBox_Output.Text; 

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
                Common.Ultility.Ultility.LogRuntimeException(ex);
                this.Close();
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
                this.Close();
            }
        }

        protected virtual void ResetUIOptions(ListViewItem item, bool resetUIvalues)
        {
            if(item == null)
            {
                ComboBox_Fairness.SelectedIndex = 0;
                ComboBox_Fairness.Enabled = false;
                Button_BAGraph.Enabled = false;
                
                CheckBox_ShortestTrace.Checked = false;
                CheckBox_ShortestTrace.Enabled = false;
                
                CheckBox_CheckNonZenoness.Checked = false;
                CheckBox_CheckNonZenoness.Enabled = false;

                Button_SimulateWitnessTrace.Enabled = false;

                Button_Verify.Enabled = true;
                Label_SelectedAssertion.Text = "";
                return;
            }

            //set text
            Label_SelectedAssertion.Text = item.SubItems[2].Text;
            Assertion = Spec.AssertionDatabase[Label_SelectedAssertion.Text];

            //the select property is an LTL property
            if ((AssertionType)item.Tag == AssertionType.LTL && !CheckBox_CheckNonZenoness.Checked)
            {
                SetFairnessChoices(Assertion.IsProcessLevelFairnessApplicable, resetUIvalues);
            }
            else
            {
                ComboBox_Fairness.SelectedIndex = 0;
                ComboBox_Fairness.Enabled = false;
            }

            if ((AssertionType)item.Tag == AssertionType.LTL || (AssertionType)item.Tag == AssertionType.LTLSafety) 
            {
                Button_BAGraph.Enabled = true;
            }
            else
            {
                Button_BAGraph.Enabled = false;
            }
            

            //if it is deadlock and reachability
            if ((AssertionType)item.Tag == AssertionType.LTL) //|| (AssertionType)item.Tag == AssertionType.Reachability || (AssertionType)item.Tag == AssertionType.LTLSafety
            {
                CheckBox_ShortestTrace.Checked = false;
                CheckBox_ShortestTrace.Enabled = false;
            }
            else
            {
                CheckBox_ShortestTrace.Enabled = true;                
            }


            if ((AssertionType)item.Tag == AssertionType.LTL || (AssertionType)item.Tag == AssertionType.LTLSafety || (AssertionType)item.Tag == AssertionType.Deadlock)
            {
                CheckBox_CheckNonZenoness.Enabled = true;
            }
            else
            {
                CheckBox_CheckNonZenoness.Checked = false;
                CheckBox_CheckNonZenoness.Enabled = false;   
            }



            //verification result handling.                
            //if ((Assertion.VerificationResult == VerificationResultType.INVALID && Assertion.AssertionType != AssertionType.Reachability) || (Assertion.AssertionType == AssertionType.Reachability && Assertion.VerificationResult == VerificationResultType.VALID))
            if (Assertion.VerificationOutput != null && Assertion.VerificationOutput.CounterExampleTrace != null && Assertion.VerificationOutput.CounterExampleTrace.Count > 0)
            {
                Button_SimulateWitnessTrace.Enabled = true;
                Button_SimulateWitnessTrace.Tag = Assertion.StartingProcess.ToString().Replace("()", ""); 
            }
            else
            {
                Button_SimulateWitnessTrace.Enabled = false;
            }

                
            Button_Verify.Enabled = true;
        }


        private void SetFairnessChoices(bool ProcessLevelFairnessApplicable, bool resetUIvalues)
        {
            //if there is no need to refill the fairness option values. 
            if (!resetUIvalues)
            {
                //if the ComboBox_Fairness needs not be to disabled.
                //we need to enable it back.
                //if(!mustAbstract || ProcessLevelFairnessApplicable)
                //{
                ComboBox_Fairness.Enabled = true;
                return;
                //}
            }

            //if (mustAbstract)
            //{
            //    if (ProcessLevelFairnessApplicable)
            //    {
            //        this.ComboBox_Fairness.Items.Clear();
            //        this.ComboBox_Fairness.Items.Add("No Fairness");
            //        this.ComboBox_Fairness.Items.Add("Event Level Weak Fairness");
            //        this.ComboBox_Fairness.Items.Add("Event Level Strong Fairness");
            //        this.ComboBox_Fairness.Items.Add("Process Level Weak Fairness");
            //        this.ComboBox_Fairness.Items.Add("Process Level Strong Fairness");
            //        this.ComboBox_Fairness.Items.Add("Strong Global Fairness");
            //    }
            //    else
            //    {
            //        this.ComboBox_Fairness.Items.Clear();
            //        this.ComboBox_Fairness.Items.Add("No Fairness");
            //        this.ComboBox_Fairness.Items.Add("Event Level Weak Fairness");
            //        this.ComboBox_Fairness.Items.Add("Event Level Strong Fairness");
            //        this.ComboBox_Fairness.Items.Add("Strong Global Fairness");
            //        return;
            //    }                
            //}
            //else
            //{
               
            if (ProcessLevelFairnessApplicable)
            {
                this.ComboBox_Fairness.Items.Clear();
                this.ComboBox_Fairness.Items.Add("No Fairness");                
                this.ComboBox_Fairness.Items.Add("Event Level Weak Fairness");
                this.ComboBox_Fairness.Items.Add("Event Level Strong Fairness");
                this.ComboBox_Fairness.Items.Add("Process Level Weak Fairness");
                this.ComboBox_Fairness.Items.Add("Process Level Strong Fairness");
                this.ComboBox_Fairness.Items.Add("Strong Global Fairness");
            }
            else
            {
                this.ComboBox_Fairness.Items.Clear();
                this.ComboBox_Fairness.Items.Add("No Fairness");
                this.ComboBox_Fairness.Items.Add("Event Level Weak Fairness");
                this.ComboBox_Fairness.Items.Add("Event Level Strong Fairness");
                this.ComboBox_Fairness.Items.Add("Strong Global Fairness");
            }

            if(ComboBox_Fairness.Items.Count > 0)
            {
                ComboBox_Fairness.SelectedIndex = 0;
            }

            ComboBox_Fairness.Enabled = true;
        }


        //private void ComboBox_Fairness_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (this.ComboBox_Fairness.Text.Contains("Strong") || PORDisabled)
        //    {
        //        CheckBox_POR.Checked = false;
        //        CheckBox_POR.Enabled = false;
        //    }
        //    else
        //    {
        //        CheckBox_POR.Checked = true;
        //        CheckBox_POR.Enabled = true;
        //    }                    
        //}



        private void CheckBox_Verbose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (CheckBox_Verbose.Checked)
                {
                    this.StatusLabel_Text.Text = Resources.Verbose_Mode_Selected;
                }
                else
                {
                    this.StatusLabel_Text.Text = Resources.Normal_Mode_Selected;
                }
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
            }
        }

        private void CheckBox_POR_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox_PartialOrderReduction.Checked)
            {
                this.StatusLabel_Text.Text = Resources.ModelCheckingForm_CheckBox_POR_CheckedChanged_Partial_Order_Reduction_Selected;
            }
            else
            {
                this.StatusLabel_Text.Text = Resources.ModelCheckingForm_CheckBox_POR_CheckedChanged_Partial_Order_Reduction_UnSelected;
            }
        }

        //this method is created
        private void CheckBox_BDD_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckBox_BDD.Checked)
            {
                //Note (Mc): this is a bit ad-hoc, since i dont know how to add a string to Resources
                this.StatusLabel_Text.Text = "Symbolic Model Checking Using BDD Selected";
            }
            else
            {
                //Note (Mc): this is a bit ad-hoc, since i dont know how to add a string to Resources
                this.StatusLabel_Text.Text = "Symbolic Model Checking Using BDD UnSelected";
            }
        }

        private void CheckBox_Parallel_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_Parallel.Checked)
            {
                this.StatusLabel_Text.Text = Resources.ModelCheckingForm_CheckBox_Parallel_CheckedChanged_Parallel_Verification_Selected;
            }
            else
            {
                this.StatusLabel_Text.Text = Resources.ModelCheckingForm_CheckBox_Parallel_CheckedChanged_Parallel_Verification_UnSelected;
            }
        }

        private void CheckBox_CheckNonZeroness_CheckedChanged(object sender, EventArgs e)
        {
            DisableAllControls();

            if (CheckBox_CheckNonZenoness.Checked)
            {
                this.StatusLabel_Text.Text = Resources.Apply_Zeno_Check_Selected;
            }
            else
            {
                this.StatusLabel_Text.Text = Resources.Apply_Zeno_Check_UnSelected;
            }

            EnableAllControls();

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
        }

        private void CheckBox_ShortestTrace_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_ShortestTrace.Checked)
            {
                this.StatusLabel_Text.Text = Resources.Shortest_Witness_Trace_Selected;
            }
            else
            {
                this.StatusLabel_Text.Text = Resources.Shortest_Witness_Trace_UnSelected;
            }
        }


    }
}