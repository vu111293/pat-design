using System;
using System.Collections.Generic;

namespace PAT.Common.Classes.Expressions.ExpressionClass
{
    public class UserDefinedDataType :  ExpressionValue
    {

        //public UserDefinedDataType()
        //{
        //    this.ExpressionType = ExpressionType.UserDefinedDataType;
        //}

        public ExpressionValue GetClone()
        {
            throw new NotImplementedException();
        }

        public string GetID()
        {
            throw new NotImplementedException();
        }

        public List<string> Vars
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        //public ExpressionType ExpressionType
        //{
        //    get { throw new NotImplementedException(); }
        //}

        public void BuildVars()
        {
            throw new NotImplementedException();
        }

        public Expression ClearConstant(Dictionary<string, Expression> constMapping)
        {
            throw new NotImplementedException();
        }
    }
}
