using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using PAT.Common;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.SemanticModels.LTS.Assertion;
using PAT.Common.GUI;
using PAT.Common.GUI.ModelChecker;
using PAT.GUI.Forms.GenerateModule;
using PAT.GUI.Properties;


namespace PAT.GUI.Forms
{
    public partial class ModelCheckingBatchForm : Form, ResourceFormInterface
    {
        private const string mystring = "(*)";
        private const int CORRECT_ICON = 0;
        private const int WRONG_ICON= 1;
        private const int UNKNOWN_ICON = 2;

        //localized string for the button text display
        private System.ComponentModel.ComponentResourceManager resources;
        private string STOP;
        private string VERIFY;

        protected SpecificationBase Spec;
        protected AssertionBase Assertion;
       
        //private Stopwatch timer;

        //private long startMemroySize;
        private TextWriter OutputWriter;
        

         public ModelCheckingBatchForm()
         {
             InitializeComponent();

             InitializeResourceText();

             //timer = new Stopwatch();

             foreach (string syntax in Common.Ultility.Ultility.ModuleNames)
             {
                 ComboBox_Modules.Items.Add(syntax);
             }

             if(ComboBox_Modules.Items.Count > 0)
             {
                 ComboBox_Modules.SelectedIndex = 0;
             }

  
         }

        
        public void InitializeResourceText()
        {
            resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelCheckingForm));
            STOP = resources.GetString("Button_Verify_Stop") ?? "Stop";
            VERIFY = resources.GetString("Button_Verify.Text") ?? "Start";
        }


        #region UI Operations

        

        //private void MenuItem_SaveAs_Click(object sender, EventArgs e)
        //{
        //    SaveFileDialog svd = new SaveFileDialog();
        //    svd.Title = "Save Output File";
        //    svd.Filter = "Text Files|*.txt|All files|*.*";

        //    if (svd.ShowDialog() == DialogResult.OK)
        //    {
        //        TextWriter tr = new StreamWriter(svd.FileName);
        //        tr.WriteLine(this.TextBox_Output.Text);
        //        tr.Flush();
        //        tr.Close();                
        //    }      
        //}

    
        #endregion


        #region Enabled Disable Button

        protected virtual void DisableAllControls()
        {
            this.Cursor = Cursors.WaitCursor;
            ListView_Assertions.Enabled = false;
            this.CheckBox_Verbose.Enabled = false;
            this.NUD_AdmissibleBehavior.Enabled = false;
            this.NUD_VerificationEngine.Enabled = false;
            NumericUpDown_TimeOut.Enabled = false;
            ComboBox_Modules.Enabled = false;
            CheckBox_GenerateCounterexample.Enabled = false;
            Button_AddFiles.Enabled = false;
            Button_AddFolder.Enabled = false;
            Button_RemoveFiles.Enabled = false;
            Button_BrowseOutput.Enabled = false;
            TextBox_OutputFile.Enabled = false;


            Button_GenerateReport.Enabled = false;

            Button_Verify.Text = STOP;
            ProgressBar.Value = 0;
            ProgressBar.Visible = false;
            StatusLabel_Text.Text = Resources.Verification_Starts___;
  
        }


        protected virtual void EnableAllControls()
        {


            if(OutputWriter != null)
            {
                OutputWriter.Close();
                OutputWriter = null;
            }

            ListView_Assertions.Enabled = true;

            this.CheckBox_Verbose.Enabled = true;
            this.NUD_AdmissibleBehavior.Enabled = true;
            this.NUD_VerificationEngine.Enabled = true;

            ComboBox_Modules.Enabled = true;
            CheckBox_GenerateCounterexample.Enabled = true;
            Button_AddFiles.Enabled = true;
            Button_AddFolder.Enabled = true;
            Button_RemoveFiles.Enabled = true;
            Button_BrowseOutput.Enabled = true;
            TextBox_OutputFile.Enabled = true;

            NumericUpDown_TimeOut.Enabled = true;

            Button_Verify.Text = VERIFY;
            Button_Verify.Enabled = true;
            Button_GenerateReport.Enabled = true;

            ProgressBar.Value = 0;
            ProgressBar.Visible = false;
            this.Cursor = Cursors.Default;

            MCTimer.Stop();


        }

        private void FlushString()
        {
            if (OutputWriter != null)
            {
                this.OutputWriter.Write(TextBox_Output.Text);
                this.OutputWriter.Flush();
            }
            TextBox_Output.Text = "";
        }
        #endregion



        private void Button_Verify_Click(object sender, EventArgs e)
        {
            try
            {
                if (Button_Verify.Text == STOP)
                {
                    if (Assertion != null)
                    {
                        FileIndex = ListView_Assertions.Items.Count;
                        AssertionIndex = 0;
                        Button_Verify.Enabled = false;
                        Assertion.Cancel();
                    }
                }
                else
                {

                    if (this.ListView_Assertions.Items.Count == 0)
                    {
                        MessageBox.Show(Resources.Please_select_some_input_files_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    if (string.IsNullOrEmpty(this.TextBox_OutputFile.Text))
                    {
                        MessageBox.Show(Resources.Please_input_a_valid_output_file_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (Common.Classes.Ultility.Ultility.ShareDataLock != null)
                    {                       
                        MessageBox.Show(Resources.Please_stop_verification_or_simulation_before_parsing_the_model_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                        return ;
                    }



                    OutputWriter = new StreamWriter(this.TextBox_OutputFile.Text, false, Encoding.UTF8);

                    FileIndex = 0;
                    AssertionIndex = 0;

                    foreach (ListViewItem item in this.ListView_Assertions.Items)
                    {
                        item.ImageIndex = UNKNOWN_ICON;
                    }

                    DisableAllControls();


                    StartVerification(false, true);
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

        private static int FileIndex;
        private static int AssertionIndex;
        private static List<string> Assertions;
        
        private void StartVerification(bool completed, bool correct)
        {
            FlushString();
            MCTimer.Stop();
           

            if (AssertionIndex == 0)
            {
                if (completed && FileIndex > 0)
                {
                    if (correct)
                    {
                        ListView_Assertions.Items[FileIndex - 1].ImageIndex = CORRECT_ICON;
                    }
                    else
                    {
                        ListView_Assertions.Items[FileIndex - 1].ImageIndex = WRONG_ICON;
                    }
                }

                if (FileIndex < ListView_Assertions.Items.Count)
                {
                    try
                    {
                        string file = ListView_Assertions.Items[FileIndex].SubItems[2].Text;
                        FileIndex++;

                        this.OutputWriter.WriteLine("*******************************************************");
                        this.OutputWriter.WriteLine("*" + file);
                        this.OutputWriter.WriteLine("**************************START************************");
                        StatusLabel_Text.Text = Resources.Checking_file_ + file;

                        StreamReader tr = new StreamReader(file);
                        string specString = tr.ReadToEnd();
                        tr.Close();

                        if (Spec != null)
                        {
                            Spec.UnLockSharedData();
                        }  
                        
                        string modulefolder = Common.Ultility.Ultility.ModuleFolderNames[this.ComboBox_Modules.SelectedIndex];
                        ModuleFacadeBase modulebase = Common.Ultility.Ultility.LoadModule(modulefolder);
                        Spec = modulebase.ParseSpecification(specString, "", file);
                        
                        if(Spec != null)
                        {
                            Assertions = new List<string>(Spec.AssertionDatabase.Keys);    
                        }  
                        else
                        {
                            throw new Exception("ERROR Spec!");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (FileIndex > 0)
                        {
                            ListView_Assertions.Items[FileIndex - 1].ImageIndex = WRONG_ICON;
                        }
                        this.OutputWriter.WriteLine("Error occurred: " + ex.Message + "\r\n" + ex.StackTrace);
                        StartVerification(false, false);
                    }
                }
                else
                {
                    EnableAllControls();
                    return;
                }
            }
          

            try
            {                               
                if (Assertions.Count > 0)
                {
                    OutputWriter.WriteLine("=======================================================");
                    OutputWriter.WriteLine("Assertion: " + Assertions[AssertionIndex]);
                    StatusLabel_Text.Text = Resources.Verifying_Assertion__ + Assertions[AssertionIndex];
                    FlushString();

                    Assertion = Spec.AssertionDatabase[Assertions[AssertionIndex]];
                    AssertionIndex++;
                    AssertionIndex = AssertionIndex % Assertions.Count;

                    //Assertion.UIInitialize(this, Fairness, this.CheckBox_PartialOrderReduction.Checked, this.CheckBox_Verbose.Checked, this.CheckBox_Parallel.Checked, this.ShortestPath, this.CheckBox_BDD.Checked, this.CheckBox_CheckNonZenoness.Checked, GetParameters());

                    Assertion.UIInitialize(this, (int)NUD_AdmissibleBehavior.Value, (int)NUD_VerificationEngine.Value);
                    Assertion.VerificationOutput.GenerateCounterExample = CheckBox_GenerateCounterexample.Checked;

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
                else
                {
                    StartVerification(true, true);
                }
            }
            catch (RuntimeException e)
            {
                EnableAllControls();
                Common.Ultility.Ultility.LogRuntimeException(e);
                this.Close();
                return;
            }
            catch (Exception ex)
            {
                EnableAllControls();
                Common.Ultility.Ultility.LogException(ex, Spec);
                this.Close();
                return;
            }
        }

        private int seconds = 1;
        private void MCTimer_Tick(object sender, EventArgs e)
        {
            StatusLabel_Text.Text = String.Format(Resources.Verification_has_been_running_for_, seconds);

            seconds++;
            if (NumericUpDown_TimeOut.Value > 0 && seconds > NumericUpDown_TimeOut.Value)
            {
                MCTimer.Stop();
                this.OutputWriter.WriteLine("Timed out after " + (seconds - 1) + " seconds");
                Assertion.Cancel();
            }
        }

        protected virtual void MC_Failed(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                if (e.Exception is RuntimeException)
                {
                    string trace = "";
                    if (e.Exception.Data.Contains("trace"))
                    {
                        trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + e.Exception.Data["trace"].ToString();
                    }

                    this.OutputWriter.WriteLine("Runtime Exception happened during the verification:");
                    this.OutputWriter.WriteLine(e.Exception.Message + trace);

                    //Common.Ultility.Ultility.LogRuntimeException(e.Exception as RuntimeException);
                    
                    if(Assertion != null)
                    {
                        this.OutputWriter.WriteLine("Total transitions visited before the exception:" + Assertion.VerificationOutput.Transitions);                        
                    }
                    
                    StatusLabel_Text.Text = Resources.Runtime_Exception_Happened;

                }
                else
                {
                    this.OutputWriter.WriteLine("Exception happened during the verification:");
                    this.OutputWriter.WriteLine(e.Exception.Message);

                    ////if the button is enabled -> the failure is not triggered by cancel action.
                    //if(Button_Verify.Enabled)
                    //{
                    //    Common.Ultility.Ultility.LogException(e.Exception, Spec);
                    //    Spec.UnLockSharedData();
                    //    this.Close();
                    //}
                    //else
                    //{
                    //    this.StatusLabel_Text.Text = "Verification Cancelled";
                    //}
                    
                }

                Assertion.Clear();
                //remove the events
                Assertion.Action -= OnAction;
                Assertion.ReturnResult -= VerificationFinished;
                Assertion.Cancelled -= Verification_Cancelled;
                Assertion.Failed -= MC_Failed;
                Assertion = null;

                StartVerification(true, false);

                
            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
            }
        }

        protected virtual void VerificationFinished()
        {
            try
            {
                //timer.Stop();
                //MCTimer.Stop();

                //long stopMemorySize = System.GC.GetTotalMemory(false);
                
                //TextBox_Output.Text = "Estimated Memory Used:" + (stopMemorySize - startMemroySize) / 1000.0 + "KB\r\n\r\n" + TextBox_Output.Text;
                //TextBox_Output.Text = "Time Used:" + timer.Elapsed.TotalSeconds + "s\r\n" + TextBox_Output.Text;


                //if(DBM.TimerNumber > 0)
                //{
                //    TextBox_Output.Text = "Number of Clocks Used: " + DBM.TimerNumber + "\r\n" + TextBox_Output.Text;
                //}


                //if (this.CheckBox_Parallel.Checked && Assertion is AssertionLTL)
                //{
                //    AssertionLTL ltl = Assertion as AssertionLTL;
                //    TextBox_Output.Text = "SCC Ratio: " + Math.Round(((double)ltl.SCCTotalSize / (double)Assertion.VerificationOutput.NoOfStates), 2).ToString() + "\r\n" + TextBox_Output.Text; //DataStore.DataManager.GetNoOfStates()
                //    if (ltl.SCCCount != 0)
                //    {
                //        TextBox_Output.Text = "Average SCC Size: " + (ltl.SCCTotalSize / ltl.SCCCount) + "\r\n" + TextBox_Output.Text;    
                //    }
                //    else
                //    {
                //        TextBox_Output.Text = "Average SCC Size: 0\r\n" + TextBox_Output.Text;    
                //    }
                    
                //    TextBox_Output.Text = "Total SCC states: " + ltl.SCCTotalSize + "\r\n" + TextBox_Output.Text;
                //    TextBox_Output.Text = "Number of SCC found: " + ltl.SCCCount + "\r\n" + TextBox_Output.Text;
                //}

                //TextBox_Output.Text = "Total Transitions:" + Assertion.VerificationOutput.Transitions + "\r\n" + TextBox_Output.Text;
                //string group = "";
                //if (Assertion.VerificationOutput.NoOfGroupedStates > 0)
                //{
                //    group = " (" + Assertion.VerificationOutput.NoOfGroupedStates + " states are grouped)";
                //}
                //TextBox_Output.Text = "Visited States:" + Assertion.VerificationOutput.NoOfStates + group + "\r\n" + TextBox_Output.Text; //DataStore.DataManager.GetNoOfStates() 
                //TextBox_Output.Text = "********Verification Statistics********\r\n" + TextBox_Output.Text;
                TextBox_Output.Text = Assertion.GetVerificationStatistics() + TextBox_Output.Text;

                
                //if(CheckBox_GenerateCounterexample.Checked)
                //{
                    StatusLabel_Text.Text = Resources.Generating_Result___;

                    Assertion.ReturnResult -= VerificationFinished;
                    Assertion.ReturnResult += VerificationResult;
                    Assertion.VerificationMode = false;
                    Assertion.Start();    
                //}
                //else
                //{
                //    StatusLabel_Text.Text = Resources.Verification_Completed;

                //    StartVerification(true, true);
                //}

            }
            catch (Exception ex)
            {
                Assertion.Clear();

                //remove the events
                Assertion.Action -= OnAction;
                Assertion.Cancelled -= Verification_Cancelled;
                Assertion.ReturnResult -= VerificationFinished;
                Assertion.Failed -= MC_Failed;

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
            }

            StartVerification(true, true);
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
                    Assertion = null;

                    StatusLabel_Text.Text = Resources.Verification_Cancelled;

                    //set the verification result to be unknow.
                    //ListView_Assertions.SelectedItems[0].ImageIndex = UNKNOWN_ICON;

                }
                //cancel the result generation
                else
                {
                    TextBox_Output.Text = Assertion.VerificationOutput.ResultString + TextBox_Output.Text;

                    StatusLabel_Text.Text = Resources.Result_Generation_Cancelled;
                                       
                }

                //TextBox_Output.Text = StatusLabel_Text.Text + "\r\n\r\n" + TextBox_Output.Text;
              
                StartVerification(false, false);

            }
            catch (Exception ex)
            {
                Common.Ultility.Ultility.LogException(ex, Spec);
            }
        }


        private void OnAction(string action)
        {
            TextBox_Output.Text = action + TextBox_Output.Text;            
        }


        
        protected virtual void ResetUIOptions(ListViewItem item, bool resetUIvalues)
        {
            Button_Verify.Enabled = true;
        }


   


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
                MessageBox.Show(Resources.Please_wait_for_the_verification_to_stop_first__Otherwise_it_may_cause_exceptional_behaviors_, Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK);
            }
            
        }

        private void ModelCheckingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }


        private void Button_AddFiles_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog opf = new OpenFileDialog();
                opf.Multiselect = true;

                string OpenFilter = "";
                foreach (SyntaxMode syntax in FormMain.Languages)
                {
                    if (this.ComboBox_Modules.Text == syntax.Name)
                    {
                        OpenFilter = syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|" + OpenFilter;
                    }
                    //else
                    //{
                    //    OpenFilter += syntax.Name + " (*" + syntax.ExtensionString + ")|*" + syntax.ExtensionString + "|";
                    //}
                }
                
                OpenFilter += "All File (*.*)|*.*";

                opf.Filter = OpenFilter;
                if (opf.ShowDialog() == DialogResult.OK)
                {
                    foreach (string entry in opf.FileNames)
                    {
                        ListViewItem item = new ListViewItem(new string[] {"", (this.ListView_Assertions.Items.Count + 1).ToString(), entry});
                        item.ImageIndex = 2;

                        this.ListView_Assertions.Items.Add(item);
                    }

                }
            }
            catch (Exception ex)
            {
                //Ultility.LogException(ex, null);
            }
        }

        private void Button_AddFolder_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog opf = new FolderBrowserDialog();
               
                //opf.Filter = OpenFilter;

                if (opf.ShowDialog() == DialogResult.OK)
                {
                    if(Directory.Exists(opf.SelectedPath))
                    {
                        string[] files = Directory.GetFiles(opf.SelectedPath);
                        foreach (string entry in files)
                        {
                            ListViewItem item = new ListViewItem(new string[] { "", (this.ListView_Assertions.Items.Count + 1).ToString(), entry });
                            item.ImageIndex = 2;

                            this.ListView_Assertions.Items.Add(item);
                        }    
                    }
                }
            }
            catch (Exception ex)
            {
                //Ultility.LogException(ex, null);
            }
        }

        private void Button_RemoveFiles_Click(object sender, EventArgs e)
        {
            foreach(ListViewItem item in ListView_Assertions.SelectedItems)
            {
                ListView_Assertions.Items.Remove(item);
            }            
        }

        private void Button_BrowseOutput_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = false;

            opf.Filter = "Text Files (*.txt)|*.txt|All File (*.*)|*.*";
            opf.CheckFileExists = false;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(opf.FileName))
                {
                    TextBox_OutputFile.Text = opf.FileName;
                }
                else
                {
                    try
                    {
                        StreamWriter sw = File.CreateText(opf.FileName);
                        sw.Close();

                    }
                    catch (Exception)
                    {
                    }
                    
                    if (File.Exists(opf.FileName))
                    {
                        TextBox_OutputFile.Text = opf.FileName;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ListView_Assertions.Items.Clear();
        }


        private void CheckBox_GenerateCounterexample_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox_GenerateCounterexample.Checked)
            {
                this.StatusLabel_Text.Text = Resources.Generate_Counterexample_Selected;
            }
            else
            {
                this.StatusLabel_Text.Text = Resources.Generate_Counterexample_UnSelected;
            }
        }

        private void Button_GenerateReport_Click(object sender, EventArgs e)
        {
            Format format = new Format(TextBox_OutputFile.Text);

            try
            {
                format.ReadFromDisk();
                format.WriteToExcel();
                MessageBox.Show("Generate the excel successful!", PAT.Common.Ultility.Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}