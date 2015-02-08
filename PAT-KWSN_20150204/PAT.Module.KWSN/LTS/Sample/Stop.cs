using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.KWSN.LTS{
    public sealed class Stop : Process
    {
        public Stop()
        {
            ProcessID = Constants.STOP;
        }

        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            System.Diagnostics.Debug.Assert(list.Count == 0);
        }

        public override string ToString()
        {
            return "Stop";
        }
       
        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            return this;
        }
    }
}