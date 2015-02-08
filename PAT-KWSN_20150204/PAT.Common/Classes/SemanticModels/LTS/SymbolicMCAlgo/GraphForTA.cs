using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using PAT.Common.Classes.CUDDLib;


namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class Model
    {
        public static int numLoopDiscrete = 0;
        /// <summary>
        /// Forward search, check destination is reachable from source
        /// Store all reachable states and check fix point
        /// At step t, find all reachable states after t time units
        /// Fix point: rechable(0, t) == rechable(0, t + 1)
        /// (S x Tick x Trans*)*
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="discreteTransitions"></param>
        /// <returns></returns>
        public bool PathForward1(CUDDNode source, CUDDNode destination, List<CUDDNode> discreteTransitions, List<CUDDNode> tickTransitions, bool simulationUsed, CUDDNode simulationRel)
        {
            numLoopDiscrete = 0;
            //
            bool reachable = false;

            CUDD.Ref(source);
            CUDDNode allReachableFromInit = (simulationUsed) ? SuccessorsStartWithSimulation(source, discreteTransitions, simulationRel) :
                                                                    SuccessorsStart(source, discreteTransitions);

            CUDD.Ref(allReachableFromInit);
            CUDDNode currentReachableFromInit = allReachableFromInit;

            CUDDNode commonNode = CUDD.Constant(0);

            int numberOfLoop = 0;
            do
            {
                numberOfLoop++;
                Debug.Write(numberOfLoop + " ");

                currentReachableFromInit = Successors(currentReachableFromInit, tickTransitions);
                currentReachableFromInit = (simulationUsed)? SuccessorsStartWithSimulation(currentReachableFromInit, discreteTransitions, simulationRel):
                                                                    SuccessorsStart(currentReachableFromInit, discreteTransitions);

                //Check 2 directions have intersection
                CUDD.Ref(destination, currentReachableFromInit);
                CUDD.Deref(commonNode);
                commonNode = CUDD.Function.And(destination, currentReachableFromInit);

                if (!commonNode.Equals(CUDD.ZERO))
                {
                    reachable = true;
                    break;
                }

                //find fixpoint
                CUDD.Ref(currentReachableFromInit, allReachableFromInit);
                CUDDNode allReachabeFromInitTemp = CUDD.Function.Or(currentReachableFromInit, allReachableFromInit);

                if (allReachabeFromInitTemp.Equals(allReachableFromInit))
                {
                    //reachable = false;
                    CUDD.Deref(allReachabeFromInitTemp);
                    break;
                }
                else
                {
                    currentReachableFromInit = CUDD.Function.Different(currentReachableFromInit, allReachableFromInit);
                    allReachableFromInit = allReachabeFromInitTemp;
                }
            } while (true);

            Debug.WriteLine("\nTA Path Forward 1: " + (numberOfLoop + 1) + " loops.");
            Debug.WriteLine("Discrete loops: " + numLoopDiscrete);
            Debug.WriteLine("Avarage Discrete loops: " + ((double) numLoopDiscrete/(numberOfLoop + 1)));

            CUDD.Deref(allReachableFromInit, currentReachableFromInit, commonNode);

            if(simulationUsed)
            {
                CUDD.Deref(simulationRel);
            }

            //
            return reachable;
        }

        /// <summary>
        /// Forward search, check destination is reachable from source
        /// NOT store all reachable states, use some special condition to terminate
        /// At step t, find all reachable states after t time units
        /// Fix point when rechable(t1 + a, t1 + a) is a subset of rechable(t1, t1)
        /// Rabbit algorithm (S x Tick x Trans*)*
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="discreteTransitions"></param>
        /// <returns></returns>
        public bool PathForward2(CUDDNode source, CUDDNode destination, List<CUDDNode> discreteTransitions, List<CUDDNode> tickTransitions)
        {
            //
            bool reachable = false;

            CUDD.Ref(source);
            CUDDNode currentReachableFromInit = SuccessorsStart(source, discreteTransitions);
            CUDDNode previousReachableFromInit = CUDD.Constant(0);

            int s, p;
            s = p = 0;
            CUDDNode commonNode = CUDD.Constant(0);

            int numberOfLoop = 0;
            while (!CUDD.IsSubSet(previousReachableFromInit, currentReachableFromInit))
            {
                numberOfLoop++;
                Debug.Write(numberOfLoop + " ");

                if(p <= s/2)
                {
                    p = s;
                    CUDD.Deref(previousReachableFromInit);
                    CUDD.Ref(currentReachableFromInit);
                    previousReachableFromInit = currentReachableFromInit;
                }
                currentReachableFromInit = Successors(currentReachableFromInit, tickTransitions);
                currentReachableFromInit = SuccessorsStart(currentReachableFromInit, discreteTransitions);

                s++;

                //Check 2 directions have intersection
                CUDD.Ref(destination, currentReachableFromInit);
                CUDD.Deref(commonNode);
                commonNode = CUDD.Function.And(destination, currentReachableFromInit);

                if (!commonNode.Equals(CUDD.ZERO))
                {
                    reachable = true;
                    break;
                }
            }

            Debug.WriteLine("\nTA Path Forward 2: " + numberOfLoop + " loops.");

            CUDD.Deref(currentReachableFromInit, previousReachableFromInit, commonNode);

            //
            return reachable;
        }

        /// <summary>
        /// Forward search, check destination is reachable from source
        /// (S x (Tick + Trans)*)
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="transitions"></param>
        /// <returns></returns>
        public bool PathForward3(CUDDNode source, CUDDNode destination, List<CUDDNode> transitions)
        {
            //
            bool reachable = false;

            CUDDNode allReachableFromInit, currentReachableFromInit;
            CUDD.Ref(source, source);
            allReachableFromInit = currentReachableFromInit = source;

            CUDDNode commonNode = CUDD.Constant(0);

            int numberOfLoop = 0;
            do
            {
                numberOfLoop++;
                Debug.Write(numberOfLoop + " ");

                currentReachableFromInit = Successors(currentReachableFromInit, transitions);

                //Check 2 directions have intersection
                CUDD.Ref(destination, currentReachableFromInit);
                CUDD.Deref(commonNode);
                commonNode = CUDD.Function.And(destination, currentReachableFromInit);

                if (!commonNode.Equals(CUDD.ZERO))
                {
                    reachable = true;
                    break;
                }

                //find fixpoint
                CUDD.Ref(currentReachableFromInit, allReachableFromInit);
                CUDDNode allReachabeFromInitTemp = CUDD.Function.Or(currentReachableFromInit, allReachableFromInit);

                if (allReachabeFromInitTemp.Equals(allReachableFromInit))
                {
                    //reachable = false;
                    CUDD.Deref(allReachabeFromInitTemp);
                    break;
                }
                else
                {
                    currentReachableFromInit = CUDD.Function.Different(currentReachableFromInit, allReachableFromInit);
                    allReachableFromInit = allReachabeFromInitTemp;
                }
            } while (true);

            Debug.WriteLine("\nTA Path Forward 3: " + numberOfLoop + " loops.");

            CUDD.Deref(allReachableFromInit, currentReachableFromInit, commonNode);

            //
            return reachable;
        }

        /// <summary>
        /// Algorithm similar to zone, with simulation
        /// (S x Tick* x Trans)*
        /// [ REFS: '', DEREFS:]
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="discreteTransitions"></param>
        /// <param name="tickTransitions"></param>
        /// <returns></returns>
        public bool PathForward4(CUDDNode source, CUDDNode destination, List<CUDDNode> discreteTransitions, List<CUDDNode> tickTransitions, bool simulationUsed, CUDDNode simulationRel)
        {
            numLoopDiscrete = 0;
            //
            bool reachable = false;

            CUDD.Ref(source);
            CUDDNode allReachableFromInit = source;

            CUDD.Ref(allReachableFromInit);
            CUDDNode currentReachableFromInit = allReachableFromInit;

            CUDDNode commonNode = CUDD.Constant(0);

            int numberOfLoop = 0;
            do
            {
                numberOfLoop++;
                Debug.Write(numberOfLoop + " ");

                currentReachableFromInit = (simulationUsed) ? SuccessorsStartWithSimulation(currentReachableFromInit, tickTransitions, simulationRel) :
                                                                    SuccessorsStart(currentReachableFromInit, tickTransitions);    
                
                currentReachableFromInit = Successors(currentReachableFromInit, discreteTransitions);

                if (simulationUsed)
                {
                    currentReachableFromInit = GetSimulatedStates(currentReachableFromInit, simulationRel);
                }

                //Check 2 directions have intersection
                CUDD.Ref(destination, currentReachableFromInit);
                CUDD.Deref(commonNode);
                commonNode = CUDD.Function.And(destination, currentReachableFromInit);

                if (!commonNode.Equals(CUDD.ZERO))
                {
                    reachable = true;
                    break;
                }

                //find fixpoint
                CUDD.Ref(currentReachableFromInit, allReachableFromInit);
                CUDDNode allReachabeFromInitTemp = CUDD.Function.Or(currentReachableFromInit, allReachableFromInit);

                if (allReachabeFromInitTemp.Equals(allReachableFromInit))
                {
                    //reachable = false;
                    CUDD.Deref(allReachabeFromInitTemp);
                    break;
                }
                else
                {
                    currentReachableFromInit = CUDD.Function.Different(currentReachableFromInit, allReachableFromInit);
                    allReachableFromInit = allReachabeFromInitTemp;
                }
            } while (true);

            Debug.WriteLine("\nTA Path Forward 4: " + (numberOfLoop + 1) + " loops.");
            Debug.WriteLine("Timed loops: " + numLoopDiscrete);
            Debug.WriteLine("Avarage Timed loops: " + ((double)numLoopDiscrete / (numberOfLoop + 1)));

            CUDD.Deref(allReachableFromInit, currentReachableFromInit, commonNode);

            if(simulationUsed)
            {
                CUDD.Deref(simulationRel);
            }
            //
            return reachable;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: states]
        /// </summary>
        /// <param name="states"></param>
        /// <param name="simulationRel"></param>
        /// <returns></returns>
        private CUDDNode GetSimulatedStates(CUDDNode states, CUDDNode simulationRel)
        {
            CUDD.Ref(simulationRel);
            return CUDD.Abstract.ThereExists(CUDD.Function.And(SwapRowColVars(states), simulationRel), AllColVars);
        }

        /// <summary>
        /// return all states reachable from start and Start
        /// P ◦ R*
        /// [ REFS: 'result', DEREFS:start]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="transition"></param>
        /// <returns></returns>
        public CUDDNode SuccessorsStartWithSimulation(CUDDNode start, List<CUDDNode> transition, CUDDNode simulationRel)
        {
            CUDDNode allReachableStates = GetSimulatedStates(start, simulationRel);

            CUDD.Ref(allReachableStates);
            CUDDNode currentStep = allReachableStates;

            int numberOfLoop = 0;

            while (true)
            {
                numberOfLoop++;
                Debug.Write(numberOfLoop + " ");

                currentStep = Successors(currentStep, transition);

                currentStep = GetSimulatedStates(currentStep, simulationRel);

                CUDD.Ref(allReachableStates, currentStep);
                CUDDNode allReachableTemp = CUDD.Function.Or(allReachableStates, currentStep);

                if (allReachableTemp.Equals(allReachableStates))
                {
                    CUDD.Deref(currentStep, allReachableTemp);

                    numLoopDiscrete += numberOfLoop;
                    Debug.WriteLine("\nSuccessor*: " + numberOfLoop + " loops.");

                    return allReachableStates;
                }
                else
                {
                    currentStep = CUDD.Function.Different(currentStep, allReachableStates);
                    allReachableStates = allReachableTemp;
                }
            }
        }
    }
}