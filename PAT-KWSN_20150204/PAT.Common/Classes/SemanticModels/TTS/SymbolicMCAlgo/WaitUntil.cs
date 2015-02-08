using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.TTS
{
    public partial class TimeBehaviors
    {
        public static AutomataBDD WaitUntil(AutomataBDD m0, int t, Model model)
        {
            AutomataBDD result = new AutomataBDD();

            List<string> varNames = WaitUntilSetVariable(m0, t, model, result);
            WaitUntilSetInit(varNames[0], varNames[1], m0, result);
            WaitUntilEncodeTransitionChannel(varNames[0], varNames[1], m0, t, model, result);
            WaitUntilEncodeTick(varNames[0], varNames[1], m0, t, model, result);

            //
            return result;
        }

        /// <summary>
        /// P.var : m0 ∪ {state, isTerminate}
        /// </summary>
        private static List<string> WaitUntilSetVariable(AutomataBDD m0, int t, Model model, AutomataBDD result)
        {
            result.variableIndex.AddRange(m0.variableIndex);
            //
            string state = Model.GetNewTempVarName();
            model.AddNewVar(state, 0, t + 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);
            //
            string isTerminate = Model.GetNewTempVarName();
            model.AddNewVar(isTerminate, 0, 1);
            result.variableIndex.Add(model.GetNumberOfVars() - 1);

            return new List<string>() { state, isTerminate };
        }

        /// <summary>
        /// P.init: m0.init ^ state = 0 and not isTerminate
        /// </summary>
        private static void WaitUntilSetInit(string state, string isTerminate, AutomataBDD m0, AutomataBDD result)
        {
            result.initExpression = new PrimitiveApplication(PrimitiveApplication.AND, m0.initExpression, new PrimitiveApplication(PrimitiveApplication.AND,
                                        new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(state), new IntConstant(0)),
                                        new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(isTerminate), new IntConstant(0))));
        }

        private static void WaitUntilEncodeTransitionChannel(string state, string isTerminate, AutomataBDD m0, int t, Model model, AutomataBDD result)
        {
            Expression guard;
            List<CUDDNode> guardDD, transTemp;

            //1. m0.Trans/In/Out and state' = state and [(event' = terminate and isTerminate') or (event' != terminate and isTerminate' = isTerminate)]
            guard = new PrimitiveApplication(PrimitiveApplication.AND, new Assignment(state, new Variable(state)),
                                                new PrimitiveApplication(PrimitiveApplication.OR, new PrimitiveApplication(PrimitiveApplication.AND,
                                                    new Assignment(Model.EVENT_NAME, new IntConstant(Model.TERMINATE_EVENT_INDEX)),
                                                    new Assignment(isTerminate, new IntConstant(1))), new PrimitiveApplication(PrimitiveApplication.AND,
                                                    new PrimitiveApplication(PrimitiveApplication.NOT, new Assignment(Model.EVENT_NAME, new IntConstant(Model.TERMINATE_EVENT_INDEX))),
                                                    new Assignment(isTerminate, new Variable(isTerminate)))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.transitionBDD, guardDD);
            result.transitionBDD.AddRange(transTemp);

            //
            CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.channelInTransitionBDD, guardDD);
            result.channelInTransitionBDD.AddRange(transTemp);

            //
            //CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.channelOutTransitionBDD, guardDD);
            result.channelOutTransitionBDD.AddRange(transTemp);

            //2. isTerminate and state = t and event' = terminate and isTermniate' & state = t + 1
            guard = new PrimitiveApplication(PrimitiveApplication.AND, new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(isTerminate), new IntConstant(1)),
                        new PrimitiveApplication(PrimitiveApplication.AND, new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(state), new IntConstant(t)),
                            new PrimitiveApplication(PrimitiveApplication.AND, new Assignment(Model.EVENT_NAME, new IntConstant(Model.TERMINATE_EVENT_INDEX)),
                                new PrimitiveApplication(PrimitiveApplication.AND, new Assignment(isTerminate, new IntConstant(1)),
                                    new Assignment(state, new IntConstant(t + 1))))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;

            CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.transitionBDD, guardDD);
            result.transitionBDD.AddRange(transTemp);

            //
            CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.channelInTransitionBDD, guardDD);
            result.channelInTransitionBDD.AddRange(transTemp);

            //
            //CUDD.Ref(guardDD);
            transTemp = CUDD.Function.And(m0.channelOutTransitionBDD, guardDD);
            result.channelOutTransitionBDD.AddRange(transTemp);
        }

        private static void WaitUntilEncodeTick(string state, string isTerminate, AutomataBDD m0, int t, Model model, AutomataBDD result)
        {
            Expression guard;
            List<CUDDNode> guardDD;

            //1. m0.Tick and [(state < t and state' = state + 1) or (state = t and state' = t)] and isTerminate' = isTerminate
            guard = new PrimitiveApplication(PrimitiveApplication.AND, new PrimitiveApplication(PrimitiveApplication.OR, new PrimitiveApplication(PrimitiveApplication.AND,
                        new PrimitiveApplication(PrimitiveApplication.LESS, new Variable(state), new IntConstant(t)),
                        new Assignment(state, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(state), new IntConstant(1)))), new PrimitiveApplication(PrimitiveApplication.AND,
                        new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(state), new IntConstant(t)),
                        new Assignment(state, new Variable(state)))),
                        new Assignment(isTerminate, new Variable(isTerminate)));

            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = CUDD.Function.And(m0.Ticks, guardDD);
            result.Ticks.AddRange(guardDD);

            //2. isTerminate and state < t and event' = tick and isTerminate' and state' = state + 1
            guard = new PrimitiveApplication(PrimitiveApplication.AND, new PrimitiveApplication(PrimitiveApplication.EQUAL, new Variable(isTerminate), new IntConstant(1)),
                        new PrimitiveApplication(PrimitiveApplication.AND, new PrimitiveApplication(PrimitiveApplication.LESS, new Variable(state), new IntConstant(t)),
                            new PrimitiveApplication(PrimitiveApplication.AND, new Assignment(Model.EVENT_NAME, new IntConstant(Model.TOCK_EVENT_INDEX)),
                                new PrimitiveApplication(PrimitiveApplication.AND, new Assignment(isTerminate, new IntConstant(1)),
                                    new Assignment(state, new PrimitiveApplication(PrimitiveApplication.PLUS, new Variable(state), new IntConstant(1)))))));
            guardDD = guard.TranslateBoolExpToBDD(model).GuardDDs;
            guardDD = model.AddVarUnchangedConstraint(guardDD, model.GlobalVarIndex);
            guardDD = model.AddVarUnchangedConstraint(guardDD, result.variableIndex);
            result.Ticks.AddRange(guardDD);
        }
    }
}
