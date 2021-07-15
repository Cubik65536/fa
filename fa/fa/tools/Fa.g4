// C++�﷨
// https://github.com/antlr/grammars-v4/blob/master/cpp/CPP14Lexer.g4
// https://github.com/antlr/grammars-v4/blob/master/cpp/CPP14Parser.g4

// ANTLR�ĵ�
// https://decaf-lang.github.io/minidecaf-tutorial/
// https://wizardforcel.gitbooks.io/antlr4-short-course/content/

grammar Fa;



//
// keyword
//
AImport:					'@import';
ALib:						'@lib';
Break:						'break';
CC__Cdecl:					'__cdecl';
CC__FastCall:				'__fastcall';
CC__StdCall:				'__stdcall';
Continue:					'continue';
Class:						'class';
Const:						'const';
Else:						'else';
FaEntryMain:				'FaEntryMain';
If:							'if';
Internal:					'internal';
Public:						'public';
Protected:					'protected';
Private:					'private';
Return:						'return';
Signed:						'signed';
Static:						'static';
Unsigned:					'unsigned';
Use:						'use';
While:						'while';



//
// element
//

// ��ֵ����
Assign:						'=';
QusQusAssign:				'??=';
AddAssign:					'+=';
SubAssign:					'-=';
StarAssign:					'*=';
StarStarAssign:				'**=';
DivAssign:					'/=';
ModAssign:					'%=';
AndAssign:					'&=';
OrAssign:					'|=';
XorAssign:					'^=';
AndAndAssign:				'&&=';
OrOrAssign:					'||=';
ShiftLAssign:				'<<=';
ShiftRAssign:				'>>=';

// һԪ����
ReverseOp:					'~';
AddAddOp:					'++';
SubSubOp:					'--';

// ��Ԫ����
PointOp:					'.';
QusQusOp:					'??';
AddOp:						'+';
SubOp:						'-';
StarOp:						'*';
DivOp:						'/';
StarStarOp:					'**';
ModOp:						'%';
AndOp:						'&';
OrOp:						'|';
XorOp:						'^';
AndAndOp:					'&&';
OrOrOp:						'||';
shiftLOp:					QuotJianL QuotJianL;
shiftROp:					QuotJianR QuotJianR;

// ��Ԫ������
Qus:						'?';
Comma:						',';
ColonColon:					'::';
Colon:						':';
Semi:						';';

// ����
QuotFangL:					'[';
QuotFangR:					']';
QuotJianL:					'<';
QuotJianR:					'>';
QuotHuaL:					'{';
QuotHuaR:					'}';
QuotYuanL:					'(';
QuotYuanR:					')';



//
// Literal
//
fragment SimpleEscape:		'\\\'' | '\\"' | '\\\\' | '\\n' | '\\r' | ('\\' ('\r' '\n'? | '\n')) | '\\t';
fragment HexEscape:			'\\x' HEX HEX;
fragment UniEscape:			'\\u' HEX HEX HEX HEX;
fragment Schar:				SimpleEscape | HexEscape | UniEscape | ~["\\\r\n];
//
BoolLiteral:				'true' | 'false';
IntLiteral:					NUM+;
FloatLiteral:				NUM+ PointOp NUM+;
String1Literal:				'"' Schar* '"';
literal:					BoolLiteral | IntLiteral | FloatLiteral | String1Literal;

fragment NUM:				[0-9];
fragment HEX:				NUM | [a-fA-F];
fragment ID_BEGIN:			[a-zA-Z_] | ('\\u' HEX HEX HEX HEX);
fragment ID_AFTER:			NUM | [a-zA-Z_] | ('\\u' HEX HEX HEX HEX);
Id:							ID_BEGIN ID_AFTER*;
ids:						Id (PointOp Id)*;



//
// type
//
typeAfter:					(QuotFangL QuotFangR) | AndOp | Qus;
type:						Const? ( Id
								| (Id QuotJianL type (Comma type)* QuotJianR)	// Lazy<int>
								| (QuotYuanL type (Comma type)+ QuotYuanR)		// (int, string)
							) typeAfter*;										// int[]&
eTypeAfter:					(QuotFangL QuotFangR) | AndOp | StarOp;
eSign:						Signed | Unsigned;
eType:						Const? eSign? Id eTypeAfter*;					// int[]&



//
// list
//
typeVar:					type Id?;
typeVarList:				typeVar (Comma typeVar)*;
eTypeVar:					eType Id?;
eTypeVarList:				eTypeVar (Comma eTypeVar)*;



//
// if
//
ifPart:						If QuotYuanL expr QuotYuanR;
stmtOrExpr:					stmt | expr;
quotStmtPart:				(QuotHuaL stmt* QuotHuaR) | stmt;
quotStmtExpr:				(QuotHuaL stmtOrExpr* expr QuotHuaR) | expr;
ifStmt:						ifPart quotStmtPart (Else ifPart quotStmtPart)* (Else quotStmtPart)?;
ifExpr:						ifPart quotStmtExpr (Else ifPart quotStmtExpr)* (Else quotStmtExpr)?;



//
// while
//
whileStmtPart:				(QuotHuaL stmt* QuotHuaR) | stmt;
whileStmt:					While QuotYuanL expr QuotYuanR whileStmtPart;



//
// expr
//
allAssign:					Assign | QusQusAssign | AddAssign | SubAssign | StarAssign | StarStarAssign | DivAssign | ModAssign | AndAssign | OrAssign | XorAssign | AndAndAssign | OrOrAssign | ShiftLAssign | ShiftRAssign;
allOp:						QusQusOp | PointOp | AddOp | SubOp | StarOp | DivOp | StarStarOp | ModOp | AndOp | OrOp | XorOp | AndAndOp | OrOrOp | shiftLOp | shiftROp;
//exprAtom:					ids
//							| value
//							| (QuotYuanL expr QuotYuanR);
//expr						: (AddOp | SubOp | AddAddOp | SubSubOp)?
//							( ids																			// io.Write
//							| literal																		// 12
//							| (QuotYuanL expr QuotYuanR)													// (12+24)
//							| (expr PointOp expr)															// 10.min
//							| (expr QuotFangL expr QuotFangR)												// list [12]
//							| (expr QuotYuanL (expr (Comma expr)*)? QuotYuanR)								// Write ("")
//							| ((AddAddOp | SubSubOp | ReverseOp) expr)										// ~val
//							| (expr (AddAddOp | SubSubOp))													// val++
//							| (expr allAssign<assoc=right> expr)											// val = 12
//							);
quotExpr:					(QuotYuanL expr QuotYuanR);
exprPrefix:					AddOp | SubOp | AddAddOp | SubSubOp | ReverseOp;								// ǰ׺ + - ++ -- ~
exprBody:					ids | (ColonColon Id) | literal;												// ����
exprSuffix:					(AddAddOp | SubSubOp)															// ��׺ ++ --
							| (QuotYuanL (expr (Comma expr)*)? QuotYuanR)									//      Write ("")
							| (QuotFangL expr QuotFangR)													//      list [12]
							| ((allAssign | allOp) expr);													//      12 += 24
normalExpr:					quotExpr | (exprPrefix* exprBody exprSuffix*);
expr:						normalExpr | ifExpr;



//
// define variable
//
defVarStmt:					type Id Assign expr Semi;



//
// stmt
//
useStmt:					Use ids Semi;
normalStmt:					((Return? expr) | Break | Continue) Semi;
stmt:						normalStmt | ifStmt | whileStmt | defVarStmt;



//
// class
//
publicLevel:				Public | Internal | Protected | Private;
classParent:				Colon ids (Comma ids)*;
classItemPart:				publicLevel? Static? Id Id;
classItemFieldBlock:		classItemPart Semi;
classItemFuncBlock:			classItemPart QuotYuanL typeVarList QuotYuanR QuotHuaL stmt* QuotHuaR;
classBlock:					publicLevel? Class Id classParent? QuotHuaL (classItemFieldBlock | classItemFuncBlock)* QuotHuaR;



//
// import
//
callConvention:				CC__Cdecl | CC__FastCall | CC__StdCall;
importStmt:					AImport eType callConvention Id QuotYuanL eTypeVarList QuotYuanR Semi;
libStmt:					ALib String1Literal Semi;
importBlock:				(importStmt | libStmt)+;



//
// fa_entry_main
//
faEntryMainFuncBlock:		Static type FaEntryMain QuotYuanL QuotYuanR QuotHuaL stmt* QuotHuaR;
program:					useStmt* importBlock? classBlock* faEntryMainFuncBlock?;



//
// skips
//
Comment1:					'/*' .*? '*/' -> channel (HIDDEN);
Comment2:					'//' ~ [\r\n]* -> channel (HIDDEN);
WS:							[ \t\r\n]+ -> channel (HIDDEN);