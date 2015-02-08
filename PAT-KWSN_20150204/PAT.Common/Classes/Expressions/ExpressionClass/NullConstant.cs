using System;
using System.Reflection.Emit;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public sealed class NullConstant : ExpressionValue
    {

        public NullConstant()
        {            
            expressionID = "null";
        }

       
        public override String ToString()
        {
            return "null";
            
        }

        public override ExpressionValue GetClone()
        {
            return this;
        }
#if DEBUG
        public override void GenerateMSIL(ILGenerator ilGenerator, TypeBuilder typeBuilder)
        {
            ilGenerator.Emit(OpCodes.Ldnull);
        }
#endif
    }
}