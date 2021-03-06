﻿using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class IndexInternalChoice:Process
    {
        public List<Process> Processes;
        public IndexedProcess IndexedProcessDefinition;

        public IndexInternalChoice(IndexedProcess definition)
        {
            IndexedProcessDefinition = definition;
        }

        public IndexInternalChoice(List<Process> processes)
        {
            Processes = new List<Process>();
            StringBuilder ID = new StringBuilder();

            foreach (Process proc in processes)
            {
                if (proc is IndexInternalChoice && (proc as IndexInternalChoice).Processes != null)
                {
                    List<Process> processes1 = (proc as IndexInternalChoice).Processes;

                    Processes.AddRange(processes1);

                    foreach (Process processe in processes1)
                    {
                        ID.Append(Constants.INTERNAL_CHOICE);
                        ID.Append(processe.ProcessID);
                    }
                }
                else
                {
                    Processes.Add(proc);
                    ID.Append(Constants.INTERNAL_CHOICE);
                    ID.Append(proc.ProcessID);
                }
            }

            ProcessID = DataStore.DataManager.InitializeProcessID(ID.ToString());
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            for (int i = 0; i < Processes.Count; i++)
            {
                list.Add(new Configuration(Processes[i], Constants.TAU, "[int_choice]", GlobalEnv, false));
            }

            //return list;
        }


        public override string ToString()
        {
            if (Processes == null)
            {
                return "<>" + IndexedProcessDefinition;
            }

            string result = "(" + Processes[0];
            for (int i = 1; i < Processes.Count; i++)
            {
                result += "<>";
                result += Processes[i].ToString();
            }
            result += ")";
            return result;
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
                Common.Classes.Ultility.Ultility.AddList<string>(Variables, Processes[i].GetGlobalVariables());
            }
            return Variables;
        }

        public override List<string> GetChannels()
        {
            if (Processes == null)
            {
                return IndexedProcessDefinition.Process.GetChannels();
            }

            List<string> channels = new List<string>();
            for (int i = 0; i < this.Processes.Count; i++)
            {
                Common.Classes.Ultility.Ultility.AddList<string>(channels, this.Processes[i].GetChannels());
            }
            return channels;
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
                list.UnionWith(this.Processes[i].GetAlphabets(visitedDefinitionRefs));
            }
            return list;
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            List<Process> newnewListProcess = Processes;
            if (Processes == null)
            {
                if (Specification.IsParsing)
                {
                    return new IndexInternalChoice(IndexedProcessDefinition.ClearConstant(constMapping));
                }

                newnewListProcess = IndexedProcessDefinition.GetIndexedProcesses(constMapping);
            }

            List<Process> newListProcess = new List<Process>();
            for (int i = 0; i < newnewListProcess.Count; i++)
            {
                Process newProc = newnewListProcess[i].ClearConstant(constMapping);
                newListProcess.Add(newProc);
            }
            return new IndexInternalChoice(newListProcess);
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
                return IndexedProcessDefinition.Process.IsBDDEncodable();
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
    }
}