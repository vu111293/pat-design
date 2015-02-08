
using System;

namespace Dike.ModelChecking.Expression
{

    // We need a new Environment class where a variable corresponds
    // to an Integer object which is initialized by 'loc' of int type passed
    // to the methods.
    // However, Since FunValue needs the class Environment, 
    // we here define a subclass of Environment rather than a new Environment
    // class.  
    public class Environment : Environment
    {
        public virtual Environment oPLExtend(String x, int loc)
        {
            Environment e = (Environment)Clone();

            //e[x] = (System.Int32)loc;
            e.Add(x, loc);
            return e;
        }

        public virtual void oPLExtendDestructive(String x, int loc)
        {
            //             System.Object tempObject;
            //             tempObject = this[x];
            //             this[x] = (System.Int32)loc;
            //             System.Object generatedAux2 = tempObject;

            Add(x, loc);
        }

        public virtual int oPLAccess(String x)
        {
            return ((Int32)this[x]);
        }
    }
}