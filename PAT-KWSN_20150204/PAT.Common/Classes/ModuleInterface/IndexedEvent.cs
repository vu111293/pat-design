using System;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public sealed class IndexedEvent 
    {
        public List<ParallelDefinition> Definitions;
        public Event Event;

        public IndexedEvent(List<ParallelDefinition> definitions, Event evt)
        {
            Definitions = definitions;
            Event = evt;
        }
        
        public override string ToString()
        {
            string returnString = "";
            if (Definitions != null)
            {
                foreach (ParallelDefinition list in Definitions)
                {
                    returnString += list.ToString() + ";";
                }
            }

            return "|| " + returnString.TrimEnd(';') + "@ {" + Event.ToString() + "}";
        }

        public IndexedEvent ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Event newEvent = Event.ClearConstant(constMapping);

            List<ParallelDefinition> newDefinitions = new List<ParallelDefinition>(Definitions.Count);

            foreach (ParallelDefinition definition in Definitions)
            {
                newDefinitions.Add(definition.ClearConstant(constMapping));
            }

            return new IndexedEvent(newDefinitions, newEvent);
        }

        public List<Event> GetIndexedEvents()
        {
            List<Event> events = new List<Event>(16);

            foreach (ParallelDefinition pd in Definitions)
            {
                if (pd.GetGlobalVariables().Count > 0)
                {
                    throw new Exception("Global variable can not be used in the index events!");
                }
                pd.DomainValues.Sort();
            }

            List<List<Expression>> list = new List<List<Expression>>();
            foreach (int v in Definitions[0].DomainValues)
            {
                List<Expression> l = new List<Expression>(Definitions.Count);
                l.Add(new IntConstant(v));
                list.Add(l);
            }

            for (int i = 1; i < Definitions.Count; i++)
            {
                List<List<Expression>> newList = new List<List<Expression>>();
                List<int> domain = Definitions[i].DomainValues;

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
                //Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>(constMapping);
                Dictionary<string, Expression> constMappingNew = new Dictionary<string, Expression>();
                for (int i = 0; i < constants.Count; i++)
                {
                    Expression constant = constants[i];
                    //constant.BuildVars();
                    constMappingNew.Add(Definitions[i].Parameter, constant);

                }

                Event newEvent = Event.ClearConstant(constMappingNew);
                events.Add(newEvent);
            }

            return events;
        }
    }
}
