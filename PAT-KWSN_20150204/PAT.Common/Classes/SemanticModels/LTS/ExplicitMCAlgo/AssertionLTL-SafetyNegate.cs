using System.Collections.Generic;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.DataStructure;
using System.Linq;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public partial class AssertionLTL
    {
        /// <summary>
        /// This method verifies an LTL whose negation is safety through refinement checking.
        /// </summary>
        /// <returns></returns>
        public virtual void RunVerificationNegate()
        {
            //The following are for identifying a counterexample trace. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying a counterexample trace. 

            Stack<EventBAPairSafety> TaskStackNegate = new Stack<EventBAPairSafety>();

            EventBAPairSafety initialstep = EventBAPairSafety.GetInitialPairs(BA, InitialStep);
            TaskStackNegate.Push(initialstep);

            StringHashTable visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            List<string> Path = new List<string>();
            while (TaskStackNegate.Count != 0)
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = visited.Count;
                    return;
                }

                EventBAPairSafety now = TaskStackNegate.Pop();
                string ID = now.GetCompressedState();

                //The following are for identifying a counterexample trace. 
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        VerificationOutput.CounterExampleTrace.RemoveAt(lastIndex);
                        Path.RemoveAt(lastIndex);
                    }
                }

                VerificationOutput.CounterExampleTrace.Add(now.configuration);
                depthList.Add(depth);
                Path.Add(ID);
                //The above are for identifying a counterexample trace. 

                if (now.States.Count != 0)
                {
                    if (!visited.ContainsKey(ID))
                    {
                        visited.Add(ID);

                        ConfigurationBase[] steps = now.configuration.MakeOneMove().ToArray();

                        //the special case of deadlock
                        //if (steps.Length == 0)
                        if(now.configuration.IsDeadLock)
                        {
                            VerificationOutput.NoOfStates = visited.Count;
                            VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                            return;                            
                        }

                        VerificationOutput.Transitions += steps.Length;

                        EventBAPairSafety[] products = now.Next(BA, steps);
                        foreach (EventBAPairSafety step in products)
                        {
                            string newID = step.GetCompressedState();
                            if (Path.Contains(newID))
                            {
                                VerificationOutput.NoOfStates = visited.Count;
                                VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                                VerificationOutput.LoopIndex = Path.IndexOf(newID);
                                return;
                            }

                            TaskStackNegate.Push(step);
                            depthStack.Push(depth + 1);
                        }
                    }
                }
            }

            VerificationOutput.CounterExampleTrace = null;
            VerificationOutput.NoOfStates = visited.Count;
            VerificationOutput.VerificationResult = VerificationResultType.VALID;
        }
    }
}