﻿using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;


namespace PAT.KWSN.LTS{
    public sealed class IndexChoice : Process
    {
        public List<Process> Processes;
        public IndexedProcess IndexedProcessDefinition;

        #region constructors 

        /// <summary>
        /// constructor for indexed choice.
        /// </summary>
        /// <param name="definition"></param>
        public IndexChoice(IndexedProcess definition)
        {
            IndexedProcessDefinition = definition;
        }

        /// <summary>
        /// constructor for choices of multiple processes.
        /// </summary>
        /// <param name="processes"></param>
        public IndexChoice(List<Process> processes)
        {
            Processes = processes;

            StringBuilder ID = new StringBuilder();
            foreach (Process processBase in Processes)
            {
                ID.Append(Constants.GENERAL_CHOICE);
                ID.Append(processBase.ProcessID);
            }

            ProcessID = DataStore.DataManager.InitializeProcessID(ID.ToString());
        }
        #endregion

        #region Runtime functions
        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            for (int i = 0; i < Processes.Count; i++)
            {
                //note: here needs to create a new list1. reusing list will give error if the process[1] is a seqencial
                List<Configuration> list1 = new List<Configuration>();
                Processes[i].MoveOneStep(GlobalEnv, list1);
                list.AddRange(list1);
            }            
        }

        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
            for (int i = 0; i < Processes.Count; i++)
            {
                List<ConfigurationWithChannelData> list1 = new List<ConfigurationWithChannelData>();
                Processes[i].SyncOutput(GlobalEnv, list1);
                list.AddRange(list1);
            }
        }

        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
            for (int i = 0; i < Processes.Count; i++)
            {
                List<Configuration> list1 = new List<Configuration>();
                Processes[i].SyncInput(eStep, list1);
                list.AddRange(list1);
            }
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.GetAlphabets(visitedDefinitionRefs);
            }

            HashSet<string> list = new HashSet<string>();
            for (int i = 0; i < Processes.Count; i++)
            {
                list.UnionWith(Processes[i].GetAlphabets(visitedDefinitionRefs));
            }
            return list;
        }


        //public override List<string> GetFairEvents(Configuration eStep, bool isWeak, bool isLive)
        //{
        //    List<string> list = new List<string>();
        //    for (int i = 0; i < this.Processes.Count; i++)
        //    {
        //        Common.Classes.Ultility.Ultility.AddList(list, Processes[i].GetFairEvents(eStep, isWeak, isLive));
        //    }
        //    return list;
        //}

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            List<Process> newnewListProcess = Processes;
            if (Processes == null)
            {
                if (Specification.IsParsing)
                {
                    return new IndexChoice(IndexedProcessDefinition.ClearConstant(constMapping));
                }

                //return new IndexChoice(IndexedProcessDefinition.GetIndexedProcesses(constMapping));
                newnewListProcess = IndexedProcessDefinition.GetIndexedProcesses(constMapping);
            }

            List<Process> newListProcess = new List<Process>();
            for (int i = 0; i < newnewListProcess.Count; i++)
            {
                Process newProc = newnewListProcess[i].ClearConstant(constMapping);
                if (!(newProc is Stop))
                {
                    newListProcess.Add(newProc);
                }
            }

            //all processes are stop processes
            if (newListProcess.Count == 0)
            {
                return new Stop();
            }

            return new IndexChoice(newListProcess);
        }
        #endregion

        #region static functions
        public override List<string> GetChannels()
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.GetChannels();
            }

            List<string> channels = new List<string>();
            for (int i = 0; i < Processes.Count; i++)
            {
                List<string> temp = Processes[i].GetChannels();
                Common.Classes.Ultility.Ultility.AddList<string>(channels, temp);
            }
            return channels;
        }

        public override bool MustBeAbstracted()
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.MustBeAbstracted();
            }

            for (int i = 0; i < Processes.Count; i++)
            {
                if (Processes[i].MustBeAbstracted())
                {
                    return true;
                }
            }

            return false;
        }

        public override bool IsBDDEncodable()
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.MustBeAbstracted();
            }

            for (int i = 0; i < Processes.Count; i++)
            {
                if (!Processes[i].IsBDDEncodable())
                {
                    return false;
                }
            }

            return true;
        }

        public override List<string> GetGlobalVariables()
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.GetGlobalVariables();
            }

            List<string> Variables = new List<string>();
            for (int i = 0; i < Processes.Count; i++)
            {
                List<string> temp = Processes[i].GetGlobalVariables();
                Common.Classes.Ultility.Ultility.AddList<string>(Variables, temp);
            }
            return Variables;
        }

        #endregion

        public override string ToString()
        {
            if (Processes == null)
            {
                return "[]" + IndexedProcessDefinition;
            }

            string result = "(" + Processes[0];
            for (int i = 1; i < Processes.Count; i++)
            {
                result += "[]";
                result += Processes[i].ToString();
            }

            result += ")";
            return result;
        }
    }

}