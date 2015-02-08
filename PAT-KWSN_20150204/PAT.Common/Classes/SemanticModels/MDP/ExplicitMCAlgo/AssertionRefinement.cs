using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.SemanticModels.MDP.Assertion
{
    public abstract class AssertionRefinement : LTS.Assertion.AssertionRefinement
    {
        private double Min = -1;
        private double Max = -1;
        protected QueryConstraintType ConstraintType;

        public override void Initialize(SpecificationBase spec)
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.Initialize(spec);
            }
            else
            {
                //initialize model checking options
                ModelCheckingOptions = new ModelCheckingOptions();
                List<string> LTLEngine = new List<string>();
                LTLEngine.Add(Constants.ENGINE_MDP_SEARCH);
                LTLEngine.Add(Constants.ENGINE_MDP_ANTICHAIN_SEARCH);
                ModelCheckingOptions.AddAddimissibleBehavior(Constants.COMPLETE_BEHAVIOR, LTLEngine);
            }
        }

        /// <summary>
        /// Run the verification and get the result.
        /// </summary>
        /// <returns></returns>
        public override void RunVerification()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                base.RunVerification();
                return;
            }
            MDP mdp = null;
            if (SelectedEngineName == Constants.ENGINE_MDP_SEARCH)
            {
                mdp = BuildMDP(); //GetTransitionRelation();
            }
            else if (SelectedEngineName == Constants.ENGINE_MDP_ANTICHAIN_SEARCH)
            {
                mdp = BuildMDPSubset(); 
                //mdp = BuildMDPAntiChain_L();
                //mdp = BuildMDPAntiChain_S();
            }
            //
            if (!CancelRequested)
            {
                if (SelectedEngineName == Constants.ENGINE_MDP_SEARCH)
                {
                    switch (ConstraintType)
                    {
                        case QueryConstraintType.PROB:
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                            mdp.ResetNonTargetState();
                            Max = 1 - mdp.MinProbability(VerificationOutput);
                            break;
                        case QueryConstraintType.PMAX:
                            Max = 1 - mdp.MinProbability(VerificationOutput);
                            break;
                        case QueryConstraintType.PMIN:
                            Min = 1 - mdp.MaxProbability(VerificationOutput);
                            break;
                    }
                }
                else if (SelectedEngineName == Constants.ENGINE_MDP_ANTICHAIN_SEARCH)
                {
                    switch (ConstraintType)
                    {
                        case QueryConstraintType.PROB:
                            Min = 1 - mdp.MaxProbability(VerificationOutput, true);
                            mdp.ResetNonTargetState();
                            Max = 1 - mdp.MinProbability(VerificationOutput, true);
                            break;
                        case QueryConstraintType.PMAX:
                            Max = 1 - mdp.MinProbability(VerificationOutput, true);
                            break;
                        case QueryConstraintType.PMIN:
                            Min = 1 - mdp.MaxProbability(VerificationOutput, true);
                            break;
                    }
                }

                if (Min == 1)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.VALID;
                }
                else if (Max == 0)
                {
                    VerificationOutput.VerificationResult = VerificationResultType.INVALID;
                }
                else
                {
                    VerificationOutput.VerificationResult = VerificationResultType.WITHPROBABILITY;
                }
            }
        }

        private MDP BuildMDP()
        {
            DeterministicAutomata specAutomaton = BuildDeterministicAutomata(InitSpecStep);

            Stack<Tuple> working = new Stack<Tuple>(1024);
            DeterministicFAState currentSpec = specAutomaton.InitialState;

            MDPState init = new MDPState(InitialStep.GetID() + Constants.SEPARATOR + currentSpec.GetID());

            working.Push(new Tuple(InitialStep as MDPConfiguration, currentSpec, init));

            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }
                // KeyValuePair<KeyValuePair<MDPConfiguration, DeterministicFAState>, MDPState>
               Tuple current = working.Pop();
                if (current.SpecState == null)
                {
                    mdp.AddTargetStates(current.MDPState);
                }
                else
                {
                    IEnumerable<MDPConfiguration> list = current.ImplState.MakeOneMoveLocal();
                    VerificationOutput.Transitions += list.Count();

                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //for (int i = 0; i < list.Count; i++)
                    foreach (var step in list)
                    {
                        //MDPConfiguration step = list[i];
                        DeterministicFAState nextSpec = current.SpecState;

                        if (step.Event != Constants.TAU)
                        {
                            nextSpec = current.SpecState.Next(step.Event);
                        }

                        //System.Diagnostics.Debug.Assert(nextSpec != null, "NextSpec is null!!");

                        //string stepID = step.GetID() + Constants.SEPARATOR + nextSpec.GetID().ToString();
                        string stepID = step.GetID() + Constants.SEPARATOR + (nextSpec == null ? "" : nextSpec.GetID().ToString());                        

                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            mdp.AddState(nextState);

                            working.Push(new Tuple(step, nextSpec, nextState));
                        }

                        if (step.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.MDPState.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(step.Event, nextState);
                            current.MDPState.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                        {
                            current.MDPState.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }

                        currentDistriIndex = step.DisIndex;
                    }

                    if (currentDistriIndex >= 0)
                    {
                        current.MDPState.AddDistribution(newDis);
                    }
                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;
        }

        public class Tuple
        {
            public MDPConfiguration ImplState;
            public DeterministicFAState SpecState;
            public MDPState MDPState;

            public Tuple(MDPConfiguration imp, DeterministicFAState spec, MDPState mdp)
            {
                ImplState = imp;
                SpecState = spec;
                MDPState = mdp;
            }
        }

        public class TupleSub
        {
            public MDPConfiguration ImplState;
            public DeterministicFAState_Subset SpecState;
            public MDPState MDPState;

            public TupleSub(MDPConfiguration imp, DeterministicFAState_Subset spec, MDPState mdp)
            {
                ImplState = imp;
                SpecState = spec;
                MDPState = mdp;
            }
        }

        private MDP BuildMDPSubset()
        {
            DeterministicAutomata_Subset specAutomaton = BuildDeterministicAutomata_Subset(InitSpecStep);

            Stack<TupleSub> working = new Stack<TupleSub>(1024);

            DeterministicFAState_Subset InitialSpec = specAutomaton.InitialState;

      
            Dictionary<string, Dictionary<int, MDPState>> visited = new Dictionary<string, Dictionary<int, MDPState>>();

            int specID = InitialSpec.GetID();
            string impID = InitialStep.GetID();

            MDPState init = new MDPState(impID + Constants.SEPARATOR + specID);

            working.Push(new TupleSub(InitialStep as MDPConfiguration, InitialSpec, init));

            Dictionary<int, MDPState> initialDic = new Dictionary<int, MDPState>();
            initialDic.Add(specID, init);
            visited.Add(impID, initialDic);

            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                TupleSub current = working.Pop();
                if (current.SpecState == null)
                {
                    mdp.AddTargetStates(current.MDPState);
                }
                else
                {
                    IEnumerable<MDPConfiguration> list = current.ImplState.MakeOneMoveLocal();
                    VerificationOutput.Transitions += list.Count();

                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //for (int i = 0; i < list.Count; i++)
                    foreach (var step in list)
                    {
                        //MDPConfiguration step = list[i];
                        DeterministicFAState_Subset nextSpec = current.SpecState;

                        if (step.Event != Constants.TAU)
                        {
                            nextSpec = current.SpecState.Next(step.Event);
                        }

                        //System.Diagnostics.Debug.Assert(nextSpec != null, "NextSpec is null!!");

                        //string stepID = step.GetID() + Constants.SEPARATOR + nextSpec.GetID().ToString();
                        impID = step.GetID();
                        
                        string stepID = impID + Constants.SEPARATOR + (nextSpec == null ? "" : nextSpec.GetID().ToString());

                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            //mdp.AddState(nextState);
                            //KeyValuePair<MDPConfiguration, DeterministicFAState_Subset> newPair =
                            //    new KeyValuePair<MDPConfiguration, DeterministicFAState_Subset>(step, nextSpec);

                            working.Push(new TupleSub(step, nextSpec, nextState));

                            if (nextSpec != null)
                            {
                                specID = nextSpec.GetID();

                                //DeterministicFAState_Subset DFAstate = nextSpec;

                                Dictionary<int, MDPState> mapping = null;
                                if (visited.TryGetValue(impID, out mapping))
                                {
                                    //int Spec = nextSpec.GetID();
                                    mapping.Add(specID, nextState);

                                    foreach (int spec in mapping.Keys)
                                    {
                                        if (specAutomaton.States[spec].Sub.Contains(nextSpec))
                                        {
                                            mapping[spec].Sub.Add(nextState);
                                        }

                                        else if (nextSpec.Sub.Contains(specAutomaton.States[spec]))
                                        {
                                            nextState.Sub.Add(mapping[spec]);
                                        }
                                    }

                                    //foreach (var x in visited)
                                    //{

                                    //    if (x.Key.Key.GetID() == newPair.Key.GetID() &&
                                    //        x.SpecState.Sub.Contains(newPair.Value))
                                    //    {
                                    //        x.Value.Sub.Add(nextState);
                                    //    }
                                    //    else if (x.Key.Key.GetID() == newPair.Key.GetID() &&
                                    //             newPair.Value.Sub.Contains(x.SpecState))
                                    //    {
                                    //        nextState.Sub.Add(x.Value);
                                    //    }
                                    //}

                                    //visited.Add(newPair, nextState);
                                }
                                else
                                {
                                    Dictionary<int, MDPState> newDicDic = new Dictionary<int, MDPState>();
                                    newDicDic.Add(specID, nextState);
                                    visited.Add(impID, newDicDic);
                                }
                            }

                            mdp.AddState(nextState);
                        }

                        if (step.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.MDPState.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(step.Event, nextState);
                            current.MDPState.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                        {
                            current.MDPState.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }

                        currentDistriIndex = step.DisIndex;
                    }


                    if (currentDistriIndex >= 0)
                    {
                        current.MDPState.AddDistribution(newDis);
                    }

                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;

        }
        
        private MDP BuildMDPAntiChain_L()
        {
            DeterministicAutomata_Subset specAutomaton = BuildDeterministicAutomata_Subset(InitSpecStep);

            Stack<TupleSub> working = new Stack<TupleSub>(1024);

            DeterministicFAState_Subset InitialSpec = specAutomaton.InitialState;
            Dictionary<string, HashSet<DeterministicFAState_Subset>> antichain = new Dictionary<string, HashSet<DeterministicFAState_Subset>>();
            
            string impID = InitialStep.GetID();
            int specID = InitialSpec.GetID();
            MDPState init = new MDPState(impID + Constants.SEPARATOR + specID);

            working.Push(new TupleSub(InitialStep as MDPConfiguration, InitialSpec, init));

            
            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);
            HashSet<DeterministicFAState_Subset> Specs = new HashSet<DeterministicFAState_Subset>();
            Specs.Add(InitialSpec);
            antichain.Add(impID, Specs);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                TupleSub current = working.Pop();
                if ( current.SpecState == null)
                {
                    mdp.AddTargetStates(current.MDPState);
                }
                else
                {
                    IEnumerable<MDPConfiguration> list = current.ImplState.MakeOneMoveLocal();
                    VerificationOutput.Transitions += list.Count();

                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //for (int i = 0; i < list.Count; i++)
                    foreach (var step in list)
                    {
                        //MDPConfiguration step = list[i];
                        DeterministicFAState_Subset nextSpec = current.SpecState;

                        if (step.Event != Constants.TAU)
                        {
                            nextSpec = current.SpecState.Next(step.Event);
                        }

                        //System.Diagnostics.Debug.Assert(nextSpec != null, "NextSpec is null!!");

                        //string stepID = step.GetID() + Constants.SEPARATOR + nextSpec.GetID().ToString();
                        impID = step.GetID();

                        string stepID = impID + Constants.SEPARATOR + (nextSpec == null ? "" : nextSpec.GetID().ToString());

                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            //mdp.AddState(nextState);
                            //KeyValuePair<MDPConfiguration, DeterministicFAState_Subset> newPair =
                            //    new KeyValuePair<MDPConfiguration, DeterministicFAState_Subset>(step, nextSpec);

                            //working.Push(new TupleSub(step, nextSpec, nextState));
                            bool addstate = true;
                            if (nextSpec != null)
                            {
                                //specID = nextSpec.GetID();

                                //DeterministicFAState_Subset DFAstate = nextSpec;
                                HashSet<DeterministicFAState_Subset> specStates;
                                //Dictionary<int, MDPState> mapping = null;
                                if (antichain.TryGetValue(impID, out specStates))
                                {
                                    HashSet<DeterministicFAState_Subset> toRemove =
                                        new HashSet<DeterministicFAState_Subset>();
                                    //int Spec = nextSpec.GetID();
                                    bool chainIncrease = true;
                                    foreach (DeterministicFAState_Subset specState in specStates)
                                    {
                                        //DeterministicFAState_Subset specState = specStates[j];
                                        if (specState.Sub.Contains(nextSpec))
                                        {
                                            nextSpec = specState;
                                            addstate = false;
                                            chainIncrease = false;
                                            break;
                                        }
                                        if (nextSpec.Sub.Contains(specState))
                                        {
                                            toRemove.Add(specState);
                                            nextSpec.Sub.Add(specState);
                                            chainIncrease = false;
                                        }

                                    }
                                    if (toRemove.Count > 0)
                                    {
                                        foreach (var specState in toRemove)
                                        {
                                            specStates.Remove(specState);
                                        }
                                        specStates.Add(nextSpec); // toRemove;
                                    }
                                    else if (chainIncrease)
                                    {
                                        specStates.Add(nextSpec);
                                    }

                                }
                                else
                                {
                                    HashSet<DeterministicFAState_Subset> newSpecs = new HashSet<DeterministicFAState_Subset>();
                                    newSpecs.Add(nextSpec);
                                    antichain.Add(impID, newSpecs);
                                }
                            }
                            if(addstate)
                            {
                                mdp.AddState(nextState);
                                working.Push(new TupleSub(step, nextSpec, nextState));
                            }
                            
                            
                        }

                        if (step.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.MDPState.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(step.Event, nextState);
                            current.MDPState.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                        {
                            current.MDPState.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }

                        currentDistriIndex = step.DisIndex;
                    }


                    if (currentDistriIndex >= 0)
                    {
                        current.MDPState.AddDistribution(newDis);
                    }

                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;

        }

        private MDP BuildMDPAntiChain_S()
        {
            DeterministicAutomata_Subset specAutomaton = BuildDeterministicAutomata_Subset(InitSpecStep);

            Stack<TupleSub> working = new Stack<TupleSub>(1024);

            DeterministicFAState_Subset InitialSpec = specAutomaton.InitialState;
            Dictionary<string, HashSet<DeterministicFAState_Subset>> antichain = new Dictionary<string, HashSet<DeterministicFAState_Subset>>();

            string impID = InitialStep.GetID();
            int specID = InitialSpec.GetID();
            MDPState init = new MDPState(impID + Constants.SEPARATOR + specID);

            working.Push(new TupleSub(InitialStep as MDPConfiguration, InitialSpec, init));


            MDP mdp = new MDP(init, Ultility.Ultility.DEFAULT_PRECISION, Ultility.Ultility.MAX_DIFFERENCE);
            HashSet<DeterministicFAState_Subset> Specs = new HashSet<DeterministicFAState_Subset>();
            Specs.Add(InitialSpec);
            antichain.Add(impID, Specs);

            do
            {
                if (CancelRequested)
                {
                    VerificationOutput.NoOfStates = mdp.States.Count;
                    return mdp;
                }

                TupleSub current = working.Pop();
                if (current.SpecState == null)
                {
                    mdp.AddTargetStates(current.MDPState);
                }
                else
                {
                    IEnumerable<MDPConfiguration> list = current.ImplState.MakeOneMoveLocal();
                    VerificationOutput.Transitions += list.Count();

                    int currentDistriIndex = -1;
                    Distribution newDis = new Distribution(Constants.TAU);

                    //for (int i = 0; i < list.Count; i++)
                    foreach (MDPConfiguration step in list)
                    {
                        //MDPConfiguration step = list[i];
                        DeterministicFAState_Subset nextSpec = current.SpecState;

                        if (step.Event != Constants.TAU)
                        {
                            nextSpec = current.SpecState.Next(step.Event);
                        }

                        //System.Diagnostics.Debug.Assert(nextSpec != null, "NextSpec is null!!");

                        //string stepID = step.GetID() + Constants.SEPARATOR + nextSpec.GetID().ToString();
                        impID = step.GetID();

                        string stepID = impID + Constants.SEPARATOR + (nextSpec == null ? "" : nextSpec.GetID().ToString());

                        MDPState nextState;

                        if (!mdp.States.TryGetValue(stepID, out nextState))
                        {
                            nextState = new MDPState(stepID);
                            //mdp.AddState(nextState);
                            //KeyValuePair<MDPConfiguration, DeterministicFAState_Subset> newPair =
                            //    new KeyValuePair<MDPConfiguration, DeterministicFAState_Subset>(step, nextSpec);

                            //working.Push(new TupleSub(step, nextSpec, nextState));
                            bool addstate = true;
                            if (nextSpec != null)
                            {
                                //specID = nextSpec.GetID();

                                //DeterministicFAState_Subset DFAstate = nextSpec;
                                HashSet<DeterministicFAState_Subset> specStates;
                                //Dictionary<int, MDPState> mapping = null;
                                if (antichain.TryGetValue(impID, out specStates))
                                {
                                    HashSet<DeterministicFAState_Subset> toRemove =
                                        new HashSet<DeterministicFAState_Subset>();
                                    //int Spec = nextSpec.GetID();
                                    bool chainIncrease = true;
                                    foreach (DeterministicFAState_Subset specState in specStates)
                                    {
                                        //DeterministicFAState_Subset specState = specStates[j];
                                        if (specState.Sub.Contains(nextSpec))
                                        {
                                            
                                            toRemove.Add(specState);
                                            nextSpec.Sub.Add(specState);
                                            chainIncrease = false;
                                        }
                                        if (nextSpec.Sub.Contains(specState))
                                        {
                                            nextSpec = specState;
                                            addstate = false;
                                            chainIncrease = false;
                                            break;
                                        }

                                    }
                                    if (toRemove.Count > 0)
                                    {
                                        foreach (var specState in toRemove)
                                        {
                                            specStates.Remove(specState);
                                        }
                                        specStates.Add(nextSpec); // toRemove;
                                    }
                                    else if (chainIncrease)
                                    {
                                        specStates.Add(nextSpec);
                                    }

                                }
                                else
                                {
                                    HashSet<DeterministicFAState_Subset> newSpecs = new HashSet<DeterministicFAState_Subset>();
                                    newSpecs.Add(nextSpec);
                                    antichain.Add(impID, newSpecs);
                                }
                            }
                            if (addstate)
                            {
                                mdp.AddState(nextState);
                                working.Push(new TupleSub(step, nextSpec, nextState));
                            }


                        }

                        if (step.DisIndex == -1)
                        {
                            if (currentDistriIndex >= 0)
                            {
                                current.MDPState.AddDistribution(newDis);
                                newDis = new Distribution(Constants.TAU);
                            }

                            Distribution newTrivialDis = new Distribution(step.Event, nextState);
                            current.MDPState.AddDistribution(newTrivialDis);
                        }
                        else if (currentDistriIndex != -1 && step.DisIndex != currentDistriIndex)
                        {
                            current.MDPState.AddDistribution(newDis);
                            newDis = new Distribution(Constants.TAU);
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }
                        else
                        {
                            newDis.AddProbStatePair(step.Probability, nextState);
                        }

                        currentDistriIndex = step.DisIndex;
                    }


                    if (currentDistriIndex >= 0)
                    {
                        current.MDPState.AddDistribution(newDis);
                    }

                }
            } while (working.Count > 0);

            VerificationOutput.NoOfStates = mdp.States.Count;
            //mdp.BackUpTargetStates();
            return mdp;

        }

        public override string GetResultString()
        {
            if (ConstraintType == QueryConstraintType.NONE)
            {
                return base.GetResultString();
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Constants.VERFICATION_RESULT_STRING);
            if (this.VerificationOutput.VerificationResult == VerificationResultType.VALID)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is VALID.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.UNKNOWN)
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NEITHER PROVED NOR DISPROVED.");
            }
            else if (this.VerificationOutput.VerificationResult == VerificationResultType.WITHPROBABILITY)
            {
                //sb.AppendLine("\t***The Assertion (" + ToString() + ") is Valid With Probability " + Ultility.Ultility.GetProbIntervalString(Min, Max, Precision) + ".");
                if (this.Max != -1 && Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Probability " + Ultility.Ultility.GetProbIntervalString((float)Min, (float)Max) + ";");
                }
                else if (this.Max != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Max Probability " + Ultility.Ultility.GetProbIntervalString((float)Max) + ";");
                }
                else if (this.Min != -1)
                {
                    sb.AppendLine("The Assertion (" + ToString() + ") is Valid with Min Probability " + Ultility.Ultility.GetProbIntervalString((float)Min) + ";");
                }
            }
            else
            {
                sb.AppendLine("The Assertion (" + ToString() + ") is NOT valid.");
                sb.AppendLine();
            }

            sb.AppendLine();

            sb.AppendLine("********Verification Setting********");
            sb.AppendLine("Admissible Behavior: " + SelectedBahaviorName);
            sb.AppendLine("Search Engine: " + SelectedEngineName);
            sb.AppendLine("System Abstraction: " + MustAbstract);
            sb.AppendLine("Maximum difference threshold : " + Ultility.Ultility.MAX_DIFFERENCE);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}