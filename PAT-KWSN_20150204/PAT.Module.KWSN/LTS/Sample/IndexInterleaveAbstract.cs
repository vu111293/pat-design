using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class IndexInterleaveAbstract : Process
    {
        public List<Process> Processes;
        public Expression RangeExpression;

        public Dictionary<string, int> ProcessesCounter;
        public Dictionary<string, int> ProcessesActualSize;

        public IndexInterleaveAbstract(Process processBase, Expression range)
        {
            Processes = new List<Process>();
            Processes.Add(processBase);

            RangeExpression = range;
        }
       
        public IndexInterleaveAbstract(Process processBase, int size)
        {
            Processes = new List<Process>();
            Processes.Add(processBase);

            ProcessesCounter = new Dictionary<string, int>();
            ProcessesCounter.Add(processBase.ProcessID, size);

            ProcessesActualSize = new Dictionary<string, int>();
            ProcessesActualSize.Add(processBase.ProcessID, size);            
        }

        public IndexInterleaveAbstract(List<Process> processes, Dictionary<string, int> counters)
        {
            Processes = new List<Process>();
            ProcessesCounter = new Dictionary<string, int>();

            //create process ID
            List<string> tmp = new List<string>(processes.Count);
            for (int i = 0; i < processes.Count; i++)
            {
                string id = processes[i].ProcessID;
                if (counters[id] != 0)
                {
                    tmp.Add(id);
                    Processes.Add(processes[i]);
                    ProcessesCounter.Add(id, counters[id]);
                }
            }

            tmp.Sort();

            ProcessID = Constants.INTERLEAVE + tmp[0] + "*" + ProcessesCounter[tmp[0]];
            for (int i = 1; i < Processes.Count; i++)
            {
                ProcessID += Constants.SEPARATOR + tmp[i] + "*" + ProcessesCounter[tmp[i]];
            }

            ProcessID = DataStore.DataManager.InitializeProcessID(ProcessID);
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            int TerminationCount = 0;
            bool hasAtomicTermination = false;

            List<Dictionary<string, int>> nextProcessCounters = null;

            for (int i = 0; i < Processes.Count; i++)
            {
                Process process = Processes[i];
                List<Configuration> list1 = new List<Configuration>();
                process.MoveOneStep(GlobalEnv, list1);

                bool hasTermination = false;

                if (list1.Count> 0)
                {
                    nextProcessCounters = Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ProcessesCounter, process.ProcessID, 1);
                }

                for (int j = 0; j < list1.Count; j++)
                {
                    Configuration step = list1[j];

                    if (step.Event == Constants.TERMINATION)
                    {
                        hasTermination = true;

                        if (step.IsAtomic)
                        {
                            hasAtomicTermination = true;
                        }
                    }
                    else
                    {
                        foreach (Dictionary<string, int> ints in nextProcessCounters)
                        {
                            Dictionary<string, int> listInstance = new Dictionary<string, int>(ints);

                            List<Process> newProcess = new List<Process>(Processes);

                            AddOneProcess(newProcess, step.Process, listInstance);

                            IndexInterleaveAbstract interleave = new IndexInterleaveAbstract(newProcess, listInstance);
                            Configuration newStep = new Configuration(interleave, step.Event, step.DisplayName, step.GlobalEnv, step.IsDataOperation);
                            newStep.IsAtomic = step.IsAtomic;

                            if (AssertionBase.CalculateParticipatingProcess)
                            {
                                newStep.ParticipatingProcesses = new string[] { process.ProcessID };
                            }

                            list.Add(newStep);                            
                        }
                    }
                }

                if (hasTermination)
                {
                    TerminationCount++;
                }

                //to check whether there are synchoronous channel input/output
                if (Specification.HasSyncrhonousChannel)
                {
                    SynchronousChannelInputOutput(list, i, GlobalEnv, null);
                }
            }

            if (TerminationCount == Processes.Count)
            {
                Configuration temp = new Configuration(new Stop(), Constants.TERMINATION, null, GlobalEnv, false);

                if(hasAtomicTermination)
                {
                    temp.IsAtomic = true;
                }

                if (AssertionBase.CalculateParticipatingProcess)
                {
                    temp.ParticipatingProcesses = new string[Processes.Count];
                    for (int i = 0; i < Processes.Count; i++)
                    {
                        temp.ParticipatingProcesses[i] = i.ToString();
                    }
                }
                list.Add(temp);
            }

            //return returnList;
        }

        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
            //List<ConfigurationWithChannelData> returnList = new List<ConfigurationWithChannelData>();

            for (int i = 0; i < Processes.Count; i++)
            {
                Process process = Processes[i];
                List<ConfigurationWithChannelData> list1 = new List<ConfigurationWithChannelData>();
                process.SyncOutput(GlobalEnv, list1);

                if (list1.Count > 0)
                {
                    List<Dictionary<string, int>> nextProcessCounters =
                        Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ProcessesCounter, process.ProcessID, 1);

                    for (int j = 0; j < list1.Count; j++)
                    {
                        Configuration step = list1[j];

                        foreach (Dictionary<string, int> ints in nextProcessCounters)
                        {
                            List<Process> newProcess = new List<Process>(Processes);

                            Dictionary<string, int> listInstance = new Dictionary<string, int>(ints);
                            AddOneProcess(newProcess, step.Process, listInstance);
                            IndexInterleaveAbstract interleave = new IndexInterleaveAbstract(newProcess, listInstance);
                            ConfigurationWithChannelData newStep = new ConfigurationWithChannelData(interleave, step.Event, step.DisplayName, step.GlobalEnv, step.IsDataOperation, list1[j].ChannelName, list1[j].Expressions);
                            newStep.IsAtomic = step.IsAtomic;
                            list.Add(newStep);                            
                        }
                    }
                }
            }

            //return returnList;
        }

        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
            //List<Configuration> returnList = new List<Configuration>();

            for (int i = 0; i < Processes.Count; i++)
            {
                Process process = Processes[i];
                List<Configuration> list1 = new List<Configuration>();
                process.SyncInput(eStep, list1);

                if (list1.Count > 0)
                {
                    List<Dictionary<string, int>> nextProcessCounters =
                        Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ProcessesCounter, process.ProcessID, 1);

                    for (int j = 0; j < list1.Count; j++)
                    {
                        Configuration step = list1[j];

                        foreach (Dictionary<string, int> ints in nextProcessCounters)
                        {
                            List<Process> newProcess = new List<Process>(Processes);
                            Dictionary<string, int> listInstance = new Dictionary<string, int>(ints);
                            AddOneProcess(newProcess, step.Process, listInstance);
                            IndexInterleaveAbstract interleave = new IndexInterleaveAbstract(newProcess, listInstance);
                            step.Process = interleave;
                            list.Add(step);                            
                        }
                    }
                }
            }

            //return returnList;
        }

        private void SynchronousChannelInputOutput(List<Configuration> returnList, int i, Valuation GlobalEnv, string evt)
        {
            List<ConfigurationWithChannelData> outputs = new List<ConfigurationWithChannelData>();
            Processes[i].SyncOutput(GlobalEnv, outputs);

            List<Dictionary<string, int>> nextProcessCounters1 =
                Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ProcessesCounter, Processes[i].ProcessID, 1);

            foreach (ConfigurationWithChannelData vm in outputs)
            {
                if (evt != null & vm.Event != evt)
                {
                    continue;
                }

                for (int k = 0; k < Processes.Count; k++)
                {
                    string id = Processes[k].ProcessID;

                    if (k != i || (k == i && (ProcessesCounter[id] > 1 || ProcessesCounter[id] == -1)))
                    {
                        List<Configuration> syncedProcess = new List<Configuration>();
                        Processes[k].SyncInput(vm, syncedProcess);
                        if (syncedProcess.Count > 0)
                        {
                            if (k == i)
                            {
                                List<Dictionary<string, int>> nextProcessCountersInner =
                                    Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ProcessesCounter,
                                                                                     Processes[i].ProcessID, 2);
                                foreach (Configuration p in syncedProcess)
                                {
                                    foreach (Dictionary<string, int> ints in nextProcessCountersInner)
                                    {
                                        Dictionary<string, int> dictionaryNew =
                                            new Dictionary<string, int>(ints);

                                        List<Process> newProcess = new List<Process>(Processes);

                                        AddOneProcess(newProcess, p.Process, dictionaryNew);
                                        AddOneProcess(newProcess, vm.Process, dictionaryNew);
                                        Configuration newStep = new Configuration(new IndexInterleaveAbstract(newProcess, dictionaryNew), vm.Event,vm.DisplayName, GlobalEnv, false);
                                        newStep.IsAtomic = vm.IsAtomic || p.IsAtomic;

                                        if (AssertionBase.CalculateParticipatingProcess)
                                        {
                                            newStep.ParticipatingProcesses = new string[]
                                                                             {
                                                                                 Processes[i].ProcessID,
                                                                                 Processes[k].ProcessID
                                                                             };
                                        }

                                        returnList.Add(newStep);                                        
                                    }
                                }
                            }
                            else
                            {
                                foreach (Dictionary<string, int> ints in nextProcessCounters1)
                                {
                                    List<Dictionary<string, int>> nextProcessCountersInner =
                                        Common.Classes.Ultility.Ultility.ProcessCounterDecrement(Common.Classes.Ultility.Ultility.CutNumber, ints, id, 1);

                                    foreach (Configuration p in syncedProcess)
                                    {
                                        foreach (Dictionary<string, int> dictionary in nextProcessCountersInner)
                                        {
                                            List<Process> newProcess = new List<Process>(Processes);

                                            AddOneProcess(newProcess, p.Process, dictionary);
                                            AddOneProcess(newProcess, vm.Process, dictionary);

                                            Configuration newStep =
                                                new Configuration(new IndexInterleaveAbstract(newProcess, dictionary),vm.Event, vm.DisplayName, GlobalEnv, false);
                                            newStep.IsAtomic = vm.IsAtomic || p.IsAtomic;

                                            if (AssertionBase.CalculateParticipatingProcess)
                                            {
                                                newStep.ParticipatingProcesses = new string[]
                                                                             {
                                                                                 Processes[i].ProcessID,
                                                                                 Processes[k].ProcessID
                                                                             };
                                            }

                                            returnList.Add(newStep);                                            
                                        }
                                    }                                    
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void AddOneProcess(List<Process> processes, Process newProcess, Dictionary<string, int> counter)
        {
            if (newProcess is IndexInterleave)
            {
                IndexInterleave index = newProcess as IndexInterleave;

                foreach (Process processe in index.Processes)
                {
                    string tmp = processe.ProcessID;

                    if (!counter.ContainsKey(tmp))
                    {
                        counter.Add(tmp, 1);
                        processes.Add(processe);
                    }
                    else
                    {
                        counter[tmp] = Common.Classes.Ultility.Ultility.ProcessCounterIncrement(Common.Classes.Ultility.Ultility.CutNumber, counter[tmp],1);
                    }
                }
            }
            else if (newProcess is IndexInterleaveAbstract)
            {
                IndexInterleaveAbstract index = newProcess as IndexInterleaveAbstract;

                foreach (Process processe in index.Processes)
                {
                    string tmp = processe.ProcessID;

                    if (!counter.ContainsKey(tmp))
                    {
                        counter.Add(tmp, index.ProcessesCounter[tmp]);
                        processes.Add(processe);
                    }
                    else
                    {
                        counter[tmp] = Common.Classes.Ultility.Ultility.ProcessCounterIncrement(Common.Classes.Ultility.Ultility.CutNumber, counter[tmp], index.ProcessesCounter[tmp]);
                    }
                }
            }
            else
            {
                string tmp = newProcess.ProcessID;
                if (!counter.ContainsKey(tmp))
                {
                    counter.Add(tmp, 1);
                    processes.Add(newProcess);
                }
                else
                {
                    counter[tmp] = Common.Classes.Ultility.Ultility.ProcessCounterIncrement(Common.Classes.Ultility.Ultility.CutNumber, counter[tmp], 1);
                }
            }
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            HashSet<string> toReturn = new HashSet<string>();

            for (int i = 0; i < Processes.Count; i++)
            {
                Process process = Processes[i];
                toReturn.UnionWith(process.GetAlphabets(visitedDefinitionRefs));
            }

            return toReturn;
        }

        public override List<string> GetGlobalVariables()
        {
            if (this.ProcessesCounter == null)
            {
                return Processes[0].GetGlobalVariables();
            }

            List<string> Variables = new List<string>();

            foreach (Process var in Processes)
            {
                Common.Classes.Ultility.Ultility.AddList(Variables, var.GetGlobalVariables());
            }

            return Variables;
        }

        public override List<string> GetChannels()
        {
            if (this.ProcessesCounter == null)
            {
                return Processes[0].GetChannels();
            }

            List<string> channels = new List<string>();

            foreach (Process var in Processes)
            {
                Common.Classes.Ultility.Ultility.AddList(channels, var.GetChannels());
            }

            return channels;
        }

        public override string ToString()
        {
            if (this.ProcessesCounter == null)
            {
                return "(||| {" + RangeExpression.ToString() + "} @" + Processes[0] + ")";
            }

            string toReturn = "";

            Debug.Assert(ProcessesCounter != null);

            foreach (Process processe in Processes)
            {
                if (ProcessesCounter[processe.ProcessID] == -1)
                {
                    toReturn += "(||| {..} @" + processe + ")|||";
                }
                else
                {
                    toReturn += "(||| {" + ProcessesCounter[processe.ProcessID] + "} @" + processe + ")|||";
                }
            }

            return toReturn.TrimEnd('|');
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            int size = Processes.Count;
            List<Process> newProceses = new List<Process>(size);
            Dictionary<string, int> processCounters = new Dictionary<string, int>(size);
            IndexInterleaveAbstract interleaveAbstract = null;

            if (Specification.IsParsing)
            {
                if (ProcessesCounter == null)
                {
                    return new IndexInterleaveAbstract(Processes[0].ClearConstant(constMapping), RangeExpression.ClearConstant(constMapping));
                }

                for (int i = 0; i < Processes.Count; i++)
                {
                    int outer = ProcessesCounter[Processes[i].ProcessID];
                    Process newProc = Processes[i].ClearConstant(constMapping);

                    string temp = newProc.ProcessID;

                    if (!processCounters.ContainsKey(temp))
                    {
                        newProceses.Add(newProc);
                        processCounters.Add(temp, outer);
                    }
                    else
                    {
                        if (processCounters[temp] != -1)
                        {
                            processCounters[temp] = processCounters[temp] + outer;
                        }
                    }
                }

                interleaveAbstract = new IndexInterleaveAbstract(newProceses, processCounters);
                interleaveAbstract.ProcessesActualSize = new Dictionary<string, int>(processCounters);
                return interleaveAbstract;
            }

            //clear constant must be side effect free
            Dictionary<string, int> newProcessesCounter = ProcessesCounter;
           // Dictionary<string, int> newProcessesActualSize = ProcessesActualSize;

            if (ProcessesCounter == null)
            {
                Expression val = RangeExpression.ClearConstant(constMapping);
                size = (EvaluatorDenotational.Evaluate(val, null) as IntConstant).Value;

                if (size == 0)
                {
                    return new Skip();
                }
                else if (size < 0)
                {
                    throw new ParsingException("Negative range " + size + " for parameterized interleave: " + RangeExpression, 0, 0, RangeExpression.ToString());
                }

                newProcessesCounter = new Dictionary<string, int>();
                newProcessesCounter.Add(Processes[0].ProcessID, size);
            }
            
            for (int i = 0; i < Processes.Count; i++)
            {
                int outer = newProcessesCounter[Processes[i].ProcessID];
                Process newProc = Processes[i].ClearConstant(constMapping);
                
                if (newProc is IndexInterleave)
                {
                    List<Process> processes = (newProc as IndexInterleave).Processes;
                    
                    for (int j = 0; j < processes.Count; j++)
                    {
                        string temp = processes[j].ProcessID;
                        if (!processCounters.ContainsKey(temp))
                        {
                            processCounters.Add(temp, outer);
                            newProceses.Add(processes[j]);                          
                        }
                        else
                        {
                            if (processCounters[temp] != -1)
                            {
                                processCounters[temp] = processCounters[temp] + outer;
                            } 
                        }
                    }
                }
                else if (newProc is IndexInterleaveAbstract)
                {
                    IndexInterleaveAbstract intAbs = newProc as IndexInterleaveAbstract;

                    for (int j = 0; j < intAbs.Processes.Count; j++)
                    {
                        string temp = intAbs.Processes[j].ProcessID;
                        int inner = intAbs.ProcessesCounter[temp];

                        if (!processCounters.ContainsKey(temp))
                        {
                            newProceses.Add(intAbs.Processes[j]);

                            if (outer != -1 && inner != -1)
                            {
                                processCounters.Add(temp, inner * outer);                                
                            }
                            else
                            {
                                processCounters.Add(temp, -1);
                            }
                        }
                        else
                        {
                            if (processCounters[temp] != -1)
                            {
                                if (inner == -1 || outer == -1)
                                {
                                    processCounters.Add(temp,-1);                                        
                                }
                                else
                                {
                                    processCounters[temp] = processCounters[temp] + outer*inner;                                                                         
                                }
                            }                            
                        }
                    }
                }
                else
                {
                    string temp = newProc.ProcessID;

                    if (!processCounters.ContainsKey(temp))
                    {
                        newProceses.Add(newProc);
                        processCounters.Add(temp, outer);                        
                    }
                    else
                    {
                        if (processCounters[temp] != -1)
                        {
                            processCounters[temp] = processCounters[temp] + outer;
                        }
                    }
                }
            }

            interleaveAbstract =  new IndexInterleaveAbstract(newProceses, processCounters);
            interleaveAbstract.ProcessesActualSize = new Dictionary<string, int>(processCounters);
            return interleaveAbstract;
        }

        public override bool MustBeAbstracted()
        {
            if (this.ProcessesCounter == null)
            {
                return Processes[0].MustBeAbstracted();
            }

            for (int i = 0; i < Processes.Count; i++)
            {
                string tmp = Processes[i].ProcessID;

                if (ProcessesActualSize[tmp] == -1)
                {
                    return true;
                }  

                bool toReturn = Processes[i].MustBeAbstracted();
                if(toReturn)
                {
                    return true;
                }              
            }

            return false;
        }

        public override bool IsBDDEncodable()
        {
            return false;
        }

        public override Process GetTopLevelConcurrency(List<string> visitedDef)
        {
            return this;
        }      
    }
}