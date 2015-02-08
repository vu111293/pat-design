using System.Text;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    /**
    * Class representing one element of 2^AP. 
    * It is currently capable of handling APSets with up 32 Elements,
    * representing one APElement by a SimpleBitSet (=unsigned int).
    */
    public class APElement : SimpleBitSet
    {
        
        public APElement(int value) : base(value)
        {

        }

          /** Copy constructor */
        public APElement(SimpleBitSet sbs) : base(sbs.bitset)
        {

        }

          /** 
   * Convert to a string representation.
   * @param ap_set The underlying APSet
   * @param spaces Print spaces in front of positive AP? 
   */
        public string toString(APSet ap_set)
        {
            return toString(ap_set, true);
        }


        public string toString(APSet ap_set, bool spaces)
        {
            if (ap_set.size() == 0)
            {
                return "\u03A3";
            }
            StringBuilder r = new StringBuilder();
            for (int i = 0; i < ap_set.size(); i++)
            {
                if (i >= 1)
                {
                    r.Append("&");
                }
                if (!this.get(i))
                {
                    r.Append("!");
                }
                else
                {
                    if (spaces)
                    {
                        r.Append(" ");
                    }
                }
                r.Append(ap_set.getAP(i));
            }
            return r.ToString();
        }



  //      /** 
  // * Generate a representation in LBTT format.
  // * @param ap_set The underlying APSet
  // */
  //public string toStringLBTT(APSet ap_set)
  //{
  //    if (ap_set.size() == 0)
  //    {
  //        return "t";
  //    }

  //    string r = "";
  //    for (int i = 0; i + 1 < ap_set.size(); i++)
  //    {
  //        r += "& ";
  //    }

  //    for (int i = 0; i < ap_set.size(); i++)
  //    {
  //        if (!this.get(i))
  //        {
  //            r += "! ";
  //        }
  //        r += ap_set.getAP(i);
  //        r += " ";
  //    }
  //    return r;
  //}

        /**
   * Prefix increment, goes to next APElement.
   * @return an APElement representing the next APElement
   */
        public static APElement operator ++(APElement other)
        {
            other.set(other.get() + 1);
            return other;
        }


        /**
   * Check for equality.
   * @param other the other APElement
   * @return <b>true</b> if this and the other APElement are equal.
   */
        public static bool operator ==(APElement one, APElement other)
        {
            bool? val = Ultility.NullCheck(one, other);
            if (val != null)
            {
                return val.Value;
            }

            return one.getBitSet() == other.getBitSet();
        }

        public static bool operator !=(APElement one, APElement other)
        {
            return !(one.getBitSet() == other.getBitSet());
        }

        /**
        * Get the internal representation.
        */
        //get the int value
        public int intValue()
        {
            return getBitSet();
        }
    }
}
