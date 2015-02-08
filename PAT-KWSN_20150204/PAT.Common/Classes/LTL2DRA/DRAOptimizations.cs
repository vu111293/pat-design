using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using PAT.Common.Classes.LTL2DRA.common;

namespace PAT.Common.Classes.LTL2DRA
{
    public class DRAOptimizations
    {

        /** 
         * Perform quotienting using bisimulation
         * @param dra the DRA to be optimized
         * @param printColoring print colorings on std::cerr?
         * @param detailedStates save detailed information on the interals in the state?
         * @param printStats print statistics on std::cerr?
         * @return shared_ptr to the quotiented DRA
         */
        public DRA optimizeBisimulation(DRA dra)
        {
            bool printColoring = false;
            bool detailedStates = false;
            bool printStats = false;

            //if (!dra.isCompact()) {dra.makeCompact();}

            List<int> states = new List<int>(dra.size());

            for (int i = 0; i < dra.size(); i++)
            {
                //states[i] = i;
                states.Add(i);
            }

            AcceptanceSignatureContainer accsig_container = new AcceptanceSignatureContainer(dra);
            AcceptanceSignatureComparator accsig_comp = new AcceptanceSignatureComparator(accsig_container);


            Coloring coloring = new Coloring(dra, detailedStates);
            // generate initial coloring by running with the 
            // different acceptance signature
            Coloring new_coloring = generateColoring(states, coloring, accsig_comp);
            //delete coloring;
            coloring = new_coloring;

            int old_size = dra.size();
            int initial_partition = coloring.countColors();

            int oldColors;
            do
            {
                oldColors = coloring.countColors();

                ColoredStateComparator cnc = new ColoredStateComparator(coloring, dra);

                Coloring new_coloring_temp = generateColoring(states, coloring, cnc);
                //delete coloring;
                coloring = new_coloring_temp;
            } while (oldColors != coloring.countColors());

            //if (printColoring) {
            //  std::cerr << *coloring << std::endl;
            //}

            DRA dra_new = generateDRAfromColoring(dra, coloring, detailedStates);
            //delete coloring;

            int new_size = dra_new.size();

            //if (printStats) {
            //  std::cerr << "Bisimulation: From (" << old_size << ") To (" << new_size << ") Initial: (" << initial_partition << ")" << std::endl;
            //}
            return dra_new;
        }


        /**
         * Generate a new coloring based on the Comparator comp 
         * (one iteration of refinement)
         * @param states A vector of the states
         * @param coloring The current coloring
         * @param comp the Comparator
         * @return a pointer to a newly created Coloring, memory ownership
         *         passes to the caller
         */
        //template <class Comparator>
        private Coloring generateColoring(List<int> states, Coloring coloring, Comparator comp)
        {

            //std::sort(states.begin(), states.end(), comp);
            states.Sort();

            Coloring result = new Coloring(coloring.size(), coloring.getFlagDetailed());

            if (states.Count == 0)
            {
                return result;
            }

            //state_vector::reverse_iterator current = states.rbegin(), last = states.rbegin();
            int current = states[states.Count - 1];
            int last = states[states.Count - 1];

            result.setColor(current, result.newColor());

            for (int i = states.Count - 1; i >= 0; i--)//////-1 or -2?
            {
                current = states[i];
                // because states is sorted and we traverse 
                // from the end, either:
                //    *current  < *last with comp(current,last)==true
                // or *current == *last with !comp(current,last)
                if (comp.comp(current, last))
                {
                    // -> we have to start a new color
                    result.setColor(current, result.newColor());
                }
                else
                {
                    // -> more of the same, we stay with this color
                    result.setColor(current, result.currentColor());
                }

                last = current;
            }

            ////while (++current != states.rend())
            //while (++current != states[0])
            //{
            //    // because states is sorted and we traverse 
            //    // from the end, either:
            //    //    *current  < *last with comp(current,last)==true
            //    // or *current == *last with !comp(current,last)
            //    if (comp.comp(current, last))
            //    {
            //        // -> we have to start a new color
            //        result.setColor(current, result.newColor());
            //    }
            //    else
            //    {
            //        // -> more of the same, we stay with this color
            //        result.setColor(current, result.currentColor());
            //    }

            //    last = current;
            //}

            return result;
        }

        /**
   * Generate a new DRA from a coloring
   */
        DRA generateDRAfromColoring(DRA oldDRA, Coloring coloring, bool detailedStates)
        {
            DRA newDRA = oldDRA.createInstance(oldDRA.getAPSet()) as DRA;


            newDRA.acceptance().newAcceptancePairs(oldDRA.acceptance().size());

            for (int color = 0; color < coloring.countColors(); ++color)
            {
                newDRA.newState();
            }

            int old_start_state = oldDRA.getStartState().getName();
            int start_state_color = coloring.state2color(old_start_state);

            newDRA.setStartState(newDRA.get(start_state_color));

            APSet apset = newDRA.getAPSet();

            for (int color = 0; color < coloring.countColors(); ++color)
            {
                DA_State new_state = newDRA.get(color);

                int old_state_representative = coloring.color2state(color);

                DA_State old_state = oldDRA[old_state_representative];

                if (detailedStates)
                {
                    BitSet old_states = coloring.color2states(color);

                    // create new description...
                    if (old_states.cardinality() == 1)
                    {
                        if (old_state.hasDescription())
                        {
                            new_state.setDescription(old_state.getDescription());
                        }
                    }
                    else
                    {
                        //std::ostringstream s;
                        //s << "<TABLE BORDER=\"1\" CELLBORDER=\"0\"><TR><TD>{</TD>";
                        StringBuilder s = new StringBuilder(@"<TABLE BORDER=\""1\"" CELLBORDER=\""0\""><TR><TD>{</TD>");

                        bool first = true;
                        for (int it = BitSetIterator.start(old_states); it != BitSetIterator.end(old_states); it = BitSetIterator.increment(old_states, it))
                        {
                            if (first)
                            {
                                first = false;
                            }
                            else
                            {
                                s.Append("<TD>,</TD>");
                            }

                            s.Append("<TD>");
                            if (!oldDRA[it].hasDescription())
                            {
                                s.Append(it);
                            }
                            else
                            {
                                s.Append(oldDRA[it].getDescription());
                            }
                            s.Append("</TD>");
                        }
                        s.Append("<TD>}</TD></TR></TABLE>");

                        new_state.setDescription(s.ToString());
                        ;
                    }
                }

                // Create appropriate acceptance conditions
                int old_state_index = old_state.getName();
                for (int i = 0; i < oldDRA.acceptance().size(); ++i)
                {
                    if (oldDRA.acceptance().isStateInAcceptance_L(i, old_state_index))
                    {
                        new_state.acceptance().addTo_L(i);
                    }

                    if (oldDRA.acceptance().isStateInAcceptance_U(i, old_state_index))
                    {
                        new_state.acceptance().addTo_U(i);
                    }
                }

                //for (APSet::element_iterator label=apset.all_elements_begin();label!=apset.all_elements_end();++label) 
                for (int label = apset.all_elements_begin(); label != apset.all_elements_end(); ++label)
                {

                    DA_State old_to = old_state.edges().get(new APElement(label));

                    int to_color = coloring.state2color(old_to.getName());

                    new_state.edges().addEdge(new APElement(label), newDRA.get(to_color));
                }
            }

            return newDRA;
        }

    }


    /** Helper class, storing a coloring of the states */
    class Coloring
    {

        /** The number of colors */
        private int _nr_of_colors;

        /** mapping state_id -> color */
        private List<int> _coloring;

        /** Keep detailed information of the equivalence classes? */
        private bool _detailed;

        /** 
         * mapping from color 
         * -> the state ids which are colored alike
         * only used when _detailed=true */
        private List<BitSet> _color2states;

        /** 
         * mapping from color -> one representative state
         */
        List<int> _color2state;

        /** 
         * Constructor, get initial size of the coloring from DRA.
         * @param dra the DRA
         * @param detailed Keep detailed information on the equivalence classes?
         */
        public Coloring(DRA dra, bool detailed) //=false
        {
            _nr_of_colors = 0;
            _detailed = detailed;
            _coloring = new List<int>();
            Ultility.resize(_coloring, dra.size());
            _color2state = new List<int>();

            //_coloring.resize(dra.size());
            if (_detailed)
            {
                _color2states = new List<BitSet>();
            }
            else
            {
                _color2states = null;
            }
        }

        /** 
       * Constructor, explicitly set initial size of the coloring
       * @param size the initial size
       * @param detailed Keep detailed information on the equivalence classes?
       */
        public Coloring(int size, bool detailed) //=false
        {
            _nr_of_colors = 0;
            _detailed = detailed;

            // _coloring.resize(size);
            _coloring = new List<int>();
            Ultility.resize(_coloring, size);

            _color2state = new List<int>();

            if (_detailed)
            {
                _color2states = new List<BitSet>();
            }
            else
            {
                _color2states = null;
            }

        }

        /** Reset (clear) coloring. */
        public void reset() { _nr_of_colors = 0; }

        /** Get the flag 'detailed' */
        public bool getFlagDetailed() { return _detailed; }

        /** Returns the size (number of states) of this coloring. */
        public int size() { return _coloring.Count; }

        /** 
         * Create a new color
         * @return the newly created color
         */
        public int newColor()
        {
            _nr_of_colors++;

            //_color2state.resize(_nr_of_colors);
            Ultility.resize(_color2state, _nr_of_colors);


            if (_detailed)
            {
                //_color2states->resize(_nr_of_colors);
                Ultility.resize(_color2states, _nr_of_colors);
            }

            return _nr_of_colors - 1;
        }

        /** Return the current (last created) color */
        public int currentColor()
        {
            Debug.Assert(_nr_of_colors > 0);
            return _nr_of_colors - 1;
        }

        /** Return the number of colors */
        public int countColors()
        {
            return _nr_of_colors;
        }

        /** Set the color of a state */
        public void setColor(int state, int color)
        {
            Debug.Assert(color < _nr_of_colors);

            _coloring[state] = color;
            _color2state[color] = state;

            if (_detailed)
            {
                _color2states[color].set(state);
            }
        }

        /** Get the color for a state */
        public int state2color(int state)
        {
            return _coloring[state];
        }

        /**
         *Get one representative state for the equivalence class with the 
         * specified color. 
         */
        public int color2state(int color)
        {
            Debug.Assert(color < _nr_of_colors);
            return _color2state[color];
        }

        /** 
       * Get the state indizes (in a BitSet) that have the specified color. 
       * Can only be called, when the 'detailed' flag is activated in the
       * constructor.
       */
        public BitSet color2states(int color)
        {
            Debug.Assert(color < _nr_of_colors);
            Debug.Assert(_detailed && _color2states != null);
            return _color2states[color];
        }

        ///** Print the coloring */
        //friend std::ostream& operator<<(std::ostream& out,
        //                const Coloring& coloring) {
        //  for (unsigned int i=0;i<coloring.size();i++) {
        //out << "color[" <<  i << "] = " << coloring.state2color(i) << std::endl;
        //  }

        //  return out;
        //}


        /** Dummy Copy constructor */
        //private Coloring(Coloring other);
    }

    interface Comparator
    {
        bool comp(int state_x, int state_y);
    }

    /** 
   * Functor, provides a 'less-than' Comparator 
   * for the states of the DRA, using the color of 
   * the states themself and the colors of the
   * to-states of the edges.
   */
    class ColoredStateComparator : Comparator
    {

        /** The coloring */
        private Coloring _coloring;
        /** The DRA */
        DRA _dra;


        /** Constructor */
        public ColoredStateComparator(Coloring coloring, DRA dra)
        {
            _coloring = coloring;
            _dra = dra;
        }

        /**
         * Compares two states 'less-than' using the
         * coloring, uses the bisimulation
         * equivalence relation to determine
         * equality.
         */
        //bool operator()(int state_x, int state_y) 
        public bool comp(int state_x, int state_y)
        {
            int cx = _coloring.state2color(state_x);
            int cy = _coloring.state2color(state_y);

            if (cx < cy)
            {
                return true;
            }
            else if (cx > cy)
            {
                return false;
            }


            //for (APSet::element_iterator label= _dra.getAPSet().all_elements_begin();label!=_dra.getAPSet().all_elements_end();++label) 
            for (int label = _dra.getAPSet().all_elements_begin(); label != _dra.getAPSet().all_elements_end(); ++label)
            {
                DA_State to_x = _dra[state_x].edges().get(new APElement(label));
                DA_State to_y = _dra[state_y].edges().get(new APElement(label));

                int ctx = _coloring.state2color(to_x.getName());
                int cty = _coloring.state2color(to_y.getName());

                if (ctx < cty)
                {
                    return true;
                }
                else if (ctx > cty)
                {
                    return false;
                }
            }

            // we get here only if x and y are equal with this
            // coloring -> return false
            return false;
        }
    }

    /** 
   * A container that stores (caches) the acceptance signatures of
   * all the states in a DRA.
   */
    class AcceptanceSignatureContainer
    {

        /** Type of an acceptance signature */
        // public KeyValuePair<BitSet, BitSet> acceptance_signature_t;

        /** 
         * Constructor, fills the container with the acceptance signatures of the states.
         * @param dra the DRA
         */
        public AcceptanceSignatureContainer(DRA dra)
        {

            //_acceptancesig_vector.resize(dra.size());
            _acceptancesig_vector = new List<KeyValuePair<BitSet, BitSet>>();
            //Ultility.resize(_acceptancesig_vector, dra.size());

            _bitsets = new List<BitSet>();

            for (int i = 0; i < dra.size(); i++)
            {
                BitSet b = dra.acceptance().getAcceptance_L_forState(i);
                BitSet bp = new BitSet(b);
                _bitsets.Add(bp); //push_back
                // _acceptancesig_vector[i].Key = bp;

                b = dra.acceptance().getAcceptance_U_forState(i);
                BitSet bp1 = new BitSet(b);
                _bitsets.Add(bp1); //
                //_acceptancesig_vector[i].Value = bp1;

                _acceptancesig_vector.Add(new KeyValuePair<BitSet, BitSet>(bp, bp1));
            }
        }

        ///** Destructor */
        //~AcceptanceSignatureContainer() {
        //  for (std::vector<BitSet*>::iterator it=_bitsets.begin();
        //   it!=_bitsets.end();
        //   ++it) {
        //delete *it;
        //  }
        //}

        /** 
         * Get the acceptance signature for state i.
         * @param i the state index
         */
        public KeyValuePair<BitSet, BitSet> get(int i)
        {
            return _acceptancesig_vector[i];
        }


        /** Type of a vector (state-id -> acceptance_signature) */
        //typedef std::vector<acceptance_signature_t> acceptancesig_vector_t;

        /** Storage for the acceptance signatures */
        private List<KeyValuePair<BitSet, BitSet>> _acceptancesig_vector;

        /**
         * Vector to store the BitSet pointers that have to
         * be cleaned up on destruction.
         */
        private List<BitSet> _bitsets;
    }

    /** 
 * Functor that compares two DRA states based on their
 * acceptance signature.
 */
    class AcceptanceSignatureComparator : Comparator
    {

        /** The acceptance signature container */
        private AcceptanceSignatureContainer _container;

        /** Constructor */
        public AcceptanceSignatureComparator(AcceptanceSignatureContainer container)
        {
            _container = container;
        }

        /** 
         * Compares (less-than) two DRAState indizes based on their
         * acceptance signature.
         */
        public bool comp(int x, int y)
        {
            //typename AcceptanceSignatureContainer::acceptance_signature_t px, py;

            KeyValuePair<BitSet, BitSet> px = _container.get(x);
            KeyValuePair<BitSet, BitSet> py = _container.get(y);

            if (px.Key < py.Key)
            {
                return true;
            }
            else if (py.Key < px.Key)
            {
                return false;
            }

            if (px.Value < py.Value)
            {
                return true;
            }

            // py.second >= px.second
            return false;
        }
    }




}
