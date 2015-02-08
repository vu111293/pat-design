using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;

namespace PAT.Common.Classes.SemanticModels.Prob.Encoding
{
    public class ComponentDDs
    {
        public CUDDNode guards;		// bdd for guards
        public CUDDNode trans;		// mtbdd for transitions

        /// <summary>
        /// Reward for each reward structure
        /// </summary>
        public List<CUDDNode> rewards = new List<CUDDNode>();

        /// <summary>
        /// choice bits are from [min...max-1]
        /// </summary>
        public int min;

        /// <summary>
        /// choice bits are from [min...max-1]
        /// </summary>
        public int max; 			

        public void Deref()
        {
            CUDD.Deref(guards, trans);
            CUDD.Deref(rewards);
        }
    }

    // data structure used to store mtbdds and related info
    // for some part of the system definition

    public class SystemDDs
    {
        public ComponentDDs ind;		// for independent bit (i.e. tau action)
        public List<ComponentDDs> synchs;	// for each synchronising action
        public CUDDNode id;	 			// mtbdd for identity matrix
        public HashSet<String> allSynchs;		// set of all synchs used (syntactic)
        public SystemDDs()
        {
            synchs = new List<ComponentDDs>();
            allSynchs = new HashSet<String>();
        }
        public void Deref()
        {
            CUDD.Deref(ind.guards, ind.trans);
            CUDD.Deref(ind.rewards);

            foreach (var componentDDse in synchs)
            {
                CUDD.Deref(componentDDse.guards, componentDDse.trans);
                CUDD.Deref(componentDDse.rewards);
            }
            CUDD.Deref(id);
        }
    }

    public partial class ModulesToBDD
    {
        private void EncodeSystemDef(SystemDef system)
        {
            List<int> currentAvailStartBits = new List<int>();
            for(int i = 0; i < synchs.Count; i++)
            {
                currentAvailStartBits.Add(0);
            }

            SystemDDs sysDDs = EncodeSystemDefRec(system, currentAvailStartBits);


            // for the nondeterministic case, add extra mtbdd variables to encode nondeterminism
            if (modules.modelType == ModelType.MDP)
            {
                int max = sysDDs.ind.max;
                for (int i = 0; i < synchs.Count; i++)
                {
                    if (sysDDs.synchs[i].max > max)
                    {
                        max = sysDDs.synchs[i].max;
                    }
                }

                //update max
                sysDDs.ind.max = max;
                for (int i = 0; i < synchs.Count; i++ )
                {
                    sysDDs.synchs[i].max = max;
                }

                //TODO Different from Prism
                // now add in new mtbdd variables to distinguish between actions
                //index 0 for empty label
                CUDDNode tmp = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), allSynchVars, 0, 1);
                sysDDs.ind.trans = CUDD.Function.Times(tmp, sysDDs.ind.trans);

                // synchronous bits
                //1... for other labels
                for (int i = 0; i < synchs.Count; i++)
                {
                    tmp = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), allSynchVars, i + 1, 1);
                    sysDDs.synchs[i].trans = CUDD.Function.Times(tmp, sysDDs.synchs[i].trans);
                }
            }

            ComputeRewards(sysDDs);

            // now, for all model types, transition matrix can be built by summing over all actions
            // also build transition rewards at the same time
            CUDD.Ref(sysDDs.ind.trans);
            trans = sysDDs.ind.trans;

            int numRewardStructs = modules.rewardStructs.Count;
            for (int j = 0; j < numRewardStructs; j++)
            {
                CUDD.Ref(sysDDs.ind.rewards[j]);
                transRewards.Add(sysDDs.ind.rewards[j]);
            }

            for (int i = 0; i < synchs.Count; i++)
            {
                CUDD.Ref(sysDDs.synchs[i].trans);
                trans = CUDD.Function.Plus(trans, sysDDs.synchs[i].trans);

                for (int j = 0; j < numRewardStructs; j++)
                {
                    CUDD.Ref(sysDDs.synchs[i].rewards[j]);
                    transRewards[j] = CUDD.Function.Plus(transRewards[j], sysDDs.synchs[i].rewards[j]);
                }
            }

            // For D/CTMCs, final rewards are scaled by dividing by total prob/rate for each transition
            // (when individual transition rewards are computed, they are multiplied by individual probs/rates).
            // Need to do this (for D/CTMCs) because transition prob/rate can be the sum of values from
            // several different actions; this gives us the "expected" reward for each transition.
            // (Note, for MDPs, nondeterministic choices are always kept separate so this never occurs.)
            if (modules.modelType != ModelType.MDP)
            {
                int numberOfRewardStructs = modules.rewardStructs.Count;
                for (int j = 0; j < numberOfRewardStructs; j++)
                {
                    CUDD.Ref(trans);
                    transRewards[j] = CUDD.Function.Divide(transRewards[j], trans);
                }
            }

            //
            sysDDs.Deref();
        }

        private SystemDDs EncodeSystemDefRec(SystemDef system, List<int> currentAvailStartBits)
        {
            if(system is SingleModuleSystem)
            {
                return EncodeSingleModule(system as SingleModuleSystem, currentAvailStartBits);
            }
            else if(system is InterleaveSystem)
            {
                return EncodeInterelave(system as InterleaveSystem, currentAvailStartBits);
            }
            else if(system is ParallelSystem)
            {
                return EncodeParallel(system as ParallelSystem, currentAvailStartBits);
            }
            else if(system is FullParallelSystem)
            {
                return EncodeFullParallelSystem(system as FullParallelSystem, currentAvailStartBits);
            }
            else
            {
                throw new Exception("Invalide system!");
            }
        }


        private SystemDDs EncodeSingleModule(SingleModuleSystem system, List<int> currentAvailStartBits)
        {
            int moduleIndex = modules.GetModuleIndex(system.moduleName);
            Module module = modules.GetModule(moduleIndex);

            SystemDDs result = new SystemDDs();
            //encode not sync command
            result.ind = EncodeModule(moduleIndex, module, string.Empty, 0);

            // build mtbdd for each synchronising action
            for (int i = 0; i < synchs.Count; i++)
            {
                result.synchs.Add(EncodeModule(moduleIndex, module, synchs.ElementAt(i), currentAvailStartBits[i]));
            }

            // store identity matrix
            CUDD.Ref(moduleIdentities[moduleIndex]);
            result.id = moduleIdentities[moduleIndex];

            // store synchs used
            result.allSynchs.UnionWith(module.GetAllSynchs());

            return result;
        }

        private SystemDDs EncodeInterelave(InterleaveSystem system, List<int> currentAvailStartBits)
        {
            SystemDDs sysDDs1, sysDDs2, sysDDs;
            int i, j;

            // construct mtbdds for first operand
            sysDDs = EncodeSystemDefRec(system.GetSystem(0), currentAvailStartBits);

            // loop through all other operands in the parallel operator
            for (i = 1; i < system.NumberSystem; i++)
            {
                // construct mtbdds for next operand
                sysDDs2 = EncodeSystemDefRec(system.GetSystem(i), currentAvailStartBits);

                // move sysDDs (operands composed so far) into sysDDs1
                sysDDs1 = sysDDs;
                // we are going to combine sysDDs1 and sysDDs2 and put the result into sysDDs
                sysDDs = new SystemDDs();

                // combine mtbdds for independent bit
                sysDDs.ind = CombineNonSynchronising(sysDDs1.ind, sysDDs2.ind, sysDDs1.id, sysDDs2.id);

                // combine mtbdds for each synchronising action
                for (j = 0; j < synchs.Count; j++)
                {
                    sysDDs.synchs.Add(CombineNonSynchronising(sysDDs1.synchs[j], sysDDs2.synchs[j], sysDDs1.id, sysDDs2.id));
                }

                // compute identity
                sysDDs.id = CUDD.Function.Times(sysDDs1.id, sysDDs2.id);

                // combine lists of synchs
                sysDDs.allSynchs.UnionWith(sysDDs1.allSynchs);
                sysDDs.allSynchs.UnionWith(sysDDs2.allSynchs);
            }

            return sysDDs;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'compDDs1, compDDs2' ]
        /// </summary>
        /// <param name="compDDs1"></param>
        /// <param name="compDDs2"></param>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        private ComponentDDs CombineNonSynchronising(ComponentDDs compDDs1, ComponentDDs compDDs2, CUDDNode id1, CUDDNode id2)
        {
            CUDD.Ref(id1, id2);
            compDDs1.trans = CUDD.Function.Times(compDDs1.trans, id2);
            compDDs2.trans = CUDD.Function.Times(compDDs2.trans, id1);

            return CombineComponentDDs(compDDs1, compDDs2);
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'compDDs1, compDDs2' ]
        /// </summary>
        /// <param name="compDDs1"></param>
        /// <param name="compDDs2"></param>
        /// <returns></returns>
        private ComponentDDs CombineComponentDDs(ComponentDDs compDDs1, ComponentDDs compDDs2)
        {
            ComponentDDs compDDs = new ComponentDDs();

            // if no nondeterminism - just add
            if (modules.modelType != ModelType.MDP)
            {
                compDDs.guards = CUDD.Function.Or(compDDs1.guards, compDDs2.guards);
                compDDs.trans = CUDD.Function.Plus(compDDs1.trans, compDDs2.trans);

                compDDs.min = 0;
                compDDs.max = 0;
            }
            // if there's nondeterminism, but one part is empty, it's also easy
            else if (compDDs1.trans.Equals(CUDD.ZERO))
            {
                compDDs1.Deref();
                compDDs.guards = compDDs2.guards;
                compDDs.trans = compDDs2.trans;

                compDDs.min = compDDs2.min;
                compDDs.max = compDDs2.max;
            }
            else if (compDDs2.trans.Equals(CUDD.ZERO))
            {
                compDDs2.Deref();

                compDDs.guards = compDDs1.guards;
                compDDs.trans = compDDs1.trans;

                compDDs.min = compDDs1.min;
                compDDs.max = compDDs1.max;
            }
            else
            {
                int max = Math.Max(compDDs1.max, compDDs2.max);

                if (max >= choiceVars.Count)
                {
                    throw new Exception("Insufficient BDD variables allocated for nondeterminism - please report this as a bug. Thank you");
                }

                //Add one bit to represent nondeterministic 2 components
                // and then combine
                CUDD.Ref(choiceVars[max]);
                CUDDNode v = choiceVars[max];

                compDDs.guards = CUDD.Function.Or(compDDs1.guards, compDDs2.guards);

                CUDD.Ref(v);
                compDDs.trans = CUDD.Function.ITE(v, compDDs2.trans, compDDs1.trans);
              
                compDDs.min = Math.Min(compDDs1.min,compDDs2.min);
                compDDs.max = max + 1;
            }

            return compDDs;
        }

        private SystemDDs EncodeParallel(ParallelSystem sys, List<int> currentAvailStartBits)
        {
            SystemDDs sysDDs1, sysDDs2, sysDDs;
            List<bool> synchBool = new List<bool>();
            for (int i = 0; i < synchs.Count; i++)
            {
                synchBool[i] = sys.ContainsAction(synchs[i]);
            }

            // construct mtbdds for first operand
            sysDDs1 = EncodeSystemDefRec(sys.system1, currentAvailStartBits);

            List<int> newCurrentAvailStartBits = new List<int>();
            for (int i = 0; i < synchs.Count; i++)
            {
                newCurrentAvailStartBits[i] = synchBool[i] ? sysDDs1.synchs[i].max : currentAvailStartBits[i];
            }

            // construct mtbdds for second operand
            sysDDs2 = EncodeSystemDefRec(sys.system2, newCurrentAvailStartBits);

            // create object to store mtbdds
            sysDDs = new SystemDDs();

            // combine mtbdds for independent bit
            sysDDs.ind = CombineNonSynchronising(sysDDs1.ind, sysDDs2.ind, sysDDs1.id, sysDDs2.id);

            // combine mtbdds for each synchronising action
            for (int i = 0; i < synchs.Count; i++)
            {
                if(synchBool[i])
                {
                    sysDDs.synchs.Add(CombineSynchronising(sysDDs1.synchs[i], sysDDs2.synchs[i]));
                }
                else
                {
                    sysDDs.synchs.Add(CombineNonSynchronising(sysDDs1.synchs[i], sysDDs2.synchs[i], sysDDs1.id, sysDDs2.id));
                }
            }

            // combine mtbdds for identity matrices
            sysDDs.id = CUDD.Function.Times(sysDDs1.id, sysDDs2.id);

            // combine lists of synchs
            sysDDs.allSynchs.UnionWith(sysDDs1.allSynchs);
            sysDDs.allSynchs.UnionWith(sysDDs2.allSynchs);

            return sysDDs;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'compDDs1, compDDs2' ]
        /// </summary>
        /// <param name="compDDs1"></param>
        /// <param name="compDDs2"></param>
        /// <returns></returns>
        private ComponentDDs CombineSynchronising(ComponentDDs compDDs1, ComponentDDs compDDs2)
        {
            ComponentDDs compDDs = new ComponentDDs();

            // combine parts synchronously
            // first guards
            compDDs.guards = CUDD.Function.And(compDDs1.guards, compDDs2.guards);

            // then transitions
            compDDs.trans = CUDD.Function.Times(compDDs1.trans, compDDs2.trans);

            //
            compDDs.min = Math.Min(compDDs1.min, compDDs2.min);
            compDDs.max = Math.Max(compDDs1.max, compDDs2.max);

            return compDDs;
        }

        private SystemDDs EncodeFullParallelSystem(FullParallelSystem sys, List<int> currentAvailStartBits)
        {
            SystemDDs sysDDs1, sysDDs2, sysDDs;
            List<int> newCurrentAvailStartBits = new List<int>();
            int i, j;

            // construct mtbdds for first operand
            sysDDs = EncodeSystemDefRec(sys.GetSystem(0), currentAvailStartBits);

            // loop through all other operands in the parallel operator
            for (i = 1; i < sys.NumberSystem; i++)
            {
                // change min to max for potentially synchronising actions
                // store this in new array - old one may still be used elsewhere
                for (j = 0; j < synchs.Count; j++)
                {
                    int newValue = (sysDDs.allSynchs.Contains(synchs[j])) ? sysDDs.synchs[j].max : currentAvailStartBits[j];
                    newCurrentAvailStartBits.Add(newValue);
                }

                // construct mtbdds for next operand
                sysDDs2 = EncodeSystemDefRec(sys.GetSystem(i), newCurrentAvailStartBits);

                // move sysDDs (operands composed so far) into sysDDs1
                sysDDs1 = sysDDs;
                // we are going to combine sysDDs1 and sysDDs2 and put the result into sysDDs
                sysDDs = new SystemDDs();

                // combine mtbdds for independent bit
                sysDDs.ind = CombineNonSynchronising(sysDDs1.ind, sysDDs2.ind, sysDDs1.id, sysDDs2.id);

                // combine mtbdds for each synchronising action
                for (j = 0; j < synchs.Count; j++)
                {
                    // do asynchronous parallel composition
                    if ((sysDDs1.allSynchs.Contains(synchs.ElementAt(j)) ? 1 : 0) + (sysDDs2.allSynchs.Contains(synchs.ElementAt(j)) ? 1 : 0) == 1)
                    {
                        sysDDs.synchs.Add(CombineNonSynchronising(sysDDs1.synchs[j], sysDDs2.synchs[j], sysDDs1.id, sysDDs2.id));
                    }
                    else
                    {
                        sysDDs.synchs.Add(CombineSynchronising(sysDDs1.synchs[j], sysDDs2.synchs[j]));
                    }
                }

                // compute identity
                sysDDs.id = CUDD.Function.Times(sysDDs1.id, sysDDs2.id);

                // combine lists of synchs
                sysDDs.allSynchs.UnionWith(sysDDs1.allSynchs);
                sysDDs.allSynchs.UnionWith(sysDDs2.allSynchs);
            }

            return sysDDs;
        }

        /// <summary>
        /// Encode all commands whose labels are the same as labelToBeEncoded
        /// </summary>
        /// <param name="moduleIndex"></param>
        /// <param name="module"></param>
        /// <param name="labelToBeEncoded"></param>
        private ComponentDDs EncodeModule(int moduleIndex, Module module, string labelToBeEncoded, int currentAvailStartBit)
        {
            //Store guard, and the final encoding of each command
            List<CUDDNode> guardDDs = new List<CUDDNode>();
            List<CUDDNode> commandDDs = new List<CUDDNode>();

            for (int i = 0; i < module.Commands.Count; i++)
            {
                Command command = module.Commands[i];
                if (command.Synch == labelToBeEncoded)
                {
                    CUDDNode guardDD = EncodeExpression(command.Guard);
                    CUDD.Ref(allRowVarRanges);
                    guardDD = CUDD.Function.And(guardDD, allRowVarRanges);

                    if (guardDD.Equals(CUDD.ZERO))
                    {
                        Debug.WriteLine("Warning: Guard " + command.Guard + " for command " + (i + 1) + " of module \"" + module.Name + "\" is never satisfied.\n");
                        guardDDs.Add(guardDD);
                        commandDDs.Add(CUDD.Constant(0));
                    }
                    else
                    {
                        CUDDNode updateDD = EncodeCommand(moduleIndex, command.Updates, guardDD);

                        guardDDs.Add(guardDD);
                        commandDDs.Add(updateDD);
                    }
                }
                else
                {
                    guardDDs.Add(CUDD.Constant(0));
                    commandDDs.Add(CUDD.Constant(0));
                }
            }

            ComponentDDs result;
            if (modules.modelType == ModelType.DTMC)
            {
                result = CombineProbCommands(moduleIndex, guardDDs, commandDDs);
            }
            else if(modules.modelType == ModelType.MDP)
            {
                result = CombineNondetCommands(guardDDs, commandDDs, currentAvailStartBit);
            }
            else
            {
                throw new Exception("Unknown model type");
            }

            return result;
        }

        /// <summary>
        /// Combine all the encoding of probabilistic commands
        /// [ REFS: 'result', DEREFS: 'guardDDs, commandDDs' ]
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        private ComponentDDs CombineProbCommands(int moduleIndex, List<CUDDNode> guardDDs, List<CUDDNode> commandDDs)
        {
            ComponentDDs result = new ComponentDDs();

            CUDDNode trans = CUDD.Constant(0);
            CUDDNode cover = CUDD.Constant(0);

            for (int i = 0; i < guardDDs.Count; i++)
            {
                if (guardDDs[i].Equals(CUDD.ZERO))
                {
                    continue;
                }

                //Check whether the guard is unsatisfiable
                CUDD.Ref(guardDDs[i], cover);
                CUDDNode temp = CUDD.Function.And(guardDDs[i], cover);

                if (!temp.Equals(CUDD.ZERO))
                {
                    Debug.WriteLine("Warning: Guard for command " + (i + 1) + " of module \"" + modules.AllModules[moduleIndex].Name + "\" overlaps with previous commands.");

                    CUDD.Deref(temp);
                }

                //
                CUDD.Ref(guardDDs[i]);
                cover = CUDD.Function.Or(cover, guardDDs[i]);

                CUDD.Ref(commandDDs[i]);
                trans = CUDD.Function.Plus(trans, commandDDs[i]);
            }

            CUDD.Deref(guardDDs, commandDDs);

            // store result
            result.guards = cover;
            result.trans = trans;

            result.min = 0;
            result.max = 0;

            return result;
        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'guardDDs, commandDDs' ]
        /// </summary>
        /// <param name="guardDDs"></param>
        /// <param name="commandDDs"></param>
        /// <param name="currentAvailStartBit"></param>
        /// <returns></returns>
        private ComponentDDs CombineNondetCommands(List<CUDDNode> guardDDs, List<CUDDNode> commandDDs, int currentAvailStartBit)
        {
            ComponentDDs compDDs;
            int i, j, maxChoices, numDDChoiceVarsUsed;
            CUDDNode covered, transDD, overlaps, guardHasIChoices, tmp;

            // create object to return result
            compDDs = new ComponentDDs();

            // use 'transDD' to build up MTBDD for transitions
            transDD = CUDD.Constant(0);

            // use 'covered' to track states covered by guards
            covered = CUDD.Constant(0);

            // find overlaps in guards by adding them all up
            overlaps = CUDD.Constant(0);
            for (i = 0; i < guardDDs.Count; i++)
            {
                CUDD.Ref(guardDDs[i]);
                overlaps = CUDD.Function.Plus(overlaps, guardDDs[i]);

                // compute bdd of all guards at same time
                CUDD.Ref(guardDDs[i]);
                covered = CUDD.Function.Or(covered, guardDDs[i]);
            }

            // find the max number of overlaps
            // (i.e. max number of nondet. choices)
            maxChoices = (int)Math.Round(CUDD.FindMax(overlaps));

            // if all the guards were false, we're done already
            if (maxChoices == 0)
            {
                compDDs.guards = covered;
                compDDs.trans = transDD;

                compDDs.min = currentAvailStartBit;
                compDDs.max = currentAvailStartBit;

                CUDD.Deref(overlaps);
                CUDD.Deref(guardDDs, commandDDs);
                return compDDs;
            }

            // likewise, if there are no overlaps, it's also pretty easy
            if (maxChoices == 1)
            {
                // add up dds for all commands
                for (i = 0; i < guardDDs.Count; i++)
                {
                    // add up transitions
                    CUDD.Ref(commandDDs[i]);
                    transDD = CUDD.Function.Plus(transDD, commandDDs[i]);
                }
                compDDs.guards = covered;
                compDDs.trans = transDD;

                compDDs.min = currentAvailStartBit;
                compDDs.max = currentAvailStartBit;

                CUDD.Deref(overlaps);
                CUDD.Deref(guardDDs, commandDDs);
                return compDDs;
            }

            // otherwise, it's a bit more complicated...

            // first, calculate how many dd vars will be needed
            //the indexes of these bits are: (currentLastBitForChoice + 1), (currentLastBitForChoice + 2), ...
            numDDChoiceVarsUsed = (int)Math.Ceiling(Math.Log(maxChoices, 2));

            // select the variables we will use and put them in a JDDVars
            if (currentAvailStartBit + numDDChoiceVarsUsed - 1 >= choiceVars.Count)
            {
                throw new Exception("Insufficient BDD variables allocated for nondeterminism - please report this as a bug. Thank you.");
            }

            CUDDVars varForThisChoice = new CUDDVars();
            for (i = 0; i < numDDChoiceVarsUsed; i++)
            {
                varForThisChoice.AddVar(choiceVars[currentAvailStartBit + i]);
            }

            // for each i (i = 1 ... max number of nondet. choices)
            for (i = 1; i <= maxChoices; i++)
            {

                // find sections of state space
                // which have exactly i nondet. choices in this module
                CUDD.Ref(overlaps);
                guardHasIChoices = CUDD.Convert.Equals(overlaps, i);

                // if there aren't any for this i, skip the iteration
                if (guardHasIChoices.Equals(CUDD.ZERO))
                {
                    CUDD.Deref(guardHasIChoices);
                    continue;
                }

                //store guards satisfying the guardHasIChoices, and classify them
                List<CUDDNode> satisfiedGuards = new List<CUDDNode>();

                //store the current number of commands satisfying the guard of the corresponding guard
                List<int> numberOfChoicesAdded = new List<int>();

                // go thru each command of the module...
                for (j = 0; j < guardDDs.Count; j++)
                {

                    // see if this command's guard overlaps with 'guardHasIChoices'
                    CUDD.Ref(guardDDs[j], guardHasIChoices);
                    tmp = CUDD.Function.And(guardDDs[j], guardHasIChoices);

                    // if it does...
                    if (!tmp.Equals(CUDD.ZERO))
                    {
                        //find guard Index
                        int guardIndex = -1;
                        int choiceIndexForThisCommand = 0;

                        for (int k = 0; k < satisfiedGuards.Count; k++ )
                        {
                            if(satisfiedGuards[k].Equals(tmp))
                            {
                                guardIndex = k;
                                break;
                            }
                        }

                        //if guard exists, then use the index from numberOfChoicesAdded, and increase it
                        if(guardIndex >= 0)
                        {
                            choiceIndexForThisCommand = numberOfChoicesAdded[guardIndex];
                            numberOfChoicesAdded[guardIndex]++;
                        }
                        else
                        {
                            //if not exists, satisfiedGuards, numberOfChoicesAdded
                            satisfiedGuards.Add(tmp);
                            numberOfChoicesAdded.Add(1);
                        }
                        

                        CUDD.Ref(commandDDs[j]);
                        tmp = CUDD.Function.Times(tmp, commandDDs[j]);


                        CUDDNode choiceIndexDD = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), varForThisChoice, choiceIndexForThisCommand, 1);

                        CUDD.Ref(tmp);
                        transDD = CUDD.Function.Plus(transDD, CUDD.Function.Times(tmp, choiceIndexDD));
                    }
                    CUDD.Deref(tmp);
                }

                // take the i bits out of 'overlaps'
                overlaps = CUDD.Function.Times(overlaps, CUDD.Function.Not(guardHasIChoices));
            }

            // store result
            compDDs.guards = covered;
            compDDs.trans = transDD;

            compDDs.min = currentAvailStartBit ;
            compDDs.max = currentAvailStartBit + numDDChoiceVarsUsed;

            CUDD.Deref(overlaps);
            CUDD.Deref(guardDDs, commandDDs);
            return compDDs;
        }

        /// <summary>
        /// The result already contains the guard of this command. In other words, this is the encoding of the whole command, not just only update part.
        /// [ REFS: 'result', DEREFS: '' ]
        /// </summary>
        /// <param name="moduleIndex"></param>
        /// <param name="updates"></param>
        /// <param name="guardDD"></param>
        /// <returns></returns>
        private CUDDNode EncodeCommand(int moduleIndex, List<Update> updates, CUDDNode guardDD)
        {
            CUDDNode result = CUDD.Constant(0);

            foreach (var update in updates)
            {
                CUDD.Ref(guardDD);
                CUDDNode updateDD = EncodeUpdate(moduleIndex, update, guardDD);

                result = CUDD.Function.Plus(result, updateDD);
            }

            return result;
        }

        /// <summary>
        ///  [ REFS: 'result', DEREFS: 'guard' ]
        /// </summary>
        /// <param name="moduleIndex"></param>
        /// <param name="update"></param>
        /// <param name="synch"></param>
        /// <param name="guard"></param>
        /// <returns></returns>
        public CUDDNode EncodeUpdate(int moduleIndex, Update update, CUDDNode guard)
        {
            List<int> updatedVarIndexes = new List<int>();

            CUDDNode result = CUDD.Constant(1);

            foreach (var assignment in update.Assignments)
            {
                int varIndex = varList.GetVarIndex(assignment.LeftHandSide);
                updatedVarIndexes.Add(varIndex);

                int low = varList.GetVarLow(varIndex);
                int high = varList.GetVarHigh(varIndex);

                CUDDNode leftDD = CUDD.Constant(0);
                for (int i = low; i <= high; i++)
                {
                    leftDD = CUDD.Matrix.SetVectorElement(leftDD, colVars[varIndex], i - low, i);
                }

                CUDDNode rightDD = EncodeExpression(assignment.RightHandSide);

                CUDDNode commandDD = CUDD.Function.Equal(leftDD, rightDD);

                CUDD.Ref(guard);
                commandDD = CUDD.Function.And(commandDD, guard);

                //filter out of range configuration
                CUDD.Ref(colVarRanges[varIndex]);
                commandDD = CUDD.Function.And(commandDD, colVarRanges[varIndex]);
                CUDD.Ref(allRowVarRanges);
                commandDD = CUDD.Function.And(commandDD, allRowVarRanges);

                result = CUDD.Function.And(result, commandDD);
            }

            //Global variables and other local variables in the current module if not updated must be the same
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                if ((varList.GetModuleIndex(i) == moduleIndex || varList.GetModuleIndex(i) == Modules.DefaultMainModuleIndex) && !updatedVarIndexes.Contains(i))
                {
                    CUDD.Ref(varIdentities[i]);
                    result = CUDD.Function.And(result, varIdentities[i]);
                }
            }

            return CUDD.Function.Times(result, CUDD.Constant((update.Probability as DoubleConstant).Value));
        }

        private CUDDNode EncodeExpression(Expression exp)
        {
            return expressionEncoder.EncodeExpression(exp);
        }
    }
}
