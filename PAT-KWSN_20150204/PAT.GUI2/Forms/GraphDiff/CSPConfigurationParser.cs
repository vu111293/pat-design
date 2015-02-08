using System;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Ultility;
using PAT.CSP.LTS;
using PAT.GenericDiff.test;
using PAT.GenericDiff.utility;
using Sequence=PAT.CSP.LTS.Sequence;

namespace PAT.GUI.Forms.GraphDiff
{
    public class CSPConfigurationParser : Parser
    {
        private ConfigurationBase<ProcessBase, Valuation> Configuration;

        private bool compareParameterizedSystem;

        private static readonly string separator = "~";

        private static int labelIndex = 1;
        public static DictionaryJ<String, Int32> mapLabelIndex = new DictionaryJ<String, Int32>();

        public static DictionaryJ<Int32, Int32[]> mapLabelIndexCount = new DictionaryJ<int, int[]>();

        public static DictionaryJ<Int32, Int32[]> mapLabelIndexMinCount = new DictionaryJ<int, int[]>();

        public static DictionaryJ<Int32, Int32[]> maplabelIndexFirstCount = new DictionaryJ<int, int[]>();

        public HashSet<string> graphDataGenerated = new HashSet<string>();

        public CSPConfigurationParser(ConfigurationBase<ProcessBase, Valuation> config, bool compareParameterizedSystem)
        {
            this.compareParameterizedSystem = compareParameterizedSystem;
            this.Configuration = config;
        }

        public void parse(List<object[]> nodesData, List<string[]> edgesData, List<List<double[]>> nodesInfo, List<List<double[]>> edgesInfo, DictionaryJ<string, GraphDataConfiguration> graphDataConfigs)
        {
            if (compareParameterizedSystem)
            {
                mapLabelIndexCount.Clear();
                GenerateGraphData(Configuration.Process, true, nodesData, edgesData, nodesInfo, edgesInfo, 0);
            }
            else
            {
                graphDataGenerated.Clear();
                GenerateGraphData(Configuration.Process, null, true, nodesData, edgesData, nodesInfo, edgesInfo); //The root of the configuration tree}
            }
        }

        private void GenerateGraphData(ProcessBase process, bool isRoot, List<object[]> nodesData, List<string[]> edgesData, List<List<double[]>> nodesInfo, List<List<double[]>> edgesInfo, int count)
        {
            string[] nodeData = new string[4];
            nodeData[0] = "NODE";
            nodeData[1] = process.ProcessID;
            nodeData[2] = process.ToString();
            nodeData[3] = process.ProcessID;

            bool validNode = true;
            if (process is DefinitionRef)
            {
                DefinitionRef def = process as DefinitionRef;

                if (isRoot)
                {
                    ProcessBase p = def.GetProcess(Configuration.GlobalEnv);

                    if (def.ProcessID.Equals(p.ProcessID)) // two processIDs will never be unequal
                    {
                        validNode = false;

                        GenerateGraphData(p, isRoot, nodesData, edgesData, nodesInfo, edgesInfo, count);
                    }
                }
            }
            else if (process is IndexInterleaveAbstract)
            {

                IndexInterleaveAbstract interleaveAbstract = process as IndexInterleaveAbstract;

                validNode = false;
                foreach (ProcessBase p in interleaveAbstract.Processes)
                {
                    GenerateGraphData(p, false, nodesData, edgesData, nodesInfo, edgesInfo, interleaveAbstract.ProcessesCounter[p.ProcessID]);
                }
            }

            if (validNode)
            {
                nodesData.Add(nodeData);
                nodesInfo.Add(new List<double[]>());

                String nodeLabel = nodeData[2];

                Int32 index = mapLabelIndex.get(nodeLabel);
                if (0 == index)
                {
                    index = labelIndex++;
                    mapLabelIndex.put(nodeLabel, index);
                }

                int[] labelIndexCount = mapLabelIndexCount.get(index);
                if (labelIndexCount == null)
                {
                    labelIndexCount = new int[1];
                    mapLabelIndexCount.put(index, labelIndexCount);
                }
                labelIndexCount[0] = labelIndexCount[0] + count;

                int[] labelIndexMinCount = mapLabelIndexMinCount.get(index);
                if (labelIndexMinCount == null)
                {
                    labelIndexMinCount = new int[1];
                    mapLabelIndexMinCount.put(index, labelIndexMinCount);
                }
                labelIndexMinCount[0] = Math.Min(labelIndexMinCount[0], count);

                int[] labelIndexFirstCount = maplabelIndexFirstCount.get(index);
                if (labelIndexFirstCount == null)
                {
                    labelIndexFirstCount = new int[1];
                    labelIndexFirstCount[0] = count;
                    maplabelIndexFirstCount.put(index, labelIndexFirstCount);
                }
            }
        }

        // recursively collect the node/edge data/info
        public void GenerateGraphData(ProcessBase process, ProcessBase parentProcess, bool isRoot, List<object[]> nodesData, List<string[]> edgesData, List<List<double[]>> nodesInfo, List<List<double[]>> edgesInfo)
        {
            if(!isRoot && graphDataGenerated.Contains(process.ProcessID))
            {
                return;
            }

            graphDataGenerated.Add(process.ProcessID);

            string[] nodeData = new string[4];
            //nodeData[0] = isRoot;
            nodeData[1] = process.ProcessID;
            //nodeData[3] = process.ProcessID; // set at the end of data generation
            List<double[]> nodeInfos = new List<double[]>(1) {null}; // todo: xingzc. size=2 if parameterized system?

            bool validNode = true;
            bool labelIndexAsNodeInfo = false;
            if (process is AtomicProcess)
            {
                AtomicProcess atomicProcess = process as AtomicProcess;

                nodeData[0] = Constants.ATOMIC_STARTED;
                nodeData[2] = "Atomic"; //Constants.ATOMIC_STARTED;
                
                nodeInfos[0] = new double[] { 1 };

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = atomicProcess.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(atomicProcess.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is CaseProcess)
            {
                CaseProcess caseP = process as CaseProcess;

                nodeData[0] = Constants.CASE;
                nodeData[2] = "Case"; //Constants.CASE;

                nodeInfos[0] = new double[] { caseP.Processes.Length };

                for (int i = 0; i < caseP.Processes.Length; i++)
                {
                    //Expression con = caseP.Conditions[i];
                    ProcessBase p = caseP.Processes[i];

                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = p.ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(p, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }
            else if (process is ChannelOutput)
            {
                ChannelOutput output = process as ChannelOutput;

                nodeData[0] = "C!";
                nodeData[2] = output.ChannelName; // "C!";
                nodeData[3] = nodeData[1] + separator + output.ChannelName;
                labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = output.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(output.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is ChannelInput)
            {
                ChannelInput input = process as ChannelInput;
                
                //foreach (Expression item in input.ExpressionList)
                //{
                //    //s += Common.Ultility.Ultility.GenerateLaTexString(item) + ",";
                //}

                nodeData[0] = "C?";
                nodeData[2] = input.ChannelName; // "C?";
                nodeData[3] = nodeData[1] + separator + input.ChannelName;
                labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };
                //nodeInfos.Add(new double[0]); // a placeholder so that ChannelInput may be paired-up with ChanelInputGuarded
                
                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = input.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(input.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

            }
            else if (process is ChannelInputGuarded)
            {
                ChannelInputGuarded inputGuarded = process as ChannelInputGuarded;

                nodeData[0] = "?[]";
                nodeData[2] = inputGuarded.ChannelName; // "?[]";
                nodeData[3] = nodeData[1] + separator + inputGuarded.ChannelName;
                labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };
                nodeInfos.Add(transformStringToDoubleArray(Common.Ultility.Ultility.GenerateLaTexString(inputGuarded.GuardExpression)));

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = inputGuarded.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(inputGuarded.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is ConditionalChoice)
            {
                ConditionalChoice conditionalChoice = process as ConditionalChoice;

                nodeData[0] = "if";
                nodeData[2] = "if";
                nodeData[3] = nodeData[1] + separator + string.Format("if({0})", Common.Ultility.Ultility.GenerateLaTexString(conditionalChoice.ConditionalExpression));

                nodeInfos[0] = new double[] { 2 };
                nodeInfos.Add(transformStringToDoubleArray(Common.Ultility.Ultility.GenerateLaTexString(conditionalChoice.ConditionalExpression)));

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = conditionalChoice.FirstProcess.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(conditionalChoice.FirstProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

                edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = conditionalChoice.SecondProcess.ProcessID;
                edgesData.Add(edgeData);

                edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(conditionalChoice.SecondProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

            }
            else if (process is ConditionalChoiceAtomic)
            {
                ConditionalChoiceAtomic conditionalChoiceAtomic = process as ConditionalChoiceAtomic;

                nodeData[0] = "ifa";
                nodeData[2] = "ifa";
                nodeData[3] = nodeData[1] + separator + string.Format("ifa({0})", Common.Ultility.Ultility.GenerateLaTexString(conditionalChoiceAtomic.ConditionalExpression));
                
                nodeInfos[0] = new double[] { 2 };
                nodeInfos.Add(transformStringToDoubleArray(Common.Ultility.Ultility.GenerateLaTexString(conditionalChoiceAtomic.ConditionalExpression)));

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = conditionalChoiceAtomic.FirstProcess.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(conditionalChoiceAtomic.FirstProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

                edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = conditionalChoiceAtomic.SecondProcess.ProcessID;
                edgesData.Add(edgeData);

                edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(conditionalChoiceAtomic.SecondProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is GuardProcess)
            {
                GuardProcess guard = process as GuardProcess;

                nodeData[0] = "GuardProcess";
                nodeData[2] = "[-]"; //+Common.Ultility.Ultility.GenerateLaTexString(guard.Condition) + "]";
                nodeData[3] = nodeData[1] + separator + string.Format("[{0}]", Common.Ultility.Ultility.GenerateLaTexString(guard.Condition));
                //labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };
                nodeInfos.Add(transformStringToDoubleArray(Common.Ultility.Ultility.GenerateLaTexString(guard.Condition))); // todo: xingzc. information extracted from guard.Condition

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = guard.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(guard.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is DataOperationPrefix)
            {
                DataOperationPrefix prefix = process as DataOperationPrefix;

                nodeData[0] = "DataOpPrefix";
                nodeData[2] = prefix.Event.BaseName + "{-}"; // GenerateProcessLaTexString(prefix.Event) + "{" + prefix.AssignmentExpr + "}";}
                nodeData[3] = nodeData[1] + separator + GenerateProcessLaTexString(prefix.Event) + "{" + Common.Ultility.Ultility.GenerateLaTexString(prefix.AssignmentExpr) + "}";
                labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };
                String[] exprs = prefix.Event.FullName.Split('.');
                double[] exprsInfo = new double[exprs.Length - 1];
                for (int i = 1; i < exprs.Length; i++)
                {
                    //String expr = Common.Ultility.Ultility.GenerateLaTexString(prefix.Event.ExpressionList[i]);
                    exprsInfo[i - 1] = double.Parse(exprs[i]);
                }
                nodeInfos.Add(exprsInfo); //todo: xingzc. args as nodeinfo

                nodeInfos.Add(transformStringToDoubleArray(Common.Ultility.Ultility.GenerateLaTexString(prefix.AssignmentExpr))); // todo: xingzc. infomation extracted from prefixAssignmentExpr

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = prefix.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(prefix.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is DefinitionRef)
            {
                DefinitionRef def = process as DefinitionRef;

                nodeData[0] = "DefRef";
                //nodeData[2] = def.Name + "(" + Common.Classes.Ultility.Ultility.PPStringList(def.Args) + ")";
                nodeData[2] = def.Name;
                nodeData[3] = nodeData[1] + "~" + def.Name + "(" + Common.Classes.Ultility.Ultility.PPStringList(def.Args) + ")";
                labelIndexAsNodeInfo = true;

                //if (!(parentProcess is EventPrefix))
                if (isRoot)
                {
                    ProcessBase p = def.GetProcess(Configuration.GlobalEnv);

                    if (def.ProcessID.Equals(p.ProcessID))
                    {
                        validNode = false;

                        nodeInfos[0] = new double[] { 0 };
                        nodeInfos.Add(new double[0]);

                        GenerateGraphData(p, parentProcess, isRoot, nodesData, edgesData, nodesInfo, edgesInfo);    
                    } 
                    else
                    {
                        // todo: xingzc. this should never happen, since def.ProcessID is always equal to p.ProcessID
                        Debug.Assert(false, "def.ProcessID should be always equal to p.ProcessId");
                        nodeInfos[0] = new double[] { 1 };

                        String[] edgeData = new String[5];
                        edgeData[0] = "EDGE";
                        edgeData[1] = "";
                        edgeData[2] = process.ProcessID;
                        edgeData[3] = p.ProcessID;
                        edgesData.Add(edgeData);

                        List<double[]> edgeInfos = new List<double[]>(0);
                        edgesInfo.Add(edgeInfos);

                        GenerateGraphData(p, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                    }
                }
                else
                {
                    nodeInfos[0] = new double[] { 0 };

                    String argsStr = Common.Classes.Ultility.Ultility.PPStringList(def.Args);

                    double[] argsInfo = new double[def.Args.Length];
                    for (int i = 0; i < def.Args.Length; i++)
                    {
                        String arg = Common.Ultility.Ultility.GenerateLaTexString(def.Args[i]);
                        argsInfo[i] = double.Parse(arg);
                    }

                    nodeInfos.Add(argsInfo); // todo: xingzc. information extracted from def.Args. See PPStringList(def.Args)
                }
            }
            else if (process is EventPrefix)
            {
                EventPrefix prefix = process as EventPrefix;

                nodeData[0] = "EventPrefix";
                //nodeData[2] = GenerateProcessLaTexString(prefix.Event); // todo: xingzc. can it be a template?
                nodeData[2] = prefix.Event.BaseName;
                nodeData[3] = nodeData[1] + "~" + GenerateProcessLaTexString(prefix.Event);
                labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };

                // prefix.Event.ExpressionList is null
                //double[] exprsInfo = new double[prefix.Event.ExpressionList.Length];
                String[] exprs = prefix.Event.FullName.Split('.');
                double[] exprsInfo = new double[exprs.Length - 1];
                for (int i = 1; i < exprs.Length; i++)
                {
                    //String expr = Common.Ultility.Ultility.GenerateLaTexString(prefix.Event.ExpressionList[i]);
                    exprsInfo[i - 1] = double.Parse(exprs[i]);
                }
                nodeInfos.Add(exprsInfo); //todo: xingzc. args as nodeinfo
                
                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = prefix.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(prefix.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is Hiding)
            {
                Hiding hiding = process as Hiding;

                nodeData[0] = Constants.HIDING;
                nodeData[2] = "Hiding"; //Constants.HIDING; // +"{" + LaTexPPStringListDot(hiding.HidingAlphabets) + "}";
                nodeData[3] = nodeData[1] + "{" + LaTexPPStringListDot(hiding.HidingAlphabets) + "}";
                //labelIndexAsNodeInfo = true;

                nodeInfos[0] = new double[] { 1 };

                List<double> eventInfo = new List<double>();
				foreach (Event hidingEvent in hiding.HidingAlphabets)
                {
                    int eventIndex = mapLabelIndex.get(hidingEvent.FullName);
                    if (0 == eventIndex)
                    {
                        eventIndex = labelIndex++;
                        mapLabelIndex.put(hidingEvent.FullName, eventIndex);
                    }
					eventInfo.Add(eventIndex);
                }
                nodeInfos.Add(eventInfo.ToArray()); // todo: xingzc. information extracted from hiding.HidingAlpahbets
                
                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = hiding.Process.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(hiding.Process, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is IndexChoice)
            {
                IndexChoice choice = process as IndexChoice;

                nodeData[0] = Constants.GENERAL_CHOICE;
                nodeData[2] = "GC";  //Constants.GENERAL_CHOICE;

                nodeInfos[0] = new double[] { choice.Processes.Count };

                for (int i = 0; i < choice.Processes.Count; i++)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = choice.Processes[i].ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(choice.Processes[i], process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }else if (process is IndexExternalChoice)
            {
                IndexExternalChoice choice = process as IndexExternalChoice;

                nodeData[0] = Constants.EXTERNAL_CHOICE;
                nodeData[2] = "IEC"; // Constants.EXTERNAL_CHOICE;

                nodeInfos[0] = new double[] { choice.Processes.Count };

                for (int i = 0; i < choice.Processes.Count; i++)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = choice.Processes[i].ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(choice.Processes[i], process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }

            else if (process is IndexInterleave)
            {
                IndexInterleave interleave = process as IndexInterleave;

                nodeData[0] = Constants.INTERLEAVE;
                nodeData[2] = "IL"; //Constants.INTERLEAVE;

                nodeInfos[0] = new double[] { interleave.Processes.Count };

                for (int i = 0; i < interleave.Processes.Count; i++)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = interleave.Processes[i].ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(interleave.Processes[i], process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }

            else if (process is IndexInterleaveAbstract)
            {
                /*
                 * IndexInterleaveAbstract is the process counter representation for parameterized systems.
                 * Parsing parameterized systems uses another GenerateData(...). Thus, this branck should
                 * not be entered.
                 */
                Debug.Assert(false);
                IndexInterleaveAbstract interleaveAbstract = process as IndexInterleaveAbstract;

                nodeData[0] = Constants.INTERLEAVE + "a";
                nodeData[2] = "ILA"; //Constants.INTERLEAVE + "a";

                nodeInfos[0] = new double[] { interleaveAbstract.Processes.Count };
                //nodeInfos[1] = new double[] interleaveAbstract.ProcessesCounter[p.ProcessID] // todo: xingzc.

                foreach (ProcessBase p in interleaveAbstract.Processes)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = interleaveAbstract.ProcessesCounter[p.ProcessID].ToString();
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = p.ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(p, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }
            else if (process is IndexInternalChoice)
            {
                IndexInternalChoice parallel = process as IndexInternalChoice;

                nodeData[0] = Constants.INTERNAL_CHOICE;
                nodeData[2] = "IIC";  //Constants.INTERNAL_CHOICE;

                nodeInfos[0] = new double[] { parallel.Processes.Count };

                for (int i = 0; i < parallel.Processes.Count; i++)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = parallel.Processes[i].ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(parallel.Processes[i], process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }
            else if (process is IndexParallel)
            {
                IndexParallel parallel = process as IndexParallel;

                nodeData[0] = Constants.PARALLEL;
                nodeData[2] = "Parallel"; //Constants.PARALLEL;

                nodeInfos[0] = new double[] { parallel.Processes.Count };

                for (int i = 0; i < parallel.Processes.Count; i++)
                {
                    String[] edgeData = new String[5];
                    edgeData[0] = "EDGE";
                    edgeData[1] = "";
                    edgeData[2] = process.ProcessID;
                    edgeData[3] = parallel.Processes[i].ProcessID;
                    edgesData.Add(edgeData);

                    List<double[]> edgeInfos = new List<double[]>(0);
                    edgesInfo.Add(edgeInfos);

                    GenerateGraphData(parallel.Processes[i], process, false, nodesData, edgesData, nodesInfo, edgesInfo);
                }
            }
            else if (process is Interrupt)
            {
                Interrupt interrupt = process as Interrupt;

                nodeData[0] = Constants.INTERRUPT;
                nodeData[2] = "Interrupt"; //Constants.INTERRUPT;

                nodeInfos[0] = new double[] { 2 };

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = interrupt.FirstProcess.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(interrupt.FirstProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

                edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = interrupt.SecondProcess.ProcessID;
                edgesData.Add(edgeData);

                edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(interrupt.SecondProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

            }
            else if (process is Sequence)
            {
                Sequence sequence = process as Sequence;

                nodeData[0] = Constants.SEQUENTIAL;
                nodeData[2] = "Seq"; //Constants.SEQUENTIAL;

                nodeInfos[0] = new double[] { 2 };

                String[] edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = sequence.FirstProcess.ProcessID;
                edgesData.Add(edgeData);

                List<double[]> edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(sequence.FirstProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);

                edgeData = new String[5];
                edgeData[0] = "EDGE";
                edgeData[1] = "";
                edgeData[2] = process.ProcessID;
                edgeData[3] = sequence.SecondProcess.ProcessID;
                edgesData.Add(edgeData);

                edgeInfos = new List<double[]>(0);
                edgesInfo.Add(edgeInfos);

                GenerateGraphData(sequence.SecondProcess, process, false, nodesData, edgesData, nodesInfo, edgesInfo);
            }
            else if (process is Skip)
            {
                nodeData[0] = Constants.SKIP;
                nodeData[2] = "Skip"; //Constants.SKIP;
                nodeInfos[0] = new double[] { 0 };
            }
            else if (process is Stop)
            {
                nodeData[0] = Constants.STOP;
                nodeData[2] = "Stop"; // Constants.STOP;
                nodeInfos[0] = new double[] { 0 };
            }

            if(nodeData[3] == null)
            {
                nodeData[3] = nodeData[1] + "~" + nodeData[2];
            }
            String nodeLabel = nodeData[2];
            Int32 index = mapLabelIndex.get(nodeLabel);
            if (0 == index)
            {
                index = labelIndex++;
                mapLabelIndex.put(nodeLabel, index);
            }
            if(labelIndexAsNodeInfo)
            {
                nodeInfos.Add(new double[] { index }); 
            }

            if (validNode)
            {
                nodesInfo.Add(nodeInfos);
                nodesData.Add(nodeData);
            }
        }

        public static string GenerateProcessLaTexString(Event evt)
        {
            string toReturn;
            if (evt.FullName == null)
            {
                toReturn = evt.BaseName + LaTexPPStringListDot(evt.ExpressionList);
            }
            else
            {
                toReturn = evt.FullName;
            }

            toReturn = toReturn.Replace("\u03C4", "\\tau");

            switch (evt.FairnessLabelType)
            {
                case FairnessLabelType.Normal:
                    return toReturn;
                case FairnessLabelType.StrongLive:
                    return "sl(" + toReturn + ")";
                case FairnessLabelType.WeakLive:
                    return "wl(" + toReturn + ")";
                case FairnessLabelType.StrongFair:
                    return "sf(" + toReturn + ")";
                case FairnessLabelType.WeakFair:
                    return "wf(" + toReturn + ")";
                default:
                    throw new Exception();
            }
        }

        public static string LaTexPPStringListDot(Expression[] list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            foreach (Expression item in list)
            {
                s += "." + Common.Ultility.Ultility.GenerateLaTexString(item);
            }
            return s;
        }

        public static string LaTexPPStringListDot(List<Event> list)
        {
            if (list == null)
            {
                return "";
            }

            string s = "";
            foreach (Event item in list)
            {
                s += "." + GenerateProcessLaTexString(item);
            }
            return s;
        }

        private static double[] transformStringToDoubleArray(string str)
        {
            double[] doubleArray = new double[str.Length];
            for (int charIndex = 0; charIndex < str.Length; charIndex++)
            {
                doubleArray[charIndex] = str[charIndex];
            }

            return doubleArray;
        }
    }
}
