using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using PAT.GUI.UpdateChecking;
using Color = Microsoft.Msagl.Drawing.Color;

namespace PAT.Common.GUI
{
    public class UpdateCheckingWorker
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


        public Boolean HasUpdate;
 

        public virtual void Initialize(Form simForm)
        {
            //initialize the parameters
            isiTarget = simForm;
            HasUpdate = false;
        }

        // This method is called on a worker thread (via asynchronous
        // delegate invocation).  This is where we call the operation (as
        // defined in the deriving class's DoWork method).
        public void InternalStart()
        {           
            // isRunning is set during Start to avoid a race condition
            try
            {
                try
                {
#if ACADEMIC
                    HasUpdate = UpdateManager.IsUpdateAvailable(Common.Ultility.Ultility.PUBLISH_URL);
#else
                    HasUpdate = UpdateManager.IsUpdateAvailable(Common.Ultility.Ultility.PUBLISH_URL_COM);
#endif
                }
                catch (Exception ex)
                {

                }

                // When a cancel occurs, the recursive DoSearch drops back
                // here asap, so we'd better acknowledge cancellation.
                if (CancelRequested)
                {
                
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