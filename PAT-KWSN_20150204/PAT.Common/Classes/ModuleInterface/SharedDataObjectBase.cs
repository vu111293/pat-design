using System;
using System.Collections.Generic;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.Common.Classes.LTS
{

    public class SharedDataObjectBase
    {
        //these data will not change for SpecProcess.SharedData, so once it is created after parsing, there is no need to set them again.
        public StringDictionary<int> VariableLowerBound;
        public StringDictionary<int> VariableUpperLowerBound;
        public StringDictionary<string> ValutionHashTable;
        public List<string> SyncrhonousChannelNames;

        public bool HasSyncrhonousChannel;
        
        public Dictionary<string, System.Reflection.MethodInfo> CSharpMethods;
        public Dictionary<string, Type> CSharpDataType;
        //public Dictionary<string, KeyValuePair<List<string>, Expression>> MacroDefinition;

        public bool HasAtomicEvent;
        public bool HasCSharpCode;
        public StringHashTable LocalVars;
        //public StringHashTable WildVars;

        //the following fields stores only default settings used for the simulation, so there is no need to change them after parsing.
        //public FairnessType FairnessType;
        //public bool CalculateParticipatingProcess;
        //public bool TimedRefinementAssertion;

        //public int TimedRefinementClockCeiling;
        //public int TimedRefinementClockFloor;

        //public CSPDataStore DataManager;

        public SharedDataObjectBase()
        {
            //DataManager = new CSPDataStore();
            //AlphaDatabase = new Dictionary<string, EventCollection>(8);
            VariableLowerBound = new StringDictionary<int>(8);
            VariableUpperLowerBound = new StringDictionary<int>(8);
            ValutionHashTable = new StringDictionary<string>(Ultility.Ultility.MC_INITIAL_SIZE);

            SyncrhonousChannelNames = new List<string>();
            HasSyncrhonousChannel = false;
            HasAtomicEvent = false;
            //TimedRefinementAssertion = false;

            CSharpMethods = new Dictionary<string, System.Reflection.MethodInfo>();
            CSharpDataType = new Dictionary<string, Type>();
            //MacroDefinition = new Dictionary<string, KeyValuePair<List<string>, Expression>>();

            //FairnessType = FairnessType.NO_FAIRNESS;
            //CalculateParticipatingProcess = false;
            //CalculateCreatedProcess = false;

            //TimedRefinementClockCeiling = Common.Classes.Ultility.Ultility.TIME_REFINEMENT_CLOCK_CEILING;
            //TimedRefinementClockFloor = Common.Classes.Ultility.Ultility.TIME_REFINEMENT_CLOCK_FLOOR;

            LocalVars = null;
            //WildVars = null;
        }

        public void LockSpecificationData()
        {


            
            //if(resetDB)
            //{
            //    specification.SharedData.DataManager= new CSPDataStore();
            //}

            //DataStore<Configuration>.DataManager = specification.SharedData.DataManager;
            //CSPDataStore.DataManager = this.SharedData.DataManager;
            //Specification.AlphaDatabase = specification.SharedData.AlphaDatabase;

            Valuation.VariableLowerBound = this.VariableLowerBound;
            Valuation.VariableUpperLowerBound = this.VariableUpperLowerBound;
            Valuation.ValutionHashTable = this.ValutionHashTable;

            Valuation.HiddenVars = this.LocalVars;
            //Valuation.WildVars = this.WildVars;
            Valuation.HasVariableConstraints = Valuation.VariableLowerBound.Count > 0 || Valuation.VariableUpperLowerBound.Count > 0;                       

            SpecificationBase.HasSyncrhonousChannel = this.HasSyncrhonousChannel;
            SpecificationBase.SyncrhonousChannelNames = this.SyncrhonousChannelNames;

            SpecificationBase.HasAtomicEvent = this.HasAtomicEvent;
            SpecificationBase.HasCSharpCode = this.HasCSharpCode;
            
            //AssertionBase.FairnessType = this.SharedData.FairnessType;
            //AssertionBase.CalculateParticipatingProcess = this.CalculateParticipatingProcess;

            Common.Ultility.Ultility.CSharpMethods = this.CSharpMethods;
            Common.Ultility.Ultility.CSharpDataType = this.CSharpDataType;
            //Common.Ultility.Ultility.MacroDefinition = this.MacroDefinition;
        }
    }

}
