using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of Sequence process
        /// </summary>
        /// <param name="P1">AutomataBDD of the first process</param>
        /// <param name="P2">AutomataBDD of the second process</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Sequence(AutomataBDD P1, AutomataBDD P2, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            List<string> varNames = AutomataBDD.SequenceSetVariable(P1, P2, model, result);
            SequenceSetInit(varNames[0], P1, P2, result);
            SequenceEncodeTransition(varNames[0], P1, P2, model, result);

            //
            return result;
        }

        /// <summary>
        /// P.var : P1.var ∪ P2.var ∪{isP1Terminate}
        /// </summary>
        public static List<string> SequenceSetVariable(AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            //
            result.variableIndex.AddRange(P1.variableIndex);
            result.variableIndex.AddRange(P2.variableIndex);

            string isP1Terminate = Model.GetNewTempVarName();
            model.AddLocalVar(isP1Terminate, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);

            return new List<string>() { isP1Terminate };
        }

        /// <summary>
        /// P.init: P1.init ∧ !isP1Terminate'
        /// </summary>
        public static void SequenceSetInit(string isP1Terminate, AutomataBDD P1, AutomataBDD P2, AutomataBDD result)
        {
            result.initExpression = Expression.AND(P1.initExpression,
                                        Expression.EQ(new Variable(isP1Terminate), new IntConstant(0)));
        }

        /// <summary>
        /// [ REFS: '', DEREFS: 'P1, P2']
        /// </summary>
        /// <param name="isP1Terminate"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        public static void SequenceEncodeTransition(string isP1Terminate, AutomataBDD P1, AutomataBDD P2, Model model, AutomataBDD result)
        {
            CUDDNode tauEvent = GetTauTransEncoding(model);
            CUDDNode terminateEvent = GetTerminationTransEncoding(model);

            CUDD.Ref(terminateEvent);
            CUDD.Ref(P1.transitionBDD);
            List<CUDDNode> notTerminateTransition = CUDD.Function.And(P1.transitionBDD, CUDD.Function.Not(terminateEvent));

            //CUDD.Ref(terminateEvent);
            //CUDD.Ref(P1.transitionBDD);
            List<CUDDNode> terminateTransition = CUDD.Function.And(P1.transitionBDD, terminateEvent);
            //Convert terminate transition to tau transition
            terminateTransition = CUDD.Abstract.ThereExists(terminateTransition, model.GetAllEventVars());
            terminateTransition = CUDD.Function.And(terminateTransition, tauEvent);

            //1. !isP1Terminate and not terminate transition, channel and !isP1Terminate
            Expression guard = Expression.AND(Expression.EQ(new Variable(isP1Terminate), new IntConstant(0)),
                        new Assignment(isP1Terminate, new IntConstant(0)));

            List<CUDDNode> guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            //
            CUDD.Ref(guardDD);
            result.transitionBDD.AddRange(CUDD.Function.And(guardDD, notTerminateTransition));

            CUDD.Ref(guardDD);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelInTransitionBDD));

            // CUDD.Ref(guardDD);
            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(guardDD, P1.channelOutTransitionBDD));

            //2. (!isP1Terminate ∧ terminate P1.transition ∧ isP1Terminate' and P2.Init')
            guard = Expression.AND(Expression.EQ(new Variable(isP1Terminate), new IntConstant(0)),
                        new Assignment(isP1Terminate, new IntConstant(1)));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = CUDD.Function.And(guardDD, P2.GetInitInColumn(model));
            result.transitionBDD.AddRange(CUDD.Function.And(guardDD, terminateTransition));

            //3. (isP1Terminate ∧ P2.Trans/In/Out ∧ isP1Terminate')
            guard = Expression.AND(Expression.EQ(new Variable(isP1Terminate), new IntConstant(1)),
                        new Assignment(isP1Terminate, new IntConstant(1)));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            //
            CUDD.Ref(guardDD);
            result.transitionBDD.AddRange(CUDD.Function.And(guardDD, P2.transitionBDD));

            CUDD.Ref(guardDD);
            result.channelInTransitionBDD.AddRange(CUDD.Function.And(guardDD, P2.channelInTransitionBDD));

            //CUDD.Ref(guardDD);
            result.channelOutTransitionBDD.AddRange(CUDD.Function.And(guardDD, P2.channelOutTransitionBDD));
        }
    }
}
