namespace PAT.Common.Classes.Ultility
{
    public class Constants
    {
        public const string TAU = "\u03C4";
        public const string INSTANTANOUS = "\u2191"; // "\u03DF";
        public const string TERMINATION = "terminate";
        public const string INITIAL_EVENT = "init";
        public const string TOCK = "tock";
        public const string IDLE = "idle";

        public const string INTERLEAVE = "|";
        public const string PARALLEL = "&";
        public const string REFINEMENT_PARALLEL = ",";
        public const string GENERAL_CHOICE = "#";
        public const string EXTERNAL_CHOICE = "*";
        public const string INTERNAL_CHOICE = "%";
        public const string INTERRUPT = "^";
        public const string STOP = "!";
        public const string SKIP = "~";
        //public const string SELECTING = "/";
        public const string HIDING = "\\";
        public const string CASE = "?";
        public const string CASECONDITIONAL = ":";
        public const string CONDITIONAL_CHOICE = "@";
        public const string CONDITIONAL_CHOICE_ATOMIC = "C";
        public const string CONDITIONAL_CHOICE_BLOCKING = "B";
        public const string ATONIC_CONDITIONAL_CHOICE = "`";
        public const string EVENTPREFIX = ">";
        public const string EVENTPREFIX_URGENT = ">>";
        public const string SEQUENTIAL = ";";
        public const string TRUE = "%";
        public const string SEPARATOR = "$";
        public const string ATOMIC = "<";
        public const string ATOMIC_STARTED = "/";
        public const string ATOMIC_NOTSTARTED = "<";
        public const string ASSERTION = "A";


        public const string DEADLIME = "D";
        public const string WAITUNTIL = "U";
        public const string WAIT = "W";
        public const string TIMEOUT = "T";
        public const string WITHIN = "I";
        public const string WITHINRANGE = "R";
        


        public const string INIT_STATE = "+";
        public const string ACCEPT_STATE = "-";

        public const string INFINITE = "\u221E";

        public const int DBM_INIT_SIZE = 4;

        public const int CONSTRAINT_NBCLOCK_INIT_SIZE = 4;

        public const int EMPTY_TIMER_ID = 0;

        public const int GLOBAL_TIMER_ID = 1;

        public const string cfull = "cfull";
        public const string cempty = "cempty";
        public const string ccount = "ccount";
        public const string csize = "csize";
        public const string cpeek = "cpeek";


        //Assertion Admissible Behavior
        public const string COMPLETE_BEHAVIOR = "All";//"Original Behavior with No Assumption";
        public const string BEHAVIOR_EVENT_LEVEL_WEAK_FAIRNESS = "Event-level Weak Fair Only";//"Event Level Weak Fairness Assumption";
        public const string BEHAVIOR_EVENT_LEVEL_STRONG_FAIRNESS = "Event-level Strong Fair Only";//" Assumption";
        public const string BEHAVIOR_PROCESS_LEVEL_WEAK_FAIRNESS = "Process-level Weak Fair Only";
        public const string BEHAVIOR_PROCESS_LEVEL_STRONG_FAIRNESS = "Process-level Strong Fair Only";
        public const string BEHAVIOR_GLOBAL_FAIRNESS = "Global Fair Only";
        public const string BEHAVIOR_ZENONESS = "Non-Zeno Only";

        //Assertion Search Engine
        public const string ENGINE_DEPTH_FIRST_SEARCH = "First Witness Trace using Depth First Search";
        public const string ENGINE_BREADTH_FIRST_SEARCH = "Shortest Witness Trace using Breadth First Search";
        public const string ENGINE_BREADTH_FIRST_SEARCH_MONO = "Shortest Witness Trace using Breadth First Search with Monotonically With Value";
        public const string ENGINE_FORWARD_SEARCH_BDD = "Symbolic Model Checking using BDD with Forward Search Strategy";
        public const string ENGINE_FORWARD_TIME_ELAPSE_BDD = "BDD Time Elapse";
        public const string ENGINE_FORWARD_ZONE_BDD = "BDD Like Zone";
        public const string ENGINE_FORWARD_TIME_ELAPSE_SIMULATION_BDD = "BDD Time Elapse With Simulation";
        public const string ENGINE_FORWARD_ZONE_SIMULATION_BDD = "BDD Like Zone With Simulation";
        public const string ENGINE_FORWARD_RABBIT = "BDD Like Rabbit";

        public const string ENGINE_BACKWARD_SEARCH_BDD = "Symbolic Model Checking using BDD with Backward Search Strategy";
        public const string ENGINE_FORWARD_BACKWARD_SEARCH_BDD = "Symbolic Model Checking using BDD with Forward-Backward Search Strategy";
        public const string ENGINE_MTBDD = "MTBDD";
        public const string ENGINE_DCNORM_SEARCH = "First Witness Trace using Diagonal Clock Normalization";

        //PAT.TA reachability
        public const string ENGINE_DEPTH_FIRST_SEARCH_ExtraPlusM = "DFS M";
        public const string ENGINE_DEPTH_FIRST_SEARCH_ExtraPlusLU = "DFS LU";
        public const string ENGINE_DEPTH_FIRST_SEARCH_LUSim = "DFS Simulation";

        public const string ENGINE_BREADTH_FIRST_SEARCH_ExtraPlusM = "BFS M";
        public const string ENGINE_BREADTH_FIRST_SEARCH_ExtraPlusLU = "BFS LU";
        public const string ENGINE_BREADTH_FIRST_SEARCH_LUSim = "BFS Simulation";

        public const string ENGINE_DEPTH_FIRST_SEARCH_Local_ExtraPlusM = "DFS M Local";
        public const string ENGINE_DEPTH_FIRST_SEARCH_Local_ExtraPlusLU = "DFS LU Local";
        public const string ENGINE_DEPTH_FIRST_SEARCH_Local_LUSim = "DFS Simulation Local";

        public const string ENGINE_BREADTH_FIRST_SEARCH_Local_ExtraPlusM = "BFS M Local";
        public const string ENGINE_BREADTH_FIRST_SEARCH_Local_ExtraPlusLU = "BFS LU Local";
        public const string ENGINE_BREADTH_FIRST_SEARCH_Local_LUSim = "BFS Simulation Local";

        public const string ENGINE_BREADTH_FIRST_SEARCH_Local_Digit = "BFS Digitization LU Local";
        public const string ENGINE_DEPTH_FIRST_SEARCH_Loal_Digit = "DFS Digitization LU Local";

        // For PAT.PAT
        public const string ENGINE_WITH_PAR_OPTIM = "With state equivalence reduction";
        public const string ENGINE_WITHOUT_PAR_OPTIM = "Without state equivalence reduction";

        public const string ENGINE_BDD_SEARCH = "Symbolic Model Checking using BDD";
        public const string ENGINE_POR_SEARCH = "Direct reachability with POR and Abstraction refinement";
        public const string ENGINE_AR_SEARCH = "Direct reachablity with Abstraction refinement";
        public const string ENGINE_DEPTH_FIRST_SEARCH_POR = "First Witness Trace with Partial Order Reduction";
        public const string ENGINE_BREADTH_FIRST_SEARCH_POR = "Shortest Witness Trace with Partial Order Reduction";
        public const string ENGINE_DEPTH_FIRST_SEARCH_DIGITIZATION = "First Witness Trace with Digitization";
        public const string ENGINE_DEPTH_FIRST_SEARCH_ZONE = "First Witness Trace with Zone Abstraction";
        public const string ENGINE_BREADTH_FIRST_SEARCH_DIGITIZATION = "Shortest Witness Trace with Digitization";
        public const string ENGINE_BREADTH_FIRST_SEARCH_ZONE = "Shortest Witness Trace with Zone Abstraction";

        public const string ENGINE_ANTICHAIN_DEPTH_FIRST_SEARCH_DIGITIZATION = "First Witness Trace with Digitization and Antichain";
        public const string ENGINE_ANTICHAIN_DEPTH_FIRST_SEARCH_ZONE = "First Witness Trace with Zone Abstraction and Antichain";
        public const string ENGINE_ANTICHAIN_BREADTH_FIRST_SEARCH_DIGITIZATION = "Shortest Witness Trace with Digitization and Antichain";
        public const string ENGINE_ANTICHAIN_BREADTH_FIRST_SEARCH_ZONE = "Shortest Witness Trace with Zone Abstraction and Antichain";

        //Statistical Simulation Engine
        public const string ENGINE_SMT_MDP = "Simluation on MDP model";
        public const string ENGINE_SMT_DTMC ="Simulation on PCSP for Nondeterministic Model Only";

        //Refinement related
        public const string ENGINE_ANTICHAIN_DEPTH_FIRST_SEARCH = "On-the-fly Trace Refinement Checking using DFS and Antichain";
        public const string ENGINE_ANTICHAIN_BREADTH_FIRST_SEARCH = "On-the-fly Trace Refinement Checking using BFS and Antichain";

        public const string ENGINE_REFINEMENT_DEPTH_FIRST_SEARCH = "On-the-fly Trace Refinement Checking using Depth First Search";
        public const string ENGINE_REFINEMENT_BREADTH_FIRST_SEARCH = "On-the-fly Trace Refinement Checking using Breadth First Search";

        public const string ENGINE_F_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH = "On-the-fly Failures Refinement Checking using DFS and Antichain";
        public const string ENGINE_F_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH = "On-the-fly Failures Refinement Checking using BFS and Antichain";

        public const string ENGINE_F_REFINEMENT_DEPTH_FIRST_SEARCH = "On-the-fly Failures Refinement Checking using Depth First Search";
        public const string ENGINE_F_REFINEMENT_BREADTH_FIRST_SEARCH = "On-the-fly Failures Refinement Checking using Breadth First Search";

        public const string ENGINE_FD_REFINEMENT_DEPTH_FIRST_SEARCH = "On-the-fly Failures/Divergence Refinement Checking using Depth First Search";
        public const string ENGINE_FD_REFINEMENT_BREADTH_FIRST_SEARCH = "On-the-fly Failures/Divergence Refinement Checking using Breadth First Search";

        public const string ENGINE_FD_REFINEMENT_ANTICHAIN_DEPTH_FIRST_SEARCH = "On-the-fly Failures/Divergence Refinement Checking using DFS and Antichain";
        public const string ENGINE_FD_REFINEMENT_ANTICHAIN_BREADTH_FIRST_SEARCH = "On-the-fly Failures/Divergence Refinement Checking using BFS and Antichain";

        public const string ENGINE_SCC_BASED_SEARCH = "Strongly Connected Component Based Search";
        public const string ENGINE_SCC_BASED_SEARCH_IMPROVED = "Strongly Connected Component Based Search (Geldenhuys-Valmari)";
        
        public const string ENGINE_SCC_BASED_SEARCH_WITH_POR = "Strongly Connected Component Based Search with POR";
        public const string ENGINE_SCC_BASED_SEARCH_MULTICORE = "(Mulit-Core) Strongly Connected Component Based Search";
        public const string ENGINE_SCC_BASED_SEARCH_ZONE = "Strongly Connected Component Based Search with Zone Abstraction";
        public const string ENGINE_SCC_BASED_SEARCH_DIGITIZATION = "Strongly Connected Component Based Search with Digitization";

        public const string ENGINE_EMPTYNESS_SCC_BASED_SEARCH = "Emptiness Checking using Strongly Connected Component Based Search";
        public const string ENGINE_EMPTYNESS_SCC_BASED_SEARCH_GZG = "Non-Zeno Only using Guessing Zone Graph";
        public const string ENGINE_EMPTYNESS_SCC_BASED_SEARCH_SNZ = "Non-Zeno Only using an Extra Clocks";
        public const string ENGINE_EMPTYNESS_SCC_BASED_SEARCH_CUB = "Non-Zeno Only using Constant Upperbounds";        

        public const string ENGINE_NESTED_DFS_SEARCH = "Nested Depth First Search";
        public const string ENGINE_SWARM_NESTED_DFS_SEARCH = "Swarm Nested Depth First Search";
        public const string ENGINE_SWARM_SCC_BASED_SEARCH = "Swarm Strongly Connected Component Based Search";
        public const string ENGINE_MULTICORE_NESTED_DFS_SEARCH = "Multi-Core Nested Depth First Search";
        public const string ENGINE_MULTICORE_SCC_BASED_SEARCH_SHARED_STATES = "Multi-Core Strongly Connected Component Based Search With Shared States";
        public const string ENGINE_MULTICORE_SCC_BASED_SEARCH_SPAWN_FAIR_THREAD = "Multi-Core Strongly Connected Component Spawning Fair Threads";

        public const string ENGINE_MDP_SEARCH = "Graph-based Probability Computation Based on Value Iteration";
        public const string ENGINE_MDP_MEC_SEARCH = "Graph-based Probability Computation Based on Value Iteration (Improved)";
        public const string ENGINE_MDP_ANTICHAIN_SEARCH = "Graph-based Probability Computation Based on Value Iteration with Antichain";
        public const string ENGINE_MDP_SIM_SEARCH = "Graph-based Probability Computation Based on Value Iteration with Simulation";

        public const string ENGINE_DIRECT_VERIFICATION = "Direct Verification";
        public const string ENGINE_DIRECT_VERIFICATION_POR = "Direct Verification with Partial Order Reduction and Abstraction Refinement";
        public const string ENGINE_DIRECT_VERIFICATION_ABTRACTION_REFINEMENT = "Direct Verification with Abstraction Refinement";
        public const string ENGINE_ASSUME_GURRANTTEE = "Assume-Guarantee Verification via L* learning";
        public const string ENGINE_ASSUME_GURRANTTEE_NLSTAR = "Assume-Guarantee via NL* learning";
        public const string ENGINE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT = "Assume-Guarantee Abstraction-Refinement Verification (split-1)";
        public const string ENGINE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT_SPLIT_REST = "Assume-Guarantee Abstraction-Refinement Verification (split-rest)";
        public const string ENGINE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT_SPLIT_RANDOM = "Assume-Guarantee Abstraction-Refinement Verification (split-random)";
        public const string ENGINE_RECURSIVE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT = "Recursive Assume-Guarantee Abstraction-Refinement Verification (split-1)";
        public const string ENGINE_RECURSIVE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT_SPLIT_REST = "Recursive Assume-Guarantee Abstraction-Refinement Verification (split-rest)";
        public const string ENGINE_RECURSIVE_ASSUME_GURRANTTEE_ABSTRACION_REFINEMENT_SPLIT_RANDOM = "Recursive Assume-Guarantee Abstraction-Refinement Verification (split-random)";
       
        public const string ENGINE_GENETIC_COMPOSITIONAL_ABSTRACION_REFINEMENT = "Genetic Compositional Abstraction Refinement";
        public const string ENGINE_DYNAMIC_GENETIC_COMPOSITIONAL_ABSTRACION_REFINEMENT = "Genetic Compositional Abstraction Refinement (dynamic)";
        public const string ENGINE_GENETIC_COMPOSITIONAL_ABSTRACION_REFINEMENT_MULTI_CORE = "Genetic Compositional Abstraction Refinement Multi Core";
        public const string ENGINE_GENETIC_ASSUME_GURRANTTEE = "Genetic Assume-Guarantee Reasoning";

        public const string ENGINE_COMPOSITIONAL_ABSTRACION_REFINEMENT = "Compositional Abstraction Refinement (split-1)";
        public const string ENGINE_COMPOSITIONAL_ABSTRACION_REFINEMENT_SPLIT_REST = "Compositional Abstraction Refinement (split-rest)";
        public const string ENGINE_COMPOSITIONAL_ABSTRACION_REFINEMENT_SPLIT_RANDOM = "Compositional Abstraction Refinement (split-random)";
        public const string ENGINE_ASSUME_GURRANTTEE_CDNF = "Assume-Guarantee CDNF Algorithms (k-induction Approach)";
        public const string ENGINE_ASSUME_GURRANTTEE_CDNF_BMC = "Assume-Guarantee CDNF Algorithms (BMC Approach)";
        public const string ENGINE_SAT_BASED_MONOLITHIC = "Direct Verification via SAT-Based Model Checking";
        public const string ENGINE_SAT_BASED_BMC = "Direct Verification via SAT-Based Bounded Model Checking";
        //BDD related contants
        public const int BDD_NON_ENCODABLE_0 = 0;
        public const int BDD_COMPOSITION_1 = 1;
        public const int BDD_LTS = 2;
        public const int BDD_LTS_COMPOSITION_3 = 3;

        public const string VERFICATION_RESULT_STRING = "********Verification Result********";
    }

    public enum FairnessType : byte
    {
        NO_FAIRNESS,
        //FAIRNESS_LABEL_ONLY,       
        EVENT_LEVEL_WEAK_FAIRNESS, 
        EVENT_LEVEL_STRONG_FAIRNESS,
        PROCESS_LEVEL_WEAK_FAIRNESS,
        PROCESS_LEVEL_STRONG_FAIRNESS,
        GLOBAL_FAIRNESS
    }

    public enum QueryConstraintType : byte
    {
        NONE,
        MIN,
        MAX,
        PMIN,
        PMAX,
        PROB,
        STATISTIC,
        REWARD,
        RMAX,
        RMIN,
        BOUND,//TiMo
    }
}