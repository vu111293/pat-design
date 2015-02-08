using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Collections.Generic;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Event-based transitions
        /// </summary>
        public List<CUDDNode> transitionBDD = new List<CUDDNode>();

        /// <summary>
        /// Sync Channel-in-based transitions
        /// </summary>
        public List<CUDDNode> channelInTransitionBDD = new List<CUDDNode>();

        /// <summary>
        /// Sync Channel-out-based transitions
        /// </summary>
        public List<CUDDNode> channelOutTransitionBDD = new List<CUDDNode>();

        /// <summary>
        /// Tick transition
        /// </summary>
        public List<CUDDNode> Ticks = new List<CUDDNode>();


        public CUDDNode SimulationRel = CUDD.Constant(0);

        /// <summary>
        /// Initial state of the AutomataBDD in expression representation
        /// </summary>
        public Expression initExpression = new BoolConstant(true);

        public Expression acceptanceExpression = new BoolConstant(true);

        /// <summary>
        /// Variables beside global varialbes used in this AutomataBDD including controling state variables and local variables
        /// </summary>
        public List<int> variableIndex = new List<int>();

        /// <summary>
        /// Use this property to store new local varaible name
        /// </summary>
        public string newLocalVarName;

        /// <summary>
        /// Print the transition for debug
        /// </summary>
        public void PrintTransition()
        {
            CUDD.Print.PrintMinterm(this.transitionBDD);
        }


        /// <summary>
        /// Returned the initialization of this process (in the form of update statements)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<CUDDNode> GetInitInColumn(Model model)
        {
            List<CUDDNode> initDD = initExpression.TranslateBoolExpToBDD(model).GuardDDs;
            //Swap variable
            return model.SwapRowColVars(initDD);
        }

        public static CUDDNode GetTauTransEncoding(Model model)
        {
            return CUDD.Function.Or(GetTauTransExpression().TranslateBoolExpToBDD(model).GuardDDs);
        }

        public static Expression GetTauTransExpression()
        {
            return new Assignment(Model.EVENT_NAME, new IntConstant(Model.TAU_EVENT_INDEX));
        }

        public static Expression GetNotTauTransExpression()
        {
            return Expression.NE(new VariablePrime(Model.EVENT_NAME),
                                     new IntConstant(Model.TAU_EVENT_INDEX));
        }

        public static CUDDNode GetTerminationTransEncoding(Model model)
        {
            return CUDD.Function.Or(GetTerminateTransExpression().TranslateBoolExpToBDD(model).GuardDDs);
        }

        public static Expression GetTerminateTransExpression()
        {
            return new Assignment(Model.EVENT_NAME, new IntConstant(Model.TERMINATE_EVENT_INDEX));
        }

        public static Expression GetNotTerminateTransExpression()
        {
            return Expression.NE(new VariablePrime(Model.EVENT_NAME),
                                     new IntConstant(Model.TERMINATE_EVENT_INDEX));
        }
    }
}
