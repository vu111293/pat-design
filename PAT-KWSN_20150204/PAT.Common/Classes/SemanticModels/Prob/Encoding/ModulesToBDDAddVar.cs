using System;
using PAT.Common.Classes.CUDDLib;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;


namespace PAT.Common.Classes.SemanticModels.Prob.Encoding
{
    public partial class ModulesToBDD
    {

        /// <summary>
        /// Add boolean variable to encode the system
        /// </summary>
        private void AddVars()
        {
            AllocateDDVars();
            SortDDVars();
            SortIdentities();
            SortRanges();
            CreateVarEncoding();
        }

        private void AllocateDDVars()
        {
            CUDDNode v;
            int bitsForChoices = 0;

            if (modules.modelType == ModelType.MDP)
            {
                //TODO: Different Prism
                //we will have choice in each label, if there are many choice in a certain guard, then we need choice vars.
                //Note that difference module has difference choice variable for each common lable
                //Therefore for each synchorize lable, we need to know what is the current number of bit used for that choice in that lable
                //Suppose label a in module 1 has 3 choices therefore we need 2 bits 0, 1 for module 1
                //Then in module 2, suppose lablel a has 4 choices, then we will need 2 bits 2, 3 for module 2.
                bitsForChoices = (int) Math.Ceiling(modules.GetNumberOfModules()*Math.Log(((double) modules.GetNumberOfCommands())/modules.GetNumberOfModules(), 2));
            }
            // module variable (row/col) vars
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                rowVars.Add(new CUDDVars());
                colVars.Add(new CUDDVars());
            }

            // now allocate variables

            //which label to choose
            if (modules.modelType == ModelType.MDP)
            {
                //0 for empty label 
                //1... for other labels
                int numberOfBitForAllLabels = (int) Math.Ceiling(Math.Log(synchs.Count + 1, 2));
                // allocate vars))
                for (int i = 0; i < numberOfBitForAllLabels; i++)
                {
                    v = CUDD.Var(numverOfBoolVars++);
                    syncVars.Add(v);
                }
            }

            //For choices in one label 
            if (modules.modelType == ModelType.MDP)
            {
                for (int i = 0; i < bitsForChoices; i++)
                {
                    v = CUDD.Var(numverOfBoolVars++);
                    choiceVars.Add(v);
                }
            }


            // allocate dd variables for module variables (i.e. rows/cols)
            // go through all vars in order (incl. global variables)
            // so overall ordering can be specified by ordering in the input file
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                // get number of dd variables needed
                // (ceiling of log2 of range of variable)
                int numberOfBits = varList.GetNumberOfBits(i);
                // add pairs of variables (row/col))
                for (int j = 0; j < numberOfBits; j++)
                {
                    // new dd row variable
                    CUDDNode vr = CUDD.Var(numverOfBoolVars++);
                    // new dd col variable
                    CUDDNode vc = CUDD.Var(numverOfBoolVars++);
                    rowVars[i].AddVar(vr);
                    colVars[i].AddVar(vc);
                }
            }
        }

        private void SortDDVars()
        {
            // put refs for all globals and all vars in each module together
            for (int i = 0; i < modules.GetNumberOfModules(); i++)
            {
                moduleRowVars.Add(new CUDDVars());
                moduleColVars.Add(new CUDDVars());
            }

            // go thru all variables
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                rowVars[i].Ref();
                colVars[i].Ref();
                // check which module it belongs to
                int moduleIndex = varList.GetModuleIndex(i);
                // if global...)
                if (moduleIndex == Modules.DefaultMainModuleIndex)
                {
                    globalRowVars.AddVars(rowVars[i]);
                    globalColVars.AddVars(colVars[i]);
                }
                else
                {
                    moduleRowVars[moduleIndex].AddVars(rowVars[i]);
                    moduleColVars[moduleIndex].AddVars(colVars[i]);
                }
            }

            // put refs for all vars in whole system together
            // go thru all variables
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                // add to list
                rowVars[i].Ref();
                colVars[i].Ref();
                allRowVars.AddVars(rowVars[i]);
                allColVars.AddVars(colVars[i]);
            }

            if (modules.modelType == ModelType.MDP)
            {
                // go thru all syncronising action vars
                for (int i = 0; i < syncVars.Count; i++)
                {
                    // add to list
                    CUDD.Ref(syncVars[i], syncVars[i]);
                    allSynchVars.AddVar(syncVars[i]);
                    allNondetVars.AddVar(syncVars[i]);
                }

                // go thru all local nondet vars
                for (int i = 0; i < choiceVars.Count; i++)
                {
                    // add to list
                    CUDD.Ref(choiceVars[i], choiceVars[i]);
                    allChoiceVars.AddVar(choiceVars[i]);
                    allNondetVars.AddVar(choiceVars[i]);
                }
            }
        }

        private void SortIdentities()
        {
            // variable identities
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                // set each element of the identity matrix
                CUDDNode varIdentity = CUDD.Constant(0);
                int lower = varList.GetVarLow(i);
                int upper = varList.GetVarHigh(i);

                for (int j = lower; j <= upper; j++)
                {
                    varIdentity = CUDD.Matrix.SetMatrixElement(varIdentity, rowVars[i], colVars[i], j - lower, j - lower, 1);
                }

                varIdentities.Add(varIdentity);
            }

            // module identities
            for (int i = 0; i < modules.GetNumberOfModules(); i++)
            {
                // product of identities for vars in module
                CUDDNode moduleIdentity = CUDD.Constant(1);
                for (int j = 0; j < varList.GetNumberOfVar(); j++)
                {
                    if (varList.GetModuleIndex(j) == i)
                    {
                        CUDD.Ref(varIdentities[j]);
                        moduleIdentity = CUDD.Function.And(moduleIdentity, varIdentities[j]);
                    }
                }
                moduleIdentities.Add(moduleIdentity);
            }
        }

        private void SortRanges()
        {
            // initialise raneg for whole system
            allRowVarRanges = CUDD.Constant(1);

            // variable ranges		
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                // obtain range dd by abstracting from identity matrix
                CUDD.Ref(varIdentities[i]);
                CUDDNode varRowRange = CUDD.Abstract.ThereExists(varIdentities[i], colVars[i]);

                // obtain range dd by abstracting from identity matrix
                CUDD.Ref(varIdentities[i]);
                colVarRanges.Add(CUDD.Abstract.ThereExists(varIdentities[i], rowVars[i]));

                // build up range for whole system as we go
                allRowVarRanges = CUDD.Function.And(allRowVarRanges, varRowRange);
            }

            // module ranges
            for (int i = 0; i < modules.GetNumberOfModules(); i++)
            {
                // obtain range dd by abstracting from identity matrix
                CUDD.Ref(moduleIdentities[i]);
                moduleRangeDDs.Add(CUDD.Abstract.ThereExists(moduleIdentities[i], moduleColVars[i]));
            }
        }

        private void CreateVarEncoding()
        {
            // variable identities
            for (int i = 0; i < varList.GetNumberOfVar(); i++)
            {
                // set each element of the identity matrix
                CUDDNode varDD = CUDD.Constant(0);
                int lower = varList.GetVarLow(i);
                int upper = varList.GetVarHigh(i);

                for (int j = lower; j <= upper; j++)
                {
                    varDD = CUDD.Matrix.SetVectorElement(varDD, rowVars[i], j - lower, j);
                }

                variableEncoding.Add(varDD);
            }
        }
    }
}
