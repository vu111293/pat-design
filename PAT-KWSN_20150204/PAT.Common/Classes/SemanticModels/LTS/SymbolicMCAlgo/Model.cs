using System;
using System.Collections.Generic;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System.Diagnostics;


namespace PAT.Common.Classes.SemanticModels.LTS.BDD
{
    /// <summary>
    /// Manage the modelling process
    /// </summary>
    public partial class Model
    {   
        /// <summary>
        /// For any array element A[i], its corresponding name is "A + NAME_SEPERATOR + i"
        /// For each event parameter i.th, its corresponding name is "EVENT_NAME + NAME_SEPERATOR + i"
        /// 
        /// </summary>
        public const string NAME_SEPERATOR = "#";
        
        public const string TEMPORARY_VARIABLE = "#temp_var";

        public static int BDD_INT_UPPER_BOUND = 127;
        public static int BDD_INT_LOWER_BOUND = 0;

        public const string TOP_CHANNEL = "#Top_";
        public const string COUNT_CHANNEL = "#Count_";
        public const string SIZE_ELEMENT_CHANNEL = "#Size_";

        public static int MAX_MESSAGE_LENGTH = 2;
        public static int MIN_ELEMENT_BUFFER = 0;
        public static int MAX_ELEMENT_BUFFER = 31;

        public const int TAU_EVENT_INDEX = 0;
        public const int TERMINATE_EVENT_INDEX = 1;
        public const int TOCK_EVENT_INDEX = 2;

        public static int NUMBER_OF_EVENT = 0;
        public const string EVENT_NAME = "#event";
        public static int MAX_NUMBER_EVENT_PARAMETERS = 2;

        /// <summary>
        /// List of variables to manage the event parameter values
        /// </summary>
        public List<string> eventParameterVariables = new List<string>();

        public static int[] MAX_EVENT_INDEX = new int[] {31, 31};
        public static int[] MIN_EVENT_INDEX = new int[] {0, 0};

        public static void ResetDefaultValue()
        {
            BDD_INT_LOWER_BOUND = 0;
            BDD_INT_UPPER_BOUND = 127;

            MAX_MESSAGE_LENGTH = 2;
            MIN_ELEMENT_BUFFER = 0;
            MAX_ELEMENT_BUFFER = 31;

            MAX_NUMBER_EVENT_PARAMETERS = 2;
            MAX_EVENT_INDEX = new int[] { 31, 31 };
            MIN_EVENT_INDEX = new int[] { 0, 0 };
        }

        private int numverOfBoolVars = 0;
        public int NumberOfBoolVars { get { return numverOfBoolVars; } }

        private VariableList varList = new VariableList();
        private Dictionary<string, List<int>> arrayRange = new Dictionary<string, List<int>>();
        public Dictionary<string, int> mapChannelToSize = new Dictionary<string, int>();

        /// <summary>
        /// List of global variables' indexes
        /// </summary>
        public List<int> GlobalVarIndex = new List<int>();


        /// <summary>
        /// Collect all row variables in one vector
        /// </summary>
        public CUDDVars AllRowVars = new CUDDVars();

        public CUDDVars AllRowVarsExceptSingleCopy = new CUDDVars();

        public CUDDVars AllEventVars = new CUDDVars();

        /// <summary>
        /// Collect all column variables in one vector
        /// </summary>
        public CUDDVars AllColVars = new CUDDVars();

        /// <summary>
        /// Seperate each row variable in one vector CUDDVars
        /// </summary>
        private List<CUDDVars> rowVars = new List<CUDDVars>();

        /// <summary>
        /// Seperate each column variable in one vector CUDDVars
        /// </summary>
        private List<CUDDVars> colVars = new List<CUDDVars>();

        /// <summary>
        /// Store as AND-explicit
        /// </summary>
        public List<CUDDNode> colVarRanges = new List<CUDDNode>();

        public CUDDNode allRowVarRanges;

        public List<CUDDNode> variableEncoding = new List<CUDDNode>();

        /// <summary>
        /// list of variable unchanged x' = x
        /// </summary>
        public List<CUDDNode> varIdentities = new List<CUDDNode>();

        public Model()
        {
            Model.tempVariableCount = 0;
            CUDD.InitialiseCUDD(2048* 1024, Math.Pow(10, -15));

            allRowVarRanges = CUDD.Constant(1);
        }


        /// <summary>
        /// Close the model, dereference variables
        /// </summary>
        public void Close()
        {
            this.AllRowVarsExceptSingleCopy.Deref();
            this.AllColVars.Deref();

            CUDD.Deref(this.varIdentities);
            CUDD.Deref(this.variableEncoding);
            CUDD.Deref(this.colVarRanges);
            CUDD.Deref(allRowVarRanges);

            CUDD.CloseDownCUDD();
        }

        /// <summary>
        /// Return List of lower bound and upper bound of the array
        /// </summary>
        /// <param name="name">Name of the array</param>
        /// <returns></returns>
        public List<int> GetArrayRange(string name)
        {
            return arrayRange[name];
        }

        private void AddArrayRange(string arrayName, int lower, int upper)
        {
            this.arrayRange.Add(arrayName, new List<int> { lower, upper });
        }


        /// <summary>
        /// Add new local variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void AddLocalVar(string name, int lower, int upper)
        {
            varList.AddNewVariable(name, lower, upper);
            int numBits = varList.GetNumberOfBits(name);

            CUDDNode vr, vc;
            CUDDVars rowVar = new CUDDVars();
            CUDDVars colVar = new CUDDVars();
            for (int j = 0; j < numBits; j++)
            {
                vr = CUDD.Var(numverOfBoolVars++);
                vc = CUDD.Var(numverOfBoolVars++);
                rowVar.AddVar(vr);
                colVar.AddVar(vc);
            }
            this.rowVars.Add(rowVar);
            this.colVars.Add(colVar);

            this.AllRowVars.AddVars(rowVar);
            this.AllRowVarsExceptSingleCopy.AddVars(rowVar);
            this.AllColVars.AddVars(colVar);

            // used for unchanged variable in transition.
            CUDDNode identity = CUDD.Constant(0);
            for (int i = lower; i <= upper; i++)
            {
                identity = CUDD.Matrix.SetMatrixElement(identity, rowVar, colVar, i - lower, i - lower, 1);
            }
            this.varIdentities.Add(identity);

            //
            CUDDNode expressionDD = CUDD.MINUS_INFINITY; CUDD.Ref(expressionDD);

            for (int i = lower; i <= upper; i++)
            {
                expressionDD = CUDD.Matrix.SetVectorElement(expressionDD, rowVar, i - lower, i);
            }
            this.variableEncoding.Add(expressionDD);

            //
            CUDD.Ref(identity);
            allRowVarRanges = CUDD.Function.And(allRowVarRanges, CUDD.Abstract.ThereExists(identity, colVar));

            CUDD.Ref(identity);
            colVarRanges.Add(CUDD.Abstract.ThereExists(identity, rowVar));
        }

        /// <summary>
        /// Add new not-global variable where row- and column- boolean variable are the same.
        /// This function is used to add BDD variables to encode event names
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void AddSingleCopyVar(string name, int lower, int upper)
        {
            varList.AddNewVariable(name, lower, upper);
            int numBits = varList.GetNumberOfBits(name);

            CUDDNode vr;
            CUDDVars rowVar = new CUDDVars();
            
            for (int j = 0; j < numBits; j++)
            {
                vr = CUDD.Var(numverOfBoolVars++);
                rowVar.AddVar(vr);
            }

            this.rowVars.Add(rowVar);
            this.colVars.Add(rowVar);

            this.AllRowVars.AddVars(rowVar);
            this.AllColVars.AddVars(rowVar);
            this.AllEventVars.AddVars(rowVar);

            // used for unchanged variable in transition.
            CUDDNode identity = (numBits == 0) ? CUDD.Constant(1) : CUDD.Matrix.Identity(rowVar, rowVar);
            this.varIdentities.Add(identity);

            //
            CUDDNode expressionDD = CUDD.MINUS_INFINITY; CUDD.Ref(expressionDD);

            for (int i = lower; i <= upper; i++)
            {
                expressionDD = CUDD.Matrix.SetVectorElement(expressionDD, rowVar, i - lower, i);
            }
            this.variableEncoding.Add(expressionDD);

            ////
            //CUDD.Ref(identity);
            //allRowVarRanges = CUDD.Function.And(allRowVarRanges, CUDD.Abstract.ThereExists(identity, rowVar));

            //CUDD.Ref(identity);
            //colVarRanges.Add(CUDD.Abstract.ThereExists(identity, rowVar));
        }
        
        /// <summary>
        /// Add new global variable. Only add global variable at the beginning.
        /// If add later, earlier transitions will not guarantee the unchange condition.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void AddGlobalVar(string name, int lower, int upper)
        {
            AddLocalVar(name, lower, upper);
            //
            this.GlobalVarIndex.Add(this.rowVars.Count - 1);
            //AddVariableRange(name, lower, upper);
        }

        /// <summary>
        /// Add new global array variable. Only add global variable at the beginning.
        /// If add later, earlier transitions will not guarantee the unchange condition.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void AddGlobalArray(string name, int size, int lower, int upper)
        {
            for (int i = 0; i < size; i++)
            {
                this.AddGlobalVar(name + NAME_SEPERATOR + i, lower, upper);
            }
            this.AddArrayRange(name, 0, size - 1);
        }

        /// <summary>
        /// Add new local array variable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        public void AddLocalArray(string name, int size, int lower, int upper)
        {
            for (int i = 0; i < size; i++)
            {
                this.AddLocalVar(name + NAME_SEPERATOR + i, lower, upper);
            }
            this.AddArrayRange(name, 0, size - 1);
        }

        public void AddGlobalChannel(string name, int size)
        {
            mapChannelToSize.Add(name, size);

            int lenghtOfBuffer = size;
            int maxMessageLength = Model.MAX_MESSAGE_LENGTH;

            //lowerBound, upperBound value of the buffer element value
            int lowerBound = MIN_ELEMENT_BUFFER;
            int upperBound = MAX_ELEMENT_BUFFER;


            //Array of buffer element
            AddGlobalArray(name, lenghtOfBuffer*maxMessageLength, lowerBound, upperBound);

            //Array of size of buffer element
            AddGlobalArray(GetArrayOfSizeElementChannel(name), lenghtOfBuffer, 1, maxMessageLength);


            //we need to assign top_a++ %L and top_a-- %L: to make this possible, we need to extend the range
            AddGlobalVar(GetTopVarChannel(name), -1, lenghtOfBuffer);
            AddGlobalVar(GetCountVarChannel(name), 0, lenghtOfBuffer);
        }

        /// <summary>
        /// Return row variables used in the dd
        /// [ REFS: '', DEREFS: '' ]
        /// </summary>
        public List<int> GetRowSupportedVars(CUDDNode dd)
        {
            List<int> result = new List<int>();
            //Get changed variables
            CUDDNode supportedVariable = CUDD.GetSupport(dd);

            for (int k = 0; k < this.rowVars.Count; k++)
            {
                CUDD.Ref(supportedVariable);
                CUDDNode temp = CUDD.Abstract.ThereExists(supportedVariable, this.rowVars[k]);

                if (!supportedVariable.Equals(temp))
                {
                    result.Add(k);
                }
                CUDD.Deref(temp);
            }
            CUDD.Deref(supportedVariable);

            return result;
        }

        /// <summary>
        /// return colomn variables used in the dd
        /// [ REFS: '', DEREFS: '' ]
        /// </summary>
        public List<int> GetColSupportedVars(CUDDNode dd)
        {
            List<int> result = new List<int>();
            //Get changed variables
            CUDDNode supportedVariable = CUDD.GetSupport(dd);

            for (int k = 0; k < this.colVars.Count; k++)
            {
                CUDD.Ref(supportedVariable);
                CUDDNode temp = CUDD.Abstract.ThereExists(supportedVariable, this.colVars[k]);

                if (!supportedVariable.Equals(temp))
                {
                    result.Add(k);
                }
                CUDD.Deref(temp);
            }
            CUDD.Deref(supportedVariable);

            return result;
        }

        /// <summary>
        /// For the case t = t + 1. We need change to temp = t + 1; t = temp
        /// </summary>
        public void CreateTemporaryVar()
        {
            if (!this.ContainsVar(TEMPORARY_VARIABLE))
            {
                this.AddLocalVar(TEMPORARY_VARIABLE, BDD_INT_LOWER_BOUND, BDD_INT_UPPER_BOUND);
            }
        }

        /// <summary>
        /// Get the row variable value (before transition) from BDD configuration. This value must be added with Lowerbound value. See the Variable Expression
        /// implementation
        /// [ REFS: '', DEREFS: '']
        /// </summary>
        public int GetRowVarValue(CUDDNode currentStateDD, string variableName)
        {
            CUDDVars notSelectedVariables = new CUDDVars();
            notSelectedVariables.AddVars(this.AllRowVars);
            notSelectedVariables.RemoveVars(this.GetRowVars(variableName));

            CUDD.Ref(currentStateDD);
            CUDDNode selectedEventDD = CUDD.Abstract.ThereExists(currentStateDD, notSelectedVariables);
            int selectedEvent = CUDD.MinTermToInt(selectedEventDD, this.GetRowVars(variableName));
            CUDD.Deref(selectedEventDD);

            //
            selectedEvent += this.varList.GetVarLow(variableName);
            return selectedEvent;
        }

        /// <summary>
        /// Get the column variable value (after transition) from BDD configuration. This value must be added with Lowerbound value. See the Variable Expression
        /// implementation
        /// [ REFS: '', DEREFS: '']
        /// </summary>
        public int GetColVarValue(CUDDNode currentStateDD, string variableName)
        {
            CUDDVars notSelectedVariables = new CUDDVars();
            notSelectedVariables.AddVars(this.AllColVars);
            notSelectedVariables.RemoveVars(this.GetColVars(variableName));

            CUDD.Ref(currentStateDD);
            CUDDNode selectedEventDD = CUDD.Abstract.ThereExists(currentStateDD, notSelectedVariables);
            int selectedEvent = CUDD.MinTermToInt(selectedEventDD, this.GetColVars(variableName));
            CUDD.Deref(selectedEventDD);

            //
            selectedEvent += this.varList.GetVarLow(variableName);
            return selectedEvent;
        }

        /// <summary>
        /// Encode transition with guard, update and unchangedVariables must be set as unchanged if they are not set in update.
        /// [ REFS: 'result', DEREFS: '']
        /// </summary>
        /// <param name="guard">Guard of the transition</param>
        /// <param name="update">Update command of the transition</param>
        /// <param name="unchangedVariables">Set of variable which are needed to set unchanged if actually unchanged in the transition</param>
        public List<CUDDNode> EncodeTransition(Expression guard, Expression update, List<int> unchangedVariables)
        {
            ExpressionBDDEncoding guardBddEncoding = guard.TranslateBoolExpToBDD(this);

            ExpressionBDDEncoding updateBddEncoding = update.TranslateBoolExpToBDD(this);

            //
            List<CUDDNode> transitions = CUDD.Function.And(guardBddEncoding.GuardDDs, updateBddEncoding.GuardDDs);

            return AddVarUnchangedConstraint(transitions, unchangedVariables);
        }

        /// <summary>
        /// For each transition, check if the unchangedVariables is not updated in the transition
        /// then add the unchanged constraint to that transition
        /// [ REFS: 'result', DEREFS: 'transitions']
        /// </summary>
        /// <param name="unchangedVariables">indexes of unchanged variables</param>
        /// <returns>New list of transitions</returns>
        public List<CUDDNode> AddVarUnchangedConstraint(List<CUDDNode> transitions, List<int> unchangedVariables)
        {
            List<CUDDNode> result = new List<CUDDNode>();

            foreach (CUDDNode transition in transitions)
            {
                CUDDNode newTransition = AddVarUnchangedConstraint(transition, unchangedVariables);

                result.Add(newTransition);
            }

            return result;

        }

        /// <summary>
        /// [ REFS: 'result', DEREFS: 'transition']
        /// </summary>
        /// <param name="transition"></param>
        /// <param name="unchangedVariables"></param>
        /// <returns></returns>
        public CUDDNode AddVarUnchangedConstraint(CUDDNode transition, List<int> unchangedVariables)
        {
            List<int> updatedVariableIndexes = this.GetColSupportedVars(transition);

            CUDDNode unchangedUpdates = CUDD.Constant(1);
            foreach (int k in unchangedVariables)
            {
                if (this.IsUnchangedVariable(k, updatedVariableIndexes))
                {
                    //if current variable is not updated
                    CUDD.Ref(this.varIdentities[k]);
                    unchangedUpdates = CUDD.Function.And(unchangedUpdates, this.varIdentities[k]);
                }
            }

            return CUDD.Function.And(transition, unchangedUpdates);
        }

        /// <summary>
        /// Check whether variable is updated in the transtion by its index
        /// </summary>
        private bool IsUnchangedVariable(int index, List<int> updatedVariableIndexes)
        {
            if (updatedVariableIndexes.Contains(index))
            {
                return false;
            }
            else
            {
                if (this.ContainsVar(Model.TEMPORARY_VARIABLE))
                {
                    int temporaryVariableIndex = this.GetVarIndex(Model.TEMPORARY_VARIABLE);
                    if (index == temporaryVariableIndex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static int tempVariableCount = 0;
        /// <summary>
        /// Return a unique temproary variable name
        /// </summary>
        /// <returns></returns>
        public static string GetNewTempVarName()
        {
            return Model.NAME_SEPERATOR + (tempVariableCount++);
        }

        /// <summary>
        /// Get lower-bound of a variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetVarLowerBound(string name)
        {
            return this.varList.GetVarLow(name);
        }

        /// <summary>
        /// Get upper_bound value of a variable
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetVarUpperBound(string name)
        {
            return this.varList.GetVarHigh(name);
        }

        /// <summary>
        /// Return variable's index based on variable's name
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public int GetVarIndex(string varName)
        {
            return this.varList.GetVarIndex(varName);
        }

        /// <summary>
        /// Check whether a variable exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsVar(string name)
        {
            return this.varList.ContainsVar(name);
        }

        /// <summary>
        /// Return the corresponding Row CUDDVars of i.th variable
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CUDDVars GetRowVars(int i)
        {
            return this.rowVars[i];
        }

        /// <summary>
        /// Return the corresponding Row CUDDVars based on variable name
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CUDDVars GetRowVars(string variableName)
        {
            return this.rowVars[this.GetVarIndex(variableName)];
        }

        /// <summary>
        /// Return the corresponding Column CUDDVars of i.th variable
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CUDDVars GetColVars(int i)
        {
            return this.colVars[i];
        }

        /// <summary>
        /// Return the corresponding Column CUDDVars of based on variable name
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CUDDVars GetColVars(string variableName)
        {
            return this.colVars[this.GetVarIndex(variableName)];
        }

        public CUDDVars GetAllEventVars()
        {
            return AllEventVars;
        }

        /// <summary>
        /// Return number of variable declared in the model
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfVars()
        {
            return this.rowVars.Count;
        }

        /// <summary>
        /// Return list of index of event information (event name + para)
        /// </summary>
        /// <returns></returns>
        public List<int> GetEventIndex()
        {
            List<int> result = new List<int>();
            result.Add(GetVarIndex(Model.EVENT_NAME));

            foreach (var eventPara in eventParameterVariables)
            {
                result.Add(GetVarIndex(eventPara));
            }

            return result;
        }
        
        /// <summary>
        /// Swap row and column variables
        /// [ REFS: 'result', DEREFS: 'dd']
        /// </summary>
        /// <param name="dd"></param>
        /// <returns></returns>
        public CUDDNode SwapRowColVars(CUDDNode dd)
        {
            return CUDD.Variable.SwapVariables(dd, this.AllRowVars, this.AllColVars);
        }

        /// <summary>
        /// Swap row and column variables
        /// [ REFS: 'result', DEREFS: 'dds']
        /// </summary>
        /// <param name="dds"></param>
        /// <returns></returns>
        public List<CUDDNode> SwapRowColVars(List<CUDDNode> dds)
        {
            return CUDD.Variable.SwapVariables(dds, this.AllRowVars, this.AllColVars);
        }

        public static string GetTopVarChannel(string channelName)
        {
            return TOP_CHANNEL + channelName;
        }

        public static string GetCountVarChannel(string channelName)
        {
            return COUNT_CHANNEL + channelName;
        }

        public static string GetArrayOfSizeElementChannel(string channelName)
        {
            return SIZE_ELEMENT_CHANNEL + channelName;
        }
    }

}
