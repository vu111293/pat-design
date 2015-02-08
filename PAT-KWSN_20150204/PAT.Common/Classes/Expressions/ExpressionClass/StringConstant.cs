using System;


namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class StringConstant : ExpressionValue
    {
        //public string Value;
        public StringConstant(string v)
        {
            //Value = v;
            expressionID = v;
        }
        public override String ToString()
        {
            return expressionID;
        }

        //public override string GetID()
        //{
        //    return Value;
        //}
    }
}