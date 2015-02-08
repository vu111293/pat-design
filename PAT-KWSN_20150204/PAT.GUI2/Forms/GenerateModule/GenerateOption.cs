/**
 * @author Ma Junwei 1/27/2011
 * 
 * */
using System.Collections.Generic;

namespace PAT.GUI.Forms.GenerateModule
{
    class GenerateOption
    {
        
        #region Option Properties

        public string ModuleName { get; set; }
        public string ModuleCode { get; set; }
        public List<string> CustomSyntax { get; set; }
        public string Semantics { get; set; }
        public bool IsBdd { get; set; }
        public Assertions Assertion { get; set; }
        public string ModuleIconLocation { get; set; }
        public string OutputFolder { get; set; }
        
        #endregion

        #region Assertion Supported
        /// <summary>
        /// Determine whether the assertion option is checked
        /// </summary>
        public class Assertions
        {
            public bool AssertionDeadlock { get; set; }
            public bool AssertionLTL { get; set; }
            public bool AssertionReachability { get; set; }
            public bool AssertionRefinement { get; set; }
            public bool AssertionDeterminism { get; set; }
            public bool AssertionDivergence { get; set; }
        }
        #endregion
    }
}
