using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.GenericDiff.diff;
using PAT.GenericDiff.graph;
using PAT.GenericDiff.select;
using PAT.GenericDiff.test;
using PAT.GenericDiff.utility;
using Graph=Microsoft.Msagl.Drawing.Graph;

namespace PAT.GUI.Forms.GraphDiff
{
    public class PATModelComparison : ModelComparison
    {
        //todo: xingzc. when comparing paramterized system, initialize when creating PATModelComparison instance
        private int infinity = 100; // todo: xingzc. a big number >> cutnumber. compute a big number based on cutnumber
        private bool compareParameterizedSystem = false;
        private int cutnumber1 = -1;  
        private int cutnumber2 = -1;

        private bool notMaximalBipartiteMatch = false;

        private bool compareConfigGraph = true;

        private bool matchEventDetails = true;

        private List<string[]> selectedVariables = new List<string[]>();
        private List<int> vectorLength = new List<int>();

        private DictionaryJ<String, GraphDataConfiguration> leftGraphDataConfigs;
        private DictionaryJ<String, GraphDataConfiguration> rightGraphDataConfigs;

        public override void initialize()
        {
            // Control what graph elements will be created
            graphCreationParameters.createQualifiedGraphNode = false;
            graphCreationParameters.containmentRelation = "";
            graphCreationParameters.encapsulateInMatchedGraphNode = false;

            // GenericDiff parameters according to domain semantics
            diffParameters.compatibleTypes =
                new String[][]
                    {
                        new String[] {"INIT", "100", "false", null},
                        new String[] {"REGULAR", "100", "false", null},
                        new String[] {"TRANSITION", "200", "false", "false, true"} //String.Format("false, {0}", matchEventDetails ? "true" : "false")}};
                    }; 

            diffParameters.mappingWeights =
                        new String[][] {
                            new String[] {"INIT", "INIT", "1", "true"},
                            new String[] {"REGULAR", "REGULAR", "1", "true"},
                            new String[] {"INIT", "REGULAR", "1", "true"},
                            new String[] {"REGULAR", "INIT", "1", "true"},
                            new String[] {"TRANSITION", "TRANSITION", "1", "true"}};

            diffParameters.distanceThresholdVectors =
                        new String[][] {
                            new String[] {"TRANSITION", "TRANSITION", "0", "0"}}; // hamming distance threshold=0 means only same-label transitions can be paired-up

            diffParameters.epsilonEdgesExcluded = new String[0][];

            // GenericDiff execution control parameters
            diffParameters.maxIteration = 100;
            diffParameters.noPairupIdenticalNodesWithOthers = false; //todo: xingzc. only make sense when nodes have meaningful labels
            diffParameters.stepsOfNeighbors = 1;
            diffParameters.stopGreedySearchThreshold = 0.8;
            diffParameters.stopPropagationThreshold = 0.00001;
            diffParameters.removeOutliersThreshold = 2;
            diffParameters.considerNodeDistance = true;
            diffParameters.considerEdgeDistance = true;
            diffParameters.includeUnmatched = false;
            diffParameters.includeNeighbors = this.matchEventDetails;
            diffParameters.conductCandidatesSearch = true;
            diffParameters.appendEpsilonPairups = true;
            diffParameters.doGreedySearch = true;
            diffParameters.compareAdditionalInfo = this.compareConfigGraph; // !compareParameterizedSystem; //todo: xingzc. set false if parameterized system
            diffParameters.notMaximalBipartiteMatch = this.notMaximalBipartiteMatch;

            // Strategy objects
            diffParameters.searchPotentialCandidatesKind = CandidatesSearchStrategyKind.Sequential;
            diffParameters.neighborhoodMatchingStrategyKind = SelectionStrategyKind.StableMarriage;
            diffParameters.removeOutliersStrtegyKind = RemoveOutliersStrategyKind.None;
            diffParameters.matchingStrategyKind = SelectionStrategyKind.StableMarriage;

            // Domain knowledge that controls the parsing and graph creation process
            int numOfVectors = selectedVariables.Count; // todo: xingzc. the number of variable groups.
            if(!this.compareConfigGraph)
            {
                numOfVectors = numOfVectors + 1;
            }
            
            string[][] nodeInfoTemplates = new string[numOfVectors][]; 
            double[][] nodeInfoNone1 = new double[numOfVectors][];
            double[][] nodeInfoNone2 = new double[numOfVectors][];
            string[][] nodeInfoCalculatorKind = new string[numOfVectors][];

            if (!this.compareConfigGraph)
            {
                nodeInfoTemplates[numOfVectors - 1] = new string[0];
                nodeInfoNone1[numOfVectors - 1] = new double[0];
                nodeInfoNone2[numOfVectors - 1] = new double[0];
                nodeInfoCalculatorKind[numOfVectors - 1] = new String[] {"Taxi", "vector"};
            }

            // todo: xingzc. initialize info vectors by selected variables
            for (int varGroupIndex = 0; varGroupIndex < selectedVariables.Count; varGroupIndex++)
            {
                string[] varGroup = selectedVariables[varGroupIndex];

                nodeInfoTemplates[varGroupIndex] = varGroup;
                nodeInfoNone1[varGroupIndex] = new double[vectorLength[varGroupIndex]];
                nodeInfoNone2[varGroupIndex] = new double[vectorLength[varGroupIndex]];
                nodeInfoCalculatorKind[varGroupIndex] = new String[] { "Hamming", "vector" }; // todo: xingzc. may be taxi if int
            }

            /*String[][] nodeInfoTemplates =
                new String[][]
                    {
                        //new String[] {""}, //todo: xingzc. A counter vector if parameterized system, otherwise no vector
                        //new String[] {"noOfReading", "writing"} //todo: xingzc. initialze from selected variables
                    };

            double[][] nodeInfoNone1 =
                new double[][]
                    {
                        //new double[] {},// todo: xingzc. A none counter vector if parameterized system, otherwise no vector
                        //new double[] {0, 0} //todo: xingzc.initialze from selected variables
                    };

            double[][] nodeInfoNone2 =
                new double[][]
                    {
                        //new double[] {},// todo: xingzc. A none counter vector if parameterized system, otherwise no vector
                        //new double[] {0, 0} //todo: xingzc.initialze from selected variables 
                    };

            String[][] nodeInfoCalculatorKind =
                new String[][]
                    {
                        //new String[] {"BagJaccard", "set"}, //todo: xingzc.
                        //new String[] {"Hamming", "vector"} //todo: xingzc. 
                    };*/

            String[][] edgeInfoTemplates = new String[][]
                                               {
                                                   new String[] {""},
                                                   new String[0]
                                               }; 

            double[][] edgeInfoNone = new double[][]
                                          {
                                              new double[] {0},
                                              new double[0]
                                          }; 

            String[][] edgeInfoCalculatorKind = new String[][]
                                                    {
                                                        new String[] { "Hamming", "vector" },
                                                        new String[] { "SeqHamming", "set" }
                                                    }; 

            leftGraphDataConfigs = new DictionaryJ<String, GraphDataConfiguration>();
            GraphDataConfiguration leftGraphDataConfig = GraphDataConfiguration.create(nodeInfoTemplates, nodeInfoNone1, nodeInfoCalculatorKind, null);
            leftGraphDataConfigs.Add("INIT", leftGraphDataConfig);
            leftGraphDataConfigs.Add("REGULAR", leftGraphDataConfig);

            rightGraphDataConfigs = new DictionaryJ<String, GraphDataConfiguration>();
            GraphDataConfiguration rightGraphDataConfig = GraphDataConfiguration.create(nodeInfoTemplates, nodeInfoNone2, nodeInfoCalculatorKind, null);
            rightGraphDataConfigs.Add("INIT", rightGraphDataConfig);
            rightGraphDataConfigs.Add("REGULAR", rightGraphDataConfig);

            GraphDataConfiguration edgeConfig = GraphDataConfiguration.create(edgeInfoTemplates, edgeInfoNone, edgeInfoCalculatorKind, null);
            leftGraphDataConfigs.Add("TRANSITION", edgeConfig);
            rightGraphDataConfigs.Add("TRANSITION", edgeConfig);
        }


        /* todo: xingzc. 
         * 
         * Need GlobalVar info to initialze nodeInfoTemplates/nodeInfoNone/nodeInfoCalculatorKind
         * 
         * Need to know if left/right is parameterized system and cutnumber. This affects initialize() and Parser.
         * For parameterized system, no need to get a subgraph from a Configuration. A counter vector will be ok.
         * Probably we can still obtain a subgraph. But do not need to compare subgraphs. Instead, extract a counter
         * vector from the subgraph and use it to represent the state info.
         */
        public List<PairUpCandidate> execute(Graph left, Graph right, List<string> vars, bool notMaximalBipartite, SpecificationBase leftSpec, SpecificationBase rightSpec, bool eventDetails, bool matchProcessParameters, bool matchStateStructure, bool matchIfGuardCondition)
        {
            if(left.UserData != null)
            {
                this.cutnumber1 = (int)left.UserData;
            }
            if(right.UserData != null)
            {
                this.cutnumber2 = (int)right.UserData;
            }
            this.compareParameterizedSystem = (this.cutnumber1 != -1) && (this.cutnumber2 != -1);

            this.compareConfigGraph = compareConfigGraph && !compareParameterizedSystem;

            this.notMaximalBipartiteMatch = notMaximalBipartite;

            this.matchEventDetails = eventDetails;

            //ly; newly added.
            this.compareConfigGraph = matchStateStructure;
            //todo: matchIfGuardCondition needs to be added.

            List<string> primitiveVariables = new List<string>();

            Valuation leftValuation = leftSpec.GetEnvironment();
            foreach (string varName in vars)
            {
                if (leftValuation != null && leftValuation.Variables != null)
                {
                    ExpressionValue value = leftValuation.Variables.GetContainsKey(varName);
                    if (value is RecordValue)
                    {
                        RecordValue array = value as RecordValue;
                        List<string> names = new List<string>();
                        for (int i = 0; i < array.Associations.Length; i++)
                        {
                            names.Add(varName);    
                        }
                        selectedVariables.Add(names.ToArray());
                        vectorLength.Add(array.Associations.Length); // array length
                    }
                    else if (false)
                    {
                        // todo: xingzc. processing other complex data type
                    }
                    else // primitive types
                    {
                        primitiveVariables.Add(varName);
                    }
                }

                /* now assume variables of left/right graphs are the same and variables of all the nodes are the same
                 * thus, no need to do this
                 */
                /*if (rightValuation != null && rightValuation.Variables != null)
                {
                    ExpressionValue value = rightValuation.Variables.GetContainsKey(varName);
                    if (value is RecordValue)
                    {
                        selectedVariables.Add(new string[] {varName});
                        //vectorsLength.Add((value as RecordValue).Associations.Length); // array length
                    }
                    else if (false)
                    {
                        // todo: xingzc. processing other complex data type
                    }
                    else // primitive types
                    {
                        primitiveVariables.Add(varName);
                    }
                }*/

            }

            if (primitiveVariables.Count > 0)
            {
                selectedVariables.Add(primitiveVariables.ToArray());
                vectorLength.Add(primitiveVariables.Count);
            }

            initialize();

            PAT.GenericDiff.diff.GenericDiff diff = new PAT.GenericDiff.diff.GenericDiff(
                                               diffParameters.compatibleTypes,
                                               diffParameters.mappingWeights,
                                               diffParameters.distanceThresholdVectors,
                                               diffParameters.epsilonEdgesExcluded,
                                               diffParameters.maxIteration,
                                               diffParameters.noPairupIdenticalNodesWithOthers,
                                               diffParameters.stepsOfNeighbors,
                                               diffParameters.stopGreedySearchThreshold,
                                               diffParameters.stopPropagationThreshold,
                                               diffParameters.removeOutliersThreshold,
                                               diffParameters.considerNodeDistance,
                                               diffParameters.considerEdgeDistance,
                                               diffParameters.includeUnmatched,
                                               diffParameters.includeNeighbors,
                                               diffParameters.conductCandidatesSearch,
                                               diffParameters.appendEpsilonPairups,
                                               diffParameters.doGreedySearch,
                                               diffParameters.compareAdditionalInfo,
                                               diffParameters.notMaximalBipartiteMatch,
                                               diffParameters.searchPotentialCandidatesKind,
                                               diffParameters.neighborhoodMatchingStrategyKind,
                                               diffParameters.removeOutliersStrtegyKind,
                                               diffParameters.matchingStrategyKind,
                                               null);

            leftSpec.GrabSharedDataLock();
            leftSpec.LockSharedData(true);
            PATModelFileParser modelParser1 = new PATModelFileParser(left, Side.LHS, compareParameterizedSystem, this.compareConfigGraph, matchProcessParameters, infinity, cutnumber1);
            PAT.GenericDiff.graph.Graph graphLeft = createGraph(Side.LHS, modelParser1, leftGraphDataConfigs, "leftgraph");
            leftSpec.UnLockSharedData();

            rightSpec.GrabSharedDataLock();
            rightSpec.LockSharedData(true);
            PATModelFileParser modelParser2 = new PATModelFileParser(right, Side.RHS, compareParameterizedSystem, this.compareConfigGraph, matchProcessParameters, infinity, cutnumber2);
            PAT.GenericDiff.graph.Graph graphRight = createGraph(Side.RHS, modelParser2, rightGraphDataConfigs, "rightgraph");
            rightSpec.UnLockSharedData();

            List<PairUpCandidate> matches = diff.compare(graphLeft, graphRight);
            
            return matches;
        }
    }

}
