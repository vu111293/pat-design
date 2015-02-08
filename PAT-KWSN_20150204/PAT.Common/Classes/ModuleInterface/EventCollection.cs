using System.Text;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public sealed class EventCollection : List<Event>
    {
        public List<string> EventNames;
        public string ProcessID;

        public EventCollection()
        {
        }

        public EventCollection(List<Event> events)
        {
            this.AddRange(events);
            EventNames = new List<string>(Count);
            StringBuilder sb = new StringBuilder("{");

            for (int i = 0; i < Count; i++)
            {
                string name = null;
                if (!this[i].ContainsVariable())
                {
                    name = this[i].GetEventID(null);
                }
                else
                {
                    name = this[i].GetID();
                }
                 
                sb.Append(name);
                EventNames.Add(name);

                if (i < Count - 1)
                {
                    sb.Append(",");
                }
            }

            sb.Append("}");

            ProcessID = sb.ToString();            
        }

        public bool HasExternalLibraryCall ()
        {
            foreach (Event evt in this)
            {
                if (evt.HasExternalLibraryCall())
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainEventName(string evt)
        {          
            return EventNames.Contains(evt);
        }

        public EventCollection ClearConstant(Dictionary<string, Expression> constMapping)
        {
            List<Event> newEC = new List<Event>();

            for (int i = 0; i < Count; i++)
            {
                newEC.Add(this[i].ClearConstant(constMapping));
            }
            return new EventCollection(newEC);
        }

        public bool ContainsVariable()
        {
            foreach (Event variable in this)
            {
                if (variable.ContainsVariable())
                {
                    return true;
                }
            }

            return false;
        }              
    }
}