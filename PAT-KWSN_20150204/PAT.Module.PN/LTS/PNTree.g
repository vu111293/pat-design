grammar PNTree;

options
{
	output=AST;
	ASTLabelType=CommonTree;
	language=CSharp3; 
	backtrack = true;
	memoize=true;
}


tokens{

	DEFINITION_REF_NODE;
	BLOCK_NODE;

	CHANNEL_NODE;
	BLOCK_NODE;
	VAR_NODE;
	CALL_NODE;
	LET_NODE;
	LET_ARRAY_NODE;
	APPLICATION_NODE;
	RECORD_NODE;
	RECORD_ELEMENT_NODE;	
	RECORD_ELEMENT_RANGE_NODE;		
	DEFINITION_NODE;
	IF_PROCESS_NODE;
	ATOMIC_IF_PROCESS_NODE;	
	BLOCKING_IF_PROCESS_NODE;
	CASE_PROCESS_NODE;
	CASE_PROCESS_CONDITION_NODE;
	INTERLEAVE_NODE;
	PARALLEL_NODE;
	INTERNAL_CHOICE_NODE;
	EXTERNAL_CHOICE_NODE;
	EVENT_NAME_NODE;
	EVENT_WL_NODE;
	EVENT_SL_NODE;
	EVENT_WF_NODE;
	EVENT_SF_NODE;
	EVENT_PLAIN_NODE;
	EVENT_NODE_CHOICE;
	CHANNEL_IN_NODE;
	CHANNEL_OUT_NODE;
	GUARD_NODE;
	PARADEF_NODE;
	PARADEF1_NODE;
	PARADEF2_NODE;
	EVENT_NODE;
	ATOM_NODE;
	ASSIGNMENT_NODE;
	DEFINE_NODE;
	DEFINE_CONSTANT_NODE;
	HIDING_ALPHA_NODE;
	SELECTING_NODE;
	UNARY_NODE;	
	ALPHABET_NOTE;
	VARIABLE_RANGE_NODE;
	LOCAL_VAR_NODE;
	LOCAL_ARRAY_NODE;
	USING_NODE;
	GENERAL_CHOICE_NODE;
	CLASS_CALL_NODE;
	CLASS_CALL_INSTANCE_NODE;
	PROCESS_NODE;
	CHANNEL_NODE;
	TRANSITION_NODE;
	PLACE_NODE;
	SELECT_NODE;
	DPARAMETER_NODE;

}

@namespace	{PAT.PN.LTS}

@header{
	using PAT.Common;
	using PAT.Common.Classes;
	using PAT.Common.Classes.LTS; // not found
	using PAT.Common.Classes.Expressions;
	using System.Collections.Generic;
}

@members
{
    private Stack paraphrases = new Stack();
    public Specification Spec;    
    public List<IToken> GlobalVarNames = new List<IToken>();
    public List<IToken> GlobalConstNames = new List<IToken>();
    public List<IToken> GlobalRecordNames = new List<IToken>();    
    public List<IToken> LTLStatePropertyNames = new List<IToken>();    
    public List<IToken> ChannelNames = new List<IToken>();
    public List<IToken> DefinitionNames = new List<IToken>();
    public bool IsParameterized = false;	
    public bool HasArbitraryProcess = false;
    public PAT.Common.Classes.DataStructure.StringHashTable HiddenVars = new PAT.Common.Classes.DataStructure.StringHashTable(8);
    public Dictionary<string,string> WildVars = new Dictionary<string,string>();
 




    //public List<IToken> UsingLibraries = new List<IToken>();
	

    public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
    {
    	  string msg = null;
    	  if (e is NoViableAltException ) {
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
            string paraphrase = (string)paraphrases.Peek();
            msg = msg + " (possibly " + paraphrase + ")";
       }
    	  return msg;             
    }
    
    
        //protected override object RecoverFromMismatchedToken(IIntStream input, int ttype, BitSet follow)
        //{
        //    return base.RecoverFromMismatchedToken(input, ttype, follow);
        //}

        //public override void EmitErrorMessage(string msg)
        //{
        //    base.EmitErrorMessage(msg);
        //}

     public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
     {
            string msg = GetErrorMessage(e, tokenNames);
            Spec.AddNewError(msg, e.Token);            
     }
}

@rulecatch {
catch (RecognitionException re) 
{		
	string ss = GetErrorMessage(re, tokenNames);	
     throw new ParsingException(ss, re.Token);
}
}

public
specification
	: (specBody)*
	{
		//re-order the tree to put all declarations at the beginning.	
		CommonTree root_1 = null;
		root_1 = (CommonTree)adaptor.Nil();
		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type == DEFINE_CONSTANT_NODE)
		    {
			   adaptor.AddChild(root_1, node);
		    }
		}
		
		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type == LET_NODE || node.Type == LET_ARRAY_NODE)
		    {
			   adaptor.AddChild(root_1, node);
		    }
		}
		
		
		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type == DEFINE_NODE)

		    {
			   adaptor.AddChild(root_1, node);
		    }
		}
		

		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type == ALPHABET_NOTE)

		    {
			   adaptor.AddChild(root_1, node);
		    }
		}
		
		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type == PROCESS_NODE)

		    {
			   adaptor.AddChild(root_1, node);
		    }
		}


		for (int i = 0; i < root_0.ChildCount; i++)
		{
		    CommonTree node = root_0.GetChild(i) as CommonTree;
		    if(node.Type != USING_NODE && node.Type != DEFINE_CONSTANT_NODE && node.Type != LET_NODE && node.Type != LET_ARRAY_NODE && node.Type != DEFINE_NODE && node.Type != ALPHABET_NOTE && node.Type != PROCESS_NODE)
		    {
			   adaptor.AddChild(root_1, node);
		    }
		}
		root_0 = root_1;
	}
	;
	
specBody 
	: library
	| letDefintion  	
	| definition 
	| assertion 
	| alphabet
	| define
	;
	
library
@init  { paraphrases.Push("in library using declaration"); }
@after { paraphrases.Pop(); }
	: '#' 'import' STRING {PAT.Common.Ultility.ParsingUltility.LoadStandardLib($STRING, Spec.FilePath);} ';' -> ^(USING_NODE STRING)
	| '#' 'include' STRING {PAT.Common.Ultility.ParsingUltility.LoadIncludeModel($STRING, Spec);} ';' -> ^(USING_NODE STRING)
	;	







	/*	
type
	: primitiveType ('[' ']')*
	;

primitiveType
    :   'boolean'
    |	'char'
    |	'byte'
    |	'short'
    |	'int'
    |	'long'
    |	'float'
    |	'double'
    ;
*/

assertion 
@init  { paraphrases.Push("in assertion declaration"); }
@after { paraphrases.Pop(); }
	: '#'! 'assert' definitionRef	
	(
		( '|=' ( '(' | ')' | '[]' | '<>' | ID | STRING | '!' | '?' | '&&' | '||' |'tau' | '->' | '<->' | '/\\' | '\\/' | '.' | INT )+ )
			|  'deadlockfree'  
			|  'nonterminating'  			
			|  'divergencefree'  			
			|  'deterministic'
			|  'reaches' ID withClause?
			|  'refines' definitionRef 
			|  'refines' '<F>' definitionRef 
			|  'refines' '<FD>' definitionRef 
//			|  'equals' definitionRef 						
//			|  'equals' '<F>' definitionRef 
//			|  'equals' '<FD>' definitionRef 
	)
	';'!
	;
	
withClause
@init  { paraphrases.Push("in with clause"); }
@after { paraphrases.Pop(); }
	: 'with'^ value=('min' | 'max') '('! expression ')'!
	;

definitionRef
	: ID ('(' (expression (',' expression )*)?  ')')? -> ^(DEFINITION_REF_NODE ID expression*)
	;	 

alphabet 	
@init  { 
paraphrases.Push("in alphabet declaration"); 
}
@after { paraphrases.Pop(); }
	: '#' 'alphabet' ID  '{' eventName  (',' eventName )* '}' ';'  ->  ^(ALPHABET_NOTE ID eventName+)
	;

define
@init  { paraphrases.Push("in constant/enum/process variable/(LTL state condition) definition"); 
}
@after { paraphrases.Pop(); }
	: '#' 'define' ID {GlobalConstNames.Add($ID);} '-'? INT ';' -> ^(DEFINE_CONSTANT_NODE ID INT '-'?)
	| '#' 'define' ID {GlobalConstNames.Add($ID);} ('true' ';'  -> ^(DEFINE_CONSTANT_NODE ID 'true')
							| 'false' ';' -> ^(DEFINE_CONSTANT_NODE ID 'false')) 
	| 'enum' '{' a=ID {GlobalConstNames.Add($a);} (',' b=ID {GlobalConstNames.Add($b);})* '}'  ';' -> ^(DEFINE_CONSTANT_NODE ID+)
	| '#' 'define' ID {LTLStatePropertyNames.Add($ID);} dparameter? dstatement ';' -> ^(DEFINE_NODE ID dparameter? dstatement)
	//| '#' 'define' id1=ID '#' definitionRef ';' {GlobalVarNames.Add($id1);} -> ^(LET_NODE $id1 definitionRef)	
	;	

dparameter
        :  '(' ID (',' ID )* ')' -> ^(DPARAMETER_NODE ID+)
        ;

dstatement
	: block 	


	| expression
	;
	

block





@init  {bool hasValidStatement=false;}
	: b='{' (s=statement {PAT.Common.Ultility.ParsingUltility.IsStateAValidOneForBlock(s, s.Tree); if(s != null && s.Tree != null) hasValidStatement = true; })* 
	(e=expression  {PAT.Common.Ultility.ParsingUltility.IsStateAValidOneForExpression(e, e.Tree);})? '}' 
	{ if(!hasValidStatement && (e==null || e.Tree == null)) {throw new ParsingException("At least one expression is needed in the expression block.", b);} }
	->  ^(BLOCK_NODE statement* expression? )
	;

statement
     : block 
     //| funExpression 
     //| recfunExpression 
    // | applicationExpression 
     | localVariableDeclaration
     | ifExpression
     | whileExpression
     | e=expression ';'!  {PAT.Common.Ultility.ParsingUltility.IsStateAValidOneForExpression(e, e.Tree);}
     | ';'!
     ;

localVariableDeclaration 	
	: 'var' ID ('=' expression)? ';' -> ^(LOCAL_VAR_NODE ID expression?)	
	| 'var' ID '=' recordExpression ';' -> ^(LOCAL_VAR_NODE ID recordExpression)
	| 'var' ID ('[' expression ']')+ ('=' recordExpression)? ';' -> ^(LOCAL_ARRAY_NODE ID expression+ recordExpression?)
	;
    
expression 
@init  { paraphrases.Push("in expression");  }
@after { paraphrases.Pop(); }
	: conditionalOrExpression 
	( 
		('=' expression) -> ^(ASSIGNMENT_NODE conditionalOrExpression expression)
		| -> conditionalOrExpression		
	)
	;
	
conditionalOrExpression 
@init  { paraphrases.Push("in || (logic or) expression");  }
@after { paraphrases.Pop(); }

    	: conditionalAndExpression ( '||'^ conditionalAndExpression )*
	;	

conditionalAndExpression 
@init  { paraphrases.Push("in && (logic and) expression");  }
@after { paraphrases.Pop(); }

    	: conditionalXorExpression ( '&&'^ conditionalXorExpression)*
	;
	
conditionalXorExpression 
@init  { paraphrases.Push("in logic xor expression");  }
@after { paraphrases.Pop(); }

    	: bitwiseLogicExpression ( 'xor'^ bitwiseLogicExpression)*
	;
	






bitwiseLogicExpression 
@init  { paraphrases.Push("in bitwise logic operator");  }
@after { paraphrases.Pop(); }
    	: equalityExpression ( ( '&'^ | '|'^ | '^'^ ) equalityExpression)*
	;


equalityExpression 
@init  { paraphrases.Push("in ==/!= expression");  }
@after { paraphrases.Pop(); }
    	: relationalExpression ( ('=='^|'!='^) relationalExpression)*
	;
	
relationalExpression 
@init  { paraphrases.Push("in comparison expression");  }
@after { paraphrases.Pop(); }
    	: additiveExpression ( ('<'^ | '>'^ | '<='^ | '>='^) additiveExpression)*
	;

additiveExpression 
@init  { paraphrases.Push("in +/- expression");  }
@after { paraphrases.Pop(); }
    	: multiplicativeExpression ( ('+'^ | '-'^) multiplicativeExpression)*
	;
multiplicativeExpression
@init  { paraphrases.Push("in */'/' expression");  }
@after { paraphrases.Pop(); }
    	: unaryExpression ( ('*'^ | '/'^ | '%'^ ) unaryExpression)*
	;

unaryExpression 
@init  { paraphrases.Push("in unary expression");  }
@after { paraphrases.Pop(); }
    :   '+' unaryExpression -> unaryExpression
    |   '-' unaryExpression -> ^(UNARY_NODE unaryExpression)
    | '!'^ unaryExpressionNotPlusMinus 
    |   unaryExpressionNotPlusMinus 
    		( op='++' -> ^(ASSIGNMENT_NODE unaryExpressionNotPlusMinus ^('+' unaryExpressionNotPlusMinus INT[$op, "1"] ) )
    		| op='--' -> ^(ASSIGNMENT_NODE unaryExpressionNotPlusMinus ^('-' unaryExpressionNotPlusMinus INT[$op, "1"] ) )
    		)    
    |   unaryExpressionNotPlusMinus
    ;

arrayExpression
@init  { paraphrases.Push("in array expression");  }
@after { paraphrases.Pop(); }
:
ID ('[' conditionalOrExpression ']')+  -> ^(VAR_NODE ID conditionalOrExpression+)
;
unaryExpressionNotPlusMinus
@init  { paraphrases.Push("in variable expression");  }
@after { paraphrases.Pop(); }
	: 
	
	 INT 

	| 'true' 
	| 'false' 	
	//| 'empty' '(' conditionalOrExpression ')' -> ^('empty' conditionalOrExpression)
	| 'call' '(' ID (',' conditionalOrExpression)* ')' {PAT.Common.Classes.ModuleInterface.SpecificationBase.HasCSharpCode=true;} -> ^(CALL_NODE ID conditionalOrExpression*)		
	| 'new' ID '(' (conditionalOrExpression (',' conditionalOrExpression)*)? ')'  -> ^('new' ID conditionalOrExpression*)	
	| var=ID '.' method=ID '(' (conditionalOrExpression (',' conditionalOrExpression)*)? ')'  -> ^(CLASS_CALL_NODE $var $method conditionalOrExpression*)
	| a1=arrayExpression '.' method=ID '(' (conditionalOrExpression (',' conditionalOrExpression)*)? ')'  -> ^(CLASS_CALL_INSTANCE_NODE $a1 $method conditionalOrExpression*)



	| arrayExpression //| ID ('[' conditionalOrExpression ']')+  -> ^(VAR_NODE ID conditionalOrExpression+)	
	| '(' conditionalOrExpression ')' -> conditionalOrExpression 

	| ID  -> ^(VAR_NODE ID)

	




	





	;

letDefintion 
	: ('var'|hvar='hvar') ('<' userType=ID '>')? name=ID varaibleRange? ('=' (expression|wildstar='*') )? ';' 
		{ if(hvar!=null){HiddenVars.Add($name.Text);}
		  if(wildstar!=null){WildVars.Add($name.Text,$name.Text);}
		  GlobalVarNames.Add($name);
		  PAT.Common.Classes.ModuleInterface.SpecificationBase.HasCSharpCode=true;
		} 
		-> ^(LET_NODE  $userType? $name varaibleRange? expression?)	
	| ('var'|hvar='hvar') ID varaibleRange? '=' recordExpression ';' {if(hvar!=null){HiddenVars.Add($ID.Text);} GlobalRecordNames.Add($ID);} -> ^(LET_NODE ID varaibleRange? recordExpression)	
	| ('var'|hvar='hvar') ID ('[' expression ']')+ varaibleRange? ('=' (recordExpression|wildstar='*') )? ';' 
	        {
	          if(hvar!=null){HiddenVars.Add($ID.Text);} GlobalRecordNames.Add($ID);
	          if(wildstar!=null){WildVars.Add($ID.Text,$ID.Text);}
	        } 
	        -> ^(LET_ARRAY_NODE ID varaibleRange? expression+ recordExpression?)
	;



varaibleRange
	: ':' '{' (lower=additiveExpression)? '..' (upper=additiveExpression)? '}' -> ^(VARIABLE_RANGE_NODE $lower? '.' $upper?)
	;	
/*

letExpression 
@init  { paraphrases.Push("in let expression");  }
@after { paraphrases.Pop(); }
	: 'let' letDefintion+ 'in' expression -> ^('let' letDefintion+ expression)
	;


funExpression 
@init  { paraphrases.Push("in fun expression");  }
@after { paraphrases.Pop(); }
	: 'fun' notEmptyIdentifierList '->' statement -> ^('fun' notEmptyIdentifierList statement)
	;
	
notEmptyIdentifierList 
	: '(' ID (',' ID)* ')' -> ID+
	;

recfunExpression 
@init  { paraphrases.Push("in recfun expression");  }
@after { paraphrases.Pop(); }
       	:'recfun' ID notEmptyIdentifierList '->' statement ->  ^('recfun' ID notEmptyIdentifierList statement)
	;
	
applicationExpression 
@init  { paraphrases.Push("in application expression");  }
@after { paraphrases.Pop(); }
	: '(' expression notEmptyExpressionList ')' -> ^(APPLICATION_NODE expression notEmptyExpressionList)
     ;

notEmptyExpressionList
	: expression+
	;
*/	

//if definition
ifExpression 
@init  { paraphrases.Push("in if expression");  }
@after { paraphrases.Pop(); }
	:  iftag='if' '(' expression ')' s1=statement {if(s1.Tree == null) {throw new PAT.Common.ParsingException("There is no body for if expression!",iftag);} } ('else' s2=statement)?  -> ^('if' expression $s1 $s2?)
	;
	
whileExpression 
@init  { paraphrases.Push("in while expression");  }
@after { paraphrases.Pop(); }
	: whiletag='while' '(' expression ')' s1=statement  {if(s1.Tree == null) {throw new PAT.Common.ParsingException("There is no body for while expression!",whiletag);} } -> ^('while' expression statement)
        ;

recordExpression 
@init  { paraphrases.Push("in record expression");  }
@after { paraphrases.Pop(); }
	: '[' recordElement (',' recordElement)* ']' -> ^(RECORD_NODE recordElement+) 
	;

recordElement
	: e1=expression 
		(
			('(' e2=expression ')') -> ^(RECORD_ELEMENT_NODE $e1 $e2)
			| ('..' e2=expression ) -> ^(RECORD_ELEMENT_RANGE_NODE $e1 $e2)
			| -> ^(RECORD_ELEMENT_NODE $e1)
		)	
	;

definition 
@init  { paraphrases.Push("in process definition");  }
@after { paraphrases.Pop(); }
	: name=ID ('(' (ID (',' ID )*)? ')')? '=' interleaveExpr ';' {DefinitionNames.Add($name);} -> ^(DEFINITION_NODE ID ID* interleaveExpr)
	| 'Process' pname=STRING  ('(' (ID (',' ID )*)? ')')? ':' transition* ';'  {DefinitionNames.Add($pname);} -> ^(PROCESS_NODE $pname ID* transition*)
	;


transition
@init  { paraphrases.Push("in transition definition");  }
@after { paraphrases.Pop(); }
	: from=STRING '--' select? ('[' conditionalOrExpression ']')? '##@@' eventT '@@##' (block)?  '-->' to=STRING -> ^(TRANSITION_NODE $from select? conditionalOrExpression? eventT block? $to)
	;

select
	: 'select' ':' (paralDef (';' paralDef )*)  -> ^(SELECT_NODE paralDef+)
	;   	
	
eventT 
	: name=ID '!' -> ^(CHANNEL_OUT_NODE $name)
	| name=ID '?' -> ^(CHANNEL_IN_NODE $name) 
	| name=ID '[' conditionalOrExpression ']' '!' -> ^(CHANNEL_OUT_NODE $name conditionalOrExpression)
	| name=ID '[' conditionalOrExpression ']' '?' -> ^(CHANNEL_IN_NODE $name conditionalOrExpression) 
	| eventM 
	;
	
//these rules below give the precedence of the operators, the losest one is interleave.
interleaveExpr 
@init  { paraphrases.Push("in interleave process");  }
@after { paraphrases.Pop(); }
	: p=parallelExpr
	  (
	  	('|||' parallelExpr)+  -> ^('|||' parallelExpr+)
	  	| -> $p
	  )
	  	
	| '|||' (paralDef (';' paralDef )*) '@' interleaveExpr -> ^(INTERLEAVE_NODE paralDef+ interleaveExpr)
	| '|||' paralDef2 '@' interleaveExpr -> ^(INTERLEAVE_NODE paralDef2 interleaveExpr)
	//| '|||' ID ':' '{' '..' '}' '@' interleaveExpr -> ^(INTERLEAVE_NODE ID interleaveExpr)
    	; 
    	
parallelExpr
@init  { paraphrases.Push("in parallel process");  }
@after { paraphrases.Pop(); }
	: p=generalChoiceExpr 
	(
		('||' generalChoiceExpr)+  -> ^('||' generalChoiceExpr+)
		| -> $p
	)
	| '||' (paralDef (';' paralDef )*) '@' interleaveExpr -> ^(PARALLEL_NODE paralDef+ interleaveExpr)	
    	;
    	 
paralDef
	: ID ':' '{' additiveExpression (',' additiveExpression)*  '}' ->  ^(PARADEF_NODE ID additiveExpression+)
	| ID ':' '{' int1=additiveExpression '..' int2=additiveExpression  '}' -> ^(PARADEF1_NODE ID $int1 $int2)
	;
	
paralDef2		
	: '{' '..' '}' {IsParameterized = true; HasArbitraryProcess = true; } -> ^(PARADEF2_NODE)	
	| '{' int1=additiveExpression '}' {IsParameterized = true; } -> ^(PARADEF2_NODE $int1)	
	;
	
 		
	
generalChoiceExpr
@init  { paraphrases.Push("in general choice process");  }
@after { paraphrases.Pop(); }
	//:  interruptExpr ('[]'^ interruptExpr)* //-> ^('[]' interruptExpr+)
	//;
	: p=internalChoiceExpr
	  (
	  	('[]' internalChoiceExpr)+  -> ^('[]' internalChoiceExpr+)
	  	| -> $p
	  )
	  	
	| '[]' (paralDef (';' paralDef )*) '@' interleaveExpr -> ^(GENERAL_CHOICE_NODE paralDef+ interleaveExpr)
    	; 		  
    	 
internalChoiceExpr
@init  { paraphrases.Push("in internal choice process");  }
@after { paraphrases.Pop(); }
	//:  ('<>'^ conditionalChoiceExpr)* //-> ^('<>' conditionalChoiceExpr+)
	//;	
	: p=externalChoiceExpr
	  (
	  	('<>' externalChoiceExpr)+  -> ^('<>' externalChoiceExpr+)
	  	| -> $p
	  )
	  	
	| '<>' (paralDef (';' paralDef )*) '@' interleaveExpr -> ^(INTERNAL_CHOICE_NODE paralDef+ interleaveExpr)
    	; 
	


externalChoiceExpr
@init  { paraphrases.Push("in external choice process");  }
@after { paraphrases.Pop(); }
	//:  interruptExpr ('[]'^ interruptExpr)* //-> ^('[]' interruptExpr+)
	//;
	: p=interruptExpr
	  (
	  	('[*]' interruptExpr)+  -> ^('[*]' interruptExpr+)
	  	| -> $p
	  )
	  	
	| '[*]' (paralDef (';' paralDef )*) '@' interleaveExpr -> ^(EXTERNAL_CHOICE_NODE paralDef+ interleaveExpr)
    	; 

interruptExpr
@init  { paraphrases.Push("in interrupt process");  }
@after { paraphrases.Pop(); }
	: hidingExpr ('interrupt'^ hidingExpr)*  //-> ^('|>' hidingExpr+)
	;
	
hidingExpr
@init  { paraphrases.Push("in hiding process");  }
@after { paraphrases.Pop(); }
	: 
	 sequentialExpr 
	 (
	 		('\\' '{' eventName  (',' eventName )* '}') -> ^(HIDING_ALPHA_NODE sequentialExpr eventName+)
	 		| -> sequentialExpr
	 )
	;

/*	
selectingExpr
@init  { paraphrases.Push("in selecting process");  }
@after { paraphrases.Pop(); }
	: 
	 sequentialExpr
	 (
	 		('/' '{' eventName  (',' eventName )* '}') -> ^(SELECTING_NODE sequentialExpr eventName+)
	 		| -> sequentialExpr
	 )
	;
*/	
sequentialExpr
@init  { paraphrases.Push("in sequential process");  }
@after { paraphrases.Pop(); }
	: guardExpr (';'^ guardExpr)*
	;

guardExpr 
@init  { paraphrases.Push("in guard process");  }
@after { paraphrases.Pop(); }
	: channelExpr
	| '[' conditionalOrExpression ']' channelExpr -> ^(GUARD_NODE conditionalOrExpression channelExpr)
	;

channelExpr
@init  { paraphrases.Push("in channel output/input process");  }
@after { paraphrases.Pop(); }
	: //type=('wl' | 'sl' | 'sf' | 'wf') '(' name=ID '!' e=expression ')' '->' channelExpr -> ^(CHANNEL_OUT_NODE $name expression channelExpr $type)
	//| type=('wl' | 'sl' | 'sf' | 'wf') '(' name=ID '?' var=ID ')' '->' channelExpr -> ^(CHANNEL_IN_NODE $name $var channelExpr $type)
	  name=ID '!' expression ('.' expression)* '->' channelExpr -> ^(CHANNEL_OUT_NODE $name channelExpr expression+)
	| name=ID '?' ('[' guard=conditionalOrExpression ']')? expression ('.' expression)*  '->' channelExpr  -> ^(CHANNEL_IN_NODE $name expression+ channelExpr $guard?)
	| eventExpr
	;

eventExpr
@init  { paraphrases.Push("in event definition");  }
@after { paraphrases.Pop(); }
	: eventM (block)? '->' channelExpr -> ^(EVENT_NODE eventM channelExpr block?)	
	| block '->' channelExpr -> ^(EVENT_NODE channelExpr block)
	| '(' eventM (',' eventM)* ')' '->' channelExpr ->  ^('[]' ^(EVENT_NODE eventM channelExpr)+ )
	| caseExpr
	;

caseExpr
@init  { paraphrases.Push("in case process");  }
@after { paraphrases.Pop(); }
	: 'case'
	  (
	  '{' 
		caseCondition+
		('default' ':' elsec=interleaveExpr)?
	   '}'
	  ) 		 
	  -> ^(CASE_PROCESS_NODE caseCondition+ $elsec?)
	| ifExpr
	;


caseCondition :
	(conditionalOrExpression ':' interleaveExpr) -> ^(CASE_PROCESS_CONDITION_NODE conditionalOrExpression interleaveExpr)
	;

ifExpr
@init  { paraphrases.Push("in if process");  }
@after { paraphrases.Pop(); }
	: atomicExpr  
	| ifExprs
	;

ifExprs
	: 'if' '(' conditionalOrExpression ')' '{' p1=interleaveExpr '}' ('else' p2=ifBlock)?  -> ^(IF_PROCESS_NODE conditionalOrExpression $p1 $p2?) 
	| 'ifa' '(' conditionalOrExpression ')'  '{' p1=interleaveExpr '}' ('else' p2=ifBlock )?  -> ^(ATOMIC_IF_PROCESS_NODE conditionalOrExpression $p1 $p2?) 
	| 'ifb' '(' conditionalOrExpression ')'  '{' interleaveExpr '}'  -> ^(BLOCKING_IF_PROCESS_NODE conditionalOrExpression interleaveExpr) 
	;	

ifBlock
	: ifExprs
	| '{'! interleaveExpr '}'! 
	;	

atomicExpr
@init  { paraphrases.Push("in atomic process");  }
@after { paraphrases.Pop(); }
	: atom  
	| 'atomic' '{' interleaveExpr '}'  -> ^('atomic' interleaveExpr) 
	;	

atom 
	:  ID  ('(' (expression (',' expression )*)?  ')')? -> ^(ATOM_NODE ID  expression*)
	|  'Skip' ('('! ')'!)?
	|  'Stop' ('('! ')'!)?
	|  'assert' '(' expression ')' -> ^('assert' expression)
	|  '(' interleaveExpr ')' ->  interleaveExpr
	;	

/*
actualParameterList 
        :  '(' (expression (',' expression )*)?  ')' -> expression*
	;
*/
eventM 
 :  eventName 
 //| 'wl' '(' eventName ')' -> ^(EVENT_WL_NODE eventName)
 //| 'sl' '(' eventName ')' -> ^(EVENT_SL_NODE eventName)
 //| 'wf' '(' eventName ')' -> ^(EVENT_WF_NODE eventName)
 //| 'sf' '(' eventName ')' -> ^(EVENT_SF_NODE eventName)
 | 'tau' -> ^(EVENT_NAME_NODE 'tau')	 
 ; 
 

eventName 
	: ID ( '.' ex=additiveExpression)* -> ^(EVENT_NAME_NODE ID additiveExpression*)	
	;
	

	

ID : ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')* 
	{
		string id = this.Text;
		for(int i = 65; i < PAT.PN.LTS.PNTreeParser.tokenNames.Length; i++)
		{
		    string s = PAT.PN.LTS.PNTreeParser.tokenNames[i];
			if(s == id)
		  	{
		
	        throw new PAT.Common.ParsingException("Identifier expected, '" + id + "' is a keyword", this.Line, this.CharPositionInLine, id);   
		  	}
		}
	}
	;
	

STRING
    :  '"' (~('"') )* '"'
    ;
	
WS  :  (' ' | '\t' | '\n' | '\r' | '\f') {$channel = Hidden;}
    ;
	
INT : ('0'..'9')+ ;
    
COMMENT
    :   '/*' ( options {greedy=false;} : . )* '*/' {$channel=Hidden;}
    ;
    
LINE_COMMENT
    : '//' ~('\n'|'\r')* '\r'? '\n' {$channel=Hidden;}
    ;



/*
    public override void ReportError(Antlr.Runtime.RecognitionException e)
    {
        throw e;        
    }

*/
 