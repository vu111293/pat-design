using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Assertion.Parallel;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{    
    public partial class AssertionLTL
    {
        TarjanThread tarjanThread;
       
        public void ModelCheckingLivenessWithFairnessMulticore()
        {
            
            PATThreadPool threadPool = new PATThreadPool();
            tarjanThread = new TarjanThread(InitialStep, BA, threadPool, FairnessType);
            threadPool.StartModelChecking(tarjanThread);

            Thread tarjan = new Thread(new ThreadStart(tarjanThread.TarjanModelChecking));
            tarjan.Start();

            while (!CancelRequested && !tarjanThread.JobFinished)
            {
                System.Threading.Thread.Sleep(100);
            }

            if (!CancelRequested)
            {
                //wait for tarjan to finish.
                tarjan.Join();
                threadPool.WaitForAllDone();

                Dictionary<string, LocalPair> FairSCC;
                Dictionary<string, List<string>> OutgoingTransitionTable;

                // Get the number of transitions and states explored
                VerificationOutput.Transitions = tarjanThread.Transitions;
                VerificationOutput.NoOfStates = tarjanThread.GetNoOfStates();

                if (tarjanThread.VerificationResult == VerificationResultType.VALID)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (tarjanThread.FairSCC != null)
                {
                    //Console.WriteLine("SCC found by Tarjan thread");
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    FairSCC = tarjanThread.FairSCC;
                    LocalTaskStack = tarjanThread.TaskStack;
                    OutgoingTransitionTable = tarjanThread.OutgoingTransitionTable;

                    LocalGetCounterExample(FairSCC, OutgoingTransitionTable);
                }
                else
                {
                    VerificationOutput.VerificationResult = threadPool.JobFinished ? VerificationResultType.INVALID : VerificationResultType.VALID;

                    if (VerificationOutput.VerificationResult == VerificationResultType.INVALID)
                    {
                        //Console.WriteLine("SCC found by Worker thread");
                        FairSCC = threadPool.ResultThread.FairSCC;
                        LocalTaskStack = tarjanThread.TaskStack;
                        OutgoingTransitionTable = tarjanThread.OutgoingTransitionTable;

                        LocalGetCounterExample(FairSCC, OutgoingTransitionTable);
                    }
                }

                //Debug.WriteLine("Number SCCs: " + tarjanThread.SCCCount);
                //Debug.WriteLine("Number non trivial SCCs: " + tarjanThread.BigSCCCount);

                //SCCCount = tarjanThread.SCCCount;
                //SCCTotalSize = tarjanThread.SCCTotalSize;
                //this.Transitions = tarjanThread.Transitions;
                ////this.SearchedDepth = tarjanThread.SearchedDepth;
                //this.NoOfStates = tarjanThread.NoOfState;
            }
            else
            {
                tarjanThread.JobFinished = true;
                tarjan.Join();
                threadPool.WaitForAllDone();
            }
        }
    }
}
