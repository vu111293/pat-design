using System;

namespace Dike.ModelChecking.Expression
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