using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.LTS
{
    public class ChannelInputEvent : Event
    {
        public ChannelInputEvent(string name, Expression ex) : base(name)
        {
            if(ex == null)
            {
                ExpressionList = new Expression[0];
            }
            else
            {
                ExpressionList = new Expression[] {ex};
            }
        }

        public override string ToString()
        {
            if (ExpressionList.Length > 0)
            {
                return BaseName + "[" + ExpressionList[0].ToString() + "]?";
            }
            else
            {
                return BaseName + "?";
            }
        }

        public override string GetEventID(Valuation global)
        {
            if (ExpressionList.Length > 0)
            {
                ExpressionValue v = EvaluatorDenotational.Evaluate(ExpressionList[0], global);
                return BaseName + "[" + v.ExpressionID + "]";
            }
            else
            {
                return BaseName;
            }
        }

        public override Event ClearConstant(Dictionary<string, Expression> constMapping)
        {
            if (ExpressionList.Length > 0)
            {
                return new ChannelInputEvent(BaseName, ExpressionList[0].ClearConstant(constMapping));
            }
            else
            {
                return this;
            }
        }
    }

    public class ChannelOutputEvent : Event
    {
        public ChannelOutputEvent(string name, Expression ex): base(name)
        {
            if (ex == null)
            {
                ExpressionList = new Expression[0];
            }
            else
            {
                ExpressionList = new Expression[] { ex };
            }
        }

        public override string GetEventID(Valuation global)
        {
            if (ExpressionList.Length > 0)
            {
                ExpressionValue v = EvaluatorDenotational.Evaluate(ExpressionList[0], global);

                return BaseName + "[" + v.ExpressionID + "]";
            }
            else
            {
                return BaseName;
            }
        }

        public override string ToString()
        {
            if (ExpressionList.Length > 0)
            {
                return BaseName + "[" + ExpressionList[0].ToString() + "]!";  
            }
            else
            {
                return BaseName + "!";
            }
        }

        public override Event ClearConstant(Dictionary<string, Expression> constMapping)
        {
            if (ExpressionList.Length > 0)
            {
                return new ChannelOutputEvent(BaseName, ExpressionList[0].ClearConstant(constMapping));
            }
            else
            {
                return this;
            }            
        }
    }
}
