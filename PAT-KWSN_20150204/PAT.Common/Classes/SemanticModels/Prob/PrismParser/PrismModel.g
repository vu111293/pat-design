grammar PrismModel;

options {
	language = 'CSharp3'; 
	backtrack = true;
}

tokens
{
		// Keywords
	A = 				'A' ;
	BOOL = 				'bool' ;
	CLOCK = 			'clock' ;
	CONST = 			'const' ;
	CTMC = 				'ctmc' ;
	C = 				'C' ;
	DOUBLE = 			'double' ;
	DTMC = 				'dtmc' ;	
	E = 				'E' ;
	ENDINIT = 			'endinit' ;
	ENDINVARIANT = 		'endinvariant' ;
	ENDMODULE = 		'endmodule' ;
	ENDREWARDS = 		'endrewards' ;
	ENDSYSTEM = 		'endsystem' ;
	FALSE = 			'false' ;
	FORMULA = 			'formula' ;
	FILTER = 			'filter' ;
	FUNC = 				'func' ;
	F = 				'F' ;
	GLOBAL = 			'global' ;
	G = 				'G' ;
	INIT = 				'init' ;
	INVARIANT = 		'invariant' ;
	I = 				'I' ;
	INT = 				'int' ;
	LABEL = 			'label' ;
	MAX = 				'max' ;
	MDP = 				'mdp' ;
	MIN = 				'min' ;
	MODULE = 			'module' ;
	X = 				'X' ;
	NONDETERMINISTIC = 	'nondeterministic' ;
	//|	< OF = 				'of' ;
	PMAX = 				'Pmax' ;
	PMIN = 				'Pmin' ;
	P = 				'P' ;
	PROBABILISTIC = 	'probabilistic' ;
	PROB = 				'prob' ;
	PTA = 				'pta' ;
	RATE = 				'rate' ;
	REWARDS = 			'rewards' ;
	RMAX = 				'Rmax' ;
	RMIN = 				'Rmin' ;
	R = 				'R' ;
	S = 				'S' ;
	STOCHASTIC = 		'stochastic' ;
	SYSTEM = 			'system' ;
	TRUE = 				'true' ;
	U = 				'U' ;
	W = 				'W' ;
	// Punctuation, etc.
	// Note that 'NOT' must be the first item of punctuation in this list
	// (PrismSyntaxHighlighter relies on this fact)
	NOT = 			'!' ;
	AND = 			'&' ;
	OR = 			'|' ;
	IMPLIES = 		'=>' ;
	IFF = 			'<=>' ;
	RARROW = 		'->' ;
	COLON = 		':' ;
	SEMICOLON = 	';' ;
	COMMA = 		',' ;
	DOTS = 			'..' ;
	DOT = 			'.' ;
	LPARENTH = 		'(' ;
	RPARENTH = 		')' ;
	LBRACKET =  	'[' ;
	RBRACKET = 		']' ;
	LBRACE = 		'{' ;
	RBRACE = 		'}' ;
	EQ = 			'=' ;
	NE = 			'!=' ;
	LT = 			'<' ;
	GT = 			'>' ;
	LE = 			'<=' ;
	GE = 			'>=' ;
	PLUS = 			'+' ;
	MINUS = 		'-' ;
	TIMES = 		'*' ;
	DIVIDE = 		'/' ;
	PRIME = 		'\'' ;
	RENAME = 		'<-' ;
	QMARK = 		'?' ;
	DQUOTE = 		'\"' ;
}

@parser::header {
using PAT.Common.Classes.SemanticModels.Prob.ExpressionClass;
using PAT.Common.Classes.SemanticModels.Prob.SystemStructure;
using System.Collections.Generic;
using PAT.Common.Classes.Expressions.ExpressionClass;
using System;
}

@parser::members {
	ModelType type = ModelType.MDP;
	FormulaList formulaList = new FormulaList();
	LabelList labelList = new LabelList();
    	ConstantList constantList = new ConstantList();
	List<VarDeclaration> globalVars = new List<VarDeclaration>();
	List<Module> allModules = new List<Module>();
	List<RewardStruct> rewardStructures = new List<RewardStruct>();
	Expression init = null;
	
	Modules probModules;
	ProbProperties probProperties = new ProbProperties();
			
	List<Expression> argsOfFunc = new List<Expression>();
	
	
	public void parsePrismModel()
	{
		prismmodel();
		probModules = new Modules(type, globalVars, formulaList, labelList, constantList, allModules, rewardStructures, init);
	}
	
	public Modules GetModules()
	{
		return probModules;
	}
	
	public ProbProperties GetProperties()
	{
		return probProperties;
	}
	
	private Module GetModuleByName(string Name)
	{
		foreach (var module in allModules)
		{
			if(module.Name == Name)
			{
				return module;
			}
		}

		throw new Exception("Module with name " + Name + " is not defined!");
	}
}


prismmodel
@init {
	
}
	: (properties | modules)*
;

properties
@init {
	
}
	: property (SEMICOLON)* {probProperties.AddProperty($property.value); }
	| labelDef[probProperties.labelList]
	| constantDef[probProperties.constantList]
;


property returns [Property value]
@init {
	string name = null;
}
	: (DQUOTE ID{name = $ID.text;} DQUOTE COLON)?
	expr = expressionITE{value = new Property(name, $expr.value);}
;


modules
@init {
	
	
}
	: 
	  moduleType{type = $moduleType.value;}
	| globalDecl{globalVars.Add($globalDecl.value);}
	| formulaDef[formulaList]
	| labelDef[labelList]
	| constantDef[constantList]
	| renamedModule{allModules.Add($renamedModule.value);}
	| module{allModules.Add($module.value);}
	| rewardStruct{rewardStructures.Add($rewardStruct.value);}
	| initExp{init = $initExp.value;}
		
	;

moduleType returns [ModelType value]
@init {
	
}
	: ((DTMC | PROBABILISTIC){$value = ModelType.DTMC;})
	| ((MDP | NONDETERMINISTIC){$value = ModelType.MDP;})
	| ((CTMC | STOCHASTIC){$value = ModelType.CTMC;})
;

globalDecl returns [VarDeclaration value]
@init {
	
}
	: GLOBAL declaration{$value = $declaration.value;}
;

declaration returns [VarDeclaration value]
@init {
	
}
	: ID COLON varType {value = new VarDeclaration($ID.text, $varType.value);}(INIT init = expression{$value.Init = $init.value;})? SEMICOLON
;

varType returns [DeclarationType value]
@init {
	
}
	: LBRACKET low = expression DOTS high = expression RBRACKET { $value = new DeclarationInt($low.value, $high.value); }
	| BOOL { $value = new DeclarationBool(); }
;

formulaDef[FormulaList formulaList]
@init {
	
}
	: FORMULA ID EQ expr = expression SEMICOLON{formulaList.AddFormula($ID.text, $expr.value);}
;

labelDef[LabelList labelList]
@init {
	
}
	: LABEL DQUOTE ID DQUOTE EQ expr = expression SEMICOLON{labelList.AddLabel($ID.text, $expr.value);}
;

constantDef[ConstantList constantList]
@init {
	VarType type = VarType.Int;
	string name = string.Empty;
	Expression expr = null;
}
	: ((CONST (INT{type = VarType.Int;} | DOUBLE{type = VarType.Double;} | BOOL{type = VarType.Bool;})?)
	| (RATE | PROB){type = VarType.Double;})
	ID{name = $ID.text;}
	(EQ expression{expr = $expression.value;})? SEMICOLON
	{constantList.AddConstant(name, expr, type);}
;

module returns [Module value]
@init {
	List<Command> commands = new List<Command>();
	List<VarDeclaration> localVars = new List<VarDeclaration>();
}
	: MODULE name = ID{value = new Module($name.text, commands, localVars);} 
	(var = declaration{localVars.Add($var.value);})*
	(comm = command{commands.Add($comm.value);})*
	ENDMODULE
;

command returns [Command value]
@init {
	string synch = string.Empty;
	Expression guard = null;
	List<Update> updatesInCommand = null;
	
}
	: LBRACKET ( ID {synch = $ID.text; } )? RBRACKET
	expression{guard = $expression.value;} RARROW updates {updatesInCommand = $updates.value;} SEMICOLON{$value = new Command(synch, guard, updatesInCommand);}
;

updates returns [List<Update> value]
@init {
	$value = new List<Update>();
}
	: ass0 = assignments{Update update0 = new Update($ass0.value, new DoubleConstant(1)); $value.Add(update0);}
	| prob1 = expression COLON ass1 = assignments{Update update1 = new Update($ass1.value, $prob1.value); $value.Add(update1);}
	(PLUS prob2 = expression COLON ass2 = assignments{Update update2 = new Update($ass2.value, $prob2.value); $value.Add(update2);})*
;

assignments returns [List<Assignment> value]
@init {
	$value = new List<Assignment>();
}
	: (ass1 = assignment {$value.Add($ass1.value);} (AND ass2 = assignment{$value.Add($ass2.value);})*)
	| TRUE
;

assignment returns [Assignment value]
@init {
	
}
	: LPARENTH idPrime EQ expression RPARENTH{$value = new Assignment($idPrime.value, $expression.value);}
;

idPrime returns [string value]
@init {
	
}
	: ID PRIME{$value = $ID.text;}
;

renamedModule returns [Module value]
@init {
	Dictionary<string, string> nameMappings = new Dictionary<string, string>();
}
	: MODULE name = ID EQ baseName = ID
	LBRACKET rename[nameMappings] (COMMA rename[nameMappings])* RBRACKET ENDMODULE{$value = GetModuleByName($baseName.text).Rename($name.text, nameMappings);}
;

rename[Dictionary<string, string> nameMappings]
@init {
	
}
	: id1 = ID EQ id2 = ID{$nameMappings.Add($id1.text, $id2.text);}
;

rewardStruct returns [RewardStruct value]
@init {
	List<RewardItem> rewardItems = new List<RewardItem>();
	string name = string.Empty;
	string s = null;
}
	: REWARDS (DQUOTE structName = ID{name = $structName.text;} DQUOTE)?
	((LBRACKET{s = string.Empty;} (transLabel = ID{s = $transLabel.text;})? RBRACKET)?
	guard = expression COLON rewardValue = expression SEMICOLON{rewardItems.Add(new RewardItem(s, $guard.value, $rewardValue.value));})* ENDREWARDS
	{value = new RewardStruct(name, rewardItems);}
;

initExp returns [Expression value]
@init {
	
}
	: INIT expression{$value = $expression.value;} ENDINIT
;
expression returns [Expression value]
@init {
	
}
	: expressionTemporalBinary{$value = $expressionTemporalBinary.value;}
;

expressionTemporalBinary returns [Expression value]
@init {
	Temporal temp = new Temporal();
}
	: opr1 = expressionTemporalUnary{value = $opr1.value;}
	(
		( U{temp.op = TemporalOpt.Until;}
		| W{temp.op = TemporalOpt.WeakUntil;}
		| R{temp.op = TemporalOpt.Release;}
		)
		(timeBound{temp.SetTimeBound($timeBound.value);})?
		opr2 = expressionTemporalUnary{temp.operand2 = $opr2.value; temp.operand1 = value; value = temp;}
	)?
;


expressionTemporalUnary returns [Expression value]
@init {
	Temporal temp = new Temporal();
}
	:(
	 X{temp.op = TemporalOpt.Next;}
	| F{temp.op = TemporalOpt.Future;}
	| G{temp.op = TemporalOpt.Global;}
	)
	(timeBound{temp.SetTimeBound($timeBound.value);})?
	opr2 = expressionTemporalUnary{temp.operand2 = $opr2.value; value = temp;}
	|expressionITE{value = $expressionITE.value;}
	
;


timeBound returns [TimeBound value]
@init {
	value = new TimeBound();
}
	: LE (var1 = ID{value.uBound = new Variable($var1.text);} | exp1 = expression{value.uBound = $exp1.value;})
	| LT {value.uBoundStrict=true;} (var2 = ID{value.uBound = new Variable($var2.text);} | exp2 = expression{value.uBound = $exp2.value;})
	| GE (var3 = ID{value.lBound = new Variable($var3.text);} | exp3 = expression{value.lBound = $exp3.value;})
	| GT {value.lBoundStrict=true;} (var4 = ID{value.lBound = new Variable($var4.text);} | exp4 = expression{value.lBound = $exp4.value;})
	| LBRACKET lowExp = expression{value.lBound = $lowExp.value;} COMMA highExp = expression{value.uBound = $highExp.value;} RBRACKET
	
;


expressionITE returns [Expression value]
@init {
	
}
	: cond = expressionImplies{value = $cond.value;}
	(QMARK ifPart = expressionImplies COLON elsePart = expressionITE {value = new If($cond.value, $ifPart.value, $elsePart.value);})?
	
;

expressionImplies returns [Expression value]
@init {
	
}
	: exp1 = expressionIff{value = $exp1.value;}
	(IMPLIES exp2 = expressionIff{value = new PrimitiveApplication(PrimitiveApplication.IMPLIES, value, $exp2.value);})*
	
;

expressionIff returns [Expression value]
@init {
	
}
	: expressionOr{value = $expressionOr.value;}
	
;

expressionOr returns [Expression value]
@init {
	
}
	: and1 = expressionAnd{$value = $and1.value;} (OR and2 = expressionAnd{$value = Expression.OR($value, $and2.value);})*
;

expressionAnd returns [Expression value]
@init {
	
}
	: equality1 = expressionEquality{$value = $equality1.value;} (AND equality2 = expressionEquality{$value = Expression.AND($value, $equality2.value);})*
;

expressionEquality returns [Expression value]
@init {
	
}
	: relop1 = expressionRelop{$value = $relop1.value;} 
	(eqNeq relop2 = expressionRelop{$value = new PrimitiveApplication($eqNeq.value, $value, $relop2.value);})*
;

expressionRelop returns [Expression value]
@init {
	
}
	: plusminus1 = expressionPlusMinus{$value = $plusminus1.value;} 
	(ltGt plusminus2 = expressionPlusMinus{$value = new PrimitiveApplication($ltGt.value, $value, $plusminus2.value);})*
;

expressionPlusMinus returns [Expression value]
@init {
	string op = PrimitiveApplication.PLUS;
}
	: timediv1 = expressionTimesDivide{$value = $timediv1.value;} 
	(
		(PLUS{op = PrimitiveApplication.PLUS;} | MINUS {op = PrimitiveApplication.MINUS;})
		timediv2 = expressionTimesDivide{$value = new PrimitiveApplication(op, $value, $timediv2.value);}
	)*
;

expressionTimesDivide returns [Expression value]
@init {
	string op = PrimitiveApplication.PLUS;
}
	: unaryminus1 = expressionUnaryMinus{$value = $unaryminus1.value;} 
	(
		(TIMES{op = PrimitiveApplication.TIMES;} | DIVIDE {op = PrimitiveApplication.DIVIDE;})
		unaryminus2 = expressionUnaryMinus{$value = new PrimitiveApplication(op, $value, $unaryminus2.value);}
	)*
;

expressionUnaryMinus returns [Expression value]
@init {
	
}
	: MINUS minusBasic = expressionBasic{$value = new PrimitiveApplication(PrimitiveApplication.MINUS, $minusBasic.value);}
	| NOT notBasic = expressionBasic{$value = new PrimitiveApplication(PrimitiveApplication.NOT, $notBasic.value);}
	| othersBasic = expressionBasic{$value = $othersBasic.value;}
;

expressionBasic returns [Expression value]
@init {
	
}
	: expressionLiteral{$value = $expressionLiteral.value;}
	| expressionFuncMinMax{$value = $expressionFuncMinMax.value;}
	| expressionParenth{$value = $expressionParenth.value;}
	| expressionFuncOrIdent{$value = $expressionFuncOrIdent.value;}
	| expressionFuncOldStyle{$value = $expressionFuncOldStyle.value;}
	// Remaining options are only applicable for properties
	| expressionProb{$value = $expressionProb.value;}
	| expressionSS{$value = $expressionSS.value;}
	| expressionReward{$value = $expressionReward.value;}
	| expressionExists{$value = $expressionExists.value;}
	| expressionForAll{$value = $expressionForAll.value;}
	| expressionLabel{$value = $expressionLabel.value;}
	| expressionFilter{$value = $expressionFilter.value;}
;

expressionLiteral returns [Expression value]
@init {
	
}
	: intNumber{$value = $intNumber.value;}
	| doubleNumber{$value = $doubleNumber.value;}
	| TRUE {$value = new BoolConstant(true);}
	| FALSE {$value = new BoolConstant(false);}
;


intNumber returns [Expression value]
@init {
	
}
	: INT_NUM {$value = new IntConstant(int.Parse($INT_NUM.text));}
;


doubleNumber returns [Expression value]
@init {
	string number = string.Empty;
}
	: zint = INT_NUM{number += $zint.text;} (DOT dint = INT_NUM{number+= "." + $dint.text; value = new DoubleConstant(double.Parse(number));}) 
;


expressionFuncMinMax returns [Expression value]
@init {
	bool isMin = false;
	List<Expression> args = new List<Expression>();
}
	: (MIN{isMin = true;} | MAX{isMin = false;})
	LPARENTH  expressionFuncArgs[args] RPARENTH{if(isMin) return Expression.MIN(args); else return Expression.MAX(args);}
;


expressionFuncArgs[List<Expression> args]
@init {
	
}
	: arg1 = expression{$args.Add($arg1.value);} (COMMA arg2 = expression{$args.Add($arg2.value);})*
;

expressionParenth returns [Expression value]
@init {
	
}
	: LPARENTH expression{$value = $expression.value;} RPARENTH
;

expressionFuncOrIdent returns [Expression value]
@init {
	argsOfFunc.Clear();
	string s = string.Empty;
}
	: ID{s = $ID.text; value = new Variable(s);}
	// If there is, it's a function
	( LPARENTH expressionFuncArgs[argsOfFunc]{value = new FuncNary(s, argsOfFunc);} RPARENTH)?
;

expressionFuncOldStyle returns [Expression value]
@init {
	argsOfFunc.Clear();
	List<Expression> args = new List<Expression>();
	string s = string.Empty;
}
	: FUNC LPARENTH ( MIN { s = "min"; } | MAX { s = "max"; } | ID{s = $ID.text;} )
	COMMA expressionFuncArgs[args] RPARENTH	{value = new FuncNary(s, args);}
;

eqNeq returns [string value]
@init {
	
}
	: EQ{$value = PrimitiveApplication.EQUAL;}
	| NE{$value = PrimitiveApplication.NOT_EQUAL;}
;

ltGt returns[string value]
@init {
	
}
	: GT{$value = PrimitiveApplication.GREATER;}
	| LT{$value = PrimitiveApplication.LESS;}
	| GE{$value = PrimitiveApplication.GREATER_EQUAL;}
	| LE{$value = PrimitiveApplication.LESS_EQUAL;}
;

// (Property) expression: probabilistic operator P

expressionProb returns[Expression value]
@init {
	string relOp = null;
	bool isBool = false;
	OldStyleFilter f = null;
}
	:( P 
	(ltGt prob = expression{relOp = $ltGt.text; isBool = true;}
	| EQ QMARK{relOp = PrimitiveApplication.EQUAL; isBool = false;}
	| MIN EQ QMARK{relOp = PrimitiveApplication.MIN; isBool = false;}
	| MAX EQ QMARK{relOp = PrimitiveApplication.MAX; isBool = false;}
	)
	| PMIN EQ QMARK{relOp = PrimitiveApplication.MIN; isBool = false;}
	| PMAX EQ QMARK{relOp = PrimitiveApplication.MAX; isBool = false;}
	)
	LBRACKET expr = expression (filter{f = $filter.value;})?
	RBRACKET{value = new ProbQuery($expr.value, relOp, $prob.value); 
			if (f != null) {
			string filterOp = isBool ? "&" : f.getFilterOpString();
			value = new NewStyleFilter(filterOp, value, f.expr);
		    }
		}
;


filter returns[OldStyleFilter value]
@init {
	
}
	: LBRACE expr = expression{value = new OldStyleFilter(expr);} RBRACE
	(LBRACE
		( MIN{value.minReq = true;}
		|MAX{value.maxReq = true;}
		)
	RBRACE)*
;


expressionSS returns[Expression value]
@init {
	string relOp = null;
	bool isBool = false;
	OldStyleFilter f = null;
}
	: S 
	(
	 ltGt prob = expression{relOp = $ltGt.text; isBool = true;}
	|EQ QMARK{relOp = PrimitiveApplication.EQUAL; isBool = false;}  
	)
	LBRACKET expr = expression (filter{f = $filter.value;})?
	RBRACKET{value = new SteadyState($expr.value, relOp, $prob.value); 
			if (f != null) {
			string filterOp = isBool ? "&" : f.getFilterOpString();
			value = new NewStyleFilter(filterOp, value, f.expr);
		    }
		}
;

expressionReward returns[Expression value]
@init {
	int rewardStructIndex = -1;
        string rewardName = null;
	
	string relOp = null;
	bool isBool = false;
	OldStyleFilter f = null;
}
	:( R (rewardIndex[out rewardStructIndex, out rewardName])?
	(ltGt prob = expression{relOp = $ltGt.text; isBool = true;}
	| EQ QMARK{relOp = PrimitiveApplication.EQUAL; isBool = false;}
	| MIN EQ QMARK{relOp = PrimitiveApplication.MIN; isBool = false;}
	| MAX EQ QMARK{relOp = PrimitiveApplication.MAX; isBool = false;}
	)
	| RMIN EQ QMARK{relOp = PrimitiveApplication.MIN; isBool = false;}
	| RMAX EQ QMARK{relOp = PrimitiveApplication.MAX; isBool = false;}
	)
	LBRACKET expr = expressionRewardContents (filter{f = $filter.value;})?
	RBRACKET{value = new RewardQuery($expr.value, relOp, $prob.value, rewardStructIndex, rewardName);
			if (f != null) {
			string filterOp = isBool ? "&" : f.getFilterOpString();
			value = new NewStyleFilter(filterOp, value, f.expr);
		    }
		}
;


rewardIndex[out int rewardIndex, out string rewardName]
@init {
	rewardIndex = -1;
        rewardName = null;
}
	: LBRACE
	(DQUOTE ID{rewardName = $ID.text;} DQUOTE
	| INT_NUM{rewardIndex = int.Parse($INT_NUM.text);}
	)
	RBRACE
	
;


expressionRewardContents returns [Expression value]
@init {
	Temporal ret = new Temporal();
}
	: (C LE exprC = expression{ret.op = TemporalOpt.Cummulative; ret.uBound = $exprC.value;}
	| I EQ exprI = expression{ret.op = TemporalOpt.Instantaneous; ret.uBound = $exprI.value;}
	| F exprF = expression{ret.op = TemporalOpt.Reachability; ret.operand2 = $exprF.value;}
	| S {ret.op = TemporalOpt.SteadyState;}
	){value = ret;}
	
	
;


expressionExists returns [Expression value]
@init {

}
	: E LBRACKET expr = expression RBRACKET{value = new Exists($expr.value);}
	
	
;

expressionForAll returns [Expression value]
@init {

}
	: A LBRACKET expr = expression RBRACKET{value = new ForAll($expr.value);}
	
	
;

expressionLabel returns [Expression value]
@init {
	string s = string.Empty;
}
	: DQUOTE (ID{s = $ID.text;} | INIT {s = "init";}) DQUOTE{value = new Label(s);}
	
	
;

expressionFilter returns [Expression value]
@init {
	string op = string.Empty;
	Expression filter = null;
}
	: FILTER LPARENTH
	( MIN { op = "min"; } 
	| MAX { op = "max"; }
	| PLUS { op = "+"; }
	| AND { op = "&"; }
	| OR { op = "|"; }
	| ID { op = $ID.text;}
	)
	COMMA expr2 = expression
	(COMMA f = expression{filter = $f.value;})?
	RPARENTH{value = new NewStyleFilter(op, $expr2.value, filter);}
;

ID  	: ('a'..'z'|'A'..'Z'|'_') ('a'..'z'|'A'..'Z'|'0'..'9'|'_')*
    	;

INT_NUM:			('0'..'9')+;

    	
WS  	: ( ' ' | '\t' | '\r' | '\n' ) {$channel=Hidden;}
    	;

COMMENT
    :   '/*' ( options {greedy=false;} : . )* '*/' {$channel=Hidden;}
    ;
    
LINE_COMMENT
    : '//' ~('\n'|'\r')* '\r'? '\n' {$channel=Hidden;}
    ;