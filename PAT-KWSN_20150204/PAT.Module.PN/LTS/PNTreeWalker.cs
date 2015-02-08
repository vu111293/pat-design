// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using PAT.Common;
using PAT.Common.Classes.DataStructure;
using PAT.Common.Classes.Expressions;
using PAT.Common.Classes.LTS;
using PAT.Common.Classes.Expressions.ExpressionClass;
using PAT.Common.Classes.Assertion;
using PAT.Common.Classes.BA;
using PAT.Common.Classes.SemanticModels.LTS.BDD;
using PAT.Common.Classes.Ultility;
using System.Collections.Generic;
using PAT.Common.GUI.PNModule;
using PAT.Common.GUI.Drawing;
using PAT.PN.LTS;

using PAT.PN.Assertions;
using Valuation = PAT.Common.Classes.Expressions.Valuation;
using Transition = PAT.PN.LTS.PNTransition;
using PAT.Common.Classes.ModuleInterface;


using System;
using Antlr.Runtime;
using Antlr.Runtime.Tree;
using RewriteRuleITokenStream = Antlr.Runtime.Tree.RewriteRuleTokenStream;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;
using Map = System.Collections.IDictionary;
using HashMap = System.Collections.Generic.Dictionary<object, object>;
namespace PAT.PN.LTS
{
    [System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3 Nov 30, 2010 12:45:30")]
    public partial class PNTreeWalker : Antlr.Runtime.Tree.TreeParser
    {
        internal static readonly string[] tokenNames = new string[] {
		"<invalid>", "<EOR>", "<DOWN>", "<UP>", "DEFINITION_REF_NODE", "BLOCK_NODE", "CHANNEL_NODE", "VAR_NODE", "CALL_NODE", "LET_NODE", "LET_ARRAY_NODE", "APPLICATION_NODE", "RECORD_NODE", "RECORD_ELEMENT_NODE", "RECORD_ELEMENT_RANGE_NODE", "DEFINITION_NODE", "IF_PROCESS_NODE", "ATOMIC_IF_PROCESS_NODE", "BLOCKING_IF_PROCESS_NODE", "CASE_PROCESS_NODE", "CASE_PROCESS_CONDITION_NODE", "INTERLEAVE_NODE", "PARALLEL_NODE", "INTERNAL_CHOICE_NODE", "EXTERNAL_CHOICE_NODE", "EVENT_NAME_NODE", "EVENT_WL_NODE", "EVENT_SL_NODE", "EVENT_WF_NODE", "EVENT_SF_NODE", "EVENT_PLAIN_NODE", "EVENT_NODE_CHOICE", "CHANNEL_IN_NODE", "CHANNEL_OUT_NODE", "GUARD_NODE", "PARADEF_NODE", "PARADEF1_NODE", "PARADEF2_NODE", "EVENT_NODE", "ATOM_NODE", "ASSIGNMENT_NODE", "DEFINE_NODE", "DEFINE_CONSTANT_NODE", "HIDING_ALPHA_NODE", "SELECTING_NODE", "UNARY_NODE", "ALPHABET_NOTE", "VARIABLE_RANGE_NODE", "LOCAL_VAR_NODE", "LOCAL_ARRAY_NODE", "USING_NODE", "GENERAL_CHOICE_NODE", "CLASS_CALL_NODE", "CLASS_CALL_INSTANCE_NODE", "PROCESS_NODE", "TRANSITION_NODE", "ARC_NODE", "PLACE_NODE", "SELECT_NODE", "DPARAMETER_NODE", "STRING", "ID", "INT", "WS", "COMMENT", "LINE_COMMENT", "'#'", "'import'", "';'", "'include'", "'assert'", "'|='", "'('", "')'", "'[]'", "'<>'", "'!'", "'?'", "'&&'", "'||'", "'tau'", "'->'", "'<->'", "'/\\'", "'\\/'", "'.'", "'deadlockfree'", "'nonterminating'", "'divergencefree'", "'deterministic'", "'reaches'", "'refines'", "'<F>'", "'<FD>'", "'with'", "'min'", "'max'", "','", "'alphabet'", "'{'", "'}'", "'define'", "'-'", "'true'", "'false'", "'enum'", "'var'", "'='", "'['", "']'", "'xor'", "'&'", "'|'", "'^'", "'=='", "'!='", "'<'", "'>'", "'<='", "'>='", "'+'", "'*'", "'/'", "'%'", "'++'", "'--'", "'call'", "'new'", "'hvar'", "':'", "'..'", "'if'", "'else'", "'while'", "'Process'", "'##@@'", "'@@##'", "'-->'", "'select'", "'|||'", "'@'", "'[*]'", "'interrupt'", "'\\'", "'case'", "'default'", "'ifa'", "'ifb'", "'atomic'", "'Skip'", "'Stop'", "'channel'", "'/\\\\'", "'empty'"
	};
        public const int EOF = -1;
        public const int T__66 = 66;
        public const int T__67 = 67;
        public const int T__68 = 68;
        public const int T__69 = 69;
        public const int T__70 = 70;
        public const int T__71 = 71;
        public const int T__72 = 72;
        public const int T__73 = 73;
        public const int T__74 = 74;
        public const int T__75 = 75;
        public const int T__76 = 76;
        public const int T__77 = 77;
        public const int T__78 = 78;
        public const int T__79 = 79;
        public const int T__80 = 80;
        public const int T__81 = 81;
        public const int T__82 = 82;
        public const int T__83 = 83;
        public const int T__84 = 84;
        public const int T__85 = 85;
        public const int T__86 = 86;
        public const int T__87 = 87;
        public const int T__88 = 88;
        public const int T__89 = 89;
        public const int T__90 = 90;
        public const int T__91 = 91;
        public const int T__92 = 92;
        public const int T__93 = 93;
        public const int T__94 = 94;
        public const int T__95 = 95;
        public const int T__96 = 96;
        public const int T__97 = 97;
        public const int T__98 = 98;
        public const int T__99 = 99;
        public const int T__100 = 100;
        public const int T__101 = 101;
        public const int T__102 = 102;
        public const int T__103 = 103;
        public const int T__104 = 104;
        public const int T__105 = 105;
        public const int T__106 = 106;
        public const int T__107 = 107;
        public const int T__108 = 108;
        public const int T__109 = 109;
        public const int T__110 = 110;
        public const int T__111 = 111;
        public const int T__112 = 112;
        public const int T__113 = 113;
        public const int T__114 = 114;
        public const int T__115 = 115;
        public const int T__116 = 116;
        public const int T__117 = 117;
        public const int T__118 = 118;
        public const int T__119 = 119;
        public const int T__120 = 120;
        public const int T__121 = 121;
        public const int T__122 = 122;
        public const int T__123 = 123;
        public const int T__124 = 124;
        public const int T__125 = 125;
        public const int T__126 = 126;
        public const int T__127 = 127;
        public const int T__128 = 128;
        public const int T__129 = 129;
        public const int T__130 = 130;
        public const int T__131 = 131;
        public const int T__132 = 132;
        public const int T__133 = 133;
        public const int T__134 = 134;
        public const int T__135 = 135;
        public const int T__136 = 136;
        public const int T__137 = 137;
        public const int T__138 = 138;
        public const int T__139 = 139;
        public const int T__140 = 140;
        public const int T__141 = 141;
        public const int T__142 = 142;
        public const int T__143 = 143;
        public const int T__144 = 144;
        public const int T__145 = 145;
        public const int T__146 = 146;
        public const int T__147 = 147;
        public const int T__148 = 148;
        public const int T__149 = 149;
        public const int T__150 = 150;
        public const int DEFINITION_REF_NODE = 4;
        public const int BLOCK_NODE = 5;
        public const int CHANNEL_NODE = 6;
        public const int VAR_NODE = 7;
        public const int CALL_NODE = 8;
        public const int LET_NODE = 9;
        public const int LET_ARRAY_NODE = 10;
        public const int APPLICATION_NODE = 11;
        public const int RECORD_NODE = 12;
        public const int RECORD_ELEMENT_NODE = 13;
        public const int RECORD_ELEMENT_RANGE_NODE = 14;
        public const int DEFINITION_NODE = 15;
        public const int IF_PROCESS_NODE = 16;
        public const int ATOMIC_IF_PROCESS_NODE = 17;
        public const int BLOCKING_IF_PROCESS_NODE = 18;
        public const int CASE_PROCESS_NODE = 19;
        public const int CASE_PROCESS_CONDITION_NODE = 20;
        public const int INTERLEAVE_NODE = 21;
        public const int PARALLEL_NODE = 22;
        public const int INTERNAL_CHOICE_NODE = 23;
        public const int EXTERNAL_CHOICE_NODE = 24;
        public const int EVENT_NAME_NODE = 25;
        public const int EVENT_WL_NODE = 26;
        public const int EVENT_SL_NODE = 27;
        public const int EVENT_WF_NODE = 28;
        public const int EVENT_SF_NODE = 29;
        public const int EVENT_PLAIN_NODE = 30;
        public const int EVENT_NODE_CHOICE = 31;
        public const int CHANNEL_IN_NODE = 32;
        public const int CHANNEL_OUT_NODE = 33;
        public const int GUARD_NODE = 34;
        public const int PARADEF_NODE = 35;
        public const int PARADEF1_NODE = 36;
        public const int PARADEF2_NODE = 37;
        public const int EVENT_NODE = 38;
        public const int ATOM_NODE = 39;
        public const int ASSIGNMENT_NODE = 40;
        public const int DEFINE_NODE = 41;
        public const int DEFINE_CONSTANT_NODE = 42;
        public const int HIDING_ALPHA_NODE = 43;
        public const int SELECTING_NODE = 44;
        public const int UNARY_NODE = 45;
        public const int ALPHABET_NOTE = 46;
        public const int VARIABLE_RANGE_NODE = 47;
        public const int LOCAL_VAR_NODE = 48;
        public const int LOCAL_ARRAY_NODE = 49;
        public const int USING_NODE = 50;
        public const int GENERAL_CHOICE_NODE = 51;
        public const int CLASS_CALL_NODE = 52;
        public const int CLASS_CALL_INSTANCE_NODE = 53;
        public const int PROCESS_NODE = 54;
        public const int TRANSITION_NODE = 55;
        public const int ARC_NODE = 56;
        public const int PLACE_NODE = 57;
        public const int SELECT_NODE = 58;
        public const int DPARAMETER_NODE = 59;
        public const int STRING = 60;
        public const int ID = 61;
        public const int INT = 62;
        public const int WS = 63;
        public const int COMMENT = 64;
        public const int LINE_COMMENT = 65;
        public const int T__151 = 151;
        public const int T__152 = 152;
        public const int T__153 = 153;

        // delegates
        // delegators

#if ANTLR_DEBUG
		private static readonly bool[] decisionCanBacktrack =
			new bool[]
			{
				false, // invalid decision
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, false, false, 
				false, false, false, false, false, false, false, false, , , , , , , 
				, , , , , , , , , , 
			};
#else
        private static readonly bool[] decisionCanBacktrack = new bool[0];
#endif
        public PNTreeWalker(ITreeNodeStream input)
            : this(input, new RecognizerSharedState())
        {
        }
        public PNTreeWalker(ITreeNodeStream input, RecognizerSharedState state)
            : base(input, state)
        {
            this.state.ruleMemo = new System.Collections.Generic.Dictionary<int, int>[154 + 1];


            OnCreated();
        }


        public override string[] TokenNames { get { return PNTreeWalker.tokenNames; } }
        public override string GrammarFileName { get { return "E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g"; } }


        public static List<DefinitionRef> dlist;
        public static List<IToken> dtokens;
        private List<IToken> eventsTokens;
        private List<IToken> alphaTokens;
        private List<IToken> declareTokens;
        private List<IToken> ltlTokens;
        public Dictionary<string, Expression> ConstantDatabase;
        private Dictionary<string, List<int>> ArrayID2DimentionMapping;
        private List<string> LocalVariables;
        private Stack<KeyValuePair<string, int>> LocalVariablesStack;
        private int BlockDepth;
        public Specification Spec;
        public List<IToken> GlobalVarNames;
        public List<IToken> GlobalConstNames;
        public List<IToken> GlobalRecordNames;
        public List<IToken> LTLStatePropertyNames;
        public List<IToken> ChannelNames;
        public List<IToken> DefinitionNames;
        private string options;
        private Stack paraphrases = new Stack();
        public bool IsParameterized = false;
        public bool HasArbitraryProcess = false;
        private Definition CurrentDefinition;
        private bool CurrentLTSGraphAlphabetsCalculable;

        private Dictionary<string, EventCollection> AlphaDatabase;
        private List<string> DefUsedInParallel;
        //public List<IToken> UsingLibraries = new List<IToken>();

        public void program(string opt)
        {
            options = opt;
            dlist = new List<DefinitionRef>();
            dtokens = new List<IToken>();
            eventsTokens = new List<IToken>();
            alphaTokens = new List<IToken>();
            declareTokens = new List<IToken>();
            ltlTokens = new List<IToken>();
            ConstantDatabase = new Dictionary<string, Expression>(8);
            ArrayID2DimentionMapping = new Dictionary<string, List<int>>();
            LocalVariables = new List<string>();
            LocalVariablesStack = new Stack<KeyValuePair<string, int>>();
            BlockDepth = 0;
            AlphaDatabase = new Dictionary<string, EventCollection>(8);
            DefUsedInParallel = new List<string>();

            specification();

            //check the number of processes > 0
            if (Spec.DefinitionDatabase.Count == 0)
            {
                throw new ParsingException("Please enter at least one process definition.", 0, 0, "");
            }

            //each definition reference needs to be defined first, also the number of arguments must match
            foreach (DefinitionRef def in dlist)
            {
                if (Spec.DefinitionDatabase.ContainsKey(def.Name))
                {
                    Definition d = Spec.DefinitionDatabase[def.Name];
                    IToken token = dtokens[dlist.IndexOf(def)];

                    if (def.Args.Length != d.LocalVariables.Length)
                    {
                        throw new ParsingException("Definition reference " + token.Text + " has different number of arguments from the definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                    }
                    def.Def = d;

                    // PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, def.Name, token);                    

                }
                else if (!Spec.PNDefinitionDatabase.ContainsKey(def.Name))
                {
                    IToken token = dtokens[dlist.IndexOf(def)];
                    throw new ParsingException("Undefined process definition: " + token.Text, token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                }
                else
                {
                    PetriNet d = Spec.PNDefinitionDatabase[def.Name];
                    if (def.Args.Length != d.Parameters.Count)
                    {
                        IToken token = dtokens[dlist.IndexOf(def)];
                        throw new ParsingException("Definition reference " + token.Text + " has different number of arguments from the definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                    }
                    //
                    // def.GraphDef = d;
                }
            }

            foreach (string def in DefUsedInParallel)
            {
                if (Spec.DefinitionDatabase.ContainsKey(def))
                {
                    Spec.DefinitionDatabase[def].UsedInParallel = true;
                }
            }

            foreach (IToken token in alphaTokens)
            {
                if (!Spec.DefinitionDatabase.ContainsKey(token.Text) && !Spec.PNDefinitionDatabase.ContainsKey(token.Text))
                {
                    throw new ParsingException("Undefined process definition: " + token.Text, token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                }

                PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, token.Text, token);

                if (Spec.DefinitionDatabase.ContainsKey(token.Text))
                {
                    Definition def = Spec.DefinitionDatabase[token.Text];
                    List<string> localVars = new List<string>(def.LocalVariables);
                    EventCollection eventCollection = AlphaDatabase[token.Text];
                    foreach (Event evt in eventCollection)
                    {
                        if (evt.ExpressionList != null)
                        {
                            foreach (Expression exp in evt.ExpressionList)
                            {
                                List<string> evars = exp.GetVars();
                                foreach (string var in evars)
                                {
                                    if (!localVars.Contains(var))
                                    {
                                        throw new ParsingException("Alphabet definition for process " + token.Text + " has an event " + evt.BaseName + " with invalid variable " + var + ", which is not declared as a parameter of its process definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                                    }
                                }
                            }
                        }
                    }

                    def.AlphabetEvents = eventCollection;
                }
                else
                {
                    PetriNet def = Spec.PNDefinitionDatabase[token.Text];
                    List<string> localVars = new List<string>(def.Parameters);
                    EventCollection eventCollection = AlphaDatabase[token.Text];
                    foreach (Event evt in eventCollection)
                    {
                        if (evt.ExpressionList != null)
                        {
                            foreach (Expression exp in evt.ExpressionList)
                            {
                                List<string> evars = exp.GetVars();
                                foreach (string var in evars)
                                {
                                    if (!localVars.Contains(var))
                                    {
                                        throw new ParsingException("Alphabet definition for process " + token.Text + " has an event " + evt.BaseName + " with invalid variable " + var + ", which is not declared as a parameter of its process definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                                    }
                                }
                            }
                        }
                    }

                    def.AlphabetEvents = eventCollection;
                }
            }

            foreach (IToken token in ltlTokens)
            {
                if (!Spec.DeclarationDatabase.ContainsKey(token.Text))
                {
                    bool matchEvent = false;
                    if (token.Type != PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN)
                    {
                        foreach (IToken var in eventsTokens)
                        {
                            if (var.Text == token.Text)
                            {
                                matchEvent = true;
                                break;
                            }
                        }
                    }

                    if (token.Type == PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN)
                    {
                        foreach (IToken name in ChannelNames)
                        {
                            if (name.Text == token.Text)
                            {
                                matchEvent = true;
                                break;
                            }
                        }
                    }

                    if (!matchEvent)
                    {
                        if (token.Text != "init")
                        {
                            throw new ParsingException("LTL proposition: " + token.Text + " is NOT declared as a valid event, channel name or declaration.", token);
                        }
                    }
                }
                else
                {
                    if (token.Type == PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN || token.Type == PAT.Common.Ultility.ParsingUltility.LTL_COMPOUND_EVENT)
                    {
                        throw new ParsingException("LTL proposition: declaration " + token.Text + " is mis-used as an event or channel.", token);
                    }
                }
            }

            foreach (IToken token in declareTokens)
            {
                if (!Spec.DeclarationDatabase.ContainsKey(token.Text))
                {
                    throw new ParsingException("Reachable assertion: " + token.Text + " is not a valid declaration.", token);
                }
                else
                {
                    PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, token.Text, token);
                }
            }

            eventsTokens.Clear();
            dtokens.Clear();
            alphaTokens.Clear();
            declareTokens.Clear();
            ltlTokens.Clear();


        }

        public override String GetErrorMessage(RecognitionException e, String[] tokenNames)
        {
            string msg = null;
            if (e is NoViableAltException)
            {
                //NoViableAltException nvae = (NoViableAltException)e;
                //msg = "Invalid Symbol: ="+e.Token+ "At line:" + token.Line + " col:" + token.CharPositionInLine;  // (decision="+nvae.decisionNumber+" state "+nvae.stateNumber+")"+" decision=<<"+nvae.grammarDecisionDescription+">>";
                //msg = "Invalid Symbol: " + e.Token.Text + " at line:" + e.Token.Line + " col:" + e.Token.CharPositionInLine;  // (decision="+nvae.decisionNumber+" state "+nvae.stateNumber+")"+" decision=<<"+nvae.grammarDecisionDescription+">>";
                msg = "Invalid Symbol: '" + e.Token.Text + "'";
            }
            else
            {
                msg = base.GetErrorMessage(e, tokenNames);
            }

            if (paraphrases.Count > 0)
            {
                String paraphrase = (String)paraphrases.Peek();
                msg = msg + " " + paraphrase;
            }
            return msg;
        }

        private void CheckIDNameDefined(List<string> vars, IToken idToken)
        {
            if (vars.Contains(idToken.Text))
            {
                throw new ParsingException("'" + idToken.Text + "' is already used as a parameter name in the containing definition. Please choose another name!", idToken);
            }

            foreach (KeyValuePair<string, int> item in LocalVariablesStack)
            {
                if (item.Key == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' is already used as a local variable in the data operation program. Please choose another name!", idToken);
                }
            }

            foreach (IToken name in DefinitionNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a process definition at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalVarNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a global variable at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalConstNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a constant at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalRecordNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a global variable at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in ChannelNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a channel name at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in LTLStatePropertyNames)
            {
                if (name.Text == idToken.Text)
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as an LTL state condition at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }
        }

        private void CheckChannelDeclared(IToken idToken)
        {
            bool declared = false;
            foreach (IToken name in ChannelNames)
            {
                if (name.Text == idToken.Text)
                {
                    declared = true;
                    break;
                }
            }
            if (!declared)
            {
                throw new ParsingException("Channel name '" + idToken.Text + "' is used without declaration!", idToken);
            }
            else
            {
                PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, idToken.Text, idToken);
            }
        }

        private void CheckDuplicatedDeclaration(IToken idToken, List<string> vars)
        {
            if (vars.Contains(idToken.Text))
            {
                throw new ParsingException("'" + idToken.Text + "' has been already defined in parameters. Please choose another name!", idToken);
            }
            CheckDuplicatedDeclaration(idToken);
        }

        private void CheckDuplicatedDeclaration(IToken idToken)
        {


            foreach (IToken name in DefinitionNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a process definition at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalVarNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a global variable at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalConstNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a constant at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in GlobalRecordNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a global variable at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in ChannelNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as a channel name at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }

            foreach (IToken name in LTLStatePropertyNames)
            {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text + "' has been already defined as an LTL state condition at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
            }
        }

        private Expression CheckVariableNotDeclared(List<string> vars, IToken idToken)
        {
            string word = idToken.Text;
            if (Spec.DeclarationDatabase.ContainsKey(word))
            {
                PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, word, idToken);
                return Spec.DeclarationDatabase[word];
            }

            bool declared = false;
            foreach (IToken name in GlobalVarNames)
            {
                if (name.Text == word)
                {
                    declared = true;
                    break;
                }
            }

            if (!declared)
            {
                foreach (IToken name in GlobalRecordNames)
                {
                    if (name.Text == word)
                    {
                        declared = true;
                        break;
                    }
                }
            }

            if (!declared)
            {
                foreach (IToken name in GlobalConstNames)
                {
                    if (name.Text == word)
                    {
                        declared = true;
                        break;
                    }
                }
            }

            if (!declared)
            {
                foreach (string name in vars)
                {
                    if (name == word)
                    {
                        declared = true;
                        break;
                    }
                }
            }

            if (!declared)
            {
                //foreach (string name in LocalVariables)
                //{
                //       if (name == word)
                //    {
                //        declared = true;
                //        break;
                //    }
                //}
                foreach (KeyValuePair<string, int> item in LocalVariablesStack)
                {
                    if (item.Key == word)
                    {
                        declared = true;
                        break;
                    }
                }
            }

            if (!declared)
            {
                throw new ParsingException("Variable is used without declaration!", idToken);
            }
            else
            {
                PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, word, idToken);
            }
            //if there is matching in declaration table
            //return the null;
            return null;
        }

        private void CheckRecordNotDeclared(List<string> vars, IToken idToken)
        {
            bool declared = false;
            foreach (IToken name in GlobalRecordNames)
            {
                if (name.Text == idToken.Text)
                {
                    declared = true;
                    break;
                }
            }
            if (!declared)
            {
                foreach (string name in vars)
                {
                    if (name == idToken.Text)
                    {
                        declared = true;
                        break;
                    }
                }
            }
            if (!declared)
            {
                foreach (KeyValuePair<string, int> item in LocalVariablesStack)
                {
                    if (item.Key == idToken.Text)
                    {
                        declared = true;
                        break;
                    }
                }
            }
            if (!declared)
            {
                throw new ParsingException("Array is used without declaration!", idToken);
            }
            else
            {
                PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, idToken.Text, idToken);
            }
        }

        //private void CheckForSelfComposition(CommonTree name, PetriNet p)
        //{
        //    if(p is IndexParallel)
        //    {
        //        IndexParallel parallel = p as IndexParallel;
        //        if (parallel.Processes != null)
        //        {
        //			foreach (PetriNet c in parallel.Processes)
        //			{
        //				if(c is DefinitionRef && (c as DefinitionRef).Name == name.Text)
        //				{
        //					throw new ParsingException("Self parallel composition will generate infinite behavior, hence it is not allowed.", name.Token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine 
        //				}
        //				else
        //				{
        //					CheckForSelfComposition(name, c);
        //				}
        //			}
        //        }
        //    }
        //    else if (p is IndexInterleave)
        //    {
        //        IndexInterleave parallel = p as IndexInterleave;
        //        if (parallel.Processes != null)
        //        {
        //            foreach (PetriNet c in parallel.Processes)
        //            {
        //                if (c is DefinitionRef && (c as DefinitionRef).Name == name.Text)
        //                {
        //                    throw new ParsingException("Self interleave composition will generate infinite behavior, hence it is not allowed!", name.Token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine 
        //                }
        //                else
        //                {
        //                    CheckForSelfComposition(name, c);
        //                }
        //            }
        //        }
        //    }
        //    else if (p is IndexChoice)
        //    {
        //        IndexChoice parallel = p as IndexChoice;
        //        if (parallel.Processes != null)
        //        {
        //			foreach (PetriNet c in parallel.Processes)
        //			{
        //				if (c is DefinitionRef && (c as DefinitionRef).Name == name.Text)
        //				{
        //					throw new ParsingException("Self choice composition will generate infinite behavior, hence it is not allowed!", name.Token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine 
        //				}
        //				else
        //				{
        //					CheckForSelfComposition(name, c);
        //				}
        //			}
        //        }
        //    } else if (p is IndexExternalChoice)
        //    {
        //        IndexExternalChoice parallel = p as IndexExternalChoice;
        //        if (parallel.Processes != null)
        //        {
        //			foreach (PetriNet c in parallel.Processes)
        //			{
        //				if (c is DefinitionRef && (c as DefinitionRef).Name == name.Text)
        //				{
        //					throw new ParsingException("Self external choice composition will generate infinite behavior, hence it is not allowed!", name.Token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine 
        //				}
        //				else
        //				{
        //					CheckForSelfComposition(name, c);
        //				}
        //			}
        //        }
        //    }
        //    else if (p is IndexInternalChoice)
        //    {
        //        IndexInternalChoice parallel = p as IndexInternalChoice;
        //        if (parallel.Processes != null)
        //        {
        //			foreach (PetriNet c in parallel.Processes)
        //			{
        //				if (c is DefinitionRef && (c as DefinitionRef).Name == name.Text)
        //				{
        //					throw new ParsingException("Self internal choice composition will generate infinite behavior, hence it is not allowed!", name.Token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine 
        //				}
        //				else
        //				{
        //					CheckForSelfComposition(name, c);
        //				}
        //			}
        //        }
        //    }   
        //}



        protected virtual void OnCreated() { }
        protected virtual void EnterRule(string ruleName, int ruleIndex) { }
        protected virtual void LeaveRule(string ruleName, int ruleIndex) { }


        protected virtual void Enter_specification() { }
        protected virtual void Leave_specification() { }

        // $ANTLR start "specification"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:645:1: specification : ( specBody )* ;
        [GrammarRule("specification")]
        private void specification()
        {

            int specification_StartIndex = input.Index;
            try
            {
                DebugEnterRule(GrammarFileName, "specification");
                DebugLocation(645, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 1)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:646:2: ( ( specBody )* )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:646:4: ( specBody )*
                    {
                        DebugLocation(646, 4);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:646:4: ( specBody )*
                        try
                        {
                            DebugEnterSubRule(1);
                            while (true)
                            {
                                int alt1 = 2;
                                try
                                {
                                    DebugEnterDecision(1, decisionCanBacktrack[1]);
                                    int LA1_0 = input.LA(1);

                                    if (((LA1_0 >= LET_NODE && LA1_0 <= LET_ARRAY_NODE) || LA1_0 == DEFINITION_NODE || (LA1_0 >= DEFINE_NODE && LA1_0 <= DEFINE_CONSTANT_NODE) || LA1_0 == ALPHABET_NOTE || LA1_0 == PROCESS_NODE || LA1_0 == 70 || LA1_0 == 151))
                                    {
                                        alt1 = 1;
                                    }


                                }
                                finally { DebugExitDecision(1); }
                                switch (alt1)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:646:5: specBody
                                        {
                                            DebugLocation(646, 5);
                                            PushFollow(Follow._specBody_in_specification92);
                                            specBody();
                                            PopFollow();
                                            if (state.failed) return;

                                        }
                                        break;

                                    default:
                                        goto loop1;
                                }
                            }

                        loop1:
                            ;

                        }
                        finally { DebugExitSubRule(1); }


                    }

                }
                catch (RecognitionException re)
                {

                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);

                }
                catch (ParsingException ex)
                {

                    throw ex;

                }
                catch (Exception ex)
                {

                    throw new ParsingException("Parsing error: " + ex.Message + (ex.InnerException != null ? ("\r\nMore details:" + ex.InnerException.Message) : ""), input.TokenStream.Get(0));

                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 1, specification_StartIndex); }
                }
                DebugLocation(647, 1);
            }
            finally { DebugExitRule(GrammarFileName, "specification"); }
            return;

        }
        // $ANTLR end "specification"


        protected virtual void Enter_specBody() { }
        protected virtual void Leave_specBody() { }

        // $ANTLR start "specBody"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:660:1: specBody : ( letDefintion | definition | assertion | alphabet | define | channel );
        [GrammarRule("specBody")]
        private void specBody()
        {

            int specBody_StartIndex = input.Index;
            try
            {
                DebugEnterRule(GrammarFileName, "specBody");
                DebugLocation(660, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 2)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:661:2: ( letDefintion | definition | assertion | alphabet | define | channel )
                    int alt2 = 6;
                    try
                    {
                        DebugEnterDecision(2, decisionCanBacktrack[2]);
                        switch (input.LA(1))
                        {
                            case LET_NODE:
                            case LET_ARRAY_NODE:
                                {
                                    alt2 = 1;
                                }
                                break;
                            case DEFINITION_NODE:
                            case PROCESS_NODE:
                                {
                                    alt2 = 2;
                                }
                                break;
                            case 70:
                                {
                                    alt2 = 3;
                                }
                                break;
                            case ALPHABET_NOTE:
                                {
                                    alt2 = 4;
                                }
                                break;
                            case DEFINE_NODE:
                            case DEFINE_CONSTANT_NODE:
                                {
                                    alt2 = 5;
                                }
                                break;
                            case 151:
                                {
                                    alt2 = 6;
                                }
                                break;
                            default:
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    NoViableAltException nvae = new NoViableAltException("", 2, 0, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                        }

                    }
                    finally { DebugExitDecision(2); }
                    switch (alt2)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:661:4: letDefintion
                            {
                                DebugLocation(661, 4);
                                PushFollow(Follow._letDefintion_in_specBody127);
                                letDefintion();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:662:4: definition
                            {
                                DebugLocation(662, 4);
                                PushFollow(Follow._definition_in_specBody132);
                                definition();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:663:4: assertion
                            {
                                DebugLocation(663, 4);
                                PushFollow(Follow._assertion_in_specBody138);
                                assertion();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;
                        case 4:
                            DebugEnterAlt(4);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:664:4: alphabet
                            {
                                DebugLocation(664, 4);
                                PushFollow(Follow._alphabet_in_specBody144);
                                alphabet();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;
                        case 5:
                            DebugEnterAlt(5);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:665:4: define
                            {
                                DebugLocation(665, 4);
                                PushFollow(Follow._define_in_specBody149);
                                define();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;
                        case 6:
                            DebugEnterAlt(6);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:666:4: channel
                            {
                                DebugLocation(666, 4);
                                PushFollow(Follow._channel_in_specBody154);
                                channel();
                                PopFollow();
                                if (state.failed) return;

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 2, specBody_StartIndex); }
                }
                DebugLocation(667, 1);
            }
            finally { DebugExitRule(GrammarFileName, "specBody"); }
            return;

        }
        // $ANTLR end "specBody"


        protected virtual void Enter_alphabet() { }
        protected virtual void Leave_alphabet() { }

        // $ANTLR start "alphabet"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:669:1: alphabet : ^( ALPHABET_NOTE ID (e= eventName[new List<string>(), false, null] )+ ) ;
        [GrammarRule("alphabet")]
        private void alphabet()
        {

            int alphabet_StartIndex = input.Index;
            CommonTree ID1 = null;
            Event e = default(Event);


            paraphrases.Push("in alphabet declaration");
            EventCollection evts = new EventCollection();

            try
            {
                DebugEnterRule(GrammarFileName, "alphabet");
                DebugLocation(669, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 3)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:675:2: ( ^( ALPHABET_NOTE ID (e= eventName[new List<string>(), false, null] )+ ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:675:4: ^( ALPHABET_NOTE ID (e= eventName[new List<string>(), false, null] )+ )
                    {
                        DebugLocation(675, 4);
                        DebugLocation(675, 6);
                        Match(input, ALPHABET_NOTE, Follow._ALPHABET_NOTE_in_alphabet178); if (state.failed) return;

                        Match(input, TokenTypes.Down, null); if (state.failed) return;
                        DebugLocation(675, 20);
                        ID1 = (CommonTree)Match(input, ID, Follow._ID_in_alphabet180); if (state.failed) return;
                        DebugLocation(675, 23);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:675:23: (e= eventName[new List<string>(), false, null] )+
                        int cnt3 = 0;
                        try
                        {
                            DebugEnterSubRule(3);
                            while (true)
                            {
                                int alt3 = 2;
                                try
                                {
                                    DebugEnterDecision(3, decisionCanBacktrack[3]);
                                    int LA3_0 = input.LA(1);

                                    if ((LA3_0 == EVENT_NAME_NODE))
                                    {
                                        alt3 = 1;
                                    }


                                }
                                finally { DebugExitDecision(3); }
                                switch (alt3)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:675:24: e= eventName[new List<string>(), false, null]
                                        {
                                            DebugLocation(675, 25);
                                            PushFollow(Follow._eventName_in_alphabet185);
                                            e = eventName(new List<string>(), false, null);
                                            PopFollow();
                                            if (state.failed) return;
                                            DebugLocation(675, 69);
                                            if ((state.backtracking == 0))
                                            {
                                                evts.Add(e);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt3 >= 1)
                                            goto loop3;

                                        if (state.backtracking > 0) { state.failed = true; return; }
                                        EarlyExitException eee3 = new EarlyExitException(3, input);
                                        DebugRecognitionException(eee3);
                                        throw eee3;
                                }
                                cnt3++;
                            }
                        loop3:
                            ;

                        }
                        finally { DebugExitSubRule(3); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return;
                        DebugLocation(676, 2);
                        if ((state.backtracking == 0))
                        {

                            AlphaDatabase.Add(ID1.Text, evts);
                            alphaTokens.Add(ID1.Token);

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 3, alphabet_StartIndex); }
                }
                DebugLocation(680, 1);
            }
            finally { DebugExitRule(GrammarFileName, "alphabet"); }
            return;

        }
        // $ANTLR end "alphabet"


        protected virtual void Enter_channel() { }
        protected virtual void Leave_channel() { }

        // $ANTLR start "channel"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:682:1: channel : ^( 'channel' ID e= expression[new List<string>(), true, null] ) ;
        [GrammarRule("channel")]
        private void channel()
        {

            int channel_StartIndex = input.Index;
            CommonTree ID2 = null;
            Expression e = default(Expression);

            paraphrases.Push("in channel declaration");
            try
            {
                DebugEnterRule(GrammarFileName, "channel");
                DebugLocation(682, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 4)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:685:2: ( ^( 'channel' ID e= expression[new List<string>(), true, null] ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:685:5: ^( 'channel' ID e= expression[new List<string>(), true, null] )
                    {
                        DebugLocation(685, 5);
                        DebugLocation(685, 7);
                        Match(input, 151, Follow._151_in_channel219); if (state.failed) return;

                        Match(input, TokenTypes.Down, null); if (state.failed) return;
                        DebugLocation(685, 17);
                        ID2 = (CommonTree)Match(input, ID, Follow._ID_in_channel221); if (state.failed) return;
                        DebugLocation(685, 21);
                        PushFollow(Follow._expression_in_channel225);
                        e = expression(new List<string>(), true, null);
                        PopFollow();
                        if (state.failed) return;

                        Match(input, TokenTypes.Up, null); if (state.failed) return;
                        DebugLocation(686, 2);
                        if ((state.backtracking == 0))
                        {

                            CheckDuplicatedDeclaration(ID2.Token);
                            CommonTree expressionTreeRoot = ((Antlr.Runtime.Tree.CommonTree)(((Antlr.Runtime.Tree.BaseTree)(input.TreeSource)).GetChild(1)));
                            int size = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e, expressionTreeRoot.Token, ConstantDatabase);
                            if (size < 0)
                            {
                                throw new ParsingException("channel " + ID2.Text + "'s size must greater than or equal to 0!", ID2.Token);
                            }
                            ChannelQueue queue = new ChannelQueue(size);
                            //Spec.ChannelDatabase.Add(ID2.Text, queue);
                            //Spec.ChannelMaximumSize.Add(ID2.Text, 0);
                            Spec.DeclaritionTable.Add(ID2.Text, new Declaration(DeclarationType.Channel, new ParsingException(ID2.Text, ID2.Token)));

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 4, channel_StartIndex); }
                }
                DebugLocation(699, 1);
            }
            finally { DebugExitRule(GrammarFileName, "channel"); }
            return;

        }
        // $ANTLR end "channel"


        protected virtual void Enter_assertion() { }
        protected virtual void Leave_assertion() { }

        // $ANTLR start "assertion"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:701:1: assertion : ASS= 'assert' proc= definitionRef ( ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ ) | ( 'deadlockfree' ) | ( 'nonterminating' ) | ( 'divergencefree' ) | ( 'deterministic' ) | ( 'reaches' label= ID (exp= withClause[out contraint] )? ) | ( 'refines' targetProcess= definitionRef ) | ( 'refines' '<F>' targetProcess= definitionRef ) | ( 'refines' '<FD>' targetProcess= definitionRef ) ) ;
        [GrammarRule("assertion")]
        private void assertion()
        {

            int assertion_StartIndex = input.Index;
            CommonTree ASS = null;
            CommonTree tk = null;
            CommonTree label = null;
            DefinitionRef proc = default(DefinitionRef);
            Expression exp = default(Expression);
            DefinitionRef targetProcess = default(DefinitionRef);

            paraphrases.Push("in assertion declaration"); string ltl = ""; AssertionBase ass = null; bool hasXoperator = false; string lastToken = ""; List<string> alphabets = new List<string>();
            QueryConstraintType contraint = QueryConstraintType.NONE;

            try
            {
                DebugEnterRule(GrammarFileName, "assertion");
                DebugLocation(701, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 5)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:706:2: (ASS= 'assert' proc= definitionRef ( ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ ) | ( 'deadlockfree' ) | ( 'nonterminating' ) | ( 'divergencefree' ) | ( 'deterministic' ) | ( 'reaches' label= ID (exp= withClause[out contraint] )? ) | ( 'refines' targetProcess= definitionRef ) | ( 'refines' '<F>' targetProcess= definitionRef ) | ( 'refines' '<FD>' targetProcess= definitionRef ) ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:706:4: ASS= 'assert' proc= definitionRef ( ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ ) | ( 'deadlockfree' ) | ( 'nonterminating' ) | ( 'divergencefree' ) | ( 'deterministic' ) | ( 'reaches' label= ID (exp= withClause[out contraint] )? ) | ( 'refines' targetProcess= definitionRef ) | ( 'refines' '<F>' targetProcess= definitionRef ) | ( 'refines' '<FD>' targetProcess= definitionRef ) )
                    {
                        DebugLocation(706, 7);
                        ASS = (CommonTree)Match(input, 70, Follow._70_in_assertion256); if (state.failed) return;
                        DebugLocation(706, 22);
                        PushFollow(Follow._definitionRef_in_assertion262);
                        proc = definitionRef();
                        PopFollow();
                        if (state.failed) return;
                        DebugLocation(706, 38);
                        if ((state.backtracking == 0))
                        {
                            if (proc.Args.Length > 0) { throw new ParsingException("Process with parameters is not allowed in assersion!\r\nPlease define a parameterless process, e.g. system=" + proc.ToString() + "; #assert system ...", ASS.Token); }
                        }
                        DebugLocation(707, 2);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:707:2: ( ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ ) | ( 'deadlockfree' ) | ( 'nonterminating' ) | ( 'divergencefree' ) | ( 'deterministic' ) | ( 'reaches' label= ID (exp= withClause[out contraint] )? ) | ( 'refines' targetProcess= definitionRef ) | ( 'refines' '<F>' targetProcess= definitionRef ) | ( 'refines' '<FD>' targetProcess= definitionRef ) )
                        int alt6 = 9;
                        try
                        {
                            DebugEnterSubRule(6);
                            try
                            {
                                DebugEnterDecision(6, decisionCanBacktrack[6]);
                                try
                                {
                                    alt6 = dfa6.Predict(input);
                                }
                                catch (NoViableAltException nvae)
                                {
                                    DebugRecognitionException(nvae);
                                    throw;
                                }
                            }
                            finally { DebugExitDecision(6); }
                            switch (alt6)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:708:5: ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ )
                                    {
                                        DebugLocation(708, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:708:5: ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:708:7: '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+
                                        {
                                            DebugLocation(708, 7);
                                            Match(input, 71, Follow._71_in_assertion277); if (state.failed) return;
                                            DebugLocation(708, 12);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:708:12: (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+
                                            int cnt4 = 0;
                                            try
                                            {
                                                DebugEnterSubRule(4);
                                                while (true)
                                                {
                                                    int alt4 = 2;
                                                    try
                                                    {
                                                        DebugEnterDecision(4, decisionCanBacktrack[4]);
                                                        int LA4_0 = input.LA(1);

                                                        if (((LA4_0 >= STRING && LA4_0 <= INT) || (LA4_0 >= 72 && LA4_0 <= 82) || (LA4_0 >= 84 && LA4_0 <= 85) || LA4_0 == 152))
                                                        {
                                                            alt4 = 1;
                                                        }


                                                    }
                                                    finally { DebugExitDecision(4); }
                                                    switch (alt4)
                                                    {
                                                        case 1:
                                                            DebugEnterAlt(1);
                                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:708:14: tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT )
                                                            {
                                                                DebugLocation(708, 17);
                                                                tk = (CommonTree)input.LT(1);
                                                                if ((input.LA(1) >= STRING && input.LA(1) <= INT) || (input.LA(1) >= 72 && input.LA(1) <= 82) || (input.LA(1) >= 84 && input.LA(1) <= 85) || input.LA(1) == 152)
                                                                {
                                                                    input.Consume();
                                                                    state.errorRecovery = false; state.failed = false;
                                                                }
                                                                else
                                                                {
                                                                    if (state.backtracking > 0) { state.failed = true; return; }
                                                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                                                    DebugRecognitionException(mse);
                                                                    throw mse;
                                                                }

                                                                DebugLocation(709, 13);
                                                                if ((state.backtracking == 0))
                                                                {


                                                                    if (tk.Text == "tau")
                                                                    {
                                                                        throw new ParsingException("tau operator cannot be used in LTL. The result may not be correct because of the tau elimination optimization.", tk.Token);
                                                                        //Spec.AddNewWarning("When using tau event in LTL, the results may not be correct because the tau elimination optimization.", tk.Token);
                                                                        //tk.Token.Text = Common.Classes.Ultility.Constants.TAU;
                                                                    }


                                                                    if (tk.Type != INT && !ConstantDatabase.ContainsKey(tk.Text))
                                                                    {
                                                                        if (ltl.EndsWith(".") || ltl.EndsWith("?")) //|| ltl.EndsWith("!")
                                                                        {
                                                                            //ltl += tk.Text; //Ultility.Ultility.EVENT_PREFIX +                            
                                                                            throw new ParsingException("Only values can be used here!", tk.Token);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (ltl.EndsWith("?") || ltl.EndsWith("!"))
                                                                        {
                                                                            //if (Spec.ChannelDatabase.ContainsKey(lastToken)) 
                                                                            //{
                                                                            //    ChannelQueue queue = Spec.ChannelDatabase[lastToken];
                                                                            //    if (queue.Size == 0)
                                                                            //    {
                                                                            //        ltl = ltl.Remove(ltl.Length - 1) + ".";
                                                                            //    }
                                                                            //    lastToken = "";
                                                                            //}

                                                                            //the event is channnel 
                                                                            ltlTokens[ltlTokens.Count - 1].Type = PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN;
                                                                        }
                                                                        else if (ltl.EndsWith("."))
                                                                        {
                                                                            //if (Spec.ChannelDatabase.ContainsKey(lastToken)) 
                                                                            //{
                                                                            //    ChannelQueue queue = Spec.ChannelDatabase[lastToken];
                                                                            //    if (queue.Size == 0)
                                                                            //    {
                                                                            //    	  //this is a sync channel event
                                                                            //        ltlTokens[ltlTokens.Count-1].Type = PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN;
                                                                            //    }
                                                                            //    else
                                                                            //    {
                                                                            //        throw new ParsingException("Asynchronous channel can not used with '.' as synchronous channel event!", tk.Token);		
                                                                            //   }
                                                                            //}
                                                                            //else if(ltlTokens.Count > 0)
                                                                            //{	//the event is compond event
                                                                            //	ltlTokens[ltlTokens.Count-1].Type = PAT.Common.Ultility.ParsingUltility.LTL_COMPOUND_EVENT;
                                                                            // }
                                                                        }
                                                                    }

                                                                    if (tk.Type == ID && !ConstantDatabase.ContainsKey(tk.Text))
                                                                    {
                                                                        string word = tk.Token.Text;
                                                                        if (word == "U" || word == "V" || word == "X" || word == "G" || word == "F" || word == "R" || word == "true" || word == "false")
                                                                        {
                                                                            if (ltl.EndsWith(" "))
                                                                            {
                                                                                ltl += word + " ";
                                                                            }
                                                                            else
                                                                            {
                                                                                ltl += " " + word + " ";
                                                                            }

                                                                            if (word == "X")
                                                                            {
                                                                                hasXoperator = true;
                                                                            }
                                                                        }
                                                                        else
                                                                        {

                                                                            ltlTokens.Add(tk.Token);

                                                                            if (word.Trim() != "")
                                                                            {
                                                                                // //+ Ultility.Ultility.EVENT_PREFIX    
                                                                                if (ltl.EndsWith(" "))
                                                                                {
                                                                                    ltl += tk.Text;
                                                                                }
                                                                                else
                                                                                {
                                                                                    ltl += " " + tk.Text;
                                                                                }

                                                                                lastToken = tk.Text;
                                                                                alphabets.Add(lastToken);
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        //lastToken = "";
                                                                        if (ConstantDatabase.ContainsKey(tk.Text))
                                                                        {
                                                                            ltl += ConstantDatabase[tk.Text].ExpressionID; //.ToString();
                                                                        }
                                                                        else
                                                                        {
                                                                            ltl += tk.Text;
                                                                        }
                                                                    }


                                                                }

                                                            }
                                                            break;

                                                        default:
                                                            if (cnt4 >= 1)
                                                                goto loop4;

                                                            if (state.backtracking > 0) { state.failed = true; return; }
                                                            EarlyExitException eee4 = new EarlyExitException(4, input);
                                                            DebugRecognitionException(eee4);
                                                            throw eee4;
                                                    }
                                                    cnt4++;
                                                }
                                            loop4:
                                                ;

                                            }
                                            finally { DebugExitSubRule(4); }

                                            DebugLocation(822, 6);
                                            if ((state.backtracking == 0))
                                            {

                                                if (ltl.EndsWith(".") || ltl.EndsWith("?") || ltl.EndsWith("!"))
                                                {
                                                    //ltl += tk.Text; //Ultility.Ultility.EVENT_PREFIX +                            
                                                    throw new ParsingException("LTL ends with invalid symbol " + tk.Token.Text + "!", tk.Token);
                                                }

                                                PNAssertionLTL assert = new PNAssertionLTL(proc, ltl.Trim());
                                                BuchiAutomata BA = LTL2BA.FormulaToBA("!(" + ltl.Trim() + ")", options, ASS.Token); //.Replace(".", Ultility.Ultility.DOT_PREFIX)      
                                                BA.HasXOperator = hasXoperator;
                                                BuchiAutomata PositiveBA = LTL2BA.FormulaToBA(ltl.Trim(), options, ASS.Token);
                                                PositiveBA.HasXOperator = hasXoperator;
                                                assert.SeteBAs(BA, PositiveBA);

                                                ass = assert;

                                            }

                                        }


                                    }
                                    break;
                                case 2:
                                    DebugEnterAlt(2);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:840:5: ( 'deadlockfree' )
                                    {
                                        DebugLocation(840, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:840:5: ( 'deadlockfree' )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:840:7: 'deadlockfree'
                                        {
                                            DebugLocation(840, 7);
                                            Match(input, 86, Follow._86_in_assertion408); if (state.failed) return;
                                            DebugLocation(841, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                ass = new PNAssertionDeadLock(proc, false);

                                            }

                                        }


                                    }
                                    break;
                                case 3:
                                    DebugEnterAlt(3);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:845:5: ( 'nonterminating' )
                                    {
                                        DebugLocation(845, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:845:5: ( 'nonterminating' )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:845:7: 'nonterminating'
                                        {
                                            DebugLocation(845, 7);
                                            Match(input, 87, Follow._87_in_assertion428); if (state.failed) return;
                                            DebugLocation(846, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                ass = new PNAssertionDeadLock(proc, true);

                                            }

                                        }


                                    }
                                    break;
                                case 4:
                                    DebugEnterAlt(4);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:850:5: ( 'divergencefree' )
                                    {
                                        DebugLocation(850, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:850:5: ( 'divergencefree' )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:850:7: 'divergencefree'
                                        {
                                            DebugLocation(850, 7);
                                            Match(input, 88, Follow._88_in_assertion452); if (state.failed) return;
                                            DebugLocation(851, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                // comment following line by Tinh because this property isn't cared now
                                                // ass = new CSPAssertionDivergence(proc);

                                            }

                                        }


                                    }
                                    break;
                                case 5:
                                    DebugEnterAlt(5);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:856:5: ( 'deterministic' )
                                    {
                                        DebugLocation(856, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:856:5: ( 'deterministic' )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:856:7: 'deterministic'
                                        {
                                            DebugLocation(856, 7);
                                            Match(input, 89, Follow._89_in_assertion470); if (state.failed) return;
                                            DebugLocation(857, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                // comment following line by Tinh because this property isn't cared now
                                                // ass = new CSPAssertionDeterminism(proc);

                                            }

                                        }


                                    }
                                    break;
                                case 6:
                                    DebugEnterAlt(6);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:863:5: ( 'reaches' label= ID (exp= withClause[out contraint] )? )
                                    {
                                        DebugLocation(863, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:863:5: ( 'reaches' label= ID (exp= withClause[out contraint] )? )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:863:7: 'reaches' label= ID (exp= withClause[out contraint] )?
                                        {
                                            DebugLocation(863, 7);
                                            Match(input, 90, Follow._90_in_assertion491); if (state.failed) return;
                                            DebugLocation(863, 23);
                                            label = (CommonTree)Match(input, ID, Follow._ID_in_assertion496); if (state.failed) return;
                                            DebugLocation(863, 28);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:863:28: (exp= withClause[out contraint] )?
                                            int alt5 = 2;
                                            try
                                            {
                                                DebugEnterSubRule(5);
                                                try
                                                {
                                                    DebugEnterDecision(5, decisionCanBacktrack[5]);
                                                    int LA5_0 = input.LA(1);

                                                    if ((LA5_0 == 94))
                                                    {
                                                        alt5 = 1;
                                                    }
                                                }
                                                finally { DebugExitDecision(5); }
                                                switch (alt5)
                                                {
                                                    case 1:
                                                        DebugEnterAlt(1);
                                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:863:29: exp= withClause[out contraint]
                                                        {
                                                            DebugLocation(863, 32);
                                                            PushFollow(Follow._withClause_in_assertion502);
                                                            exp = withClause(out contraint);
                                                            PopFollow();
                                                            if (state.failed) return;

                                                        }
                                                        break;

                                                }
                                            }
                                            finally { DebugExitSubRule(5); }

                                            DebugLocation(864, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                if (exp != null)
                                                {
                                                    ass = new PNAssertionReachabilityWith(proc, label.Text, contraint, exp);
                                                    declareTokens.Add(label.Token);
                                                }
                                                else
                                                {
                                                    ass = new PNAssertionReachability(proc, label.Text);
                                                    declareTokens.Add(label.Token);
                                                }

                                            }

                                        }


                                    }
                                    break;
                                case 7:
                                    DebugEnterAlt(7);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:878:5: ( 'refines' targetProcess= definitionRef )
                                    {
                                        DebugLocation(878, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:878:5: ( 'refines' targetProcess= definitionRef )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:878:7: 'refines' targetProcess= definitionRef
                                        {
                                            DebugLocation(878, 7);
                                            Match(input, 91, Follow._91_in_assertion530); if (state.failed) return;
                                            DebugLocation(878, 31);
                                            PushFollow(Follow._definitionRef_in_assertion535);
                                            targetProcess = definitionRef();
                                            PopFollow();
                                            if (state.failed) return;
                                            DebugLocation(879, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                // comment following line by Tinh because this property isn't cared now
                                                // ass = new CSPAssertionRefinement(proc, targetProcess);

                                            }

                                        }


                                    }
                                    break;
                                case 8:
                                    DebugEnterAlt(8);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:885:5: ( 'refines' '<F>' targetProcess= definitionRef )
                                    {
                                        DebugLocation(885, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:885:5: ( 'refines' '<F>' targetProcess= definitionRef )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:885:7: 'refines' '<F>' targetProcess= definitionRef
                                        {
                                            DebugLocation(885, 7);
                                            Match(input, 91, Follow._91_in_assertion557); if (state.failed) return;
                                            DebugLocation(885, 17);
                                            Match(input, 92, Follow._92_in_assertion559); if (state.failed) return;
                                            DebugLocation(885, 37);
                                            PushFollow(Follow._definitionRef_in_assertion564);
                                            targetProcess = definitionRef();
                                            PopFollow();
                                            if (state.failed) return;
                                            DebugLocation(886, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                // comment following line by Tinh because this property isn't cared now
                                                // ass = new CSPAssertionRefinementF(proc, targetProcess);

                                            }

                                        }


                                    }
                                    break;
                                case 9:
                                    DebugEnterAlt(9);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:892:5: ( 'refines' '<FD>' targetProcess= definitionRef )
                                    {
                                        DebugLocation(892, 5);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:892:5: ( 'refines' '<FD>' targetProcess= definitionRef )
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:892:7: 'refines' '<FD>' targetProcess= definitionRef
                                        {
                                            DebugLocation(892, 7);
                                            Match(input, 91, Follow._91_in_assertion586); if (state.failed) return;
                                            DebugLocation(892, 17);
                                            Match(input, 93, Follow._93_in_assertion588); if (state.failed) return;
                                            DebugLocation(892, 38);
                                            PushFollow(Follow._definitionRef_in_assertion593);
                                            targetProcess = definitionRef();
                                            PopFollow();
                                            if (state.failed) return;
                                            DebugLocation(893, 3);
                                            if ((state.backtracking == 0))
                                            {

                                                // comment following line by Tinh because this property isn't cared now
                                                // ass = new CSPAssertionRefinementFD(proc, targetProcess);

                                            }

                                        }


                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(6); }

                        DebugLocation(899, 2);
                        if ((state.backtracking == 0))
                        {

                            string assString = ass.ToString();
                            if (Spec.AssertionDatabase.ContainsKey(assString))
                            {
                                throw new ParsingException("Assertion " + assString + " is defined already!", ASS.Token);
                            }
                            else
                            {
                                ass.AssertToken = ASS.Token;
                                Spec.AssertionDatabase.Add(assString, ass);
                            }

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 5, assertion_StartIndex); }
                }
                DebugLocation(911, 1);
            }
            finally { DebugExitRule(GrammarFileName, "assertion"); }
            return;

        }
        // $ANTLR end "assertion"


        protected virtual void Enter_withClause() { }
        protected virtual void Leave_withClause() { }

        // $ANTLR start "withClause"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:913:1: withClause[out QueryConstraintType contraint] returns [Expression exp = null] : ^(wtag= 'with' (tag= 'min' | tag= 'max' ) e= expression[new List<string>(), true, null] ) ;
        [GrammarRule("withClause")]
        private Expression withClause(out QueryConstraintType contraint)
        {

            Expression exp = null;
            int withClause_StartIndex = input.Index;
            CommonTree wtag = null;
            CommonTree tag = null;
            Expression e = default(Expression);

            paraphrases.Push("in with clause"); contraint = QueryConstraintType.NONE;
            try
            {
                DebugEnterRule(GrammarFileName, "withClause");
                DebugLocation(913, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 6)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:916:2: ( ^(wtag= 'with' (tag= 'min' | tag= 'max' ) e= expression[new List<string>(), true, null] ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:916:4: ^(wtag= 'with' (tag= 'min' | tag= 'max' ) e= expression[new List<string>(), true, null] )
                    {
                        DebugLocation(916, 4);
                        DebugLocation(916, 10);
                        wtag = (CommonTree)Match(input, 94, Follow._94_in_withClause653); if (state.failed) return exp;

                        Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                        DebugLocation(916, 18);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:916:18: (tag= 'min' | tag= 'max' )
                        int alt7 = 2;
                        try
                        {
                            DebugEnterSubRule(7);
                            try
                            {
                                DebugEnterDecision(7, decisionCanBacktrack[7]);
                                int LA7_0 = input.LA(1);

                                if ((LA7_0 == 95))
                                {
                                    alt7 = 1;
                                }
                                else if ((LA7_0 == 96))
                                {
                                    alt7 = 2;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return exp; }
                                    NoViableAltException nvae = new NoViableAltException("", 7, 0, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                            }
                            finally { DebugExitDecision(7); }
                            switch (alt7)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:916:19: tag= 'min'
                                    {
                                        DebugLocation(916, 22);
                                        tag = (CommonTree)Match(input, 95, Follow._95_in_withClause658); if (state.failed) return exp;
                                        DebugLocation(916, 29);
                                        if ((state.backtracking == 0))
                                        {
                                            contraint = QueryConstraintType.MIN;
                                        }

                                    }
                                    break;
                                case 2:
                                    DebugEnterAlt(2);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:916:71: tag= 'max'
                                    {
                                        DebugLocation(916, 74);
                                        tag = (CommonTree)Match(input, 96, Follow._96_in_withClause667); if (state.failed) return exp;
                                        DebugLocation(916, 81);
                                        if ((state.backtracking == 0))
                                        {
                                            contraint = QueryConstraintType.MAX;
                                        }

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(7); }

                        DebugLocation(916, 122);
                        PushFollow(Follow._expression_in_withClause674);
                        e = expression(new List<string>(), true, null);
                        PopFollow();
                        if (state.failed) return exp;

                        Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                        DebugLocation(917, 2);
                        if ((state.backtracking == 0))
                        {

                            IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(wtag.Children[1] as CommonTree, input);
                            PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e, token1, "in " + tag.Text + " clause", Spec.SpecValuation, ConstantDatabase);
                            exp = e;

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 6, withClause_StartIndex); }
                }
                DebugLocation(922, 1);
            }
            finally { DebugExitRule(GrammarFileName, "withClause"); }
            return exp;

        }
        // $ANTLR end "withClause"


        protected virtual void Enter_definitionRef() { }
        protected virtual void Leave_definitionRef() { }

        // $ANTLR start "definitionRef"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:924:1: definitionRef returns [DefinitionRef def = null] : ^( DEFINITION_REF_NODE ID (e= expression[new List<string>(), true, null] )* ) ;
        [GrammarRule("definitionRef")]
        private DefinitionRef definitionRef()
        {

            DefinitionRef def = null;
            int definitionRef_StartIndex = input.Index;
            CommonTree ID3 = null;
            Expression e = default(Expression);

            paraphrases.Push("in process invocation");
            List<Expression> para = new List<Expression>();

            try
            {
                DebugEnterRule(GrammarFileName, "definitionRef");
                DebugLocation(924, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 7)) { return def; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:929:2: ( ^( DEFINITION_REF_NODE ID (e= expression[new List<string>(), true, null] )* ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:929:4: ^( DEFINITION_REF_NODE ID (e= expression[new List<string>(), true, null] )* )
                    {
                        DebugLocation(929, 4);
                        DebugLocation(929, 6);
                        Match(input, DEFINITION_REF_NODE, Follow._DEFINITION_REF_NODE_in_definitionRef708); if (state.failed) return def;

                        Match(input, TokenTypes.Down, null); if (state.failed) return def;
                        DebugLocation(929, 26);
                        ID3 = (CommonTree)Match(input, ID, Follow._ID_in_definitionRef710); if (state.failed) return def;
                        DebugLocation(930, 3);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:930:3: (e= expression[new List<string>(), true, null] )*
                        try
                        {
                            DebugEnterSubRule(8);
                            while (true)
                            {
                                int alt8 = 2;
                                try
                                {
                                    DebugEnterDecision(8, decisionCanBacktrack[8]);
                                    int LA8_0 = input.LA(1);

                                    if (((LA8_0 >= VAR_NODE && LA8_0 <= CALL_NODE) || LA8_0 == ASSIGNMENT_NODE || LA8_0 == UNARY_NODE || (LA8_0 >= CLASS_CALL_NODE && LA8_0 <= CLASS_CALL_INSTANCE_NODE) || LA8_0 == INT || LA8_0 == 76 || (LA8_0 >= 78 && LA8_0 <= 79) || (LA8_0 >= 102 && LA8_0 <= 104) || (LA8_0 >= 110 && LA8_0 <= 123) || LA8_0 == 127 || LA8_0 == 153))
                                    {
                                        alt8 = 1;
                                    }


                                }
                                finally { DebugExitDecision(8); }
                                switch (alt8)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:930:4: e= expression[new List<string>(), true, null]
                                        {
                                            DebugLocation(930, 5);
                                            PushFollow(Follow._expression_in_definitionRef718);
                                            e = expression(new List<string>(), true, null);
                                            PopFollow();
                                            if (state.failed) return def;
                                            DebugLocation(931, 4);
                                            if ((state.backtracking == 0))
                                            {

                                                if (ConstantDatabase.Count > 0)
                                                {
                                                    e = e.ClearConstant(ConstantDatabase);
                                                }
                                                //e.BuildVars();
                                                if (e.HasVar)
                                                {
                                                    throw new ParsingException("Variables are not allowed in process invocation!", ID3.Token);
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        e = EvaluatorDenotational.Evaluate(e, null) as Expression;
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        throw new ParsingException(ex.Message, ID3.Token);
                                                    }
                                                }
                                                para.Add(e);

                                            }

                                        }
                                        break;

                                    default:
                                        goto loop8;
                                }
                            }

                        loop8:
                            ;

                        }
                        finally { DebugExitSubRule(8); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return def;
                        DebugLocation(956, 2);
                        if ((state.backtracking == 0))
                        {

                            def = new DefinitionRef(ID3.Text, para.ToArray());
                            dlist.Add(def);
                            dtokens.Add(ID3.Token);

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 7, definitionRef_StartIndex); }
                }
                DebugLocation(961, 1);
            }
            finally { DebugExitRule(GrammarFileName, "definitionRef"); }
            return def;

        }
        // $ANTLR end "definitionRef"


        protected virtual void Enter_define() { }
        protected virtual void Leave_define() { }

        // $ANTLR start "define"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:963:1: define : ( ^( DEFINE_CONSTANT_NODE ID INT (negtive= '-' )? ) | ^( DEFINE_CONSTANT_NODE name= ID value= ( 'true' | 'false' ) ) | ^( DEFINE_CONSTANT_NODE ( ID )+ ) | ^( DEFINE_NODE ID (parameters= dparameter )? p= statement[vars, null] ) );
        [GrammarRule("define")]
        private void define()
        {

            int define_StartIndex = input.Index;
            CommonTree negtive = null;
            CommonTree name = null;
            CommonTree value = null;
            CommonTree ID4 = null;
            CommonTree INT5 = null;
            CommonTree ID6 = null;
            CommonTree ID7 = null;
            CommonTree DEFINE_NODE8 = null;
            List<string> parameters = default(List<string>);
            Expression p = default(Expression);

            paraphrases.Push("in constant/enum/(LTL proposition condition) definition"); int i = 0; List<string> vars = new List<string>();
            try
            {
                DebugEnterRule(GrammarFileName, "define");
                DebugLocation(963, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 8)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:966:2: ( ^( DEFINE_CONSTANT_NODE ID INT (negtive= '-' )? ) | ^( DEFINE_CONSTANT_NODE name= ID value= ( 'true' | 'false' ) ) | ^( DEFINE_CONSTANT_NODE ( ID )+ ) | ^( DEFINE_NODE ID (parameters= dparameter )? p= statement[vars, null] ) )
                    int alt12 = 4;
                    try
                    {
                        DebugEnterDecision(12, decisionCanBacktrack[12]);
                        int LA12_0 = input.LA(1);

                        if ((LA12_0 == DEFINE_CONSTANT_NODE))
                        {
                            int LA12_1 = input.LA(2);

                            if ((LA12_1 == DOWN))
                            {
                                int LA12_3 = input.LA(3);

                                if ((LA12_3 == ID))
                                {
                                    switch (input.LA(4))
                                    {
                                        case INT:
                                            {
                                                alt12 = 1;
                                            }
                                            break;
                                        case 103:
                                        case 104:
                                            {
                                                alt12 = 2;
                                            }
                                            break;
                                        case UP:
                                        case ID:
                                            {
                                                alt12 = 3;
                                            }
                                            break;
                                        default:
                                            {
                                                if (state.backtracking > 0) { state.failed = true; return; }
                                                NoViableAltException nvae = new NoViableAltException("", 12, 4, input);

                                                DebugRecognitionException(nvae);
                                                throw nvae;
                                            }
                                    }

                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    NoViableAltException nvae = new NoViableAltException("", 12, 3, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                            }
                            else
                            {
                                if (state.backtracking > 0) { state.failed = true; return; }
                                NoViableAltException nvae = new NoViableAltException("", 12, 1, input);

                                DebugRecognitionException(nvae);
                                throw nvae;
                            }
                        }
                        else if ((LA12_0 == DEFINE_NODE))
                        {
                            alt12 = 4;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return; }
                            NoViableAltException nvae = new NoViableAltException("", 12, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(12); }
                    switch (alt12)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:966:4: ^( DEFINE_CONSTANT_NODE ID INT (negtive= '-' )? )
                            {
                                DebugLocation(966, 4);
                                DebugLocation(966, 6);
                                Match(input, DEFINE_CONSTANT_NODE, Follow._DEFINE_CONSTANT_NODE_in_define760); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(966, 27);
                                ID4 = (CommonTree)Match(input, ID, Follow._ID_in_define762); if (state.failed) return;
                                DebugLocation(966, 30);
                                INT5 = (CommonTree)Match(input, INT, Follow._INT_in_define764); if (state.failed) return;
                                DebugLocation(966, 34);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:966:34: (negtive= '-' )?
                                int alt9 = 2;
                                try
                                {
                                    DebugEnterSubRule(9);
                                    try
                                    {
                                        DebugEnterDecision(9, decisionCanBacktrack[9]);
                                        int LA9_0 = input.LA(1);

                                        if ((LA9_0 == 102))
                                        {
                                            alt9 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(9); }
                                    switch (alt9)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:966:35: negtive= '-'
                                            {
                                                DebugLocation(966, 42);
                                                negtive = (CommonTree)Match(input, 102, Follow._102_in_define769); if (state.failed) return;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(9); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return;
                                DebugLocation(967, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckDuplicatedDeclaration(ID4.Token);
                                    if (negtive == null)
                                    {
                                        ConstantDatabase.Add(ID4.Text, new IntConstant(int.Parse(INT5.Text), ID4.Text));
                                    }
                                    else
                                    {
                                        ConstantDatabase.Add(ID4.Text, new IntConstant(int.Parse(INT5.Text) * -1, ID4.Text));
                                    }
                                    Spec.DeclaritionTable.Add(ID4.Text, new Declaration(DeclarationType.Constant, new ParsingException(ID4.Text, ID4.Token)));

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:979:4: ^( DEFINE_CONSTANT_NODE name= ID value= ( 'true' | 'false' ) )
                            {
                                DebugLocation(979, 4);
                                DebugLocation(979, 6);
                                Match(input, DEFINE_CONSTANT_NODE, Follow._DEFINE_CONSTANT_NODE_in_define781); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(979, 31);
                                name = (CommonTree)Match(input, ID, Follow._ID_in_define785); if (state.failed) return;
                                DebugLocation(979, 40);
                                value = (CommonTree)input.LT(1);
                                if ((input.LA(1) >= 103 && input.LA(1) <= 104))
                                {
                                    input.Consume();
                                    state.errorRecovery = false; state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    DebugRecognitionException(mse);
                                    throw mse;
                                }


                                Match(input, TokenTypes.Up, null); if (state.failed) return;
                                DebugLocation(980, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckDuplicatedDeclaration(name.Token);
                                    ConstantDatabase.Add(name.Text, new BoolConstant(bool.Parse(value.Text), name.Text));
                                    Spec.DeclaritionTable.Add(name.Text, new Declaration(DeclarationType.Constant, new ParsingException(name.Text, name.Token)));

                                }

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:985:4: ^( DEFINE_CONSTANT_NODE ( ID )+ )
                            {
                                DebugLocation(985, 4);
                                DebugLocation(985, 6);
                                Match(input, DEFINE_CONSTANT_NODE, Follow._DEFINE_CONSTANT_NODE_in_define805); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(986, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:986:3: ( ID )+
                                int cnt10 = 0;
                                try
                                {
                                    DebugEnterSubRule(10);
                                    while (true)
                                    {
                                        int alt10 = 2;
                                        try
                                        {
                                            DebugEnterDecision(10, decisionCanBacktrack[10]);
                                            int LA10_0 = input.LA(1);

                                            if ((LA10_0 == ID))
                                            {
                                                alt10 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(10); }
                                        switch (alt10)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:986:5: ID
                                                {
                                                    DebugLocation(986, 5);
                                                    ID6 = (CommonTree)Match(input, ID, Follow._ID_in_define812); if (state.failed) return;
                                                    DebugLocation(987, 5);
                                                    if ((state.backtracking == 0))
                                                    {

                                                        CheckDuplicatedDeclaration(ID6.Token);
                                                        ConstantDatabase.Add(ID6.Text, new IntConstant(i++, ID6.Text));
                                                        Spec.DeclaritionTable.Add(ID6.Text, new Declaration(DeclarationType.Constant, new ParsingException(ID6.Text, ID6.Token)));

                                                    }

                                                }
                                                break;

                                            default:
                                                if (cnt10 >= 1)
                                                    goto loop10;

                                                if (state.backtracking > 0) { state.failed = true; return; }
                                                EarlyExitException eee10 = new EarlyExitException(10, input);
                                                DebugRecognitionException(eee10);
                                                throw eee10;
                                        }
                                        cnt10++;
                                    }
                                loop10:
                                    ;

                                }
                                finally { DebugExitSubRule(10); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return;

                            }
                            break;
                        case 4:
                            DebugEnterAlt(4);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:994:4: ^( DEFINE_NODE ID (parameters= dparameter )? p= statement[vars, null] )
                            {
                                DebugLocation(994, 4);
                                DebugLocation(994, 6);
                                DEFINE_NODE8 = (CommonTree)Match(input, DEFINE_NODE, Follow._DEFINE_NODE_in_define834); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(994, 18);
                                ID7 = (CommonTree)Match(input, ID, Follow._ID_in_define836); if (state.failed) return;
                                DebugLocation(994, 21);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:994:21: (parameters= dparameter )?
                                int alt11 = 2;
                                try
                                {
                                    DebugEnterSubRule(11);
                                    try
                                    {
                                        DebugEnterDecision(11, decisionCanBacktrack[11]);
                                        int LA11_0 = input.LA(1);

                                        if ((LA11_0 == DPARAMETER_NODE))
                                        {
                                            alt11 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(11); }
                                    switch (alt11)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:994:22: parameters= dparameter
                                            {
                                                DebugLocation(994, 32);
                                                PushFollow(Follow._dparameter_in_define841);
                                                parameters = dparameter();
                                                PopFollow();
                                                if (state.failed) return;
                                                DebugLocation(994, 44);
                                                if ((state.backtracking == 0))
                                                {
                                                    vars.AddRange(parameters);
                                                }

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(11); }

                                DebugLocation(994, 76);
                                PushFollow(Follow._statement_in_define849);
                                p = statement(vars, null);
                                PopFollow();
                                if (state.failed) return;

                                Match(input, TokenTypes.Up, null); if (state.failed) return;
                                DebugLocation(995, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckDuplicatedDeclaration(ID7.Token);
                                    IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(DEFINE_NODE8.Children[1] as CommonTree, input);
                                    //PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(p, token1, "in define declaration", Spec.SpecValuation, ConstantDatabase);		
                                    p = p.ClearConstant(ConstantDatabase);

                                    if (parameters == null)
                                    {
                                        Spec.DeclarationDatabase.Add(ID7.Text, p);
                                        Spec.DeclaritionTable.Add(ID7.Text, new Declaration(DeclarationType.Declaration, new ParsingException(ID7.Text, ID7.Token)));
                                    }
                                    else
                                    {
                                        Spec.MacroDefinition.Add(ID7.Text + parameters.Count, new KeyValuePair<List<string>, Expression>(parameters, p));
                                        Spec.DeclaritionTable.Add(ID7.Text, new Declaration(DeclarationType.Declaration, new ParsingException(ID7.Text + "(" + Common.Classes.Ultility.Ultility.PPStringList(parameters) + ")", ID7.Token)));
                                    }


                                }

                            }
                            break;

                    }
                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 8, define_StartIndex); }
                }
                DebugLocation(1013, 1);
            }
            finally { DebugExitRule(GrammarFileName, "define"); }
            return;

        }
        // $ANTLR end "define"


        protected virtual void Enter_dparameter() { }
        protected virtual void Leave_dparameter() { }

        // $ANTLR start "dparameter"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1016:1: dparameter returns [List<string> parameters = new List<string>()] : ^( DPARAMETER_NODE ( ID )+ ) ;
        [GrammarRule("dparameter")]
        private List<string> dparameter()
        {

            List<string> parameters = new List<string>();
            int dparameter_StartIndex = input.Index;
            CommonTree ID9 = null;

            try
            {
                DebugEnterRule(GrammarFileName, "dparameter");
                DebugLocation(1016, 8);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 9)) { return parameters; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1017:9: ( ^( DPARAMETER_NODE ( ID )+ ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1017:12: ^( DPARAMETER_NODE ( ID )+ )
                    {
                        DebugLocation(1017, 12);
                        DebugLocation(1017, 14);
                        Match(input, DPARAMETER_NODE, Follow._DPARAMETER_NODE_in_dparameter879); if (state.failed) return parameters;

                        Match(input, TokenTypes.Down, null); if (state.failed) return parameters;
                        DebugLocation(1017, 30);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1017:30: ( ID )+
                        int cnt13 = 0;
                        try
                        {
                            DebugEnterSubRule(13);
                            while (true)
                            {
                                int alt13 = 2;
                                try
                                {
                                    DebugEnterDecision(13, decisionCanBacktrack[13]);
                                    int LA13_0 = input.LA(1);

                                    if ((LA13_0 == ID))
                                    {
                                        alt13 = 1;
                                    }


                                }
                                finally { DebugExitDecision(13); }
                                switch (alt13)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1017:31: ID
                                        {
                                            DebugLocation(1017, 31);
                                            ID9 = (CommonTree)Match(input, ID, Follow._ID_in_dparameter882); if (state.failed) return parameters;
                                            DebugLocation(1017, 34);
                                            if ((state.backtracking == 0))
                                            {
                                                CheckDuplicatedDeclaration(ID9.Token, parameters); parameters.Add(ID9.Text);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt13 >= 1)
                                            goto loop13;

                                        if (state.backtracking > 0) { state.failed = true; return parameters; }
                                        EarlyExitException eee13 = new EarlyExitException(13, input);
                                        DebugRecognitionException(eee13);
                                        throw eee13;
                                }
                                cnt13++;
                            }
                        loop13:
                            ;

                        }
                        finally { DebugExitSubRule(13); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return parameters;

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 9, dparameter_StartIndex); }
                }
                DebugLocation(1018, 8);
            }
            finally { DebugExitRule(GrammarFileName, "dparameter"); }
            return parameters;

        }
        // $ANTLR end "dparameter"


        protected virtual void Enter_block() { }
        protected virtual void Leave_block() { }

        // $ANTLR start "block"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1020:1: block[List<string> vars, List<string> sourceVars] returns [Expression exp = null] : ^( BLOCK_NODE (s= statement[vars, sourceVars] )* ) ;
        [GrammarRule("block")]
        private Expression block(List<string> vars, List<string> sourceVars)
        {

            Expression exp = null;
            int block_StartIndex = input.Index;
            Expression s = default(Expression);

            List<Expression> stmlist = new List<Expression>();
            try
            {
                DebugEnterRule(GrammarFileName, "block");
                DebugLocation(1020, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 10)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1022:2: ( ^( BLOCK_NODE (s= statement[vars, sourceVars] )* ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1022:5: ^( BLOCK_NODE (s= statement[vars, sourceVars] )* )
                    {
                        DebugLocation(1022, 5);
                        DebugLocation(1022, 7);
                        Match(input, BLOCK_NODE, Follow._BLOCK_NODE_in_block918); if (state.failed) return exp;

                        DebugLocation(1023, 2);
                        if ((state.backtracking == 0))
                        {

                            BlockDepth++;

                        }

                        if (input.LA(1) == TokenTypes.Down)
                        {
                            Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                            DebugLocation(1026, 3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1026:3: (s= statement[vars, sourceVars] )*
                            try
                            {
                                DebugEnterSubRule(14);
                                while (true)
                                {
                                    int alt14 = 2;
                                    try
                                    {
                                        DebugEnterDecision(14, decisionCanBacktrack[14]);
                                        int LA14_0 = input.LA(1);

                                        if ((LA14_0 == BLOCK_NODE || (LA14_0 >= VAR_NODE && LA14_0 <= CALL_NODE) || LA14_0 == ASSIGNMENT_NODE || LA14_0 == UNARY_NODE || (LA14_0 >= LOCAL_VAR_NODE && LA14_0 <= LOCAL_ARRAY_NODE) || (LA14_0 >= CLASS_CALL_NODE && LA14_0 <= CLASS_CALL_INSTANCE_NODE) || LA14_0 == INT || LA14_0 == 76 || (LA14_0 >= 78 && LA14_0 <= 79) || (LA14_0 >= 102 && LA14_0 <= 104) || (LA14_0 >= 110 && LA14_0 <= 123) || LA14_0 == 127 || LA14_0 == 131 || LA14_0 == 133 || LA14_0 == 153))
                                        {
                                            alt14 = 1;
                                        }


                                    }
                                    finally { DebugExitDecision(14); }
                                    switch (alt14)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1026:4: s= statement[vars, sourceVars]
                                            {
                                                DebugLocation(1026, 5);
                                                PushFollow(Follow._statement_in_block928);
                                                s = statement(vars, sourceVars);
                                                PopFollow();
                                                if (state.failed) return exp;
                                                DebugLocation(1026, 34);
                                                if ((state.backtracking == 0))
                                                {
                                                    stmlist.Add(s);
                                                }

                                            }
                                            break;

                                        default:
                                            goto loop14;
                                    }
                                }

                            loop14:
                                ;

                            }
                            finally { DebugExitSubRule(14); }


                            Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                        }
                        DebugLocation(1027, 2);
                        if ((state.backtracking == 0))
                        {

                            if (stmlist.Count > 0)
                            {
                                exp = stmlist[stmlist.Count - 1];
                            }
                            if (stmlist.Count > 1)
                            {
                                for (int i = stmlist.Count - 2; i >= 0; i--)
                                {
                                    exp = new PAT.Common.Classes.Expressions.ExpressionClass.Sequence(stmlist[i], exp);
                                }
                            }

                            while (LocalVariablesStack.Count > 0)
                            {
                                if (LocalVariablesStack.Peek().Value == BlockDepth)
                                {
                                    LocalVariablesStack.Pop();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            BlockDepth--;


                        }

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 10, block_StartIndex); }
                }
                DebugLocation(1054, 1);
            }
            finally { DebugExitRule(GrammarFileName, "block"); }
            return exp;

        }
        // $ANTLR end "block"


        protected virtual void Enter_statement() { }
        protected virtual void Leave_statement() { }

        // $ANTLR start "statement"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1056:1: statement[List<string> vars, List<string> sourceVars] returns [Expression exp = null] : (e= block[vars, sourceVars] | e= localVariableDeclaration[vars, sourceVars] | e= ifExpression[vars, sourceVars] | e= whileExpression[vars, sourceVars] | e= expression[vars, true, sourceVars] );
        [GrammarRule("statement")]
        private Expression statement(List<string> vars, List<string> sourceVars)
        {

            Expression exp = null;
            int statement_StartIndex = input.Index;
            Expression e = default(Expression);

            try
            {
                DebugEnterRule(GrammarFileName, "statement");
                DebugLocation(1056, 5);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 11)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1057:6: (e= block[vars, sourceVars] | e= localVariableDeclaration[vars, sourceVars] | e= ifExpression[vars, sourceVars] | e= whileExpression[vars, sourceVars] | e= expression[vars, true, sourceVars] )
                    int alt15 = 5;
                    try
                    {
                        DebugEnterDecision(15, decisionCanBacktrack[15]);
                        switch (input.LA(1))
                        {
                            case BLOCK_NODE:
                                {
                                    alt15 = 1;
                                }
                                break;
                            case LOCAL_VAR_NODE:
                            case LOCAL_ARRAY_NODE:
                                {
                                    alt15 = 2;
                                }
                                break;
                            case 131:
                                {
                                    alt15 = 3;
                                }
                                break;
                            case 133:
                                {
                                    alt15 = 4;
                                }
                                break;
                            case VAR_NODE:
                            case CALL_NODE:
                            case ASSIGNMENT_NODE:
                            case UNARY_NODE:
                            case CLASS_CALL_NODE:
                            case CLASS_CALL_INSTANCE_NODE:
                            case INT:
                            case 76:
                            case 78:
                            case 79:
                            case 102:
                            case 103:
                            case 104:
                            case 110:
                            case 111:
                            case 112:
                            case 113:
                            case 114:
                            case 115:
                            case 116:
                            case 117:
                            case 118:
                            case 119:
                            case 120:
                            case 121:
                            case 122:
                            case 123:
                            case 127:
                            case 153:
                                {
                                    alt15 = 5;
                                }
                                break;
                            default:
                                {
                                    if (state.backtracking > 0) { state.failed = true; return exp; }
                                    NoViableAltException nvae = new NoViableAltException("", 15, 0, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                        }

                    }
                    finally { DebugExitDecision(15); }
                    switch (alt15)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1057:8: e= block[vars, sourceVars]
                            {
                                DebugLocation(1057, 10);
                                PushFollow(Follow._block_in_statement963);
                                e = block(vars, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1057, 36);
                                if ((state.backtracking == 0))
                                {
                                    exp = e;
                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1058:8: e= localVariableDeclaration[vars, sourceVars]
                            {
                                DebugLocation(1058, 10);
                                PushFollow(Follow._localVariableDeclaration_in_statement979);
                                e = localVariableDeclaration(vars, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1058, 55);
                                if ((state.backtracking == 0))
                                {
                                    exp = e;
                                }

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1059:8: e= ifExpression[vars, sourceVars]
                            {
                                DebugLocation(1059, 10);
                                PushFollow(Follow._ifExpression_in_statement995);
                                e = ifExpression(vars, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1059, 43);
                                if ((state.backtracking == 0))
                                {
                                    exp = e;
                                }

                            }
                            break;
                        case 4:
                            DebugEnterAlt(4);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1060:8: e= whileExpression[vars, sourceVars]
                            {
                                DebugLocation(1060, 10);
                                PushFollow(Follow._whileExpression_in_statement1011);
                                e = whileExpression(vars, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1060, 46);
                                if ((state.backtracking == 0))
                                {
                                    exp = e;
                                }

                            }
                            break;
                        case 5:
                            DebugEnterAlt(5);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1061:8: e= expression[vars, true, sourceVars]
                            {
                                DebugLocation(1061, 10);
                                PushFollow(Follow._expression_in_statement1027);
                                e = expression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1061, 47);
                                if ((state.backtracking == 0))
                                {
                                    exp = e;
                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 11, statement_StartIndex); }
                }
                DebugLocation(1062, 5);
            }
            finally { DebugExitRule(GrammarFileName, "statement"); }
            return exp;

        }
        // $ANTLR end "statement"


        protected virtual void Enter_localVariableDeclaration() { }
        protected virtual void Leave_localVariableDeclaration() { }

        // $ANTLR start "localVariableDeclaration"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1064:1: localVariableDeclaration[List<string> vars, List<string> sourceVars] returns [LetDefinition exp = null] : ( ^( LOCAL_VAR_NODE ID (e= expression[vars, true, null] | (e= recordExpression[vars, null, $ID.Token] ) )? ) | ^( LOCAL_ARRAY_NODE ID (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? ) );
        [GrammarRule("localVariableDeclaration")]
        private LetDefinition localVariableDeclaration(List<string> vars, List<string> sourceVars)
        {

            LetDefinition exp = null;
            int localVariableDeclaration_StartIndex = input.Index;
            CommonTree ID10 = null;
            CommonTree ID11 = null;
            Expression e = default(Expression);
            Expression rd = default(Expression);


            List<int> list = new List<int>();

            try
            {
                DebugEnterRule(GrammarFileName, "localVariableDeclaration");
                DebugLocation(1064, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 12)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1068:2: ( ^( LOCAL_VAR_NODE ID (e= expression[vars, true, null] | (e= recordExpression[vars, null, $ID.Token] ) )? ) | ^( LOCAL_ARRAY_NODE ID (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? ) )
                    int alt19 = 2;
                    try
                    {
                        DebugEnterDecision(19, decisionCanBacktrack[19]);
                        int LA19_0 = input.LA(1);

                        if ((LA19_0 == LOCAL_VAR_NODE))
                        {
                            alt19 = 1;
                        }
                        else if ((LA19_0 == LOCAL_ARRAY_NODE))
                        {
                            alt19 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return exp; }
                            NoViableAltException nvae = new NoViableAltException("", 19, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(19); }
                    switch (alt19)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1068:5: ^( LOCAL_VAR_NODE ID (e= expression[vars, true, null] | (e= recordExpression[vars, null, $ID.Token] ) )? )
                            {
                                DebugLocation(1068, 5);
                                DebugLocation(1068, 7);
                                Match(input, LOCAL_VAR_NODE, Follow._LOCAL_VAR_NODE_in_localVariableDeclaration1064); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1068, 22);
                                ID10 = (CommonTree)Match(input, ID, Follow._ID_in_localVariableDeclaration1066); if (state.failed) return exp;
                                DebugLocation(1069, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckIDNameDefined(vars, ID10.Token);
                                    LocalVariables.Add(ID10.Text);
                                    LocalVariablesStack.Push(new KeyValuePair<string, int>(ID10.Text, BlockDepth));

                                }
                                DebugLocation(1074, 2);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1074:2: (e= expression[vars, true, null] | (e= recordExpression[vars, null, $ID.Token] ) )?
                                int alt16 = 3;
                                try
                                {
                                    DebugEnterSubRule(16);
                                    try
                                    {
                                        DebugEnterDecision(16, decisionCanBacktrack[16]);
                                        int LA16_0 = input.LA(1);

                                        if (((LA16_0 >= VAR_NODE && LA16_0 <= CALL_NODE) || LA16_0 == ASSIGNMENT_NODE || LA16_0 == UNARY_NODE || (LA16_0 >= CLASS_CALL_NODE && LA16_0 <= CLASS_CALL_INSTANCE_NODE) || LA16_0 == INT || LA16_0 == 76 || (LA16_0 >= 78 && LA16_0 <= 79) || (LA16_0 >= 102 && LA16_0 <= 104) || (LA16_0 >= 110 && LA16_0 <= 123) || LA16_0 == 127 || LA16_0 == 153))
                                        {
                                            alt16 = 1;
                                        }
                                        else if ((LA16_0 == RECORD_NODE))
                                        {
                                            alt16 = 2;
                                        }
                                    }
                                    finally { DebugExitDecision(16); }
                                    switch (alt16)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1074:3: e= expression[vars, true, null]
                                            {
                                                DebugLocation(1074, 4);
                                                PushFollow(Follow._expression_in_localVariableDeclaration1078);
                                                e = expression(vars, true, null);
                                                PopFollow();
                                                if (state.failed) return exp;

                                            }
                                            break;
                                        case 2:
                                            DebugEnterAlt(2);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1074:36: (e= recordExpression[vars, null, $ID.Token] )
                                            {
                                                DebugLocation(1074, 36);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1074:36: (e= recordExpression[vars, null, $ID.Token] )
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1074:38: e= recordExpression[vars, null, $ID.Token]
                                                {
                                                    DebugLocation(1074, 39);
                                                    PushFollow(Follow._recordExpression_in_localVariableDeclaration1087);
                                                    e = recordExpression(vars, null, ID10.Token);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1074, 80);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        Record r = e as Record;
                                                    }

                                                }


                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(16); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1075, 2);
                                if ((state.backtracking == 0))
                                {

                                    if (e == null)
                                    {
                                        e = new IntConstant(0);
                                    }

                                    try
                                    {
                                        if (ConstantDatabase.Count > 0)
                                        {
                                            e = e.ClearConstant(ConstantDatabase);
                                        }

                                        Expression rhv = e;
                                        if (!e.HasVar)
                                        {
                                            rhv = EvaluatorDenotational.Evaluate(e, null);
                                        }

                                        exp = new LetDefinition(ID10.Text, rhv);

                                        if (Spec.SpecValuation.Variables == null)
                                        {
                                            Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ParsingException("Local variable definition error:" + ex.Message, ID10.Token);
                                    }

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1107:4: ^( LOCAL_ARRAY_NODE ID (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? )
                            {
                                DebugLocation(1107, 4);
                                DebugLocation(1107, 6);
                                Match(input, LOCAL_ARRAY_NODE, Follow._LOCAL_ARRAY_NODE_in_localVariableDeclaration1105); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1107, 23);
                                ID11 = (CommonTree)Match(input, ID, Follow._ID_in_localVariableDeclaration1107); if (state.failed) return exp;
                                DebugLocation(1108, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckIDNameDefined(vars, ID11.Token);
                                    LocalVariables.Add(ID11.Text);
                                    LocalVariablesStack.Push(new KeyValuePair<string, int>(ID11.Text, BlockDepth));

                                }
                                DebugLocation(1113, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1113:3: (e= expression[vars, true, null] )+
                                int cnt17 = 0;
                                try
                                {
                                    DebugEnterSubRule(17);
                                    while (true)
                                    {
                                        int alt17 = 2;
                                        try
                                        {
                                            DebugEnterDecision(17, decisionCanBacktrack[17]);
                                            int LA17_0 = input.LA(1);

                                            if (((LA17_0 >= VAR_NODE && LA17_0 <= CALL_NODE) || LA17_0 == ASSIGNMENT_NODE || LA17_0 == UNARY_NODE || (LA17_0 >= CLASS_CALL_NODE && LA17_0 <= CLASS_CALL_INSTANCE_NODE) || LA17_0 == INT || LA17_0 == 76 || (LA17_0 >= 78 && LA17_0 <= 79) || (LA17_0 >= 102 && LA17_0 <= 104) || (LA17_0 >= 110 && LA17_0 <= 123) || LA17_0 == 127 || LA17_0 == 153))
                                            {
                                                alt17 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(17); }
                                        switch (alt17)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1113:4: e= expression[vars, true, null]
                                                {
                                                    DebugLocation(1113, 5);
                                                    PushFollow(Follow._expression_in_localVariableDeclaration1119);
                                                    e = expression(vars, true, null);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1114, 3);
                                                    if ((state.backtracking == 0))
                                                    {

                                                        try
                                                        {
                                                            if (ConstantDatabase.Count > 0)
                                                            {
                                                                e = e.ClearConstant(ConstantDatabase);
                                                            }
                                                            if (Spec.SpecValuation.Variables == null)
                                                            {
                                                                Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                                            }

                                                            ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);

                                                            if (rhv is IntConstant)
                                                            {
                                                                IntConstant v = rhv as IntConstant;
                                                                if (v.Value >= 0)
                                                                {
                                                                    list.Add(v.Value);
                                                                }
                                                                else
                                                                {
                                                                    throw new ParsingException("The record size must be greater than or equal to 0!", ID11.Token);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                throw new ParsingException("The record size must be an integer value!", ID11.Token);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            throw new ParsingException("Variable definition error:" + ex.Message, ID11.Token);
                                                        }

                                                    }

                                                }
                                                break;

                                            default:
                                                if (cnt17 >= 1)
                                                    goto loop17;

                                                if (state.backtracking > 0) { state.failed = true; return exp; }
                                                EarlyExitException eee17 = new EarlyExitException(17, input);
                                                DebugRecognitionException(eee17);
                                                throw eee17;
                                        }
                                        cnt17++;
                                    }
                                loop17:
                                    ;

                                }
                                finally { DebugExitSubRule(17); }

                                DebugLocation(1152, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1152:3: (rd= recordExpression[vars, null, $ID.Token] )?
                                int alt18 = 2;
                                try
                                {
                                    DebugEnterSubRule(18);
                                    try
                                    {
                                        DebugEnterDecision(18, decisionCanBacktrack[18]);
                                        int LA18_0 = input.LA(1);

                                        if ((LA18_0 == RECORD_NODE))
                                        {
                                            alt18 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(18); }
                                    switch (alt18)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1152:5: rd= recordExpression[vars, null, $ID.Token]
                                            {
                                                DebugLocation(1152, 7);
                                                PushFollow(Follow._recordExpression_in_localVariableDeclaration1139);
                                                rd = recordExpression(vars, null, ID11.Token);
                                                PopFollow();
                                                if (state.failed) return exp;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(18); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1154, 2);
                                if ((state.backtracking == 0))
                                {

                                    try
                                    {
                                        int size = 1;
                                        foreach (int i in list)
                                        {
                                            size = size * i;
                                        }
                                        if (Spec.SpecValuation.Variables == null)
                                        {
                                            Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                        }

                                        ExpressionValue record = null;

                                        if (rd != null)
                                        {
                                            int recordSize = (rd as Record).Associations.Length;
                                            if (recordSize != size)
                                            {
                                                throw new ParsingException("The declared record size " + size + " is not same as the number of given elements " + recordSize + "!", ID11.Token);
                                            }
                                            record = EvaluatorDenotational.Evaluate(rd as Record, Spec.SpecValuation);
                                        }
                                        else
                                        {
                                            record = EvaluatorDenotational.Evaluate(new Record(size), null);
                                        }

                                        //Spec.SpecValuation.ExtendDestructive(ID11.Text, record);
                                        exp = new LetDefinition(ID11.Text, record);

                                        ArrayID2DimentionMapping.Add(ID11.Text, list);

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ParsingException("Variable definition error: " + ex.Message, ID11.Token);
                                    }

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 12, localVariableDeclaration_StartIndex); }
                }
                DebugLocation(1194, 1);
            }
            finally { DebugExitRule(GrammarFileName, "localVariableDeclaration"); }
            return exp;

        }
        // $ANTLR end "localVariableDeclaration"


        protected virtual void Enter_expression() { }
        protected virtual void Leave_expression() { }

        // $ANTLR start "expression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1196:1: expression[List<string> vars, bool check, List<string> sourceVars] returns [Expression exp = null] : (e1= conditionalOrExpression[vars, check, sourceVars] | ^( ASSIGNMENT_NODE e1= conditionalOrExpression[vars, check, sourceVars] e2= expression[vars, check, sourceVars] ) );
        [GrammarRule("expression")]
        private Expression expression(List<string> vars, bool check, List<string> sourceVars)
        {

            Expression exp = null;
            int expression_StartIndex = input.Index;
            CommonTree ASSIGNMENT_NODE12 = null;
            Expression e1 = default(Expression);
            Expression e2 = default(Expression);

            paraphrases.Push("in expression");
            try
            {
                DebugEnterRule(GrammarFileName, "expression");
                DebugLocation(1196, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 13)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1199:2: (e1= conditionalOrExpression[vars, check, sourceVars] | ^( ASSIGNMENT_NODE e1= conditionalOrExpression[vars, check, sourceVars] e2= expression[vars, check, sourceVars] ) )
                    int alt20 = 2;
                    try
                    {
                        DebugEnterDecision(20, decisionCanBacktrack[20]);
                        int LA20_0 = input.LA(1);

                        if (((LA20_0 >= VAR_NODE && LA20_0 <= CALL_NODE) || LA20_0 == UNARY_NODE || (LA20_0 >= CLASS_CALL_NODE && LA20_0 <= CLASS_CALL_INSTANCE_NODE) || LA20_0 == INT || LA20_0 == 76 || (LA20_0 >= 78 && LA20_0 <= 79) || (LA20_0 >= 102 && LA20_0 <= 104) || (LA20_0 >= 110 && LA20_0 <= 123) || LA20_0 == 127 || LA20_0 == 153))
                        {
                            alt20 = 1;
                        }
                        else if ((LA20_0 == ASSIGNMENT_NODE))
                        {
                            alt20 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return exp; }
                            NoViableAltException nvae = new NoViableAltException("", 20, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(20); }
                    switch (alt20)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1199:4: e1= conditionalOrExpression[vars, check, sourceVars]
                            {
                                DebugLocation(1199, 6);
                                PushFollow(Follow._conditionalOrExpression_in_expression1180);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1199, 56);
                                if ((state.backtracking == 0))
                                {
                                    exp = e1;
                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1200:4: ^( ASSIGNMENT_NODE e1= conditionalOrExpression[vars, check, sourceVars] e2= expression[vars, check, sourceVars] )
                            {
                                DebugLocation(1200, 4);
                                DebugLocation(1200, 6);
                                ASSIGNMENT_NODE12 = (CommonTree)Match(input, ASSIGNMENT_NODE, Follow._ASSIGNMENT_NODE_in_expression1189); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1200, 24);
                                PushFollow(Follow._conditionalOrExpression_in_expression1193);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1200, 76);
                                PushFollow(Follow._expression_in_expression1198);
                                e2 = expression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1201, 7);
                                if ((state.backtracking == 0))
                                {

                                    if (e1 is Variable)
                                    {
                                        string name = e1.ExpressionID; //((Variable) e1).VarName;
                                        if (vars.Contains(name))
                                        {
                                            throw new ParsingException("CANNOT assign value to local variable " + name + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                        }
                                        else if (ConstantDatabase.ContainsKey(name))
                                        {
                                            throw new ParsingException("CANNOT assign value to constant variable " + name + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                        }
                                        /*else if(Spec.ParameterVariables.ContainsKey(name))
                                        {
                                            throw new ParsingException("Can not assign value to parameter variable " + name + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                        }*/
                                        else
                                        {
                                            exp = new Assignment(name, e2);
                                        }
                                    }
                                    else if ((e1 is PrimitiveApplication) && ((PrimitiveApplication)e1).Operator.Equals("."))
                                    {
                                        string name = ((PrimitiveApplication)e1).Argument1.ExpressionID;
                                        if (vars.Contains(name))
                                        {
                                            throw new ParsingException("CANNOT assign value to local variable " + name + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                        }
                                        else if (ConstantDatabase.ContainsKey(name))
                                        {
                                            throw new ParsingException("Constant variable " + name + " CANNOT be used here!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                        }
                                        PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e2, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(ASSIGNMENT_NODE12.Children[1] as CommonTree, input), "in array assignment", Spec.SpecValuation, ConstantDatabase);
                                        exp = new PropertyAssignment(
                                                ((PrimitiveApplication)e1).Argument1,
                                                ((PrimitiveApplication)e1).Argument2,
                                                e2);
                                    }

                                    else if (e1 is ClassProperty)
                                    {
                                        if (((ClassProperty)e1).Variable is Variable)
                                        {
                                            string name = (((ClassProperty)e1).Variable as Variable).ExpressionID;
                                            if (vars.Contains(name))
                                            {
                                                throw new ParsingException("CANNOT assign value to local variable " + name + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                            }
                                            else if (ConstantDatabase.ContainsKey(name))
                                            {
                                                throw new ParsingException("Constant variable " + name + " CANNOT be used here!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                            }
                                        }

                                        exp = new ClassPropertyAssignment((e1 as ClassProperty), e2);
                                    }
                                    else
                                    {
                                        //error state
                                        throw new ParsingException("Invalid assignment:" + e1.ToString() + "=" + e2.ToString() + "!", ((ASSIGNMENT_NODE12.Children[0] as CommonTree).Children[0] as CommonTree).Token);
                                    }

                                }

                            }
                            break;

                    }
                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 13, expression_StartIndex); }
                }
                DebugLocation(1263, 1);
            }
            finally { DebugExitRule(GrammarFileName, "expression"); }
            return exp;

        }
        // $ANTLR end "expression"


        protected virtual void Enter_letDefintion() { }
        protected virtual void Leave_letDefintion() { }

        // $ANTLR start "letDefintion"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1265:1: letDefintion : ( ^( LET_NODE (userType= ID )? varID= ID ( varaibleRange[$varID, vars, true, null] )? (e= expression[vars, true, null] | (e= recordExpression[vars, null, $varID.Token] ) )? ) | ^( LET_ARRAY_NODE ( ID ) ( varaibleRange[$ID, vars, true, null] )? (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? ) );
        [GrammarRule("letDefintion")]
        private void letDefintion()
        {

            int letDefintion_StartIndex = input.Index;
            CommonTree userType = null;
            CommonTree varID = null;
            CommonTree ID13 = null;
            Expression e = default(Expression);
            Expression rd = default(Expression);


            List<string> vars = new List<string>();
            List<int> list = new List<int>();

            try
            {
                DebugEnterRule(GrammarFileName, "letDefintion");
                DebugLocation(1265, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 14)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:2: ( ^( LET_NODE (userType= ID )? varID= ID ( varaibleRange[$varID, vars, true, null] )? (e= expression[vars, true, null] | (e= recordExpression[vars, null, $varID.Token] ) )? ) | ^( LET_ARRAY_NODE ( ID ) ( varaibleRange[$ID, vars, true, null] )? (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? ) )
                    int alt27 = 2;
                    try
                    {
                        DebugEnterDecision(27, decisionCanBacktrack[27]);
                        int LA27_0 = input.LA(1);

                        if ((LA27_0 == LET_NODE))
                        {
                            alt27 = 1;
                        }
                        else if ((LA27_0 == LET_ARRAY_NODE))
                        {
                            alt27 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return; }
                            NoViableAltException nvae = new NoViableAltException("", 27, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(27); }
                    switch (alt27)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:6: ^( LET_NODE (userType= ID )? varID= ID ( varaibleRange[$varID, vars, true, null] )? (e= expression[vars, true, null] | (e= recordExpression[vars, null, $varID.Token] ) )? )
                            {
                                DebugLocation(1270, 6);
                                DebugLocation(1270, 8);
                                Match(input, LET_NODE, Follow._LET_NODE_in_letDefintion1231); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(1270, 25);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:25: (userType= ID )?
                                int alt21 = 2;
                                try
                                {
                                    DebugEnterSubRule(21);
                                    try
                                    {
                                        DebugEnterDecision(21, decisionCanBacktrack[21]);
                                        int LA21_0 = input.LA(1);

                                        if ((LA21_0 == ID))
                                        {
                                            int LA21_1 = input.LA(2);

                                            if ((LA21_1 == ID))
                                            {
                                                alt21 = 1;
                                            }
                                        }
                                    }
                                    finally { DebugExitDecision(21); }
                                    switch (alt21)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: userType= ID
                                            {
                                                DebugLocation(1270, 25);
                                                userType = (CommonTree)Match(input, ID, Follow._ID_in_letDefintion1235); if (state.failed) return;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(21); }

                                DebugLocation(1270, 35);
                                varID = (CommonTree)Match(input, ID, Follow._ID_in_letDefintion1240); if (state.failed) return;
                                DebugLocation(1270, 39);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:39: ( varaibleRange[$varID, vars, true, null] )?
                                int alt22 = 2;
                                try
                                {
                                    DebugEnterSubRule(22);
                                    try
                                    {
                                        DebugEnterDecision(22, decisionCanBacktrack[22]);
                                        int LA22_0 = input.LA(1);

                                        if ((LA22_0 == VARIABLE_RANGE_NODE))
                                        {
                                            alt22 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(22); }
                                    switch (alt22)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: varaibleRange[$varID, vars, true, null]
                                            {
                                                DebugLocation(1270, 39);
                                                PushFollow(Follow._varaibleRange_in_letDefintion1242);
                                                varaibleRange(varID, vars, true, null);
                                                PopFollow();
                                                if (state.failed) return;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(22); }

                                DebugLocation(1270, 80);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:80: (e= expression[vars, true, null] | (e= recordExpression[vars, null, $varID.Token] ) )?
                                int alt23 = 3;
                                try
                                {
                                    DebugEnterSubRule(23);
                                    try
                                    {
                                        DebugEnterDecision(23, decisionCanBacktrack[23]);
                                        int LA23_0 = input.LA(1);

                                        if (((LA23_0 >= VAR_NODE && LA23_0 <= CALL_NODE) || LA23_0 == ASSIGNMENT_NODE || LA23_0 == UNARY_NODE || (LA23_0 >= CLASS_CALL_NODE && LA23_0 <= CLASS_CALL_INSTANCE_NODE) || LA23_0 == INT || LA23_0 == 76 || (LA23_0 >= 78 && LA23_0 <= 79) || (LA23_0 >= 102 && LA23_0 <= 104) || (LA23_0 >= 110 && LA23_0 <= 123) || LA23_0 == 127 || LA23_0 == 153))
                                        {
                                            alt23 = 1;
                                        }
                                        else if ((LA23_0 == RECORD_NODE))
                                        {
                                            alt23 = 2;
                                        }
                                    }
                                    finally { DebugExitDecision(23); }
                                    switch (alt23)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:81: e= expression[vars, true, null]
                                            {
                                                DebugLocation(1270, 82);
                                                PushFollow(Follow._expression_in_letDefintion1249);
                                                e = expression(vars, true, null);
                                                PopFollow();
                                                if (state.failed) return;

                                            }
                                            break;
                                        case 2:
                                            DebugEnterAlt(2);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:114: (e= recordExpression[vars, null, $varID.Token] )
                                            {
                                                DebugLocation(1270, 114);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:114: (e= recordExpression[vars, null, $varID.Token] )
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1270:116: e= recordExpression[vars, null, $varID.Token]
                                                {
                                                    DebugLocation(1270, 117);
                                                    PushFollow(Follow._recordExpression_in_letDefintion1258);
                                                    e = recordExpression(vars, null, varID.Token);
                                                    PopFollow();
                                                    if (state.failed) return;
                                                    DebugLocation(1270, 161);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        Record r = e as Record; list.Add(r.Associations.Length); ArrayID2DimentionMapping.Add(varID.Text, list);
                                                    }

                                                }


                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(23); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return;
                                DebugLocation(1271, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckDuplicatedDeclaration(varID.Token);

                                    if (userType != null)
                                    {
                                        if (e != null)
                                        {
                                            if (e is NewObjectCreation)
                                            {
                                                if ((e as NewObjectCreation).ClassName != userType.Text)
                                                {
                                                    throw new ParsingException("Class name " + (e as NewObjectCreation).ClassName + " does not match variable type " + userType.Text + ".", varID.Token);
                                                }
                                            }
                                            else
                                            {
                                                throw new ParsingException("User defined data type can only be initialized using new keyword.", varID.Token);
                                            }
                                        }
                                        else
                                        {
                                            e = PAT.Common.Ultility.Ultility.InitializeUserDefinedDataType(userType.Text);
                                        }

                                        if (e == null)
                                        {
                                            throw new ParsingException("Can not find the user defined data type. Please make sure you have imported it.", userType.Token);
                                        }
                                    }

                                    if (e == null)
                                    {
                                        e = new IntConstant(0);
                                    }

                                    try
                                    {
                                        if (ConstantDatabase.Count > 0)
                                        {
                                            e = e.ClearConstant(ConstantDatabase);
                                        }
                                        if (Spec.SpecValuation.Variables == null)
                                        {
                                            Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                        }
                                        ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);
                                        Spec.SpecValuation.ExtendDestructive(varID.Text, rhv);
                                        Spec.DeclaritionTable.Add(varID.Text, new Declaration(DeclarationType.Variable, new ParsingException(varID.Text, varID.Token)));
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ParsingException("Variable definition error: " + ex.Message, varID.Token);
                                    }

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1325:5: ^( LET_ARRAY_NODE ( ID ) ( varaibleRange[$ID, vars, true, null] )? (e= expression[vars, true, null] )+ (rd= recordExpression[vars, null, $ID.Token] )? )
                            {
                                DebugLocation(1325, 5);
                                DebugLocation(1325, 7);
                                Match(input, LET_ARRAY_NODE, Follow._LET_ARRAY_NODE_in_letDefintion1278); if (state.failed) return;

                                Match(input, TokenTypes.Down, null); if (state.failed) return;
                                DebugLocation(1325, 22);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1325:22: ( ID )
                                DebugEnterAlt(1);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1325:23: ID
                                {
                                    DebugLocation(1325, 23);
                                    ID13 = (CommonTree)Match(input, ID, Follow._ID_in_letDefintion1281); if (state.failed) return;
                                    DebugLocation(1325, 26);
                                    if ((state.backtracking == 0))
                                    {
                                        CheckDuplicatedDeclaration(ID13.Token);
                                    }

                                }

                                DebugLocation(1325, 68);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1325:68: ( varaibleRange[$ID, vars, true, null] )?
                                int alt24 = 2;
                                try
                                {
                                    DebugEnterSubRule(24);
                                    try
                                    {
                                        DebugEnterDecision(24, decisionCanBacktrack[24]);
                                        int LA24_0 = input.LA(1);

                                        if ((LA24_0 == VARIABLE_RANGE_NODE))
                                        {
                                            alt24 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(24); }
                                    switch (alt24)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: varaibleRange[$ID, vars, true, null]
                                            {
                                                DebugLocation(1325, 68);
                                                PushFollow(Follow._varaibleRange_in_letDefintion1286);
                                                varaibleRange(ID13, vars, true, null);
                                                PopFollow();
                                                if (state.failed) return;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(24); }

                                DebugLocation(1327, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1327:3: (e= expression[vars, true, null] )+
                                int cnt25 = 0;
                                try
                                {
                                    DebugEnterSubRule(25);
                                    while (true)
                                    {
                                        int alt25 = 2;
                                        try
                                        {
                                            DebugEnterDecision(25, decisionCanBacktrack[25]);
                                            int LA25_0 = input.LA(1);

                                            if (((LA25_0 >= VAR_NODE && LA25_0 <= CALL_NODE) || LA25_0 == ASSIGNMENT_NODE || LA25_0 == UNARY_NODE || (LA25_0 >= CLASS_CALL_NODE && LA25_0 <= CLASS_CALL_INSTANCE_NODE) || LA25_0 == INT || LA25_0 == 76 || (LA25_0 >= 78 && LA25_0 <= 79) || (LA25_0 >= 102 && LA25_0 <= 104) || (LA25_0 >= 110 && LA25_0 <= 123) || LA25_0 == 127 || LA25_0 == 153))
                                            {
                                                alt25 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(25); }
                                        switch (alt25)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1327:4: e= expression[vars, true, null]
                                                {
                                                    DebugLocation(1327, 5);
                                                    PushFollow(Follow._expression_in_letDefintion1298);
                                                    e = expression(vars, true, null);
                                                    PopFollow();
                                                    if (state.failed) return;
                                                    DebugLocation(1328, 4);
                                                    if ((state.backtracking == 0))
                                                    {

                                                        try
                                                        {
                                                            if (ConstantDatabase.Count > 0)
                                                            {
                                                                e = e.ClearConstant(ConstantDatabase);
                                                            }
                                                            if (Spec.SpecValuation.Variables == null)
                                                            {
                                                                Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                                            }

                                                            ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);

                                                            if (rhv is IntConstant)
                                                            {
                                                                IntConstant v = rhv as IntConstant;
                                                                if (v.Value >= 0)
                                                                {
                                                                    list.Add(v.Value);
                                                                }
                                                                else
                                                                {
                                                                    throw new ParsingException("The record size must be greater than or equal to 0!", ID13.Token);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                throw new ParsingException("The record size must be an integer value!", ID13.Token);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            throw new ParsingException("Variable definition error:" + ex.Message, ID13.Token);
                                                        }

                                                    }

                                                }
                                                break;

                                            default:
                                                if (cnt25 >= 1)
                                                    goto loop25;

                                                if (state.backtracking > 0) { state.failed = true; return; }
                                                EarlyExitException eee25 = new EarlyExitException(25, input);
                                                DebugRecognitionException(eee25);
                                                throw eee25;
                                        }
                                        cnt25++;
                                    }
                                loop25:
                                    ;

                                }
                                finally { DebugExitSubRule(25); }

                                DebugLocation(1366, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1366:3: (rd= recordExpression[vars, null, $ID.Token] )?
                                int alt26 = 2;
                                try
                                {
                                    DebugEnterSubRule(26);
                                    try
                                    {
                                        DebugEnterDecision(26, decisionCanBacktrack[26]);
                                        int LA26_0 = input.LA(1);

                                        if ((LA26_0 == RECORD_NODE))
                                        {
                                            alt26 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(26); }
                                    switch (alt26)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1366:5: rd= recordExpression[vars, null, $ID.Token]
                                            {
                                                DebugLocation(1366, 7);
                                                PushFollow(Follow._recordExpression_in_letDefintion1319);
                                                rd = recordExpression(vars, null, ID13.Token);
                                                PopFollow();
                                                if (state.failed) return;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(26); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return;
                                DebugLocation(1368, 2);
                                if ((state.backtracking == 0))
                                {

                                    try
                                    {
                                        int size = 1;
                                        foreach (int i in list)
                                        {
                                            size = size * i;
                                        }
                                        if (Spec.SpecValuation.Variables == null)
                                        {
                                            Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                        }

                                        ExpressionValue record = null;

                                        if (rd != null)
                                        {
                                            int recordSize = (rd as Record).Associations.Length;
                                            if (recordSize != size)
                                            {
                                                throw new ParsingException("The declared record size " + size + " is not same as the number of given elements " + recordSize + "!", ID13.Token);
                                            }
                                            record = EvaluatorDenotational.Evaluate(rd as Record, Spec.SpecValuation);
                                        }
                                        else
                                        {
                                            record = EvaluatorDenotational.Evaluate(new Record(size), null);
                                        }

                                        Spec.SpecValuation.ExtendDestructive(ID13.Text, record);
                                        Spec.DeclaritionTable.Add(ID13.Text, new Declaration(DeclarationType.Variable, new ParsingException(ID13.Text, ID13.Token)));
                                        ArrayID2DimentionMapping.Add(ID13.Text, list);

                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ParsingException("Variable definition error: " + ex.Message, ID13.Token);
                                    }



                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 14, letDefintion_StartIndex); }
                }
                DebugLocation(1409, 1);
            }
            finally { DebugExitRule(GrammarFileName, "letDefintion"); }
            return;

        }
        // $ANTLR end "letDefintion"


        protected virtual void Enter_varaibleRange() { }
        protected virtual void Leave_varaibleRange() { }

        // $ANTLR start "varaibleRange"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1411:1: varaibleRange[CommonTree ID, List<string> vars, bool check, List<string> sourceVars] : ^( VARIABLE_RANGE_NODE (lower= conditionalOrExpression[vars, check, sourceVars] )? '.' (upper= conditionalOrExpression[vars, check, sourceVars] )? ) ;
        [GrammarRule("varaibleRange")]
        private void varaibleRange(CommonTree ID, List<string> vars, bool check, List<string> sourceVars)
        {

            int varaibleRange_StartIndex = input.Index;
            Expression lower = default(Expression);
            Expression upper = default(Expression);

            try
            {
                DebugEnterRule(GrammarFileName, "varaibleRange");
                DebugLocation(1411, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 15)) { return; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:2: ( ^( VARIABLE_RANGE_NODE (lower= conditionalOrExpression[vars, check, sourceVars] )? '.' (upper= conditionalOrExpression[vars, check, sourceVars] )? ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:5: ^( VARIABLE_RANGE_NODE (lower= conditionalOrExpression[vars, check, sourceVars] )? '.' (upper= conditionalOrExpression[vars, check, sourceVars] )? )
                    {
                        DebugLocation(1412, 5);
                        DebugLocation(1412, 7);
                        Match(input, VARIABLE_RANGE_NODE, Follow._VARIABLE_RANGE_NODE_in_varaibleRange1347); if (state.failed) return;

                        Match(input, TokenTypes.Down, null); if (state.failed) return;
                        DebugLocation(1412, 28);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:28: (lower= conditionalOrExpression[vars, check, sourceVars] )?
                        int alt28 = 2;
                        try
                        {
                            DebugEnterSubRule(28);
                            try
                            {
                                DebugEnterDecision(28, decisionCanBacktrack[28]);
                                int LA28_0 = input.LA(1);

                                if (((LA28_0 >= VAR_NODE && LA28_0 <= CALL_NODE) || LA28_0 == UNARY_NODE || (LA28_0 >= CLASS_CALL_NODE && LA28_0 <= CLASS_CALL_INSTANCE_NODE) || LA28_0 == INT || LA28_0 == 76 || (LA28_0 >= 78 && LA28_0 <= 79) || (LA28_0 >= 102 && LA28_0 <= 104) || (LA28_0 >= 110 && LA28_0 <= 123) || LA28_0 == 127 || LA28_0 == 153))
                                {
                                    alt28 = 1;
                                }
                            }
                            finally { DebugExitDecision(28); }
                            switch (alt28)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:29: lower= conditionalOrExpression[vars, check, sourceVars]
                                    {
                                        DebugLocation(1412, 34);
                                        PushFollow(Follow._conditionalOrExpression_in_varaibleRange1353);
                                        lower = conditionalOrExpression(vars, check, sourceVars);
                                        PopFollow();
                                        if (state.failed) return;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(28); }

                        DebugLocation(1412, 86);
                        Match(input, 85, Follow._85_in_varaibleRange1358); if (state.failed) return;
                        DebugLocation(1412, 90);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:90: (upper= conditionalOrExpression[vars, check, sourceVars] )?
                        int alt29 = 2;
                        try
                        {
                            DebugEnterSubRule(29);
                            try
                            {
                                DebugEnterDecision(29, decisionCanBacktrack[29]);
                                int LA29_0 = input.LA(1);

                                if (((LA29_0 >= VAR_NODE && LA29_0 <= CALL_NODE) || LA29_0 == UNARY_NODE || (LA29_0 >= CLASS_CALL_NODE && LA29_0 <= CLASS_CALL_INSTANCE_NODE) || LA29_0 == INT || LA29_0 == 76 || (LA29_0 >= 78 && LA29_0 <= 79) || (LA29_0 >= 102 && LA29_0 <= 104) || (LA29_0 >= 110 && LA29_0 <= 123) || LA29_0 == 127 || LA29_0 == 153))
                                {
                                    alt29 = 1;
                                }
                            }
                            finally { DebugExitDecision(29); }
                            switch (alt29)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1412:91: upper= conditionalOrExpression[vars, check, sourceVars]
                                    {
                                        DebugLocation(1412, 96);
                                        PushFollow(Follow._conditionalOrExpression_in_varaibleRange1363);
                                        upper = conditionalOrExpression(vars, check, sourceVars);
                                        PopFollow();
                                        if (state.failed) return;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(29); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return;
                        DebugLocation(1413, 2);
                        if ((state.backtracking == 0))
                        {

                            int lowerV = 0;
                            int upperV = 0;
                            if (lower != null)
                            {
                                try
                                {
                                    if (ConstantDatabase.Count > 0)
                                    {
                                        lower = lower.ClearConstant(ConstantDatabase);
                                    }
                                    if (Spec.SpecValuation.Variables == null)
                                    {
                                        Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                    }
                                    ExpressionValue rhv = EvaluatorDenotational.Evaluate(lower, Spec.SpecValuation);
                                    if (rhv is IntConstant)
                                    {
                                        IntConstant v = rhv as IntConstant;
                                        lowerV = v.Value;
                                        if (Valuation.VariableLowerBound == null)
                                        {
                                            Valuation.VariableLowerBound = new StringDictionary<int>(16);
                                        }
                                        Valuation.VariableLowerBound.Add(ID.Text, lowerV);
                                    }
                                    else
                                    {
                                        throw new ParsingException("The lower bound must be an integer value!", ID.Token);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new ParsingException("Variable lower bound error:" + ex.Message, ID.Token);
                                }

                            }

                            if (upper != null)
                            {
                                try
                                {
                                    if (ConstantDatabase.Count > 0)
                                    {
                                        upper = upper.ClearConstant(ConstantDatabase);
                                    }
                                    if (Spec.SpecValuation.Variables == null)
                                    {
                                        Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();
                                    }
                                    ExpressionValue rhv = EvaluatorDenotational.Evaluate(upper, Spec.SpecValuation);
                                    if (rhv is IntConstant)
                                    {
                                        IntConstant v = rhv as IntConstant;
                                        upperV = v.Value;
                                        Valuation.VariableUpperLowerBound.Add(ID.Text, upperV);
                                    }
                                    else
                                    {
                                        throw new ParsingException("The upper bound must be an integer value!", ID.Token);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new ParsingException("Variable upper bound error:" + ex.Message, ID.Token);
                                }
                            }

                            if (lower != null && upper != null && lowerV > upperV)
                            {
                                throw new ParsingException("Variable's upper bound must be greater than lower bound:", ID.Token);
                            }

                        }

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 15, varaibleRange_StartIndex); }
                }
                DebugLocation(1486, 1);
            }
            finally { DebugExitRule(GrammarFileName, "varaibleRange"); }
            return;

        }
        // $ANTLR end "varaibleRange"


        protected virtual void Enter_conditionalOrExpression() { }
        protected virtual void Leave_conditionalOrExpression() { }

        // $ANTLR start "conditionalOrExpression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1488:1: conditionalOrExpression[List<string> vars, bool check, List<string> sourceVars] returns [Expression exp = null] : ( ^( '||' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'xor' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '|' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '^' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '==' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '!=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '+' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '-' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '*' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '/' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '%' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( UNARY_NODE e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( VAR_NODE ID ) | a1= arrayExpression[vars, check, sourceVars, varName] | INT | 'true' | 'false' | ^( '!' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'empty' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( CALL_NODE ( ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_NODE (var= ID ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_INSTANCE_NODE (a1= arrayExpression[vars, check, sourceVars,varName] ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( 'new' (className= ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) );
        [GrammarRule("conditionalOrExpression")]
        private Expression conditionalOrExpression(List<string> vars, bool check, List<string> sourceVars)
        {

            Expression exp = null;
            int conditionalOrExpression_StartIndex = input.Index;
            CommonTree opt = null;
            CommonTree var = null;
            CommonTree method = null;
            CommonTree className = null;
            CommonTree UNARY_NODE14 = null;
            CommonTree ID15 = null;
            CommonTree INT16 = null;
            CommonTree ID17 = null;
            Expression e1 = default(Expression);
            Expression e2 = default(Expression);
            Expression a1 = default(Expression);

            List<int> list = null; List<Expression> indices = null; List<CommonTree> varName = null;
            try
            {
                DebugEnterRule(GrammarFileName, "conditionalOrExpression");
                DebugLocation(1488, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 16)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1490:2: ( ^( '||' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'xor' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '|' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '^' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '==' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '!=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '+' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '-' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '*' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '/' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '%' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( UNARY_NODE e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( VAR_NODE ID ) | a1= arrayExpression[vars, check, sourceVars, varName] | INT | 'true' | 'false' | ^( '!' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'empty' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( CALL_NODE ( ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_NODE (var= ID ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_INSTANCE_NODE (a1= arrayExpression[vars, check, sourceVars,varName] ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( 'new' (className= ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) )
                    int alt34 = 29;
                    try
                    {
                        DebugEnterDecision(34, decisionCanBacktrack[34]);
                        try
                        {
                            alt34 = dfa34.Predict(input);
                        }
                        catch (NoViableAltException nvae)
                        {
                            DebugRecognitionException(nvae);
                            throw;
                        }
                    }
                    finally { DebugExitDecision(34); }
                    switch (alt34)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1490:4: ^( '||' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1490, 4);
                                DebugLocation(1490, 6);
                                Match(input, 79, Follow._79_in_conditionalOrExpression1394); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1490, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1398);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1490, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1403);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1490, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("||", e1, e2);
                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1491:4: ^( '&&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1491, 4);
                                DebugLocation(1491, 6);
                                Match(input, 78, Follow._78_in_conditionalOrExpression1413); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1491, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1417);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1491, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1422);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1491, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("&&", e1, e2);
                                }

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1492:4: ^( 'xor' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1492, 4);
                                DebugLocation(1492, 6);
                                Match(input, 110, Follow._110_in_conditionalOrExpression1432); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1492, 14);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1436);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1492, 66);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1441);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1492, 117);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("xor", e1, e2);
                                }

                            }
                            break;
                        case 4:
                            DebugEnterAlt(4);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1493:4: ^( '|' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1493, 4);
                                DebugLocation(1493, 6);
                                Match(input, 112, Follow._112_in_conditionalOrExpression1451); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1493, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1455);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1493, 64);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1460);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1493, 115);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("|", e1, e2);
                                }

                            }
                            break;
                        case 5:
                            DebugEnterAlt(5);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1494:4: ^( '&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1494, 4);
                                DebugLocation(1494, 6);
                                Match(input, 111, Follow._111_in_conditionalOrExpression1470); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1494, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1474);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1494, 64);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1479);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1494, 115);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("&", e1, e2);
                                }

                            }
                            break;
                        case 6:
                            DebugEnterAlt(6);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1495:4: ^( '^' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1495, 4);
                                DebugLocation(1495, 6);
                                Match(input, 113, Follow._113_in_conditionalOrExpression1489); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1495, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1493);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1495, 64);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1498);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1495, 115);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("^", e1, e2);
                                }

                            }
                            break;
                        case 7:
                            DebugEnterAlt(7);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1496:4: ^( '==' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1496, 4);
                                DebugLocation(1496, 6);
                                Match(input, 114, Follow._114_in_conditionalOrExpression1508); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1496, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1512);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1496, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1517);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1496, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("==", e1, e2);
                                }

                            }
                            break;
                        case 8:
                            DebugEnterAlt(8);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1497:4: ^( '!=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1497, 4);
                                DebugLocation(1497, 6);
                                Match(input, 115, Follow._115_in_conditionalOrExpression1528); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1497, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1532);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1497, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1537);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1497, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("!=", e1, e2);
                                }

                            }
                            break;
                        case 9:
                            DebugEnterAlt(9);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1498:4: ^( '<' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1498, 4);
                                DebugLocation(1498, 6);
                                Match(input, 116, Follow._116_in_conditionalOrExpression1550); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1498, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1554);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1498, 64);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1559);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1498, 115);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("<", e1, e2);
                                }

                            }
                            break;
                        case 10:
                            DebugEnterAlt(10);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1499:4: ^( '>' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1499, 4);
                                DebugLocation(1499, 6);
                                Match(input, 117, Follow._117_in_conditionalOrExpression1573); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1499, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1577);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1499, 64);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1582);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1499, 115);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication(">", e1, e2);
                                }

                            }
                            break;
                        case 11:
                            DebugEnterAlt(11);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1500:4: ^( '<=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1500, 4);
                                DebugLocation(1500, 6);
                                Match(input, 118, Follow._118_in_conditionalOrExpression1597); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1500, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1601);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1500, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1606);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1500, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("<=", e1, e2);
                                }

                            }
                            break;
                        case 12:
                            DebugEnterAlt(12);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1501:4: ^( '>=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1501, 4);
                                DebugLocation(1501, 6);
                                Match(input, 119, Follow._119_in_conditionalOrExpression1620); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1501, 13);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1624);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1501, 65);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1629);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1501, 116);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication(">=", e1, e2);
                                }

                            }
                            break;
                        case 13:
                            DebugEnterAlt(13);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1502:4: ^(opt= '+' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1502, 4);
                                DebugLocation(1502, 9);
                                opt = (CommonTree)Match(input, 120, Follow._120_in_conditionalOrExpression1647); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1502, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1651);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1502, 68);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1656);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1503, 2);
                                if ((state.backtracking == 0))
                                {


                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[0] as CommonTree, input));
                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[1] as CommonTree, input));
                                    exp = new PrimitiveApplication("+", e1, e2);

                                }

                            }
                            break;
                        case 14:
                            DebugEnterAlt(14);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1509:4: ^(opt= '-' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1509, 4);
                                DebugLocation(1509, 9);
                                opt = (CommonTree)Match(input, 102, Follow._102_in_conditionalOrExpression1674); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1509, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1678);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1509, 68);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1683);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1510, 2);
                                if ((state.backtracking == 0))
                                {

                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[0] as CommonTree, input));
                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[1] as CommonTree, input));
                                    exp = new PrimitiveApplication("-", e1, e2);

                                }

                            }
                            break;
                        case 15:
                            DebugEnterAlt(15);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1515:4: ^(opt= '*' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1515, 4);
                                DebugLocation(1515, 9);
                                opt = (CommonTree)Match(input, 121, Follow._121_in_conditionalOrExpression1703); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1515, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1707);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1515, 68);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1712);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1516, 2);
                                if ((state.backtracking == 0))
                                {

                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[0] as CommonTree, input));
                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[1] as CommonTree, input));	
                                    exp = new PrimitiveApplication("*", e1, e2);

                                }

                            }
                            break;
                        case 16:
                            DebugEnterAlt(16);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1521:4: ^(opt= '/' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1521, 4);
                                DebugLocation(1521, 9);
                                opt = (CommonTree)Match(input, 122, Follow._122_in_conditionalOrExpression1730); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1521, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1734);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1521, 68);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1739);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1522, 2);
                                if ((state.backtracking == 0))
                                {

                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[0] as CommonTree, input));
                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[1] as CommonTree, input));	
                                    exp = new PrimitiveApplication("/", e1, e2);

                                }

                            }
                            break;
                        case 17:
                            DebugEnterAlt(17);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1527:4: ^(opt= '%' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1527, 4);
                                DebugLocation(1527, 9);
                                opt = (CommonTree)Match(input, 123, Follow._123_in_conditionalOrExpression1760); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1527, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1764);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1527, 68);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1769);
                                e2 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1528, 2);
                                if ((state.backtracking == 0))
                                {

                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[0] as CommonTree, input));
                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(opt.Children[1] as CommonTree, input));	
                                    exp = new PrimitiveApplication("mod", e1, e2);

                                }

                            }
                            break;
                        case 18:
                            DebugEnterAlt(18);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1533:4: ^( UNARY_NODE e1= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1533, 4);
                                DebugLocation(1533, 6);
                                UNARY_NODE14 = (CommonTree)Match(input, UNARY_NODE, Follow._UNARY_NODE_in_conditionalOrExpression1789); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1533, 19);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1793);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1534, 2);
                                if ((state.backtracking == 0))
                                {

                                    //PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken(UNARY_NODE14.Children[0] as CommonTree, input));
                                    exp = new PrimitiveApplication("~", e1);

                                }

                            }
                            break;
                        case 19:
                            DebugEnterAlt(19);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1538:4: ^( VAR_NODE ID )
                            {
                                DebugLocation(1538, 4);
                                DebugLocation(1538, 6);
                                Match(input, VAR_NODE, Follow._VAR_NODE_in_conditionalOrExpression1805); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1538, 15);
                                ID15 = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1807); if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1539, 2);
                                if ((state.backtracking == 0))
                                {


                                    //if the variable must be one inside of the source varaibles, we do a check here
                                    if (sourceVars != null && sourceVars.Count > 0)
                                    {

                                        bool hasMatch = false;
                                        foreach (IToken name in GlobalConstNames)
                                        {
                                            if (name.Text == ID15.Text)
                                            {
                                                hasMatch = true;
                                            }
                                        }


                                        //if the variable is not a constant
                                        if (!hasMatch)
                                        {
                                            //and it is not inside the sourceVars list
                                            if (!sourceVars.Contains(ID15.Text))
                                            {
                                                throw new ParsingException("Variable " + ID15.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) + "}", ID15.Token);
                                            }
                                        }
                                        exp = new Variable(ID15.Text);

                                    }
                                    else
                                    {
                                        if (Specification.CheckVariableDeclare && check)
                                        {
                                            exp = CheckVariableNotDeclared(vars, ID15.Token);
                                        }
                                        //if there is no declaration, then this is a variable expression, we need to 
                                        if (exp == null)
                                        {
                                            exp = new Variable(ID15.Text);
                                        }
                                    }



                                }

                            }
                            break;
                        case 20:
                            DebugEnterAlt(20);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1582:4: a1= arrayExpression[vars, check, sourceVars, varName]
                            {
                                DebugLocation(1582, 6);
                                PushFollow(Follow._arrayExpression_in_conditionalOrExpression1818);
                                a1 = arrayExpression(vars, check, sourceVars, varName);
                                PopFollow();
                                if (state.failed) return exp;
                                DebugLocation(1582, 56);
                                if ((state.backtracking == 0))
                                {
                                    exp = a1;
                                }

                            }
                            break;
                        case 21:
                            DebugEnterAlt(21);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1583:4: INT
                            {
                                DebugLocation(1583, 4);
                                INT16 = (CommonTree)Match(input, INT, Follow._INT_in_conditionalOrExpression1827); if (state.failed) return exp;
                                DebugLocation(1583, 8);
                                if ((state.backtracking == 0))
                                {
                                    exp = new IntConstant(int.Parse(INT16.Text));
                                }

                            }
                            break;
                        case 22:
                            DebugEnterAlt(22);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1584:4: 'true'
                            {
                                DebugLocation(1584, 4);
                                Match(input, 103, Follow._103_in_conditionalOrExpression1835); if (state.failed) return exp;
                                DebugLocation(1584, 11);
                                if ((state.backtracking == 0))
                                {
                                    exp = new BoolConstant(true);
                                }

                            }
                            break;
                        case 23:
                            DebugEnterAlt(23);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1585:4: 'false'
                            {
                                DebugLocation(1585, 4);
                                Match(input, 104, Follow._104_in_conditionalOrExpression1843); if (state.failed) return exp;
                                DebugLocation(1585, 12);
                                if ((state.backtracking == 0))
                                {
                                    exp = new BoolConstant(false);
                                }

                            }
                            break;
                        case 24:
                            DebugEnterAlt(24);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1586:4: ^( '!' e1= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1586, 4);
                                DebugLocation(1586, 6);
                                Match(input, 76, Follow._76_in_conditionalOrExpression1852); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1586, 12);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1856);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1586, 63);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("!", e1);
                                }

                            }
                            break;
                        case 25:
                            DebugEnterAlt(25);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1587:4: ^( 'empty' e1= conditionalOrExpression[vars, check, sourceVars] )
                            {
                                DebugLocation(1587, 4);
                                DebugLocation(1587, 6);
                                Match(input, 153, Follow._153_in_conditionalOrExpression1866); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1587, 16);
                                PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1870);
                                e1 = conditionalOrExpression(vars, check, sourceVars);
                                PopFollow();
                                if (state.failed) return exp;

                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1587, 67);
                                if ((state.backtracking == 0))
                                {
                                    exp = new PrimitiveApplication("empty", e1);
                                }

                            }
                            break;
                        case 26:
                            DebugEnterAlt(26);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1588:4: ^( CALL_NODE ( ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* )
                            {
                                DebugLocation(1588, 4);
                                DebugLocation(1588, 6);
                                Match(input, CALL_NODE, Follow._CALL_NODE_in_conditionalOrExpression1880); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1588, 16);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1588:16: ( ID )
                                DebugEnterAlt(1);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1588:17: ID
                                {
                                    DebugLocation(1588, 17);
                                    ID17 = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1883); if (state.failed) return exp;
                                    DebugLocation(1588, 20);
                                    if ((state.backtracking == 0))
                                    {
                                        indices = new List<Expression>(); sourceVars = PAT.Common.Ultility.ParsingUltility.CheckWhetherIsChannelCall(ID17.Token, sourceVars, Spec.ChannelDatabase);
                                    }

                                }

                                DebugLocation(1588, 179);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1588:179: (e1= conditionalOrExpression[vars, check, sourceVars] )*
                                try
                                {
                                    DebugEnterSubRule(30);
                                    while (true)
                                    {
                                        int alt30 = 2;
                                        try
                                        {
                                            DebugEnterDecision(30, decisionCanBacktrack[30]);
                                            int LA30_0 = input.LA(1);

                                            if (((LA30_0 >= VAR_NODE && LA30_0 <= CALL_NODE) || LA30_0 == UNARY_NODE || (LA30_0 >= CLASS_CALL_NODE && LA30_0 <= CLASS_CALL_INSTANCE_NODE) || LA30_0 == INT || LA30_0 == 76 || (LA30_0 >= 78 && LA30_0 <= 79) || (LA30_0 >= 102 && LA30_0 <= 104) || (LA30_0 >= 110 && LA30_0 <= 123) || LA30_0 == 127 || LA30_0 == 153))
                                            {
                                                alt30 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(30); }
                                        switch (alt30)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1588:180: e1= conditionalOrExpression[vars, check, sourceVars]
                                                {
                                                    DebugLocation(1588, 182);
                                                    PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1891);
                                                    e1 = conditionalOrExpression(vars, check, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1588, 232);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        indices.Add(e1);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop30;
                                        }
                                    }

                                loop30:
                                    ;

                                }
                                finally { DebugExitSubRule(30); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1589, 2);
                                if ((state.backtracking == 0))
                                {


                                    exp = new StaticMethodCall(ID17.Text, indices.ToArray());
                                    exp = PAT.Common.Ultility.ParsingUltility.TestMethod((exp as StaticMethodCall), ID17.Token, Spec.ChannelDatabase, ConstantDatabase, Spec);

                                    if (sourceVars != null)
                                    {
                                        foreach (string key in Spec.ChannelDatabase.Keys)
                                        {
                                            sourceVars.Remove(key);
                                        }
                                    }

                                }

                            }
                            break;
                        case 27:
                            DebugEnterAlt(27);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1602:4: ^( CLASS_CALL_NODE (var= ID ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* )
                            {
                                DebugLocation(1602, 4);
                                DebugLocation(1602, 6);
                                Match(input, CLASS_CALL_NODE, Follow._CLASS_CALL_NODE_in_conditionalOrExpression1908); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1602, 22);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1602:22: (var= ID )
                                DebugEnterAlt(1);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1602:23: var= ID
                                {
                                    DebugLocation(1602, 26);
                                    var = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1913); if (state.failed) return exp;
                                    DebugLocation(1602, 30);
                                    if ((state.backtracking == 0))
                                    {
                                        indices = new List<Expression>();
                                    }

                                }

                                DebugLocation(1602, 74);
                                method = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1920); if (state.failed) return exp;
                                DebugLocation(1602, 78);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1602:78: (e1= conditionalOrExpression[vars, check, sourceVars] )*
                                try
                                {
                                    DebugEnterSubRule(31);
                                    while (true)
                                    {
                                        int alt31 = 2;
                                        try
                                        {
                                            DebugEnterDecision(31, decisionCanBacktrack[31]);
                                            int LA31_0 = input.LA(1);

                                            if (((LA31_0 >= VAR_NODE && LA31_0 <= CALL_NODE) || LA31_0 == UNARY_NODE || (LA31_0 >= CLASS_CALL_NODE && LA31_0 <= CLASS_CALL_INSTANCE_NODE) || LA31_0 == INT || LA31_0 == 76 || (LA31_0 >= 78 && LA31_0 <= 79) || (LA31_0 >= 102 && LA31_0 <= 104) || (LA31_0 >= 110 && LA31_0 <= 123) || LA31_0 == 127 || LA31_0 == 153))
                                            {
                                                alt31 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(31); }
                                        switch (alt31)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1602:79: e1= conditionalOrExpression[vars, check, sourceVars]
                                                {
                                                    DebugLocation(1602, 81);
                                                    PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1925);
                                                    e1 = conditionalOrExpression(vars, check, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1602, 131);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        indices.Add(e1);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop31;
                                        }
                                    }

                                loop31:
                                    ;

                                }
                                finally { DebugExitSubRule(31); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1603, 2);
                                if ((state.backtracking == 0))
                                {

                                    if (sourceVars != null && sourceVars.Count > 0)
                                    {
                                        if (!sourceVars.Contains(var.Text))
                                        {
                                            throw new ParsingException("Variable " + var.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) + "}", var.Token);
                                        }
                                    }
                                    else if (check)
                                    {
                                        exp = CheckVariableNotDeclared(vars, var.Token);
                                        if (Spec.SpecValuation.Variables != null && Spec.SpecValuation.Variables.ContainsKey(var.Text))
                                        {
                                            PAT.Common.Ultility.ParsingUltility.TestMethodDefined(method.Token, indices.Count, Spec.SpecValuation.Variables[var.Text]);
                                        }
                                        else
                                        {
                                            Spec.AddNewWarning("When using user defined data structures as parameter variables, you have to make sure that its method call has no side effects, otherwise the verification result can not be guaranteed.", var.Token);
                                        }
                                    }
                                    exp = new ClassMethodCall(var.Text, method.Text, indices.ToArray());

                                }

                            }
                            break;
                        case 28:
                            DebugEnterAlt(28);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1625:4: ^( CLASS_CALL_INSTANCE_NODE (a1= arrayExpression[vars, check, sourceVars,varName] ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* )
                            {
                                DebugLocation(1625, 4);
                                DebugLocation(1625, 6);
                                Match(input, CLASS_CALL_INSTANCE_NODE, Follow._CLASS_CALL_INSTANCE_NODE_in_conditionalOrExpression1942); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1625, 31);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1625:31: (a1= arrayExpression[vars, check, sourceVars,varName] )
                                DebugEnterAlt(1);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1625:32: a1= arrayExpression[vars, check, sourceVars,varName]
                                {
                                    DebugLocation(1625, 32);
                                    if ((state.backtracking == 0))
                                    {
                                        varName = new List<CommonTree>();
                                    }
                                    DebugLocation(1625, 67);
                                    PushFollow(Follow._arrayExpression_in_conditionalOrExpression1948);
                                    a1 = arrayExpression(vars, check, sourceVars, varName);
                                    PopFollow();
                                    if (state.failed) return exp;
                                    DebugLocation(1625, 117);
                                    if ((state.backtracking == 0))
                                    {
                                        indices = new List<Expression>();
                                    }

                                }

                                DebugLocation(1625, 161);
                                method = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1956); if (state.failed) return exp;
                                DebugLocation(1625, 165);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1625:165: (e1= conditionalOrExpression[vars, check, sourceVars] )*
                                try
                                {
                                    DebugEnterSubRule(32);
                                    while (true)
                                    {
                                        int alt32 = 2;
                                        try
                                        {
                                            DebugEnterDecision(32, decisionCanBacktrack[32]);
                                            int LA32_0 = input.LA(1);

                                            if (((LA32_0 >= VAR_NODE && LA32_0 <= CALL_NODE) || LA32_0 == UNARY_NODE || (LA32_0 >= CLASS_CALL_NODE && LA32_0 <= CLASS_CALL_INSTANCE_NODE) || LA32_0 == INT || LA32_0 == 76 || (LA32_0 >= 78 && LA32_0 <= 79) || (LA32_0 >= 102 && LA32_0 <= 104) || (LA32_0 >= 110 && LA32_0 <= 123) || LA32_0 == 127 || LA32_0 == 153))
                                            {
                                                alt32 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(32); }
                                        switch (alt32)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1625:166: e1= conditionalOrExpression[vars, check, sourceVars]
                                                {
                                                    DebugLocation(1625, 168);
                                                    PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1961);
                                                    e1 = conditionalOrExpression(vars, check, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1625, 218);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        indices.Add(e1);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop32;
                                        }
                                    }

                                loop32:
                                    ;

                                }
                                finally { DebugExitSubRule(32); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1626, 2);
                                if ((state.backtracking == 0))
                                {

                                    CommonTree recordVar = varName[0];
                                    if (sourceVars != null && sourceVars.Count > 0)
                                    {
                                        if (!sourceVars.Contains(recordVar.Text))
                                        {
                                            throw new ParsingException("Variable " + recordVar.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) + "}", var.Token);
                                        }
                                    }
                                    else if (check)
                                    {
                                        exp = CheckVariableNotDeclared(vars, recordVar.Token);

                                        if (Spec.SpecValuation.Variables != null && Spec.SpecValuation.Variables.ContainsKey(recordVar.Text))
                                        {

                                        }

                                        else
                                        {
                                            Spec.AddNewWarning("When using user defined data structures as parameter variables, you have to make sure that its method call has no side effects, otherwise the verification result can not be guaranteed.", var.Token);
                                        }
                                    }
                                    exp = new ClassMethodCallInstance(a1, method.Text, indices.ToArray());

                                }

                            }
                            break;
                        case 29:
                            DebugEnterAlt(29);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1651:4: ^( 'new' (className= ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* )
                            {
                                DebugLocation(1651, 4);
                                DebugLocation(1651, 6);
                                Match(input, 127, Follow._127_in_conditionalOrExpression1978); if (state.failed) return exp;

                                Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                                DebugLocation(1651, 12);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1651:12: (className= ID )
                                DebugEnterAlt(1);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1651:13: className= ID
                                {
                                    DebugLocation(1651, 22);
                                    className = (CommonTree)Match(input, ID, Follow._ID_in_conditionalOrExpression1983); if (state.failed) return exp;
                                    DebugLocation(1651, 26);
                                    if ((state.backtracking == 0))
                                    {
                                        indices = new List<Expression>();
                                    }

                                }

                                DebugLocation(1651, 64);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1651:64: (e1= conditionalOrExpression[vars, check, sourceVars] )*
                                try
                                {
                                    DebugEnterSubRule(33);
                                    while (true)
                                    {
                                        int alt33 = 2;
                                        try
                                        {
                                            DebugEnterDecision(33, decisionCanBacktrack[33]);
                                            int LA33_0 = input.LA(1);

                                            if (((LA33_0 >= VAR_NODE && LA33_0 <= CALL_NODE) || LA33_0 == UNARY_NODE || (LA33_0 >= CLASS_CALL_NODE && LA33_0 <= CLASS_CALL_INSTANCE_NODE) || LA33_0 == INT || LA33_0 == 76 || (LA33_0 >= 78 && LA33_0 <= 79) || (LA33_0 >= 102 && LA33_0 <= 104) || (LA33_0 >= 110 && LA33_0 <= 123) || LA33_0 == 127 || LA33_0 == 153))
                                            {
                                                alt33 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(33); }
                                        switch (alt33)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1651:65: e1= conditionalOrExpression[vars, check, sourceVars]
                                                {
                                                    DebugLocation(1651, 67);
                                                    PushFollow(Follow._conditionalOrExpression_in_conditionalOrExpression1991);
                                                    e1 = conditionalOrExpression(vars, check, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return exp;
                                                    DebugLocation(1651, 117);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        indices.Add(e1);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop33;
                                        }
                                    }

                                loop33:
                                    ;

                                }
                                finally { DebugExitSubRule(33); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                                DebugLocation(1652, 2);
                                if ((state.backtracking == 0))
                                {

                                    if (!PAT.Common.Ultility.Ultility.IsUserDefinedDataTypeDefined(className.Text))
                                    {
                                        throw new ParsingException("Can not find the user defined data type. Please make sure you have imported it.", className.Token);
                                    }
                                    exp = new NewObjectCreation(className.Text, indices.ToArray());

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 16, conditionalOrExpression_StartIndex); }
                }
                DebugLocation(1659, 1);
            }
            finally { DebugExitRule(GrammarFileName, "conditionalOrExpression"); }
            return exp;

        }
        // $ANTLR end "conditionalOrExpression"


        protected virtual void Enter_arrayExpression() { }
        protected virtual void Leave_arrayExpression() { }

        // $ANTLR start "arrayExpression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1661:1: arrayExpression[List<string> vars, bool check, List<string> sourceVars, List<CommonTree> varName] returns [Expression aexp = null] : ^( VAR_NODE ( ID ) (index= conditionalOrExpression[vars, check, sourceVars] )+ ) ;
        [GrammarRule("arrayExpression")]
        private Expression arrayExpression(List<string> vars, bool check, List<string> sourceVars, List<CommonTree> varName)
        {

            Expression aexp = null;
            int arrayExpression_StartIndex = input.Index;
            CommonTree ID18 = null;
            CommonTree VAR_NODE19 = null;
            Expression index = default(Expression);

            paraphrases.Push("in array expression"); List<int> list = null; List<Expression> indices = null;
            try
            {
                DebugEnterRule(GrammarFileName, "arrayExpression");
                DebugLocation(1661, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 17)) { return aexp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1664:1: ( ^( VAR_NODE ( ID ) (index= conditionalOrExpression[vars, check, sourceVars] )+ ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1665:1: ^( VAR_NODE ( ID ) (index= conditionalOrExpression[vars, check, sourceVars] )+ )
                    {
                        DebugLocation(1665, 1);
                        DebugLocation(1665, 3);
                        VAR_NODE19 = (CommonTree)Match(input, VAR_NODE, Follow._VAR_NODE_in_arrayExpression2031); if (state.failed) return aexp;

                        Match(input, TokenTypes.Down, null); if (state.failed) return aexp;
                        DebugLocation(1665, 12);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1665:12: ( ID )
                        DebugEnterAlt(1);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1665:13: ID
                        {
                            DebugLocation(1665, 13);
                            ID18 = (CommonTree)Match(input, ID, Follow._ID_in_arrayExpression2034); if (state.failed) return aexp;
                            DebugLocation(1665, 16);
                            if ((state.backtracking == 0))
                            {
                                CheckRecordNotDeclared(vars, ID18.Token); if (ArrayID2DimentionMapping.ContainsKey(ID18.Text)) { list = ArrayID2DimentionMapping[ID18.Text]; } indices = new List<Expression>();
                            }

                        }

                        DebugLocation(1666, 3);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1666:3: (index= conditionalOrExpression[vars, check, sourceVars] )+
                        int cnt35 = 0;
                        try
                        {
                            DebugEnterSubRule(35);
                            while (true)
                            {
                                int alt35 = 2;
                                try
                                {
                                    DebugEnterDecision(35, decisionCanBacktrack[35]);
                                    int LA35_0 = input.LA(1);

                                    if (((LA35_0 >= VAR_NODE && LA35_0 <= CALL_NODE) || LA35_0 == UNARY_NODE || (LA35_0 >= CLASS_CALL_NODE && LA35_0 <= CLASS_CALL_INSTANCE_NODE) || LA35_0 == INT || LA35_0 == 76 || (LA35_0 >= 78 && LA35_0 <= 79) || (LA35_0 >= 102 && LA35_0 <= 104) || (LA35_0 >= 110 && LA35_0 <= 123) || LA35_0 == 127 || LA35_0 == 153))
                                    {
                                        alt35 = 1;
                                    }


                                }
                                finally { DebugExitDecision(35); }
                                switch (alt35)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1666:5: index= conditionalOrExpression[vars, check, sourceVars]
                                        {
                                            DebugLocation(1666, 10);
                                            PushFollow(Follow._conditionalOrExpression_in_arrayExpression2045);
                                            index = conditionalOrExpression(vars, check, sourceVars);
                                            PopFollow();
                                            if (state.failed) return aexp;
                                            DebugLocation(1667, 4);
                                            if ((state.backtracking == 0))
                                            {

                                                IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(VAR_NODE19.Children[indices.Count + 1] as CommonTree, input);
                                                PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(index, token1, "in array access", Spec.SpecValuation, ConstantDatabase);
                                                indices.Add(index);

                                                if (list != null && indices.Count > list.Count)
                                                {
                                                    throw new ParsingException("Array " + ID18.Text + " has only dimmention " + list.Count + "!", ID18.Token);
                                                }

                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt35 >= 1)
                                            goto loop35;

                                        if (state.backtracking > 0) { state.failed = true; return aexp; }
                                        EarlyExitException eee35 = new EarlyExitException(35, input);
                                        DebugRecognitionException(eee35);
                                        throw eee35;
                                }
                                cnt35++;
                            }
                        loop35:
                            ;

                        }
                        finally { DebugExitSubRule(35); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return aexp;
                        DebugLocation(1680, 2);
                        if ((state.backtracking == 0))
                        {

                            if (list != null && indices.Count != list.Count)
                            {
                                throw new ParsingException("Array " + ID18.Text + " has dimmention " + list.Count + "!", ID18.Token);
                            }

                            Expression index1 = indices[indices.Count - 1];

                            if (list != null)
                            {
                                int cumulator = list[indices.Count - 1];
                                for (int i = list.Count - 2; i >= 0; i--)
                                {
                                    index1 = new PrimitiveApplication("+", index1, new PrimitiveApplication("*", new IntConstant(cumulator), indices[i]));
                                    cumulator = cumulator * list[i];
                                }
                            }
                            else //the array is used inside channel or variables
                            {
                                if (indices.Count > 1)
                                {
                                    throw new ParsingException("Array " + ID18.Text + " can have only one dimmention here. PAT does not support multi-dimensional array in input channel variables or process parameters! But you can still use access the multi-dimentional array by treating it as 1 dimensional array and manually calculating the index.", ID18.Token);
                                }

                            }
                            if (varName != null)
                            {
                                varName.Add(ID18);// Add the var name for CLASS_CALL_INSTANCE_NODE checking
                            }
                            aexp = new PrimitiveApplication(".", new Variable(ID18.Text), index1);



                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 17, arrayExpression_StartIndex); }
                }
                DebugLocation(1712, 1);
            }
            finally { DebugExitRule(GrammarFileName, "arrayExpression"); }
            return aexp;

        }
        // $ANTLR end "arrayExpression"


        protected virtual void Enter_ifExpression() { }
        protected virtual void Leave_ifExpression() { }

        // $ANTLR start "ifExpression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1714:1: ifExpression[List<string> vars, List<string> sourceVars] returns [Expression exp = null] : ^(token= 'if' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] (e3= statement[vars, sourceVars] )? ) ;
        [GrammarRule("ifExpression")]
        private Expression ifExpression(List<string> vars, List<string> sourceVars)
        {

            Expression exp = null;
            int ifExpression_StartIndex = input.Index;
            CommonTree token = null;
            Expression e1 = default(Expression);
            Expression e2 = default(Expression);
            Expression e3 = default(Expression);

            paraphrases.Push("in if expression");
            try
            {
                DebugEnterRule(GrammarFileName, "ifExpression");
                DebugLocation(1714, 2);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 18)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1717:2: ( ^(token= 'if' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] (e3= statement[vars, sourceVars] )? ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1717:5: ^(token= 'if' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] (e3= statement[vars, sourceVars] )? )
                    {
                        DebugLocation(1717, 5);
                        DebugLocation(1717, 12);
                        token = (CommonTree)Match(input, 131, Follow._131_in_ifExpression2097); if (state.failed) return exp;

                        Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                        DebugLocation(1717, 20);
                        PushFollow(Follow._expression_in_ifExpression2101);
                        e1 = expression(vars, true, sourceVars);
                        PopFollow();
                        if (state.failed) return exp;
                        DebugLocation(1717, 59);
                        PushFollow(Follow._statement_in_ifExpression2107);
                        e2 = statement(vars, sourceVars);
                        PopFollow();
                        if (state.failed) return exp;
                        DebugLocation(1717, 90);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1717:90: (e3= statement[vars, sourceVars] )?
                        int alt36 = 2;
                        try
                        {
                            DebugEnterSubRule(36);
                            try
                            {
                                DebugEnterDecision(36, decisionCanBacktrack[36]);
                                int LA36_0 = input.LA(1);

                                if ((LA36_0 == BLOCK_NODE || (LA36_0 >= VAR_NODE && LA36_0 <= CALL_NODE) || LA36_0 == ASSIGNMENT_NODE || LA36_0 == UNARY_NODE || (LA36_0 >= LOCAL_VAR_NODE && LA36_0 <= LOCAL_ARRAY_NODE) || (LA36_0 >= CLASS_CALL_NODE && LA36_0 <= CLASS_CALL_INSTANCE_NODE) || LA36_0 == INT || LA36_0 == 76 || (LA36_0 >= 78 && LA36_0 <= 79) || (LA36_0 >= 102 && LA36_0 <= 104) || (LA36_0 >= 110 && LA36_0 <= 123) || LA36_0 == 127 || LA36_0 == 131 || LA36_0 == 133 || LA36_0 == 153))
                                {
                                    alt36 = 1;
                                }
                            }
                            finally { DebugExitDecision(36); }
                            switch (alt36)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: e3= statement[vars, sourceVars]
                                    {
                                        DebugLocation(1717, 90);
                                        PushFollow(Follow._statement_in_ifExpression2112);
                                        e3 = statement(vars, sourceVars);
                                        PopFollow();
                                        if (state.failed) return exp;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(36); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                        DebugLocation(1718, 3);
                        if ((state.backtracking == 0))
                        {

                            IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(token.Children[0] as CommonTree, input);
                            PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(e1, token1, "in if expression", Spec.SpecValuation, ConstantDatabase);
                            exp = new If(e1, e2, e3);

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 18, ifExpression_StartIndex); }
                }
                DebugLocation(1723, 2);
            }
            finally { DebugExitRule(GrammarFileName, "ifExpression"); }
            return exp;

        }
        // $ANTLR end "ifExpression"


        protected virtual void Enter_whileExpression() { }
        protected virtual void Leave_whileExpression() { }

        // $ANTLR start "whileExpression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1725:1: whileExpression[List<string> vars, List<string> sourceVars] returns [Expression exp = null] : ^(token= 'while' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] ) ;
        [GrammarRule("whileExpression")]
        private Expression whileExpression(List<string> vars, List<string> sourceVars)
        {

            Expression exp = null;
            int whileExpression_StartIndex = input.Index;
            CommonTree token = null;
            Expression e1 = default(Expression);
            Expression e2 = default(Expression);

            paraphrases.Push("in while expression");
            try
            {
                DebugEnterRule(GrammarFileName, "whileExpression");
                DebugLocation(1725, 6);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 19)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1728:2: ( ^(token= 'while' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1728:4: ^(token= 'while' e1= expression[vars, true, sourceVars] e2= statement[vars, sourceVars] )
                    {
                        DebugLocation(1728, 4);
                        DebugLocation(1728, 11);
                        token = (CommonTree)Match(input, 133, Follow._133_in_whileExpression2152); if (state.failed) return exp;

                        Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                        DebugLocation(1728, 22);
                        PushFollow(Follow._expression_in_whileExpression2156);
                        e1 = expression(vars, true, sourceVars);
                        PopFollow();
                        if (state.failed) return exp;
                        DebugLocation(1728, 60);
                        PushFollow(Follow._statement_in_whileExpression2161);
                        e2 = statement(vars, sourceVars);
                        PopFollow();
                        if (state.failed) return exp;

                        Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                        DebugLocation(1729, 8);
                        if ((state.backtracking == 0))
                        {

                            IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(token.Children[0] as CommonTree, input);
                            PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(e1, token1, "in while expression", Spec.SpecValuation, ConstantDatabase);
                            exp = new While(e1, e2);

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 19, whileExpression_StartIndex); }
                }
                DebugLocation(1734, 6);
            }
            finally { DebugExitRule(GrammarFileName, "whileExpression"); }
            return exp;

        }
        // $ANTLR end "whileExpression"


        protected virtual void Enter_recordExpression() { }
        protected virtual void Leave_recordExpression() { }

        // $ANTLR start "recordExpression"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1736:1: recordExpression[List<string> vars, List<string> sourceVars, IToken idToken] returns [Expression exp = null] : ^( RECORD_NODE (s= recordElement[vars, sourceVars, idToken] )+ ) ;
        [GrammarRule("recordExpression")]
        private Expression recordExpression(List<string> vars, List<string> sourceVars, IToken idToken)
        {

            Expression exp = null;
            int recordExpression_StartIndex = input.Index;
            List<Expression> s = default(List<Expression>);

            paraphrases.Push("in array expression"); List<Expression> list = new List<Expression>();
            try
            {
                DebugEnterRule(GrammarFileName, "recordExpression");
                DebugLocation(1736, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 20)) { return exp; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1739:2: ( ^( RECORD_NODE (s= recordElement[vars, sourceVars, idToken] )+ ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1739:5: ^( RECORD_NODE (s= recordElement[vars, sourceVars, idToken] )+ )
                    {
                        DebugLocation(1739, 5);
                        DebugLocation(1739, 7);
                        Match(input, RECORD_NODE, Follow._RECORD_NODE_in_recordExpression2214); if (state.failed) return exp;

                        Match(input, TokenTypes.Down, null); if (state.failed) return exp;
                        DebugLocation(1739, 19);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1739:19: (s= recordElement[vars, sourceVars, idToken] )+
                        int cnt37 = 0;
                        try
                        {
                            DebugEnterSubRule(37);
                            while (true)
                            {
                                int alt37 = 2;
                                try
                                {
                                    DebugEnterDecision(37, decisionCanBacktrack[37]);
                                    int LA37_0 = input.LA(1);

                                    if (((LA37_0 >= RECORD_ELEMENT_NODE && LA37_0 <= RECORD_ELEMENT_RANGE_NODE)))
                                    {
                                        alt37 = 1;
                                    }


                                }
                                finally { DebugExitDecision(37); }
                                switch (alt37)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1739:20: s= recordElement[vars, sourceVars, idToken]
                                        {
                                            DebugLocation(1739, 21);
                                            PushFollow(Follow._recordElement_in_recordExpression2219);
                                            s = recordElement(vars, sourceVars, idToken);
                                            PopFollow();
                                            if (state.failed) return exp;
                                            DebugLocation(1739, 63);
                                            if ((state.backtracking == 0))
                                            {
                                                list.AddRange(s);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt37 >= 1)
                                            goto loop37;

                                        if (state.backtracking > 0) { state.failed = true; return exp; }
                                        EarlyExitException eee37 = new EarlyExitException(37, input);
                                        DebugRecognitionException(eee37);
                                        throw eee37;
                                }
                                cnt37++;
                            }
                        loop37:
                            ;

                        }
                        finally { DebugExitSubRule(37); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return exp;
                        DebugLocation(1740, 2);
                        if ((state.backtracking == 0))
                        {
                            exp = new Record(list.ToArray());
                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 20, recordExpression_StartIndex); }
                }
                DebugLocation(1741, 1);
            }
            finally { DebugExitRule(GrammarFileName, "recordExpression"); }
            return exp;

        }
        // $ANTLR end "recordExpression"


        protected virtual void Enter_recordElement() { }
        protected virtual void Leave_recordElement() { }

        // $ANTLR start "recordElement"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1744:1: recordElement[List<string> vars, List<string> sourceVars, IToken idToken] returns [List<Expression> list = new List<Expression>()] : ( ^(token= RECORD_ELEMENT_NODE e1= expression[vars, true, sourceVars] (e2= expression[vars, true, sourceVars] )? ) | ^(token= RECORD_ELEMENT_RANGE_NODE e1= expression[vars, true, sourceVars] e2= expression[vars, true, sourceVars] ) );
        [GrammarRule("recordElement")]
        private List<Expression> recordElement(List<string> vars, List<string> sourceVars, IToken idToken)
        {

            List<Expression> list = new List<Expression>();
            int recordElement_StartIndex = input.Index;
            CommonTree token = null;
            Expression e1 = default(Expression);
            Expression e2 = default(Expression);

            try
            {
                DebugEnterRule(GrammarFileName, "recordElement");
                DebugLocation(1744, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 21)) { return list; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1745:2: ( ^(token= RECORD_ELEMENT_NODE e1= expression[vars, true, sourceVars] (e2= expression[vars, true, sourceVars] )? ) | ^(token= RECORD_ELEMENT_RANGE_NODE e1= expression[vars, true, sourceVars] e2= expression[vars, true, sourceVars] ) )
                    int alt39 = 2;
                    try
                    {
                        DebugEnterDecision(39, decisionCanBacktrack[39]);
                        int LA39_0 = input.LA(1);

                        if ((LA39_0 == RECORD_ELEMENT_NODE))
                        {
                            alt39 = 1;
                        }
                        else if ((LA39_0 == RECORD_ELEMENT_RANGE_NODE))
                        {
                            alt39 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return list; }
                            NoViableAltException nvae = new NoViableAltException("", 39, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(39); }
                    switch (alt39)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1745:4: ^(token= RECORD_ELEMENT_NODE e1= expression[vars, true, sourceVars] (e2= expression[vars, true, sourceVars] )? )
                            {
                                DebugLocation(1745, 4);
                                DebugLocation(1745, 11);
                                token = (CommonTree)Match(input, RECORD_ELEMENT_NODE, Follow._RECORD_ELEMENT_NODE_in_recordElement2251); if (state.failed) return list;

                                Match(input, TokenTypes.Down, null); if (state.failed) return list;
                                DebugLocation(1745, 34);
                                PushFollow(Follow._expression_in_recordElement2255);
                                e1 = expression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return list;
                                DebugLocation(1745, 70);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1745:70: (e2= expression[vars, true, sourceVars] )?
                                int alt38 = 2;
                                try
                                {
                                    DebugEnterSubRule(38);
                                    try
                                    {
                                        DebugEnterDecision(38, decisionCanBacktrack[38]);
                                        int LA38_0 = input.LA(1);

                                        if (((LA38_0 >= VAR_NODE && LA38_0 <= CALL_NODE) || LA38_0 == ASSIGNMENT_NODE || LA38_0 == UNARY_NODE || (LA38_0 >= CLASS_CALL_NODE && LA38_0 <= CLASS_CALL_INSTANCE_NODE) || LA38_0 == INT || LA38_0 == 76 || (LA38_0 >= 78 && LA38_0 <= 79) || (LA38_0 >= 102 && LA38_0 <= 104) || (LA38_0 >= 110 && LA38_0 <= 123) || LA38_0 == 127 || LA38_0 == 153))
                                        {
                                            alt38 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(38); }
                                    switch (alt38)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1745:71: e2= expression[vars, true, sourceVars]
                                            {
                                                DebugLocation(1745, 73);
                                                PushFollow(Follow._expression_in_recordElement2261);
                                                e2 = expression(vars, true, sourceVars);
                                                PopFollow();
                                                if (state.failed) return list;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(38); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return list;
                                DebugLocation(1746, 2);
                                if ((state.backtracking == 0))
                                {

                                    //IntConstant i = PAT.Common.Ultility.ParsingUltility.EvaluateIntExpression(e1, token1, ConstantDatabase);
                                    e1 = e1.ClearConstant(ConstantDatabase);
                                    if (e2 == null)
                                    {
                                        list.Add(e1);
                                    }
                                    else
                                    {
                                        IToken token2 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(token.Children[1] as CommonTree, input);
                                        int j = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e2, token2, ConstantDatabase);
                                        while (j > 0)
                                        {
                                            list.Add(e1);
                                            j--;
                                        }

                                    }

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1765:4: ^(token= RECORD_ELEMENT_RANGE_NODE e1= expression[vars, true, sourceVars] e2= expression[vars, true, sourceVars] )
                            {
                                DebugLocation(1765, 4);
                                DebugLocation(1765, 11);
                                token = (CommonTree)Match(input, RECORD_ELEMENT_RANGE_NODE, Follow._RECORD_ELEMENT_RANGE_NODE_in_recordElement2276); if (state.failed) return list;

                                Match(input, TokenTypes.Down, null); if (state.failed) return list;
                                DebugLocation(1765, 40);
                                PushFollow(Follow._expression_in_recordElement2280);
                                e1 = expression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return list;
                                DebugLocation(1765, 78);
                                PushFollow(Follow._expression_in_recordElement2285);
                                e2 = expression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return list;

                                Match(input, TokenTypes.Up, null); if (state.failed) return list;
                                DebugLocation(1766, 2);
                                if ((state.backtracking == 0))
                                {

                                    IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(token.Children[0] as CommonTree, input);
                                    IToken token2 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(token.Children[1] as CommonTree, input);

                                    int i = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e1, token1, ConstantDatabase);
                                    int j = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e2, token2, ConstantDatabase);

                                    if (j < i)
                                    {
                                        for (int k = i; k >= j; k--)
                                        {
                                            list.Add(new IntConstant(k));
                                        }
                                    }
                                    else
                                    {
                                        for (int k = i; k <= j; k++)
                                        {
                                            list.Add(new IntConstant(k));
                                        }
                                    }

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 21, recordElement_StartIndex); }
                }
                DebugLocation(1788, 1);
            }
            finally { DebugExitRule(GrammarFileName, "recordElement"); }
            return list;

        }
        // $ANTLR end "recordElement"


        protected virtual void Enter_definition() { }
        protected virtual void Leave_definition() { }

        // $ANTLR start "definition"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1790:1: definition returns [Definition proc = null] : ( ^( DEFINITION_NODE name= ID (v= ID )* p= processExpr[vars, name.Text, null] ) | ^( PROCESS_NODE pname= STRING (v= ID )* (tran= transition[vars, $pname.Text.Trim('\"'), states] )* ) );
        [GrammarRule("definition")]
        private Definition definition()
        {

            Definition proc = null;
            int definition_StartIndex = input.Index;
            CommonTree name = null;
            CommonTree v = null;
            CommonTree pname = null;
            PetriNet p = default(PetriNet);
            Transition tran = default(Transition);

            paraphrases.Push("in process definition");
            List<string> vars = new List<string>();
            List<Transition> trans = new List<Transition>();
            List<PNPlace> states = new List<PNPlace>();

            try
            {
                DebugEnterRule(GrammarFileName, "definition");
                DebugLocation(1790, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 22)) { return proc; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1797:2: ( ^( DEFINITION_NODE name= ID (v= ID )* p= processExpr[vars, name.Text, null] ) | ^( PROCESS_NODE pname= STRING (v= ID )* (tran= transition[vars, $pname.Text.Trim('\"'), states] )* ) )
                    int alt43 = 2;
                    try
                    {
                        DebugEnterDecision(43, decisionCanBacktrack[43]);
                        int LA43_0 = input.LA(1);

                        if ((LA43_0 == DEFINITION_NODE))
                        {
                            alt43 = 1;
                        }
                        else if ((LA43_0 == PROCESS_NODE))
                        {
                            alt43 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return proc; }
                            NoViableAltException nvae = new NoViableAltException("", 43, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(43); }
                    switch (alt43)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1797:4: ^( DEFINITION_NODE name= ID (v= ID )* p= processExpr[vars, name.Text, null] )
                            {
                                DebugLocation(1797, 4);
                                DebugLocation(1797, 6);
                                Match(input, DEFINITION_NODE, Follow._DEFINITION_NODE_in_definition2319); if (state.failed) return proc;

                                Match(input, TokenTypes.Down, null); if (state.failed) return proc;
                                DebugLocation(1797, 26);
                                name = (CommonTree)Match(input, ID, Follow._ID_in_definition2323); if (state.failed) return proc;
                                DebugLocation(1797, 30);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1797:30: (v= ID )*
                                try
                                {
                                    DebugEnterSubRule(40);
                                    while (true)
                                    {
                                        int alt40 = 2;
                                        try
                                        {
                                            DebugEnterDecision(40, decisionCanBacktrack[40]);
                                            int LA40_0 = input.LA(1);

                                            if ((LA40_0 == ID))
                                            {
                                                alt40 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(40); }
                                        switch (alt40)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1797:31: v= ID
                                                {
                                                    DebugLocation(1797, 32);
                                                    v = (CommonTree)Match(input, ID, Follow._ID_in_definition2328); if (state.failed) return proc;
                                                    DebugLocation(1797, 35);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        CheckDuplicatedDeclaration(v.Token, vars); vars.Add(v.Text);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop40;
                                        }
                                    }

                                loop40:
                                    ;

                                }
                                finally { DebugExitSubRule(40); }

                                DebugLocation(1798, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckDuplicatedDeclaration(name.Token);
                                    if (!Spec.DefinitionDatabase.ContainsKey(name.Text))
                                    {
                                        proc = new Definition(name.Text, vars.ToArray(), p);
                                        Spec.DeclaritionTable.Add(name.Text, new Declaration(DeclarationType.Process, new ParsingException(name.Text, name.Token)));
                                        Spec.DefinitionDatabase.Add(name.Text, proc);
                                        CurrentDefinition = proc;

                                    }
                                    else
                                    {
                                        //throw new Exception("Error happened at line " + ID8.Line +" col "+ ID7.CharPositionInLine + " Process " + ID7.Text + " is defined already!");
                                        throw new ParsingException("Process " + name.Text + " is defined already!", name.Token);
                                    }

                                }
                                DebugLocation(1814, 4);
                                PushFollow(Follow._processExpr_in_definition2341);
                                p = processExpr(vars, name.Text, null);
                                PopFollow();
                                if (state.failed) return proc;

                                Match(input, TokenTypes.Up, null); if (state.failed) return proc;
                                DebugLocation(1816, 2);
                                if ((state.backtracking == 0))
                                {

                                    proc.Process = p;
                                    // Tinh comment following line - skip now
                                    // PAT.LTS.Ultility.Ultility.CheckForSelfComposition(name, p);
                                    CurrentDefinition = null;

                                    if (p is DefinitionRef && (p as DefinitionRef).Name == name.Text)
                                    {
                                        throw new ParsingException("Self-looping definition is not allowed!", name.Token);
                                    }

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1827:4: ^( PROCESS_NODE pname= STRING (v= ID )* (tran= transition[vars, $pname.Text.Trim('\"'), states] )* )
                            {
                                DebugLocation(1827, 4);
                                DebugLocation(1827, 6);
                                Match(input, PROCESS_NODE, Follow._PROCESS_NODE_in_definition2354); if (state.failed) return proc;

                                Match(input, TokenTypes.Down, null); if (state.failed) return proc;
                                DebugLocation(1827, 24);
                                pname = (CommonTree)Match(input, STRING, Follow._STRING_in_definition2358); if (state.failed) return proc;
                                DebugLocation(1827, 32);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1827:32: (v= ID )*
                                try
                                {
                                    DebugEnterSubRule(41);
                                    while (true)
                                    {
                                        int alt41 = 2;
                                        try
                                        {
                                            DebugEnterDecision(41, decisionCanBacktrack[41]);
                                            int LA41_0 = input.LA(1);

                                            if ((LA41_0 == ID))
                                            {
                                                alt41 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(41); }
                                        switch (alt41)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1827:33: v= ID
                                                {
                                                    DebugLocation(1827, 34);
                                                    v = (CommonTree)Match(input, ID, Follow._ID_in_definition2363); if (state.failed) return proc;
                                                    DebugLocation(1827, 37);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        vars.Add(v.Text); CheckDuplicatedDeclaration(v.Token);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop41;
                                        }
                                    }

                                loop41:
                                    ;

                                }
                                finally { DebugExitSubRule(41); }

                                DebugLocation(1828, 5);
                                if ((state.backtracking == 0))
                                {

                                    CurrentLTSGraphAlphabetsCalculable = true;

                                }
                                DebugLocation(1833, 6);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1833:6: (tran= transition[vars, $pname.Text.Trim('\"'), states] )*
                                try
                                {
                                    DebugEnterSubRule(42);
                                    while (true)
                                    {
                                        int alt42 = 2;
                                        try
                                        {
                                            DebugEnterDecision(42, decisionCanBacktrack[42]);
                                            int LA42_0 = input.LA(1);

                                            if ((LA42_0 == TRANSITION_NODE))
                                            {
                                                alt42 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(42); }
                                        switch (alt42)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1833:7: tran= transition[vars, $pname.Text.Trim('\"'), states]
                                                {
                                                    DebugLocation(1833, 11);
                                                    PushFollow(Follow._transition_in_definition2396);
                                                    tran = transition(vars, pname.Text.Trim('"'), states);
                                                    PopFollow();
                                                    if (state.failed) return proc;
                                                    DebugLocation(1833, 60);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        trans.Add(tran);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop42;
                                        }
                                    }

                                loop42:
                                    ;

                                }
                                finally { DebugExitSubRule(42); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return proc;
                                DebugLocation(1834, 2);
                                if ((state.backtracking == 0))
                                {

                                    if (!Spec.PNDefinitionDatabase.ContainsKey(pname.Text.Trim('"')))
                                    {
                                        if (states.Count == 0)
                                        {
                                            states.Add(new PNPlace("Test", "", ""));
                                        }
                                        PetriNet process = new PetriNet(pname.Text.Trim('"'), vars, states);
                                        process.SetTransitions(trans);
                                        Spec.PNDefinitionDatabase.Add(pname.Text.Trim('"'), process);
                                        Spec.DeclaritionTable.Add(pname.Text, new Declaration(DeclarationType.Process, new ParsingException(pname.Text, pname.Token)));
                                        process.AlphabetsCalculable = CurrentLTSGraphAlphabetsCalculable;
                                    }
                                    else
                                    {
                                        //throw new Exception("Error happened at line " + ID8.Line +" col "+ ID7.CharPositionInLine + " Process " + ID7.Text + " is defined already!");
                                        throw new GraphParsingException(pname.Text.Trim('"'), "Process " + pname.Text.Trim('"') + " is defined already!", pname.Token);
                                    }
                                    CurrentLTSGraphAlphabetsCalculable = true;

                                }

                            }
                            break;

                    }
                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 22, definition_StartIndex); }
                }
                DebugLocation(1852, 1);
            }
            finally { DebugExitRule(GrammarFileName, "definition"); }
            return proc;

        }
        // $ANTLR end "definition"


        protected virtual void Enter_stateDef() { }
        protected virtual void Leave_stateDef() { }

        // $ANTLR start "stateDef"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1854:1: stateDef[List<string> vars, String defID, string ID, List<PNPlace> states] returns [PNPlace stateVar = null] : ^( PLACE_NODE name= STRING ) ;
        [GrammarRule("stateDef")]
        private PNPlace stateDef(List<string> vars, String defID, string ID, List<PNPlace> states)
        {

            PNPlace stateVar = null;
            int stateDef_StartIndex = input.Index;
            CommonTree name = null;

            paraphrases.Push("in state definition");
            try
            {
                DebugEnterRule(GrammarFileName, "stateDef");
                DebugLocation(1854, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 23)) { return stateVar; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1857:2: ( ^( PLACE_NODE name= STRING ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1857:4: ^( PLACE_NODE name= STRING )
                    {
                        DebugLocation(1857, 4);
                        DebugLocation(1857, 6);
                        Match(input, PLACE_NODE, Follow._PLACE_NODE_in_stateDef2436); if (state.failed) return stateVar;

                        Match(input, TokenTypes.Down, null); if (state.failed) return stateVar;
                        DebugLocation(1857, 21);
                        name = (CommonTree)Match(input, STRING, Follow._STRING_in_stateDef2440); if (state.failed) return stateVar;

                        Match(input, TokenTypes.Up, null); if (state.failed) return stateVar;
                        DebugLocation(1858, 2);
                        if ((state.backtracking == 0))
                        {

                            foreach (PNPlace s in states)
                            {
                                if (s.Name == name.Text.Trim('"'))
                                {
                                    throw new Exception("Duplicated state name: " + s.Name);
                                }
                            }
                            stateVar = new PNPlace(name.Text.Trim('"'), ID);

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }
                catch (Exception ex)
                {

                    throw new GraphParsingException(defID, ex.Message, -1, -1, "");

                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 23, stateDef_StartIndex); }
                }
                DebugLocation(1868, 1);
            }
            finally { DebugExitRule(GrammarFileName, "stateDef"); }
            return stateVar;

        }
        // $ANTLR end "stateDef"


        protected virtual void Enter_transition() { }
        protected virtual void Leave_transition() { }

        // $ANTLR start "transition"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1873:1: transition[List<string> varsInit, String defID, List<PNPlace> states] returns [Transition tran = null] : ^( TRANSITION_NODE from= STRING (selects= select[vars] )? (guard= conditionalOrExpression[vars, true, null] )? (evt= eventT[vars, defID, false] ) (e= block[vars, null] )? to= STRING ) ;
        [GrammarRule("transition")]
        private Transition transition(List<string> varsInit, String defID, List<PNPlace> states)
        {

            Transition tran = null;
            int transition_StartIndex = input.Index;
            CommonTree from = null;
            CommonTree to = null;
            CommonTree TRANSITION_NODE20 = null;
            List<ParallelDefinition> selects = default(List<ParallelDefinition>);
            Expression guard = default(Expression);
            Event evt = default(Event);
            Expression e = default(Expression);

            paraphrases.Push("in transition definition");
            List<string> vars = new List<string>(varsInit);

            try
            {
                DebugEnterRule(GrammarFileName, "transition");
                DebugLocation(1873, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 24)) { return tran; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1878:2: ( ^( TRANSITION_NODE from= STRING (selects= select[vars] )? (guard= conditionalOrExpression[vars, true, null] )? (evt= eventT[vars, defID, false] ) (e= block[vars, null] )? to= STRING ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1878:5: ^( TRANSITION_NODE from= STRING (selects= select[vars] )? (guard= conditionalOrExpression[vars, true, null] )? (evt= eventT[vars, defID, false] ) (e= block[vars, null] )? to= STRING )
                    {
                        DebugLocation(1878, 5);
                        DebugLocation(1878, 7);
                        TRANSITION_NODE20 = (CommonTree)Match(input, TRANSITION_NODE, Follow._TRANSITION_NODE_in_transition2482); if (state.failed) return tran;

                        Match(input, TokenTypes.Down, null); if (state.failed) return tran;
                        DebugLocation(1878, 27);
                        from = (CommonTree)Match(input, STRING, Follow._STRING_in_transition2486); if (state.failed) return tran;
                        DebugLocation(1878, 42);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1878:42: (selects= select[vars] )?
                        int alt44 = 2;
                        try
                        {
                            DebugEnterSubRule(44);
                            try
                            {
                                DebugEnterDecision(44, decisionCanBacktrack[44]);
                                int LA44_0 = input.LA(1);

                                if ((LA44_0 == SELECT_NODE))
                                {
                                    alt44 = 1;
                                }
                            }
                            finally { DebugExitDecision(44); }
                            switch (alt44)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: selects= select[vars]
                                    {
                                        DebugLocation(1878, 42);
                                        PushFollow(Follow._select_in_transition2490);
                                        selects = select(vars);
                                        PopFollow();
                                        if (state.failed) return tran;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(44); }

                        DebugLocation(1879, 12);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1879:12: (guard= conditionalOrExpression[vars, true, null] )?
                        int alt45 = 2;
                        try
                        {
                            DebugEnterSubRule(45);
                            try
                            {
                                DebugEnterDecision(45, decisionCanBacktrack[45]);
                                int LA45_0 = input.LA(1);

                                if (((LA45_0 >= VAR_NODE && LA45_0 <= CALL_NODE) || LA45_0 == UNARY_NODE || (LA45_0 >= CLASS_CALL_NODE && LA45_0 <= CLASS_CALL_INSTANCE_NODE) || LA45_0 == INT || LA45_0 == 76 || (LA45_0 >= 78 && LA45_0 <= 79) || (LA45_0 >= 102 && LA45_0 <= 104) || (LA45_0 >= 110 && LA45_0 <= 123) || LA45_0 == 127 || LA45_0 == 153))
                                {
                                    alt45 = 1;
                                }
                            }
                            finally { DebugExitDecision(45); }
                            switch (alt45)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: guard= conditionalOrExpression[vars, true, null]
                                    {
                                        DebugLocation(1879, 12);
                                        PushFollow(Follow._conditionalOrExpression_in_transition2503);
                                        guard = conditionalOrExpression(vars, true, null);
                                        PopFollow();
                                        if (state.failed) return tran;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(45); }

                        DebugLocation(1880, 7);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1880:7: (evt= eventT[vars, defID, false] )
                        DebugEnterAlt(1);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1880:8: evt= eventT[vars, defID, false]
                        {
                            DebugLocation(1880, 11);
                            PushFollow(Follow._eventT_in_transition2517);
                            evt = eventT(vars, defID, false);
                            PopFollow();
                            if (state.failed) return tran;

                        }

                        DebugLocation(1881, 7);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1881:7: (e= block[vars, null] )?
                        int alt46 = 2;
                        try
                        {
                            DebugEnterSubRule(46);
                            try
                            {
                                DebugEnterDecision(46, decisionCanBacktrack[46]);
                                int LA46_0 = input.LA(1);

                                if ((LA46_0 == BLOCK_NODE))
                                {
                                    alt46 = 1;
                                }
                            }
                            finally { DebugExitDecision(46); }
                            switch (alt46)
                            {
                                case 1:
                                    DebugEnterAlt(1);
                                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1881:8: e= block[vars, null]
                                    {
                                        DebugLocation(1881, 9);
                                        PushFollow(Follow._block_in_transition2531);
                                        e = block(vars, null);
                                        PopFollow();
                                        if (state.failed) return tran;

                                    }
                                    break;

                            }
                        }
                        finally { DebugExitSubRule(46); }

                        DebugLocation(1883, 9);
                        to = (CommonTree)Match(input, STRING, Follow._STRING_in_transition2552); if (state.failed) return tran;

                        Match(input, TokenTypes.Up, null); if (state.failed) return tran;
                        DebugLocation(1884, 2);
                        if ((state.backtracking == 0))
                        {


                            if (guard != null)
                            {
                                int index = 1;
                                if (selects != null)
                                {
                                    index = 2;
                                }
                                IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(TRANSITION_NODE20.Children[index] as CommonTree, input);
                                PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(guard, token1, "in guard expression", Spec.SpecValuation, ConstantDatabase);
                            }

                            PNPlace newFrom = null, newTo = null;
                            foreach (PNPlace s in states)
                            {
                                if (s.Name == from.Text.Trim('"'))
                                {
                                    newFrom = s;
                                }

                                if (s.Name == to.Text.Trim('"'))
                                {
                                    newTo = s;
                                }
                            }

                            tran = new Transition(evt, selects == null ? null : selects.ToArray(), guard, e, newFrom, newTo);
                            tran.HasLocalVariable = (LocalVariables.Count != 0);
                            if (ConstantDatabase.Count > 0)
                            {
                                tran = tran.ClearConstant(states, ConstantDatabase, false);
                            }
                            //clear the localVariables
                            LocalVariables.Clear();

                        }

                    }

                    if ((state.backtracking == 0))
                    {
                        paraphrases.Pop();
                    }
                }
                catch (Exception ex)
                {

                    throw new GraphParsingException(defID, ex.Message, -1, -1, "");

                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 24, transition_StartIndex); }
                }
                DebugLocation(1920, 1);
            }
            finally { DebugExitRule(GrammarFileName, "transition"); }
            return tran;

        }
        // $ANTLR end "transition"


        protected virtual void Enter_select() { }
        protected virtual void Leave_select() { }

        // $ANTLR start "select"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1925:1: select[List<string> vars] returns [List<ParallelDefinition> ppds = new List<ParallelDefinition>()] : ^( SELECT_NODE (pdd= paralDef[vars, svars] )+ ) ;
        [GrammarRule("select")]
        private List<ParallelDefinition> select(List<string> vars)
        {

            List<ParallelDefinition> ppds = new List<ParallelDefinition>();
            int select_StartIndex = input.Index;
            ParallelDefinition pdd = default(ParallelDefinition);


            List<PetriNet> processes = new List<PetriNet>(16);
            List<string> svars = new List<string>(vars);

            try
            {
                DebugEnterRule(GrammarFileName, "select");
                DebugLocation(1925, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 25)) { return ppds; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1930:2: ( ^( SELECT_NODE (pdd= paralDef[vars, svars] )+ ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1930:4: ^( SELECT_NODE (pdd= paralDef[vars, svars] )+ )
                    {
                        DebugLocation(1930, 4);
                        DebugLocation(1930, 6);
                        Match(input, SELECT_NODE, Follow._SELECT_NODE_in_select2591); if (state.failed) return ppds;

                        Match(input, TokenTypes.Down, null); if (state.failed) return ppds;
                        DebugLocation(1931, 5);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1931:5: (pdd= paralDef[vars, svars] )+
                        int cnt47 = 0;
                        try
                        {
                            DebugEnterSubRule(47);
                            while (true)
                            {
                                int alt47 = 2;
                                try
                                {
                                    DebugEnterDecision(47, decisionCanBacktrack[47]);
                                    int LA47_0 = input.LA(1);

                                    if (((LA47_0 >= PARADEF_NODE && LA47_0 <= PARADEF1_NODE)))
                                    {
                                        alt47 = 1;
                                    }


                                }
                                finally { DebugExitDecision(47); }
                                switch (alt47)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1931:6: pdd= paralDef[vars, svars]
                                        {
                                            DebugLocation(1931, 9);
                                            PushFollow(Follow._paralDef_in_select2601);
                                            pdd = paralDef(vars, svars);
                                            PopFollow();
                                            if (state.failed) return ppds;
                                            DebugLocation(1931, 32);
                                            if ((state.backtracking == 0))
                                            {
                                                ppds.Add(pdd);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt47 >= 1)
                                            goto loop47;

                                        if (state.backtracking > 0) { state.failed = true; return ppds; }
                                        EarlyExitException eee47 = new EarlyExitException(47, input);
                                        DebugRecognitionException(eee47);
                                        throw eee47;
                                }
                                cnt47++;
                            }
                        loop47:
                            ;

                        }
                        finally { DebugExitSubRule(47); }

                        DebugLocation(1931, 51);
                        if ((state.backtracking == 0))
                        {
                            foreach (ParallelDefinition pd in ppds) { vars.Add(pd.Parameter); svars.Add(pd.Parameter); }
                        }

                        Match(input, TokenTypes.Up, null); if (state.failed) return ppds;

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 25, select_StartIndex); }
                }
                DebugLocation(1932, 1);
            }
            finally { DebugExitRule(GrammarFileName, "select"); }
            return ppds;

        }
        // $ANTLR end "select"


        protected virtual void Enter_eventT() { }
        protected virtual void Leave_eventT() { }

        // $ANTLR start "eventT"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1934:1: eventT[List<string> vars, string defID, bool hasClockConstraints] returns [Event evt = new Event(\"\")] : (ev= eventName[vars, true, defID] | ^( EVENT_NAME_NODE 'tau' ) | ^( CHANNEL_OUT_NODE name= ID (e= expression[vars, true, null] )? ) | ^( CHANNEL_IN_NODE name= ID (e= expression[vars, true, null] )? ) );
        [GrammarRule("eventT")]
        private Event eventT(List<string> vars, string defID, bool hasClockConstraints)
        {

            Event evt = new Event("");
            int eventT_StartIndex = input.Index;
            CommonTree name = null;
            Event ev = default(Event);
            Expression e = default(Expression);


            List<Expression> para = new List<Expression>(16);
            List<string> channelInputVars = new List<string>();

            try
            {
                DebugEnterRule(GrammarFileName, "eventT");
                DebugLocation(1934, 2);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 26)) { return evt; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1939:6: (ev= eventName[vars, true, defID] | ^( EVENT_NAME_NODE 'tau' ) | ^( CHANNEL_OUT_NODE name= ID (e= expression[vars, true, null] )? ) | ^( CHANNEL_IN_NODE name= ID (e= expression[vars, true, null] )? ) )
                    int alt50 = 4;
                    try
                    {
                        DebugEnterDecision(50, decisionCanBacktrack[50]);
                        switch (input.LA(1))
                        {
                            case EVENT_NAME_NODE:
                                {
                                    int LA50_1 = input.LA(2);

                                    if ((LA50_1 == DOWN))
                                    {
                                        int LA50_4 = input.LA(3);

                                        if ((LA50_4 == ID))
                                        {
                                            alt50 = 1;
                                        }
                                        else if ((LA50_4 == 80))
                                        {
                                            alt50 = 2;
                                        }
                                        else
                                        {
                                            if (state.backtracking > 0) { state.failed = true; return evt; }
                                            NoViableAltException nvae = new NoViableAltException("", 50, 4, input);

                                            DebugRecognitionException(nvae);
                                            throw nvae;
                                        }
                                    }
                                    else
                                    {
                                        if (state.backtracking > 0) { state.failed = true; return evt; }
                                        NoViableAltException nvae = new NoViableAltException("", 50, 1, input);

                                        DebugRecognitionException(nvae);
                                        throw nvae;
                                    }
                                }
                                break;
                            case CHANNEL_OUT_NODE:
                                {
                                    alt50 = 3;
                                }
                                break;
                            case CHANNEL_IN_NODE:
                                {
                                    alt50 = 4;
                                }
                                break;
                            default:
                                {
                                    if (state.backtracking > 0) { state.failed = true; return evt; }
                                    NoViableAltException nvae = new NoViableAltException("", 50, 0, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                        }

                    }
                    finally { DebugExitDecision(50); }
                    switch (alt50)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1939:9: ev= eventName[vars, true, defID]
                            {
                                DebugLocation(1939, 11);
                                PushFollow(Follow._eventName_in_eventT2639);
                                ev = eventName(vars, true, defID);
                                PopFollow();
                                if (state.failed) return evt;
                                DebugLocation(1940, 6);
                                if ((state.backtracking == 0))
                                {

                                    evt = ev;

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1943:8: ^( EVENT_NAME_NODE 'tau' )
                            {
                                DebugLocation(1943, 8);
                                DebugLocation(1943, 10);
                                Match(input, EVENT_NAME_NODE, Follow._EVENT_NAME_NODE_in_eventT2660); if (state.failed) return evt;

                                Match(input, TokenTypes.Down, null); if (state.failed) return evt;
                                DebugLocation(1943, 26);
                                Match(input, 80, Follow._80_in_eventT2662); if (state.failed) return evt;

                                Match(input, TokenTypes.Up, null); if (state.failed) return evt;
                                DebugLocation(1944, 6);
                                if ((state.backtracking == 0))
                                {

                                    evt.BaseName = Common.Classes.Ultility.Constants.TAU;
                                    evt.ExpressionList = new Expression[0];

                                }

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1948:8: ^( CHANNEL_OUT_NODE name= ID (e= expression[vars, true, null] )? )
                            {
                                DebugLocation(1948, 8);
                                DebugLocation(1948, 10);
                                Match(input, CHANNEL_OUT_NODE, Follow._CHANNEL_OUT_NODE_in_eventT2683); if (state.failed) return evt;

                                Match(input, TokenTypes.Down, null); if (state.failed) return evt;
                                DebugLocation(1948, 31);
                                name = (CommonTree)Match(input, ID, Follow._ID_in_eventT2687); if (state.failed) return evt;
                                DebugLocation(1948, 35);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1948:35: (e= expression[vars, true, null] )?
                                int alt48 = 2;
                                try
                                {
                                    DebugEnterSubRule(48);
                                    try
                                    {
                                        DebugEnterDecision(48, decisionCanBacktrack[48]);
                                        int LA48_0 = input.LA(1);

                                        if (((LA48_0 >= VAR_NODE && LA48_0 <= CALL_NODE) || LA48_0 == ASSIGNMENT_NODE || LA48_0 == UNARY_NODE || (LA48_0 >= CLASS_CALL_NODE && LA48_0 <= CLASS_CALL_INSTANCE_NODE) || LA48_0 == INT || LA48_0 == 76 || (LA48_0 >= 78 && LA48_0 <= 79) || (LA48_0 >= 102 && LA48_0 <= 104) || (LA48_0 >= 110 && LA48_0 <= 123) || LA48_0 == 127 || LA48_0 == 153))
                                        {
                                            alt48 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(48); }
                                    switch (alt48)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1948:36: e= expression[vars, true, null]
                                            {
                                                DebugLocation(1948, 37);
                                                PushFollow(Follow._expression_in_eventT2692);
                                                e = expression(vars, true, null);
                                                PopFollow();
                                                if (state.failed) return evt;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(48); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return evt;
                                DebugLocation(1949, 6);
                                if ((state.backtracking == 0))
                                {

                                    CheckChannelDeclared(name.Token);
                                    if (e != null)
                                    {
                                        e = e.ClearConstant(ConstantDatabase);
                                    }
                                    evt = new ChannelOutputEvent(name.Text, e);

                                }

                            }
                            break;
                        case 4:
                            DebugEnterAlt(4);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1957:6: ^( CHANNEL_IN_NODE name= ID (e= expression[vars, true, null] )? )
                            {
                                DebugLocation(1957, 6);
                                DebugLocation(1957, 8);
                                Match(input, CHANNEL_IN_NODE, Follow._CHANNEL_IN_NODE_in_eventT2711); if (state.failed) return evt;

                                Match(input, TokenTypes.Down, null); if (state.failed) return evt;
                                DebugLocation(1957, 28);
                                name = (CommonTree)Match(input, ID, Follow._ID_in_eventT2715); if (state.failed) return evt;
                                DebugLocation(1957, 32);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1957:32: (e= expression[vars, true, null] )?
                                int alt49 = 2;
                                try
                                {
                                    DebugEnterSubRule(49);
                                    try
                                    {
                                        DebugEnterDecision(49, decisionCanBacktrack[49]);
                                        int LA49_0 = input.LA(1);

                                        if (((LA49_0 >= VAR_NODE && LA49_0 <= CALL_NODE) || LA49_0 == ASSIGNMENT_NODE || LA49_0 == UNARY_NODE || (LA49_0 >= CLASS_CALL_NODE && LA49_0 <= CLASS_CALL_INSTANCE_NODE) || LA49_0 == INT || LA49_0 == 76 || (LA49_0 >= 78 && LA49_0 <= 79) || (LA49_0 >= 102 && LA49_0 <= 104) || (LA49_0 >= 110 && LA49_0 <= 123) || LA49_0 == 127 || LA49_0 == 153))
                                        {
                                            alt49 = 1;
                                        }
                                    }
                                    finally { DebugExitDecision(49); }
                                    switch (alt49)
                                    {
                                        case 1:
                                            DebugEnterAlt(1);
                                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1957:33: e= expression[vars, true, null]
                                            {
                                                DebugLocation(1957, 34);
                                                PushFollow(Follow._expression_in_eventT2720);
                                                e = expression(vars, true, null);
                                                PopFollow();
                                                if (state.failed) return evt;

                                            }
                                            break;

                                    }
                                }
                                finally { DebugExitSubRule(49); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return evt;
                                DebugLocation(1958, 2);
                                if ((state.backtracking == 0))
                                {

                                    CheckChannelDeclared(name.Token);

                                    if (e != null)
                                    {
                                        e = e.ClearConstant(ConstantDatabase);
                                    }
                                    evt = new ChannelInputEvent(name.Text, e);

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 26, eventT_StartIndex); }
                }
                DebugLocation(1967, 2);
            }
            finally { DebugExitRule(GrammarFileName, "eventT"); }
            return evt;

        }
        // $ANTLR end "eventT"


        protected virtual void Enter_processExpr() { }
        protected virtual void Leave_processExpr() { }

        // $ANTLR start "processExpr"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1969:1: processExpr[List<string> varsInit, String defID, List<string> sourceVars] returns [PetriNet proc = null] : ( ^( '|||' (p= processExpr[vars, defID, sourceVars] )+ ) | ^( ATOM_NODE ID (e= expression[vars, true, null] )* ) | ^( INTERLEAVE_NODE cond= paralDef2[vars, svars] p= processExpr[vars, defID, svars] ) );
        [GrammarRule("processExpr")]
        private PetriNet processExpr(List<string> varsInit, String defID, List<string> sourceVars)
        {

            PetriNet proc = null;
            int processExpr_StartIndex = input.Index;
            CommonTree ID21 = null;
            PetriNet p = default(PetriNet);
            Expression e = default(Expression);
            Expression cond = default(Expression);


            List<PetriNet> processes = new List<PetriNet>(16);
            List<Expression> para = new List<Expression>();
            List<string> vars = new List<string>();
            List<string> svars = new List<string>();

            if (varsInit != null)
            {
                vars.AddRange(varsInit);
                svars.AddRange(varsInit);
            }

            if (sourceVars != null)
            {
                svars.AddRange(sourceVars);
            }

            try
            {
                DebugEnterRule(GrammarFileName, "processExpr");
                DebugLocation(1969, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 27)) { return proc; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1988:2: ( ^( '|||' (p= processExpr[vars, defID, sourceVars] )+ ) | ^( ATOM_NODE ID (e= expression[vars, true, null] )* ) | ^( INTERLEAVE_NODE cond= paralDef2[vars, svars] p= processExpr[vars, defID, svars] ) )
                    int alt53 = 3;
                    try
                    {
                        DebugEnterDecision(53, decisionCanBacktrack[53]);
                        switch (input.LA(1))
                        {
                            case 139:
                                {
                                    alt53 = 1;
                                }
                                break;
                            case ATOM_NODE:
                                {
                                    alt53 = 2;
                                }
                                break;
                            case INTERLEAVE_NODE:
                                {
                                    alt53 = 3;
                                }
                                break;
                            default:
                                {
                                    if (state.backtracking > 0) { state.failed = true; return proc; }
                                    NoViableAltException nvae = new NoViableAltException("", 53, 0, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                        }

                    }
                    finally { DebugExitDecision(53); }
                    switch (alt53)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1988:4: ^( '|||' (p= processExpr[vars, defID, sourceVars] )+ )
                            {
                                DebugLocation(1988, 4);
                                DebugLocation(1988, 6);
                                Match(input, 139, Follow._139_in_processExpr2760); if (state.failed) return proc;

                                Match(input, TokenTypes.Down, null); if (state.failed) return proc;
                                DebugLocation(1988, 12);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1988:12: (p= processExpr[vars, defID, sourceVars] )+
                                int cnt51 = 0;
                                try
                                {
                                    DebugEnterSubRule(51);
                                    while (true)
                                    {
                                        int alt51 = 2;
                                        try
                                        {
                                            DebugEnterDecision(51, decisionCanBacktrack[51]);
                                            int LA51_0 = input.LA(1);

                                            if ((LA51_0 == INTERLEAVE_NODE || LA51_0 == ATOM_NODE || LA51_0 == 139))
                                            {
                                                alt51 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(51); }
                                        switch (alt51)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1988:13: p= processExpr[vars, defID, sourceVars]
                                                {
                                                    DebugLocation(1988, 14);
                                                    PushFollow(Follow._processExpr_in_processExpr2765);
                                                    p = processExpr(vars, defID, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return proc;
                                                    DebugLocation(1988, 52);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        processes.Add(p);
                                                    }

                                                }
                                                break;

                                            default:
                                                if (cnt51 >= 1)
                                                    goto loop51;

                                                if (state.backtracking > 0) { state.failed = true; return proc; }
                                                EarlyExitException eee51 = new EarlyExitException(51, input);
                                                DebugRecognitionException(eee51);
                                                throw eee51;
                                        }
                                        cnt51++;
                                    }
                                loop51:
                                    ;

                                }
                                finally { DebugExitSubRule(51); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return proc;
                                DebugLocation(1989, 2);
                                if ((state.backtracking == 0))
                                {

                                    // Tinh comment following line
                                    // proc = new IndexInterleave(processes);

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1993:5: ^( ATOM_NODE ID (e= expression[vars, true, null] )* )
                            {
                                DebugLocation(1993, 5);
                                DebugLocation(1993, 7);
                                Match(input, ATOM_NODE, Follow._ATOM_NODE_in_processExpr2783); if (state.failed) return proc;

                                Match(input, TokenTypes.Down, null); if (state.failed) return proc;
                                DebugLocation(1993, 17);
                                ID21 = (CommonTree)Match(input, ID, Follow._ID_in_processExpr2785); if (state.failed) return proc;
                                DebugLocation(1993, 20);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1993:20: (e= expression[vars, true, null] )*
                                try
                                {
                                    DebugEnterSubRule(52);
                                    while (true)
                                    {
                                        int alt52 = 2;
                                        try
                                        {
                                            DebugEnterDecision(52, decisionCanBacktrack[52]);
                                            int LA52_0 = input.LA(1);

                                            if (((LA52_0 >= VAR_NODE && LA52_0 <= CALL_NODE) || LA52_0 == ASSIGNMENT_NODE || LA52_0 == UNARY_NODE || (LA52_0 >= CLASS_CALL_NODE && LA52_0 <= CLASS_CALL_INSTANCE_NODE) || LA52_0 == INT || LA52_0 == 76 || (LA52_0 >= 78 && LA52_0 <= 79) || (LA52_0 >= 102 && LA52_0 <= 104) || (LA52_0 >= 110 && LA52_0 <= 123) || LA52_0 == 127 || LA52_0 == 153))
                                            {
                                                alt52 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(52); }
                                        switch (alt52)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:1993:22: e= expression[vars, true, null]
                                                {
                                                    DebugLocation(1993, 23);
                                                    PushFollow(Follow._expression_in_processExpr2791);
                                                    e = expression(vars, true, null);
                                                    PopFollow();
                                                    if (state.failed) return proc;
                                                    DebugLocation(1993, 53);
                                                    if ((state.backtracking == 0))
                                                    {
                                                        para.Add(e);
                                                    }

                                                }
                                                break;

                                            default:
                                                goto loop52;
                                        }
                                    }

                                loop52:
                                    ;

                                }
                                finally { DebugExitSubRule(52); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return proc;
                                DebugLocation(1994, 2);
                                if ((state.backtracking == 0))
                                {


                                    if (CurrentDefinition != null)
                                    {
                                        if (!Spec.PNDefinitionDatabase.ContainsKey(ID21.Text))
                                        {
                                            throw new ParsingException("Only LTS Processes Can Be Referenced!", ID21.Token);
                                        }
                                    }

                                    proc = new DefinitionRef(ID21.Text, para.ToArray());
                                    dlist.Add((DefinitionRef)proc);
                                    dtokens.Add(ID21.Token);

                                }

                            }
                            break;
                        case 3:
                            DebugEnterAlt(3);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2008:4: ^( INTERLEAVE_NODE cond= paralDef2[vars, svars] p= processExpr[vars, defID, svars] )
                            {
                                DebugLocation(2008, 4);
                                DebugLocation(2008, 6);
                                Match(input, INTERLEAVE_NODE, Follow._INTERLEAVE_NODE_in_processExpr2807); if (state.failed) return proc;

                                Match(input, TokenTypes.Down, null); if (state.failed) return proc;
                                DebugLocation(2008, 26);
                                PushFollow(Follow._paralDef2_in_processExpr2811);
                                cond = paralDef2(vars, svars);
                                PopFollow();
                                if (state.failed) return proc;
                                DebugLocation(2008, 51);
                                PushFollow(Follow._processExpr_in_processExpr2816);
                                p = processExpr(vars, defID, svars);
                                PopFollow();
                                if (state.failed) return proc;

                                Match(input, TokenTypes.Up, null); if (state.failed) return proc;
                                DebugLocation(2009, 2);
                                if ((state.backtracking == 0))
                                {

                                    if (cond == null)
                                    {
                                        // Tinh comment following line
                                        // proc = new IndexInterleaveAbstract(p, -1);
                                    }
                                    else
                                    {
                                        //proc = (new IndexInterleaveAbstractTemp(p, cond)).ClearConstant(ConstantDatabase);
                                        // Tinh comment following line
                                        // proc = (new IndexInterleaveAbstract(p, cond)).ClearConstant(ConstantDatabase);
                                    }

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 27, processExpr_StartIndex); }
                }
                DebugLocation(2022, 1);
            }
            finally { DebugExitRule(GrammarFileName, "processExpr"); }
            return proc;

        }
        // $ANTLR end "processExpr"


        protected virtual void Enter_paralDef() { }
        protected virtual void Leave_paralDef() { }

        // $ANTLR start "paralDef"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2023:1: paralDef[List<string> vars, List<string> sourceVars] returns [ParallelDefinition pd = new ParallelDefinition()] : ( ^( PARADEF_NODE id1= ID (int1= conditionalOrExpression[vars, true, sourceVars] )+ ) | ^( PARADEF1_NODE id1= ID int1= conditionalOrExpression[vars, true, sourceVars] int2= conditionalOrExpression[vars, true, sourceVars] ) );
        [GrammarRule("paralDef")]
        private ParallelDefinition paralDef(List<string> vars, List<string> sourceVars)
        {

            ParallelDefinition pd = new ParallelDefinition();
            int paralDef_StartIndex = input.Index;
            CommonTree id1 = null;
            Expression int1 = default(Expression);
            Expression int2 = default(Expression);

            try
            {
                DebugEnterRule(GrammarFileName, "paralDef");
                DebugLocation(2023, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 28)) { return pd; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2024:2: ( ^( PARADEF_NODE id1= ID (int1= conditionalOrExpression[vars, true, sourceVars] )+ ) | ^( PARADEF1_NODE id1= ID int1= conditionalOrExpression[vars, true, sourceVars] int2= conditionalOrExpression[vars, true, sourceVars] ) )
                    int alt55 = 2;
                    try
                    {
                        DebugEnterDecision(55, decisionCanBacktrack[55]);
                        int LA55_0 = input.LA(1);

                        if ((LA55_0 == PARADEF_NODE))
                        {
                            alt55 = 1;
                        }
                        else if ((LA55_0 == PARADEF1_NODE))
                        {
                            alt55 = 2;
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return pd; }
                            NoViableAltException nvae = new NoViableAltException("", 55, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(55); }
                    switch (alt55)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2024:4: ^( PARADEF_NODE id1= ID (int1= conditionalOrExpression[vars, true, sourceVars] )+ )
                            {
                                DebugLocation(2024, 4);
                                DebugLocation(2025, 3);
                                Match(input, PARADEF_NODE, Follow._PARADEF_NODE_in_paralDef2844); if (state.failed) return pd;

                                Match(input, TokenTypes.Down, null); if (state.failed) return pd;
                                DebugLocation(2026, 6);
                                id1 = (CommonTree)Match(input, ID, Follow._ID_in_paralDef2851); if (state.failed) return pd;
                                DebugLocation(2027, 3);
                                if ((state.backtracking == 0))
                                {

                                    CheckIDNameDefined(vars, id1.Token);
                                    pd = new ParallelDefinition(id1.Text, id1.Token);


                                }
                                DebugLocation(2032, 3);
                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2032:3: (int1= conditionalOrExpression[vars, true, sourceVars] )+
                                int cnt54 = 0;
                                try
                                {
                                    DebugEnterSubRule(54);
                                    while (true)
                                    {
                                        int alt54 = 2;
                                        try
                                        {
                                            DebugEnterDecision(54, decisionCanBacktrack[54]);
                                            int LA54_0 = input.LA(1);

                                            if (((LA54_0 >= VAR_NODE && LA54_0 <= CALL_NODE) || LA54_0 == UNARY_NODE || (LA54_0 >= CLASS_CALL_NODE && LA54_0 <= CLASS_CALL_INSTANCE_NODE) || LA54_0 == INT || LA54_0 == 76 || (LA54_0 >= 78 && LA54_0 <= 79) || (LA54_0 >= 102 && LA54_0 <= 104) || (LA54_0 >= 110 && LA54_0 <= 123) || LA54_0 == 127 || LA54_0 == 153))
                                            {
                                                alt54 = 1;
                                            }


                                        }
                                        finally { DebugExitDecision(54); }
                                        switch (alt54)
                                        {
                                            case 1:
                                                DebugEnterAlt(1);
                                                // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2033:11: int1= conditionalOrExpression[vars, true, sourceVars]
                                                {
                                                    DebugLocation(2033, 16);
                                                    PushFollow(Follow._conditionalOrExpression_in_paralDef2879);
                                                    int1 = conditionalOrExpression(vars, true, sourceVars);
                                                    PopFollow();
                                                    if (state.failed) return pd;
                                                    DebugLocation(2034, 11);
                                                    if ((state.backtracking == 0))
                                                    {

                                                        if (ConstantDatabase.Count > 0)
                                                        {
                                                            int1 = int1.ClearConstant(ConstantDatabase);
                                                        }

                                                        Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int1, vars, (CommonTree)id1.Parent.GetChild(pd.Domain.Count + 1), id1.Token);

                                                        pd.Domain.Add(int1);

                                                    }

                                                }
                                                break;

                                            default:
                                                if (cnt54 >= 1)
                                                    goto loop54;

                                                if (state.backtracking > 0) { state.failed = true; return pd; }
                                                EarlyExitException eee54 = new EarlyExitException(54, input);
                                                DebugRecognitionException(eee54);
                                                throw eee54;
                                        }
                                        cnt54++;
                                    }
                                loop54:
                                    ;

                                }
                                finally { DebugExitSubRule(54); }


                                Match(input, TokenTypes.Up, null); if (state.failed) return pd;

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2047:2: ^( PARADEF1_NODE id1= ID int1= conditionalOrExpression[vars, true, sourceVars] int2= conditionalOrExpression[vars, true, sourceVars] )
                            {
                                DebugLocation(2047, 2);
                                DebugLocation(2047, 4);
                                Match(input, PARADEF1_NODE, Follow._PARADEF1_NODE_in_paralDef2914); if (state.failed) return pd;

                                Match(input, TokenTypes.Down, null); if (state.failed) return pd;
                                DebugLocation(2047, 21);
                                id1 = (CommonTree)Match(input, ID, Follow._ID_in_paralDef2918); if (state.failed) return pd;
                                DebugLocation(2047, 30);
                                PushFollow(Follow._conditionalOrExpression_in_paralDef2923);
                                int1 = conditionalOrExpression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return pd;
                                DebugLocation(2047, 83);
                                PushFollow(Follow._conditionalOrExpression_in_paralDef2928);
                                int2 = conditionalOrExpression(vars, true, sourceVars);
                                PopFollow();
                                if (state.failed) return pd;

                                Match(input, TokenTypes.Up, null); if (state.failed) return pd;
                                DebugLocation(2048, 2);
                                if ((state.backtracking == 0))
                                {


                                    CheckIDNameDefined(vars, id1.Token);
                                    pd = new ParallelDefinition(id1.Text, id1.Token);

                                    if (ConstantDatabase.Count > 0)
                                    {
                                        int1 = int1.ClearConstant(ConstantDatabase);
                                        int2 = int2.ClearConstant(ConstantDatabase);
                                    }

                                    Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int1, vars, (CommonTree)id1.Parent.GetChild(1), id1.Token);
                                    Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int2, vars, (CommonTree)id1.Parent.GetChild(2), id1.Token);

                                    pd.LowerBound = int1;
                                    pd.UpperBound = int2;

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 28, paralDef_StartIndex); }
                }
                DebugLocation(2065, 1);
            }
            finally { DebugExitRule(GrammarFileName, "paralDef"); }
            return pd;

        }
        // $ANTLR end "paralDef"


        protected virtual void Enter_paralDef2() { }
        protected virtual void Leave_paralDef2() { }

        // $ANTLR start "paralDef2"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2067:1: paralDef2[List<string> vars, List<string> sourceVars] returns [Expression expr = null] : ^( PARADEF2_NODE (e= conditionalOrExpression[vars, true, sourceVars] )? ) ;
        [GrammarRule("paralDef2")]
        private Expression paralDef2(List<string> vars, List<string> sourceVars)
        {

            Expression expr = null;
            int paralDef2_StartIndex = input.Index;
            CommonTree PARADEF2_NODE22 = null;
            Expression e = default(Expression);

            try
            {
                DebugEnterRule(GrammarFileName, "paralDef2");
                DebugLocation(2067, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 29)) { return expr; }
                    DebugEnterAlt(1);
                    {
                        DebugLocation(2068, 4);
                        DebugLocation(2068, 6);
                        PARADEF2_NODE22 = (CommonTree)Match(input, PARADEF2_NODE, Follow._PARADEF2_NODE_in_paralDef22957); if (state.failed) return expr;

                        if (input.LA(1) == TokenTypes.Down)
                        {
                            Match(input, TokenTypes.Down, null); if (state.failed) return expr;
                            DebugLocation(2068, 21);
                            int alt56 = 2;
                            try
                            {
                                DebugEnterSubRule(56);
                                try
                                {
                                    DebugEnterDecision(56, decisionCanBacktrack[56]);
                                    int LA56_0 = input.LA(1);

                                    if (((LA56_0 >= VAR_NODE && LA56_0 <= CALL_NODE) || LA56_0 == UNARY_NODE || (LA56_0 >= CLASS_CALL_NODE && LA56_0 <= CLASS_CALL_INSTANCE_NODE) || LA56_0 == INT || LA56_0 == 76 || (LA56_0 >= 78 && LA56_0 <= 79) || (LA56_0 >= 102 && LA56_0 <= 104) || (LA56_0 >= 110 && LA56_0 <= 123) || LA56_0 == 127 || LA56_0 == 153))
                                    {
                                        alt56 = 1;
                                    }
                                }
                                finally { DebugExitDecision(56); }
                                switch (alt56)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:0:0: e= conditionalOrExpression[vars, true, sourceVars]
                                        {
                                            DebugLocation(2068, 21);
                                            PushFollow(Follow._conditionalOrExpression_in_paralDef22961);
                                            e = conditionalOrExpression(vars, true, sourceVars);
                                            PopFollow();
                                            if (state.failed) return expr;

                                        }
                                        break;

                                }
                            }
                            finally { DebugExitSubRule(56); }


                            Match(input, TokenTypes.Up, null); if (state.failed) return expr;
                        }
                        DebugLocation(2069, 2);
                        if ((state.backtracking == 0))
                        {

                            if (e != null)
                            {
                                IToken token = PAT.Common.Ultility.ParsingUltility.GetExpressionToken(PARADEF2_NODE22.Children[0] as CommonTree, input);
                                PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e, token, "in interleave abstract process", Spec.SpecValuation, ConstantDatabase);

                                if (ConstantDatabase.Count > 0)
                                {
                                    e = e.ClearConstant(ConstantDatabase);
                                }

                                Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(e, vars, (CommonTree)PARADEF2_NODE22.Children[0] as CommonTree, null);

                            }
                            expr = e;

                        }

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 29, paralDef2_StartIndex); }
                }
                DebugLocation(2085, 1);
            }
            finally { DebugExitRule(GrammarFileName, "paralDef2"); }
            return expr;

        }
        // $ANTLR end "paralDef2"


        protected virtual void Enter_eventM() { }
        protected virtual void Leave_eventM() { }

        // $ANTLR start "eventM"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2088:1: eventM[List<string> vars, string defID] returns [Event evt = new Event(\"\")] : (e= eventName[vars, true, defID] | ^( EVENT_NAME_NODE 'tau' ) );
        [GrammarRule("eventM")]
        private Event eventM(List<string> vars, string defID)
        {

            Event evt = new Event("");
            int eventM_StartIndex = input.Index;
            Event e = default(Event);

            try
            {
                DebugEnterRule(GrammarFileName, "eventM");
                DebugLocation(2088, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 30)) { return evt; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2089:2: (e= eventName[vars, true, defID] | ^( EVENT_NAME_NODE 'tau' ) )
                    int alt57 = 2;
                    try
                    {
                        DebugEnterDecision(57, decisionCanBacktrack[57]);
                        int LA57_0 = input.LA(1);

                        if ((LA57_0 == EVENT_NAME_NODE))
                        {
                            int LA57_1 = input.LA(2);

                            if ((LA57_1 == DOWN))
                            {
                                int LA57_2 = input.LA(3);

                                if ((LA57_2 == ID))
                                {
                                    alt57 = 1;
                                }
                                else if ((LA57_2 == 80))
                                {
                                    alt57 = 2;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return evt; }
                                    NoViableAltException nvae = new NoViableAltException("", 57, 2, input);

                                    DebugRecognitionException(nvae);
                                    throw nvae;
                                }
                            }
                            else
                            {
                                if (state.backtracking > 0) { state.failed = true; return evt; }
                                NoViableAltException nvae = new NoViableAltException("", 57, 1, input);

                                DebugRecognitionException(nvae);
                                throw nvae;
                            }
                        }
                        else
                        {
                            if (state.backtracking > 0) { state.failed = true; return evt; }
                            NoViableAltException nvae = new NoViableAltException("", 57, 0, input);

                            DebugRecognitionException(nvae);
                            throw nvae;
                        }
                    }
                    finally { DebugExitDecision(57); }
                    switch (alt57)
                    {
                        case 1:
                            DebugEnterAlt(1);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2089:5: e= eventName[vars, true, defID]
                            {
                                DebugLocation(2089, 6);
                                PushFollow(Follow._eventName_in_eventM2990);
                                e = eventName(vars, true, defID);
                                PopFollow();
                                if (state.failed) return evt;
                                DebugLocation(2090, 2);
                                if ((state.backtracking == 0))
                                {

                                    evt = e;

                                }

                            }
                            break;
                        case 2:
                            DebugEnterAlt(2);
                            // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2117:4: ^( EVENT_NAME_NODE 'tau' )
                            {
                                DebugLocation(2117, 4);
                                DebugLocation(2117, 6);
                                Match(input, EVENT_NAME_NODE, Follow._EVENT_NAME_NODE_in_eventM3005); if (state.failed) return evt;

                                Match(input, TokenTypes.Down, null); if (state.failed) return evt;
                                DebugLocation(2117, 22);
                                Match(input, 80, Follow._80_in_eventM3007); if (state.failed) return evt;

                                Match(input, TokenTypes.Up, null); if (state.failed) return evt;
                                DebugLocation(2118, 2);
                                if ((state.backtracking == 0))
                                {

                                    evt.BaseName = Common.Classes.Ultility.Constants.TAU;
                                    evt.ExpressionList = new Expression[0];

                                }

                            }
                            break;

                    }
                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 30, eventM_StartIndex); }
                }
                DebugLocation(2122, 1);
            }
            finally { DebugExitRule(GrammarFileName, "eventM"); }
            return evt;

        }
        // $ANTLR end "eventM"


        protected virtual void Enter_eventName() { }
        protected virtual void Leave_eventName() { }

        // $ANTLR start "eventName"
        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2124:1: eventName[List<string> vars, bool checkVar, String defID] returns [Event e = new Event(\"\")] : ^( EVENT_NAME_NODE ID (ex= conditionalOrExpression[vars, checkVar, null] )* ) ;
        [GrammarRule("eventName")]
        private Event eventName(List<string> vars, bool checkVar, String defID)
        {

            Event e = new Event("");
            int eventName_StartIndex = input.Index;
            CommonTree ID23 = null;
            Expression ex = default(Expression);


            List<Expression> ExpressionList = new List<Expression>();

            try
            {
                DebugEnterRule(GrammarFileName, "eventName");
                DebugLocation(2124, 1);
                try
                {
                    if (state.backtracking > 0 && AlreadyParsedRule(input, 31)) { return e; }
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2128:2: ( ^( EVENT_NAME_NODE ID (ex= conditionalOrExpression[vars, checkVar, null] )* ) )
                    DebugEnterAlt(1);
                    // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2128:4: ^( EVENT_NAME_NODE ID (ex= conditionalOrExpression[vars, checkVar, null] )* )
                    {
                        DebugLocation(2128, 4);
                        DebugLocation(2128, 6);
                        Match(input, EVENT_NAME_NODE, Follow._EVENT_NAME_NODE_in_eventName3036); if (state.failed) return e;

                        Match(input, TokenTypes.Down, null); if (state.failed) return e;
                        DebugLocation(2128, 22);
                        ID23 = (CommonTree)Match(input, ID, Follow._ID_in_eventName3038); if (state.failed) return e;
                        DebugLocation(2129, 3);
                        if ((state.backtracking == 0))
                        {

                            if (ID23.Text == "init") { throw new ParsingException("init is an internal event. Please use other names!", ID23.Token); }
                            if (ID23.Text == "<missing ID>") { ID23.Token.Text = " "; throw new ParsingException("Please input at least one event!", ID23.Token); }

                        }
                        DebugLocation(2133, 3);
                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2133:3: (ex= conditionalOrExpression[vars, checkVar, null] )*
                        try
                        {
                            DebugEnterSubRule(58);
                            while (true)
                            {
                                int alt58 = 2;
                                try
                                {
                                    DebugEnterDecision(58, decisionCanBacktrack[58]);
                                    int LA58_0 = input.LA(1);

                                    if (((LA58_0 >= VAR_NODE && LA58_0 <= CALL_NODE) || LA58_0 == UNARY_NODE || (LA58_0 >= CLASS_CALL_NODE && LA58_0 <= CLASS_CALL_INSTANCE_NODE) || LA58_0 == INT || LA58_0 == 76 || (LA58_0 >= 78 && LA58_0 <= 79) || (LA58_0 >= 102 && LA58_0 <= 104) || (LA58_0 >= 110 && LA58_0 <= 123) || LA58_0 == 127 || LA58_0 == 153))
                                    {
                                        alt58 = 1;
                                    }


                                }
                                finally { DebugExitDecision(58); }
                                switch (alt58)
                                {
                                    case 1:
                                        DebugEnterAlt(1);
                                        // E:\\Dropbox\\PAT\\pat-tl\\PAT.Module.PN\\LTS\\PNTreeWalker.g:2133:9: ex= conditionalOrExpression[vars, checkVar, null]
                                        {
                                            DebugLocation(2133, 11);
                                            PushFollow(Follow._conditionalOrExpression_in_eventName3055);
                                            ex = conditionalOrExpression(vars, checkVar, null);
                                            PopFollow();
                                            if (state.failed) return e;
                                            DebugLocation(2134, 4);
                                            if ((state.backtracking == 0))
                                            {

                                                ex = ex.ClearConstant(ConstantDatabase);
                                                ExpressionList.Add(ex);
                                                if (ex.HasVar)
                                                {
                                                    if (CurrentDefinition != null)
                                                    {
                                                        CurrentDefinition.AlphabetsCalculable = false;
                                                    }

                                                    CurrentLTSGraphAlphabetsCalculable = false;
                                                }

                                                Common.Ultility.ParsingUltility.TestIsNonVoidExpression(ex, ID23.Token, Spec.SpecValuation);

                                            }

                                        }
                                        break;

                                    default:
                                        goto loop58;
                                }
                            }

                        loop58:
                            ;

                        }
                        finally { DebugExitSubRule(58); }


                        Match(input, TokenTypes.Up, null); if (state.failed) return e;
                        DebugLocation(2151, 2);
                        if ((state.backtracking == 0))
                        {

                            bool channelNameAsEvent = false;
                            foreach (IToken name in ChannelNames)
                            {
                                if (name.Text == ID23.Text)
                                {
                                    channelNameAsEvent = true;
                                }
                            }

                            if (!channelNameAsEvent)
                            {
                                CheckIDNameDefined(vars, ID23.Token);
                            }

                            e.BaseName = ID23.Text;
                            e.ExpressionList = ExpressionList.ToArray();
                            eventsTokens.Add(ID23.Token);

                        }

                    }

                }

                catch (RecognitionException re)
                {
                    Spec.ClearDatabase();
                    string ss = GetErrorMessage(re, tokenNames);
                    throw new ParsingException(ss, re.Token);
                }
                finally
                {
                    if (state.backtracking > 0) { Memoize(input, 31, eventName_StartIndex); }
                }
                DebugLocation(2170, 1);
            }
            finally { DebugExitRule(GrammarFileName, "eventName"); }
            return e;

        }
        // $ANTLR end "eventName"

        #region DFA
        DFA6 dfa6;
        DFA34 dfa34;

        protected override void InitDFAs()
        {
            base.InitDFAs();
            dfa6 = new DFA6(this);
            dfa34 = new DFA34(this);
        }

        private class DFA6 : DFA
        {
            private const string DFA6_eotS =
                "\x0b\uffff";
            private const string DFA6_eofS =
                "\x0b\uffff";
            private const string DFA6_minS =
                "\x01\x47\x06\uffff\x01\x04\x03\uffff";
            private const string DFA6_maxS =
                "\x01\x5b\x06\uffff\x01\x5d\x03\uffff";
            private const string DFA6_acceptS =
                "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01\uffff" +
                "\x01\x08\x01\x09\x01\x07";
            private const string DFA6_specialS =
                "\x0b\uffff}>";
            private static readonly string[] DFA6_transitionS =
			{
				"\x01\x01\x0e\uffff\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01\x07",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x01\x0a\x57\uffff\x01\x08\x01\x09",
				"",
				"",
				""
			};

            private static readonly short[] DFA6_eot = DFA.UnpackEncodedString(DFA6_eotS);
            private static readonly short[] DFA6_eof = DFA.UnpackEncodedString(DFA6_eofS);
            private static readonly char[] DFA6_min = DFA.UnpackEncodedStringToUnsignedChars(DFA6_minS);
            private static readonly char[] DFA6_max = DFA.UnpackEncodedStringToUnsignedChars(DFA6_maxS);
            private static readonly short[] DFA6_accept = DFA.UnpackEncodedString(DFA6_acceptS);
            private static readonly short[] DFA6_special = DFA.UnpackEncodedString(DFA6_specialS);
            private static readonly short[][] DFA6_transition;

            static DFA6()
            {
                int numStates = DFA6_transitionS.Length;
                DFA6_transition = new short[numStates][];
                for (int i = 0; i < numStates; i++)
                {
                    DFA6_transition[i] = DFA.UnpackEncodedString(DFA6_transitionS[i]);
                }
            }

            public DFA6(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 6;
                this.eot = DFA6_eot;
                this.eof = DFA6_eof;
                this.min = DFA6_min;
                this.max = DFA6_max;
                this.accept = DFA6_accept;
                this.special = DFA6_special;
                this.transition = DFA6_transition;
            }

            public override string Description { get { return "707:2: ( ( '|=' (tk= ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\\\' | '\\/' | '.' | INT ) )+ ) | ( 'deadlockfree' ) | ( 'nonterminating' ) | ( 'divergencefree' ) | ( 'deterministic' ) | ( 'reaches' label= ID (exp= withClause[out contraint] )? ) | ( 'refines' targetProcess= definitionRef ) | ( 'refines' '<F>' targetProcess= definitionRef ) | ( 'refines' '<FD>' targetProcess= definitionRef ) )"; } }

            public override void Error(NoViableAltException nvae)
            {
                DebugRecognitionException(nvae);
            }
        }

        private class DFA34 : DFA
        {
            private const string DFA34_eotS =
                "\x21\uffff";
            private const string DFA34_eofS =
                "\x21\uffff";
            private const string DFA34_minS =
                "\x01\x07\x12\uffff\x01\x02\x09\uffff\x01\x3d\x01\x03\x02\uffff";
            private const string DFA34_maxS =
                "\x01\u0099\x12\uffff\x01\x02\x09\uffff\x01\x3d\x01\u0099\x02\uffff";
            private const string DFA34_acceptS =
                "\x01\uffff\x01\x01\x01\x02\x01\x03\x01\x04\x01\x05\x01\x06\x01\x07\x01" +
                "\x08\x01\x09\x01\x0a\x01\x0b\x01\x0c\x01\x0d\x01\x0e\x01\x0f\x01\x10" +
                "\x01\x11\x01\x12\x01\uffff\x01\x15\x01\x16\x01\x17\x01\x18\x01\x19\x01" +
                "\x1a\x01\x1b\x01\x1c\x01\x1d\x02\uffff\x01\x13\x01\x14";
            private const string DFA34_specialS =
                "\x21\uffff}>";
            private static readonly string[] DFA34_transitionS =
			{
				"\x01\x13\x01\x19\x24\uffff\x01\x12\x06\uffff\x01\x1a\x01\x1b\x08\uffff"+
				"\x01\x14\x0d\uffff\x01\x17\x01\uffff\x01\x02\x01\x01\x16\uffff\x01\x0e"+
				"\x01\x15\x01\x16\x05\uffff\x01\x03\x01\x05\x01\x04\x01\x06\x01\x07\x01"+
				"\x08\x01\x09\x01\x0a\x01\x0b\x01\x0c\x01\x0d\x01\x0f\x01\x10\x01\x11"+
				"\x03\uffff\x01\x1c\x19\uffff\x01\x18",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x01\x1d",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x01\x1e",
				"\x01\x1f\x03\uffff\x02\x20\x24\uffff\x01\x20\x06\uffff\x02\x20\x08"+
				"\uffff\x01\x20\x0d\uffff\x01\x20\x01\uffff\x02\x20\x16\uffff\x03\x20"+
				"\x05\uffff\x0e\x20\x03\uffff\x01\x20\x19\uffff\x01\x20",
				"",
				""
			};

            private static readonly short[] DFA34_eot = DFA.UnpackEncodedString(DFA34_eotS);
            private static readonly short[] DFA34_eof = DFA.UnpackEncodedString(DFA34_eofS);
            private static readonly char[] DFA34_min = DFA.UnpackEncodedStringToUnsignedChars(DFA34_minS);
            private static readonly char[] DFA34_max = DFA.UnpackEncodedStringToUnsignedChars(DFA34_maxS);
            private static readonly short[] DFA34_accept = DFA.UnpackEncodedString(DFA34_acceptS);
            private static readonly short[] DFA34_special = DFA.UnpackEncodedString(DFA34_specialS);
            private static readonly short[][] DFA34_transition;

            static DFA34()
            {
                int numStates = DFA34_transitionS.Length;
                DFA34_transition = new short[numStates][];
                for (int i = 0; i < numStates; i++)
                {
                    DFA34_transition[i] = DFA.UnpackEncodedString(DFA34_transitionS[i]);
                }
            }

            public DFA34(BaseRecognizer recognizer)
            {
                this.recognizer = recognizer;
                this.decisionNumber = 34;
                this.eot = DFA34_eot;
                this.eof = DFA34_eof;
                this.min = DFA34_min;
                this.max = DFA34_max;
                this.accept = DFA34_accept;
                this.special = DFA34_special;
                this.transition = DFA34_transition;
            }

            public override string Description { get { return "1488:1: conditionalOrExpression[List<string> vars, bool check, List<string> sourceVars] returns [Expression exp = null] : ( ^( '||' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'xor' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '|' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '&' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '^' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '==' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '!=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '<=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( '>=' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '+' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '-' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '*' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '/' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^(opt= '%' e1= conditionalOrExpression[vars, check, sourceVars] e2= conditionalOrExpression[vars, check, sourceVars] ) | ^( UNARY_NODE e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( VAR_NODE ID ) | a1= arrayExpression[vars, check, sourceVars, varName] | INT | 'true' | 'false' | ^( '!' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( 'empty' e1= conditionalOrExpression[vars, check, sourceVars] ) | ^( CALL_NODE ( ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_NODE (var= ID ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( CLASS_CALL_INSTANCE_NODE (a1= arrayExpression[vars, check, sourceVars,varName] ) method= ID (e1= conditionalOrExpression[vars, check, sourceVars] )* ) | ^( 'new' (className= ID ) (e1= conditionalOrExpression[vars, check, sourceVars] )* ) );"; } }

            public override void Error(NoViableAltException nvae)
            {
                DebugRecognitionException(nvae);
            }
        }


        #endregion DFA

        #region Follow sets
        private static class Follow
        {
            public static readonly BitSet _specBody_in_specification92 = new BitSet(new ulong[] { 0x0040460000008602UL, 0x0000000000000040UL, 0x0000000000800000UL });
            public static readonly BitSet _letDefintion_in_specBody127 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _definition_in_specBody132 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _assertion_in_specBody138 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _alphabet_in_specBody144 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _define_in_specBody149 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _channel_in_specBody154 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _ALPHABET_NOTE_in_alphabet178 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_alphabet180 = new BitSet(new ulong[] { 0x0000000002000000UL });
            public static readonly BitSet _eventName_in_alphabet185 = new BitSet(new ulong[] { 0x0000000002000008UL });
            public static readonly BitSet _151_in_channel219 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_channel221 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_channel225 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _70_in_assertion256 = new BitSet(new ulong[] { 0x0000000000000010UL });
            public static readonly BitSet _definitionRef_in_assertion262 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x000000000FC00080UL });
            public static readonly BitSet _71_in_assertion277 = new BitSet(new ulong[] { 0x7000000000000000UL, 0x000000000037FF00UL, 0x0000000001000000UL });
            public static readonly BitSet _set_in_assertion285 = new BitSet(new ulong[] { 0x7000000000000002UL, 0x000000000037FF00UL, 0x0000000001000000UL });
            public static readonly BitSet _86_in_assertion408 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _87_in_assertion428 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _88_in_assertion452 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _89_in_assertion470 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _90_in_assertion491 = new BitSet(new ulong[] { 0x2000000000000000UL });
            public static readonly BitSet _ID_in_assertion496 = new BitSet(new ulong[] { 0x0000000000000002UL, 0x0000000040000000UL });
            public static readonly BitSet _withClause_in_assertion502 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _91_in_assertion530 = new BitSet(new ulong[] { 0x0000000000000010UL });
            public static readonly BitSet _definitionRef_in_assertion535 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _91_in_assertion557 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000010000000UL });
            public static readonly BitSet _92_in_assertion559 = new BitSet(new ulong[] { 0x0000000000000010UL });
            public static readonly BitSet _definitionRef_in_assertion564 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _91_in_assertion586 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000020000000UL });
            public static readonly BitSet _93_in_assertion588 = new BitSet(new ulong[] { 0x0000000000000010UL });
            public static readonly BitSet _definitionRef_in_assertion593 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _94_in_withClause653 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _95_in_withClause658 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _96_in_withClause667 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_withClause674 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _DEFINITION_REF_NODE_in_definitionRef708 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_definitionRef710 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_definitionRef718 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _DEFINE_CONSTANT_NODE_in_define760 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_define762 = new BitSet(new ulong[] { 0x4000000000000000UL });
            public static readonly BitSet _INT_in_define764 = new BitSet(new ulong[] { 0x0000000000000008UL, 0x0000004000000000UL });
            public static readonly BitSet _102_in_define769 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _DEFINE_CONSTANT_NODE_in_define781 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_define785 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000018000000000UL });
            public static readonly BitSet _set_in_define789 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _DEFINE_CONSTANT_NODE_in_define805 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_define812 = new BitSet(new ulong[] { 0x2000000000000008UL });
            public static readonly BitSet _DEFINE_NODE_in_define834 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_define836 = new BitSet(new ulong[] { 0x48332100000001A0UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _dparameter_in_define841 = new BitSet(new ulong[] { 0x48332100000001A0UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _statement_in_define849 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _DPARAMETER_NODE_in_dparameter879 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_dparameter882 = new BitSet(new ulong[] { 0x2000000000000008UL });
            public static readonly BitSet _BLOCK_NODE_in_block918 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _statement_in_block928 = new BitSet(new ulong[] { 0x48332100000001A8UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _block_in_statement963 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _localVariableDeclaration_in_statement979 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _ifExpression_in_statement995 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _whileExpression_in_statement1011 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _expression_in_statement1027 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _LOCAL_VAR_NODE_in_localVariableDeclaration1064 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_localVariableDeclaration1066 = new BitSet(new ulong[] { 0x4030210000001188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_localVariableDeclaration1078 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _recordExpression_in_localVariableDeclaration1087 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _LOCAL_ARRAY_NODE_in_localVariableDeclaration1105 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_localVariableDeclaration1107 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_localVariableDeclaration1119 = new BitSet(new ulong[] { 0x4030210000001188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _recordExpression_in_localVariableDeclaration1139 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _conditionalOrExpression_in_expression1180 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _ASSIGNMENT_NODE_in_expression1189 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_expression1193 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_expression1198 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _LET_NODE_in_letDefintion1231 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_letDefintion1235 = new BitSet(new ulong[] { 0x2000000000000000UL });
            public static readonly BitSet _ID_in_letDefintion1240 = new BitSet(new ulong[] { 0x4030A10000001188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _varaibleRange_in_letDefintion1242 = new BitSet(new ulong[] { 0x4030210000001188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_letDefintion1249 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _recordExpression_in_letDefintion1258 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _LET_ARRAY_NODE_in_letDefintion1278 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_letDefintion1281 = new BitSet(new ulong[] { 0x4030A10000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _varaibleRange_in_letDefintion1286 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_letDefintion1298 = new BitSet(new ulong[] { 0x4030210000001188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _recordExpression_in_letDefintion1319 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _VARIABLE_RANGE_NODE_in_varaibleRange1347 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_varaibleRange1353 = new BitSet(new ulong[] { 0x0000000000000000UL, 0x0000000000200000UL });
            public static readonly BitSet _85_in_varaibleRange1358 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_varaibleRange1363 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _79_in_conditionalOrExpression1394 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1398 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1403 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _78_in_conditionalOrExpression1413 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1417 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1422 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _110_in_conditionalOrExpression1432 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1436 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1441 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _112_in_conditionalOrExpression1451 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1455 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1460 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _111_in_conditionalOrExpression1470 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1474 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1479 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _113_in_conditionalOrExpression1489 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1493 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1498 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _114_in_conditionalOrExpression1508 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1512 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1517 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _115_in_conditionalOrExpression1528 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1532 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1537 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _116_in_conditionalOrExpression1550 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1554 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1559 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _117_in_conditionalOrExpression1573 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1577 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1582 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _118_in_conditionalOrExpression1597 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1601 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1606 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _119_in_conditionalOrExpression1620 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1624 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1629 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _120_in_conditionalOrExpression1647 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1651 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1656 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _102_in_conditionalOrExpression1674 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1678 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1683 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _121_in_conditionalOrExpression1703 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1707 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1712 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _122_in_conditionalOrExpression1730 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1734 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1739 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _123_in_conditionalOrExpression1760 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1764 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1769 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _UNARY_NODE_in_conditionalOrExpression1789 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1793 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _VAR_NODE_in_conditionalOrExpression1805 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1807 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _arrayExpression_in_conditionalOrExpression1818 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _INT_in_conditionalOrExpression1827 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _103_in_conditionalOrExpression1835 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _104_in_conditionalOrExpression1843 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _76_in_conditionalOrExpression1852 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1856 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _153_in_conditionalOrExpression1866 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1870 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _CALL_NODE_in_conditionalOrExpression1880 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1883 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1891 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _CLASS_CALL_NODE_in_conditionalOrExpression1908 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1913 = new BitSet(new ulong[] { 0x2000000000000000UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1920 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1925 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _CLASS_CALL_INSTANCE_NODE_in_conditionalOrExpression1942 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _arrayExpression_in_conditionalOrExpression1948 = new BitSet(new ulong[] { 0x2000000000000000UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1956 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1961 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _127_in_conditionalOrExpression1978 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_conditionalOrExpression1983 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_conditionalOrExpression1991 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _VAR_NODE_in_arrayExpression2031 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_arrayExpression2034 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_arrayExpression2045 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _131_in_ifExpression2097 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _expression_in_ifExpression2101 = new BitSet(new ulong[] { 0x48332100000001A0UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _statement_in_ifExpression2107 = new BitSet(new ulong[] { 0x48332100000001A8UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _statement_in_ifExpression2112 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _133_in_whileExpression2152 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _expression_in_whileExpression2156 = new BitSet(new ulong[] { 0x48332100000001A0UL, 0x8FFFC1C00000D000UL, 0x0000000002000028UL });
            public static readonly BitSet _statement_in_whileExpression2161 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _RECORD_NODE_in_recordExpression2214 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _recordElement_in_recordExpression2219 = new BitSet(new ulong[] { 0x0000000000006008UL });
            public static readonly BitSet _RECORD_ELEMENT_NODE_in_recordElement2251 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _expression_in_recordElement2255 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_recordElement2261 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _RECORD_ELEMENT_RANGE_NODE_in_recordElement2276 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _expression_in_recordElement2280 = new BitSet(new ulong[] { 0x4030210000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_recordElement2285 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _DEFINITION_NODE_in_definition2319 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_definition2323 = new BitSet(new ulong[] { 0x2000008000200000UL, 0x0000000000000000UL, 0x0000000000000800UL });
            public static readonly BitSet _ID_in_definition2328 = new BitSet(new ulong[] { 0x2000008000200000UL, 0x0000000000000000UL, 0x0000000000000800UL });
            public static readonly BitSet _processExpr_in_definition2341 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _PROCESS_NODE_in_definition2354 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _STRING_in_definition2358 = new BitSet(new ulong[] { 0x2080000000000008UL });
            public static readonly BitSet _ID_in_definition2363 = new BitSet(new ulong[] { 0x2080000000000008UL });
            public static readonly BitSet _transition_in_definition2396 = new BitSet(new ulong[] { 0x0080000000000008UL });
            public static readonly BitSet _PLACE_NODE_in_stateDef2436 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _STRING_in_stateDef2440 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _TRANSITION_NODE_in_transition2482 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _STRING_in_transition2486 = new BitSet(new ulong[] { 0x4430200302000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _select_in_transition2490 = new BitSet(new ulong[] { 0x4430200302000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_transition2503 = new BitSet(new ulong[] { 0x4430200302000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _eventT_in_transition2517 = new BitSet(new ulong[] { 0x1000000000000020UL });
            public static readonly BitSet _block_in_transition2531 = new BitSet(new ulong[] { 0x1000000000000000UL });
            public static readonly BitSet _STRING_in_transition2552 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _SELECT_NODE_in_select2591 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _paralDef_in_select2601 = new BitSet(new ulong[] { 0x0000001800000008UL });
            public static readonly BitSet _eventName_in_eventT2639 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _EVENT_NAME_NODE_in_eventT2660 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _80_in_eventT2662 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _CHANNEL_OUT_NODE_in_eventT2683 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_eventT2687 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_eventT2692 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _CHANNEL_IN_NODE_in_eventT2711 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_eventT2715 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_eventT2720 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _139_in_processExpr2760 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _processExpr_in_processExpr2765 = new BitSet(new ulong[] { 0x2000008000200008UL, 0x0000000000000000UL, 0x0000000000000800UL });
            public static readonly BitSet _ATOM_NODE_in_processExpr2783 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_processExpr2785 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _expression_in_processExpr2791 = new BitSet(new ulong[] { 0x4030210000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _INTERLEAVE_NODE_in_processExpr2807 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _paralDef2_in_processExpr2811 = new BitSet(new ulong[] { 0x2000008000200000UL, 0x0000000000000000UL, 0x0000000000000800UL });
            public static readonly BitSet _processExpr_in_processExpr2816 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _PARADEF_NODE_in_paralDef2844 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_paralDef2851 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_paralDef2879 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _PARADEF1_NODE_in_paralDef2914 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_paralDef2918 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_paralDef2923 = new BitSet(new ulong[] { 0x4030200000000180UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_paralDef2928 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _PARADEF2_NODE_in_paralDef22957 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _conditionalOrExpression_in_paralDef22961 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _eventName_in_eventM2990 = new BitSet(new ulong[] { 0x0000000000000002UL });
            public static readonly BitSet _EVENT_NAME_NODE_in_eventM3005 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _80_in_eventM3007 = new BitSet(new ulong[] { 0x0000000000000008UL });
            public static readonly BitSet _EVENT_NAME_NODE_in_eventName3036 = new BitSet(new ulong[] { 0x0000000000000004UL });
            public static readonly BitSet _ID_in_eventName3038 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });
            public static readonly BitSet _conditionalOrExpression_in_eventName3055 = new BitSet(new ulong[] { 0x4030200000000188UL, 0x8FFFC1C00000D000UL, 0x0000000002000000UL });

        }
        #endregion Follow sets
    }
}