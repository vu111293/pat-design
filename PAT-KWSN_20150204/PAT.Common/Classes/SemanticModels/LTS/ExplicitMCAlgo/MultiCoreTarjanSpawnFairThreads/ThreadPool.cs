using System.Collections.Generic;
using System.Threading;

namespace PAT.Common.Classes.Assertion.Parallel
{
    /// <summary>Managed thread pool.</summary>
    public sealed class PATThreadPool
    {
        List<FairThread> WorkThreads = new List<FairThread>(16);
        private TarjanThread Tarjan;
        public FairThread ResultThread;
        public bool JobFinished;
        private int FinishedThread;
        public int ThreadNumber;

        public PATThreadPool()
        {
            JobFinished = false;
            ResultThread = null;
            FinishedThread = 0;
            ThreadNumber = 0;
        }

        public void StartModelChecking(TarjanThread tarjan)
        {
            Tarjan = tarjan;
        }

        public void AddThread(FairThread thread)
        {
            lock (WorkThreads)
            {
                if (!JobFinished)
                {
                    ThreadNumber++;
                    WorkThreads.Add(thread);
                    thread.ReturnAction += new ReturnEvent(thread_ReturnAction);
                    System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(thread.InternalStart), null);
                    
                }
            }
        }

        // Send the signal to all the threads to stop
        public void StopAllThreads()
        {
            lock (WorkThreads)
            {
                foreach (FairThread thread in WorkThreads)
                {
                    thread.CancelRequested = true;
                }
            }
        }

        void thread_ReturnAction(FairThread fairThread)
        {
            lock (WorkThreads)
            {
                //WorkThreads.Remove(fairThread);
                if (fairThread.result)
                {
                    ResultThread = fairThread;
                    JobFinished = true;
                    Tarjan.JobFinished = true;
                    foreach (FairThread thread in WorkThreads)
                    {
                        thread.CancelRequested = true;
                    }
                }

                fairThread.ReturnAction -= new ReturnEvent(thread_ReturnAction);
                FinishedThread++;
                ThreadNumber--;
            }
        }

        public void WaitForAllDone()
        {
            lock (this)
            {
                // Now sit and wait either for the operation to
                // complete or the cancellation to be acknowledged.
                // (Wake up and check every second - shouldn't be
                // necessary, but it guarantees we won't deadlock
                // if for some reason the Pulse gets lost - means
                // we don't have to worry so much about bizarre
                // race conditions.)
                while (!JobFinished && FinishedThread < WorkThreads.Count)
                {
                    Monitor.Wait(this, 100);
                }
            }            
        }
    }
}