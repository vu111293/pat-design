﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PAT.Common.Classes.BDD
{
    public abstract class BDDPairing
    {
        /**
    * Adds the pair (oldvar, newvar) to this table of pairs. This results in
    * oldvar being substituted with newvar in a call to BDD.replace().
    * 
    * Compare to bdd_setpair.
    */
        public abstract void set(int oldvar, int newvar);

        /**
         * Like set(), but with a whole list of pairs.
         * 
         * Compare to bdd_setpairs.
         */
        public void set(int[] oldvar, int[] newvar)
        {
            if (oldvar.Length != newvar.Length)
                throw new BDDException();

            for (int n = 0; n < oldvar.Length; n++)
                this.set(oldvar[n], newvar[n]);
        }

        /**
         * Adds the pair (oldvar, newvar) to this table of pairs. This results in
         * oldvar being substituted with newvar in a call to bdd.replace().  The
         * variable oldvar is substituted with the BDD newvar.  The possibility to
         * substitute with any BDD as newvar is utilized in BDD.compose(), whereas
         * only the topmost variable in the BDD is used in BDD.replace().
         * 
         * Compare to bdd_setbddpair.
         */
        public abstract void set(int oldvar, BDD newvar);

        /**
         * Like set(), but with a whole list of pairs.
         * 
         * Compare to bdd_setbddpairs.
         */
        public void set(int[] oldvar, BDD[] newvar)
        {
            if (oldvar.Length != newvar.Length)
                throw new BDDException();

            for (int n = 0; n < newvar.Length; n++)
                this.set(oldvar[n], newvar[n]);
        }

        /**
         * Defines each variable in the finite domain block p1 to be paired with the
         * corresponding variable in p2.
         * 
         * Compare to fdd_setpair.
         */
        public void set(BDDDomain p1, BDDDomain p2)
        {
            int[] ivar1 = p1.vars();
            int[] ivar2 = p2.vars();
            this.set(ivar1, ivar2);
        }

        /**
         * Like set(), but with a whole list of pairs.
         * 
         * Compare to fdd_setpairs.
         */
        public void set(BDDDomain[] p1, BDDDomain[] p2)
        {
            if (p1.Length != p2.Length)
                throw new BDDException();

            for (int n = 0; n < p1.Length; n++)
                if (p1[n].varNum() != p2[n].varNum())
                    throw new BDDException();

            for (int n = 0; n < p1.Length; n++)
            {
                this.set(p1[n], p2[n]);
            }
        }

        /**
         * Resets this table of pairs by setting all substitutions to their default
         * values (that is, no change).
         * 
         * Compare to bdd_resetpair.
         */
        public abstract void reset();
    }
}
