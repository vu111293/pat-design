using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    /// <summary>
    /// Contains all of the modules declared in the program
    /// </summary>
    public class Modules
    {
        /// <summary>
        /// DefaultModuleIndex for the main module
        /// </summary>
        public const int DefaultMainModuleIndex = -1;

        public ModelType modelType;

        public FormulaList formulaList;
        public LabelList labelList;
        public ConstantList constantList;


        public List<VarDeclaration> GlobalVars = new List<VarDeclaration>();
        public List<Module> AllModules = new List<Module>();
        public SystemDef systemDef;
        public List<RewardStruct> rewardStructs = new List<RewardStruct>(); // Rewards structures
        public Expression init;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="vars"></param>
        /// <param name="allAllModules"></param>
        /// <param name="rewardStructs"></param>
        /// <param name="system"></param>
        /// <param name="initExp">give null if not exists</param>
        public Modules(ModelType type, List<VarDeclaration> vars, List<Module> allAllModules, List<RewardStruct> rewardStructs, SystemDef system, Expression initExp)
        {
            modelType = type;
            GlobalVars = vars;
            AllModules = allAllModules;
            this.rewardStructs = rewardStructs;
            systemDef = system;
            init = initExp;
        }

        public Modules(ModelType type, List<VarDeclaration> vars, FormulaList formulae, LabelList labels, ConstantList constants, List<Module> allAllModules, List<RewardStruct> rewardStructs, Expression initExp)
        {
            modelType = type;
            GlobalVars = vars;
            formulaList = formulae;
            labelList = labels;
            constantList = constants;
            AllModules = allAllModules;
            this.rewardStructs = rewardStructs;
            init = initExp;

            List<SystemDef> sys = new List<SystemDef>();
            foreach (var module in AllModules)
            {
                sys.Add(new SingleModuleSystem(module.Name));
            }

            systemDef = new FullParallelSystem(sys);
        }

        public int GetNumberOfModules()
        {
            return AllModules.Count;
        }

        public HashSet<string> GetAllSynchs()
        {
            HashSet<string> allSyncs = new HashSet<string>();
            foreach (var module in AllModules)
            {
                HashSet<string> syncsInModule = module.GetAllSynchs();
                allSyncs.UnionWith(syncsInModule);
            }

            return allSyncs;
        }

        public int GetModuleIndex(string name)
        {
            for (int i = 0; i < AllModules.Count; i++ )
            {
                if (AllModules[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        public Module GetModule(int index)
        {
            return AllModules[index];
        }

        public int GetNumberOfCommands()
        {
            int count = 0;
            foreach (var module in AllModules)
            {
                count += module.Commands.Count;
            }

            return count;
        }
    }

    public enum ModelType 
    {
        DTMC, MDP, CTMC
    }
}
