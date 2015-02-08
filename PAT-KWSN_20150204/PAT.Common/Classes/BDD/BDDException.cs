using System;
using System.Collections.Generic;
using System.Text;
using PAT.Common.Classes.Expressions.ExpressionClass;

namespace PAT.Common.Classes.BDD
{
    public class BDDException : RuntimeException
    {
        /**
        * Version ID for serialization.
        */
        private static long serialVersionUID = 3761969363112243251L;

        public BDDException() : base("BDDException")
        {

        }

        public BDDException(string s) : base(s)
        {

        }
    }
}
