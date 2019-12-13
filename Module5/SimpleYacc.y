%{
// Ёти объ€влени€ добавл€ютс€ в класс GPPGParser, представл€ющий собой парсер, генерируемый системой gppg
    public Parser(AbstractScanner<int, LexLocation> scanner) : base(scanner) { }
%}

%output = SimpleYacc.cs

%namespace SimpleParser

%token BEGIN END CYCLE INUM RNUM ID ASSIGN SEMICOLON FOR TO DO IF THEN ELSE WHILE REPEAT UNTIL WRITE BRACKET_O BRACKET_C COMMA VAR MINUS PLUS MULT DELIM

%%

progr   : block
		;

stlist	: statement 
		| stlist SEMICOLON statement 
		;

statement: assign
		| block  
		| cycle  
		| for
		| if
		| while
		| repeat
		| write
		| var
		;

ident 	: ID 
		;

idents	: ident
		| ident COMMA idents
		;
	
assign 	: ident ASSIGN expr 
		;

expr	: ident  
		| INUM 
		| math_expr
		;

math_op : PLUS
		| MINUS
		| MULT
		| DELIM
		;

math_expr	: expr math_op expr
			| BRACKET_O expr BRACKET_C
			;

block	: BEGIN stlist END 
		;

cycle	: CYCLE expr statement 
		;

for		: FOR assign TO expr DO statement
		;

if		: IF expr THEN statement ELSE statement
		;

while	: WHILE expr DO statement
		;

repeat	: REPEAT stlist UNTIL expr
		;

write	: WRITE BRACKET_O expr BRACKET_C
		;

var		: VAR idents
		;

%%
