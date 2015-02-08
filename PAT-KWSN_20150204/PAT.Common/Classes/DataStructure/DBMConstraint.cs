using System.Diagnostics;

namespace PAT.Common.Classes.DataStructure
{

    /** Bound *strictness*. Vital constant values *DO NOT CHANGE*. */
    //public const int dbm_STRICT = 0; /**< strict less than constraints:  < x */
    //public const int dbm_WEAK = 1;    /**< less or equal constraints   : <= x */
    /** Bound *strictness*. Vital constant values *DO NOT CHANGE*.
*/
    public enum Strictness
    {
        dbm_STRICT = 0, /**< strict less than constraints:  < x */
        dbm_WEAK = 1    /**< less or equal constraints   : <= x */
    }

    public class DBMConstraint
    {
        public static int dbm_INFINITY = int.MaxValue >> 1; /**< infinity                           */
        public static int dbm_OVERFLOW = int.MaxValue >> 2;  /**< to detect overflow on computations */
      
        public const int dbm_LE_ZERO = 1;                       /**< Less Equal Zero                    */
        public static int dbm_LS_INFINITY = (dbm_INFINITY << 1); /**< Less Strict than infinity          */
        public static int dbm_LE_OVERFLOW = dbm_LS_INFINITY >> 1; /**< to detect overflow on computations */


        public const int dbm_STRICT = 0; /**< strict less than constraints:  < x */
        public const int dbm_WEAK = 1;    /**< less or equal constraints   : <= x */


        public byte i, j;
        public int value;

        public DBMConstraint() { }
        public DBMConstraint(DBMConstraint c) : this(c.i, c.j, c.value) { }
        public DBMConstraint(byte ci, byte cj, int vij)
        {
            i = ci;
            j = cj;
            value = vij;
        }
        public DBMConstraint(byte ci, byte cj, int bound, bool isStrict)
        {
            i = ci;
            j = cj;

            if (isStrict)
            {
                value = ((bound << 1) | 1);
            }
            else
            {
                value = (bound << 1);
            }
        }

        /** Encoding of bound into (strict) less or less equal.
 * @param bound,strict: the bound to encode with the strictness.
 * @return encoded constraint ("raw").
 */
        public static int dbm_bound2raw(int bound, int strict)
        {
            return (bound << 1) | strict;
        }


        /** Encoding of bound into (strict) less or less equal.
         * @param bound,isStrict: the bound to encode with a flag
         * telling if the bound is strict or not.
         * if isStrict is TRUE then dbm_STRICT is taken,
         * otherwise dbm_WEAK.
         * @return encoded constraint ("raw").
         */

        public static int dbm_boundbool2raw(int bound, bool isStrict)
        {
            if (isStrict)
            {
                return (bound << 1) | 1;
            }
            else
            {
                return (bound << 1);
            }
        }

        /** Decoding of raw representation: bound.
         * @param raw: encoded constraint (bound + strictness).
         * @return the decoded bound value.
         */
        public static int dbm_raw2bound(int raw)
        {
            return (raw >> 1);
        }

        /** Make an encoded constraint weak.
         * @param raw: bound to make weak.
         * @pre raw != dbm_LS_INFINITY because <= infinity
         * is wrong.
         * @return weak raw.
         */
        public static
        int dbm_weakRaw(int raw)
        {
            //Debug.Assert(dbm_WEAK == 1);
            //Debug.Assert(raw != dbm_LS_INFINITY);
            return raw | dbm_WEAK; // set bit
        }

        /** Make an encoded constraint strict.
         * @param raw: bound to make strict.
         * @return strict raw.
         */
        public static
        int dbm_strictRaw(int raw)
        {
            //Debug.Assert(dbm_WEAK == 1);
            return raw & ~dbm_WEAK; // set bit
        }

        /** Decoding of raw representation: strictness.
         * @param raw: encoded constraint (bound + strictness).
         * @return the decoded strictness.
         */
        public static Strictness dbm_raw2strict(int raw)
        {
            return (Strictness)(raw & 1);
        }


        /** Tests of strictness.
         * @param raw: encoded constraint (bound + strictness).
         * @return TRUE if the constraint is strict.
         * dbm_rawIsStrict(x) == !dbm_rawIsEq(x)
         */
        public static bool dbm_rawIsStrict(int raw)
        {
            return ((raw & 1) ^ dbm_WEAK) == 1;
        }


        /** Tests of non strictness.
         * @param raw: encoded constraint (bound + strictness).
         * @return TRUE if the constraint is not strict.
         * dbm_rawIsStrict(x) == !dbm_rawIsEq(x)
         */
        public static bool dbm_rawIsWeak(int raw)
        {
            return ((raw & 1) ^ dbm_STRICT) == 1;
        }


        /** Negate the strictness of a constraint.
         * @param strictness: the flag to negate.
         */
        public static bool dbm_negStrict(Strictness strictness)
        {
            return ((int)strictness ^ 1) == 1;
        }


        /** Negate a constraint:
         * neg(<a) = <=-a
         * neg(<=a) = <-a
         * @param c: the constraint.
         */
        public static int dbm_negRaw(int c)
        {
            /* Check that the trick is correct */
            //Debug.Assert(1 - c == dbm_bound2raw(-dbm_raw2bound(c), dbm_negStrict(dbm_raw2strict(c))));
            return 1 - c;
        }


        /** "Weak" negate a constraint:
         * neg(<=a) = <= -a.
         * @pre c is weak.
         */
        public static int dbm_weakNegRaw(int c)
        {
            //Debug.Assert(dbm_rawIsWeak(c));
            //Debug.Assert(2 - c == dbm_bound2raw(-dbm_raw2bound(c), dbm_WEAK));
            return 2 - c;
        }


        /** A valid raw bound should not cause overflow in computations.
         * @param x: encoded constraint (bound + strictness)
         * @return TRUE if adding this constraint to any constraint
         * does not overflow.
         */
        public static bool dbm_isValidRaw(int x)
        {
            return (x == dbm_LS_INFINITY || (x < dbm_LE_OVERFLOW && -x < dbm_LE_OVERFLOW));
        }


        /** Constraint addition on raw values : + constraints - excess bit.
         * @param x,y: encoded constraints to add.
         * @return encoded constraint x+y.
         */
        public static int dbm_addRawRaw(int x, int y)
        {
            Debug.Assert(x <= dbm_LS_INFINITY);
            Debug.Assert(y <= dbm_LS_INFINITY);
            Debug.Assert(dbm_isValidRaw(x));
            Debug.Assert(dbm_isValidRaw(y));
            return (x == dbm_LS_INFINITY || y == dbm_LS_INFINITY) ? dbm_LS_INFINITY : (x + y) - ((x | y) & 1);
        }


        /** Constraint addition:
         * @param x,y: encoded constraints to add.
         * @return encoded constraint x+y.
         * @pre y finite.
         */
        public static
        int dbm_addRawFinite(int x, int y)
        {
            Debug.Assert(x <= dbm_LS_INFINITY);
            Debug.Assert(y < dbm_LS_INFINITY);
            Debug.Assert(dbm_isValidRaw(x));
            Debug.Assert(dbm_isValidRaw(y));
            return x == dbm_LS_INFINITY ? dbm_LS_INFINITY : (x + y) - ((x | y) & 1);
        }

        public static
        int dbm_addFiniteRaw(int x, int y)
        {
            return dbm_addRawFinite(y, x);
        }


        /** Constraint addition.
         * @param x,y: encoded constraints to add.
         * @return encoded constraint x+y.
         * @pre x and y finite.
         */
        public static
        int dbm_addFiniteFinite(int x, int y)
        {
            Debug.Assert(x < dbm_LS_INFINITY);
            Debug.Assert(y < dbm_LS_INFINITY);
            Debug.Assert(dbm_isValidRaw(x));
            Debug.Assert(dbm_isValidRaw(y));
            return (x + y) - ((x | y) & 1);
        }


        /** Specialized constraint addition.
         * @param x,y: finite encoded constraints to add.
         * @pre x & y finite, x or y weak.
         * @return encoded constraint x+y
         */
        public static
        int dbm_addFiniteWeak(int x, int y)
        {
            Debug.Assert(x < dbm_LS_INFINITY);
            Debug.Assert(y < dbm_LS_INFINITY);
            Debug.Assert(dbm_isValidRaw(x));
            Debug.Assert(dbm_isValidRaw(y));
            //Debug.Assert((x | y) & 1);
            return x + y - 1;
        }


        /** Raw constraint increment:
         * @return constraint + increment with test infinity
         * @param c: constraint
         * @param i: increment
         */
        public static int dbm_rawInc(int c, int i)
        {
            return c < dbm_LS_INFINITY ? c + i : c;
        }


        /** Raw constraint decrement:
 * @return constraint + decremen with test infinity
 * @param c: constraint
 * @param d: decrement
 */

        public static int dbm_rawDec(int c, int d)
        {
            return c < dbm_LS_INFINITY ? c - d : c;
        }

        /** Convenience function to build a constraint.
 * @param i,j: indices.
 * @param bound: the bound.
 * @param strictness: strictness of the constraint.
 */

        //public static constraint_t dbm_constraint(byte i, byte j, int bound, strictness_t strictness)
        //{
        //    return new constraint_t(i, j, dbm_bound2raw(bound, strictness));
        //}

        /** 2nd convenience function to build a constraint.
        * @param i,j: indices.
        * @param bound: the bound.
        * @param isStrict: true if constraint is strict
        */
        public static DBMConstraint dbm_constraint2(byte i, byte j, int bound, bool isStrict)
        {
            return new DBMConstraint(i, j, dbm_boundbool2raw(bound, isStrict));
        }


        /** Negation of a constraint.
        * Swap indices i,j, negate value, and toggle the strictness.
        * @param c: constraint to negate.
        * @return negated constraint.
        */
        public static DBMConstraint dbm_negConstraint(DBMConstraint c)
        {
            byte tmp = c.i;
            c.i = c.j;
            c.j = tmp;
            c.value = dbm_negRaw(c.value);
            return c;
        }

        /** Equality of constraints.
        * @param c1, c2: constraints.
        * @return TRUE if c1 == c2.
        */
        public static bool dbm_areConstraintsEqual(DBMConstraint c1, DBMConstraint c2)
        {
            return (c1.i == c2.i && c1.j == c2.j && c1.value == c2.value);
        }


        /** Equality operator for constraint_t. */
        public static bool operator ==(DBMConstraint c1, DBMConstraint c2)
        {
            return c1.i == c2.i && c1.j == c2.j && c1.value == c2.value;
        }

        public static bool operator !=(DBMConstraint c1, DBMConstraint c2)
        {
            return !(c1 == c2);
        }

        /** Comparison operator < defined if C++
        * @param a,b: constraints to compare.
        * @return true: if a < b
        */
        public static bool operator <(DBMConstraint a, DBMConstraint b)
        {
            return (a.i < b.i) || (a.i == b.i && a.j < b.j) || (a.i == b.i && a.j == b.j && a.value < b.value);
        }

        public static bool operator >(DBMConstraint a, DBMConstraint b)
        {
            return !(a < b || a == b);
        }

        /** Negation operator for constraint_t. */
        public static DBMConstraint operator !(DBMConstraint c1)
        {
            return new DBMConstraint(c1.j, c1.i, dbm_negRaw(c1.value));
        }

        //bool operator == (const constraint_t& b) const;
    }
}
