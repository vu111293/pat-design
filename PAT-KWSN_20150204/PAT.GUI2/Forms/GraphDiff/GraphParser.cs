using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Msagl.Drawing;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.CSP.LTS;
using PAT.GenericDiff.graph;
using PAT.GenericDiff.test;
using PAT.GenericDiff.utility;
using Graph=Microsoft.Msagl.Drawing.Graph;

namespace PAT.GUI.Forms.GraphDiff
{
	public class PATModelFileParser : Parser {
        private Graph Graph;
        private PAT.GenericDiff.graph.Side side;

        private ConfigurationGraphBuilder configGraphBuilder;

        private static int transitionLabelIndex = 1;
        private static DictionaryJ<String, Int32> mapTranslabelIndex = new DictionaryJ<String, Int32>();

        // infinity and cutnumber is only used when compareParameterizedSystem=true
	    private bool compareParameterizedSystem;
		private int infinity;
		private int cutnumber;

        private bool compareConfigGraph;

        public PATModelFileParser(Graph graph, PAT.GenericDiff.graph.Side side, bool compareParameterizedSystem, bool compareConfigGraph, bool matchProcessParameters, int infinity, int cutnumber)
        {
            this.Graph = graph;
            this.side = side;

            this.configGraphBuilder = new ConfigurationGraphBuilder(compareParameterizedSystem, matchProcessParameters);

            this.compareParameterizedSystem = compareParameterizedSystem;
            this.compareConfigGraph = compareConfigGraph;
			this.infinity = infinity;
			this.cutnumber = cutnumber;
		}

	    public void parse(List<object[]> nodesData, List<string[]> edgesData, List<List<double[]>> nodesInfo, List<List<double[]>> edgesInfo, DictionaryJ<string, GraphDataConfiguration> graphDataConfigs)
        {
            try
            {
                DictionaryJ<String, object[]> nodesMap = new DictionaryJ<String, object[]>();
                foreach (Edge edge in Graph.Edges)
                {
                    String sourceNodeId = edge.SourceNode.LabelText;
                    String targetNodeId = edge.TargetNode.LabelText;

                    object[] sourceNodeData; // length=4 [1:type, 2:id, 3;label, 4:additionalinfo]
                    if (!string.IsNullOrEmpty(edge.LabelText) && !nodesMap.ContainsKey(sourceNodeId))
                    {
                        EventStepSim userData = edge.SourceNode.UserData as EventStepSim;
                        sourceNodeData = new object[4];
                        sourceNodeData[0] = "REGULAR";
                        sourceNodeData[1] = sourceNodeId;
                        sourceNodeData[2] = String.Format("[{0}] {1}", sourceNodeId, userData.ToString());
                        PAT.GenericDiff.graph.Graph configGraph = configGraphBuilder.execute(this.side, userData.Config, sourceNodeId);
                        sourceNodeData[3] = configGraph;
                        nodesData.Add(sourceNodeData);

                        nodesInfo.Add(extractNodeInfos(userData.Config.GlobalEnv, configGraph, graphDataConfigs["REGULAR"].elementInfoTemplates));

                        nodesMap.put(sourceNodeId, sourceNodeData);
                    }

                    object[] targetNodeData; // length=4 [1:type, 2:id, 3;label, 4:additionalinfo]
                    if (!nodesMap.ContainsKey(targetNodeId))
                    {
                        EventStepSim userData = edge.TargetNode.UserData as EventStepSim;
                        targetNodeData = new object[4];
                        targetNodeData[0] = !string.IsNullOrEmpty(edge.LabelText) ? "REGULAR" : "INIT";
                        targetNodeData[1] = targetNodeId;
                        targetNodeData[2] = String.Format("[{0}] {1}", targetNodeId, userData.ToString());
                        PAT.GenericDiff.graph.Graph configGraph = configGraphBuilder.execute(this.side, userData.Config, targetNodeId);
                        targetNodeData[3] = configGraph;
                        nodesData.Add(targetNodeData);

                        nodesInfo.Add(extractNodeInfos(userData.Config.GlobalEnv, configGraph, graphDataConfigs[!string.IsNullOrEmpty(edge.LabelText) ? "REGULAR" : "INIT"].elementInfoTemplates));

                        nodesMap.put(targetNodeId, targetNodeData);
                    }

                    if (!string.IsNullOrEmpty(edge.LabelText)) {
                        String[] edgeData = new String[5];
                        edgeData[0] = "TRANSITION";
                        edgeData[1] = edge.LabelText;
                        edgeData[2] = sourceNodeId;
                        edgeData[3] = targetNodeId;
                        edgesData.Add(edgeData);
                        edgesInfo.Add(extractEdgeInfos(edge.LabelText, graphDataConfigs["TRANSITION"].elementInfoTemplates));
                    }

                }
            }
            catch (Exception e)
            {
                //e.printStackTrace();
            }

            if(!compareConfigGraph)
            {
                if(compareParameterizedSystem)
                {
                    adjustNodeInfosForParameterizedSystems(nodesInfo, graphDataConfigs);
                }
                else
                {
                    adjustNodeInfosRegular(nodesInfo, graphDataConfigs);
                }
            }
        }

	    private void adjustNodeInfosRegular(List<List<double[]>> nodesInfo, DictionaryJ<string, GraphDataConfiguration> graphDataConfigs)
	    {
            foreach (List<double[]> nodeInfos in nodesInfo)
            {
                double[] counters = nodeInfos[nodeInfos.Count - 1]; // label info vector always the last one in nodeInfos list
                double[] newcounters = new double[CSPConfigurationParser.mapLabelIndex.Count];
                int infoIndex = 0;
                foreach (int labelIndex in CSPConfigurationParser.mapLabelIndex.Values)
                {
                    double value = 0;
                    for (int counterIndex = 0; counterIndex < counters.Length; counterIndex = counterIndex + 2)
                    {
                        if (counters[counterIndex] == labelIndex)
                        {
                            value = counters[counterIndex + 1];
                            break;
                        }
                    }

                    newcounters[infoIndex] = value;
                    infoIndex++;
                }

                nodeInfos[nodeInfos.Count - 1] = newcounters;
            }

            double[] nodeInfoNone = new double[CSPConfigurationParser.mapLabelIndex.Count];

            GraphDataConfiguration dataConfig = graphDataConfigs.get("REGULAR");
            dataConfig.elementInfoNone[dataConfig.elementInfoNone.Length - 1] = nodeInfoNone;

            dataConfig = graphDataConfigs.get("INIT");
            dataConfig.elementInfoNone[dataConfig.elementInfoNone.Length - 1] = nodeInfoNone;
	    }

	    private void adjustNodeInfosForParameterizedSystems(List<List<double[]>> nodesInfo, DictionaryJ<string, GraphDataConfiguration> graphDataConfigs)
	    {
            foreach (List<double[]> nodeInfos in nodesInfo)
            {
                double[] counters = nodeInfos[nodeInfos.Count - 1]; // label info vector always the last one in nodeInfos list
                double[] newcounters = new double[CSPConfigurationParser.mapLabelIndexMinCount.Count];
                int infoIndex = 0;
                foreach (int labelIndex in CSPConfigurationParser.mapLabelIndexMinCount.Keys)
                {
                    double value = 0;
                    for (int counterIndex = 0; counterIndex < counters.Length; counterIndex = counterIndex+2)
                    {
                        if(counters[counterIndex] == labelIndex)
                        {
                            value = counters[counterIndex + 1];
                            break;
                        }
                    }

                    int[] labelIndexFirstCount = CSPConfigurationParser.maplabelIndexFirstCount.get(labelIndex);
                    int[] labelIndexMinCount = CSPConfigurationParser.mapLabelIndexMinCount.get(labelIndex);

                    if (value == -1)
                    {
                        newcounters[infoIndex] = infinity;
                    }
                    else
                    {
                        if (labelIndexMinCount[0] == -1)
                        {
                            newcounters[infoIndex] = Math.Abs(cutnumber - value);
                        }
                        else if (labelIndexMinCount[0] == 0)
                        {
                            newcounters[infoIndex] = value;
                        }
                        else
                        {
                            throw new Exception(String.Format("Invalid LabelIndex MinCount {0}", labelIndexMinCount[0]));
                        }
                    }

                    infoIndex++;
                }

                nodeInfos[nodeInfos.Count - 1] = newcounters;
                
                /*for(int index = 0; index < counters.Length; index=index+2)
                {
                    int[] labelIndexFirstCount = CSPConfigurationParser.maplabelIndexFirstCount.get((int)counters[index]);
                    int[] labelIndexMinCount = CSPConfigurationParser.mapLabelIndexMinCount.get((int)counters[index]);
                    if (counters[index + 1] == -1)
                    {
                        counters[index + 1] = infinity;
                    }
                    else
                    {
                        if (labelIndexMinCount[0] == -1)
                        {
                            counters[index + 1] = Math.Abs(cutnumber - counters[index + 1]);
                            if (labelIndexFirstCount[0] == -1)
                            {
                                // do nothing
                            }
                            else if (labelIndexFirstCount[0] > 0)
                            {
                                //counters[index + 1] = Math.Abs(cutnumber - counters[index + 1]);
                            }
                            else
                            {
                                throw new Exception(String.Format("Invalid LabelIndex FirstCount {0}", labelIndexFirstCount[0]));
                            }
                        }
                        else if (labelIndexMinCount[0] == 0)
                        {
                            // do nothing
                        }
                        else
                        {
                            throw new Exception(String.Format("Invalid LabelIndex MinCount {0}", labelIndexMinCount[0]));
                        }
                    }
                }*/
            }

            double[] nodeInfoNone = new double[CSPConfigurationParser.mapLabelIndexMinCount.Count];
            int infoNoneIndex = 0;
            foreach (int index in CSPConfigurationParser.mapLabelIndexMinCount.Keys)
            {
                int[] labelIndexFirstCount = CSPConfigurationParser.maplabelIndexFirstCount.get(index);
                int[] labelIndexMinCount = CSPConfigurationParser.mapLabelIndexMinCount.get(index);

                //nodeInfoNone[infoNoneIndex] = index;
                if (labelIndexMinCount[0] == -1)
                {
                    if (labelIndexFirstCount[0] == -1)
                    {
                        nodeInfoNone[infoNoneIndex] = infinity;
                    }
                    else if (labelIndexFirstCount[0] > 0)
                    {
                        nodeInfoNone[infoNoneIndex] = cutnumber;
                    }
                    else
                    {
                        throw new Exception(String.Format("Invalid LabelIndex FirstCount {0}", labelIndexFirstCount[0]));
                    }
                }
                else if (labelIndexMinCount[0] == 0)
                {
                    nodeInfoNone[infoNoneIndex] = 0;
                }
                else
                {
                    throw new Exception(String.Format("Invalid LabelIndex MinCount {0}", labelIndexMinCount[0]));
                }
                infoNoneIndex = infoNoneIndex + 1;
            }

            GraphDataConfiguration dataConfig = graphDataConfigs.get("REGULAR");
            dataConfig.elementInfoNone[dataConfig.elementInfoNone.Length - 1] = nodeInfoNone;

            dataConfig = graphDataConfigs.get("INIT");
            dataConfig.elementInfoNone[dataConfig.elementInfoNone.Length - 1] = nodeInfoNone;
        }

        private List<double[]> extractEdgeInfos(String transitionLabel, String[][] edgeInfoTemplates) 
        {
            List<double[]> edgeInfos = new List<double[]>(1);

            transitionLabel = transitionLabel.Trim(new char[] {'[', ']'}); // for hidden events
	        String[] labels = transitionLabel.Split('.');
            Int32 index = mapTranslabelIndex.get(labels[0]);
            if (0 == index)
            {
                index = transitionLabelIndex++;
                mapTranslabelIndex.put(labels[0], index);
            }
            edgeInfos.Add(new double[] { index });

            double[] values = new double[labels.Length - 1];
	        for (int i = 1; i < labels.Length; i++)
	        {
	            values[i - 1] = double.Parse(labels[i]);
	        } 
            edgeInfos.Add(values);
			
			return edgeInfos;
		}

        private List<double[]> extractNodeInfos(Valuation globalVars, PAT.GenericDiff.graph.Graph configGraph, String[][] nodeInfoTemplates)
        {
            List<double[]> nodeInfos = new List<double[]>(nodeInfoTemplates.Length);
            for(int i = 0; i < nodeInfoTemplates.Length; i++)
            {
                nodeInfos.Add(null);
            }

            if (globalVars != null)
            {
                if (globalVars.Variables != null)
                {
                    foreach (StringDictionaryEntryWithKey<ExpressionValue> pair in globalVars.Variables._entries)
                    {
                        if (pair != null)
                        {
                            string varName = pair.Key;

                            int i = 0;
                            int j = 0;
                            for(; i < nodeInfoTemplates.Length; i++)
                            {
                                String[] keys = nodeInfoTemplates[i];
                                for (j = 0; j < keys.Length; j++)
                                {
                                    if(keys[j].Equals(varName))
                                    {
                                        break;
                                    }
                                }
                                if(j < keys.Length)
                                {
                                    break;

                                }
                            }

                            if(i < nodeInfoTemplates.Length)
                            {
                                double value = Double.NaN;
                                ExpressionValue varValue = pair.Value;
                                if (varValue is RecordValue)
                                {
                                    // todo: xingzc. need to get array length and values in the array
                                    RecordValue array = varValue as RecordValue;
                                    ExpressionValue[] values = array.Associations;
                                    nodeInfos[i] = new double[values.Length];
                                    for (int valueIndex = 0; valueIndex < values.Length; valueIndex++)
                                    {
                                        if(values[valueIndex] is IntConstant)
                                        {
                                            nodeInfos[i][valueIndex] = (values[valueIndex] as IntConstant).Value;
                                        } 
                                        else if(values[valueIndex] is BoolConstant)
                                        {
                                            nodeInfos[i][valueIndex] = (values[valueIndex] as BoolConstant).Value ? 1.0 : 0.0;
                                        }
                                        else
                                        {
                                            // todo: xingzc. process other primitive types
                                        }
                                    }
                                }
                                else if (false)
                                {
                                    // todo: xingzc. process other complex data type
                                }
                                else
                                {
                                    if (varValue is IntConstant)
                                    {
                                        value = (varValue as IntConstant).Value;
                                    }
                                    else if (varValue is BoolConstant)
                                    {
                                        value = (varValue as BoolConstant).Value ? 1.0 : 0.0;
                                    }
                                    else
                                    {
                                        // todo: xingzc. process other primitive types
                                    }

                                    if (nodeInfos[i] == null)
                                    {
                                        nodeInfos[i] = new double[nodeInfoTemplates[i].Length];
                                    }

                                    if (!Double.IsNaN(value))
                                    {
                                        nodeInfos[i][j] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!compareConfigGraph)
            {
                DictionaryJ<int, int[]> mapLabelindexCount = new DictionaryJ<int, int[]>();
                List<String> nodeLabels = configGraph.getNodelabels();
                foreach (String nodeLabel in nodeLabels)
                {
                    int index = CSPConfigurationParser.mapLabelIndex.get(nodeLabel);
                    if (0 != index) // index should not be 0 according to the way to build configGraph
                    {
                        int[] count = mapLabelindexCount.get(index);
                        if (null == count)
                        {
                            count = new int[1];
                            mapLabelindexCount.put(index, count);
                        }
                        int[] labelIndexCount = CSPConfigurationParser.mapLabelIndexCount.get(index); // shold never be null
                        if (labelIndexCount != null)
                        {
                            count[0] = count[0] + labelIndexCount[0];
                        }
                        else
                        {
                            count[0]++;
                        }
                    }
                }

                int infoIndex = 0;
                double[] labelindexCount = new double[mapLabelindexCount.Count * 2];
                foreach (int labelindex in mapLabelindexCount.Keys)
                {
                    int[] count = mapLabelindexCount.get(labelindex);
                    if (null != count) // count should not be null
                    {
                        labelindexCount[infoIndex] = labelindex;
                        labelindexCount[infoIndex + 1] = count[0];
                    }
                    infoIndex = infoIndex + 2;
                }

                nodeInfos[nodeInfoTemplates.Length - 1] = labelindexCount;
            }

            return nodeInfos;
		}		
	}
}
