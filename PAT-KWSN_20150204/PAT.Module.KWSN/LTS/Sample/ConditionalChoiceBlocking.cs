using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS{
   public sealed class ConditionalChoiceBlocking : Process
    {
        public Process FirstProcess;
        public Expression ConditionalExpression;

        public ConditionalChoiceBlocking(Process firstProcess, Expression conditionExpression)
        {
            FirstProcess = firstProcess;
            ConditionalExpression = conditionExpression;

            ProcessID = DataStore.DataManager.InitializeProcessID(FirstProcess.ProcessID + Constants.CONDITIONAL_CHOICE + conditionExpression.ExpressionID);
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);

            ExpressionValue v = EvaluatorDenotational.Evaluate(ConditionalExpression, GlobalEnv);

            if ((v as BoolConstant).Value)
            {
                list.Add(new Configuration(FirstProcess, Constants.TAU, "[ifb(" + ConditionalExpression + ")]", GlobalEnv, false));                
            }
        }
    
        public override string ToString()
        {
            return "ifb " + ConditionalExpression + " {" + FirstProcess.ToString() + "}";
        }

        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            return FirstProcess.GetAlphabets(visitedDefinitionRefs);
        }

        public override List<string> GetGlobalVariables()
        {
            List<string> Variables = FirstProcess.GetGlobalVariables();
            Common.Classes.Ultility.Ultility.AddList(Variables, ConditionalExpression.GetVars());

            return Variables;
        }

        public override List<string> GetChannels()
        {
            return FirstProcess.GetChannels();
        }

        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Expression newCon = ConditionalExpression.ClearConstant(constMapping);

            Process newFirstProc = FirstProcess.ClearConstant(constMapping);
            
            return new ConditionalChoiceBlocking(newFirstProc, newCon);
        }

        public override bool MustBeAbstracted()
        {
            return FirstProcess.MustBeAbstracted();
        }

        public override bool IsBDDEncodable()
        {
            return FirstProcess.IsBDDEncodable();
        }
    }
}