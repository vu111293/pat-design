using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.Common.GUI
{
    public sealed class EventStepSim
    {
        public string SourceProcess;

        public string Event
        {
            get 
            {
                return Config.GetDisplayEvent();
            }
        }

        private string processString;
        private string processID;
        public bool IsUnvisitedStep;
        public bool IsCurrentEvent = true;

        public ConfigurationBase Config;

        public string StepToString
        {
            get
            {
                if (processString == null)
                {
                    processString = Config.ToString(); 
                }
                return processString;
            }
            set
            {
                processString = value;
            }
        }

        //processString used in model checking
        public string StepID
        {
            get
            {
                if (processID == null)
                {
                    processID = Config.GetID();
                }
                return processID;
            }
            set
            {
                processID = value;
            }
        }

        public EventStepSim(ConfigurationBase configuration)
        {
            Config = configuration;
        }

        public List<EventStepSim> MakeOneMove(bool HideTauTransition)
        {
            List<EventStepSim> listResutlt = new List<EventStepSim>();
            IEnumerable<ConfigurationBase> list = Config.MakeOneMove(); 
            
            if(HideTauTransition)
            {
                //for the current moves
                foreach (ConfigurationBase step in list)
                {
                    //if is not tau, means a valid one
                    if (step.Event != Common.Classes.Ultility.Constants.TAU)
                    {
                        //then we find the tau-reachable of this step
                        AddAllTauReachableSteps(step, listResutlt);
                    }
                    //this should be case for initial event
                    else if (Config.Event == Common.Classes.Ultility.Constants.INITIAL_EVENT)
                    {
                        Debug.Assert(Config.Event == Common.Classes.Ultility.Constants.INITIAL_EVENT);

                        NormalizedState NState = NormalizedState.TauReachable(new List<ConfigurationBase>() {step});

                        foreach (ConfigurationBase state in NState.States)
                        {
                            IEnumerable<ConfigurationBase> templist = state.MakeOneMove();

                            foreach (ConfigurationBase step1 in templist)
                            {
                                if (step1.Event != Common.Classes.Ultility.Constants.TAU)
                                {
                                    //then we find the tau-reachable of this step
                                    AddAllTauReachableSteps(step1, listResutlt);
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                foreach (ConfigurationBase step in list)
                {
                    bool contains = false;
                    EventStepSim step1Sim = new EventStepSim(step);
                    foreach (EventStepSim sim in listResutlt)
                    {
                        if (sim.Event == step.Event && sim.StepID == step1Sim.StepID)
                        {
                            contains = true;
                            break;
                        }
                    }

                    //duplicated steps should not be added in.
                    if (!contains)
                    {
                        listResutlt.Add(step1Sim);
                    }                  
                }    
            }
            
            return listResutlt;
        }

        private void AddAllTauReachableSteps(ConfigurationBase step, List<EventStepSim> listResutlt)
        {
            NormalizedState NState = NormalizedState.TauReachable(new List<ConfigurationBase>() {step});

            foreach (ConfigurationBase state in NState.States)
            {
                //for each step in this 
                IEnumerable<ConfigurationBase> templist = state.MakeOneMove();

                bool hasNonTauEvent = false;
                foreach (ConfigurationBase step1 in templist)
                {
                    if (step1.Event != Common.Classes.Ultility.Constants.TAU)
                    {
                        hasNonTauEvent = true;
                        break;
                    }
                }

                //if the step has non-tau transition, this should be a boundary step, and we need to keep it
                if (hasNonTauEvent || state.IsDeadLock)
                {
                    if (state.Event == Common.Classes.Ultility.Constants.TAU)
                    {
                        //do we need to add the *
                        state.Event = step.Event; //+ "*"
                        state.DisplayName = null;
                    }

                    EventStepSim step1Sim = new EventStepSim(state);

                    bool contains = false;
                    foreach (EventStepSim sim in listResutlt)
                    {
                        if (sim.Event == step1Sim.Event && sim.StepID == step1Sim.StepID)
                        {
                            contains = true;
                            break;
                        }
                    }

                    //duplicated steps should not be added in.
                    if (!contains)
                    {
                        listResutlt.Add(step1Sim);
                    }
                }
            }
        }

        public override string ToString()
        {
            return StepToString;
        }


        //The following is a replicate of the next method - to reduce one state.        
        public bool StepVisited(Hashtable visited, out string stepString)
        {
            stepString = StepID;
            return visited.ContainsKey(stepString);
        }
    }
}