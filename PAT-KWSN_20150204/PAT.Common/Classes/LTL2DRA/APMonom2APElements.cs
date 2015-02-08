using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class APMonom2APElements
    {

        /** The underlying APSet. */
        private APSet _ap_set;

        /** The underlying APMonom. */
        private APMonom _m;

        /** The current APElement. */
        public APElement _cur_e;

        /** Marker, true if end was reached */
        private bool _end_marker;


        /**
         * Constructor that generates an iterator pointing to the first
         * APElement.
         * @param ap_set the underlying APSet
         * @param m the APMonom over which we iterate
         */
        public APMonom2APElements(APSet ap_set, APMonom m)
        {
            _ap_set = ap_set;
            _m = m;
            _cur_e = new APElement(m.getValueBits());
            _end_marker = false;

            if (m.isFalse())
            {
                _end_marker = true;
            }
        }

        /**
 * Constructor that generates an iterator pointing
 * to a specific APElement.
 * @param ap_set the underlying APSet
 * @param m the APMonom over which we iterate
 * @param cur_e the current APElement
 */
        public APMonom2APElements(APSet ap_set, APMonom m, APElement cur_e)
        {
            _ap_set = ap_set;
            _m = m;
            _cur_e = cur_e;
            _end_marker = false;
            if (m.isFalse())
            {
                _end_marker = true;
            }
        }

        /**
 * Provides an iterator pointing to the first APElement
 * in the subset represented by the APMonom <i>m</i>.
 * @param ap_set the underlying APSet
 * @param m the APMonom over which we iterate
 * @return the iterator.
 */
        public static APMonom2APElements begin(APSet ap_set, APMonom m)
        {
            return new APMonom2APElements(ap_set, m);
        }

        /**
 * Provides an iterator pointing after the last APElement
 * in the subset represented by the APMonom <i>m</i>.
 * @param ap_set the underlying APSet
 * @param m the APMonom over which we iterate
 * @return the iterator.
 */
        public static APMonom2APElements end(APSet ap_set, APMonom m)
        {
            APMonom2APElements m2e = new APMonom2APElements(ap_set, m);
            m2e._end_marker = true;
            return m2e;
        }

        /**
      * Increment the iterator (used by the boost iterator base class).
      */
        public void increment()
        {
            //BitSet set_mask = new BitSet(_m.getSetBits());
            BitSet set_mask = _m.getSetBits().getBitSetObject();
            int i = set_mask.nextClearBit(0);

            while (i < _ap_set.size())
            {
                if (_cur_e.get(i) == false)
                {
                    _cur_e.set(i, true);
                    return;
                }
                else
                {
                    _cur_e.set(i, false);
                    i = set_mask.nextClearBit(i + 1);
                }
            }

            // overflow -> end
            _end_marker = true;
        }

        /**
 * Checks iterators for equality (used by the boost iterator base class).
 */

        public bool equal(APMonom2APElements other)
        {
            return (this._end_marker == other._end_marker) && (this._end_marker || this._cur_e == other._cur_e);
        }


    }
}