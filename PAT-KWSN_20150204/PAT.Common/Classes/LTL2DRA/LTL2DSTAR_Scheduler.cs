using System;
using System.Collections.Generic;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.LTL2DRA.exception;

namespace PAT.Common.Classes.LTL2DRA
{
    public class LTL2DSTAR_Scheduler
    {

        /** The LTL2DRA wrapper for Safra's algorithm and the external LTL->NBA translator */
        private LTL2DRA _ltl2dra;

        /** Use limiting? */
        private bool _opt_limits;

        /** The limiting factor */
        private double _alpha;

        /** Print stats on the NBA? */
        private bool _stat_NBA;


        public bool flagOptLimits()
        {
            return _opt_limits;
        }

        /** Constructor
 * @param ltl2dra the wrapper for LTL->NBA and NBA->DRA 
 * @param opt_limits use limiting?
 * @param alpha the limiting factor 
*/
        public LTL2DSTAR_Scheduler(LTL2DRA ltl2dra, bool opt_limits, double alpha)
        {
            _ltl2dra = ltl2dra;
            _opt_limits = opt_limits;
            _alpha = alpha;
            _stat_NBA = false;
        }

        public int calcLimit(int limit)
        {
            if (limit == 0)
            {
                return limit;
            }
            if (flagOptLimits())
            {
                double new_limit = (limit * _alpha) + 1.0;
                if (new_limit > int.MaxValue)
                {
                    limit = 0;
                }
                else
                {
                    limit = (int)new_limit;
                }
            }

            return limit;
        }



        /** 
      * Generate a DRA/DSA for the LTL formula 
      */
        public DRA calculate(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options ltl_opt)
        {
            LTLFormula ltl_p = (ltl.toPNF());

            //if (ltl_opt.verbose_scheduler) {
            //  std::cerr << ltl_p->toStringInfix() << std::endl;
            //}

            LTL2DSTAR_Tree_Start root = new LTL2DSTAR_Tree_Start(ltl_p, ba, ltl_opt, this);

            //if (ltl_opt.verbose_scheduler) {
            //  root.printTree(std::cerr);
            //}

            root.calculate();

            DRA result = root._automaton;
            if (result != null)
            {
                result.setComment(root._comment); //+ getTimingInformation()
            }
            return result;
        }

        /** Get the LTL2DRA wrapper class */
        public LTL2DRA getLTL2DRA()
        {
            return _ltl2dra;
        }

        /** Get the state of the StatNBA flag */
        public bool flagStatNBA() { return _stat_NBA; }

        /** Set the value of the StatNBA flag */
        public void flagStatNBA(bool value) { _stat_NBA = value; }

    }

    /** Base class for the building blocks for the scheduler */
    public abstract class LTL2DSTAR_Tree
    {

        public BuchiAutomata buchiAutomata;
        //public NBA nba;
        public LTLFormula _ltl;
        public LTL2DSTAR_Options _options;
        public int priority;
        public DRA _automaton;
        public string _comment;
        public LTL2DSTAR_Scheduler _sched;

        public List<LTL2DSTAR_Tree> children;

        /** Type of a vector over the children */
        //typedef std::vector<LTL2DSTAR_Tree*> child_vector;

        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */
        protected LTL2DSTAR_Tree(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options options, LTL2DSTAR_Scheduler sched)
        {
            _ltl = ltl;
            buchiAutomata = ba;
            _options = options;
            _sched = sched;

            children = new List<LTL2DSTAR_Tree>();
        }

        ///** Print the tree on output stream */
        //virtual void printTree(std::ostream& out,
        //           unsigned int level=0) {
        //  for (unsigned int i=0;i<level;i++) {
        //out << " ";
        //  }
        //  out << typeid(*this).name() << " = " << this <<
        //"(" <<  _ltl.get() << ")" << "\n";
        //  for (child_vector::iterator it=children.begin();
        //   it!=children.end();
        //   ++it) {
        //(*it)->printTree(out, level+1);
        //  }
        //}

        /** Abstract virtual function for tree generation */
        public abstract void generateTree(); //= 0;

        /** Estimate the size of the automaton */
        public virtual int guestimate()
        {
            return 0;
        }

        /** Hook that is called after calculate() finishes */
        public virtual void hook_after_calculate()
        {

        }

        public virtual void calculate()
        {
            calculate(0, 0);
        }
        /** Calculate the automaton for this building block, by default
         * calculate the automata for the children and then choose the smallest. */
        public virtual void calculate(int level, int limit)
        {
            //if (_options.verbose_scheduler) {
            //      std::cerr << "Calculate ("<< level <<"): " << typeid(*this).name() << std::endl;
            //}

            calculateChildren(level, limit);

            bool first = true;
            //for (child_vector::iterator it=children.begin();it!=children.end();++it) 
            for (int i = 0; i < children.Count; i++)
            {
                LTL2DSTAR_Tree it = children[i];

                if (it._automaton == null)
                {
                    continue;
                }

                if (first)
                {
                    _automaton = it._automaton;
                    _comment = it._comment;
                }
                else
                {
                    if (_automaton.size() > it._automaton.size())
                    {
                        _automaton = it._automaton;
                        _comment = it._comment;
                    }
                }

                first = false;
            }

            hook_after_calculate();
        }

        /** Add a new child */
        public void addChild(LTL2DSTAR_Tree child)
        {
            if (child == null) { return; }

            children.Add(child);
        }

        /** Calculate the automata for the children */
        public void calculateChildren(int level, int limit)
        {
            if (_sched.flagOptLimits())
            {
                DRA _min_automaton;
                int _min_size = 0;

                //for (child_vector::iterator it=children.begin(); it!=children.end(); ++it) 
                for (int i = 0; i < children.Count; i++)
                {
                    LTL2DSTAR_Tree it = children[i];
                    int child_limit;
                    if (_min_size != 0)
                    {
                        if (limit > 0)
                        {
                            child_limit = Math.Min(_sched.calcLimit(_min_size), limit);
                        }
                        else
                        {
                            child_limit = _sched.calcLimit(_min_size);
                        }
                    }
                    else
                    {
                        child_limit = limit;
                    }

                    //  if (_options.verbose_scheduler) {
                    //  std::cerr << " Limit (with alpha) = " << child_limit << std::endl;
                    //}

                    try
                    {
                        it.calculate(level + 1, child_limit);

                        if (it._automaton != null)
                        {
                            if (_min_size == 0 || it._automaton.size() < _min_size)
                            {
                                _min_automaton = it._automaton;
                                _min_size = _min_automaton.size();
                            }
                            else
                            {
                                // delete automaton as it is bigger
                                // than necessary
                                //it._automaton.reset();
                                it._automaton = null;
                            }
                        }
                    }
                    catch (LimitReachedException e)
                    {
                        //it._automaton.reset();
                        it._automaton = null;
                    }
                }
            }
            else
            {
                //for (child_vector::iterator it=children.begin();it!=children.end();++it) {
                for (int i = 0; i < children.Count; i++)
                {
                    LTL2DSTAR_Tree it = children[i];
                    it.calculate(level + 1, limit);
                }
            }
        }

    }




    /** The root building block for the calculation of DRA/DSA */
    public class LTL2DSTAR_Tree_Start : LTL2DSTAR_Tree
    {

        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */
        public LTL2DSTAR_Tree_Start(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options options, LTL2DSTAR_Scheduler sched)
            : base(ltl, ba, options, sched)
        {
            generateTree();
        }

        /** Generate the tree */
        public override void generateTree()
        {
            LTL2DSTAR_Tree_Rabin rabin = null;
            LTL2DSTAR_Tree_Streett streett = null;

            if (_options.automata == automata_type.RABIN || _options.automata == automata_type.RABIN_AND_STREETT)
            {
                rabin = new LTL2DSTAR_Tree_Rabin(_ltl, buchiAutomata, _options, _sched);
            }

            if (_options.automata == automata_type.STREETT || _options.automata == automata_type.RABIN_AND_STREETT)
            {
                streett = new LTL2DSTAR_Tree_Streett(_ltl.negate().toPNF(), buchiAutomata, _options, _sched);
            }

            if (rabin != null && streett != null)
            {
                int rabin_est = rabin.guestimate();
                int streett_est = streett.guestimate();

                //if (_options.verbose_scheduler) {
                //  std::cerr << "NBA-Estimates: Rabin: "<<rabin_est <<
                //    " Streett: " << streett_est << std::endl;
                //}

                if (rabin_est <= streett_est)
                {
                    addChild(rabin);
                    addChild(streett);
                }
                else
                {
                    addChild(streett);
                    addChild(rabin);
                }
            }
            else
            {
                if (rabin != null)
                    addChild(rabin);
                if (streett != null)
                    addChild(streett);
            }


            if (_options.opt_safra.stutter)
            {
                StutterSensitivenessInformation stutter_information = new StutterSensitivenessInformation();
                stutter_information.checkLTL(_ltl);

                if (!stutter_information.isCompletelyInsensitive() && _options.opt_safra.partial_stutter_check)
                {
                    NBA nba = null;
                    NBA complement_nba = null;
                    if (rabin != null)
                    {
                        nba = rabin.getNBA();
                    }
                    else if (streett != null)
                    {
                        nba = streett.getNBA();
                    }

                    if (rabin != null && streett != null)
                    {
                        complement_nba = streett.getNBA();
                    }

                    if (nba == null)
                    {
                        stutter_information.checkPartial(_ltl, buchiAutomata, _sched.getLTL2DRA());//////////add buchiAutomata
                    }
                    else if (complement_nba == null)
                    {
                        stutter_information.checkPartial(nba, buchiAutomata, _ltl.negate().toPNF(), _sched.getLTL2DRA());///////////add buchiAutomata
                    }
                    else
                    {
                        stutter_information.checkNBAs(nba, complement_nba);
                    }
                }

                if (rabin != null)
                {
                    rabin.setStutterInformation(stutter_information);
                }
                if (streett != null)
                {
                    streett.setStutterInformation(stutter_information);
                }
            }
        }
    }

    /** A building block for the calculation of a Rabin automaton 
     * (via Safra, Scheck or Union) */
    public class LTL2DSTAR_Tree_Rabin : LTL2DSTAR_Tree
    {
        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */

        public LTL2DSTAR_Tree_Rabin(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options options, LTL2DSTAR_Scheduler sched)
            : base(ltl, ba, options, sched)
        {
            _tree_normal = null;
            _tree_union = null;
            generateTree();
        }

        /** Estimate the size of the automaton (use the estimate of Safra's
       * building block ) */
        public override int guestimate()
        {
            if (_tree_normal != null)
            {
                return _tree_normal.guestimate();
            }

            return 0;
        }

        /** Hook after calculation */
        public override void hook_after_calculate()
        {
            if (_tree_normal != null && _sched.flagStatNBA())
            {
                _comment = _comment + "+NBAstd=" + guestimate();
            }
        }

        /** Generate the tree */
        public override void generateTree()
        {
            //  if (_options.scheck_path!="") {
            //if (LTL2DSTAR_Tree_Scheck.worksWith(_ltl, _options.verbose_scheduler)) {
            //  addChild(new LTL2DSTAR_Tree_Scheck(_ltl, _options, _sched));

            //}
            // add stuff for path. check here
            //  }

            if (_options.allow_union && _ltl.getRootNode().getType() == type_t.T_OR)
            {
                _tree_union = new LTL2DSTAR_Tree_Union(_ltl, buchiAutomata, _options, _sched);
                addChild(_tree_union);
            }


            if (!((_options.only_union && _options.allow_union) ||
                  (_options.only_safety && _options.safety)))
            {
                _tree_normal = new LTL2DSTAR_Tree_Safra(_ltl, this.buchiAutomata, _options, _sched);
                addChild(_tree_normal);
            }
        }

        public NBA getNBA()
        {
            if (_tree_normal != null)
            {
                return _tree_normal.getNBA();
            }
            return null;
        }

        public void setStutterInformation(StutterSensitivenessInformation stutter_information)
        {
            _stutter_information = stutter_information;
            if (_tree_normal != null)
            {
                _tree_normal.setStutterInformation(_stutter_information);
            }

            if (_tree_union != null)
            {
                _tree_union.setStutterInformation(_stutter_information);
            }
        }

        // memory will be freed by normal tree destructor
        private LTL2DSTAR_Tree_Safra _tree_normal;
        private LTL2DSTAR_Tree_Union _tree_union;

        private StutterSensitivenessInformation _stutter_information;
    }


    /** Building block for the translation from LTL to DRA using Safra's algorithm */
    public class LTL2DSTAR_Tree_Safra : LTL2DSTAR_Tree
    {

        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */
        public LTL2DSTAR_Tree_Safra(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options options, LTL2DSTAR_Scheduler sched)
            : base(ltl, ba, options, sched)
        {
            generateTree();
        }

        /** Generate the tree */
        public override void generateTree()
        {
        }

        /** Translate LTL -> NBA */
        public void generateNBA()
        {
            if (_nba == null)
            {
                _nba = _sched.getLTL2DRA().ltl2nba(_ltl, buchiAutomata);
            }
        }

        public NBA getNBA()
        {
            generateNBA();
            return _nba;
        }

        /** Estimate the size of the DRA (returns the size of the NBA) */
        public override int guestimate()
        {
            generateNBA();
            if (_nba != null)
            {
                return _nba.size();
            }

            return 0;
        }

        /** Translate the LTL formula to DRA using Safra's algorithm */
        public override void calculate(int level, int limit)
        {
            //  if (_options.verbose_scheduler) {
            //std::cerr << "Calculate ("<< level <<"): " << typeid(*this).name() << std::endl;
            //std::cerr << " Limit = " << limit << std::endl;
            //  }

            generateNBA();

            if (_nba == null)
            {
                throw new Exception("Couldn't create NBA from LTL formula");
            };

            _automaton = _sched.getLTL2DRA().nba2dra(_nba, limit, _options.detailed_states, _stutter_information);
            _comment = "Safra[NBA=" + _nba.size() + "]";

            if (_options.optimizeAcceptance)
            {
                _automaton.optimizeAcceptanceCondition();
            }

            if (_options.bisim)
            {
                DRAOptimizations dra_optimizer = new DRAOptimizations();
                _automaton = dra_optimizer.optimizeBisimulation(_automaton); //, false, _options.detailed_states, false
            }
        }


        public void setStutterInformation(StutterSensitivenessInformation stutter_information)
        {
            _stutter_information = stutter_information;
        }

        private NBA _nba;

        public StutterSensitivenessInformation _stutter_information;
    }


    /** Generate DRA by using the union construction on two DRAs */
    public class LTL2DSTAR_Tree_Union : LTL2DSTAR_Tree
    {


        LTL2DSTAR_Tree_Rabin _left_tree;
        LTL2DSTAR_Tree_Rabin _right_tree;

        public LTLFormula _left;
        public LTLFormula _right;

        StutterSensitivenessInformation _stutter_information;

        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */

        public LTL2DSTAR_Tree_Union(LTLFormula ltl, BuchiAutomata ba, LTL2DSTAR_Options options, LTL2DSTAR_Scheduler sched) :
            base(ltl, ba, options, sched)
        {

            _left_tree = null;
            _right_tree = null; //(0)

            _left = _ltl.getSubFormula(_ltl.getRootNode().getLeft());

            _right = _ltl.getSubFormula(_ltl.getRootNode().getRight());

            generateTree();
        }

        /**
         * Generate the tree
         */
        public override void generateTree()
        {
            LTL2DSTAR_Options rec_opt = _options;
            rec_opt.recursion();
            _left_tree = new LTL2DSTAR_Tree_Rabin(_left, buchiAutomata, rec_opt, _sched);
            addChild(_left_tree);
            _right_tree = new LTL2DSTAR_Tree_Rabin(_right, buchiAutomata, rec_opt, _sched);
            addChild(_right_tree);
        }

        /**
         * Perform union construction
         */
        public override void calculate(int level, int limit)
        {
            //  if (_options.verbose_scheduler) {
            //std::cerr << "Calculate ("<< level <<"): " << typeid(*this).name() << std::endl;
            //  }

            try
            {
                children[0].calculate(level + 1, limit);
                children[1].calculate(level + 1, limit);
            }
            catch (LimitReachedException e)
            {
                //_automaton.reset();
                return;
            }

            if (children[0]._automaton == null || children[1]._automaton == null)
            {
                return;
            }

            bool union_trueloop = _sched.getLTL2DRA().getOptions().union_trueloop;
            if (_sched.getLTL2DRA().getOptions().stutter)
            {
                _automaton = DRA.calculateUnionStuttered(children[0]._automaton, children[1]._automaton, _stutter_information, union_trueloop, _options.detailed_states) as DRA;
            }
            else
            {
                _automaton = DRA.calculateUnion(children[0]._automaton, children[1]._automaton, union_trueloop, _options.detailed_states) as DRA;

                /*      _automaton=DRAOperations::dra_union(*children[0]->_automaton, 
                                  *children[1]->_automaton,
                                  union_trueloop,
                                  _options.detailed_states); */
            }
            _comment = "Union{" + children[0]._comment + "," + children[1]._comment + "}";

            if (_options.optimizeAcceptance)
            {
                _automaton.optimizeAcceptanceCondition();
            }

            if (_options.bisim)
            {
                DRAOptimizations dra_optimizer = new DRAOptimizations();
                _automaton = dra_optimizer.optimizeBisimulation(_automaton); //, false, _options.detailed_states, false
            }

            hook_after_calculate();
        }


        public void setStutterInformation(StutterSensitivenessInformation stutter_information)
        {
            _stutter_information = stutter_information;

            //new StutterSensitivenessInformation(
            //new StutterSensitivenessInformation(
            StutterSensitivenessInformation left_stutter_info = stutter_information;
            StutterSensitivenessInformation right_stutter_info = stutter_information;

            if (!stutter_information.isCompletelyInsensitive())
            {
                left_stutter_info.checkLTL(_left);
                right_stutter_info.checkLTL(_right);
            }

            if (!left_stutter_info.isCompletelyInsensitive())
            {
                left_stutter_info.checkPartial(_left_tree.getNBA(), buchiAutomata, _left.negate().toPNF(), _sched.getLTL2DRA());/////////////add buchiautomata
            }

            if (!right_stutter_info.isCompletelyInsensitive())
            {
                right_stutter_info.checkPartial(_right_tree.getNBA(), buchiAutomata, _right.negate().toPNF(), _sched.getLTL2DRA());///////add buchiautomata
            }

            _left_tree.setStutterInformation(left_stutter_info);
            _right_tree.setStutterInformation(right_stutter_info);
        }

    }


    /**
     * Generate Streett automaton by calculating the Rabin automaton
     * for the negated formula
     */
    public class LTL2DSTAR_Tree_Streett : LTL2DSTAR_Tree
    {

        /**
         * Constructor 
         * @param ltl The LTL formula
         * @param options the LTL2DSTAR options
         * @param sched a reference back to the scheduler 
         */

        public LTL2DSTAR_Tree_Streett(LTLFormula ltl, BuchiAutomata ba,
                   LTL2DSTAR_Options options,
                   LTL2DSTAR_Scheduler sched) :
            base(ltl, ba, options, sched)
        {
            generateTree();
        }

        /** Estimate automaton size (use estimate of Rabin building block) */
        public override int guestimate()
        {
            if (children[0] != null)
            {
                return children[0].guestimate();
            }

            return 0;
        }

        //public override void hook_after_calculate()
        //{
        //    throw new NotImplementedException();
        //}

        public NBA getNBA()
        {
            if (_tree_rabin != null)
            {
                return _tree_rabin.getNBA();
            }
            return null;
        }


        /** Generate tree */
        public override void generateTree()
        {
            LTL2DSTAR_Options opt = _options;
            opt.automata = automata_type.RABIN;
            //opt.scheck_path = ""; // disable scheck
            _tree_rabin = new LTL2DSTAR_Tree_Rabin(_ltl, this.buchiAutomata, opt, _sched);
            addChild(_tree_rabin);
        }

        /** Calculate */
        public override void calculate(int level, int limit)
        {
            //  if (_options.verbose_scheduler) {
            //std::cerr << "Calculate ("<< level <<"): " << typeid(*this).name() << std::endl;
            //  }

            try
            {
                children[0].calculate(level, limit);
            }
            catch (LimitReachedException e)
            {
                //_automaton.reset();
                return;
            }

            _automaton = children[0]._automaton;
            _comment = "Streett{" + children[0]._comment + "}";

            if (_automaton != null)
            {
                _automaton.considerAsStreett();
            }

            hook_after_calculate();
        }

        public void setStutterInformation(StutterSensitivenessInformation stutter_information)
        {
            _stutter_information = stutter_information;
            _tree_rabin.setStutterInformation(stutter_information);
        }


        private LTL2DSTAR_Tree_Rabin _tree_rabin;
        private StutterSensitivenessInformation _stutter_information;
    }


    ///**
    // * Use Scheck to generate a DBA (transformed into a DRA) for a (co-)safe LTL formula
    // */
    //class LTL2DSTAR_Tree_Scheck :  LTL2DSTAR_Tree {
    //public:
    //  /**
    //   * Constructor 
    //   * @param ltl The LTL formula
    //   * @param options the LTL2DSTAR options
    //   * @param sched a reference back to the scheduler 
    //   */

    //   LTL2DSTAR_Tree_Scheck(LTLFormula ltl, LTL2DSTAR_Options options,LTL2DSTAR_Scheduler sched) :
    //     base (ltl, options, sched) {
    //     generateTree();
    //   }

    //  /** Check if the formula is syntactically (co-)safe */
    //  static bool worksWith(LTLFormula ltl) {
    //      bool verbose = false;
    //    if (ltl.isSafe()) {
    //  //if (verbose) {
    //  //  std::cerr << "Formula is safe" << std::endl;
    //  //}
    //  return true;
    //    } else if (ltl.isCoSafe()) {
    //  if (verbose) {
    //    std::cerr << "Formula is cosafe" << std::endl;
    //  }
    //  return true;
    //    }
    //    return false;
    //  }

    //  /** Generate tree */
    //  virtual void generateTree() {
    //  }

    //  /** Calculate */
    //  virtual void calculate(int level, unsigned int limit) {
    //    if (_options.verbose_scheduler) {
    //  std::cerr << "Calculate ("<< level <<"): " << typeid(*this).name() << std::endl;
    //    }

    //     LTLSafetyAutomata lsa;
    //     _automaton=lsa.ltl2dra<DRA_t>(*_ltl, _options.scheck_path);

    //     if (_automaton.get()==0) {return;}
    //     _comment=std::string("Scheck");

    //     if (_options.optimizeAcceptance) {
    //   _automaton->optimizeAcceptanceCondition();
    //     }

    //     /* not really necessary with scheck
    //   if (_options.bisim) {
    //   DRAOptimizations<DRA_t> dra_optimizer;
    //   _automaton=dra_optimizer.optimizeBisimulation(*_automaton,
    //                            false,
    //                            _options.detailed_states,
    //                            false);
    //  } */

    //     hook_after_calculate();
    //   }
    //};


}
