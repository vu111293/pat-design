using System;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.GenericDiff.graph;
using PAT.GenericDiff.diff;
using PAT.GenericDiff.utility;
using PAT.GenericDiff.test;
using PAT.GenericDiff.select;

namespace PAT.GUI.Forms.GraphDiff
{
    class ConfigurationGraphBuilder : ModelComparison
    {
        private DictionaryJ<String, GraphDataConfiguration> graphDataConfigs;

        private PAT.GenericDiff.diff.GenericDiff diff;

        private bool compareParameterizedSystem;

        private bool matchProcessParameters = true;

        private bool compareSameSpec = true;

        public ConfigurationGraphBuilder(bool compareParameterizedSystem, bool matchProcessParameters)
        {
            this.compareParameterizedSystem = compareParameterizedSystem;

            this.matchProcessParameters = matchProcessParameters;

            //this.compareSameSpec = ?? set true if compare models of same spec

            initialize();

            this.removedIsolatedNodes = false;

            diff = new PAT.GenericDiff.diff.GenericDiff(
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
        }

        public override void initialize()
        {
            if(compareParameterizedSystem)
            {
                initializeParameterizedSystem();
            }
            else
            {
                initializeNonParameterizedSystem();
            }
        }

        private void initializeParameterizedSystem()
        {
            // Control what graph elements will be created
            graphCreationParameters.createQualifiedGraphNode = false;
            graphCreationParameters.containmentRelation = "";
            graphCreationParameters.encapsulateInMatchedGraphNode = false;

            // GenericDiff parameters according to domain semantics
            diffParameters.compatibleTypes =
                new String[][] {new string[] {"NODE", "1000", "false", null},
                                new String[] {"EDGE", "2000", "false", null}};

            diffParameters.mappingWeights =
                new String[][] {new String[] {"NODE", "NODE", "1", "true"},
                                new String[] {"EDGE", "EDGE", "1", "true"}};

            diffParameters.distanceThresholdVectors =
                    new String[][] {};

            diffParameters.epsilonEdgesExcluded = new String[0][];

            // GenericDiff execution control parameters
            diffParameters.maxIteration = 100;
            diffParameters.noPairupIdenticalNodesWithOthers = false; // value no longer checked in GenericDiff; it is defined for each type and checked insider GraphNode
            diffParameters.stepsOfNeighbors = 1;
            diffParameters.stopGreedySearchThreshold = 0.8;
            diffParameters.stopPropagationThreshold = 0.00001;
            diffParameters.removeOutliersThreshold = 2;
            diffParameters.considerNodeDistance = false;
            diffParameters.considerEdgeDistance = false;
            diffParameters.includeUnmatched = false;
            diffParameters.includeNeighbors = false;
            diffParameters.conductCandidatesSearch = false;
            diffParameters.appendEpsilonPairups = false;
            diffParameters.doGreedySearch = false;
            diffParameters.compareAdditionalInfo = false;
            diffParameters.notMaximalBipartiteMatch = false;

            // Strategy objects
            diffParameters.searchPotentialCandidatesKind = CandidatesSearchStrategyKind.None;
            diffParameters.neighborhoodMatchingStrategyKind = SelectionStrategyKind.None;
            diffParameters.removeOutliersStrtegyKind = RemoveOutliersStrategyKind.None;
            diffParameters.matchingStrategyKind = SelectionStrategyKind.None;

            // Domain knowledge that controls the parsing and graph creation process
            String[][] simpleNodeInfoTemplates = new String[][] {};

            double[][] simpleNodeInfoNone = new double[][] {};

            String[][] simpleNodeInfoCalculatorKind = new String[][] {};

            String[][] edgeInfoTemplates = new String[][] { };

            double[][] edgeInfoNone = new double[][] { };

            String[][] edgeInfoCalculatorKind = new String[][] { };

            graphDataConfigs = new DictionaryJ<String, GraphDataConfiguration>();

            GraphDataConfiguration simpleNodeConfig = GraphDataConfiguration.create(simpleNodeInfoTemplates, simpleNodeInfoNone, simpleNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("NODE", simpleNodeConfig);

            GraphDataConfiguration edgeConfig = GraphDataConfiguration.create(edgeInfoTemplates, edgeInfoNone, edgeInfoCalculatorKind, null);
            graphDataConfigs.Add("EDGE", edgeConfig);
        }

        private void initializeNonParameterizedSystem()
        {
            // Control what graph elements will be created
            graphCreationParameters.createQualifiedGraphNode = false;
            graphCreationParameters.containmentRelation = "";
            graphCreationParameters.encapsulateInMatchedGraphNode = false;

            // GenericDiff parameters according to domain semantics
            string channelInOutputValidDistanceElements = "true, false"; // [0] outgoing branch; [1] channelName
            string channelInputGuardedValidDistanceElements = string.Format("true, {0}, false", !compareSameSpec && matchProcessParameters ? "true" : "false"); // [0] outgoing branch; [1] guard expr char array; [2] channelName
            string conditionalChoiceValidDistanceElements = string.Format("true, {0}", !compareSameSpec && matchProcessParameters ? "true" : "false"); // [0] outgoing branck; [1] 
            string dataOpValidDistanceElements = string.Format("true, {0}, {0}, false", !compareSameSpec && matchProcessParameters ? "true" : "false");
            string defRefValidDistanceElements = string.Format("true, {0}, false", matchProcessParameters ? "true" : "false");
            string hidingValidDistanceElements = string.Format("true, {0}", matchProcessParameters ? "true" : "false");
            
            diffParameters.compatibleTypes =
                new String[][] {new String[] {Constants.ATOMIC_STARTED, "1000", "false", null},
                                new String[] {Constants.CASE, "1001", "false", null},
                                new String[] {Constants.GENERAL_CHOICE, "1012", "false", null},
                                new String[] {Constants.EXTERNAL_CHOICE, "1013", "false", null},
                                new String[] {Constants.INTERLEAVE, "1014", "false", null},
                                new String[] {Constants.INTERLEAVE + "a", "1015", "false", null},
                                new String[] {Constants.INTERNAL_CHOICE, "1016", "false", null},
                                new String[] {Constants.PARALLEL, "1017", "false", null},
                                new String[] {Constants.INTERRUPT, "1018", "false", null},
                                new String[] {Constants.SEQUENTIAL, "1019", "false", null},
                                new String[] {Constants.SKIP, "1020", "false", null},
                                new String[] {Constants.STOP, "1021", "false", null},
                                
                                new String[] {"C!", "1004", "false", channelInOutputValidDistanceElements }, // different node has different label
                                new String[] {"C?", "1002", "false", channelInOutputValidDistanceElements }, // different node has different label

                                new String[] {"?[]", "1003", "false", channelInputGuardedValidDistanceElements }, // different node has different label
                                
                                new String[] {"if", "1005", "false", conditionalChoiceValidDistanceElements }, 
                                new String[] {"ifa", "1006", "false", conditionalChoiceValidDistanceElements }, 
                                new String[] {"GuardProcess", "1010", "false", conditionalChoiceValidDistanceElements }, 
                                
                                new String[] {"DataOpPrefix", "1007", "false",dataOpValidDistanceElements }, // different node has different label
                                
                                new String[] {"DefRef", "1008", "false", defRefValidDistanceElements }, // different node has different label
                                new String[] {"EventPrefix", "1009", "false", defRefValidDistanceElements }, // different node has different label
                                
                                new String[] {Constants.HIDING, "1011", "false", hidingValidDistanceElements }, 
                                
                                new String[] {"EDGE", "2000", "false", null}};

            diffParameters.mappingWeights =
                new String[][] {new String[] {Constants.ATOMIC_STARTED, Constants.ATOMIC_STARTED, "1", "true"},
                                new String[] {Constants.CASE, Constants.CASE, "1", "true"},
                                new String[] {"C?", "C?", "1", "true"},
                                new String[] {"?[]", "?[]", "1", "true"},
                                new String[] {"C!", "C!", "1", "true"},
                                new String[] {"if", "if", "1", "true"},
                                new String[] {"ifa", "ifa", "1", "true"},
                                new String[] {"DataOpPrefix", "DataOpPrefix", "1", "true"},
                                new String[] {"DefRef", "DefRef", "1", "true"},
                                new String[] {"EventPrefix", "EventPrefix", "1", "true"},
                                new String[] {"GuardProcess", "GuardProcess", "1", "true"},
                                new String[] {Constants.HIDING, Constants.HIDING, "1", "true"},
                                new String[] {Constants.GENERAL_CHOICE, Constants.GENERAL_CHOICE, "1", "true"},
                                new String[] {Constants.EXTERNAL_CHOICE, Constants.EXTERNAL_CHOICE, "1", "true"},
                                new String[] {Constants.INTERLEAVE, Constants.INTERLEAVE, "1", "true"},
                                new String[] {Constants.INTERLEAVE + "a", Constants.INTERLEAVE + "a", "1", "true"},
                                new String[] {Constants.INTERNAL_CHOICE, Constants.INTERNAL_CHOICE, "1", "true"},
                                new String[] {Constants.PARALLEL, Constants.PARALLEL, "1", "true"},
                                new String[] {Constants.INTERRUPT, Constants.INTERRUPT, "1", "true"},
                                new String[] {Constants.SEQUENTIAL, Constants.SEQUENTIAL, "1", "true"},
                                new String[] {Constants.SKIP, Constants.SKIP, "1", "true"},
                                new String[] {Constants.STOP, Constants.STOP, "1", "true"},
                                new String[] {"EDGE", "EDGE", "1", "true"}};

            if(compareSameSpec)
            {
                diffParameters.distanceThresholdVectors =
                    new String[][]
                    {
                        new String[] {"C!", "C!", "1", "0"},
                        new String[] {"C?", "C?", "1", "0"},
                        
                        new String[] {"?[]", "?[]", "1", "0", "2", "0"},

                        new String[] {"if", "if", "1", "0"},
                        new String[] {"ifa", "ifa", "1", "0"},
                        new String[] {"GuardProcess", "GuardProcess", "1", "0"},

                        new String[] {"DataOpPrefix", "DataOpPrefix","1", "0", "2", "0", "3", "0"}, // hamming distance threshold=0 means only same-label tree nodes can be paired-up
                        
                        new String[] {"DefRef", "DefRef", "2", "0"},
                        new String[] {"EventPrefix", "EventPrefix", "2", "0"}
                    };
            }
            else
            {
                diffParameters.distanceThresholdVectors =
                    new String[][]
                    {
                        new String[] {"C!", "C!", "1", "0"},
                        new String[] {"C?", "C?", "1", "0"},
                        
                        new String[] {"?[]", "?[]", "2", "0"},
                        
                        new String[] {"DataOpPrefix", "DataOpPrefix", "3", "0"}, // hamming distance threshold=0 means only same-label tree nodes can be paired-up
                        
                        new String[] {"DefRef", "DefRef", "2", "0"},
                        new String[] {"EventPrefix", "EventPrefix", "2", "0"}
                    };
            }

            diffParameters.epsilonEdgesExcluded = new String[0][];

            // GenericDiff execution control parameters
            diffParameters.maxIteration = 100;
            diffParameters.noPairupIdenticalNodesWithOthers = false; // value no longer checked in GenericDiff; it is defined for each type and checked insider GraphNode
            diffParameters.stepsOfNeighbors = 1;
            diffParameters.stopGreedySearchThreshold = 0.8;
            diffParameters.stopPropagationThreshold = 0.00001;
            diffParameters.removeOutliersThreshold = 2;
            diffParameters.considerNodeDistance = true;
            diffParameters.considerEdgeDistance = false;
            diffParameters.includeUnmatched = true;
            diffParameters.includeNeighbors = false;
            diffParameters.conductCandidatesSearch = true;
            diffParameters.appendEpsilonPairups = false;
            diffParameters.doGreedySearch = false;
            diffParameters.compareAdditionalInfo = false;
            diffParameters.notMaximalBipartiteMatch = true;

            // Strategy objects
            diffParameters.searchPotentialCandidatesKind = CandidatesSearchStrategyKind.Sequential;
            diffParameters.neighborhoodMatchingStrategyKind = SelectionStrategyKind.StableMarriage;
            diffParameters.removeOutliersStrtegyKind = RemoveOutliersStrategyKind.None;
            diffParameters.matchingStrategyKind = SelectionStrategyKind.StableMarriage;

            // Domain knowledge that controls the parsing and graph creation process
            String[][] simpleNodeInfoTemplates =
                new String[][] {
                                   new String[] {""} // number of outgoing branches
                               };

            double[][] simpleNodeInfoNone =
                new double[][] {
                                   new double[] {0}
                               };

            String[][] simpleNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"}
                               };

            String[][] channelInOutNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[] {""} // node label index
                               };

            double[][] channelInOutNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[] {0}
                               };

            String[][] channelInOutNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"Hamming", "vector"}
                               };

            String[][] channelInGuardedNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[0], // char arrray of guard condition
                                   new String[] {""} // node label index
                               };

            double[][] channelInGuardedNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[0],
                                   new double[] {0}
                               };

            String[][] channelInGuardedNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"SeqLCS", "set"},
                                   new String[] {"Hamming", "vector"}
                               };

            String[][] conditionalChoiceNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[0], // char arrray of guard condition
                               };

            double[][] conditionalChoiceNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[0],
                               };

            String[][] conditionalChoiceNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"SeqLCS", "set"},
                               };

            String[][] dataOpNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[0], // event args such as get.0.1
                                   new String[0], // char array for AssignmentExpr
                                   new String[] {""} // node label index
                               };

            double[][] dataOpNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[0],
                                   new double[0],
                                   new double[] {0}
                               };

            String[][] dataOpNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"SeqHamming", "set"},
                                   new String[] {"SeqLCS", "set"},
                                   new String[] {"Hamming", "vector"}
                               };

            String[][] defRefNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[0], // additional args such as get.0.1
                                   new String[] {""} // node label index
                               };

            double[][] defRefNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[0],
                                   new double[] {0}
                               };

            String[][] defRefNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"SeqHamming", "set"},
                                   new String[] {"Hamming", "vector"}
                               };

            String[][] hidingNodeInfoTemplates =
                new String[][] {
                                   new String[] {""}, // number of outgoing branches
                                   new String[0], // additional args such as get.0.1
                               };

            double[][] hidingNodeInfoNone =
                new double[][] {
                                   new double[] {0},
                                   new double[0],
                               };

            String[][] hidingNodeInfoCalculatorKind =
                new String[][] {
                                   new String[] {"Taxi", "vector"},
                                   new String[] {"SetJaccard", "set"},
                               };

            String[][] edgeInfoTemplates = new String[][] { };

            double[][] edgeInfoNone = new double[][] {};

            String[][] edgeInfoCalculatorKind = new String[][] {};

            graphDataConfigs = new DictionaryJ<String, GraphDataConfiguration>();
            
            GraphDataConfiguration simpleNodeConfig = GraphDataConfiguration.create(simpleNodeInfoTemplates, simpleNodeInfoNone, simpleNodeInfoCalculatorKind, null);
            graphDataConfigs.Add(Constants.ATOMIC_STARTED, simpleNodeConfig);
            graphDataConfigs.Add(Constants.CASE, simpleNodeConfig);
            graphDataConfigs.Add(Constants.GENERAL_CHOICE, simpleNodeConfig);
            graphDataConfigs.Add(Constants.EXTERNAL_CHOICE, simpleNodeConfig);
            graphDataConfigs.Add(Constants.INTERLEAVE, simpleNodeConfig);
            graphDataConfigs.Add(Constants.INTERLEAVE + "a", simpleNodeConfig);
            graphDataConfigs.Add(Constants.INTERNAL_CHOICE, simpleNodeConfig);
            graphDataConfigs.Add(Constants.PARALLEL, simpleNodeConfig);
            graphDataConfigs.Add(Constants.INTERRUPT, simpleNodeConfig);
            graphDataConfigs.Add(Constants.SEQUENTIAL, simpleNodeConfig);
            graphDataConfigs.Add(Constants.SKIP, simpleNodeConfig);
            graphDataConfigs.Add(Constants.STOP, simpleNodeConfig);

            GraphDataConfiguration channelInOutNodeConfig = GraphDataConfiguration.create(channelInOutNodeInfoTemplates, channelInOutNodeInfoNone, channelInOutNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("C?", channelInOutNodeConfig);
            graphDataConfigs.Add("C!", channelInOutNodeConfig);

            GraphDataConfiguration channelInGuardedNodeConfig = GraphDataConfiguration.create(channelInGuardedNodeInfoTemplates, channelInGuardedNodeInfoNone, channelInGuardedNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("?[]", channelInGuardedNodeConfig);

            GraphDataConfiguration conditionalChoiceNodeConfig = GraphDataConfiguration.create(conditionalChoiceNodeInfoTemplates, conditionalChoiceNodeInfoNone, conditionalChoiceNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("if", conditionalChoiceNodeConfig);
            graphDataConfigs.Add("ifa", conditionalChoiceNodeConfig);
            graphDataConfigs.Add("GuardProcess", conditionalChoiceNodeConfig);

            GraphDataConfiguration dataOpNodeConfig = GraphDataConfiguration.create(dataOpNodeInfoTemplates, dataOpNodeInfoNone, dataOpNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("DataOpPrefix", dataOpNodeConfig);

            GraphDataConfiguration defRefNodeConfig = GraphDataConfiguration.create(defRefNodeInfoTemplates, defRefNodeInfoNone, defRefNodeInfoCalculatorKind, null);
            graphDataConfigs.Add("DefRef", defRefNodeConfig);
            graphDataConfigs.Add("EventPrefix", defRefNodeConfig);

            GraphDataConfiguration hidingNodeConfig = GraphDataConfiguration.create(hidingNodeInfoTemplates, hidingNodeInfoNone, hidingNodeInfoCalculatorKind, null);
            graphDataConfigs.Add(Constants.HIDING, hidingNodeConfig);

            GraphDataConfiguration edgeConfig = GraphDataConfiguration.create(edgeInfoTemplates, edgeInfoNone, edgeInfoCalculatorKind, null);
            graphDataConfigs.Add("EDGE", edgeConfig);
        }

        public Graph execute(Side side, ConfigurationBase<ProcessBase, Valuation> config, String graphid)
        {
            CSPConfigurationParser configParser = new CSPConfigurationParser(config, compareParameterizedSystem);

            Graph graph = createGraph(side, configParser, graphDataConfigs, side + "Graph" + graphid);

            graph.setDiffer(diff);

            return graph;
        }
    }
}
