using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    public partial class AutomataBDD
    {
        /// <summary>
        /// Return AutomataBDD of Interleave process
        /// </summary>
        /// <param name="processes">List of AutomataBDD of interleaving processes</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static AutomataBDD Interleave(List<AutomataBDD> processes, Model model)
        {
            AutomataBDD result = new AutomataBDD();
            
            InterleaveSetVariable(processes, result);
            InterleaveSetInit(processes, result);
            InterleaveEncodeTransition(processes, model, result);
            InterleaveEncodeChannel(processes, model, result);

            //
            return result;
        }

        /// <summary>
        /// ∪ Pi.var
        /// </summary>
        private static void InterleaveSetVariable(List<AutomataBDD> processes, AutomataBDD result)
        {
            foreach (AutomataBDD process in processes)
            {
                result.variableIndex.AddRange(process.variableIndex);
            }
        }

        /// <summary>
        /// ∧ Pi.init
        /// </summary>
        private static void InterleaveSetInit(List<AutomataBDD> processes, AutomataBDD result)
        {
            result.initExpression = processes[0].initExpression;
            for (int i = 1; i < processes.Count; i++)
            {
                result.initExpression = Expression.AND(result.initExpression, processes[i].initExpression);
            }
        }

        /// <summary>
        ///  ∨ (Pi.transition ∧ unchanged.i)
        /// [ REFS: '', DEREFS: 'Pi.transitionBDD, priorityTransitionsBDD']
        /// </summary>
        private static void InterleaveEncodeTransition(List<AutomataBDD> processes, Model model, AutomataBDD result)
        {
            foreach (var process in processes)
            {
                List<int> unchangedVariableIndex = new List<int>(result.variableIndex);
                foreach (int index in process.variableIndex)
                {
                    unchangedVariableIndex.Remove(index);
                }

                process.transitionBDD = new List<CUDDNode>() {CUDD.Function.Or(process.transitionBDD)};
                result.transitionBDD.AddRange(model.AddVarUnchangedConstraint(process.transitionBDD, unchangedVariableIndex));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processes"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void InterleaveEncodeChannel(List<AutomataBDD> processes, Model model, AutomataBDD result)
        {
            for (int i = 0; i < processes.Count; i++)
            {
                List<int> unchangedVariableIndex = new List<int>(result.variableIndex);
                foreach (int index in processes[i].variableIndex)
                {
                    unchangedVariableIndex.Remove(index);
                }

                //Channel communication implicitly is not terminate transition
                CUDD.Ref(processes[i].channelInTransitionBDD);
                result.channelInTransitionBDD.AddRange(model.AddVarUnchangedConstraint(processes[i].channelInTransitionBDD, unchangedVariableIndex));

                CUDD.Ref(processes[i].channelOutTransitionBDD);
                result.channelOutTransitionBDD.AddRange(model.AddVarUnchangedConstraint(processes[i].channelOutTransitionBDD, unchangedVariableIndex));
            }

            InterleaveGetChannelTransition(processes, model, result);
        }

        /// <summary>
        /// Get new Transition from Channel input, channel ouput of sub processes
        /// [ REFS: '', DEREFS: 'processes.channelInTransitionBDD, channelOutTransitionBDD']
        /// </summary>
        /// <param name="processes"></param>
        /// <param name="model"></param>
        /// <param name="result"></param>
        private static void InterleaveGetChannelTransition(List<AutomataBDD> processes, Model model, AutomataBDD result)
        {
            List<CUDDNode> syncedChannelTransition = new List<CUDDNode>();

            //Make sure only 2 different processes can join the synchronized channel event
            for (int i = 0; i < processes.Count; i++)
            {
                for (int j = i + 1; j < processes.Count; j++)
                {
                    if (processes[i].channelInTransitionBDD.Count > 0 && processes[j].channelOutTransitionBDD.Count > 0)
                    {
                        //process i Channel Input, process j Channel Output
                        CUDD.Ref(processes[i].channelInTransitionBDD);
                        CUDD.Ref(processes[j].channelOutTransitionBDD);
                        syncedChannelTransition.AddRange(CUDD.Function.And(processes[i].channelInTransitionBDD, processes[j].channelOutTransitionBDD));
                    }

                    if (processes[i].channelOutTransitionBDD.Count > 0 && processes[j].channelInTransitionBDD.Count > 0)
                    {
                        //Process i Channel Output, Process j Channel Input
                        CUDD.Ref(processes[i].channelOutTransitionBDD);
                        CUDD.Ref(processes[j].channelInTransitionBDD);
                        syncedChannelTransition.AddRange(CUDD.Function.And(processes[i].channelOutTransitionBDD, processes[j].channelInTransitionBDD));
                    }
                }
            }

            //Dereference the Channel input, channel output of all sub processes
            foreach (AutomataBDD process in processes)
            {
                CUDD.Deref(process.channelInTransitionBDD, process.channelOutTransitionBDD);
            }

            //2 processes join in the channel sycchronized event, other must be unchanged
            syncedChannelTransition = model.AddVarUnchangedConstraint(syncedChannelTransition, result.variableIndex);
            syncedChannelTransition = model.AddVarUnchangedConstraint(syncedChannelTransition, model.GlobalVarIndex);

            //
            if (syncedChannelTransition.Count > 0)
            {
                result.transitionBDD.Add(CUDD.Function.Or(syncedChannelTransition));
            }
        }

    }
}
