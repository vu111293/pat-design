using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.LTS;
using System.Linq;
using System;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.Assertion
{
    public sealed class NormalizedState
    {
        private const int EXPENDING_FACTOR = 5;

        public List<ConfigurationBase> States;
        
        private string StateString;

        public NormalizedState(List<ConfigurationBase> states)
        {
            States = new List<ConfigurationBase>();
            Dictionary<string, bool> StateIDs = new Dictionary<string, bool>();

            for (int i = 0; i < states.Count; i++)
            {
                string id = states[i].GetID();
                if (!StateIDs.ContainsKey(id))
                {
                    States.Add(states[i]);
                    StateIDs.Add(id, false);
                }
            }
        }

        public NormalizedState MakeOneMove(string evt)
        {
            if (evt == Constants.TAU)
            {
                return this;
            }

            int size = States.Count * EXPENDING_FACTOR;

            List<ConfigurationBase> nextSpecStates = new List<ConfigurationBase>(size);
            //StringHashTable visited = new StringHashTable(size);

            for (int i = 0; i < States.Count; i++)
            {
                IEnumerable<ConfigurationBase> moves = States[i].MakeOneMove(evt);

                foreach (ConfigurationBase var in moves)
                {
                    //string tmp = var.GetID();
                    //if (!visited.ContainsKey(tmp))
                    //{
                        nextSpecStates.Add(var);
                        //visited.Add(tmp);
                    //}
                }
            }

            //return TauReachable(specNext, localVisited);
            //To get all states which can be reached via tau-transitions only.
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(size);

            foreach (ConfigurationBase vm in nextSpecStates)
            {
                working.Push(vm);
            }

            while (working.Count > 0)
            {
                IEnumerable<ConfigurationBase> vms = working.Pop().MakeOneMove(Constants.TAU);

                //for (int i = 0; i < vms.Length; i++)
                foreach (ConfigurationBase configuration in vms)
                {
                    //ConfigurationBase configuration = vms[i];

                    //string tmp = configuration.GetID();
                    //if (!visited.ContainsKey(tmp))
                    //{
                        nextSpecStates.Add(configuration);
                        //visited.Add(tmp);
                        working.Push(configuration);
                    //}
                }
            }

            return new NormalizedState(nextSpecStates);
        }
        
        /// <summary>
        /// Get the compressed state representation
        /// </summary>
        /// <returns></returns>
        public string GetCompressedState()
        {
            if (StateString == null)
            {
                //List<string> stateIDs = new List<string>(States.Length);

                //for (int i = 0; i < States.Length; i++)
                //{
                //    stateIDs.Add(States[i].GetID());
                //}

                //stateIDs.Sort();

                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < States.Count; i++)
                {
                    sb.Append(States[i].GetID());
                    sb.Append(Constants.SEPARATOR);
                }

                StateString = sb.ToString();
            }

            return StateString;
        }

        /// <summary>
        /// To get all states which can be reached via tau-transitions only.
        /// </summary>
        /// <returns></returns>
        public static NormalizedState TauReachable(List<ConfigurationBase> states)
        {
            int size = states.Count * EXPENDING_FACTOR;

            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>(size);
            StringHashTable visited = new StringHashTable(size);

            foreach (ConfigurationBase vm in states)
            {
                working.Push(vm);
                visited.Add(vm.GetID());
            }

            while (working.Count > 0)
            {
                ConfigurationBase current = working.Pop();
                IEnumerable<ConfigurationBase> vms = current.MakeOneMove(Constants.TAU);

                //for (int i = 0; i < vms.Length; i++)
                foreach (ConfigurationBase configuration in vms)
                {
                    //ConfigurationBase configuration = vms[i];

                    string tmp = configuration.GetID();
                    if (!visited.ContainsKey(tmp))
                    {
                        states.Add(configuration);
                        visited.Add(tmp);
                        working.Push(configuration);
                    }
                }
            }

            return new NormalizedState(states);
        }

        /// <summary>
        /// Check whether this normalized state is divergence. Return true if and only if it contains a state in the parameter isDiv.
        /// </summary>
        /// <param name="isDiv"></param>
        /// <returns></returns>
        public bool isDivergence(Dictionary<string, bool> isDiv)
        {
            foreach (ConfigurationBase item in States)
            {
                if (isDiv[item.GetID()])
                {
                    return true;
                }
            }

            //foreach (string s in StateIDs)
            //{
            //    if (isDiv[s])
            //    {
            //        return true;
            //    }
            //}

            return false;
        }

        ///// <summary>
        ///// To get all states which can be reached by tau-transitions only; meanwhile, identify which of them are divergent states.
        ///// </summary>
        ///// <param name="States"></param>
        ///// <param name="isDiv"></param>
        ///// <returns></returns>
        //public NormalizedState TAUALL(Dictionary<string, bool> isDiv)
        //{
        //    int size = States.Count * EXPENDING_FACTOR;

        //    List<ConfigurationBase> reachable = new List<ConfigurationBase>(size);
        //    Dictionary<string, int> visited = new Dictionary<string, int>(size);
        //    Dictionary<int, List<int>> transitions = new Dictionary<int, List<int>>(size);
        //    List<string> scc = new List<string>(size);
        //    Stack<ConfigurationBase> TaskStack = new Stack<ConfigurationBase>(size);

        //    for (int i = 0; i < States.Count; i++)
        //    {
        //        ConfigurationBase configuration = States[i];
        //        string tmp = configuration.GetID();
        //        //if (!visited.ContainsKey(tmp))
        //        //{
        //            int index = visited.Count;
        //            visited.Add(tmp, index);
        //            TaskStack.Push(configuration);
        //            reachable.Add(configuration);
                    
        //            if (!isDiv.ContainsKey(tmp))
        //            {
        //                isDiv.Add(tmp, false);
        //            }
        //            transitions.Add(index, new List<int>());
        //        //}
        //    }

        //    Dictionary<int, int> preorder = new Dictionary<int, int>(size);
        //    Dictionary<int, int> lowlink = new Dictionary<int, int>(size);
        //    Stack<ConfigurationBase> stepStack = new Stack<ConfigurationBase>(size);
        //    List<int> scc_found = new List<int>(size);
        //    //# Preorder counter 
        //    int ii = 0;

        //    //store the expended event step of a node to avoid multiple invocation of the make one move.
        //    Dictionary<int, IEnumerable<ConfigurationBase>> ExpendedNode = new Dictionary<int, IEnumerable<ConfigurationBase>>(size);

        //    while (TaskStack.Count > 0)
        //    {
        //        ConfigurationBase pair = TaskStack.Peek();
        //        string StepID = pair.GetID();

        //        int v = visited[StepID];

        //        List<int> outgoing = transitions[v];

        //        if (!preorder.ContainsKey(v))
        //        {
        //            preorder.Add(v, ii);
        //            ii++;
        //        }

        //        bool done = true;

        //        if (ExpendedNode.ContainsKey(v))
        //        {
        //            IEnumerable<ConfigurationBase> list = ExpendedNode[v];
        //            if (list.Length > 0)
        //            {
        //                //transverse all steps
        //                for (int k = list.Length - 1; k >= 0; k--)
        //                {
        //                    ConfigurationBase step = list[k];

        //                    if (step != null)
        //                    {
        //                        int t = visited[step.GetID()];

        //                        //if the step is a unvisited step
        //                        if (!preorder.ContainsKey(t))
        //                        {
        //                            //only add the first unvisited step
        //                            //for the second or more unvisited steps, ignore at the monent
        //                            if (done)
        //                            {
        //                                TaskStack.Push(step);

        //                                done = false;

        //                                //list.RemoveAt(k);
        //                                list[k] = null;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            //list.RemoveAt(k);
        //                            list[k] = null;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            IEnumerable<ConfigurationBase> product = pair.MakeOneMove(Constants.TAU);

        //            for (int k = product.Length - 1; k >= 0; k--)
        //            {
        //                ConfigurationBase step = product[k];

        //                string stateString = step.GetID();

        //                if (visited.ContainsKey(stateString))
        //                {
        //                    int t = visited[stateString];
        //                    outgoing.Add(t);

        //                    //if this node is still not visited
        //                    if (!preorder.ContainsKey(t))
        //                    {
        //                        //only put the first one to the work list stack.
        //                        //if there are more than one node to be visited, 
        //                        //simply ignore them and keep its event step in the list.
        //                        if (done)
        //                        {
        //                            TaskStack.Push(step);
        //                            done = false;
        //                            //product.RemoveAt(k);
        //                            product[k] = null;
        //                        }
        //                        else
        //                        {
        //                            product[k] = step;
        //                        }
        //                    }
        //                    //this node is truly visited. can be removed
        //                    else
        //                    {
        //                        //product.RemoveAt(k);
        //                        product[k] = null;
        //                    }
        //                }
        //                else
        //                {
        //                    int stateID = visited.Count;
        //                    visited.Add(stateString, stateID);
        //                    reachable.Add(step);
        //                    if (!isDiv.ContainsKey(stateString))
        //                    {
        //                        isDiv.Add(stateString, false);
        //                    }

        //                    transitions.Add(stateID, new List<int>(8));
        //                    outgoing.Add(stateID);
        //                    //only put the first one into the stack.
        //                    if (done)
        //                    {
        //                        TaskStack.Push(step);
        //                        done = false;
        //                        //product.RemoveAt(k);
        //                        product[k] = null;
        //                    }
        //                    else
        //                    {
        //                        product[k] = step;
        //                    }
        //                }
        //            }

        //            //create the remaining steps as the expending list for v
        //            ExpendedNode.Add(v, product);
        //        }

        //        if (done)
        //        {
        //            lowlink[v] = preorder[v];

        //            bool selfLoop = false;
        //            for (int j = 0; j < outgoing.Count; j++)
        //            {
        //                int w = outgoing[j];
        //                if (w == v)
        //                {
        //                    selfLoop = true;
        //                }

        //                if (!scc_found.Contains(w))
        //                {
        //                    if (preorder[w] > preorder[v])
        //                    {
        //                        lowlink[v] = Math.Min(lowlink[v], lowlink[w]);
        //                    }
        //                    else
        //                    {
        //                        lowlink[v] = Math.Min(lowlink[v], preorder[w]);
        //                    }
        //                }
        //            }

        //            TaskStack.Pop();

        //            if (lowlink[v] == preorder[v])
        //            {
        //                scc.Add(StepID);
        //                scc_found.Add(v);

        //                while (stepStack.Count > 0 && preorder[visited[stepStack.Peek().GetID()]] > preorder[v])
        //                {
        //                    ConfigurationBase s = stepStack.Pop();
        //                    string tmp = s.GetID();
        //                    int k = visited[tmp];
        //                    scc.Add(tmp);
        //                    scc_found.Add(k);
        //                }

        //                //outgoing.Count == 0 --> deadlock, we need to check //outgoing.Count == 0
        //                //StronglyConnectedComponets.Count > 1 || selfLoop -> non-trivial case, we need to check
        //                if (scc.Count > 1 || selfLoop)
        //                {
        //                    foreach (string item in scc)
        //                    {
        //                        isDiv[item] = true;
        //                    }

        //                    foreach (ConfigurationBase vm in TaskStack)
        //                    {
        //                        isDiv[vm.GetID()] = true;
        //                    }
        //                }

        //                foreach (string componet in scc)
        //                {
        //                    ExpendedNode.Remove(visited[componet]);
        //                }

        //                scc.Clear();
        //            }
        //            else
        //            {
        //                stepStack.Push(pair);
        //            }
        //        }
        //    }

        //    return new NormalizedState(reachable);
        //}

        /// <summary>
        /// To get all states which can be reached by tau-transitions only; meanwhile, identify which of them are divergent states.
        /// </summary>
        /// <param name="States"></param>
        /// <param name="isDiv"></param>
        /// <returns></returns>
        public static NormalizedState TAUALL(List<ConfigurationBase> States, Dictionary<string, bool> isDiv)
        {
            int size = States.Count * EXPENDING_FACTOR;

            List<ConfigurationBase> reachable = new List<ConfigurationBase>(size);
            Dictionary<string, int> visited = new Dictionary<string, int>(size);
            Dictionary<int, List<int>> transitions = new Dictionary<int, List<int>>(size);
            List<string> scc = new List<string>(size);
            Stack<ConfigurationBase> TaskStack = new Stack<ConfigurationBase>(size);

            for (int i = 0; i < States.Count; i++)
            {
                ConfigurationBase configuration = States[i];
                string tmp = configuration.GetID();
                if(!visited.ContainsKey(tmp))
                {
                    int index = visited.Count;
                    visited.Add(tmp, index);
                    TaskStack.Push(configuration);
                    reachable.Add(configuration);
                    if (!isDiv.ContainsKey(tmp))
                    {
                        isDiv.Add(tmp, false);
                    }
                    transitions.Add(index, new List<int>());
                }               
            }

            Dictionary<int, int> preorder = new Dictionary<int, int>(size);
            Dictionary<int, int> lowlink = new Dictionary<int, int>(size);
            Stack<ConfigurationBase> stepStack = new Stack<ConfigurationBase>(size);
            List<int> scc_found = new List<int>(size);
            //# Preorder counter 
            int ii = 0;

            //store the expended event step of a node to avoid multiple invocation of the make one move.
            Dictionary<int, IEnumerable<ConfigurationBase>> ExpendedNode = new Dictionary<int, IEnumerable<ConfigurationBase>>(size);

            while (TaskStack.Count > 0)
            {
                ConfigurationBase pair = TaskStack.Peek();
                string StepID = pair.GetID();
                int v = visited[StepID];

                List<int> outgoing = transitions[v];

                if (!preorder.ContainsKey(v))
                {
                    preorder.Add(v, ii);
                    ii++;
                }

                bool done = true;

                if (ExpendedNode.ContainsKey(v))
                {
                    ConfigurationBase[] list = ExpendedNode[v].ToArray();
                    if (list.Length > 0)
                    {
                        //transverse all steps
                        for (int k = list.Length - 1; k >= 0; k--)
                        {
                            ConfigurationBase step = list[k];

                            if (step != null)
                            {
                                int t = visited[step.GetID()];

                                //if the step is a unvisited step
                                if (!preorder.ContainsKey(t))
                                {
                                    //only add the first unvisited step
                                    //for the second or more unvisited steps, ignore at the monent
                                    if (done)
                                    {
                                        TaskStack.Push(step);

                                        done = false;

                                        //list.RemoveAt(k);
                                        list[k] = null;
                                    }
                                }
                                else
                                {
                                    //list.RemoveAt(k);
                                    list[k] = null;
                                }
                            }
                        }
                    }
                }
                else
                {
                    ConfigurationBase[] product = pair.MakeOneMove(Constants.TAU).ToArray();

                    for (int k = product.Length - 1; k >= 0; k--)
                    {
                        ConfigurationBase step = product[k];

                        string stateString = step.GetID();

                        if (visited.ContainsKey(stateString))
                        {
                            int t = visited[stateString];
                            outgoing.Add(t);

                            //if this node is still not visited
                            if (!preorder.ContainsKey(t))
                            {
                                //only put the first one to the work list stack.
                                //if there are more than one node to be visited, 
                                //simply ignore them and keep its event step in the list.
                                if (done)
                                {
                                    TaskStack.Push(step);
                                    done = false;
                                    //product.RemoveAt(k);
                                    product[k] = null;
                                }
                                else
                                {
                                    product[k] = step;
                                }
                            }
                                //this node is truly visited. can be removed
                            else
                            {
                                //product.RemoveAt(k);
                                product[k] = null;
                            }
                        }
                        else
                        {
                            int stateID = visited.Count;
                            visited.Add(stateString, stateID);
                            reachable.Add(step);
                            if (!isDiv.ContainsKey(stateString))
                            {
                                isDiv.Add(stateString, false);
                            }

                            transitions.Add(stateID, new List<int>(8));
                            outgoing.Add(stateID);
                            //only put the first one into the stack.
                            if (done)
                            {
                                TaskStack.Push(step);
                                done = false;
                                //product.RemoveAt(k);
                                product[k] = null;
                            }
                            else
                            {
                                product[k] = step;
                            }
                        }
                    }

                    //create the remaining steps as the expending list for v
                    ExpendedNode.Add(v, product);
                }

                if (done)
                {
                    lowlink[v] = preorder[v];

                    bool selfLoop = false;
                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        int w = outgoing[j];
                        if (w == v)
                        {
                            selfLoop = true;
                        }

                        if (!scc_found.Contains(w))
                        {
                            if (preorder[w] > preorder[v])
                            {
                                lowlink[v] = Math.Min(lowlink[v], lowlink[w]);
                            }
                            else
                            {
                                lowlink[v] = Math.Min(lowlink[v], preorder[w]);
                            }
                        }
                    }

                    TaskStack.Pop();

                    if (lowlink[v] == preorder[v])
                    {
                        scc.Add(StepID);
                        scc_found.Add(v);

                        while (stepStack.Count > 0 && preorder[visited[stepStack.Peek().GetID()]] > preorder[v])
                        {
                            ConfigurationBase s = stepStack.Pop();
                            string tmp = s.GetID();
                            int k = visited[tmp];
                            scc.Add(tmp);
                            scc_found.Add(k);
                        }

                        //outgoing.Count == 0 --> deadlock, we need to check //outgoing.Count == 0
                        //StronglyConnectedComponets.Count > 1 || selfLoop -> non-trivial case, we need to check
                        if (scc.Count > 1 || selfLoop)
                        {
                            foreach (string item in scc)
                            {
                                isDiv[item] = true;
                            }

                            foreach (ConfigurationBase vm in TaskStack)
                            {
                                isDiv[vm.GetID()] = true;
                            }
                        }

                        foreach (string componet in scc)
                        {
                            ExpendedNode.Remove(visited[componet]);
                        }

                        scc.Clear();
                    }
                    else
                    {
                        stepStack.Push(pair);
                    }
                }
            }

            return new NormalizedState(reachable);
        }

        public bool isEmpty ()
        {
            return States.Count == 0;
        }
    }
}