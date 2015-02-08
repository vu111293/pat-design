
using System;
using System.Collections.Generic;
using PAT.GenericDiff.diff;
using PAT.GenericDiff.graph;
using PAT.GenericDiff.select;
using PAT.GenericDiff.utility;

namespace PAT.GenericDiff.test
{
    public interface Parser {
        void parse(List<String[]> nodesData, 
                   List<String[]> edgesData,
                   List<List<double[]>> nodesInfo, 
                   List<List<double[]>> edgesInfo,
                   Dictionary<String, GraphDataConfiguration> graphDataConfigs);
    }

    public  class GraphDataConfiguration {
        public static GraphDataConfiguration create(String[][] elementInfoTemplates, double[][] elementInfoNone, String[][] elementInfoCalculatorKind)  {
            if(elementInfoTemplates.Length != elementInfoNone.Length) {
                throw new Exception(String.Format("elementInfoTemplates.Length %1$s != elementInfoNone.Length %2$s", elementInfoTemplates.Length, elementInfoNone.Length));
            }
            if(elementInfoTemplates.Length != elementInfoCalculatorKind.Length) {
                throw new Exception(String.Format("elementInfoTemplates.Length %1$s != elementInfoCalculatorKind.Length %2$s", elementInfoTemplates.Length, elementInfoCalculatorKind.Length));
            }
			
            for(int index = 0; index < elementInfoTemplates.Length; index++) {
                String[] calculatorKind = elementInfoCalculatorKind[index];
                if(calculatorKind[1].Equals("vector")) {
                    if(elementInfoTemplates[index].Length != elementInfoNone[index].Length) {
                        throw new Exception(String.Format("elementInfoTemplates[%3$S].Length %1$s != elementInfoNone[%3$s].Length %2$s", elementInfoTemplates[index].Length, elementInfoNone[index].Length, index));
                    }
                }
            }
			
            return new GraphDataConfiguration(elementInfoTemplates, elementInfoNone, elementInfoCalculatorKind);
        }

        public String[][] elementInfoTemplates;
        public double[][] elementInfoNone;
        public String[][] elementInfoCalculatorKind;

        public GraphDataConfiguration(String[][] elementInfoTemplates, double[][] elementInfoNone, String[][] elementInfoCalculatorKind)
        {
            this.elementInfoTemplates = elementInfoTemplates;
            this.elementInfoNone = elementInfoNone;
            this.elementInfoCalculatorKind = elementInfoCalculatorKind;
        }
    }

    public class GenericDiffParameters {
        // Parameters for domain semantics
        public String[][] compatibleTypes;
        public String[][] mappingWeights;
        public String[][] distanceThresholdVectors;
        public String[][] epsilonEdgesExcluded;
		
        // Parameters for diff execution control
        public int maxIteration;
        public bool noPairupIdenticalNodesWithOthers;
        public int stepsOfNeighbors;
        public double stopGreedySearchThreshold;
        public double stopPropagationThreshold;
        public double removeOutliersThreshold;
        public bool considerNodeDistance;
        public bool considerEdgeDistance;
        public bool includeUnmatched;
        public bool includeNeighbors;
        public bool conductCandidatesSearch;
        public bool appendEpsilonPairups;
        public bool doGreedySearch;
		
        // Strategy objects for candidates-search, neighborhood-distance-computation, outlier-candidates-removal, and matches-selection
        public CandidatesSearchStrategyKind searchPotentialCandidatesKind;
        public SelectionStrategyKind neighborhoodMatchingStrategyKind;
        public RemoveOutliersStrategyKind removeOutliersStrtegyKind;
        public SelectionStrategyKind matchingStrategyKind;
    }

    public class GraphCreationParameters {
        public  bool createQualifiedGraphNode;
        public String containmentRelation;
        public bool encapsulateInMatchedGraphNode;
    }

    public abstract class ModelComparison {

        public GenericDiffParameters diffParameters = new GenericDiffParameters();
        public GraphCreationParameters graphCreationParameters = new GraphCreationParameters();
	

	
        protected Graph createGraph(Side side, Parser parser, DictionaryJ<String, GraphDataConfiguration> graphDataConfigs, String graphName)  {
            
            Graph graph = new Graph(graphName);
		
            List<String[]> nodesData = new List<String[]>();
            List<String[]> edgesData = new List<String[]>();
            List<List<double[]>> nodesInfo = new List<List<double[]>>();
            List<List<double[]>> edgesInfo = new List<List<double[]>>();
		
            parser.parse(nodesData, edgesData, nodesInfo, edgesInfo, graphDataConfigs);
		
            if(nodesData.Count != nodesInfo.Count) {
                throw new IncomparableException(nodesData.Count, nodesInfo.Count);
            }
		
            if(edgesData.Count != edgesInfo.Count) {
                throw new IncomparableException(edgesData.Count, edgesInfo.Count);
            }
		
            DictionaryJ<String, GraphNode> nodes = new DictionaryJ<String, GraphNode>();
            for(int nodeIndex = 0; nodeIndex < nodesData.Count; nodeIndex++) {
                String[] nodeData = nodesData[nodeIndex];
                String label = nodeData[2] != null ? nodeData[2] : nodeData[1];
                GraphNode node; 
                if(graphCreationParameters.createQualifiedGraphNode) {
                    node = new QualifiedGraphNode(label, nodeData[0], side, diffParameters.noPairupIdenticalNodesWithOthers);
                } else {
                    node = new ConcreteGraphNode(label, nodeData[0], side, diffParameters.noPairupIdenticalNodesWithOthers);				
                }
                ((AbstractGraphElement)node).setAdditionalInfo(nodeData[3]);
			
                List<double[]> nodeInfos = nodesInfo[nodeIndex];
                GraphDataConfiguration nodeConfig = graphDataConfigs.get(nodeData[0]);
                if(nodeInfos.Count != nodeConfig.elementInfoTemplates.Length) {
                    throw new IncomparableException(nodeInfos.Count, nodeConfig.elementInfoTemplates.Length);
                }
			
                if(nodeInfos.Count > 0) {
                    InfoDescriptor[] descriptors = new InfoDescriptor[nodeInfos.Count];
                    for(int infoIndex = 0; infoIndex < nodeInfos.Count; infoIndex++) {
                        double[] characteristicVector = nodeInfos[infoIndex];
                        DistanceCalculatorKind calculatorkind = (DistanceCalculatorKind)Enum.Parse(typeof(DistanceCalculatorKind), nodeConfig.elementInfoCalculatorKind[infoIndex][0]);
                        if(nodeConfig.elementInfoCalculatorKind[infoIndex][1].Equals("vector")) { // VectorBasedLeafInfoDescriptor
                            double[] noneVector = nodeConfig.elementInfoNone[infoIndex];
                            if(characteristicVector.Length != noneVector.Length) {
                                throw new IncomparableException(characteristicVector.Length, noneVector.Length);
                            }
                            descriptors[infoIndex] = VectorBasedLeafInfoDescriptor.create(characteristicVector, noneVector, calculatorkind);
                        } else if(nodeConfig.elementInfoCalculatorKind[infoIndex][1].Equals("set")) { // SetBasedLeafInfoDescriptor
                            descriptors[infoIndex] = new SetBasedLeafInfoDescriptor(characteristicVector, calculatorkind);
                        } else {
                            descriptors[infoIndex] = null; // should not happen. this column is ignore.
                        }
                    }
                    ((ConcreteGraphNode)node).attachInfoDescriptor(new CompositeInfoDescriptor(descriptors));
                }
			
                if(graphCreationParameters.encapsulateInMatchedGraphNode) {
                    node = new MatchedGraphNode((ConcreteGraphNode)node, diffParameters.noPairupIdenticalNodesWithOthers);
                }

                nodes.put(nodeData[1], node);
            }
		
            List<GraphEdge> edges = new List<GraphEdge>(edgesData.Count);
            for(int edgeIndex = 0; edgeIndex < edgesData.Count; edgeIndex++) {
                String[] edgeData = edgesData[edgeIndex];
                GraphNode source = nodes.get(edgeData[2]);
                GraphNode target = nodes.get(edgeData[3]);
                if(source == null || target == null) {
                    continue;
                }
			
                ConcreteGraphEdge edge = new ConcreteGraphEdge(edgeData[1], edgeData[0], source, target);
                edge.setAdditionalInfo(edgeData[4]);

                List<double[]> edgeInfos = edgesInfo[edgeIndex];
                GraphDataConfiguration edgeConfig = graphDataConfigs.get(edgeData[0]);
                if(edgeInfos.Count != edgeConfig.elementInfoTemplates.Length) {
                    throw new IncomparableException(edgeInfos.Count, edgeConfig.elementInfoTemplates.Length);
                }
                if(edgeInfos.Count > 0) {
                    InfoDescriptor[] descriptors = new InfoDescriptor[edgeInfos.Count];
                    for(int infoIndex = 0; infoIndex < edgeInfos.Count; infoIndex++) {
                        double[] characteristicVector = edgeInfos[infoIndex];
                        DistanceCalculatorKind calculatorkind = (DistanceCalculatorKind) Enum.Parse(typeof(DistanceCalculatorKind), edgeConfig.elementInfoCalculatorKind[infoIndex][0]);
                        if(edgeConfig.elementInfoCalculatorKind[infoIndex][1].Equals("vector")) { // VectorBasedLeafInfoDescriptor
                            double[] noneVector = edgeConfig.elementInfoNone[infoIndex];
                            if(characteristicVector.Length != noneVector.Length) {
                                throw new IncomparableException(characteristicVector.Length, noneVector.Length);
                            }
                            descriptors[infoIndex] = VectorBasedLeafInfoDescriptor.create(characteristicVector, noneVector, calculatorkind);
                        }
                        else if (edgeConfig.elementInfoCalculatorKind[infoIndex][1].Equals("set"))
                        { // SetBasedLeafInfoDescriptor
                            descriptors[infoIndex] = new SetBasedLeafInfoDescriptor(characteristicVector, calculatorkind);
                        } else { // should not happen. this InfoDescriptor column is ignored
                            descriptors[infoIndex] = null; 
                        }
                    }
                    edge.attachInfoDescriptor(new CompositeInfoDescriptor(descriptors));
                }
			
                edges.Add(edge);
			
                // TODO Should find a better way
                if(graphCreationParameters.createQualifiedGraphNode && graphCreationParameters.containmentRelation.Equals(edgeData[0])) {
                    QualifiedGraphNode node;
                    if(graphCreationParameters.encapsulateInMatchedGraphNode) {
                        node = (QualifiedGraphNode)((MatchedGraphNode)target).getNode();
                    } else {
                        node = (QualifiedGraphNode)target;
                    }
                    node.setParent(source);
                }
            }
		
        
            //for(Iterator<GraphNode> iter = nodes.values().iterator(); iter.hasNext();) {
            foreach (KeyValuePair<string, GraphNode> pair in nodes)
            {
                graph.addNode(pair.Value);
            }

            foreach (GraphEdge list in edges)
            {
                graph.addEdge(list);
            }
		
		
            //for(Iterator<Set<GraphNode>> iter = graph.getNodes().iterator(); iter.hasNext();) {
            foreach (HashSet<GraphNode> nodesSet in graph.getNodes())
            {
                foreach (GraphNode node in nodesSet)
                {
                    if(!node.hasBackwardEdges() && !node.hasForwardEdges()) {
                        //todo:liuyang
                        //nodesIter.remove();
                    }
                }
			
				
                if(nodesSet.Count == 0) {
                    //todo:liuyang
                    //iter.remove();
                }
            }
		
            return graph;
        }
	
        public abstract void initialize();
    }
}