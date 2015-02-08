tree grammar PNTreeWalker;

options
{
    tokenVocab=PNTree;
    ASTLabelType = CommonTree;
    language=CSharp2; 
    backtrack = true;
    memoize=true;
}

@namespace	{PAT.PN.LTS}

@header{
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
    }

@members{
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

    public void program(string opt){
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
    	if(Spec.DefinitionDatabase.Count == 0)
    	{
    		throw new ParsingException("Please enter at least one process definition.", 0, 0, ""); 
    	}	
    	
    	//each definition reference needs to be defined first, also the number of arguments must match
     	foreach (DefinitionRef def in dlist){
     		if (Spec.DefinitionDatabase.ContainsKey(def.Name)){
     			Definition d =  Spec.DefinitionDatabase[def.Name];
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
            	PetriNet d =  Spec.PNDefinitionDatabase[def.Name];
                if (def.Args.Length != d.Parameters.Count)
                    {
                        IToken token = dtokens[dlist.IndexOf(def)];
                        throw new ParsingException("Definition reference " + token.Text + " has different number of arguments from the definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
                    }
                //
				// def.GraphDef = d;
            }
     	}

     	foreach(string def in DefUsedInParallel)
        {
            if (Spec.DefinitionDatabase.ContainsKey(def))
            {
                Spec.DefinitionDatabase[def].UsedInParallel = true;
            }
        }

        foreach(IToken token in alphaTokens)
        {
        	if (!Spec.DefinitionDatabase.ContainsKey(token.Text) && !Spec.PNDefinitionDatabase.ContainsKey(token.Text))
            {                     	
               	throw new ParsingException("Undefined process definition: " + token.Text, token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
            }

            PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, token.Text, token);

            if (Spec.DefinitionDatabase.ContainsKey(token.Text)){
            	Definition def = Spec.DefinitionDatabase[token.Text];
            	List<string> localVars = new List<string>(def.LocalVariables);
            	EventCollection eventCollection = AlphaDatabase[token.Text];
            	foreach (Event evt in eventCollection){
            		if(evt.ExpressionList != null){
            			foreach (Expression exp in evt.ExpressionList){
            				List<string> evars = exp.GetVars();
            				foreach (string var in evars){
            					if (!localVars.Contains(var)){
            						throw new ParsingException("Alphabet definition for process " + token.Text + " has an event " + evt.BaseName + " with invalid variable " + var + ", which is not declared as a parameter of its process definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
            					}
            				}
            			}
            		}
            	}

            	def.AlphabetEvents = eventCollection;
            }
            else{
            	PetriNet def = Spec.PNDefinitionDatabase[token.Text];
            	List<string> localVars = new List<string>(def.Parameters);
            	EventCollection eventCollection = AlphaDatabase[token.Text];
            	foreach (Event evt in eventCollection){
            		if(evt.ExpressionList != null){
            			foreach (Expression exp in evt.ExpressionList){
            				List<string> evars = exp.GetVars();
            				foreach (string var in evars){
								if (!localVars.Contains(var)){
									throw new ParsingException("Alphabet definition for process " + token.Text + " has an event " + evt.BaseName + " with invalid variable " + var + ", which is not declared as a parameter of its process definition!", token); //+ " at line:" + token.Line + " col:" + token.CharPositionInLine
								}
							}
						}
					}
				}
            	def.AlphabetEvents = eventCollection;
            }
        }

        foreach(IToken token in ltlTokens){
        	if (!Spec.DeclarationDatabase.ContainsKey(token.Text)){
        		bool matchEvent = false;
        		if(token.Type != PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN){
        			foreach (IToken var in eventsTokens)
    	            {
    	                if (var.Text == token.Text)
    	                {
    	                    matchEvent = true;
    	                    break;
    	                }
    	            }
        		}

    	    	if(token.Type == PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN)
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
    		  	if(token.Type == PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN || token.Type == PAT.Common.Ultility.ParsingUltility.LTL_COMPOUND_EVENT)
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

               // Thuanle: Tinh remove this
               // if (ConstantDatabase.Count > 0)
               //{
               //     foreach (Definition entry in Spec.DefinitionDatabase.Values)
               //     {                      
               //         entry.ClearConstant(ConstantDatabase);
               //     }
               // }
       
        eventsTokens.Clear();
        dtokens.Clear();
    	alphaTokens.Clear();
    	declareTokens.Clear();
    	ltlTokens.Clear();
    }

    public override String GetErrorMessage(RecognitionException e, String[] tokenNames)
    {
        string msg = null;
        if ( e is NoViableAltException ) {
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

    private void CheckIDNameDefined(List<string> vars, IToken idToken){
        if(vars.Contains(idToken.Text))
        {
            throw new ParsingException("'" + idToken.Text + "' is already used as a parameter name in the containing definition. Please choose another name!", idToken);
        }

        foreach(KeyValuePair<string, int> item in LocalVariablesStack)
        {
            if(item.Key == idToken.Text)
            {
                throw new ParsingException("'" + idToken.Text + "' is already used as a local variable in the data operation program. Please choose another name!", idToken);
            }
        }

        foreach (IToken name in DefinitionNames)
        {
            if(name.Text == idToken.Text)
            {
                throw new ParsingException("'" + idToken.Text + "' has been already defined as a process definition at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);                        
            }
        }

        foreach (IToken name in GlobalVarNames)
        {
            if(name.Text == idToken.Text)
            {
                throw new ParsingException("'" + idToken.Text + "' has been already defined as a global variable at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);                        
            }
        }
                    
        foreach (IToken name in GlobalConstNames)
        {
            if(name.Text == idToken.Text)
            {
                throw new ParsingException("'" + idToken.Text + "' has been already defined as a constant at line " + name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);                        
            }
        }
 
        foreach (IToken name in GlobalRecordNames)
        {
            if(name.Text == idToken.Text)
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
        if(!declared)
        {
            throw new ParsingException("Channel name '" + idToken.Text + "' is used without declaration!", idToken);                    
        }
        else 
		{
			PAT.Common.Ultility.ParsingUltility.AddIntoUsageTable(Spec.UsageTable, idToken.Text, idToken);
        }
    }
	
    //check whether the same name is used for different definitions
	private void CheckDuplicatedDeclaration(IToken idToken, List<string> vars)
    {
    	if(vars.Contains(idToken.Text))
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
                throw new ParsingException("'" + idToken.Text +"' has been already defined as a global variable at line "+ name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
            }
        }
        
        foreach (IToken name in GlobalConstNames)
        {
            if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
            {
                throw new ParsingException("'" + idToken.Text +"' has been already defined as a constant at line "+name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
            }
        }
        
        foreach (IToken name in GlobalRecordNames)
        {
                if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
                {
                    throw new ParsingException("'" + idToken.Text +"' has been already defined as a global variable at line "+name.Line + " column " + name.CharPositionInLine + ". Please choose another name!", idToken);
                }
        }
    
        foreach (IToken name in ChannelNames)
        {
            if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
            {
                throw new ParsingException("'" + idToken.Text +"' has been already defined as a channel name at line "+name.Line + " column " + name.CharPositionInLine+ ". Please choose another name!", idToken);
            }
        }
    
        foreach (IToken name in LTLStatePropertyNames)
        {
            if (name.Text == idToken.Text && ((name.Line != idToken.Line) || (name.CharPositionInLine != idToken.CharPositionInLine)))
            {
                throw new ParsingException("'" + idToken.Text +"' has been already defined as an LTL state condition at line "+name.Line + " column " + name.CharPositionInLine+ ". Please choose another name!", idToken);
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
        
        if(!declared)
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
            		foreach(KeyValuePair<string, int> item in LocalVariablesStack)
			{
				if(item.Key == word)
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
                 foreach(KeyValuePair<string, int> item in LocalVariablesStack)
		   {
			   if(item.Key == idToken.Text)
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
	
	//Thuanle: Tinh remove this
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
}

@rulecatch {
	catch (RecognitionException re) 
	{
		Spec.ClearDatabase();
		string ss = GetErrorMessage(re, tokenNames);	
		throw new ParsingException(ss, re.Token);
	}
}

specification	
	: (specBody)*
	;
	catch [RecognitionException re] {
		Spec.ClearDatabase();
		string ss = GetErrorMessage(re, tokenNames);	
     	throw new ParsingException(ss, re.Token);
	}
	catch [ParsingException ex] {
		throw ex;
	}
	catch [Exception ex] {
          throw new ParsingException("Parsing error: "+ ex.Message + (ex.InnerException != null? ("\r\nMore details:"+ex.InnerException.Message): ""), input.TokenStream.Get(0));
	}
	
specBody
	: letDefintion
	| definition 
	| assertion 
	| alphabet
	| define
	| channel
	;
	
alphabet
@init  { 
paraphrases.Push("in alphabet declaration"); 
EventCollection evts = new EventCollection();
}
@after { paraphrases.Pop(); }
	: ^(ALPHABET_NOTE ID (e=eventName[new List<string>(), false, null] {evts.Add(e);})+)
	{
	     AlphaDatabase.Add($ID.Text, evts);
             alphaTokens.Add($ID.Token);
	}
	;
	
channel
@init  { paraphrases.Push("in channel declaration"); }
@after { paraphrases.Pop(); }
	:  ^('channel' ID e=expression[new List<string>(), true, null])
	{
		CheckDuplicatedDeclaration($ID.Token);
		CommonTree expressionTreeRoot = ((Antlr.Runtime.Tree.CommonTree) (((Antlr.Runtime.Tree.BaseTree) (input.TreeSource)).GetChild(1)));
		int size = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e, expressionTreeRoot.Token, ConstantDatabase);
		if(size < 0)
		{
			throw new ParsingException("channel "+$ID.Text+"'s size must greater than or equal to 0!", $ID.Token);	
		}
		//thuanle: Tinh comment this
		ChannelQueue queue = new ChannelQueue(size);
		//Spec.ChannelDatabase.Add($ID.Text, queue);
		//Spec.ChannelMaximumSize.Add($ID.Text, 0);
		Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Channel, new ParsingException($ID.Text, $ID.Token)));
	}
	;
	
assertion 
@init  { paraphrases.Push("in assertion declaration");  string ltl = ""; AssertionBase ass= null; bool hasXoperator = false; string lastToken = ""; List<string> alphabets = new List<string>();
QueryConstraintType contraint = QueryConstraintType.NONE;
}
@after { paraphrases.Pop(); }
	: ASS='assert' proc = definitionRef { if(proc.Args.Length > 0 ) {throw new ParsingException("Process with parameters is not allowed in assersion!\r\nPlease define a parameterless process, e.g. system="+proc.ToString() +"; #assert system ...", $ASS.Token);} }	
	( 
	   ( '|=' ( tk = ( '(' | ')' | '[]' | '<>' | ID  | STRING | '!' | '?' | '&&' | '||' | 'tau' | '->' | '<->' | '/\\' | '\/' | '.' | INT) 
	   	       {		
	   	       
	   	          if (tk.Text == "tau") 
			  {
				  throw new ParsingException("tau operator cannot be used in LTL. The result may not be correct because of the tau elimination optimization.", tk.Token);
			  	//Spec.AddNewWarning("When using tau event in LTL, the results may not be correct because the tau elimination optimization.", tk.Token);
			  	//tk.Token.Text = Common.Classes.Ultility.Constants.TAU;
			  }

			  
	   	          if(tk.Type != INT && !ConstantDatabase.ContainsKey(tk.Text))
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
					    ltlTokens[ltlTokens.Count-1].Type = PAT.Common.Ultility.ParsingUltility.LTL_CHANNEL_TOKEN;
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
				    if (word == "U" || word == "V" || word == "X" || word == "G" ||  word == "F" || word == "R" || word == "true" || word == "false")
				    {
                                          if(ltl.EndsWith(" "))
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
					if(ConstantDatabase.ContainsKey(tk.Text))
					{
				    		ltl += ConstantDatabase[tk.Text].ExpressionID; //.ToString();
				    	}
				    	else
				    	{
				    		ltl += tk.Text;				    	
				    	}
				}

	   	       } 
	        )+ 
	   	{
	   		if (ltl.EndsWith(".") || ltl.EndsWith("?") || ltl.EndsWith("!")) 
			{
				//ltl += tk.Text; //Ultility.Ultility.EVENT_PREFIX +                            
				throw new ParsingException("LTL ends with invalid symbol " + tk.Token.Text + "!", tk.Token);		   
			}
				     
	   		PNAssertionLTL assert = new PNAssertionLTL(proc, ltl.Trim());
	   		BuchiAutomata BA = LTL2BA.FormulaToBA("!(" +ltl.Trim() + ")", options, ASS.Token); //.Replace(".", Ultility.Ultility.DOT_PREFIX)      
			BA.HasXOperator = hasXoperator;
			BuchiAutomata PositiveBA = LTL2BA.FormulaToBA(ltl.Trim(), options, ASS.Token);
			PositiveBA.HasXOperator = hasXoperator;
			assert.SeteBAs(BA, PositiveBA);
			
			ass = assert;	   		
		}
	   )
	   
	|  ( 'deadlockfree'  
		{
		  ass = new PNAssertionDeadLock(proc, false);
		}
	   )
	|  ( 'nonterminating'  
		{
		  ass = new PNAssertionDeadLock(proc, true);
		}
	   )	   
	|  ( 'divergencefree'
		{
		// comment following line by Tinh because this property isn't cared now
   	  	 // ass = new CSPAssertionDivergence(proc);
		}
	   )
	|  ( 'deterministic'
		{
		// comment following line by Tinh because this property isn't cared now
   	  	 // ass = new CSPAssertionDeterminism(proc);
		}
	   )
	|
	   ( 'reaches'  label=ID  (exp=withClause[out contraint])?
		{
		   if(exp != null)
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
	   )	   
	|
	   ( 'refines'  targetProcess=definitionRef
		{
		   // comment following line by Tinh because this property isn't cared now
		   // ass = new CSPAssertionRefinement(proc, targetProcess);
		}
	   )
	 |
	   ( 'refines' '<F>'  targetProcess=definitionRef
		{
		   // comment following line by Tinh because this property isn't cared now
		   // ass = new CSPAssertionRefinementF(proc, targetProcess);
		}
	   )
	 |
	   ( 'refines' '<FD>'  targetProcess=definitionRef
		{
		   // comment following line by Tinh because this property isn't cared now
		   // ass = new CSPAssertionRefinementFD(proc, targetProcess);
		}
	   )	   	   	   	
	)
	{		  
		  string assString = ass.ToString();
		  if(Spec.AssertionDatabase.ContainsKey(assString))
		  {
            	throw new ParsingException("Assertion " + assString + " is defined already!", ASS.Token);		      
		  }
            else
		  {
		  	  ass.AssertToken = ASS.Token;
                 Spec.AssertionDatabase.Add(assString, ass);		      
		  }
	}
	;
	
withClause[out QueryConstraintType contraint] returns [Expression exp = null]
@init  { paraphrases.Push("in with clause"); contraint = QueryConstraintType.NONE; }
@after { paraphrases.Pop(); }
	: ^(wtag='with' (tag='min' {contraint = QueryConstraintType.MIN;}  | tag='max' {contraint = QueryConstraintType.MAX;}) e=expression[new List<string>(), true, null] )
	{
		IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($wtag.Children[1] as CommonTree, input);   
		PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e, token1, "in "+ $tag.Text +" clause", Spec.SpecValuation, ConstantDatabase);
		exp = e;
	}
	;
	
definitionRef returns [DefinitionRef def = null]
@init  { paraphrases.Push("in process invocation"); 
		 List<Expression> para = new List<Expression>();
}
@after { paraphrases.Pop(); }
	: ^(DEFINITION_REF_NODE ID 
		(e=expression[new List<string>(), true, null] 
			{
				if(ConstantDatabase.Count > 0)
				{
					e = e.ClearConstant(ConstantDatabase);
				}
				//e.BuildVars();
				if(e.HasVar)
				{					
					throw new ParsingException("Variables are not allowed in process invocation!", $ID.Token);					
				}
				else
				{
					try
            			{
                	        	e = EvaluatorDenotational.Evaluate(e, null) as Expression;
					}
			          catch (Exception ex)
            			{
			                throw new ParsingException(ex.Message, $ID.Token);
            			}
				}				
				para.Add(e);
			}
		)*
	)
	{
		def = new DefinitionRef($ID.Text, para.ToArray()); 
		dlist.Add(def);
		dtokens.Add($ID.Token);
	} 
	;

define
@init  { paraphrases.Push("in constant/enum/(LTL proposition condition) definition"); int i = 0;List<string> vars = new List<string>();}
@after { paraphrases.Pop(); }
	: ^(DEFINE_CONSTANT_NODE ID INT (negtive='-')?)
	{
		CheckDuplicatedDeclaration($ID.Token);
		if($negtive == null)
		{
			ConstantDatabase.Add($ID.Text, new IntConstant(int.Parse($INT.Text), $ID.Text));		
		}
		else
		{
			ConstantDatabase.Add($ID.Text, new IntConstant(int.Parse($INT.Text)*-1, $ID.Text));
		}
		Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Constant, new ParsingException($ID.Text, $ID.Token)));
	}
	| ^(DEFINE_CONSTANT_NODE name=ID value=('true' | 'false'))
	{
		CheckDuplicatedDeclaration($ID.Token);	
		ConstantDatabase.Add(name.Text, new BoolConstant(bool.Parse(value.Text), $ID.Text));
		Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Constant, new ParsingException($ID.Text, $ID.Token)));
	}
	| ^(DEFINE_CONSTANT_NODE 
		( ID 
		  {
			CheckDuplicatedDeclaration($ID.Token);	
			ConstantDatabase.Add($ID.Text, new IntConstant(i++, $ID.Text));
			Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Constant, new ParsingException($ID.Text, $ID.Token)));
		  }
		)+
	)	
	| ^(DEFINE_NODE ID (parameters=dparameter {vars.AddRange(parameters);})? p=statement[vars, null])
	{		
		CheckDuplicatedDeclaration($ID.Token);	
		IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($DEFINE_NODE.Children[1] as CommonTree, input);   
		//PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(p, token1, "in define declaration", Spec.SpecValuation, ConstantDatabase);		
		p=p.ClearConstant(ConstantDatabase);
		
		if(parameters == null)
		{
			Spec.DeclarationDatabase.Add($ID.Text, p);
			Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Declaration, new ParsingException($ID.Text, $ID.Token)));
		}
		else
		{
			Spec.MacroDefinition.Add($ID.Text+parameters.Count, new KeyValuePair<List<string>, Expression>(parameters, p));
			Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Declaration, new ParsingException($ID.Text+ "(" + Common.Classes.Ultility.Ultility.PPStringList(parameters) + ")" , $ID.Token)));
		}
		
	}
	;


dparameter returns [List<string> parameters = new List<string>()]
        :  ^(DPARAMETER_NODE (ID {CheckDuplicatedDeclaration($ID.Token, parameters); parameters.Add($ID.Text); })+)
        ;

block[List<string> vars, List<string> sourceVars] returns [Expression exp = null]
@init  { List<Expression> stmlist = new List<Expression>(); }
	:  ^(BLOCK_NODE
	{
		BlockDepth++;
	}
	 (s=statement[vars, sourceVars] {stmlist.Add(s);})* )
	{
		if(stmlist.Count > 0)
		{
			exp = stmlist[stmlist.Count-1];	
		}	
		if(stmlist.Count > 1)
		{			
			for(int i = stmlist.Count - 2; i >= 0; i--)
			{
				exp = new PAT.Common.Classes.Expressions.ExpressionClass.Sequence(stmlist[i], exp); 
			}	
     	}
     	
     	while(LocalVariablesStack.Count > 0)
     	{
     		if(LocalVariablesStack.Peek().Value == BlockDepth)
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
	;
	
statement[List<string> vars, List<string> sourceVars] returns [Expression exp = null]
     : e = block[vars, sourceVars] {exp = e;}
     | e = localVariableDeclaration[vars, sourceVars] { exp = e; }
     | e = ifExpression[vars, sourceVars] {exp = e;}
     | e = whileExpression[vars, sourceVars] {exp = e; }
     | e = expression[vars, true, sourceVars] {exp = e;}
     ;	
     
localVariableDeclaration [List<string> vars, List<string> sourceVars] returns [LetDefinition exp = null]	
@init{
List<int> list = new List<int>();
}
	:  ^(LOCAL_VAR_NODE ID 
	{
		CheckIDNameDefined(vars, $ID.Token); 
		LocalVariables.Add($ID.Text); 
		LocalVariablesStack.Push(new KeyValuePair<string, int>($ID.Text, BlockDepth));
	} 	
	(e=expression[vars, true, null] | ( e=recordExpression[vars, null, $ID.Token] {Record r = e as Record;} ) )?)
	{
		if(e == null)
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
			if(!e.HasVar)
               {
                    rhv = EvaluatorDenotational.Evaluate(e, null);
               }
			
			exp = new LetDefinition($ID.Text, rhv);	
			
			if(Spec.SpecValuation.Variables == null)
			{
				Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
			}
			
		}
		catch(Exception ex)
		{
			throw new ParsingException("Local variable definition error:" + ex.Message, $ID.Token);
		}		
	}
	| ^(LOCAL_ARRAY_NODE ID 
	{
		CheckIDNameDefined(vars, $ID.Token); 
		LocalVariables.Add($ID.Text); 
		LocalVariablesStack.Push(new KeyValuePair<string, int>($ID.Text, BlockDepth));
	}	
		(e=expression[vars, true, null]
		{
				try
				{
					if (ConstantDatabase.Count > 0)
					{
						e = e.ClearConstant(ConstantDatabase);
					}
					if(Spec.SpecValuation.Variables == null)
					{
						Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
					}	 
	
				     ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);				
				
					if(rhv is IntConstant)
					{
						IntConstant v = rhv as IntConstant;
						if(v.Value >= 0)
						{
							list.Add(v.Value);
						}
						else
						{
							throw new ParsingException("The record size must be greater than or equal to 0!", $ID.Token);
						}
					}
					else
					{
						throw new ParsingException("The record size must be an integer value!", $ID.Token);
					}	
				}
				catch(Exception ex)
				{
					throw new ParsingException("Variable definition error:" + ex.Message, $ID.Token);
				}
			}
		)+
	
		( rd=recordExpression[vars, null, $ID.Token] )?
		)
	{
		try
		{
			int size = 1;
			foreach(int i in list)
			{
				size = size * i;
			}				
			if(Spec.SpecValuation.Variables == null)
			{
				Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
			}
			
			ExpressionValue record = null;
			
			if(rd != null)
			{
				int recordSize = (rd as Record).Associations.Length;
				if(recordSize != size)
				{
					throw new ParsingException("The declared record size "+ size + " is not same as the number of given elements "+ recordSize+"!", $ID.Token);
				}
				record=EvaluatorDenotational.Evaluate(rd as Record, Spec.SpecValuation);
			}
			else
			{
				record=EvaluatorDenotational.Evaluate(new Record(size), null);
			}
			
			//Spec.SpecValuation.ExtendDestructive($ID.Text, record);
			exp = new LetDefinition($ID.Text, record);	

			ArrayID2DimentionMapping.Add($ID.Text, list);
		
		}
		catch(Exception ex)
		{
			throw new ParsingException("Variable definition error: " + ex.Message, $ID.Token);
		}
	}
	;
	
expression[List<string> vars, bool check, List<string> sourceVars] returns [Expression exp = null]
@init  { paraphrases.Push("in expression");  }
@after { paraphrases.Pop(); }
	: e1=conditionalOrExpression[vars, check, sourceVars] {exp=e1;}
	| ^(ASSIGNMENT_NODE e1=conditionalOrExpression[vars, check, sourceVars] e2=expression[vars, check, sourceVars])	   
    		{ 
		if (e1 is Variable) 
             	{
             		string name = e1.ExpressionID; //((Variable) e1).VarName;
             		if(vars.Contains(name))
             		{
             			throw new ParsingException("CANNOT assign value to local variable " + name + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
             		}
             		else if(ConstantDatabase.ContainsKey(name))
             		{
             			throw new ParsingException("CANNOT assign value to constant variable " + name + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
             		}
             		/*else if(Spec.ParameterVariables.ContainsKey(name))
             		{
             			throw new ParsingException("Can not assign value to parameter variable " + name + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
             		}*/
             		else
             		{
             			exp = new Assignment(name, e2); 
             		}
		} 
		else if ((e1 is PrimitiveApplication) && ((PrimitiveApplication) e1).Operator.Equals(".")) 
		{
		 	string name = ((PrimitiveApplication) e1).Argument1.ExpressionID;
		 	if(vars.Contains(name))
             		{
             				throw new ParsingException("CANNOT assign value to local variable " + name + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
             		}
	         	else if(ConstantDatabase.ContainsKey(name))
             		{
	             			throw new ParsingException("Constant variable " + name + " CANNOT be used here!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
             		}
             		PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e2, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($ASSIGNMENT_NODE.Children[1] as CommonTree, input), "in array assignment", Spec.SpecValuation, ConstantDatabase);
	         	exp = new PropertyAssignment(
						((PrimitiveApplication) e1).Argument1,
						((PrimitiveApplication) e1).Argument2,
						e2);
		 }
		 
		 else if (e1 is ClassProperty)
	         {
	         	if(((ClassProperty) e1).Variable is Variable)
	         	{
		          	string name = (((ClassProperty) e1).Variable as Variable).ExpressionID;
			 	if(vars.Contains(name))
	             		{
	             				throw new ParsingException("CANNOT assign value to local variable " + name + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
	             		}
	             		else if(ConstantDatabase.ContainsKey(name))
	             		{
	             			throw new ParsingException("Constant variable " + name + " CANNOT be used here!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
	             		}             		
             		}
	         	
	         	exp = new ClassPropertyAssignment((e1 as ClassProperty), e2);
	         }
		 else
		 {
		 	//error state
		 	throw new ParsingException("Invalid assignment:" + e1.ToString() + "=" + e2.ToString() + "!", (($ASSIGNMENT_NODE.Children[0] as CommonTree).Children[0] as CommonTree).Token);
		 }  
         }
	;
	
letDefintion
@init{
List<string> vars = new List<string>();
List<int> list = new List<int>();
}
	:   ^(LET_NODE userType=ID? varID=ID varaibleRange[$varID, vars, true, null]? (e=expression[vars, true, null] | ( e=recordExpression[vars, null, $varID.Token] {Record r = e as Record; list.Add(r.Associations.Length); ArrayID2DimentionMapping.Add($varID.Text, list); } ) )?) // | e=funExpression[vars] | e=recfunExpression[vars]
	{
		CheckDuplicatedDeclaration($varID.Token);
		
		if(userType != null)
		{
			if(e != null)
			{
				if(e is NewObjectCreation)
				{
					if((e as NewObjectCreation).ClassName != $userType.Text)				
					{
						throw new ParsingException("Class name " + (e as NewObjectCreation).ClassName+" does not match variable type " + $userType.Text + ".", $varID.Token);
					}					
				}
				else 
				{
					throw new ParsingException("User defined data type can only be initialized using new keyword.", $varID.Token);
				}
			}
			else
			{
				e = PAT.Common.Ultility.Ultility.InitializeUserDefinedDataType($userType.Text);
			}
			
			if(e == null)
			{
				throw new ParsingException("Can not find the user defined data type. Please make sure you have imported it.", $userType.Token);
			}			
		}
		
		if(e == null)
		{
			e = new IntConstant(0);
		}
					    
		try
		{			    
			if (ConstantDatabase.Count > 0)
			{
				e = e.ClearConstant(ConstantDatabase);
			}
			if(Spec.SpecValuation.Variables == null)
			{
				Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
			}
			ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);
			Spec.SpecValuation.ExtendDestructive($varID.Text, rhv);	
			Spec.DeclaritionTable.Add($varID.Text, new Declaration(DeclarationType.Variable, new ParsingException($varID.Text, $varID.Token)));
		}
		catch(Exception ex)
		{
			throw new ParsingException("Variable definition error: " + ex.Message, $varID.Token);
		}			
	}
	|  ^(LET_ARRAY_NODE (ID {CheckDuplicatedDeclaration($ID.Token);}) varaibleRange[$ID, vars, true, null]? 
	
		(e=expression[vars, true, null]
			{
				try
				{
					if (ConstantDatabase.Count > 0)
					{
						e = e.ClearConstant(ConstantDatabase);
					}
					if(Spec.SpecValuation.Variables == null)
					{
						Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
					}	 
	
				     ExpressionValue rhv = EvaluatorDenotational.Evaluate(e, Spec.SpecValuation);				
				
					if(rhv is IntConstant)
					{
						IntConstant v = rhv as IntConstant;
						if(v.Value >= 0)
						{
							list.Add(v.Value);
						}
						else
						{
							throw new ParsingException("The record size must be greater than or equal to 0!", $ID.Token);
						}
					}
					else
					{
						throw new ParsingException("The record size must be an integer value!", $ID.Token);
					}	
				}
				catch(Exception ex)
				{
					throw new ParsingException("Variable definition error:" + ex.Message, $ID.Token);
				}
			}
		)+
	
		( rd=recordExpression[vars, null, $ID.Token] )?
		)
	{
		try
		{
			int size = 1;
			foreach(int i in list)
			{
				size = size * i;
			}				
			if(Spec.SpecValuation.Variables == null)
			{
				Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
			}
			
			ExpressionValue record = null;
			
			if(rd != null)
			{
				int recordSize = (rd as Record).Associations.Length;
				if(recordSize != size)
				{
					throw new ParsingException("The declared record size "+ size + " is not same as the number of given elements "+ recordSize+"!", $ID.Token);
				}
				record=EvaluatorDenotational.Evaluate(rd as Record, Spec.SpecValuation);
			}
			else
			{
				record=EvaluatorDenotational.Evaluate(new Record(size), null);
			}
			
			Spec.SpecValuation.ExtendDestructive($ID.Text, record);
			Spec.DeclaritionTable.Add($ID.Text, new Declaration(DeclarationType.Variable, new ParsingException($ID.Text, $ID.Token)));
			ArrayID2DimentionMapping.Add($ID.Text, list);
		
		}
		catch(Exception ex)
		{
			throw new ParsingException("Variable definition error: " + ex.Message, $ID.Token);
		}
		

	} 
	;
	
varaibleRange[CommonTree ID, List<string> vars, bool check, List<string> sourceVars] 
	:  ^(VARIABLE_RANGE_NODE  (lower=conditionalOrExpression[vars, check, sourceVars])? '.' (upper=conditionalOrExpression[vars, check, sourceVars])?)
	{
		int lowerV = 0;
		int upperV = 0;
		if(lower != null)
		{
			try
			{
				if (ConstantDatabase.Count > 0)
				{
					lower = lower.ClearConstant(ConstantDatabase);
				}
				if(Spec.SpecValuation.Variables == null)
				{
					Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
				}	 
				ExpressionValue rhv = EvaluatorDenotational.Evaluate(lower, Spec.SpecValuation);
				if(rhv is IntConstant)
				{	
					IntConstant v = rhv as IntConstant;
					lowerV = v.Value;
					if (Valuation.VariableLowerBound == null)
                                        {
                                            Valuation.VariableLowerBound = new StringDictionary<int>(16);
                                        }
					Valuation.VariableLowerBound.Add($ID.Text, lowerV);
				}
				else
				{
					throw new ParsingException("The lower bound must be an integer value!", $ID.Token);
				}
			}
			catch(Exception ex)
			{
				throw new ParsingException("Variable lower bound error:" + ex.Message, $ID.Token);
			}
			
		}
		
		if(upper != null)
		{
			try
			{
				if (ConstantDatabase.Count > 0)
				{
					upper = upper.ClearConstant(ConstantDatabase);
				}
				if(Spec.SpecValuation.Variables == null)
				{
					Spec.SpecValuation.Variables = new PAT.Common.Classes.DataStructure.StringDictionaryWithKey<ExpressionValue>();	
				}	
				ExpressionValue rhv = EvaluatorDenotational.Evaluate(upper, Spec.SpecValuation);
				if(rhv is IntConstant)
				{	
					IntConstant v = rhv as IntConstant;
					upperV = v.Value;
					Valuation.VariableUpperLowerBound.Add($ID.Text, upperV);
				}
				else
				{
					throw new ParsingException("The upper bound must be an integer value!", $ID.Token);
				}
			}
			catch(Exception ex)
			{
				throw new ParsingException("Variable upper bound error:" + ex.Message, $ID.Token);
			}			
		}
		
		if(lower != null && upper != null && lowerV > upperV)
		{
			throw new ParsingException("Variable's upper bound must be greater than lower bound:", $ID.Token);
		}
	}
	;
	
conditionalOrExpression[List<string> vars, bool check, List<string> sourceVars] returns [Expression exp = null]
@init  { List<int> list= null; List<Expression> indices = null; List<CommonTree> varName = null; }
	: ^('||' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("||", e1, e2);}
	| ^('&&' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("&&", e1, e2);}
	| ^('xor' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("xor", e1, e2);}
	| ^('|' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("|", e1, e2);}
	| ^('&' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("&", e1, e2);}
	| ^('^' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("^", e1, e2);}
	| ^('==' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("==", e1, e2);}	
	| ^('!=' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("!=", e1, e2);}			
	| ^('<' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("<", e1, e2);}				
	| ^('>' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication(">", e1, e2);}					
	| ^('<=' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("<=", e1, e2);}				
	| ^('>=' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication(">=", e1, e2);}						
	| ^(opt='+' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) 
	{

		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[0] as CommonTree, input));
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[1] as CommonTree, input));
		exp = new PrimitiveApplication("+", e1, e2);
	}				
	| ^(opt='-' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) 
	{
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[0] as CommonTree, input));
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[1] as CommonTree, input));
		exp = new PrimitiveApplication("-", e1, e2);
	}						
	| ^(opt='*' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) 
	{
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[0] as CommonTree, input));
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[1] as CommonTree, input));	
		exp = new PrimitiveApplication("*", e1, e2);
	}				
	| ^(opt='/' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) 
	{
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[0] as CommonTree, input));
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[1] as CommonTree, input));	
		exp = new PrimitiveApplication("/", e1, e2);
	}							
	| ^(opt='%' e1=conditionalOrExpression[vars, check, sourceVars] e2=conditionalOrExpression[vars, check, sourceVars]) 
	{
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[0] as CommonTree, input));
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e2, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($opt.Children[1] as CommonTree, input));	
		exp = new PrimitiveApplication("mod", e1, e2);
	}								
	| ^(UNARY_NODE e1=conditionalOrExpression[vars, check, sourceVars]) 
	{
		//PAT.Common.Ultility.ParsingUltility.CheckParameterVarUsedInMathExpression(e1, Spec.ParameterVariables, PAT.Common.Ultility.ParsingUltility.GetExpressionToken($UNARY_NODE.Children[0] as CommonTree, input));
		exp = new PrimitiveApplication("~",e1);
	}
	| ^(VAR_NODE ID)
	{				
		
		//if the variable must be one inside of the source varaibles, we do a check here
		if(sourceVars != null && sourceVars.Count > 0)
		{
 			    
 			    bool hasMatch = false;  				
 			    foreach (IToken name in GlobalConstNames)
 			    {
 			        if(name.Text == $ID.Text)
 			        {
 			            hasMatch = true;   
 			        }
 			    }
 			    
			    
			    //if the variable is not a constant
                   	    if (!hasMatch)
 			    {
 					//and it is not inside the sourceVars list
 					if(!sourceVars.Contains($ID.Text))
 					{
 						throw new ParsingException("Variable " + $ID.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) +"}", $ID.Token);
 					}
 			    }
 			    exp= new Variable($ID.Text); 				

		}
		else
		{						
			if(Specification.CheckVariableDeclare && check)
			{
				exp = CheckVariableNotDeclared(vars, $ID.Token);
			}
			//if there is no declaration, then this is a variable expression, we need to 
			if(exp == null)
			{
				exp= new Variable($ID.Text); 
			}
		}
		
					
	}
	| a1=arrayExpression[vars, check, sourceVars, varName]{exp= a1;}		
	| INT {exp= new IntConstant(int.Parse($INT.Text));}	
	| 'true' {exp= new BoolConstant(true);} 
	| 'false' {exp= new BoolConstant(false);} 
	| ^('!' e1=conditionalOrExpression[vars, check, sourceVars]) {exp = new PrimitiveApplication("!",e1);}
	| ^('empty' e1=conditionalOrExpression[vars, check, sourceVars]) { exp = new PrimitiveApplication("empty",e1); }
	| ^(CALL_NODE (ID {indices = new List<Expression>(); sourceVars = PAT.Common.Ultility.ParsingUltility.CheckWhetherIsChannelCall($ID.Token, sourceVars, Spec.ChannelDatabase); }) (e1=conditionalOrExpression[vars, check, sourceVars] {indices.Add(e1); })* ) 
	{ 
		
		exp = new StaticMethodCall($ID.Text, indices.ToArray());		
		exp = PAT.Common.Ultility.ParsingUltility.TestMethod((exp as StaticMethodCall), $ID.Token, Spec.ChannelDatabase, ConstantDatabase, Spec);		
		
		if(sourceVars != null)
		{
			foreach (string key in Spec.ChannelDatabase.Keys)
			{
		                sourceVars.Remove(key);
			}
		}  
	}
	| ^(CLASS_CALL_NODE (var=ID {indices = new List<Expression>(); }) method=ID (e1=conditionalOrExpression[vars, check, sourceVars] {indices.Add(e1); })* ) 
	{
		if(sourceVars != null && sourceVars.Count > 0)
		{
 			if(!sourceVars.Contains($var.Text))
 			{
 				throw new ParsingException("Variable " + $var.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) +"}", $var.Token);
 			}
		}
		else if(check)
		{
			exp = CheckVariableNotDeclared(vars, $var.Token);
			if (Spec.SpecValuation.Variables != null && Spec.SpecValuation.Variables.ContainsKey($var.Text))
               {
				PAT.Common.Ultility.ParsingUltility.TestMethodDefined($method.Token, indices.Count, Spec.SpecValuation.Variables[$var.Text]);
			}
			else
			{
				Spec.AddNewWarning("When using user defined data structures as parameter variables, you have to make sure that its method call has no side effects, otherwise the verification result can not be guaranteed.", $var.Token);
			}
		}
	 	exp = new ClassMethodCall($var.Text, $method.Text, indices.ToArray()); 
	}
	| ^(CLASS_CALL_INSTANCE_NODE ({varName=new List<CommonTree>();}a1=arrayExpression[vars, check, sourceVars,varName] {indices = new List<Expression>(); }) method=ID (e1=conditionalOrExpression[vars, check, sourceVars] {indices.Add(e1); })* ) 
	{
	  CommonTree recordVar=varName[0];
		if(sourceVars != null && sourceVars.Count > 0)
		{
 			if(!sourceVars.Contains(recordVar.Text))
 			{
 				throw new ParsingException("Variable " + recordVar.Text + " must be one of the variable inside {" + Common.Classes.Ultility.Ultility.PPStringList(sourceVars) +"}", $var.Token);
 			}
		}
		else if(check)
		{
			exp = CheckVariableNotDeclared(vars, recordVar.Token);
			
			if (Spec.SpecValuation.Variables != null && Spec.SpecValuation.Variables.ContainsKey(recordVar.Text))
      			{
      						
			}
			
			else
			{
				Spec.AddNewWarning("When using user defined data structures as parameter variables, you have to make sure that its method call has no side effects, otherwise the verification result can not be guaranteed.", $var.Token);
			}
		}
		exp = new ClassMethodCallInstance(a1, $method.Text, indices.ToArray()); 
	}
	| ^('new' (className=ID {indices = new List<Expression>(); }) (e1=conditionalOrExpression[vars, check, sourceVars] {indices.Add(e1); })* ) 
	{
		if (!PAT.Common.Ultility.Ultility.IsUserDefinedDataTypeDefined($className.Text))
		{
			throw new ParsingException("Can not find the user defined data type. Please make sure you have imported it.", $className.Token);
		}	
		exp = new NewObjectCreation($className.Text, indices.ToArray()); 
	}
	;
	
arrayExpression[List<string> vars, bool check, List<string> sourceVars, List<CommonTree> varName] returns [Expression aexp = null]	
@init  { paraphrases.Push("in array expression"); List<int> list= null; List<Expression> indices = null; }
@after { paraphrases.Pop(); }
:
^(VAR_NODE (ID {CheckRecordNotDeclared(vars, $ID.Token); if(ArrayID2DimentionMapping.ContainsKey($ID.Text)) {list = ArrayID2DimentionMapping[$ID.Text];} indices = new List<Expression>(); })
		( index=conditionalOrExpression[vars, check, sourceVars]
			{
				IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($VAR_NODE.Children[indices.Count+1] as CommonTree, input);   
				PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(index, token1, "in array access", Spec.SpecValuation, ConstantDatabase);
				indices.Add(index);
				
				if(list != null && indices.Count > list.Count)
				{
					throw new ParsingException("Array " + $ID.Text + " has only dimmention "+ list.Count +"!", $ID.Token);
				}				
			}
		)+ 
	
	)
	{				
		if(list != null && indices.Count != list.Count)
		{
			throw new ParsingException("Array " + $ID.Text + " has dimmention "+ list.Count +"!", $ID.Token);
		}
		
		Expression index1 = indices[indices.Count - 1];
		
		if(list != null)
		{
			int cumulator = list[indices.Count - 1];
			for(int i = list.Count - 2; i >= 0; i--)
			{							
				index1 = new PrimitiveApplication("+", index1, new PrimitiveApplication("*", new IntConstant(cumulator), indices[i]));
				cumulator = cumulator * list[i];
			}
		}
		else //the array is used inside channel or variables
		{
			if(indices.Count > 1)
			{
				throw new ParsingException("Array " + $ID.Text + " can have only one dimmention here. PAT does not support multi-dimensional array in input channel variables or process parameters! But you can still use access the multi-dimentional array by treating it as 1 dimensional array and manually calculating the index.", $ID.Token);
			}
		
		}
		if(varName!=null){
			varName.Add($ID);// Add the var name for CLASS_CALL_INSTANCE_NODE checking
		}
		aexp = new PrimitiveApplication(".", new Variable($ID.Text), index1);
		

	}
	;
	
ifExpression[List<string> vars, List<string> sourceVars] returns [Expression exp = null]
@init  { paraphrases.Push("in if expression"); }
@after { paraphrases.Pop(); }
	:  ^(token='if' e1=expression[vars, true, sourceVars]  e2=statement[vars, sourceVars] e3=statement[vars, sourceVars]?)
	 {
	 	IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($token.Children[0] as CommonTree, input);   
	  	PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(e1, token1, "in if expression", Spec.SpecValuation, ConstantDatabase);
	 	exp = new If(e1,e2,e3);
	 }
	 ;
	
whileExpression[List<string> vars, List<string> sourceVars] returns [Expression exp = null]	
@init  { paraphrases.Push("in while expression"); }
@after { paraphrases.Pop(); }
	: ^(token='while' e1=expression[vars, true, sourceVars] e2=statement[vars, sourceVars])
       {
       		IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($token.Children[0] as CommonTree, input);   
       		PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(e1, token1, "in while expression", Spec.SpecValuation, ConstantDatabase);
	       	exp = new While(e1,e2); 
       }
      ;
        
recordExpression[List<string> vars, List<string> sourceVars, IToken idToken] returns [Expression exp = null]
@init  { paraphrases.Push("in array expression"); List<Expression> list = new List<Expression>();}
@after { paraphrases.Pop(); }
	:  ^(RECORD_NODE (s=recordElement[vars, sourceVars, idToken] {list.AddRange(s);})+ )  
	{exp = new Record(list.ToArray());}
	;


recordElement[List<string> vars, List<string> sourceVars, IToken idToken] returns [List<Expression> list = new List<Expression>()]
	: ^(token=RECORD_ELEMENT_NODE e1=expression[vars, true, sourceVars] (e2=expression[vars, true, sourceVars])?)
	{
		//IntConstant i = PAT.Common.Ultility.ParsingUltility.EvaluateIntExpression(e1, token1, ConstantDatabase);
		e1 = e1.ClearConstant(ConstantDatabase);
		if(e2 == null)
		{
			list.Add(e1);
		}
		else
		{
			IToken token2 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($token.Children[1] as CommonTree, input);
		  	int j = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e2, token2, ConstantDatabase);
		  	while(j > 0)
		  	{
		  		list.Add(e1);
		  		j--;
		  	}
		  	
		}
	}
	| ^(token=RECORD_ELEMENT_RANGE_NODE e1=expression[vars, true, sourceVars] e2=expression[vars, true, sourceVars])
	{
		IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($token.Children[0] as CommonTree, input); 
		IToken token2 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($token.Children[1] as CommonTree, input);
		
		int i = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e1, token1, ConstantDatabase);
		int j = PAT.Common.Ultility.ParsingUltility.EvaluateExpression(e2, token2, ConstantDatabase);
		
		if(j < i)
		{
			for(int k = i; k >= j; k--)
			{
				list.Add(new IntConstant(k));
			}
		}
		else
		{
			for(int k = i; k <= j; k++)
			{
				list.Add(new IntConstant(k));
			}
		}
	}
	;

definition returns [Definition proc = null]  
@init  { paraphrases.Push("in process definition"); 
	List<string> vars = new List<string>(); 
	List<Transition> trans = new List<Transition>(); 
	List<PNPlace> states = new List<PNPlace>(); 
}
@after { paraphrases.Pop(); }
	: ^(DEFINITION_NODE name=ID (v=ID{ CheckDuplicatedDeclaration(v.Token, vars);vars.Add(v.Text); })* 
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
		p=processExpr[vars, name.Text, null]
	)
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
	| ^(PROCESS_NODE pname=STRING (v=ID{vars.Add(v.Text);  CheckDuplicatedDeclaration(v.Token);})* 
	   {
	   	CurrentLTSGraphAlphabetsCalculable = true;
	   } 
	    // init=STRING 
	    // (st=stateDef[vars, $pname.Text.Trim('"'), states.Count.ToString(), states] {states.Add(st);})+
	    (tran=transition[vars, $pname.Text.Trim('"'), states] {trans.Add(tran);})*)
	{
	     if (!Spec.PNDefinitionDatabase.ContainsKey(pname.Text.Trim('"')))
          {
          	if (states.Count == 0)
                {
                    states.Add(new PNPlace("Test", "", ""));
                }
                PetriNet process = new PetriNet(pname.Text.Trim('"'), vars,states);
                // process.SetTransitions(trans);
                Spec.PNDefinitionDatabase.Add(pname.Text.Trim('"'), process);              	                	
                Spec.DeclaritionTable.Add(pname.Text, new Declaration(DeclarationType.Process, new ParsingException(pname.Text, pname.Token)));                
                process.AlphabetsCalculable = CurrentLTSGraphAlphabetsCalculable;
          } else {
		      //throw new Exception("Error happened at line " + ID8.Line +" col "+ ID7.CharPositionInLine + " Process " + ID7.Text + " is defined already!");
		      throw new GraphParsingException(pname.Text.Trim('"'), "Process " + pname.Text.Trim('"') + " is defined already!", pname.Token);
	     }
	     CurrentLTSGraphAlphabetsCalculable = true;
	}
	;
	
stateDef [List<string> vars, String defID, string ID, List<PNPlace> states] returns [PNPlace stateVar = null]	
@init  { paraphrases.Push("in state definition");  }
@after { paraphrases.Pop(); }
	: ^(PLACE_NODE name=STRING) // constraints=clockConstraints[vars, true]? urgent='[U]'? committed='[C]'?
	{		
	     foreach(PNPlace s in states)
	     {
	     	if(s.Name == $name.Text.Trim('"'))
	     	{
	     		throw new Exception("Duplicated state name: " + s.Name);
	     	}
	     }
		stateVar = new PNPlace($name.Text.Trim('"'), ID);
	}
	;
	catch [Exception ex] {
		throw new GraphParsingException(defID, ex.Message, -1, -1, ""); 
	}

transition [List<string> varsInit, String defID, List<PNPlace> states] returns [Transition tran = null]
@init  { paraphrases.Push("in transition definition"); 
List<string> vars = new List<string>(varsInit);
 }
@after { paraphrases.Pop(); }
	:  ^(TRANSITION_NODE from=STRING selects=select[vars]? //constraints=clockConstraints[vars, false]? 
	     guard=conditionalOrExpression[vars, true, null]? 
	     (evt=eventT[vars, defID, false]) 
	     (e=block[vars, null])? 
	     //clockReset=clockRestExpression[vars, true, null]? 
	     to=STRING)
	{		

		if(guard != null)
		{
			int index = 1;
			if(selects != null)
			{
				index =2;
			}
			IToken token1 = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($TRANSITION_NODE.Children[index] as CommonTree, input);
			PAT.Common.Ultility.ParsingUltility.TestIsBooleanExpression(guard, token1, "in guard expression", Spec.SpecValuation, ConstantDatabase);
		}
		
		PNPlace newFrom = null, newTo = null;
        foreach (PNPlace s in states)
        {
	        if(s.Name == $from.Text.Trim('"'))
	        {
	            newFrom = s;
	        }
	        
	        if (s.Name ==  $to.Text.Trim('"'))
	        {
	            newTo = s;
	        }
        }
		
		tran = new Transition(evt, selects==null?null:selects.ToArray(), guard, e, newFrom, newTo);
		tran.HasLocalVariable = (LocalVariables.Count != 0);
		if (ConstantDatabase.Count > 0)
		{
			tran = tran.ClearConstant(states, ConstantDatabase, false);
		}
		//clear the localVariables
		LocalVariables.Clear();		
	}
	;
	catch [Exception ex] {
		throw new GraphParsingException(defID, ex.Message, -1, -1, ""); 
	}
	
select[List<string> vars] returns [List<ParallelDefinition> ppds = new List<ParallelDefinition>()]    
@init  { 
List<PetriNet> processes = new List<PetriNet>(16);
List<string> svars = new List<string>(vars);
}
	: ^(SELECT_NODE 
	   (pdd=paralDef[vars, svars] {ppds.Add(pdd);})+ {foreach (ParallelDefinition pd in ppds){vars.Add(pd.Parameter); svars.Add(pd.Parameter);}})
	;
	
eventT[List<string> vars, string defID, bool hasClockConstraints] returns [Event evt = new Event("")]
@init  { 
List<Expression> para = new List<Expression>(16);
List<string> channelInputVars = new List<string>(); 
}
     :  ev=eventName[vars, true, defID]   
     {
 	   evt = ev;  	
     }
     | ^(EVENT_NAME_NODE 'tau')
     {
 	    evt.BaseName = Common.Classes.Ultility.Constants.TAU;
 	    evt.ExpressionList = new Expression[0];
     }	  
     | ^(CHANNEL_OUT_NODE name=ID (e=expression[vars, true, null])?)
     {
	    CheckChannelDeclared($name.Token);
		if(e != null)
		{
			e = e.ClearConstant(ConstantDatabase);
		}
		evt = new ChannelOutputEvent($name.Text, e);
	}
  	| ^(CHANNEL_IN_NODE name=ID (e=expression[vars, true, null])?)	      
	{
		CheckChannelDeclared($name.Token);		
		
		if(e != null)
		{
			e = e.ClearConstant(ConstantDatabase);
		}
		evt = new ChannelInputEvent($name.Text, e);
	}	
 	;
 	
processExpr[List<string> varsInit, String defID, List<string> sourceVars] returns [PetriNet proc = null]
@init{
List<PetriNet> processes = new List<PetriNet>(16);
List<Expression> para = new List<Expression>();
List<string> vars = new List<string>();
List<string> svars = new List<string>();

if(varsInit != null)
{
	vars.AddRange(varsInit);
	svars.AddRange(varsInit);
}

if(sourceVars != null)
{
	svars.AddRange(sourceVars);
}
}

	: ^('|||' (p=processExpr[vars, defID, sourceVars] {processes.Add(p);} )+) //(p=processExpr[vars] {processes.Add(p);} ) 
	{
		// Tinh comment following line
		// proc = new IndexInterleave(processes);
	}
	|  ^(ATOM_NODE ID ( e=expression[vars, true, null] {para.Add(e);} )*)
	{
	
		if(CurrentDefinition != null)
		{
			if (!Spec.PNDefinitionDatabase.ContainsKey($ID.Text))
     	     		{
     	    	 		throw new ParsingException("Only LTS Processes Can Be Referenced!", $ID.Token);					
          		}
		}
			
		proc = new DefinitionRef($ID.Text, para.ToArray()); 
		dlist.Add((DefinitionRef)proc);
		dtokens.Add($ID.Token);
	}
	| ^(INTERLEAVE_NODE cond=paralDef2[vars, svars] p=processExpr[vars, defID, svars])
	{
		if(cond == null)
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
	;
paralDef[List<string> vars, List<string> sourceVars] returns [ParallelDefinition pd = new ParallelDefinition()]    
	: ^(
		PARADEF_NODE 
		id1=ID 
		{
			CheckIDNameDefined(vars, id1.Token);			
			pd = new ParallelDefinition(id1.Text, id1.Token);

		} 		
		(
		        int1 = conditionalOrExpression[vars, true, sourceVars]
		        {
		        	if (ConstantDatabase.Count > 0)
				{
					int1 = int1.ClearConstant(ConstantDatabase);			
				}
				            
                		Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int1, vars, (CommonTree) id1.Parent.GetChild(pd.Domain.Count+1), id1.Token);
                
		     		pd.Domain.Add(int1);	   	 			
		        } 
		)+		
	   )
	| 
	^(PARADEF1_NODE id1=ID  int1=conditionalOrExpression[vars, true, sourceVars] int2=conditionalOrExpression[vars, true, sourceVars])	
	{
		
		CheckIDNameDefined(vars, id1.Token);		
		pd = new ParallelDefinition(id1.Text, id1.Token);
		
		if (ConstantDatabase.Count > 0)
		{
			int1 = int1.ClearConstant(ConstantDatabase);
			int2 = int2.ClearConstant(ConstantDatabase);
		}
				            
                Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int1, vars, (CommonTree) id1.Parent.GetChild(1), id1.Token);
                Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(int2, vars, (CommonTree) id1.Parent.GetChild(2), id1.Token);
                
                pd.LowerBound = int1;
		pd.UpperBound = int2;
	}
	;
		
paralDef2[List<string> vars, List<string> sourceVars] returns [Expression expr = null]    
	: ^(PARADEF2_NODE e=conditionalOrExpression[vars, true, sourceVars]?)	
	{
		if(e != null)
		{
			IToken token = PAT.Common.Ultility.ParsingUltility.GetExpressionToken($PARADEF2_NODE.Children[0] as CommonTree, input);
			PAT.Common.Ultility.ParsingUltility.TestIsIntExpression(e, token, "in interleave abstract process", Spec.SpecValuation, ConstantDatabase);
			
			if (ConstantDatabase.Count > 0)
			{
				e = e.ClearConstant(ConstantDatabase);			
			}
				            
                	Common.Ultility.ParsingUltility.CheckExpressionWithGlobalVariable(e, vars, (CommonTree) $PARADEF2_NODE.Children[0] as CommonTree, null);

		}
		expr = e;
	}
	;		


eventM[List<string> vars, string defID] returns [Event evt = new Event("")]
 :  e=eventName[vars, true, defID]   
 {
 	evt = e;  	
 }
/* |^(EVENT_WL_NODE e=eventName[vars, true, defID])
 {
	evt = e;
 	evt.FairnessLabelType = FairnessLabelType.WeakLive;
 	Spec.HasFairEvent = true;
 }
 | ^(EVENT_SL_NODE e=eventName[vars, true, defID])
 {
 	evt = e;
 	evt.FairnessLabelType = FairnessLabelType.StrongLive;
 	Spec.HasFairEvent = true;
 } 
 | ^(EVENT_WF_NODE e=eventName[vars, true, defID])
 {
	evt = e;
 	evt.FairnessLabelType = FairnessLabelType.WeakFair;
 	 	Spec.HasFairEvent = true;
 }
 | ^(EVENT_SF_NODE e=eventName[vars, true, defID])
 {
  	evt = e;
 	evt.FairnessLabelType = FairnessLabelType.StrongFair;
 	Spec.HasFairEvent = true;
 } */
 | ^(EVENT_NAME_NODE 'tau')
 {
 	evt.BaseName = Common.Classes.Ultility.Constants.TAU;
 	evt.ExpressionList = new Expression[0];
 }	  
 ; 

eventName[List<string> vars, bool checkVar, String defID] returns [Event e = new Event("")]
@init{
List<Expression> ExpressionList = new List<Expression>();
}
	: ^(EVENT_NAME_NODE ID 
		{ 
			if($ID.Text == "init") {throw new ParsingException("init is an internal event. Please use other names!", $ID.Token); } 
			if($ID.Text == "<missing ID>") {$ID.Token.Text =" "; throw new ParsingException("Please input at least one event!", $ID.Token); } 
		}
		(    	ex=conditionalOrExpression[vars, checkVar, null]	
			{
				ex = ex.ClearConstant(ConstantDatabase);
				ExpressionList.Add(ex);
				if(ex.HasVar)
				{
					if(CurrentDefinition != null)
					{
						CurrentDefinition.AlphabetsCalculable = false;
					}
					
					CurrentLTSGraphAlphabetsCalculable = false;
				}
				
				Common.Ultility.ParsingUltility.TestIsNonVoidExpression(ex, $ID.Token, Spec.SpecValuation);
			}
		)*
	)
	{
		bool channelNameAsEvent = false;		
		foreach (IToken name in ChannelNames)
                {
                    if (name.Text ==  $ID.Text)
                    {
                     	channelNameAsEvent = true;     
                    }
                }
                    
                if(!channelNameAsEvent)
                {
                	CheckIDNameDefined(vars, $ID.Token);
                }
		
		e.BaseName = $ID.Text; 
		e.ExpressionList = ExpressionList.ToArray();
	 	eventsTokens.Add($ID.Token); 
	}
	;