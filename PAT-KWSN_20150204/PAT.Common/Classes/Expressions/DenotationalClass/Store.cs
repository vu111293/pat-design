using System.Collections.Generic;

namespace Dike.ModelChecking.Expression
{
    public class Store
    {
        private List<Value> a;

        public Store()
        {
            a = new List<Value>();
        }

        public virtual void Update(int loc, Value v)
        {
            a[loc] = v;
        }

        public virtual int Extend(Value v)
        {
            a.Add(v);
            return a.Count - 1;
        }

        public virtual Value GetValue(int loc)
        {
            return a[loc];
        }
    }
}
