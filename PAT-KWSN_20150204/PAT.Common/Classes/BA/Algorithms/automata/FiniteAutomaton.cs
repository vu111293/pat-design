using System;
using System.Collections.Generic;
using PAT.Common.Classes.BA.Algorithms.automata;

namespace PAT.Common.Classes.Assertion.Algorithms.automata
{
    public class FiniteAutomaton
    {
        public List<FAState> states;
        public List<FAState> F;
        List<String> S;
        public HashSet<String> alphabet;
        public int trans = 0;
        internal FAState InitialState;
        private int num = 0;


        public FiniteAutomaton()
        {
            states = new List<FAState>();
            F = new List<FAState>();
            S = new List<String>(200);
            alphabet = new HashSet<String>();
        }
        //public FiniteAutomaton(System.IO.FileInfo source)
        //{
        //    loadAutomaton(source);
        //}
        //public FiniteAutomaton(System.String filename)
        //{
        //    if (!loadAutomaton(filename))
        //        throw new InvalidAutomatonFormat("The source file \"" + filename + "\" does not define a valid automaton.");
        //}

        public virtual FAState createState()
        {
            FAState st = new FAState(num);
            num++;
            states.Add(st);
            return st;
        }

        public virtual FAState createState(System.String name)
        {
            int i = S.IndexOf(name);
            if (i < 0)
            {
                FAState st = new FAState(num);
                S.Add(name);
                states.Add(st);
                num++;
                return st;
            }
            else
            {
                return states[i];
            }
        }

        public HashSet<string> getAllTransitionSymbols()
        {
            return alphabet;
        }

        public HashSet<String> getAllTransitionSymbolsAL()
        {
            return alphabet;
        }

        public virtual void addTransition(FAState state, FAState state2, System.String label)
        {
            if (state.next.ContainsKey(label))
            {
                if (state.next[label].Contains(state2))
                {
                    return;
                }
            }


            trans++;
            if (!alphabet.Contains(label))
            {
                alphabet.Add(label);
            }
            state.addNext(label, state2, this);
            state2.addPre(label, state);
        }

        //UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        //Override
        public override System.String ToString()
        {
            System.String result = "\n";

            foreach (FAState st in states)
            {
                foreach (KeyValuePair<string, HashSet<FAState>> pair in st.next)
                {
                    System.String label = pair.Key;
                    foreach (FAState to in pair.Value)
                    {
                        result += (st + " --" + label + "-->" + to + "\n");
                    }
                }
            }

            result += ("\nInit:" + InitialState);
            result += ("\nACC:" + F + "\n");
            return result;
        }

        public virtual System.String toMh()
        {
            System.String result = states.Count + " ";
            Dictionary<int, int> stMap = new Dictionary<int, int>();
            Dictionary<string, int> labelMap = new Dictionary<string, int>();
            int currentMaxLabel = 0;
            int swap = 1;

            foreach (FAState st in states)
            {
                if (st.ID == InitialState.id)
                {
                    stMap.Add(st.ID, 1);
                    swap = st.ID;
                }
                else
                {
                    stMap.Add(st.ID, st.ID + 1);
                }
            }
            stMap.Add(0, swap + 1);

            foreach (FAState st in states)
            {
                if (F.Contains(st))
                {
                    result = result + stMap[st.id] + " ";
                }
            }
            result = result + "- ";


            foreach (FAState st in states)
            {

                foreach (KeyValuePair<string, HashSet<FAState>> pair in st.next)
                {
                    System.String label = pair.Key;
                    if (!labelMap.ContainsKey(label))
                    {
                        labelMap.Add(label, currentMaxLabel);
                        currentMaxLabel++;
                    }
                    foreach (FAState to in pair.Value)
                    {
                        result += (stMap[st.ID] + " ");
                        result += (labelMap[label] + " ");
                        result += (stMap[to.ID] + " ");
                    }
                }
            }
            result += " ";
            return result;
        }

        public virtual System.String toString2()
        {
            System.String result = "\n";
            foreach (FAState st in states)
            {
                foreach (KeyValuePair<string, HashSet<FAState>> pair in st.next)
                {
                    System.String label = pair.Key;
                    foreach (FAState to in pair.Value)
                    {

                        result += ("t(" + st.id + "," + to.id + ",\"" + label + "\");");
                    }
                }
                result += "\n";
            }
            result += ("\nInit:" + InitialState + "\n");

            foreach (FAState st in F)
            {
                result += ("f(" + st.id + ");\n");
            }
            return result;
        }
        public virtual System.String getName(FAState s)
        {
            string name = S[s.ID];
            return name == "" ? s.ToString() : name;
        }

        //public virtual bool loadAutomaton(System.String filename)
        //{
        //    try
        //    {
        //        return loadAutomaton(new System.IO.FileInfo(filename));
        //    }
        //    catch (System.IO.IOException e)
        //    {
        //        //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
        //        System.Console.Out.WriteLine(e.Message);
        //        return false;
        //    }
        //}
        //public virtual bool loadAutomaton(System.IO.FileInfo source)
        //{
        //    System.IO.StreamReader input = null;
        //    //UPGRADE_TODO: The differences in the expected value  of parameters for constructor 'java.io.BufferedReader.BufferedReader'  may cause compilation errors.  "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1092'"
        //    //UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
        //    //UPGRADE_TODO: Constructor 'java.io.FileReader.FileReader' was converted to 'System.IO.StreamReader' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
        //    input = new System.IO.StreamReader(new System.IO.StreamReader(source.FullName, System.Text.Encoding.Default).BaseStream, new System.IO.StreamReader(source.FullName, System.Text.Encoding.Default).CurrentEncoding);
        //    bool init = true, trans = true;
        //    init();
        //    while (input.Peek() != -1)
        //    {
        //        System.String s = input.ReadLine();
        //        if (init)
        //        {
        //            if (s.IndexOf(',') < 0)
        //            {
        //                InitialState = createState(s);
        //                init = false;
        //                continue;
        //            }
        //        }
        //        if (trans)
        //        {
        //            System.String[] ss = s.split("[,\\->]");
        //            if (ss.Length == 4)
        //            {
        //                addTransition(createState(ss[1]), createState(ss[3]), ss[0]);
        //                continue;
        //            }
        //            trans = false;
        //        }
        //        if (s.IndexOf(',') < 0)
        //            F.add(createState(s));
        //    } ;
        //    if (init)
        //        InitialState = states.get_Renamed(0);
        //    if (trans)
        //        F.addAll(states);
        //    input.Close();

        //    return (this.init_Renamed_Field != null) && (this.num > 0);
        //}
        //public virtual bool saveAutomaton(System.String filename)
        //{
        //    try
        //    {
        //        return saveAutomaton(new System.IO.FileInfo(filename));
        //    }
        //    catch (System.IO.IOException e)
        //    {
        //        //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
        //        System.Console.Out.WriteLine(e.Message);
        //        return false;
        //    }
        //}
        //public virtual bool saveAutomaton(System.IO.FileInfo dest)
        //{
        //    System.IO.StreamWriter output;
        //    FAState[] states = this.states.toArray(new FAState[0]);
        //    //UPGRADE_WARNING: At least one expression was used more than once in the target code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1181'"
        //    //UPGRADE_TODO: Constructor 'java.io.FileWriter.FileWriter' was converted to 'System.IO.StreamWriter' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileWriterFileWriter_javaioFile'"
        //    //UPGRADE_TODO: Class 'java.io.FileWriter' was converted to 'System.IO.StreamWriter' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioFileWriter'"
        //    output = new System.IO.StreamWriter(new System.IO.StreamWriter(dest.FullName, false, System.Text.Encoding.Default).BaseStream, new System.IO.StreamWriter(dest.FullName, false, System.Text.Encoding.Default).Encoding);
        //    output.Write(getName(InitialState) + "\n");

        //    for (int i = 0; i < states.Length; i++)
        //    {
        //        //UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
        //        System.String from_name = getName(states[i]);

        //        while (label_it.hasNext())
        //        {
        //            System.String label = label_it.next();
        //            System.String prefix = label + "," + from_name + "->";
        //            //UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
        //            //UPGRADE_NOTE: There is an untranslated Statement.  Please refer to original code. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1153'"
        //            while (next_it.hasNext())
        //            {
        //                FAState s = next_it.next();
        //                System.String to_name = getName(s);
        //                output.Write(prefix + to_name + "\n");
        //            }
        //        }
        //    }
        //    for (int i = 0; i < F.size(); i++)
        //        output.Write(getName(F.get_Renamed(i)) + "\n");
        //    output.Flush();
        //    return true;
        //}

        //[STAThread]
        //public static void  Main(System.String[] args)
        //{
        //    System.String file = "mcs.2.1.crtcl2.final.ba";
        //    System.Console.Out.WriteLine(file + " started...");
        //    FiniteAutomaton fa = new FiniteAutomaton();
        //    fa.loadAutomaton(file);
        //    fa.saveAutomaton("mcs.2.1.crtcl2.final.ba_");
        //    System.Console.Out.WriteLine("done!");
        //}
    }
}