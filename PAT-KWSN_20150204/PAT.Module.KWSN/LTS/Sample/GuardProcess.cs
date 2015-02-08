using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
    public sealed class GuardProcess : Process
    {
        public Process Process;
        public Expression Condition;

        public GuardProcess(Process process, Expression cond)
        {
            Process = process;
            Condition = cond;
            ProcessID = DataStore.DataManager.InitializeProcessID("[" + cond.ExpressionID + "]" + Process.ProcessID);
        }

        public override List<string> GetGlobalVariables()
        {
            List<string> Variables = Process.GetGlobalVariables();
            Common.Classes.Ultility.Ultility.AddList(Variables, Condition.GetVars());
            return Variables;
        }

        public override List<string> GetChannels()
        {
            return Process.GetChannels();
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            ExpressionValue v = EvaluatorDenotational.Evaluate(Condition, GlobalEnv);

            if ((v as BoolConstant).Value)
            {
                Process.MoveOneStep(GlobalEnv, list);
            }

            //return new List<Configuration>(0);
        }

        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
            ExpressionValue v = EvaluatorDenotational.Evaluate(Condition, GlobalEnv);

            if ((v as BoolConstant).Value)
            {
                 Process.SyncOutput(GlobalEnv, list);                
            }

            //return new List<ConfigurationWithChannelData>(0);
        }

        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
            ExpressionValue v = EvaluatorDenotational.Evaluate(Condition, eStep.GlobalEnv);

            if ((v as BoolConstant).Value)
            {
                Process.SyncInput(eStep, list);
            }

            //return new List<Configuration>(0);
        }

        public override string ToString()
        {
            return "([" + Condition + "]" + Process.ToString() + ")";
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            return Process.GetAlphabets(visitedDefinitionRefs);
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression newCon = Condition.ClearConstant(constMapping);
            return new GuardProcess(Process.ClearConstant(constMapping), newCon);
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