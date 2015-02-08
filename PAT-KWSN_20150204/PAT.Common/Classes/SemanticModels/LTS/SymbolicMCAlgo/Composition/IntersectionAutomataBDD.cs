using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.CUDDLib;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// General algorithm of the intersection of 2 automata
        /// Follow the algorithm in Linear Temporal Logic Symbolic Model Checking at http://ti.arc.nasa.gov/m/profile/kyrozier/papers/COSREV_62.pdf page 23
        /// [ REFS: 'result', DEREFS:'automata1, automata2' ]
        /// </summary>
        /// <param name="automata1"></param>
        /// <param name="automata2"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD IntersectionGeneralAutomata(AutomataBDD automata1, AutomataBDD automata2, Model model)
        {
            //AddIdleTransAtDeadlockStates(automata1, model);

            AutomataBDD result = new AutomataBDD();

            string newVarName = Model.GetNewTempVarName();

            //Set var
            result.variableIndex.AddRange(automata1.variableIndex);
            result.variableIndex.AddRange(automata2.variableIndex);
            model.AddLocalVar(newVarName, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);

            //Set Init
            Expression initTemp = Expression.AND(automata1.initExpression, automata2.initExpression);
            Expression initValueOfT = Expression.EQ(new Variable(newVarName), new IntConstant(0));

            result.initExpression = Expression.AND(initTemp, initValueOfT);

            //Set Acceptance State
            result.acceptanceExpression = Expression.AND(automata1.acceptanceExpression, initValueOfT);

            //Set Transition
            //(temp = 0 and automata1.accept) or (temp = 1 and buchi.accept)
            Expression guard = Expression.OR(
                                                        Expression.AND(
                                                                                 new PrimitiveApplication(
                                                                                     PrimitiveApplication.EQUAL,
                                                                                     new Variable(newVarName),
                                                                                     new IntConstant(0)),
                                                                                 automata1.acceptanceExpression),
                                                        Expression.AND(
                                                                                 new PrimitiveApplication(
                                                                                     PrimitiveApplication.EQUAL,
                                                                                     new Variable(newVarName),
                                                                                     new IntConstant(1)),
                                                                                 automata2.acceptanceExpression));

            //guard and (temp' = 1 - temp)
            Expression transition1Exp = Expression.AND(guard,
                                                                 new Assignment(newVarName,
                                                                                new PrimitiveApplication(
                                                                                    PrimitiveApplication.MINUS,
                                                                                    new IntConstant(1),
                                                                                    new Variable(newVarName))));
            List<CUDDNode> transition1 = transition1Exp.TranslateBoolExpToBDD(model).GuardDDs;

            //!guard and (temp' = temp)
            Expression transition2Exp = Expression.AND(
                                                                 Expression.NOT(guard),
                                                                 new Assignment(newVarName, new Variable(newVarName)));

            List<CUDDNode> transition2 = transition2Exp.TranslateBoolExpToBDD(model).GuardDDs;

            //transition must happen at both automata1 + negation LTL
            List<CUDDNode> bothTransition = CUDD.Function.And(automata1.transitionBDD, automata2.transitionBDD);

            CUDD.Ref(bothTransition);
            transition1 = CUDD.Function.And(transition1, bothTransition);
            result.transitionBDD.AddRange(transition1);

            transition2 = CUDD.Function.And(transition2, bothTransition);
            result.transitionBDD.AddRange(transition2);

            //
            CUDD.Deref(automata1.channelInTransitionBDD, automata1.channelOutTransitionBDD, automata2.channelInTransitionBDD, automata2.channelOutTransitionBDD);

            return result;
        }

        /// <summary>
        /// Return the intersection of model and negation of LTL
        /// Note that all of the state of model are the accepting state
        /// [ REFS: 'result', DEREFS:'automata1, automata2' ]
        /// </summary>
        /// <param name="system"></param>
        /// <param name="negationLTL"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Intersection(AutomataBDD system, AutomataBDD negationLTL, Model model)
        {
            //AddIdleTransAtDeadlockStates(system, model);

            AutomataBDD result = new AutomataBDD();

            //Set var
            result.variableIndex.AddRange(system.variableIndex);
            result.variableIndex.AddRange(negationLTL.variableIndex);
            
            //Set Init
            result.initExpression = Expression.AND(system.initExpression, negationLTL.initExpression);

            //Set Acceptance State
            result.acceptanceExpression = negationLTL.acceptanceExpression;

            //Set Transition
            //transition must happen at both automata1 + negation LTL
            result.transitionBDD = CUDD.Function.And(system.transitionBDD, negationLTL.transitionBDD);

            //
            CUDD.Deref(system.channelInTransitionBDD, system.channelOutTransitionBDD, negationLTL.channelInTransitionBDD, negationLTL.channelOutTransitionBDD);

            return result;
        }

        ///// <summary>
        ///// Add idle transition loop at deadlock states
        ///// </summary>
        ///// <param name="system"></param>
        ///// <param name="model"></param>
        //private static void AddIdleTransAtDeadlockStates(AutomataBDD system, Model model)
        //{
        //    CUDD.Ref(system.transitionBDD);
        //    CUDDNode deadlockStates = AssertionDeadLock.GetDeadlockDD(system.transitionBDD, model);

        //    List<CUDDNode> idleTrans = (new Assignment(Model.EVENT_NAME, new IntConstant(Model.TEMP_TRANSITION_INDEX))).TranslateBoolExpToBDD(model).GuardDDs;
        //    List<int> unchangedVars = new List<int>(model.GlobalVarIndex);
        //    unchangedVars.AddRange(system.variableIndex);
        //    idleTrans = model.AddVarUnchangedConstraint(idleTrans, unchangedVars);

        //    idleTrans = CUDD.Function.And(idleTrans, deadlockStates);

        //    system.transitionBDD.AddRange(idleTrans);
        //}
    }
}
