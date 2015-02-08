using System.Collections.Generic;
using PAT.Common;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.ModuleInterface;

namespace PAT.KWSN.LTS
{
    public sealed class Definition
    {
        public string Name;
        public ParsingException DefinitionToken;
        public Process Process;
        public string[] LocalVariables;

        //=========================Fileds for Static Analysis================================
        public List<Definition> SubDefinitions;
        public List<string> SubDefinitionNames;

        public List<string> GlobalVars;
        public List<string> Channels;
        public bool MustAbstract;

        public EventCollection AlphabetEvents;
        public HashSet<string> Alphabets;

        //after static analysis, AlphabetsCalculable is true if and only if Alphabets are not null.
        public bool AlphabetsCalculable;
        public bool UsedInParallel;
        //=========================Fileds for Static Analysis================================


        public Definition(string name, string[] vars, Process process)
        {
            Name = name;
            Process = process;
            LocalVariables = vars;

            SubDefinitions = new List<Definition>();
            SubDefinitionNames = new List<string>();

            Channels = new List<string>();
            GlobalVars = new List<string>();

            AlphabetsCalculable = true;
            Alphabets = new HashSet<string>();
        }

        public override string ToString()
        {
            return Process.ToString();
        }

        public string GetFullDefinition()
        {
            return Name + "(" + Common.Classes.Ultility.Ultility.PPStringList(LocalVariables) + ")=\r\n" + Process + ";";
        }

        public Definition ClearConstant(Dictionary<string, Expression> constMapping)
        {
            Process = Process.ClearConstant(constMapping);
            return this;
        }

        /// <summary>
        /// To Perform the static analysis on a single definition first.
        /// </summary>
        public void StaticAnalysis()
        {
            MustAbstract = Process.MustBeAbstracted();

            Channels = Process.GetChannels();

            GlobalVars = Process.GetGlobalVariables();

            if (AlphabetEvents != null)
            {
                if (AlphabetEvents.ContainsVariable())
                {
                    AlphabetsCalculable = false;
                    Alphabets = null;
                }
                else
                {
                    Alphabets = new HashSet<string>(new EventCollection(AlphabetEvents).EventNames);
                    AlphabetsCalculable = true;
                }
            }
            else
            {
                if (AlphabetsCalculable)
                {
                    //to check why is null here? forget the reason le.
                    Alphabets = Process.GetAlphabets(null);
                }
            }
        }
    }
}