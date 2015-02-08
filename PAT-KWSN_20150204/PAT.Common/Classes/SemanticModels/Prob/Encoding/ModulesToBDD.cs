using System;
using System.Collections.Generic;
using System.Linq;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;


namespace PAT.Common.Classes.SemanticModels.Prob.Encoding
{
    public partial class ModulesToBDD
    {
        private ExpressionToBDD expressionEncoder;

        /// <summary>
        /// List of all command labels in modules
        /// </summary>
        private List<string> synchs = new List<string>();

        public VariableList varList;

        /// <summary>
        /// All row vars of global and local variable. Exclude nondet vars
        /// </summary>
        private CUDDVars allRowVars = new CUDDVars();

        /// <summary>
        /// All col vars of global and local variable. Exclude nondet vars
        /// </summary>
        private CUDDVars allColVars = new CUDDVars();

        /// <summary>
        /// All vars of synchronized vars to distinguish command labels
        /// </summary>
        private CUDDVars allSynchVars = new CUDDVars();


        /// <summary>
        /// All vars of choices by one guard and one command label
        /// </summary>
        private CUDDVars allChoiceVars = new CUDDVars();

        /// <summary>
        /// Combination of allSynchVars and allNondetVars
        /// </summary>
        private CUDDVars allNondetVars = new CUDDVars();

        /// <summary>
        /// All row vars of global variables
        /// </summary>
        private CUDDVars globalRowVars = new CUDDVars();

        /// <summary>
        /// All col vars of global variables
        /// </summary>
        private CUDDVars globalColVars = new CUDDVars();

        /// <summary>
        /// All row vars of local variables in modules
        /// </summary>
        private List<CUDDVars> moduleRowVars = new List<CUDDVars>();

        /// <summary>
        /// All col vars of local variables in modules
        /// </summary>
        private List<CUDDVars> moduleColVars = new List<CUDDVars>();

        /// <summary>
        /// Constraint of vars in range in modules
        /// </summary>
        private List<CUDDNode> moduleRangeDDs = new List<CUDDNode>();

        /// <summary>
        /// Module identity. Constraints of unchanged local variables
        /// </summary>
        private List<CUDDNode> moduleIdentities = new List<CUDDNode>();

        /// <summary>
        /// List of row vars by variable
        /// </summary>
        private List<CUDDVars> rowVars = new List<CUDDVars>();

        /// <summary>
        /// List of col vars by variable
        /// </summary>
        private List<CUDDVars> colVars = new List<CUDDVars>();

        /// <summary>
        /// Row-var-based constraints of variable in range
        /// </summary>
        private CUDDNode allRowVarRanges;


        /// <summary>
        /// List of col-var-based constraints of variable in range. Use this in update expression to limit the value of the assigned variable
        /// </summary>
        private List<CUDDNode> colVarRanges = new List<CUDDNode>();

        /// <summary>
        /// List of variable identities
        /// </summary>
        private List<CUDDNode> varIdentities = new List<CUDDNode>();

        
        /// <summary>
        /// List of bits for synchornization
        /// </summary>
        private List<CUDDNode> syncVars = new List<CUDDNode>();


        /// <summary>
        /// List of bits for choice
        /// </summary>
        private List<CUDDNode> choiceVars = new List<CUDDNode>();

        /// <summary>
        /// List of encodings of variables
        /// </summary>
        public List<CUDDNode> variableEncoding = new List<CUDDNode>();

        /// <summary>
        /// transition matrix
        /// </summary>
        public CUDDNode trans;

        /// <summary>
        /// Init condition
        /// </summary>
        public CUDDNode init;

        /// <summary>
        /// List of state rewards by reward structure
        /// </summary>
        public List<CUDDNode> stateRewards = new List<CUDDNode>();

        /// <summary>
        /// List of transition rewards by reward structure
        /// </summary>
        public List<CUDDNode> transRewards = new List<CUDDNode>();

        /// <summary>
        /// Count number of bool variables used
        /// </summary>
        private int numverOfBoolVars = 0;

        public Modules modules;

        public ModulesToBDD(Modules modules)
        {
            this.modules = modules;
            CUDD.InitialiseCUDD(2048 * 1024, Math.Pow(10, -15));
        }

        public IModel Encode()
        {
            //get variable info from ModulesFile
            varList = new VariableList(modules);

            //Collect all sync label of all modules
            synchs = modules.GetAllSynchs().ToList();

            AddVars();

            //Create Expression Encoder, use the same copy of varList, variableEncoding
            expressionEncoder = new ExpressionToBDD(varList, variableEncoding);

            EncodeSystemDef(modules.systemDef);

            // get rid of any nondet dd variables not needed
            if (modules.modelType == ModelType.MDP)
            {
                CUDDNode tmp = CUDD.GetSupport(trans);
                tmp = CUDD.Abstract.ThereExists(tmp, allRowVars);
                tmp = CUDD.Abstract.ThereExists(tmp, allColVars);

                CUDDVars ddv = new CUDDVars();
                while (!tmp.Equals(CUDD.ONE))
                {
                    ddv.AddVar(CUDD.Var(tmp.GetIndex()));
                    tmp = tmp.GetThen();
                }
                CUDD.Deref(tmp);
                allNondetVars.Deref();
                allNondetVars = ddv;
            }

            init = GetInitState();

            //
            CUDD.Deref(moduleRangeDDs, moduleIdentities, colVarRanges, syncVars, choiceVars);
            CUDD.Deref(moduleRowVars, moduleColVars, rowVars, colVars, new List<CUDDVars>(){globalRowVars, globalColVars, allSynchVars, allChoiceVars});
            
            IModel result;
            if (modules.modelType == ModelType.DTMC)
            {
                //
                allNondetVars.Deref();

                result = new ProbModel(trans, init, stateRewards, transRewards, allRowVars, allColVars, varList, allRowVarRanges,
                                     varIdentities, variableEncoding);
            }
            else
            {
                result = new NonDetModel(trans, init, stateRewards, transRewards, allRowVars, allColVars, allNondetVars,
                                       varList, allRowVarRanges, varIdentities, variableEncoding);
            }

            return result;
        }

        /// <summary>
        /// Return init state based on init value of variables
        /// </summary>
        /// <returns></returns>
        private CUDDNode GetInitState()
        {
            if (modules.init != null)
            {
                return EncodeExpression(modules.init);
            }
            else
            {

                CUDDNode result = CUDD.Constant(1);

                for (int i = 0; i < varList.GetNumberOfVar(); i++)
                {
                    CUDDNode tmp = CUDD.Matrix.SetVectorElement(CUDD.Constant(0), rowVars[i], varList.GetVarInit(i) - varList.GetVarLow(i), 1);
                    result = CUDD.Function.And(result, tmp);
                }

                return result;
            }
        }

        /// <summary>
        /// Calculate state rewards and store in stateRewards
        /// Transition rewards are prepared and stored in sysDDs
        /// </summary>
        /// <param name="sysDDs"></param>
        private void ComputeRewards(SystemDDs sysDDs)
        {
            int numRewardStructs = modules.rewardStructs.Count;

            //Initialize reward of State and the resulted SystemDDs sysDDs
            for (int j = 0; j < numRewardStructs; j++)
            {
                stateRewards.Add(CUDD.Constant(0));

                sysDDs.ind.rewards.Add(CUDD.Constant(0));

                for (int i = 0; i < synchs.Count; i++)
                {
                    sysDDs.synchs[i].rewards.Add(CUDD.Constant(0));
                }
            }

            // for each reward structure...
            for (int j = 0; j < numRewardStructs; j++)
            {
                // get reward struct
                RewardStruct rs = modules.rewardStructs[j];

                // work through list of items in reward struct
                foreach (var rewardItem in rs.items)
                {
                    // translate states predicate and reward expression
                    CUDDNode guard = EncodeExpression(rewardItem.guard);
                    CUDDNode rewards = EncodeExpression(rewardItem.reward);

                    CUDDNode rewardDD;
                    string synch = rewardItem.synch;
                    if (synch == null)
                    {
                        // first case: item corresponds to state rewards

                        // restrict rewards to relevant states
                        rewardDD = CUDD.Function.Times(guard, rewards);

                        // check for negative rewards
                        if (CUDD.FindMin(rewardDD) < 0)
                        {
                            throw new Exception("Reward structure item with guard " + rewardItem.guard + " contains negative rewards");
                        }
                        // add to state rewards
                        stateRewards[j] = CUDD.Function.Plus(stateRewards[j], rewardDD);
                    }
                    else
                    {
                        // second case: item corresponds to transition rewards

                        //find the corresponding component to update the reward
                        ComponentDDs compDDs;

                        // work out which (if any) action this is for
                        if (synch == string.Empty)
                        {
                            compDDs = sysDDs.ind;
                        }
                        else if (synchs.Contains(synch))
                        {
                            int k = synchs.IndexOf(synch);
                            compDDs = sysDDs.synchs[k];
                        }
                        else
                        {
                            throw new Exception("Invalid action name \"" + synch + "\" in reward structure item");
                        }


                        // identify corresponding transitions
                        // (for dtmcs/ctmcs, keep actual values - need to weight rewards; for mdps just store 0/1)
                        CUDD.Ref(compDDs.trans);
                        rewardDD = (modules.modelType == ModelType.MDP)? (CUDD.Convert.GreaterThan(compDDs.trans, 0)): compDDs.trans;

                        // restrict to relevant states
                        rewardDD = CUDD.Function.Times(rewardDD, guard);
                        // multiply by reward values
                        rewardDD = CUDD.Function.Times(rewardDD, rewards);

                        // check for negative rewards
                        if (CUDD.FindMin(rewardDD) < 0)
                        {
                            throw new Exception("Reward structure item with guard " + rewardItem.guard + " contains negative rewards");
                        }
                        // add result to rewards
                        compDDs.rewards[j] = CUDD.Function.Plus(compDDs.rewards[j], rewardDD);
                    }
                }
            }
        }

        
	
    }
}
