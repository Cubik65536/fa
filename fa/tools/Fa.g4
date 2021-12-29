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
Do:							'do';
Else:						'else';
Enum:						'enum';
FaMain:						'FaMain';
For:						'for';
If:							'if';
Is:							'is';
Internal:					'internal';
Mut:						'mut';
Namespace:					'namespace';
New:						'new';
Params:						'params';
Public:						'public';
Protected:					'protected';
Private:					'private';
Return:						'return';
Signed:						'signed';
Static:						'static';
Step:						'step';
Switch:						'switch';
Unsigned:					'unsigned';
Use:						'use';
When:						'when';
While:						'while';



//
// element
//

// ��ֵ����
Assign:						'=';
AddAssign:					AddOp Assign;
SubAssign:					SubOp Assign;
StarAssign:					StarOp Assign;
DivAssign:					DivOp Assign;
ModAssign:					ModOp Assign;
OrAssign:					OrOp Assign;
AndAssign:					AndOp Assign;
XorAssign:					XorOp Assign;
QusQusAssign:				QusQusOp Assign;
StarStarAssign:				StarStarOp Assign;
AndAndAssign:				AndAndOp Assign;
OrOrAssign:					OrOrOp Assign;
shiftLAssign:				shiftLOp Assign; // ������
shiftRAssign:				shiftROp Assign;
allAssign:					Assign | QusQusAssign | AddAssign | SubAssign | StarAssign | StarStarAssign | DivAssign | ModAssign | AndAssign | OrAssign | XorAssign | AndAndAssign | OrOrAssign | shiftLAssign | shiftRAssign;

// һԪ����
ReverseOp:					'~';
AddAddOp:					'++';
SubSubOp:					'--';
Exclam:						'!';

// ��Ԫ����
PointPoint:					'..';
PointOp:					'.';
AddOp:						'+';
SubOp:						'-';
StarOp:						'*';
DivOp:						'/';
ModOp:						'%';
OrOp:						'|';
AndOp:						'&';
XorOp:						'^';
QusQusOp:					Qus Qus;
StarStarOp:					StarOp StarOp;
AndAndOp:					AndOp AndOp;
OrOrOp:						OrOp OrOp;
shiftLOp:					QuotJianL QuotJianL;
shiftROp:					QuotJianR QuotJianR;

// ��Ԫ������
Qus:						'?';
Comma:						',';
ColonColon:					'::';
Colon:						':';
Semi:						';';
Underline:					'_';

// ����
QuotFangL:					'[';
QuotFangR:					']';
QuotJianL:					'<';
QuotJianR:					'>';
QuotHuaL:					'{';
QuotHuaR:					'}';
QuotYuanL:					'(';
QuotYuanR:					')';

// �Ƚ�   TODO
ltOp:						QuotJianL;
ltEqualOp:					QuotJianL Assign;
gtOp:						QuotJianR;
gtEqualOp:					QuotJianR Assign;
equalOp:					Assign Assign;
notEqualOp:					Exclam Assign;
exprFuncDef:				Assign QuotJianR;


selfOp2:					AddOp | SubOp | StarOp | DivOp | StarStarOp | ModOp | AndOp | OrOp | XorOp | AndAndOp | OrOrOp | shiftLOp | shiftROp;
compareOp2:					ltOp | ltEqualOp | gtOp | gtEqualOp | equalOp | notEqualOp;
changeOp2:					QusQusOp | compareOp2;
allOp2:						selfOp2 | changeOp2;



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
intNum:						SubOp? IntLiteral;
FloatLiteral:				NUM+ PointOp NUM+;
floatNum:					SubOp? FloatLiteral;
String1Literal:				'"' Schar* '"';
literal:					BoolLiteral | intNum | floatNum | String1Literal;

fragment NUM:				[0-9];
fragment HEX:				NUM | [a-fA-F];
//fragment ID_BEGIN:			[a-zA-Z_] | ('\\u' HEX HEX HEX HEX);
//fragment ID_AFTER:			NUM | [a-zA-Z_] | ('\\u' HEX HEX HEX HEX);
RawId:						('@' | [a-zA-Z_] | ('\\u' HEX HEX HEX HEX)) (NUM | [a-zA-Z_] | ('\\u' HEX HEX HEX HEX))*;
id:							Underline | RawId;
ids:						id (PointOp id)*;
idExt:						ids QuotJianL type (Comma type)* QuotJianR PointOp id;



//
// type
//
typeAfter:					(QuotFangL QuotFangR) | Qus;
typeSingle:					ids (QuotJianL type (Comma type)* QuotJianR)?;
typeMulti:					QuotYuanL typeVar (Comma typeVar)+ QuotYuanR;
type:						(Mut | Params)? (typeSingle | typeMulti) typeAfter*;



//
// list
//
typeVar:					type id?;
typeVarList:				typeVar (Comma typeVar)*;
typeVar2:					type? id;
typeVar2List:				typeVar2 (Comma typeVar2)*;
//eTypeVar:					eType id?;
//eTypeVarList:				eTypeVar (Comma eTypeVar)*;



//
// if
//
quotStmtPart:				QuotHuaL stmt* QuotHuaR;
quotStmtExpr:				QuotHuaL stmt* expr QuotHuaR;
ifStmt:						If expr quotStmtPart (Else If expr quotStmtPart)* (Else quotStmtPart)?;
ifExpr:						If expr quotStmtExpr (Else If expr quotStmtExpr)* Else quotStmtExpr;



//
// loop
//
whileStmt:					While expr QuotHuaL stmt* QuotHuaR;
whileStmt2:					Do QuotHuaL stmt* QuotHuaR While expr;
forStmt:					For stmt expr Semi (expr (Comma expr)*)? QuotHuaL stmt* QuotHuaR;
forStmt2:					For type id Colon expr QuotHuaL stmt* QuotHuaR;



//
// switch
//
switchStmtPart2Last:		Underline exprFuncDef stmt;
quotStmtExprWrap:			quotStmtExpr | expr;
switchExprPartLast:			Underline exprFuncDef quotStmtExprWrap Comma;
//
switchStmtPart:				expr (When expr)? exprFuncDef stmt;
switchStmt:					Switch expr QuotHuaL switchStmtPart* QuotHuaR;
switchStmtPart2:			When expr exprFuncDef stmt;
switchStmt2:				Switch QuotHuaL switchStmtPart2* switchStmtPart2Last QuotHuaR;
//
switchExprPart:				expr (When expr)? exprFuncDef quotStmtExprWrap Comma;
switchExpr:					Switch expr QuotHuaL switchExprPart* switchExprPartLast QuotHuaR;
switchExprPart2:			When expr exprFuncDef quotStmtExprWrap Comma;
switchExpr2:				Switch QuotHuaL switchExprPart2* switchExprPartLast QuotHuaR;



//
// expr
//
quotExpr:					QuotYuanL expr QuotYuanR;
exprOpt:					expr?;
newExprItem:				id (Assign middleExpr)?;
newExpr1:					New ids? QuotHuaL (newExprItem (Comma newExprItem)*)? QuotHuaR;
newExpr2:					New ids? QuotYuanL (expr (Comma expr)*)? QuotYuanR;
//newArray:					New ids? QuotFangL middleExpr QuotFangR;
arrayExpr1:					QuotFangL expr PointPoint expr (Step expr)? QuotFangR;
arrayExpr2:					QuotFangL expr (Comma expr)* QuotFangR;
lambdaExpr:					QuotYuanL typeVar2List? QuotYuanR exprFuncDef (expr | (QuotHuaL stmt* QuotHuaR));
strongExprBase:				(ColonColon? id) | literal | ifExpr | quotExpr | newExpr1 | newExpr2 | arrayExpr1 | arrayExpr2 | switchExpr2 | switchExpr | lambdaExpr | idExt;
strongExprPrefix:			SubOp | AddAddOp | SubSubOp | ReverseOp | Exclam;								// ǰ׺ - ++ -- ~ !
strongExprSuffix			: AddAddOp | SubSubOp															// ��׺ ++ --
							| (QuotYuanL (expr (Comma expr)*)? QuotYuanR)									//     Write ("")
							| (QuotFangL (exprOpt (Colon exprOpt)*) QuotFangR)								//     list [12]
							| (PointOp id)																	//     wnd.Name
							| (Is ids (QuotYuanL id QuotYuanR)?)											//     _a is EnumA (_val)
							;
strongExpr:					strongExprPrefix* strongExprBase strongExprSuffix*;
middleExpr:					strongExpr (allOp2 strongExpr)*;												//     a == 24    a + b - c
//expr:						middleExpr ((Qus strongExprBase Colon strongExprBase) | (allAssign middleExpr)*); // ������Ŀ�����?:
expr:						middleExpr (allAssign middleExpr)*;



//
// define variable
//
tmpAssignExpr:				Assign middleExpr Semi;
idAssignExpr:				id Assign expr;
defVarStmt:					type idAssignExpr (Comma idAssignExpr)* Semi;



//
// stmt
//
normalStmt:					((Return? expr?) | Break | Continue) Semi;
stmt:						ifStmt | whileStmt | whileStmt2 | forStmt | forStmt2 | quotStmtPart | switchStmt2 | switchStmt | defVarStmt | normalStmt;



//
// class
//
publicLevel:				Public | Internal | Protected | Private;
classTemplates:				QuotJianL type (Comma type)* QuotJianR;
classParent:				Colon ids (Comma ids)*;
enumStmt:					publicLevel? Enum id classTemplates? QuotHuaL (classEnum Comma)* classEnum (Comma classFunc*)? QuotHuaR;
classStmt:					publicLevel? Class id classTemplates? classParent? QuotHuaL (classVar | classFunc)* QuotHuaR;
//
classVarExtFunc:			publicLevel? id (Semi | classFuncBody);
classVarExt:				QuotHuaL classVarExtFunc+ QuotHuaR tmpAssignExpr?;
classVar:					publicLevel? Static? type id (Semi | tmpAssignExpr | classVarExt);
//
classFuncName:				id ((QuotFangL QuotFangR) | allOp2 | allAssign)?;
classFuncBody:				(exprFuncDef expr Semi) | (QuotHuaL stmt* QuotHuaR);
classFunc:					publicLevel? Static? type classFuncName QuotYuanL typeVarList? QuotYuanR classFuncBody;
//
classEnum:					id (QuotYuanL type QuotYuanR)?;



//
// file
//
useStmt:					Use ids Semi;
callConvention:				CC__Cdecl | CC__FastCall | CC__StdCall;
importStmt:					AImport type callConvention id QuotYuanL typeVarList QuotYuanR Semi;
libStmt:					ALib String1Literal Semi;
namespaceStmt:				Namespace ids Semi;
program:					(useStmt | importStmt | libStmt | namespaceStmt)* (enumStmt | classStmt)*;



//
// entry
//
programEntry:				program EOF;
classFuncEntry:				classFunc EOF;
typeEntry:					type EOF;



//
// skips
//
Comment1:					'/*' .*? '*/' -> channel (HIDDEN);
Comment2:					'//' ~ [\r\n]* -> channel (HIDDEN);
WS:							[ \t\r\n]+ -> channel (HIDDEN);
