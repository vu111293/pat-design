using System.Collections.Generic;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;

namespace PAT.PN.LTS
{
    /// <summary>
    /// Một mạng PetriNet 
    /// </summary>
    public class PetriNet  //: ProcessBase<Configuration>
    {
        public List<PNTransition> Transitions { get; set; }
        public string Name { get; set; }


        #region TL: copy from PAT.Common.Classes.SemanticModels.LTS.BDD.SymbolicLTS, can delete?
        /// <summary>
        /// Parameter information is used to add new boolean variables. UpperBound, LowerBound and Arguments are used to identify the parameter ranges.
        /// </summary>
        public List<string> Parameters = new List<string>();

        public EventCollection AlphabetEvents;
        public HashSet<string> Alphabets;

        //after static analysis, AlphabetsCalculable is true if and only if Alphabets are not null.
        public bool AlphabetsCalculable;

        public PetriNet()
        {
        }

        public PetriNet(string name, List<string> vars, List<PNPlace> states)
        {
            Name = name;
            if (vars != null)
                Parameters = vars;

            AlphabetsCalculable = true;
            Alphabets = new HashSet<string>();
        }

        #endregion

        public string ProcessID { get; set; }

        /// <summary>
        /// Get the set of global variables which may be accessed by this process. Notice that arrays will be flatened (for one level). 
        /// For instance, let leader[3] be an array, leader[0], leader[1] will be listed as two different variables. 
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetGlobalVariables()
        {
            return new List<string>(0);
        }

        /// <summary>
        /// Get the set of relevant channels.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetChannels()
        {
            return new List<string>(0);
        }

        /// <summary>
        /// This method returns true iff it can be encoded using Khanh's BDD library. 
        /// A process can be encoded using Khanh's BDD library iff it is composed of compositions of LTSs.
        /// Notice that a process which contains the following features is not BDD encodable for the moment.
        /// 1. Atomic
        /// 2. external library
        /// 3. |||{..} 
        /// 4. Hiding
        /// </summary>
        /// <returns></returns>
        public virtual bool IsBDDEncodable()
        {
            return false;
        }

        /// <summary>
        /// This method defines the logic for calculating the default alphabet of a process. That is, the set of events consitituting the 
        /// process expression with process reference unfolded ONCE! We found this to be intuitive at the moment.
        /// </summary>
        /// <returns></returns>
        public virtual HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            return new HashSet<string>();
        }

        public virtual bool MustBeAbstracted()
        {
            return false;
        }


        ///// <summary>
        ///// returns all the possible synchoronous input process on the given channel
        ///// </summary>
        ///// <returns></returns>
        public virtual void SyncInput(PNConfigurationWithChannelData eStep, List<PNConfiguration> list) //, string syncChannel, Expression[] values
        {
        }

        ///// <summary>
        ///// returns all the possible synchronous output steps
        ///// </summary>
        ///// <returns></returns>
        public virtual void SyncOutput(Valuation GlobalEnv, List<PNConfigurationWithChannelData> list)
        {
        }

        public virtual PetriNet GetTopLevelConcurrency(List<string> visitedDef)
        {
            return null;
        }

        public virtual PetriNet ClearConstant(Dictionary<string, Expression> constMapping)
        {
            //code mo` by ThuanLe
            foreach (var t in Transitions)
            {
                //t.clearConstran
            }
            return this;
        }

        public void SetTransitions(List<PNTransition> trans)
        {
            Transitions = trans;
        }
    }
}