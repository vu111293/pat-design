using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public class CountedProcess
    {
        public ProcessBase Process;
        public Expression RangeExpression;

        public CountedProcess(ProcessBase process, Expression range)
        {
            Process = process;
            RangeExpression = range;
        }

        public List<string> GetGlobalVariables(List<string> visitedDef)
        {
            List<string> Variables = new List<string>();


            Common.Classes.Ultility.Ultility.AddList(Variables, RangeExpression.Vars);
            Common.Classes.Ultility.Ultility.AddList(Variables, Process.GetGlobalVariables(visitedDef));

            return Variables;
        }

        public bool GlobalVariablesAsIndex(List<string> visitedDef)
        {
           return RangeExpression.Vars.Count > 0;
           
        }

        public List<ProcessBase> GetCountedProcess(SortedDictionary<string, Expression> environment)
        {
            Dictionary<string, Expression> constMapping;

            if(environment != null)
            {
                constMapping = new Dictionary<string, Expression>(environment);
            }
            else
            {
                constMapping = new Dictionary<string, Expression>();
            }
            

            //ProcessBase process = this.Process.ClearConstant(constMapping);
            List<ParallelDefinition> newDefinitions = new List<ParallelDefinition>();
           // bool hasVar = false;
            foreach (ParallelDefinition definition in Definitions)
            {
                ParallelDefinition newPD = definition.ClearConstant(constMapping);
                //if (newPD.HasVaraible)
                //{
                //    hasVar = true;
                //}
                newDefinitions.Add(newPD);
              

            }

           // System.Diagnostics.Debug.Assert(!hasVar);
            //if (hasVar)
            //{
            //    return new IndexInterleaveTemp(Process, newDefinitions);
            //}
            //else
            {
                List<ProcessBase> processes = new List<ProcessBase>(16);

                //List<string> avoidsVars = new List<string>();
                foreach (ParallelDefinition pd in newDefinitions)
                {
                    pd.DomainValues.Sort();
                }

                List<List<Expression>> list = new List<List<Expression>>();
                foreach (int v in newDefinitions[0].DomainValues)
                {
                    List<Expression> l = new List<Expression>(newDefinitions.Count);
                    l.Add(new IntConstant(v));
                    list.Add(l);
                }

                for (int i = 1; i < newDefinitions.Count; i++)
                {
                    List<List<Expression>> newList = new List<List<Expression>>();
                    List<int> domain = newDefinitions[i].DomainValues;

                    for (int j = 0; j < list.Count; j++)
                    {
                        foreach (int i1 in domain)
                        {
                            List<Expression> cList = new List<Expression>(list[j]);
                            cList.Add(new IntConstant(i1));
                            newList.Add(cList);
                        }
                    }
                    list = newList;
                }

                foreach (List<Expression> constants in list)
                {
                    Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>();
                    for (int i = 0; i < constants.Count; i++)
                    {
                        Expression constant = constants[i];
                        constant.BuildVars();
                        constMappingNew.Add(newDefinitions[i].Parameter, constant);

                      
                    }

                    ProcessBase newProcess = Process.ClearConstant(constMappingNew);
                    processes.Add(newProcess);
                }


                return processes;
            }
        }

        public IndexedProcess ClearConstant(Dictionary<string, Expression> constMapping)
        {
            ProcessBase newProcess = Process.ClearConstant(constMapping);

            List<ParallelDefinition> newDefinitions = new List<ParallelDefinition>(Definitions.Count);

            foreach (ParallelDefinition definition in Definitions)
            {
                newDefinitions.Add(definition.ClearConstant(constMapping));
            }

            return new IndexedProcess(newProcess, newDefinitions);
        }

        public override string ToString()
        {
            string returnString = "";
            foreach (ParallelDefinition list in Definitions)
            {
                returnString += list.ToString() + ";";
            }

            return returnString.TrimEnd(';') + "@" + Process;
        }
    }
}
