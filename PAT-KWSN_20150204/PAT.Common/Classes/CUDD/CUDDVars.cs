using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.CUDDLib
{
    /// <summary>
    /// Wrapper of variables. Each variables is represented as a list of CUDDNode where each node is a boolean variable
    /// </summary>
    public class CUDDVars
    {
        /// <summary>
        /// Array of boolean variables representing the variable
        /// </summary>
        private List<CUDDNode> vars;

        /// <summary>
        /// Pointer to the array of boolean variables
        /// </summary>
        private IntPtr ptr;

        private bool isArrayAllocated;

        public CUDDVars()
        {
            vars = new List<CUDDNode>();
            ptr = IntPtr.Zero;
            isArrayAllocated = false;
        }

        /// <summary>
        /// Add new boolean variable to encode variable
        /// </summary>
        /// <param name="var"></param>
        public void AddVar(CUDDNode var)
        {
            vars.Add(var);
            isArrayAllocated = false;
        }

        /// <summary>
        /// Add boolean variables from ddv
        /// </summary>
        /// <param name="ddv"></param>
        public void AddVars(CUDDVars ddv)
        {
            vars.AddRange(ddv.vars);
            isArrayAllocated = false;
        }

        /// <summary>
        /// Remove boolean variables in ddv
        /// </summary>
        /// <param name="ddv"></param>
        public void RemoveVars(CUDDVars ddv)
        {
            foreach (CUDDNode var in ddv.vars)
            {
                vars.Remove(var);
            }
            isArrayAllocated = false;
        }

        public void Ref()
        {
            CUDD.Ref(this.vars);
        }

        public void Deref()
        {
            CUDD.Deref(this.vars);
        }

        /// <summary>
        /// Return the pointer address of the array of boolean variables in the memory
        /// </summary>
        /// <returns></returns>
        public IntPtr GetArrayPointer()
        {
            if (isArrayAllocated)
            {
                return ptr;
            }
            else
            {
                if (ptr != IntPtr.Zero)
                {
                    CUDD.FreeArray(ptr);
                }
                ptr = BuildArray();
                isArrayAllocated = true;
                return ptr;
            }
        }

        /// <summary>
        /// Return number of boolean variables
        /// </summary>
        /// <returns></returns>
        public int GetNumVars()
        {
            return vars.Count;
        }

        /// <summary>
        /// Create array of boolean variables in memory
        /// </summary>
        /// <returns></returns>
        private IntPtr BuildArray()
        {
            ptr = CUDD.AllocateNodes(vars.Count);
            for(int i = 0; i < vars.Count; i++)
            {
                CUDD.SetElement(this.ptr, vars[i].Ptr, i);
            }
            return ptr;
        }

    }
}