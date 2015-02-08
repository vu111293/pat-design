using System.Text;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class ChannelInput : Process
    {
        public string ChannelName;
        public Expression[] ExpressionList;
        public Process Process;
        
        public ChannelInput(string evtName, Expression[] exp, Process process)
        {
            ChannelName = evtName;

            ExpressionList = exp;
            Process = process;

            StringBuilder ID = new StringBuilder();
            ID.Append(ChannelName);
            ID.Append("?");
            ID.Append(Common.Classes.Ultility.Ultility.PPIDListDot(ExpressionList));
            ID.Append(Constants.EVENTPREFIX);
            ID.Append(Process.ProcessID);
            ProcessID = DataStore.DataManager.InitializeProcessID(ID.ToString());
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            ChannelQueue Buffer = null;

            if (GlobalEnv.Channels.TryGetValue(this.ChannelName, out Buffer))
            {

                if (Buffer.Count > 0)
                {
                    ChannelQueue newBuffer = Buffer.Clone();
                    ExpressionValue[] values = newBuffer.Dequeue();

                    if (values.Length == ExpressionList.Length)
                    {
                        Dictionary<string, Expression> mapping = new Dictionary<string, Expression>(values.Length);

                        Valuation newEnv = GlobalEnv.GetChannelClone();
                        newEnv.Channels[ChannelName] = newBuffer;

                        string eventName = ChannelName + "?";
                        string eventID = ChannelName + "?";


                        for (int i = 0; i < ExpressionList.Length; i++)
                        {
                            ExpressionValue v = values[i];
                            if (i == ExpressionList.Length - 1)
                            {
                                eventName += v;
                                eventID += v.ExpressionID;//.GetID();
                            }
                            else
                            {
                                eventName += v + ".";
                                eventID += v.ExpressionID + ".";
                            }

                            if (ExpressionList[i] is Variable)
                            {
                                mapping.Add(ExpressionList[i].ExpressionID, v); //.GetID()
                            }
                            else
                            {
                                if (v.ExpressionID != ExpressionList[i].ExpressionID) //.GetID() .GetID() 
                                {
                                    return; //list
                                }
                            }
                        }

                        Process newProcess = mapping.Count > 0 ? Process.ClearConstant(mapping) : Process;

                        if (eventID != eventName)
                        {
                            list.Add(new Configuration(newProcess, eventID, eventName, newEnv, false));
                        }
                        else
                        {
                            list.Add(new Configuration(newProcess, eventID, null, newEnv, false));
                        }
                    }
                }
            }

            //return list;
        }

        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
            //List<Configuration> list = new List<Configuration>(1);
            if (eStep.ChannelName == ChannelName && eStep.Expressions.Length == ExpressionList.Length)
            {
                Dictionary<string, Expression> mapping = new Dictionary<string, Expression>(eStep.Expressions.Length);

                for (int i = 0; i < ExpressionList.Length; i++)
                {
                    Expression v = eStep.Expressions[i];
                   
                    if (ExpressionList[i] is Variable)
                    {
                        mapping.Add(ExpressionList[i].ExpressionID, v); //.GetID()
                    }
                    else
                    {
                        if (v.ExpressionID != ExpressionList[i].ExpressionID) //.GetID()
                        {
                            return ;
                        }
                    }
                }

                Configuration vm;
                if (mapping.Count > 0)
                {
                    vm = new Configuration(Process.ClearConstant(mapping), null, null, eStep.GlobalEnv, false);
                }
                else
                {
                    vm = new Configuration(Process, null, null, eStep.GlobalEnv, false);
                }

                list.Add(vm);
            }

            //return list;                
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
            List<string> toReturn = Process.GetChannels();

            if (!toReturn.Contains(ChannelName))
            {
                toReturn.Add(ChannelName);
            }

            return toReturn;
        }

        public override string ToString()
        {
            return ChannelName + "?" + Common.Classes.Ultility.Ultility.PPStringListDot(ExpressionList).TrimStart('.') + "->" + Process;
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newExpression = new Expression[ExpressionList.Length];
            for (int i = 0; i < ExpressionList.Length; i++)
            {
                if (ExpressionList[i] is ExpressionValue)
                {
                    newExpression[i] = ExpressionList[i];
                }
                else
                {
                    newExpression[i] = ExpressionList[i].ClearConstant(constMapping);
                    //evaluate the value after the clearance, to make sure there only single variable or single value for each expression
                    if (!newExpression[i].HasVar)
                    {
                        newExpression[i] = EvaluatorDenotational.Evaluate(newExpression[i], null);
                    }                        
                }                
            }

            return new ChannelInput(ChannelName, newExpression, Process.ClearConstant(constMapping));
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