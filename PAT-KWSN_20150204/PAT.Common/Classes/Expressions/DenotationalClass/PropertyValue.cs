using System;

namespace PAT.ModelChecking.Expressions.DenotationalClass
{
    public class PropertyValue : Value
    {
        public String PropertyName;

        public PropertyValue(String propertyname)
        {
            this.PropertyName = propertyname;
        }
    }
}