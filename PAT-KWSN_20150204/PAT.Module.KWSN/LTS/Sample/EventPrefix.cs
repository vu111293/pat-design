using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class EventPrefix : Process
    {
        public Event Event;
        public Process Process;
        
        public EventPrefix(Event e, Process process)
        {
            Event = e;
            Process = process;
            ProcessID = DataStore.DataManager.InitializeProcessID(Event.GetID() + Constants.EVENTPREFIX + Process.ProcessID);
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            string ID = Event.GetEventID(GlobalEnv);
            string name = Event.GetEventName(GlobalEnv);
            
            if(ID != name)
            {
                list.Add(new Configuration(Process, ID ,name, GlobalEnv, false));
            }
            else
            {
                list.Add(new Configuration(Process, ID, null, GlobalEnv, false));
            }
        }

        public override string ToString()
        {
            return "(" + Event + "->" + Process.ToString() + ")";
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            HashSet<string> list = Process.GetAlphabets(visitedDefinitionRefs);

            if (Specification.CollectDataOperationEvent == null)
            {
                if (Event.ExpressionList != null)
                {
                    foreach (Expression expression in Event.ExpressionList)
                    {
                        if (expression.HasVar)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("ERROR - PAT FAILED to calculate the alphabet.");
                            sb.AppendLine("CAUSE - Event " + Event + " contains global variables!");
                            sb.AppendLine(
                                "REMEDY - 1) Avoid using global variables in events 2) Or manually specify the alphabet of the relevant process using the following syntax: \n\r\t #alphabet someProcess {X}; \n\rwhere X is a set of event names.");
                            throw new RuntimeException(sb.ToString());
                        }
                    }
                }

                string name = Event.GetEventID(null);

                if (name != Constants.TAU && !list.Contains(name))
                {
                    list.Add(name);
                }
            }
            else if (Specification.CollectDataOperationEvent == false && Event.ToString() != Constants.TAU)
            {
                HashSet<string> set = Process.GetAlphabets(visitedDefinitionRefs);
                set.Add(Event.BaseName);

                return set;
            }
            return list;
        }

        public override List<string> GetGlobalVariables()
        {
            List<string> toReturn = Process.GetGlobalVariables();

            if (Event.ExpressionList != null)
            {
                foreach (Expression expression in Event.ExpressionList)
                {
                    Common.Classes.Ultility.Ultility.AddList(toReturn,expression.GetVars());
                }
            }

            return toReturn;
        }

        public override List<string> GetChannels()
        {
            return Process.GetChannels();
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return new EventPrefix(Event.ClearConstant(constMapping), Process.ClearConstant(constMapping));
        }

        public override bool MustBeAbstracted()
        {
            return Process.MustBeAbstracted();
        }

        public override bool IsBDDEncodable()
        {
            return Process.IsBDDEncodable();
        }
    }
}