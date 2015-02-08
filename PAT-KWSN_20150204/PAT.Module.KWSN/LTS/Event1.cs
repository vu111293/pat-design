using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;
using PAT.Common.Classes.Ultility;
using PAT.KWSN.Assertions;

namespace PAT.KWSN.LTS
{
    public class Event1 : Process
    {
        //Todo: put possible process sub components here
        //public Event Event;
        //public Process Process;

        //TODO: constructor to initialize the object
        public Event1()
        {
            //Event = e;
            //Process = process;

            //process ID should be initialized here by concatinating all the sub process ID
            //ProcessID = DataStore.DataManager.InitializeProcessID(Event.GetID() + Constants.EVENTPREFIX + Process.ProcessID);
        }

        /// <summary>
        /// returns all the possible moves of the current process
        /// </summary>
        /// <returns></returns>
        public override void MoveOneStep(Valuation GlobalEnv, List<Configuration> list)
        {
            //TODO: the operational semantics should be implemented here    
            //string ID = Event.GetEventID(eStep.GlobalEnv);
            //string name = Event.GetEventName(eStep.GlobalEnv);

            //if(ID != name)
            //{
            //    list.Add(new Configuration(Process, ID ,name, eStep.GlobalEnv, false));
            //}
            //else
            //{
            //    list.Add(new Configuration(Process, ID, null, eStep.GlobalEnv, false));
            //}
        }

        /// <summary>
        /// returns the string representation of the object, which is used by the simulator
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //todo: implement the return string format
            //return "(" + Event + "->" + Process.ToString() + ")";
            return "Event1";
        }

        /// <summary>
        /// Get the set of global variables which may be accessed by this process. Notice that arrays will be flatened (for one level). 
        /// For instance, let leader[3] be an array, leader[0], leader[1] will be listed as two different variables. 
        /// </summary>
        /// <returns></returns>
        public override List<string> GetGlobalVariables()
        {
            return new List<string>(0);
        }

        /// <summary>
        /// Get the set of relevant channels.
        /// </summary>
        /// <returns></returns>
        public override List<string> GetChannels()
        {
            return new List<string>(0);
        }

        /// <summary>
        /// This method defines the logic for calculating the default alphabet of a process. That is, the set of events consitituting the 
        /// process expression with process reference unfolded ONCE! We found this to be intuitive at the moment.
        /// </summary>
        /// <returns></returns>
        public override HashSet<string> GetAlphabets(Dictionary<string, string> visitedDefinitionRefs)
        {
            return new HashSet<string>();
        }

        public override bool MustBeAbstracted()
        {
            return false;
        }


        /// <summary>
        /// clear global constants and process parameters; This method is the starting point of run-time execution of the process.
        /// </summary>
        /// <param name="constMapping"></param>
        /// <returns></returns>
        public override Process ClearConstant(Dictionary<string, Expression> constMapping)
        {
            //todo: return a new object by clearing all the constants in expressions and sub-processes
            //return new EventPrefix(Event.ClearConstant(constMapping), Process.ClearConstant(constMapping));
            return null;
        }

        /// <summary>
        /// returns all the possible synchoronous input process on the given channel
        /// </summary>
        /// <returns></returns>
        public override void SyncInput(ConfigurationWithChannelData eStep, List<Configuration> list)
        {
        }

        /// <summary>
        /// returns all the possible synchronous output steps
        /// </summary>
        /// <returns></returns>
        public override void SyncOutput(Valuation GlobalEnv, List<ConfigurationWithChannelData> list)
        {
        }

        public override Process GetTopLevelConcurrency(List<string> visitedDef)
        {
            return null;
        }
    }
}