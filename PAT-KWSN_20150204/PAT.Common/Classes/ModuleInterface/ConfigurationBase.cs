using System.Collections.Generic;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Ultility;

namespace PAT.Common.Classes.ModuleInterface
{
    public abstract class ConfigurationBase
    {
        public string Event;
        public string DisplayName;
        public Valuation GlobalEnv;

        public bool IsDeadLock;
        public bool IsAtomic;
        public bool IsDataOperation;
        public string[] ParticipatingProcesses;

        public abstract IEnumerable<ConfigurationBase> MakeOneMove();

        public IEnumerable<ConfigurationBase> MakeOneMove(string evt)
        {
            List<ConfigurationBase> returnList = new List<ConfigurationBase>();
            IEnumerable<ConfigurationBase> steps = MakeOneMove();
            foreach (ConfigurationBase step in steps)
            {
                if (step.Event == evt)
                {
                    returnList.Add(step);
                }
            }
            return returnList;
        }

        /// <summary>
        /// Test whether a condition is true or false based on the current global valuation.
        /// To be overridden by the deriving class. 
        /// </summary>
        /// <returns></returns>
        public virtual bool ImplyCondition(Expression expression)
        {
            ExpressionValue v = EvaluatorDenotational.Evaluate(expression, GlobalEnv);
            return (v as BoolConstant).Value;
        }

        public virtual Expression EvaluateExpression(Expression expression)
        {
            return EvaluatorDenotational.Evaluate(expression, GlobalEnv);
        }

        public bool EqualsV(ConfigurationBase input)
        {
            return GlobalEnv.GetID() == input.GlobalEnv.GetID();
        }

        public virtual string GetDisplayEvent()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                return Event;
            }

            return DisplayName;
        }

        /// <summary>
        /// This method returns the string representation of the configuration.
        /// In general, it should return current program counter, global valuation, channel buffer 
        /// </summary>
        /// <returns></returns>
        public abstract string GetID();

        ///// <summary>
        ///// This method returns the string representation of the configuration.
        ///// In general, it should return current program counter, global valuation, channel buffer 
        ///// </summary>
        ///// <returns></returns>
        //public virtual bool IsStopProcess()
        //{
        //    return false;
        //}

        /// <summary>
        /// Return the string representation of the state, together with event.
        /// each state is a string of "event + ToString()";
        /// </summary>
        /// <returns></returns>
        public virtual string GetIDWithEvent()
        {
            return GetID() + Constants.SEPARATOR + Event;
        }

        public bool IsDivergent()
        {
            Stack<ConfigurationBase> working = new Stack<ConfigurationBase>();
            List<string> path = new List<string>(100);
            StringHashTable visited = new StringHashTable(100);

            //The following are for identifying the current path. 
            Stack<int> depthStack = new Stack<int>(1024);
            depthStack.Push(0);
            List<int> depthList = new List<int>(1024);
            //The above are for identifying the current path. 

            working.Push(this);
            visited.Add(GetID());

            while (working.Count > 0)
            {
                ConfigurationBase current = working.Pop();
                IEnumerable<ConfigurationBase> nextStates = current.MakeOneMove(Constants.TAU);

                //The following are for identifying the current path. 
                int depth = depthStack.Pop();

                if (depth > 0)
                {
                    while (depthList[depthList.Count - 1] >= depth)
                    {
                        int lastIndex = depthList.Count - 1;
                        depthList.RemoveAt(lastIndex);
                        path.RemoveAt(lastIndex);
                    }
                }

                path.Add(current.GetID());
                depthList.Add(depth);

                if (nextStates != null)
                {
                    //for (int i = 0; i < nextStates.Length; i++)
                    foreach (ConfigurationBase next in nextStates)
                    {
                        //ConfigurationBase next = nextStates[i];
                        string ID = next.GetID();
                        if (path.Contains(ID))
                        {
                            return true;
                        }
                        else
                        {
                            if (!visited.ContainsKey(ID))
                            {
                                visited.Add(ID);
                                working.Push(next);
                                depthStack.Push(depth + 1);
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
