using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class AtomicProcess : Process
    {
        public Process Process;
        public bool Started;

        public AtomicProcess(Process process, bool started)
        {
            if (process is AtomicProcess)
            {
                Process = (process as AtomicProcess).Process;
            }
            else
            {
                Process = process;                
            }

            Started = started;
            
            if(started)
            {
                ProcessID = DataStore.DataManager.InitializeProcessID(Constants.ATOMIC_STARTED + Process.ProcessID);    
            }
            else
            {
                ProcessID = DataStore.DataManager.InitializeProcessID(Constants.ATOMIC_NOTSTARTED + Process.ProcessID);    
            }            
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            Process.MoveOneStep(GlobalEnv, list);

            foreach (Configuration configuration in list)
            {
                configuration.Process = new AtomicProcess(configuration.Process, true);
                configuration.IsAtomic = Started;
            }
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new AtomicProcess(Process.ClearConstant(constMapping), Started);
        }

        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
            Process.SyncOutput(GlobalEnv, list);
            foreach (ConfigurationWithChannelData step in list)
            {
                step.Process = new AtomicProcess(step.Process, true);
                step.IsAtomic = Started;
            }
            //return steps;
        }

        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
            Process.SyncInput(eStep, list);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Process = new AtomicProcess(list[i].Process, true);
                list[i].IsAtomic = Started;
            }

            //return returnlist;
        }

        public override string ToString()
        {
            return " atomic{" + Process + "}";
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            return Process.GetAlphabets(visitedDefinitionRefs);
        }

        public override List<string> GetGlobalVariables()
        {
            return Process.GetGlobalVariables();
        }

        public override List<string> GetChannels()
        {
            return Process.GetChannels();
        }

        public override bool MustBeAbstracted()
        {
            return Process.MustBeAbstracted();
        }

        public override Process GetTopLevelConcurrency(List<string> visitedDef)
        {
            return Process.GetTopLevelConcurrency(visitedDef);
        }

        public override bool IsBDDEncodable()
        {
            return false;
        }

    }
}
