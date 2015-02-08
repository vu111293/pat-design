using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.SemanticModels.Prob.SystemStructure
{
    public class TimeBound
    {
        public Expression lBound;
        public Expression uBound;

        /// <summary>
        /// true: >, false: >= 
        /// </summary>
        public bool lBoundStrict = false;

        /// <summary>
        /// true: <, false: <=
        /// </summary>
        public bool uBoundStrict = false;
    }
}
