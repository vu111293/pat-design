using System;
using System.Diagnostics;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    /**
 * Class representing a subset of 2^AP where AP is the set of 
 * atomic propositions (APSet). It stores two bits per AP: 
 * One bit to determine if the value of this AP is set, 
 * the second bit to store the value.<br>
 * Additionally, the APMonom can have the special values
 * TRUE or FALSE.<br>
 * Currently, the APSet can only have 31 members.
 */
    public class APMonom
    {
        public static APMonom TRUE
        {
            get
            {
                return new APMonom(0, 0);
            }
        }
        public static APMonom FALSE
        {
            get
            {
                return new APMonom(0, 1);
            }
        }

        /** The bitset for the values */
        private SimpleBitSet bits_value;
        /** The bitset for the occurence */
        private SimpleBitSet bits_set;

        
        /**
         * Constructor.
         */
        public APMonom()
        {
            //added by ly
            bits_set = new SimpleBitSet();
            bits_value = new SimpleBitSet();

            //APMonmType = APMonmType.TRUE;
            bits_set.set(0);
            bits_value.set(0);
        }

        /**
        * Constructor.
        * @param set_bits integer representation of the bits which are set
        * @param value_bits integer representation of the value bits
        */
        public APMonom(int set_bits, int value_bits)
        {
            //added by ly
            bits_set = new SimpleBitSet();
            bits_value = new SimpleBitSet();

            bits_set.set(set_bits);
            bits_value.set(value_bits);

        }

        /**
        * Constructor.
        * @param set_bits integer representation of the bits which are set
        * @param value_bits integer representation of the value bits
        */
        public void SetAPMonom(APMonom AP)
        {
            //APMonmType = AP.APMonmType;
            bits_set.set(AP.bits_set.bitset);
            bits_value.set(AP.bits_value.bitset);
        }

        /**
        * Is the AP set?
        * @param index index of AP
        * @return <b>true</b> if AP is set
        */
        public bool isSet(int index)
        {
            if (!isNormal())
            {
                throw new Exception("Can't get AP, is either TRUE/FALSE!");
                ;
            }
            return bits_set.get(index);
        }

        /**
        * Gets the value for this AP. You can't get the value if the AP is not set.
        * @param index index of AP
        * @return <b>true</b> if AP is true
        */
        public bool getValue(int index)
        {
            if (!isNormal())
            {
                throw new Exception("Can't get AP, is either TRUE/FALSE!");
            }

            if (!bits_set.get(index))
            {
                throw new Exception("Can't get value: AP not set!");

            }

            return bits_value.get(index);
        }

        /**
        * Sets the value for this AP. Implicitly, it also sets the AP to 'set'.
        * @param index index of AP
        * @param value value of AP
        */
        public void setValue(int index, bool value)
        {
            bits_set.set(index, true);
            bits_value.set(index, value);

//#if APMONOM_DEBUG
    Debug.Assert(isNormalized());
//#endif
        }

        /**
        * Perform a logical AND operation of this APMonom with a single AP.
        * @param index index index of AP
        * @param value value of AP
        */
        public void andAP(int index, bool value)
        {
            if (isFalse()) { return; }

            if (!isTrue())
            {
                if (isSet(index) && getValue(index) != value)
                {
                    // contradiction
                    this.SetAPMonom(FALSE);                    
                    return;
                }
            }

            setValue(index, value);
        }

        /**
        * Unsets this AP.
        * @param index index of AP
        */
        public void unset(int index)
        {
            bits_value.set(index, false);
            bits_set.set(index, false);

//#if APMONOM_DEBUG
    Debug.Assert(isNormalized());
//#endif
        }

        /**
        * Checks if this APMonom is equivalent to TRUE.
        * @return <b>true</b> if this APMonom is TRUE
        */
        public bool isTrue()
        {
            return this == TRUE;
        }

        /**
        * Checks if this APMonom is equivalent to FALSE.
        * @return <b>true</b> if this APMonom is FALSE
        */
        public bool isFalse()
        {
            return this == FALSE;
        }

        /**
        * Checks if this APMonom is a normal APMonon (not equivalent to TRUE or FALSE).
        * @return <b>true</b> if this APMonom is normal (not TRUE/FALSE).
        */
        public bool isNormal()
        {
            return bits_set.intValue() != 0;
        }

        /**
        * Provides access to the underlying bitset representing the
        * value (AP occurs in positive or negative form).
        * @return the SimpleBitSet of the values
        */
        public SimpleBitSet getValueBits()
        {
//#if APMONOM_DEBUG
   // Debug.Assert(isNormalized());
//#endif
            return bits_value;
        }

        /**
        * Provides access to the underlying bitset representing the
        * bits that are set (AP occurs).
        * @return the SimpleBitSet of the occuring APs
        */
        public SimpleBitSet getSetBits()
        {
            //#if APMONOM_DEBUG
            //Debug.Assert(isNormalized());
            //#endif
            return bits_set;
        }

        /**
   * Performs an intersection check.
   * @param m1 the first APMonom
   * @param m2 the second APMonom
   * @return <b>true</b> if the intersection of <i>m1</i> and <i>m2</i> is empty.
   */
        public static bool isIntersectionEmpty(APMonom m1, APMonom m2)
        {
            // check if there are contradicting values 
            int set_in_both = m1.getSetBits().bitset & m2.getSetBits().bitset;

            if ((m1.getValueBits().bitset & set_in_both) != (m2.getValueBits().bitset & set_in_both))
            {
                // contradiction 
                return true;
            }
            else
            {
                return false;
            }
        }

        /**
  * Perform logical conjunction with other APMonom.
  * @param other the other APMonom
  */
        public static APMonom operator &(APMonom one, APMonom other)
        {
            if (one.isFalse() || other.isFalse())
            {
                return FALSE;
            }

            if (one.isTrue())
            {
                return other;
            }
            if (other.isTrue())
            {
                return one;
            }

            // both are not TRUE/FALSE:

            if (isIntersectionEmpty(one, other))
            {
                //  return APMonom equivalent to false
                return FALSE;
            }

            // both Monoms are not contradicting...
            int result_set = one.getSetBits().bitset | other.getSetBits().bitset;

            int result_value = one.getValueBits().bitset | other.getValueBits().bitset;

            return new APMonom(result_set, result_value);
        }

        /**
   * Perform 'minus' operation (equal to *this & !other).
   * @param other the other APMonom
   */
        public static APMonom operator -(APMonom one, APMonom other)
        {
            if (one.isFalse())
            {
                // false & anything == false
                return FALSE;
            }

            if (other.isFalse())
            {
                // *this & !(false) == *this & true == *this
                return one;
            }

            if (other.isTrue())
            {
                // *this & !(true) == *this & false == false
                return FALSE;
            }

            // the result will be false, if there are two set bits
            // with equal value
            int set_in_both = one.getSetBits().bitset & other.getSetBits().bitset;

            if ((one.getValueBits().bitset & set_in_both) != ((~other.getValueBits().bitset) & set_in_both))
            {
                // return false;
                return FALSE;
            }

            int result_set = one.getSetBits().bitset | other.getSetBits().bitset;

            int result_value = one.getValueBits().bitset & (~(other.getValueBits().bitset));

            return new APMonom(result_set, result_value);
        }

        /**
   * Checks for equality.
   * @param other the other APMonom
   * @return <b>true</b> if this and the other APMonom are equal
   */
        public static bool operator ==(APMonom one, APMonom other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            return (one.getValueBits() == other.getValueBits()) && (one.getSetBits() == other.getSetBits());
        }

        public static bool operator !=(APMonom one, APMonom other)
        {
            return !(one == other);
        }

        /** Checks to see if the bitset representation is normalized. */
        private bool isNormalized()
        {
            if (isTrue() || isFalse())
            {
                return true;
            }

            return (bits_value.getBitSet() & ~(bits_set.getBitSet())) == 0;
        }
    }
}
