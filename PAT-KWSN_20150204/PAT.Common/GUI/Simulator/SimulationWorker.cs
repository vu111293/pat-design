using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using Color = Microsoft.Msagl.Drawing.Color;

namespace PAT.Common.GUI
{
    public class SimulationWorker
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

        protected void OnAction(ListViewItem item)
        {
            lock (this)
            {
                FireAsync(Action, item, graph);
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

        

        private EventStepSim initialStep;
        private string initialString;
        private ListView ListView_Trace;
        private Hashtable visited;
        public Graph graph;
        public Dictionary<string, ProcessData> Mapping;
        public bool ForceSimuationStop = false;
        public GViewer SimulatorViewer;
        public Graph intGraph;
        private bool HideTauTransition = false;

        public virtual void Initialize(SimulationForm simForm, EventStepSim currentStep, string currentNode, ListView trace, Graph igraph)
        {
            //initialize the parameters
            isiTarget = simForm;
           
            initialStep = currentStep;
            initialString = currentNode;
            ListView_Trace = new ListView();
            if(trace.Items.Count > 0)
            {
                ListView_Trace.Items.Add(trace.Items[0].Clone() as ListViewItem);    
            }

            HideTauTransition = simForm.HideTauTransition;
            visited = new Hashtable(simForm.visited);
            graph = CloneGraph(igraph);
            intGraph = CloneGraph(igraph);
            Mapping = new Dictionary<string, ProcessData>(simForm.Mapping);


            SimulatorViewer = new GViewer();
            this.SimulatorViewer.AsyncLayout = false;
            this.SimulatorViewer.BackwardEnabled = true;
            this.SimulatorViewer.BuildHitTree = true;
            this.SimulatorViewer.ForwardEnabled = true;
            this.SimulatorViewer.Graph = null;
            this.SimulatorViewer.LayoutAlgorithmSettingsButtonVisible = true;
            this.SimulatorViewer.MouseHitDistance = 0.05;
            this.SimulatorViewer.Name = "SimulatorViewer";
            this.SimulatorViewer.NavigationVisible = true;
            this.SimulatorViewer.NeedToCalculateLayout = true;
            this.SimulatorViewer.PanButtonPressed = false;
            this.SimulatorViewer.SaveAsImageEnabled = true;
            this.SimulatorViewer.SaveAsMsaglEnabled = true;
            this.SimulatorViewer.SaveButtonVisible = true;
            this.SimulatorViewer.SaveGraphButtonVisible = true;
            this.SimulatorViewer.SaveInVectorFormatEnabled = true;
            this.SimulatorViewer.ToolBarIsVisible = true;
            this.SimulatorViewer.ZoomF = 1;
            this.SimulatorViewer.ZoomFraction = 0.5;
            this.SimulatorViewer.ZoomWindowThreshold = 0.05;
            this.SimulatorViewer.Dock = DockStyle.Fill;

        }

        // This method is called on a worker thread (via asynchronous
        // delegate invocation).  This is where we call the operation (as
        // defined in the deriving class's DoWork method).
        public void InternalStart()
        {           
            // isRunning is set during Start to avoid a race condition
            try
            {

                PrintMessage("Generating Graph....");
                BuildCompleteGraph();
                
                // When a cancel occurs, the recursive DoSearch drops back
                // here asap, so we'd better acknowledge cancellation.
                if (CancelRequested)
                {
                    PrintMessage("Cancelling...");
                    SimulatorViewer.Graph = intGraph;
                    AcknowledgeCancel();
                }
                else
                {
                    PrintMessage("Rendering Graph...");
                    SimulatorViewer.Graph = graph;
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
                    SimulatorViewer.Graph = intGraph;
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


        private void BuildCompleteGraph()
        {
            Stack<EventStepSim> searchStack = new Stack<EventStepSim>();
            searchStack.Push(initialStep);

            Stack<string> stringStack = new Stack<string>();
            stringStack.Push(initialString);

            while (searchStack.Count > 0)
            {
                if (visited.Count > Classes.Ultility.Ultility.SIMULATION_BOUND)
                {
                    ForceSimuationStop = true;
                    return;
                }

                if (CancelRequested)
                {
                    return;
                }

                EventStepSim currentStep = searchStack.Pop();
                string currentNode = stringStack.Pop();

                List<EventStepSim> list = currentStep.MakeOneMove(HideTauTransition);

                foreach (EventStepSim step in list)
                {
                    //change all nodes to white node!
                    foreach (Node mapN in graph.NodeMap.Values)
                    {
                        mapN.Attr.FillColor = Color.White;
                    }

                    string stepString;
                    //string nextState = step.ToFullString();
                    step.SourceProcess = currentStep.StepToString;//.Process.ToString();


                    if (step.StepVisited(visited, out stepString))
                    {
                        Node srcNode = graph.FindNode(currentNode);

                        Edge edge = null;
                        foreach (Edge outEdge in srcNode.OutEdges)
                        {
                            if(outEdge.LabelText == step.Event && outEdge.Target == stepString)
                            {
                                //duplicate edge is found
                                edge = outEdge;
                                break;
                            }
                        }
                        
                        if(edge == null)
                        {
                            edge = graph.AddEdge(currentNode, step.Event, stepString);

                            edge.TargetNode.Attr.FillColor = Color.Red;
                            //(graph.NodeMap[stepString] as Node).Attr.FillColor = Color.Red;

                            //update the ListView_Trace and clone graph
                            AddToTrace(edge.SourceNode.LabelText, step, edge.TargetNode.LabelText, stepString);

                            string key = GetTraceEvent(ListView_Trace.Items.Count) + stepString;
                            if (!Mapping.ContainsKey(key))
                            {
                                Mapping.Add(key, new ProcessData(step, CloneGraph(graph), new List<EventStepSim>(0)));
                            }
                        }
                    }
                    else
                    {
                        visited.Add(stepString, null);

                        Edge e = graph.AddEdge(currentNode, step.Event, stepString);
                        
                        e.TargetNode.LabelText = (graph.NodeCount - 1).ToString();
                        e.TargetNode.UserData = step; // nextState;
                        e.TargetNode.Attr.FillColor = Color.Red;

                        //update the ListView_Trace and clone graph
                        AddToTrace(e.SourceNode.LabelText, step, e.TargetNode.LabelText, stepString);
                        string key = GetTraceEvent(ListView_Trace.Items.Count) + stepString;
                        if (!Mapping.ContainsKey(key))
                        {
                            Mapping.Add(key, new ProcessData(step, CloneGraph(graph), new List<EventStepSim>(0)));
                        }

                        searchStack.Push(step);
                        stringStack.Push(stepString);
                        //BuildCompleteGraph(step, stepString);
                    }
                }
            }

            foreach (Node node in graph.NodeMap.Values)
            {
                foreach (Edge edge1 in node.OutEdges)
                {
                    foreach (Edge edge2 in node.OutEdges)
                    {
                        if (edge1 != edge2)
                        {
                            if (edge1.TargetNode == edge2.TargetNode)
                            {
                                if (edge1.LabelText == edge2.LabelText)
                                {
                                    System.Diagnostics.Debug.Assert(false, "Duplicated Transition in the Simulator");
                                }
                            }
                        }
                    }
                }
            }
        }

        public string GetTraceEvent(int size)
        {
            string evts = "";
            for (int i = 0; i < size; i++)
            {
                evts += "->" + ListView_Trace.Items[i].SubItems[0].Text;
            }
            return evts;
        }

        public void AddToTrace(string srcID, EventStepSim step, string id, string key)
        {
            ListViewItem item = new ListViewItem(new string[] { srcID, step.Event, id, step.SourceProcess });
            ListView_Trace.Items.Add(item);

            item = new ListViewItem(new string[] { srcID, step.Event, id, step.SourceProcess });
            item.Tag = key;

            OnAction(item);            
        }

        public static Graph CloneGraph(Graph graph)
        {
            Graph Graph = new Graph(graph.Label.Text);
            //Graph.GraphAttr.Orientation = graph.GraphAttr.Orientation;
            Graph.Attr.LayerDirection = graph.Attr.LayerDirection;

            Debug.Assert(graph.Edges.Count > 0);

            foreach (Edge edge in graph.Edges)
            {
                Edge newEdge = Graph.AddEdge(edge.Source, edge.LabelText, edge.Target);
                if (edge.Source == SimulationForm.INITIAL_STATE)
                {
                    newEdge.SourceNode.Attr.LineWidth = 0;
                    newEdge.SourceNode.Attr.Color = Color.White;
                }
                newEdge.SourceNode.LabelText = edge.SourceNode.LabelText;
                newEdge.SourceNode.UserData = edge.SourceNode.UserData;
                newEdge.TargetNode.LabelText = edge.TargetNode.LabelText;
                newEdge.TargetNode.Attr.FillColor = edge.TargetNode.Attr.FillColor;
                newEdge.TargetNode.UserData = edge.TargetNode.UserData;
                newEdge.Attr.Color = edge.Attr.Color;
            }

            return Graph;
        }
    }
}