using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using System.Text;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.LTS.Assertion
{
    public abstract class AssertionDeterminism : AssertionBase
    {
        private string Event = "";

        protected AssertionDeterminism()
        {
        }

        public override string ToString()
        {
            return StartingProcess  + " deterministic";
        }


        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (SelectedEngineName == Constants.ENGINE_DEPTH_FIRST_SEARCH)
            {
                DFSVerification();
            }
            else
            {
                BFSVerification();
            }
        }

        public void DFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(1024);

            Visited.Add(InitialStep.GetID());

            working.Push(InitialStep);
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);

            List<int> depthList = new List<int>(1024);

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Pop();
                
                int depth = depthStack.Pop();
                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        this.VerificationOutput.CounterExampleTrace.RemoveAt(lastIndex);
                    }
                }

                this.VerificationOutput.CounterExampleTrace.Add(current);
                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                int size = list.Count();
                this.VerificationOutput.Transitions += size;

                //check if it is nondeterministic, it is nondeterministic if and only if there exists two different configuration with the same event.
                bool deterministic = true;
                Dictionary<string, string> mapping = new Dictionary<string, string>(size);
                foreach (ConfigurationBase configuration in list)
                {
                    string ID = configuration.GetID();
                    string mappedID;

                    if (mapping.TryGetValue(configuration.Event, out mappedID))
                    {
                        if (mappedID != ID)
                        {
                            deterministic = false;
                            Event = configuration.Event;
                            break;
                        }                        
                    }
                    else
                    {
                        mapping.Add(configuration.Event, ID);
                    }
                }

                if (!deterministic)
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                depthList.Add(depth);

                //for (int i = list.Length - 1; i >= 0; i--)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];
                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Push(step);
                        depthStack.Push(depth + 1);
                    }
                }
            } while (working.Count > 0);

            this.VerificationOutput.CounterExampleTrace = null;

            if (MustAbstract)
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.UNKNOWN;
            }
            else
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            this.VerificationOutput.NoOfStates = Visited.Count;
            return;
        }

        public void BFSVerification()
        {
            StringHashTable Visited = new StringHashTable(Ultility.Ultility.MC_INITIAL_SIZE);

            Queue<ConfigurationBase> working = new Queue<ConfigurationBase>(1024);
            Queue<List<ConfigurationBase>> paths = new Queue<List<ConfigurationBase>>(1024);
            
            Visited.Add(InitialStep.GetID());
            
            working.Enqueue(InitialStep);
            List<ConfigurationBase> path = new List<ConfigurationBase>();
            path.Add(InitialStep);
            paths.Enqueue(path);

            do
            {
                if (CancelRequested)
                {
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    return;
                }

                ConfigurationBase current = working.Dequeue();
                List<ConfigurationBase> currentPath = paths.Dequeue();
                //ConfigurationBase[] list = current.MakeOneMove().ToArray();
                //this.VerificationOutput.Transitions += list.Length;

                IEnumerable<ConfigurationBase> list = current.MakeOneMove();
                int size = list.Count();
                this.VerificationOutput.Transitions += size;

                bool deterministic = true;
                Dictionary<string, string> mapping = new Dictionary<string, string>(size);
                foreach (ConfigurationBase configuration in list)
                {
                    string ID = configuration.GetID();

                    //if (mapping.ContainsKey(configuration.Event))
                    //{
                    //    if (mapping[configuration.Event] != ID)
                    //    {
                    string mappedID;
                    if (mapping.TryGetValue(configuration.Event, out mappedID))
                    {
                        if (mappedID != ID)
                        {
                            deterministic = false;
                            Event = configuration.Event;
                            break;
                        }
                    }
                    else
                    {
                        mapping.Add(configuration.Event, ID);
                    }
                }

                if (!deterministic)
                {
                    this.VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                    this.VerificationOutput.NoOfStates = Visited.Count;
                    this.VerificationOutput.CounterExampleTrace = currentPath;
                    return;
                }
                
                //for (int i = list.Length - 1; i >= 0; i--)
                foreach (ConfigurationBase step in list)
                {
                    //ConfigurationBase step = list[i];
                    string stepID = step.GetID();
                    if (!Visited.ContainsKey(stepID))
                    {
                        Visited.Add(stepID);
                        working.Enqueue(step);

                        List<ConfigurationBase> newPath = new List<ConfigurationBase>(currentPath);
                        newPath.Add(step);
                        paths.Enqueue(newPath);
                    }
                }
            } while (working.Count > 0);

            if (MustAbstract)
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.UNKNOWN;
            }
            else
            {
                this.VerificationOutput.VerificationResult = VerificationResultType.VALID;
            }

            this.VerificationOutput.NoOfStates = Visited.Count;
            return;
        }

        protected override bool IsCounterExampleSpurious()
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


                if (ConcreteCounterExampleTrace.Count == this.VerificationOutput.CounterExampleTrace.Count)
                {
                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(Event);
                    if (steps.Count() > 1)
                    //if (steps.Length > 1)
                    {
                        this.VerificationOutput.CounterExampleTrace = ConcreteCounterExampleTrace;
                        return false;
                    }
                }
                else
                {
                    ConfigurationBase abstractStep = this.VerificationOutput.CounterExampleTrace[depth + 1];
                    IEnumerable<ConfigurationBase> steps = current.MakeOneMove(abstractStep.Event);
                    //for (int j = 0; j < steps.Length; j++)
                    foreach (ConfigurationBase step in steps)
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
            } while (working.Count > 0);

            return true;
        }

        public override string GetResultString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");    
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");                
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine("The following trace leads to a situation where engaging in event " + Event + " leads to different system configurations.");
                //GetCounterxampleString(sb);
                VerificationOutput.GetCounterxampleString(sb);
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);

            sb.AppendLine();

            return sb.ToString();
        }

        
    }
}