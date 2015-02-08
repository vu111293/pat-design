using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.Expressions
{
    public class Valuation
    {
        
        public Dictionary<string, ChannelQueue> Channels;
        public StringDictionaryWithKey<ExpressionValue> Variables;

        public static StringDictionary<string> ValutionHashTable = new StringDictionary<string>(8);
        public static StringDictionary<int> VariableLowerBound = new StringDictionary<int>(8);
        public static StringDictionary<int> VariableUpperLowerBound = new StringDictionary<int>(8);

        public static bool HasVariableConstraints;
        public static StringHashTable HiddenVars;
        //public static StringHashTable WildVars;
        public string ID;

        // For recursive functions, we need an operation
        // that makes a binding in a given environment
        // destructively. That means that the given environment
        // is changed. The environment before the change is
        // lost.
        public void ExtendDestructive(string x, ExpressionValue v)
        {
            if (!Variables.SetValue(x, v))
            {
                this.Variables.Add(x, v);
            }
        }

 
        public static void CheckVariableRange(string x, ExpressionValue v)
        {
#if !OPTIMAL_FOR_EXP      
            if (HasVariableConstraints && v is IntConstant)
            {
                int val = (v as IntConstant).Value;

                if (VariableLowerBound.ContainsKey(x))
                {
                    int bound = VariableLowerBound.GetContainsKey(x);
                    if (bound > val)
                    {
                        throw new VariableValueOutOfRangeException("Variable " + x + "'s current value " + val + " is smaller than its lower bound " + bound);
                    }
                }

                if (VariableUpperLowerBound.ContainsKey(x))
                {
                    int bound = VariableUpperLowerBound.GetContainsKey(x);
                    if (val > bound)
                    {
                        throw new VariableValueOutOfRangeException("Variable " + x + "'s current value " + val + " is greater than its upper bound " + bound);
                    }
                }
            }
#endif
        }

        public static void CheckVariableRange(string x, ExpressionValue v, int line, int position)
        {
#if !OPTIMAL_FOR_EXP    
            if (VariableLowerBound.Count > 0 || VariableUpperLowerBound.Count > 0)
            {
                if (v is IntConstant)
                {
                    int val = (v as IntConstant).Value;

                    if (VariableLowerBound.ContainsKey(x))
                    {
                        int bound = VariableLowerBound.GetContainsKey(x);
                        if (bound > val)
                        {
                            throw new ParsingException(
                                "Variable " + x + "'s current value " + val + " is smaller than its lower bound " +
                                bound, line, position, x);
                        }
                    }

                    if (VariableUpperLowerBound.ContainsKey(x))
                    {
                        int bound = VariableUpperLowerBound.GetContainsKey(x);
                        if (val > bound)
                        {
                            throw new ParsingException(
                                "Variable " + x + "'s current value " + val + " is greater than its upper bound " +
                                bound, line, position, x);
                        }
                    }
                }
                else if(v is RecordValue)
                {
                    RecordValue record = v as RecordValue;
                    foreach (ExpressionValue value in record.Associations)
                    {
                        CheckVariableRange(x, value, line, position);
                    }
                }
            }
#endif
        }

        public virtual Valuation GetClone()
        {
            Valuation newEnv = new Valuation();
            if (Variables != null)
            {
                StringDictionaryWithKey<ExpressionValue> newVars = new StringDictionaryWithKey<ExpressionValue>(Variables);
                for (int i = 0; i < Variables._entries.Length; i++)
                {
                    StringDictionaryEntryWithKey<ExpressionValue> pair = Variables._entries[i];
                    if (pair != null)
                    {
                        newVars._entries[i] = new StringDictionaryEntryWithKey<ExpressionValue>(pair.HashA, pair.HashB, pair.Value.GetClone(), pair.Key);
                    }
                }

                newEnv.Variables = newVars;
            }

            if (Channels != null)
            {
                newEnv.Channels = new Dictionary<string, ChannelQueue>(this.Channels);
            }

            return newEnv;
        }


        public virtual Valuation GetVariableClone()
        {
            Valuation newEnv = new Valuation();
            if (Variables != null)
            {
                StringDictionaryWithKey<ExpressionValue> newVars = new StringDictionaryWithKey<ExpressionValue>(Variables);
                for (int i = 0; i < Variables._entries.Length; i++)
                {
                    StringDictionaryEntryWithKey<ExpressionValue> pair = Variables._entries[i];
                    if (pair != null)
                    {
                        newVars._entries[i] = new StringDictionaryEntryWithKey<ExpressionValue>(pair.HashA, pair.HashB, pair.Value.GetClone(), pair.Key);
                    }
                }
                newEnv.Variables = newVars;
            }

            newEnv.Channels = this.Channels;
            return newEnv;
        }

        /// <summary>
        /// Clone only the relevant environment, i.e., the variables provided.
        /// </summary>
        /// <param name="visibleVars"></param>
        /// <returns></returns>
        public virtual Valuation GetVariableClone(List<string> visibleVars)
        {
            Valuation newEnv = new Valuation();
            if (Variables != null)
            {
                StringDictionaryWithKey<ExpressionValue> newVar = new StringDictionaryWithKey<ExpressionValue>();
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in Variables._entries)
                {
                    if (pair != null)
                    {
                        foreach (string var in visibleVars)
                        {
                            if (var == pair.Key)
                            {
                                newVar.Add(pair.Key, pair.Value.GetClone());
                                break;
                            }
                        }
                    }
                }
                newEnv.Variables = newVar;
            }
            newEnv.Channels = this.Channels;
            return newEnv;
        }


        /// <summary>
        /// todo: when channel clone, we also clone the DMB, since they are used together always.
        /// </summary>
        /// <returns></returns>
        public virtual Valuation GetChannelClone()
        {
            Valuation newEnv = new Valuation();
            newEnv.Variables = this.Variables;
            Debug.Assert(Channels != null);
            newEnv.Channels = new Dictionary<string, ChannelQueue>(this.Channels);

            return newEnv;
        }

        public virtual Valuation GetVariableChannelClone(List<string> visibleVars, List<string> visibleChannels)
        {
            Valuation newEnv = GetVariableClone(visibleVars);

            if (Channels != null)
            {
                Dictionary<string, ChannelQueue> channels = new Dictionary<string, ChannelQueue>();

                foreach (KeyValuePair<string, ChannelQueue> pair in Channels)
                {
                    if (visibleChannels.Contains(pair.Key))
                    {
                        if (pair.Value.Size != 0)
                        {
                            channels.Add(pair.Key, pair.Value);
                        }
                    }
                }

                newEnv.Channels = channels;
            }
            return newEnv;
        }

        public virtual string GetID()
        {
            if (ID == null)
            {
                StringBuilder IDBuilder = new StringBuilder();
                if (Variables != null)
                {
                    if (HiddenVars != null)
                    {
                        foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in this.Variables._entries)
                        {
                            if (pair != null && !HiddenVars.ContainsKey(pair.Key))
                            {
                                IDBuilder.Append(pair.Value.ExpressionID + "&");
                            }
                        }
                    }
                    else
                    {
                        foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in this.Variables._entries)
                        {
                            if (pair != null)
                            {
                                IDBuilder.Append(pair.Value.ExpressionID + "&");
                            }
                        }
                    }

                }

                if (Channels != null && Channels.Count > 0)
                {
                    foreach (ChannelQueue value in Channels.Values)
                    {
                        IDBuilder.Append(value.GetID());
                        IDBuilder.Append("+");
                    }
                }

                ID = IDBuilder.ToString();
                string newID = ValutionHashTable.GetContainsKey(ID);

                if (newID == null)
                {
                    newID = ValutionHashTable.Count.ToString(); 
                    ValutionHashTable.Add(ID, newID);
                }

                ID = newID;
            }

            return ID;
        }

        public virtual string GetID(string processID)
        {
            return GetID() + Constants.SEPARATOR + processID;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (Variables != null)
            {
                sb.AppendLine("Variables:");
                foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in Variables._entries)
                {
                    if (pair != null)
                    {
                        sb.AppendLine(pair.Key + "=" + pair.Value.ToString()+ ";"); //.ToString() 
                    }
                }
            }

            if (Channels != null && Channels.Count > 0)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                }
                sb.AppendLine("Channels:");
                foreach (KeyValuePair<string, ChannelQueue> pair in Channels)
                {
                    sb.AppendLine(pair.Key + "=<" + pair.Value.ToString() + ">;");
                }
            }

            return sb.ToString();
        }

        public virtual bool IsEmpty()
        {
            return Variables == null && Channels == null;
        }
    }
}