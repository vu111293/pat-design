using System.Text;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;


namespace PAT.KWSN.LTS{
    public sealed class ChannelOutput : Process
    {
        public string ChannelName;
        public  Expression[] ExpressionList;
        public  Process Process;

        public ChannelOutput(string evtName, Expression[] e, Process process)
        {
            ChannelName = evtName;
            Process = process;
            ExpressionList = e;

            StringBuilder ID = new StringBuilder();
            ID.Append(ChannelName);
            ID.Append("!");
            ID.Append(Common.Classes.Ultility.Ultility.PPIDListDot(ExpressionList));
            ID.Append(Constants.EVENTPREFIX);
            ID.Append(Process.ProcessID);

            ProcessID = DataStore.DataManager.InitializeProcessID(ID.ToString());
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            Valuation globalEnv = GlobalEnv;
            ChannelQueue Buffer = null;
            
            if (globalEnv.Channels.TryGetValue(this.ChannelName,out Buffer))
            {
                if (Buffer.Count < Buffer.Size)
                {

                    ExpressionValue[] values = new ExpressionValue[ExpressionList.Length];
                    string eventName = ChannelName + "!";
                    string eventID = ChannelName + "!";

                    for (int i = 0; i < ExpressionList.Length; i++)
                    {
                        values[i] = EvaluatorDenotational.Evaluate(ExpressionList[i], globalEnv);
                        if (i == ExpressionList.Length - 1)
                        {
                            eventName += values[i].ToString();
                            eventID += values[i].ExpressionID;
                        }
                        else
                        {
                            eventName += values[i].ToString() + ".";
                            eventID += values[i].ExpressionID + ".";
                        }
                    }

                    ChannelQueue newBuffer = Buffer.Clone();
                    newBuffer.Enqueue(values);

                    globalEnv = globalEnv.GetChannelClone();
                    globalEnv.Channels[ChannelName] = newBuffer;

                    if (eventID != eventName)
                    {
                        list.Add(new Configuration(Process, eventID, eventName, globalEnv, false));
                    }
                    else
                    {
                        list.Add(new Configuration(Process, eventID, null, globalEnv, false));
                    }
                }
            }

           // return list;
        }

        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
            //List<ConfigurationWithChannelData> list = new List<ConfigurationWithChannelData>(1);

            if (Specification.SyncrhonousChannelNames.Contains(ChannelName))
            {               
                string eventName = ChannelName;
                string eventID = ChannelName;

                Expression[] newExpressionList = new Expression[ExpressionList.Length];

                for (int i = 0; i < ExpressionList.Length; i++)
                {
                    newExpressionList[i] = EvaluatorDenotational.Evaluate(ExpressionList[i], GlobalEnv);
                    eventName += "." + newExpressionList[i];
                    eventID += "." + newExpressionList[i].ExpressionID;
                }

                if(eventID != eventName)
                {
                    list.Add(new ConfigurationWithChannelData(Process, eventID, eventName, GlobalEnv, false, ChannelName, newExpressionList));
                }
                else
                {
                    list.Add(new ConfigurationWithChannelData(Process, eventID, null, GlobalEnv, false, ChannelName, newExpressionList));
                }
            }

            //return list;
        }


        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {         
            return Process.GetAlphabets(visitedDefinitionRefs);
        }

        public override List<string> GetGlobalVariables()
        {
            List<string> toReturn = Process.GetGlobalVariables();

            foreach (Expression expression in ExpressionList)
            {
                Common.Classes.Ultility.Ultility.AddList(toReturn, expression.GetVars());
            }

            return toReturn;
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
            return ChannelName + "!" + Common.Classes.Ultility.Ultility.PPStringListDot(ExpressionList).TrimStart('.') + "->" + Process;
        }


        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression[] newExpression = new Expression[ExpressionList.Length];
            for (int i = 0; i < ExpressionList.Length; i++)
            {
                newExpression[i] = ExpressionList[i].ClearConstant(constMapping);
            }
            
            return new ChannelOutput(ChannelName, newExpression, Process.ClearConstant(constMapping));
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