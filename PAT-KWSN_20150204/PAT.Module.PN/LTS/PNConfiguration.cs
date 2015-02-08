using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.PN.LTS
{
    /// <summary>
    /// Represent a configuration of the system. 
    /// 
    /// A system in the PAT is the group of (P,V) with:
    ///   P: a Process/Model
    ///   V: the value of current process.
    ///     
    /// The Gloabal values are stored in GlobalEnv variable.
    /// </summary>
    public class PNConfiguration : ConfigurationBase
    {
        public PetriNet Process { get; set; }
        public string ConfigID;

        /**
         * BASE CLASS VARIABLEs
         * 
         * public string Event;
         * public string DisplayName;
         * public Valuation GlobalEnv; //-> the global variable 
         * 
         * public bool IsDeadLock; //-> fire to mark when checking Deadlock Assertion
         * public bool IsAtomic;
         * public bool IsDataOperation;
         * public string[] ParticipatingProcesses;
         */

        /// <summary>
        /// This constructor is called from the assertion to intial a configuration
        /// 
        /// Status: incompleted: How to initial value for globalEnv 
        /// </summary>
        /// <param name="p"></param>
        /// <param name="e"></param>
        /// <param name="displayName"></param>
        /// <param name="globalEnv"></param>
        /// <param name="isDataOperation"></param>
        public PNConfiguration(PetriNet p, string e, string displayName, Valuation globalEnv, bool isDataOperation)
        {
            Process = p;
            Event = e;//base event
            GlobalEnv = globalEnv;
            DisplayName = displayName;
            IsDataOperation = isDataOperation;
        }

        // added by Tinh
        public PNConfiguration(PetriNet p, string e, string displayName, Valuation globalEnv, bool isDataOperation, SpecificationBase spec)
        {
            Process = p;
            Event = e;//base event
            GlobalEnv = globalEnv;
            DisplayName = displayName;
            IsDataOperation = isDataOperation;

            if (spec != null)
            {
                p.Transitions = new List<PNTransition>(16);
                foreach (KeyValuePair<string, PetriNet> entry in (spec as Specification).PNDefinitionDatabase)
                    p.Transitions.AddRange(entry.Value.Transitions);
            }
        }

        /// <summary>
        /// Stautus: Completed
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("The process is:");
            sb.AppendLine(Process.ToString());
            if (!GlobalEnv.IsEmpty())
            {
                sb.AppendLine();
                sb.AppendLine("The environment is:");
                sb.AppendLine(GlobalEnv.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dự đoán: để tính unique string cho 1 trạng thái để
        /// </summary>
        /// <returns></returns>
        public override string GetID()
        {
            if (ConfigID == null)
            {
                if (GlobalEnv.IsEmpty())
                    ConfigID = Process.ProcessID;
                else
                    ConfigID = GlobalEnv.GetID(Process.ProcessID);
            }

            //if we can make sure ConfigID is not changed, we can calculate one time only
            System.Diagnostics.Debug.Assert(ConfigID == Process.ProcessID || ConfigID == GlobalEnv.GetID(Process.ProcessID));

            return ConfigID;
        }

        public override IEnumerable<ConfigurationBase> MakeOneMove()
        {
            var nextPNConfigurations = new List<PNConfiguration>();

            foreach (var t in Process.Transitions)
            {
                if (t.GuardCondition != null)
                {
                    ExpressionValue v = EvaluatorDenotational.Evaluate(t.GuardCondition, GlobalEnv);
                    if (!(v as BoolConstant).Value)
                        continue;

                    //transition can fire --> move one step
                    var newGlobleEnv = GlobalEnv.GetVariableClone();
                    EvaluatorDenotational.Evaluate(t.ProgramBlock, newGlobleEnv);

                    var name = t.Event.GetEventName(GlobalEnv);
                    var id = t.Event.GetEventID(GlobalEnv);

                    var newConfiguration = new PNConfiguration(Process, id, name, newGlobleEnv, IsDataOperation);
                    nextPNConfigurations.Add(newConfiguration);
                }
                else
                {
                    //SOME THING WRONG HERE: 
                    System.Diagnostics.Debug.WriteLine("Transition don't have guardCondition/fired condition.");
                }
            }

            if (nextPNConfigurations.Count == 0)
                IsDeadLock = true;

            return nextPNConfigurations;
        }
    }
}
