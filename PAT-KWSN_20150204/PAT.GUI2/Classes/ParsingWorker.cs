using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using PAT.Common;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.GUI.Classes
{

            private ParsingWorker ParsingWorker;
        private Stopwatch t = new Stopwatch();

        private void Parsing_ReturnResult()
        {
          
            SpecificationBase spec = null;
            try
            {
                t.Stop();
                //spec = CurrentModule.ParseSpecification(this.CurrentEditorTabItem.Text, option, CurrentEditorTabItem.FileName);
               
                spec = ParsingWorker.Spec;
                if (spec != null)
                {
                    CurrentEditorTabItem.Specification = spec;

                    if (spec.Errors.Count > 0)
                    {
                        string key = "";
                        foreach (KeyValuePair<string, ParsingException> pair in spec.Errors)
                        {
                            key = pair.Key;
                            break;
                        }
                        ParsingException parsingException = spec.Errors[key];
                        spec.Errors.Remove(key);
                        throw parsingException;
                    }

                    if (ParsingWorker.ShowVerbolMsg)
                    {
                        this.StatusLabel_Status.Text = Resources.Grammar_Checked;

                        MenuButton_OutputPanel.Checked = true;
                        if (enableSpecificationOutputToolStripMenuItem.Checked)
                        {
                            Output_Window.TextBox.Text =
                                string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                "\r\n" + spec.GetSpecification() + "\r\n" + Output_Window.TextBox.Text;
                        }
                        else
                        {
                            Output_Window.TextBox.Text =
                                string.Format(Resources.Specification_is_parsed_in__0_s, t.Elapsed.TotalSeconds) +
                                "\r\n" + "\r\n" + Output_Window.TextBox.Text;
                        }

                        Output_Window.Show(DockContainer);

                        if (spec.Warnings.Count > 0)
                        {
                            this.MenuButton_ErrorList.Checked = true;
                            ErrorListWindow.AddWarnings(spec.Warnings);

                            //ErrorListWindow.Show(DockContainer);
                            ShowErrorMessage();
                        }
                    }

                    //Open the translation result .csp file 
                    if (CurrentModule.ModuleName == "MDL Model")
                    {
                        ShowModel(spec.InputModelText, "CSP Model");
                        CurrentModule.ParseSpecification(spec.InputModelText, ParsingWorker.Options, CurrentEditorTabItem.FileName);
                    }

                    if (ModelExplorerWindow != null)
                    {
                        ModelExplorerWindow.DisplayTree(spec);
                        if (ModelExplorerWindow.VisibleState == DockState.DockRightAutoHide)
                        {
                            DockContainer.ActiveAutoHideContent = ModelExplorerWindow;
                        }
                    }

                    EnableAllControls();

                    //return spec;
                }
                else
                {
                    EnableAllControls();

                    //return null;
                }             
            }
           catch (ParsingException ex)
            {

              EnableAllControls();

              if (ParsingWorker.ShowVerbolMsg)
                {
                    if (spec != null)
                    {
                        ErrorListWindow.AddWarnings(spec.Warnings);
                        ErrorListWindow.AddErrors(spec.Errors);
                        //MenuButton_ErrorList.Checked = true;                        
                    }


                    CurrentEditorTabItem.HandleParsingException(ex);
                    ErrorListWindow.InsertError(ex);
                    MenuButton_ErrorList.Checked = true;
                   
                    if(ex.Line > 0)
                    {
                        MessageBox.Show(Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n"   + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n" + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    else
                    {
                        MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    ShowErrorMessage();
                }

            }
            catch (Exception ex)
            {
                EnableAllControls();
                if (ParsingWorker.ShowVerbolMsg)
                {
#if (DEBUG) 
                    MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\n" + ex.StackTrace + "\r\n\r\n" + this.Output_Window.TextBox.Text;
#else
                    MessageBox.Show("Unknow Parsing Error!", Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = "Unknow Parsing Error!\r\n\r\n" + this.Output_Window.TextBox.Text;
#endif
                }
            }
        }

        private SpecificationBase ParseSpecification(bool showVerbolMsg)
        {
            if (CurrentEditorTabItem == null || this.CurrentEditorTabItem.Text.Trim() == "")
            {
                if(showVerbolMsg)
                {
                    MessageBox.Show(Resources.Please_input_a_model_first_, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
            }

            DisableAllControls();
            SpecificationBase spec = null;
            try
            {
                //clear the error list
                if (!ErrorListWindow.IsDisposed)
                {
                    ErrorListWindow.Clear();
                }

                string moduleName = CurrentEditorTabItem.ModuleName;
                if(LoadModule(moduleName))
                {
                    string option = GetOption();

                    t.Start();

                    ParsingWorker = new ParsingWorker();
                    ParsingWorker.Initialize(this, CurrentModule, this.CurrentEditorTabItem.Text, option, CurrentEditorTabItem.FileName, showVerbolMsg);

                    ParsingWorker.ReturnResult += new ParsingWorker.ReturnResultEvent(Parsing_ReturnResult);
                    ParsingWorker.Failed += new ThreadExceptionEventHandler(ParsingWorker_Failed);
                    ParsingWorker.Start();
                }
            }
            catch (ParsingException ex)
            {

              EnableAllControls();

                if (showVerbolMsg)
                {
                    if (spec != null)
                    {
                        ErrorListWindow.AddWarnings(spec.Warnings);
                        ErrorListWindow.AddErrors(spec.Errors);
                        //MenuButton_ErrorList.Checked = true;                        
                    }


                    CurrentEditorTabItem.HandleParsingException(ex);
                    ErrorListWindow.InsertError(ex);
                    MenuButton_ErrorList.Checked = true;
                   
                    if(ex.Line > 0)
                    {
                        MessageBox.Show(Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n"   + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n" + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    else
                    {
                        MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    ShowErrorMessage();
                }

            }
            catch (Exception ex)
            {
                EnableAllControls();
                if (showVerbolMsg)
                {
#if (DEBUG) 
                    MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\n" + ex.StackTrace + "\r\n\r\n" + this.Output_Window.TextBox.Text;
#else
                    MessageBox.Show("Unknow Parsing Error!", Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = "Unknow Parsing Error!\r\n\r\n" + this.Output_Window.TextBox.Text;
#endif
                }
            }

            return null;
        }

        void ParsingWorker_Failed(object sender, ThreadExceptionEventArgs e)
        {
            if(e.Exception is ParsingException)
            {
                ParsingException ex = e.Exception as ParsingException;
                EnableAllControls();

                if (ParsingWorker.ShowVerbolMsg)
                {
                    if (ParsingWorker.Spec != null)
                    {
                        ErrorListWindow.AddWarnings(ParsingWorker.Spec.Warnings);
                        ErrorListWindow.AddErrors(ParsingWorker.Spec.Errors);
                        //MenuButton_ErrorList.Checked = true;                        
                    }


                    CurrentEditorTabItem.HandleParsingException(ex);
                    ErrorListWindow.InsertError(ex);
                    MenuButton_ErrorList.Checked = true;
                   
                    if(ex.Line > 0)
                    {
                        MessageBox.Show(Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n"   + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error_at_line_ + ex.Line + Resources._column_ + ex.CharPositionInLine + ": " + ex.Text + "\nFile: " + ex.DisplayFileName + (string.IsNullOrEmpty(ex.NodeName)?"":", Node: " + ex.NodeName) + "\n" + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    else
                    {
                        MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        MenuButton_OutputPanel.Checked = true;
                        this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\r\n\r\n" + this.Output_Window.TextBox.Text; //"\n" + ex.StackTrace +     
                    }
                    ShowErrorMessage();
                }

            }
            else // (Exception ex)
            {
                EnableAllControls();
                if (ParsingWorker.ShowVerbolMsg)
                {
                    Exception ex = e.Exception;
#if (DEBUG) 
                    MessageBox.Show(Resources.Parsing_error__ + ex.Message, Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = Resources.Parsing_error__ + ex.Message + "\n" + ex.StackTrace + "\r\n\r\n" + this.Output_Window.TextBox.Text;
#else
                    MessageBox.Show("Unknow Parsing Error!", Ultility.APPLICATION_NAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MenuButton_OutputPanel.Checked = true;
                    this.Output_Window.TextBox.Text = "Unknow Parsing Error!\r\n\r\n" + this.Output_Window.TextBox.Text;
#endif
                }
            }
        }

    public class ParsingWorker
    {

        #region Events and Multi-threading functions

        /// <summary>
        /// Launch the operation on a worker thread.  This method will
        /// return immediately, and the operation will start asynchronously
        /// on a worker thread.
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                //if (isRunning)
                //{
                //    throw new AlreadyRunningException();
                //}
                // Set this flag here, not inside InternalStart, to avoid
                // race condition when Start called twice in quick
                // succession.
                isRunning = true;
            }
            //IAsyncResult result = 
            new MethodInvoker(InternalStart).BeginInvoke(null, null);            
        }

        /// <summary>
        /// Attempt to cancel the current operation.  This returns
        /// immediately to the caller.  No guarantee is made as to
        /// whether the operation will be successfully cancelled.  All
        /// that can be known is that at some point, one of the
        /// three events Completed, Cancelled, or Failed will be raised
        /// at some point.
        /// </summary>
        public void Cancel()
        {
            lock (this)
            {
                cancelledFlag = true;
                //AcknowledgeCancel();
                //ClearDB();
            }
        }


        /// <summary>
        /// Attempt to cancel the current operation and block until either
        /// the cancellation succeeds or the operation completes.
        /// </summary>
        /// <returns>true if the operation was successfully cancelled
        /// or it failed, false if it ran to completion.</returns>
        public bool CancelAndWait()
        {
            lock (this)
            {
                // Set the cancelled flag

                cancelledFlag = true;


                // Now sit and wait either for the operation to
                // complete or the cancellation to be acknowledged.
                // (Wake up and check every second - shouldn't be
                // necessary, but it guarantees we won't deadlock
                // if for some reason the Pulse gets lost - means
                // we don't have to worry so much about bizarre
                // race conditions.)
                while (!IsDone)
                {
                    Monitor.Wait(this, 1000);                    
                }
            }
            return !HasCompleted;
        }

        /// <summary>
        /// Blocks until the operation has either run to completion, or has
        /// been successfully cancelled, or has failed with an internal
        /// exception.
        /// </summary>
        /// <returns>true if the operation completed, false if it was
        /// cancelled before completion or failed with an internal
        /// exception.</returns>
        public bool WaitUntilDone()
        {
            lock (this)
            {
                // Wait for either completion or cancellation.  As with
                // CancelAndWait, we don't sleep forever - to reduce the
                // chances of deadlock in obscure race conditions, we wake
                // up every second to check we didn't miss a Pulse.
                while (!IsDone)
                {
                    Monitor.Wait(this, 1000);
                }
            }
            return HasCompleted;
        }


        /// <summary>
        /// Returns false if the operation is still in progress, or true if
        /// it has either completed successfully, been cancelled
        ///  successfully, or failed with an internal exception.
        /// </summary>
        public bool IsDone
        {
            get
            {
                lock (this)
                {
                    return completeFlag || cancelAcknowledgedFlag || failedFlag;
                }
            }
        }

        /// <summary>
        /// This event will be fired if the operation runs to completion
        /// without being cancelled.  This event will be raised through the
        /// ISynchronizeTarget supplied at construction time.  Note that
        /// this event may still be received after a cancellation request
        /// has been issued.  (This would happen if the operation completed
        /// at about the same time that cancellation was requested.)  But
        /// the event is not raised if the operation is cancelled
        /// successfully.
        /// </summary>
        public event EventHandler Completed;


        /// <summary>
        /// This event will be fired when the operation is successfully
        /// stoped through cancellation.  This event will be raised through
        /// the ISynchronizeTarget supplied at construction time.
        /// </summary>
        public event EventHandler Cancelled;


        /// <summary>
        /// This event will be fired if the operation throws an exception.
        /// This event will be raised through the ISynchronizeTarget
        /// supplied at construction time.
        /// </summary>
        public event ThreadExceptionEventHandler Failed;


        public delegate void ActionEvent(ListViewItem item, Graph graph);
        public event ActionEvent Action;

        public delegate void PrintingEvent(string msg);
        public event PrintingEvent Print;

        public delegate void ReturnResultEvent();
        public event ReturnResultEvent ReturnResult;



        /// <summary>
        /// The ISynchronizeTarget supplied during construction - this can
        /// be used by deriving classes which wish to add their own events.
        /// </summary>
        public ISynchronizeInvoke Target
        {
            get { return isiTarget; }
        }
        protected ISynchronizeInvoke isiTarget;


        /// <summary>
        /// Flag indicating whether the request has been cancelled.  Long-
        /// running operations should check this flag regularly if they can
        /// and cancel their operations as soon as they notice that it has
        /// been set.
        /// </summary>
        protected bool CancelRequested
        {
            get
            {
                lock (this) { return cancelledFlag; }
            }
        }
        private bool cancelledFlag;


        /// <summary>
        /// Flag indicating whether the request has run through to
        /// completion.  This will be false if the request has been
        /// successfully cancelled, or if it failed.
        /// </summary>
        protected bool HasCompleted
        {
            get
            {
                lock (this) { return completeFlag; }
            }
        }
        private bool completeFlag;


        /// <summary>
        /// This is called by the operation when it wants to indicate that
        /// it saw the cancellation request and honoured it.
        /// </summary>
        protected void AcknowledgeCancel()
        {
            lock (this)
            {
                cancelAcknowledgedFlag = true;
                isRunning = false;

                // Pulse the event in case the main thread is blocked
                // waiting for us to finish (e.g. in CancelAndWait or
                // WaitUntilDone).
                Monitor.Pulse(this);

                // Using async invocation to avoid a potential deadlock
                // - using Invoke would involve a cross-thread call
                // whilst we still held the object lock.  If the event
                // handler on the UI thread tries to access this object
                // it will block because we have the lock, but using
                // async invocation here means that once we've fired
                // the event, we'll run on and release the object lock,
                // unblocking the UI thread.
                FireAsync(Cancelled, this, EventArgs.Empty);
            }
        }

        private bool cancelAcknowledgedFlag;


        // Set to true if the operation fails with an exception.
        private bool failedFlag;
        // Set to true if the operation is running
        protected bool isRunning;
        


        // This is called when the operation runs to completion.
        // (This is private because it is called automatically
        // by this base class when the deriving class's DoWork
        // method exits without having cancelled

        private void CompleteOperation()
        {
            lock (this)
            {
                completeFlag = true;
                isRunning = false;
                Monitor.Pulse(this);
                // See comments in AcknowledgeCancel re use of
                // Async.
                FireAsync(Completed, this, EventArgs.Empty);
            }
        }

        private void FailOperation(Exception e)
        {
            lock (this)
            {
                failedFlag = true;
                isRunning = false;
                Monitor.Pulse(this);
                FireAsync(Failed, this, new ThreadExceptionEventArgs(e));
            }
        }

        // Utility function for firing an event through the target.
        // It uses C#'s variable length parameter list support
        // to build the parameter list.
        // This functions presumes that the caller holds the object lock.
        // (This is because the event list is typically modified on the UI
        // thread, but events are usually raised on the worker thread.)
        protected void FireAsync(Delegate dlg, params object[] pList)
        {
            if (dlg != null)
            {
                Target.BeginInvoke(dlg, pList);
            }
        }

        protected void OnReturnResult()
        {
            lock (this)
            {
                FireAsync(ReturnResult);
            }
        }

        protected void PrintMessage(string msg)
        {

            lock (this)
            {
                FireAsync(Print, msg);
            }
          
        }


        #endregion


        private string Text;
        public string Options;
        private string File;
        public SpecificationBase Spec;
        private ModuleFacadeBase CurrentModule;
        public bool ShowVerbolMsg ;
        public virtual void Initialize(Form simForm, ModuleFacadeBase module, string text, string options, string file, bool showVerbolMsg)
        {
            //initialize the parameters
            isiTarget = simForm;
            CurrentModule = module;
            Text = text;
            Options = options;
            File = file;
            ShowVerbolMsg = showVerbolMsg;
        }

        // This method is called on a worker thread (via asynchronous
        // delegate invocation).  This is where we call the operation (as
        // defined in the deriving class's DoWork method).
        public void InternalStart()
        {           
            // isRunning is set during Start to avoid a race condition
            try
            {
                Spec = CurrentModule.ParseSpecification(this.Text, Options, File);
                
                OnReturnResult();

            }
            catch (CancelRunningException)
            {
                AcknowledgeCancel();
            }
            catch (Exception e)
            {
                // Raise the Failed event.  We're in a catch handler, so we
                // had better try not to throw another exception.
                try
                {
                    if (e is System.OutOfMemoryException)
                    {
                        e = new PAT.Common.Classes.Expressions.ExpressionClass.OutOfMemoryException("");
                    }

                    FailOperation(e);
                }
                catch
                {
                }

                // The documentation recommends not catching
                // SystemExceptions, so having notified the caller we
                // rethrow if it was one of them.
                if (e is SystemException)
                {
                    throw;
                }
            }


            lock (this)
            {
                // If the operation wasn't cancelled (or if the UI thread
                // tried to cancel it, but the method ran to completion
                // anyway before noticing the cancellation) and it
                // didn't fail with an exception, then we complete the
                // operation - if the UI thread was blocked waiting for
                // cancellation to complete it will be unblocked, and
                // the Completion event will be raised.
                if (!cancelAcknowledgedFlag && !failedFlag)
                {
                    CompleteOperation();
                }
            }
        }

    }
}