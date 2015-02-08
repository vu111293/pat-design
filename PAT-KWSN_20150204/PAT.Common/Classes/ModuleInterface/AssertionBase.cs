using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Antlr.Runtime;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.ModuleInterface
{
    /// <summary>
    /// The base class for all assertions
    /// </summary>
    public abstract class AssertionBase
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


        public delegate void ActionEvent(string action);
        public event ActionEvent Action;

        public delegate void ReturnResultEvent();
        public event ReturnResultEvent ReturnResult;

        public delegate void ProgressEvent(int progress);
        public event ProgressEvent Progress;


        /// <summary>
        /// The ISynchronizeTarget supplied during construction - this can
        /// be used by deriving classes which wish to add their own events.
        /// </summary>
        //public ISynchronizeInvoke Target
        //{
        //    get { return isiTarget; }
        //}
        protected ISynchronizeInvoke isiTarget;


        /// <summary>
        /// Flag indicating whether the request has been cancelled.  Long-
        /// running operations should check this flag regularly if they can
        /// and cancel their operations as soon as they notice that it has
        /// been set.
        /// </summary>
        public bool CancelRequested
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
                isiTarget.BeginInvoke(dlg, pList);
            }
        }

        /// <summary>
        /// Signal to GUI that verification is finished.
        /// </summary>
        protected void OnReturnResult()
        {
            lock (this)
            {
                FireAsync(ReturnResult);
                //if (ReturnResult != null)
                //{
                //    ReturnResult();
                //}
            }
        }

        /// <summary>
        /// In the verbose mode, this method returns the message to the GUI to display.
        /// </summary>
        /// <param name="msg"></param>
        protected void PrintMessage(string msg)
        {
            //if (VerboseMode)
            //{
            //    //OnAction(msg + "\r\n");
            //    lock (this)
            //    {
            //        FireAsync(Action, msg + "\r\n");
            //    }
            //}
        }

        #endregion

        public ModelCheckingOptions ModelCheckingOptions;

        private int SelectedBahavior;
        private int SelectedEngine;
        public string SelectedBahaviorName;
        public string SelectedEngineName;

        /// <summary>
        /// The assertion token generated during the parsing, which is used for throw exception error msg.
        /// </summary>
        public IToken AssertToken;
        
        /// <summary>
        /// The initial configuration of the system
        /// </summary>
        public ConfigurationBase InitialStep;

        //static variables
        public static bool CalculateParticipatingProcess;
                           
        public VerificationOutput VerificationOutput;

        public bool MustAbstract;
       
        public bool VerificationMode = true;
     
        /// <summary>
        /// Assertion Initialization to create the initial step based on the concrete types.
        /// This method shall be invoked after the parsing immediately to instanciate the initial step
        /// </summary>
        /// <param name="spec">The concrete specification of the module</param>
        public virtual void Initialize(SpecificationBase spec)
        {
            //initialize model checking options, the default option is for deadlock/reachablity algorithms
            ModelCheckingOptions = new ModelCheckingOptions();
            List<string> DeadlockEngine = new List<string>();
            DeadlockEngine.Add(Constants.ENGINE_DEPTH_FIRST_SEARCH);
            DeadlockEngine.Add(Constants.ENGINE_BREADTH_FIRST_SEARCH);
            ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, DeadlockEngine);
        }

        /// <summary>
        /// The GUI initialization method is invoked before the verification so that the verification options are passed
        /// into the assertion.
        /// </summary>
        /// <param name="target">The model checking form or null in the console mode</param>
        /// <param name="behavior"></param>
        /// <param name="engine"></param>
        public virtual void UIInitialize(ISynchronizeInvoke target, int behavior, int engine)
        {
            //initialize the parameters
            isiTarget = target;
            isRunning = false;

            SelectedBahavior = behavior;
            SelectedEngine = engine;

            if (ModelCheckingOptions.AddimissibleBehaviorsNames.Count <= SelectedBahavior)
            {
                throw new RuntimeException("Invalid Behavior Selection");
            }
            SelectedBahaviorName = ModelCheckingOptions.AddimissibleBehaviorsNames[SelectedBahavior];

            if (ModelCheckingOptions.AddimissibleBehaviors[SelectedBahavior].VerificationEngines.Count <= SelectedEngine)
            {
                throw new RuntimeException("Invalid Engine Selection");
            }
            SelectedEngineName = ModelCheckingOptions.AddimissibleBehaviors[SelectedBahavior].VerificationEngines[SelectedEngine];

            VerificationOutput = new VerificationOutput(SelectedEngineName);
            VerificationMode = true;
        }

        /// <summary>
        /// The GUI initialization method is invoked before the verification so that the verification options are passed
        /// into the assertion.
        /// </summary>
        /// <param name="target">The model checking form or null in the console mode</param>
        /// <param name="behavior"></param>
        /// <param name="engine"></param>
        public virtual void UIInitialize(ISynchronizeInvoke target, string behavior, string engine)
        {
            int BehaviorIndex = ModelCheckingOptions.AddimissibleBehaviorsNames.IndexOf(behavior);
            int EngineIndex = -1;

            if (ModelCheckingOptions.AddimissibleBehaviorsNames.Contains(behavior))
            {
                EngineIndex = ModelCheckingOptions.AddimissibleBehaviors[BehaviorIndex].VerificationEngines.IndexOf(engine);
            }
                        
            UIInitialize(target, BehaviorIndex, EngineIndex);
        }

        /// <summary>
        /// This method is called on a worker thread (via asynchronous
        /// delegate invocation).  This is where we call the operation (as
        /// defined in the deriving class's DoWork method).
        /// </summary>
        public void InternalStart()
        {
            // Reset our state - we might be run more than once.
            cancelledFlag = false;
            completeFlag = false;
            cancelAcknowledgedFlag = false;
            failedFlag = false;

            // isRunning is set during Start to avoid a race condition
            try
            {
                if (VerificationMode)
                {
                    //start the timer and memory checking
                    VerificationOutput.StartVerification();

                    PrintMessage("Verification starts....");

                    //this checking to skip the verification for the pre-determined result: as in the LTL assertions.
                    if (VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
                    {
                        ModelChecking();
                    }
                    PrintMessage("Verification ends....");
                }
                else
                {
                    VerificationOutput.ResultString = GetResultString();
                }

                // When a cancel occurs, the recursive DoSearch drops back
                // here asap, so we'd better acknowledge cancellation.
                if (CancelRequested)
                {
                    VerificationOutput.ResultString = GetResultStringForUnfinishedSearching(null);

                    AcknowledgeCancel();
                }
                else
                {
                    OnReturnResult();
                }

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
                        e = new Expressions.ExpressionClass.OutOfMemoryException("");
                    }

                    if (VerificationOutput.CounterExampleTrace != null && VerificationOutput.CounterExampleTrace.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        VerificationOutput.GetCounterxampleString(sb);
                        //this.GetCounterxampleString(sb);
                        e.Data.Add("trace", sb.ToString());                        
                    }

                    VerificationOutput.ResultString = GetResultStringForUnfinishedSearching(e);

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
            finally
            {

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

        /// <summary>
        /// Clear the data after verification
        /// </summary>
        public virtual void Clear()
        {
            isiTarget = null;
        }

        protected virtual void ModelChecking()
        {
            RunVerification();

            if (MustAbstract && VerificationOutput.CounterExampleTrace != null && VerificationOutput.CounterExampleTrace.Count > 0)
            {
                Ultility.Ultility.CutNumber = -1;
                if (IsCounterExampleSpurious())
                {
                    VerificationOutput.VerificationResult = VerificationResultType.UNKNOWN;
                }
                Ultility.Ultility.CutNumber = 2;
            }
        }

        /// <summary>
        /// Run the verification and get the result.
        /// To be overridden by the deriving class - this is where the work
        /// will be done.  The base class calls this method on a worker
        /// thread when the Start method is called.
        /// </summary>
        /// <returns></returns>
        public abstract void RunVerification();

        /// <summary>
        /// Get a counterexample. To be overridden by the deriving class. 
        /// </summary>
        /// <returns></returns>
        public abstract string GetResultString();

        /// <summary>
        /// Get a counterexample. To be overridden by the deriving class. 
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAddtionalStatsString()
        {
            return "";
        }


        public virtual string GetVerificationStatistics()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("********Verification Statistics********");
            sb.Append(GetAddtionalStatsString());
            sb.Append(VerificationOutput.GetVerificationStatistics());
            return sb.ToString();
        }

        /// <summary>
        /// GetResultStringForUnfinishedSearching
        /// </summary>
        /// <returns></returns>
        public virtual string GetResultStringForUnfinishedSearching(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            
            if (ex != null)
            {
                sb.AppendLine(Resources.Exception_happened_during_the_verification);
                string trace = "";
                if (ex.Data.Contains("trace"))
                {
                    trace = Environment.NewLine + "Trace leads to exception:" + Environment.NewLine + ex.Data["trace"].ToString();
                }
                if(ex is RuntimeException)
                {
                    RuntimeException rex = ex as RuntimeException;
                    sb.AppendLine(rex.Message + trace + (rex.InnerStackTrace != null ? Environment.NewLine + "Exception stack trace:" + Environment.NewLine + rex.InnerStackTrace : ""));
                }
                else
                {
                    sb.AppendLine(ex.Message + trace);    
                }
            }
            else
            {
                sb.AppendLine("Verification cancelled");
            }
            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine();

            return sb.ToString();
        }


        /// <summary>
        /// Get the starting process string for display purpose. To be overridden by the deriving class. 
        /// </summary>
        public abstract string StartingProcess
        {
            get;
        }

        /// <summary>
        /// This method checks whether the found counterexample is spurious or not.
        /// This checking only works for abstraction for parameterized systems. 
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsCounterExampleSpurious()
        {
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);
            List<ConfigurationBase> ConcreteCounterExampleTrace = new List<ConfigurationBase>(64);
            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            StringHashTable visited = new StringHashTable(1024);
            visited.Add("0-" + InitialStep.GetID());

            do
            {
                ConfigurationBase current = working.Pop();
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        ConcreteCounterExampleTrace.RemoveAt(lastIndex);
                    }
                }

                ConcreteCounterExampleTrace.Add(current);
                depthList.Add(depth);

                if (ConcreteCounterExampleTrace.Count == VerificationOutput.CounterExampleTrace.Count)
                {
                    VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                    Ultility.Ultility.CutNumber = 2;
                    return false;
                }
                else
                {
                    ConfigurationBase abstractStep = VerificationOutput.CounterExampleTrace[depth + 1];

                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(abstractStep.Event);
                    //for (int j = 0; j < steps.Length; j++)
                    foreach (ConfigurationBase step in steps)
                    {
                        if (abstractStep.EqualsV(step))
                        {
                            string tmp = (depth + 1) + "-" + step.GetID();
                            if (!visited.ContainsKey(tmp))
                            {
                                working.Push(step);
                                depthStack.Push(depth + 1);
                                visited.Add(tmp);
                            }
                        }
                    }
                }

            } while (working.Count > 0);

            return true;
        }

        public bool IsBDDSelected()
        {
            return (SelectedEngineName == Constants.ENGINE_FORWARD_SEARCH_BDD || SelectedEngineName == Constants.ENGINE_BACKWARD_SEARCH_BDD ||
                SelectedEngineName == Constants.ENGINE_FORWARD_BACKWARD_SEARCH_BDD);
        }
    }
}