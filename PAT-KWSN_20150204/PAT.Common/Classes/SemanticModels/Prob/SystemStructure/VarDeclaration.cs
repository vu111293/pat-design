using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{

    public class VarDeclaration
    {
        public string Name;
        public Expression Low;
        public Expression High;
        public Expression Init;
        public DeclarationType dataType;

        public VarDeclaration(string name, Expression low, Expression high, Expression init)
        {
            Name = name;
            Low = low;
            High = high;
            Init = init;
        }

        public VarDeclaration(string name, DeclarationType type)
        {
            Name = name;
            dataType = type;

            if (type is DeclarationBool)
            {
                Low = new IntConstant(0);
                High = new IntConstant(1);
                Init = new IntConstant(0);
            }
            else
            {
                DeclarationInt intType = (DeclarationInt) type;
                Low = intType.low;
                High = intType.high;
                Init = intType.low;
            }
        }

        public override string ToString()
        {
            if(dataType is DeclarationBool)
            {
                return Name + " : bool init " + Init;
            }
            else
            {
                return Name + " : [" + Low + ".." + High + "] init " + Init;
            }
        }
    }
}
