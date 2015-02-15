/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/
%using QS._qss_p_.ParserGenerator_
%namespace QS._qss_p_.Parser_
%valuetype LexValue
%partial

%union 
{
    public QS._qss_p_.Structure_.IAssignment_ _assignment;
    public QS._qss_p_.Structure_.ICode_ _code;    
    public QS._qss_p_.Structure_.IDeclaration_ _declaration;
    public List<QS._qss_p_.Structure_.IDeclaration_> _declarations;
    public QS._qss_p_.Structure_.IDependency_ _dependency;
    public QS._qss_p_.Structure_.IEmbedded_ _embedded;
    public List<QS._qss_p_.Structure_.IEmbedded_> _embeddeds;
    public QS._qss_p_.Structure_.IEmbedding_ _embedding;
    public QS._qss_p_.Structure_.IEmbeddedFlow_ _embeddedflow;
    public QS._qss_p_.Structure_.IEndpoint_ _endpoint;
    public QS._qss_p_.Structure_.IEndpointClass_ _endpointclass;
    public List<QS._qss_p_.Structure_.IEndpoint_> _endpoints;
    public QS._qss_p_.Structure_.IExpression_ _expression;
    public QS._qss_p_.Structure_.IFlow_ _flow;
    public QS._qss_p_.Structure_.IFlowClass_ _flowclass;
    public List<QS._qss_p_.Structure_.IFlow_> _flows;
	public string _identifier;
    public List<string> _identifiers;
    public QS._qss_p_.Structure_.IInterfaceClass_ _interfaceclass;
    public QS._qss_p_.Structure_.ILibrary_ _library;
    public QS._qss_p_.Structure_.IMetadata_ _metadata;
    public int _number;
    public QS._qss_p_.Structure_.IObject_ _object;
    public QS._qss_p_.Structure_.IObjectClass_ _objectclass;
    public QS._qss_p_.Structure_.IOperation_ _operation;
    public QS._qss_p_.Structure_.IOperator_ _operator;
    public QS._qss_p_.Structure_.IOperatorClass_ _operatorclass;
    public QS._qss_p_.Structure_.Operator_.Type_ _operatortype;
    public QS._qss_p_.Structure_.IParameter_ _parameter;
    public List<QS._qss_p_.Structure_.IParameter_> _parameters;    
    public QS._qss_p_.Structure_.Properties_ _properties;
    public QS._qss_p_.Structure_.IStatement_ _statement;
    public List<QS._qss_p_.Structure_.IStatement_> _statements;
	public string _string;
	public string _uuid;
	public QS._qss_p_.Structure_.IValue_ _value;
    public List<QS._qss_p_.Structure_.IValue_> _values;
    public QS._qss_p_.Structure_.IValueClass_ _valueclass;
    public List<QS._qss_p_.Structure_.IValueClass_> _valueclasses;
}

%{
    ErrorHandler handler = null;
    public void SetHandler(ErrorHandler hdlr) { handler = hdlr; }
    internal void CallHdlr(string msg, LexLocation val)
    {
        handler.AddError(msg, val.sLin, val.sCol, val.eCol - val.sCol);
    }

	internal QS._qss_p_.Structure_.IEnvironment_ _environment = new QS._qss_p_.Structure_.Environment_(null);
	internal List<QS._qss_p_.Structure_.IUnresolved_> _unresolved = new List<QS._qss_p_.Structure_.IUnresolved_>();
	internal Stack<QS._qss_p_.Structure_.IMetadata_> _metadata_stack = new Stack<QS._qss_p_.Structure_.IMetadata_>();
	internal Stack<IEnumerable<QS._qss_p_.Structure_.IParameter_>> _parameters_stack = new Stack<IEnumerable<QS._qss_p_.Structure_.IParameter_>>();
	internal List<QS._qss_p_.Structure_.IDeclaration_> _automatic_declarations = new List<QS._qss_p_.Structure_.IDeclaration_>();
	internal QS._qss_p_.Structure_.ILibrary_ _library;
	internal QS._qss_p_.Structure_.ILibrary_ _Library
	{
		get { return this._library; }
	}	    
	
	void _GenerateClassParametersAndDeclarations(IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters,
		out IEnumerable<QS._qss_p_.Structure_.IParameter_> _class_parameters, out IEnumerable<QS._qss_p_.Structure_.IDeclaration_> _class_declarations)
	{		
		if (_parameters != null)
		{
			List<QS._qss_p_.Structure_.IParameter_> _list_of_class_parameters = new List<QS._qss_p_.Structure_.IParameter_>();
			List<QS._qss_p_.Structure_.IDeclaration_> _list_of_class_declarations = new List<QS._qss_p_.Structure_.IDeclaration_>();
			foreach (QS._qss_p_.Structure_.IParameter_ _parameter in _parameters)
			{
				if (((QS._qss_p_.Structure_.IDeclaration_) _parameter)._Type != QS._qss_p_.Structure_.Declaration_.Type_._Value)
				{
					_list_of_class_parameters.Add(_parameter);
					_list_of_class_declarations.Add(new QS._qss_p_.Structure_.Declaration_Reference_(_parameter, null));
				}
			}
			_class_parameters = _list_of_class_parameters;
			_class_declarations = _list_of_class_declarations;
		}
		else
		{
			_class_parameters = null;
			_class_declarations = null;
		}
	}			
/*	
	QS._qss_p_.Structure_.Flow_.Type_ _flowtype = QS._qss_p_.Structure_.Flow_.Type_._Embedded;
*/	
%}

%token <_identifier> IDENTIFIER 
%token <_number> NUMBER 
%token <_string> STRING
%token <_uuid> UUID

%token KWBOOL KWCLASS KWCONJUNCTION KWCONST KWDISJUNCTION KWDOWN KWELSEWHERE KWEMPTY KWENDPOINT 
%token KWFALSE KWFRESH KWID  KWINCOMPLETE KWINDEPENDENTLY KWINT KWINTERFACE KWINTERSECTION KWLIBRARY 
%token KWMAX KWMESSAGE KWMIN KWOBJECT KWOPERATOR KWOTHER KWPRODUCT KWSAME KWSINGLETON KWSOME KWSTRONG 
%token KWSUM KWTRUE KWUNCOORDINATED KWUNION KWUNORDERED KWUP KWUUID KWVERSION KWVALUE KWWEAK KWWHERE

%token EQ NEQ GT GTE LT LTE AMPAMP BARBAR ASSIGN DOTDOT
%token maxParseToken 
%token LEX_WHITE LEX_COMMENT LEX_ERROR

%left '['
%left UMINUS
%left '*' '/'
%left '+' '-'
%nonassoc DOTDOT
%nonassoc GT GTE LT LTE
%nonassoc EQ NEQ
%left AMPAMP BARBAR
%nonassoc ASSIGN
%nonassoc KWELSEWHERE
%nonassoc LOWER_THAN_ELSEWHERE

%type <_assignment> Assignment Assignment_Body
	/* %type <_code> Code */
%type <_declaration> Declaration Declaration_Reference Parameter_Value
%type <_declarations> Declarations Declarations_Body Parameters_Values Parameters_Values_Optional
	/* %type <_dependency> Dependency */
	/* %type <_embedded> Embedded */
	/* %type <_embeddeds> Embeddeds */
	/* %type <_embeddedflow> EmbeddedFlow */
	/* %type <_endpoint> Endpoint */
%type <_endpointclass> EndpointClass_Declaration EndpointClass_Functional /* EndpointClass */
	/* %type <_endpoints> Endpoints */
	/* %type <_expression> Expression */
	/* %type <_flow> Flow */
	/* %type <_flowclass> FlowClass */
	/* %type <_flows> Flows */
	/* %type <_identifiers> Identifiers */
%type <_interfaceclass> InterfaceClass_Declaration
	/* %type <_interfaceclass> InterfaceClass InterfaceClass2 InterfaceClass3 */
%type <_library> Library
%type <_metadata> Metadata Metadata_Internal
%type <_object> Object_Declaration Object_Declaration_Body
%type <_objectclass> ObjectClass_Declaration
%type <_operator> Operator_Declaration Operator_Declaration_Body
%type <_operatorclass> OperatorClass_Declaration OperatorClass_Declaration_Body
	/* %type <_operatortype> AggregationOperator */
%type <_parameter> Parameter_Declaration Parameter_Declaration_Value Parameter_Declaration_Class
%type <_parameters> Parameters_Declarations Parameters_Declarations_Optional Parameters_Declarations_Classes Parameters_Declarations_Classes_Optional 
	/* %type <_properties> Property Properties */
	/* %type <_statement> Statement Statement2 */
	/* %type <_statements> Statements */
%type <_value> Value_Incoming Value_Internal Value_Member Value_Outgoing Value_Reference Value_Declaration Value_Declaration_Body
%type <_values> Values_Incoming Values_Incoming_Body Values_Internal Values_Internal_Body Values_Members Values_Members_Commas Values_Members_Commas_Body Values_Outgoing Values_Outgoing_Body Values_References
%type <_valueclass> ValueClass ValueClass_Declaration ValueClass_Declaration_Body ValueClass_Reference ValueClass_Unambiguous
%type <_valueclasses> ValueClasses

%start Library

%%

/* ....................................................................................................

AggregationAttribute
	: KWUNORDERED
		{
		}
	| KWINCOMPLETE
		{
		}
	| KWUNCOORDINATED
		{
		}
	;
*/

/* ....................................................................................................

AggregationAttributes
	: AggregationAttribute AggregationAttributes
		{
		}
	| AggregationAttribute
		{
		}
	;
*/

/* ....................................................................................................
	
AggregationOperator
	: KWUNION
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Union);
		}
	| KWINTERSECTION
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Intersection);
		}
	| KWSUM
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Addition);
		}
	| KWPRODUCT
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Multiplication);
		}
	| KWMIN
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Minimum);
		}
	| KWMAX
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Maximum);
		}
	| KWCONJUNCTION
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Conjunction);
		}
	| KWDISJUNCTION
		{
			$$ = QS._qss_p_.Structure_.Operator_._Predefined(QS._qss_p_.Structure_.Operator_.Predefined_._Disjunction);
		}
	;
*/

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Assignment
	: Values_References '=' 
		{
		}
		Assignment_Body
		{
			$$ = $4;
		}
	;		

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Assignment_Body
	: 
		{
			$$ = null;
		}
	;

/* ....................................................................................................

Code
	: Embeddeds Statements
		{
			$$ = new QS._qss_p_.Structure_.Code_($1, $2);
		}
	;	
*/
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Declaration
	: ValueClass_Declaration
		{
			$$ = $1;
		}		
	| OperatorClass_Declaration
		{
			$$ = $1;
		}				
	| InterfaceClass_Declaration
		{
			$$ = $1;
		}		
	| EndpointClass_Declaration
		{
			$$ = $1;
		}		
	| ObjectClass_Declaration
		{		
			$$ = $1;
		}
	| Value_Declaration
		{
			$$ = $1;
		}				
	| Operator_Declaration
		{
			$$ = $1;
		}				
	| Object_Declaration
		{		
			$$ = $1;
		}		
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	
Declarations
    : Declarations_Body Declarations
		{
			$$ = $1;
			$$.AddRange($2);
		}
    | Declarations_Body
		{ 
			$$ = $1;
		}
    ;			

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Declarations_Body
	: Declaration
		{
			$$ = new List<QS._qss_p_.Structure_.IDeclaration_>();
			foreach (QS._qss_p_.Structure_.IDeclaration_ _declaration in this._automatic_declarations)			
			{
				$$.Add(_declaration);
				/* this._environment._Add	(_declaration); */
			}
			this._automatic_declarations.Clear();
			$$.Add($1);
			/* this._environment._Add	($1); */
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Declaration_Reference
	: IDENTIFIER Parameters_Values_Optional
		{
			QS._qss_p_.Structure_.Declaration_Reference_ _reference = new QS._qss_p_.Structure_.Declaration_Reference_(this._environment, $1, $2);
			this._unresolved.Add(_reference);
			$$ = _reference; 
		} 
	;		

/* ....................................................................................................
	
Dependency
	: IDENTIFIER '=' Expression
		{						
			$$ = new QS._qss_p_.Structure_.Assignment_(
				this._environment._Get<QS._qss_p_.Structure_.IFlow_>($1, QS._qss_p_.Structure_.Declaration_.Type_._Flow), $3);
		}
	| IDENTIFIER '=' OptionalEmbeddingAttribute IDENTIFIER OptionalEndpointIdentifier '(' Expression ')'
		{
		}
	| IDENTIFIER ',' Identifiers '=' OptionalEmbeddingAttribute IDENTIFIER OptionalEndpointIdentifier '(' Expression ')'
		{
		}
	;	
*/

/* ....................................................................................................

Embedded
	: EmbeddedFlow
		{
			$$ = $1;
		}
	;
*/

/* ....................................................................................................

Embeddeds
    : Embedded ';' Embeddeds
		{
			$$ = new List<QS._qss_p_.Structure_.IEmbedded_>();
			$$.Add($1);
			$$.AddRange($3);
		}
    | 
		{ 
			$$ = new List<QS._qss_p_.Structure_.IEmbedded_>();
		}
    ;	
*/

/* ....................................................................................................
        
EmbeddedFlow
	: Flow
		{
			$$ = new QS._qss_p_.Structure_.EmbeddedFlow_($1, null);
		}
	| Flow '=' Expression
		{
			$$ = new QS._qss_p_.Structure_.EmbeddedFlow_($1, null);
		}
	;
*/

/* ....................................................................................................

Endpoint
	: KWENDPOINT IDENTIFIER EndpointClass '{' Code '}'
		{
			$$ = new QS._qss_p_.Structure_.Endpoint_($2, $3, $5);
		}
	;
*/

EndpointClass_Functional
	: 
		{
			$$ = new QS._qss_p_.Structure_.EndpointClass_DualInterface_
			(
				QS._qss_p_.Structure_.InterfaceClass_Primitive_._Interface,
				QS._qss_p_.Structure_.InterfaceClass_Primitive_._Interface
			);
		}
/*	
	'(' InterfaceClass2 ')' 
		{
			throw new NotImplementedException();	
			$$ = new QS._qss_p_.Structure_.EndpointClass_($2, null);
		}
	| '(' InterfaceClass2 ')' ':' InterfaceClass3
		{
			throw new NotImplementedException();
			$$ = new QS._qss_p_.Structure_.EndpointClass_($2, $5);
		}
*/
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

EndpointClass_Declaration
	: KWENDPOINT KWCLASS Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
		}	
		Parameters_Declarations_Classes_Optional '{' '}'
		{
			throw new NotImplementedException();
/* ....................................................................................................			
			QS._qss_p_.Structure_.EndpointClass_Custom_ _endpointclass = new QS._qss_p_.Structure_.EndpointClass_Custom_($3._Name, $3, $5);
			$$ = _endpointclass;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_endpointclass);
*/			
		}
	;	

/* ....................................................................................................

Endpoints
    : Endpoint Endpoints
		{
			$$ = new List<QS._qss_p_.Structure_.IEndpoint_>();
			$$.Add($1);
			$$.AddRange($2);		
		}
    | Endpoint
		{ 
			$$ = new List<QS._qss_p_.Structure_.IEndpoint_>();
			$$.Add($1);
		}
    ;			
*/

/* ....................................................................................................

Expression
	: IDENTIFIER
		{
		}
	| FlowModifier IDENTIFIER
		{
		}
	| KWTRUE
		{
		}
	| KWFALSE
		{
		}
	| NUMBER
		{
		}
	| KWEMPTY
		{
		}
	| KWID
		{
		}
	| '-' Expression %prec UMINUS
		{
		}
	| '!' Expression %prec UMINUS
		{
		}
	| Expression '+' Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Addition;
		}
	| Expression '-' Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Substraction;		
		}
	| Expression '*' Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Multiplication;		
		}
	| Expression '/' Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Division;		
		}
	| Expression EQ Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Equals;		
		}
	| Expression NEQ Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._NotEquals;		
		}
	| Expression GT Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._GreaterThan;		
		}
	| Expression GTE Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._GreaterThanOrEquals;		
		}
	| Expression LT Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._LessThan;		
		}
	| Expression LTE Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._LessThanOrEquals;		
		}
	| Expression AMPAMP Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Conjunction;		
		}
	| Expression BARBAR Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Disjunction;		
		}
	| Expression DOTDOT Expression
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Range;		
		}
	| '(' Expression ',' Expressions ')'
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Tuple;		
		}
	| '{' Expressions '}'
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Set;		
		}
	| Expression '[' NUMBER ']'
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = QS._qss_p_.Structure_.Operator_.Type_._Element;		
		}
	| '(' Expression ')'
		{
			$$ = $2;
		}
	| AggregationOperator '(' Expressions ')'
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = $1;		
		}
	| AggregationOperator IDENTIFIER
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = $1;		
		}
	| AggregationAttributes AggregationOperator IDENTIFIER
		{
			QS._qss_p_.Structure_.Operator_.Type_ _operatortype = $2;		
		}
	| KWSINGLETON
		{		
		}				
	;
*/		
	
/* ....................................................................................................

Expressions
	: Expression ',' Expressions	
		{
		}
	| Expression
		{
		}
	;
*/

/* ....................................................................................................

Flow
	: FlowClass IDENTIFIER
		{
			QS._qss_p_.Structure_.IFlow_ _flow = new QS._qss_p_.Structure_.Flow_(this._flowtype, $2, $1);
			this._environment._Add(_flow);
			$$ = _flow;
		}
	;
*/

/* ....................................................................................................

FlowClass
	: Properties ValueClass 
		{
			$$ = new QS._qss_p_.Structure_.FlowClass_($2, $1);
		}
	;
*/

/* ....................................................................................................

Flows
    : Flow ',' Flows
		{
			$$ = new List<QS._qss_p_.Structure_.IFlow_>();
			$$.Add($1);
			$$.AddRange($3);		
		}
    | Flow
		{ 
			$$ = new List<QS._qss_p_.Structure_.IFlow_>();
			$$.Add($1);
		}
    ;			
*/

/* ....................................................................................................

FlowModifier
	: KWOTHER
		{
		}
	| KWFRESH
		{
		}
	| KWSOME
		{
		}
	;
*/

/* ....................................................................................................

Identifiers
	: IDENTIFIER ',' Identifiers
		{
		}
	| IDENTIFIER
		{
		}
	;
*/

/* ....................................................................................................

InterfaceClass
	: Flows
		{
			throw new NotImplementedException();
			$$ = new QS._qss_p_.Structure_.InterfaceClass_($1);
		}
	;
*/

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

InterfaceClass_Declaration
	: KWINTERFACE KWCLASS Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
		}	
		Parameters_Declarations_Classes_Optional '{' '}'
		{
			QS._qss_p_.Structure_.InterfaceClass_Custom_ _interfaceclass = new QS._qss_p_.Structure_.InterfaceClass_Custom_($3._Name, $3, $5);
			$$ = _interfaceclass;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_interfaceclass);
		}
	;	
	
/* ....................................................................................................

InterfaceClass2
	: 
		{ 
			this._flowtype = QS._qss_p_.Structure_.Flow_.Type_._Incoming; 
		} 	 
		InterfaceClass
		{
			$$ = $2;
		}
	;
*/

/* ....................................................................................................

InterfaceClass3
	: 
		{ 
			this._flowtype = QS._qss_p_.Structure_.Flow_.Type_._Outgoing; 
		} 	 
		InterfaceClass
		{
			$$ = $2;
		}
	;
*/
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	
Library
	: KWLIBRARY Metadata '{' Declarations '}'
		{
			this._library = new QS._qss_p_.Structure_.Library_($2._Name, $2, $4);
			foreach (QS._qss_p_.Structure_.IUnresolved_ _unresolved in this._unresolved)
				_unresolved._Resolve();
			$$ = this._library;
		}
	;				

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Metadata
	: Metadata_Internal
		{
			$$ = $1;
		}
	| UUID IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_($1, "0", $2, null);
		}
	| UUID STRING IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_($1, "0", $3, $2);
		}
	| UUID '`' NUMBER IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_($1, $3.ToString(), $4, null);
		}
	| UUID '`' NUMBER STRING IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_($1, $3.ToString(), $5, $4);
		}
	;
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Metadata_Internal		
	: IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", $1, string.Empty);
		}
	| STRING IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", $2, $1);
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Object_Declaration
	: KWOBJECT Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
			this._metadata_stack.Push($2);
		}
		Parameters_Declarations_Optional 
		{
			this._parameters_stack.Push($4);
		}
		Object_Declaration_Body
		{
			QS._qss_p_.Structure_.IObject_ _object = $6;
			$$ = _object;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_object);
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	
Object_Declaration_Body
	: EndpointClass_Functional '{' /* Code */ '}'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _class_parameters;
			IEnumerable<QS._qss_p_.Structure_.IDeclaration_> _class_declarations;
			_GenerateClassParametersAndDeclarations(_parameters, out _class_parameters, out _class_declarations);
			string _identifier = _metadata._Name;
			List<QS._qss_p_.Structure_.IEndpoint_> _objectclass_endpoints = new List<QS._qss_p_.Structure_.IEndpoint_>();
			_objectclass_endpoints.Add(new QS._qss_p_.Structure_.Endpoint_Member_(null, null, $1));			
			QS._qss_p_.Structure_.IObjectClass_ _objectclass = 
				new QS._qss_p_.Structure_.ObjectClass_Custom_(
					_identifier, new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", _identifier, string.Empty), _class_parameters, _objectclass_endpoints);
			this._automatic_declarations.Add(_objectclass);			
			List<QS._qss_p_.Structure_.IEndpoint_> _object_endpoints = new List<QS._qss_p_.Structure_.IEndpoint_>();
			_object_endpoints.Add(new QS._qss_p_.Structure_.Endpoint_Internal_(null, null, $1));						
			$$ = new QS._qss_p_.Structure_.Object_Custom_(
				_metadata._Name, _metadata, _parameters, new QS._qss_p_.Structure_.Declaration_Reference_(_objectclass, _class_declarations), _object_endpoints);
		}
/* ....................................................................................................	
	| '{' Endpoints Code '}'
		{			
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			$$ = new QS._qss_p_.Structure_.Object_Custom_(_metadata._Name, _metadata, _parameters, $2, $3);
		}
*/
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
ObjectClass_Declaration
	: KWOBJECT KWCLASS Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
		}	
		Parameters_Declarations_Classes_Optional '{' '}'
		{
			QS._qss_p_.Structure_.ObjectClass_Custom_ _objectclass = new QS._qss_p_.Structure_.ObjectClass_Custom_($3._Name, $3, $5, null);
			$$ = _objectclass;
			this._environment = this._environment._Environment;					
			this._environment._Add	(_objectclass);
		}
	;	

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Operator_Declaration
	: KWOPERATOR Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
			this._metadata_stack.Push($2);			
		}	
		Parameters_Declarations_Optional
		{
			this._parameters_stack.Push($4);				
		}
		Operator_Declaration_Body
		{
			QS._qss_p_.Structure_.IOperator_ _operator = $6;
			$$ = _operator;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_operator);		
		}
	;		
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Operator_Declaration_Body
	: '(' Values_Incoming ')' Values_Outgoing '{' Values_Internal '}'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _class_parameters;
			IEnumerable<QS._qss_p_.Structure_.IDeclaration_> _class_declarations;
			_GenerateClassParametersAndDeclarations(_parameters, out _class_parameters, out _class_declarations);
			string _identifier = _metadata._Name;
			QS._qss_p_.Structure_.IOperatorClass_ _operatorclass = 
				new QS._qss_p_.Structure_.OperatorClass_Custom_(
					_identifier, new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", _identifier, string.Empty), _class_parameters, $2, $4);
			this._automatic_declarations.Add(_operatorclass);			
			$$ = new QS._qss_p_.Structure_.Operator_Custom_(
				_metadata._Name, _metadata, _parameters, new QS._qss_p_.Structure_.Declaration_Reference_(_operatorclass, _class_declarations), $2, $4, $6);
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

OperatorClass_Declaration
	: KWOPERATOR KWCLASS Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
			this._metadata_stack.Push($3);			
		}	
		Parameters_Declarations_Classes_Optional 
		{
			this._parameters_stack.Push($5);		
		}
		OperatorClass_Declaration_Body
		{
			QS._qss_p_.Structure_.IOperatorClass_ _operatorclass = $7;
			$$ = _operatorclass;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_operatorclass);
		}
	;
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

OperatorClass_Declaration_Body
	: '(' Values_Incoming ')' Values_Outgoing ';'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			$$ = new QS._qss_p_.Structure_.OperatorClass_Custom_(_metadata._Name, _metadata, _parameters, $2, $4);
		}
	;
	
/* ....................................................................................................
					
OptionalEmbeddingAttribute 
	: KWINDEPENDENTLY
		{
		}
	|
		{
		}
	;
*/

/* ....................................................................................................

OptionalEndpointIdentifier
	: '.' IDENTIFIER
		{
		}
	|
		{
		}
	;
*/
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Parameter_Declaration
	: Parameter_Declaration_Value
		{
			$$ = $1;
		}
	| Parameter_Declaration_Class
		{
			$$ = $1;
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
			
Parameter_Declaration_Class
	: KWVALUE KWCLASS IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Declaration_Parameter_($3, QS._qss_p_.Structure_.Declaration_.Type_._ValueClass, null);			
		}
	| KWVALUE '~' IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Declaration_Parameter_($3, QS._qss_p_.Structure_.Declaration_.Type_._ValueClass, null);			
		}
	| ValueClass_Unambiguous '~' IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Declaration_Parameter_($3, QS._qss_p_.Structure_.Declaration_.Type_._ValueClass, $1);			
		}
	| Declaration_Reference '~' IDENTIFIER
		{
			QS._qss_p_.Structure_.Declaration_Parameter_ _parameter = new QS._qss_p_.Structure_.Declaration_Parameter_($3, $1);
			$$ = _parameter;
			this._unresolved.Add(_parameter);			
		}				
/* ....................................................................................................
*/		
	;		

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
			
Parameter_Declaration_Value
	: ValueClass_Unambiguous IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Declaration_Parameter_($2, QS._qss_p_.Structure_.Declaration_.Type_._Value, $1);			
		}
	| Declaration_Reference IDENTIFIER
		{
			$$ = new QS._qss_p_.Structure_.Declaration_Parameter_($2, QS._qss_p_.Structure_.Declaration_.Type_._Value, $1);
		}
/* ....................................................................................................
*/		
	;
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Parameters_Declarations
	: Parameter_Declaration ',' Parameters_Declarations
		{
			$$ = new List<QS._qss_p_.Structure_.IParameter_>();
			$$.Add($1);
			$$.AddRange($3);	
			this._environment._Add	($1);
		}
	| Parameter_Declaration
		{
			$$ = new List<QS._qss_p_.Structure_.IParameter_>();
			$$.Add($1);
			this._environment._Add	($1);
		}
	;
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Parameters_Declarations_Optional
	: LT Parameters_Declarations GT
		{
			$$ = $2;
		}
	| 
		{
			$$ = null;
		}
	;	
				
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Parameters_Declarations_Classes
	: Parameter_Declaration_Class ',' Parameters_Declarations_Classes
		{
			$$ = new List<QS._qss_p_.Structure_.IParameter_>();
			$$.Add($1);
			$$.AddRange($3);	
			this._environment._Add	($1);
		}
	| Parameter_Declaration_Class
		{
			$$ = new List<QS._qss_p_.Structure_.IParameter_>();
			$$.Add($1);
			this._environment._Add	($1);
		}
	;
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Parameters_Declarations_Classes_Optional
	: LT Parameters_Declarations_Classes GT
		{
			$$ = $2;
		}
	| 
		{
			$$ = null;
		}
	;	
					
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Parameter_Value
	: ValueClass_Unambiguous
		{
			$$ = $1;
		}
	| Declaration_Reference
		{
			$$ = $1;
		}		
/* ....................................................................................................		
*/		
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Parameters_Values
	: Parameter_Value ',' Parameters_Values
		{
			$$ = new List<QS._qss_p_.Structure_.IDeclaration_>();
			$$.Add($1);
			$$.AddRange($3);		
		}
	| Parameter_Value
		{
			$$ = new List<QS._qss_p_.Structure_.IDeclaration_>();
			$$.Add($1);
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Parameters_Values_Optional
	: LT Parameters_Values GT
		{
			$$ = $2;
		}			
	| 
		{
			$$ = null;
		}
	;		

/* ....................................................................................................
	
Properties
    : Property Properties
		{
			$$ = $1 | $2;
		}
	|
		{
			$$ = QS._qss_p_.Structure_.Properties_._None;
		}		
    ;			
*/

/* ....................................................................................................

Property
	: KWSAME
		{
			$$ = QS._qss_p_.Structure_.Properties_._Consistent;
		}
	| KWCONST
		{
			$$ = QS._qss_p_.Structure_.Properties_._Constant;
		}
	| KWUP
		{
			$$ = QS._qss_p_.Structure_.Properties_._Increasing;		
		}
	| KWDOWN
		{
			$$ = QS._qss_p_.Structure_.Properties_._Decreasing;		
		}
	| KWWEAK
		{
			$$ = QS._qss_p_.Structure_.Properties_._None;		
		}
	| KWSTRONG 
		{
			$$ = QS._qss_p_.Structure_.Properties_._Strongly;		
		}		
	;
*/

/* ....................................................................................................

Statement
	: Statement2
		{
			$$ = $1;
		}
	| '{' 
		{ 
			this._flowtype = QS._qss_p_.Structure_.Flow_.Type_._Embedded; 
		} 
		Code '}'
		{
			$$ = $3;
		}
	;
*/

/* ....................................................................................................

Statement2
	: Dependency ';'
		{
			$$ = $1;
		}
	| KWWHERE '(' Expression ')' Statement KWELSEWHERE Statement 
		{
			$$ = new QS._qss_p_.Structure_.Conditional_($3, $5, $7);
		}
	| KWWHERE '(' Expression ')' Statement %prec LOWER_THAN_ELSEWHERE
		{
			$$ = new QS._qss_p_.Structure_.Conditional_($3, $5, null);
		}
	;
*/

/* ....................................................................................................

Statements
    : Statement2 Statements
		{
			$$ = new List<QS._qss_p_.Structure_.IStatement_>();
			$$.Add($1);
			$$.AddRange($2);		
		}
    |  
		{ 
			$$ = new List<QS._qss_p_.Structure_.IStatement_>();
		}
    ;	    
*/

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
Value_Declaration
	: KWVALUE Metadata
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
			this._metadata_stack.Push($2);
		}
		Parameters_Declarations_Optional 
		{
			this._parameters_stack.Push($4);
		}
		Value_Declaration_Body
		{
			QS._qss_p_.Structure_.IValue_ _value = $6;
			$$ = _value;
			this._environment = this._environment._Environment;			
			this._environment._Add	(_value);
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	
Value_Declaration_Body
	: '{' Values_Members '}'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _class_parameters;
			IEnumerable<QS._qss_p_.Structure_.IDeclaration_> _class_declarations;
			_GenerateClassParametersAndDeclarations(_parameters, out _class_parameters, out _class_declarations);
			string _identifier = _metadata._Name;
			QS._qss_p_.Structure_.IValueClass_ _valueclass = 
				new QS._qss_p_.Structure_.ValueClass_Custom_(
					_identifier, new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", _identifier, string.Empty), _class_parameters, $2);
			this._automatic_declarations.Add(_valueclass);			
			$$ = new QS._qss_p_.Structure_.Value_Custom_(
				_metadata._Name, _metadata, _parameters, new QS._qss_p_.Structure_.Declaration_Reference_(_valueclass, _class_declarations), $2);
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
    
Value_Incoming
	: ValueClass Metadata_Internal
		{
			$$ = new QS._qss_p_.Structure_.Value_Incoming_($2._Name, $2, $1);
		}
	;
    
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
    
Value_Member
	: ValueClass Metadata_Internal
		{
			$$ = new QS._qss_p_.Structure_.Value_Member_($2._Name, $2, $1);
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
    
Value_Internal
	: ValueClass Metadata_Internal
		{
			$$ = new QS._qss_p_.Structure_.Value_Internal_($2._Name, $2, $1);
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
    
Value_Outgoing
	: ValueClass Metadata_Internal
		{
			$$ = new QS._qss_p_.Structure_.Value_Outgoing_($2._Name, $2, $1);
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Value_Reference
	: IDENTIFIER
		{
			QS._qss_p_.Structure_.Declaration_Reference_ _reference = 
				new QS._qss_p_.Structure_.Declaration_Reference_(this._environment, $1, QS._qss_p_.Structure_.Declaration_.Type_._Value, null);
			this._unresolved.Add(_reference);
			$$ = _reference; 
		} 
	;		
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Values_Incoming
	: Values_Incoming_Body
		{
			$$ = $1;
		}
	| 
		{
			$$ = null;
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_Incoming_Body
	: Value_Incoming ',' Values_Incoming_Body
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
			this._environment._Add($1);		
		}
	| Value_Incoming
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			this._environment._Add($1);		
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Values_Internal
	: Values_Internal_Body
		{
			$$ = $1;
		}
	| 
		{
			$$ = null;
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_Internal_Body
	: Value_Internal ';' Values_Internal_Body
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
			this._environment._Add($1);		
		}
	| Value_Internal ';'
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			this._environment._Add($1);		
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Values_Outgoing
	: ':' Values_Outgoing_Body
		{
			$$ = $2;
		}
	| 
		{
			$$ = null;
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_Outgoing_Body
	: Value_Outgoing ',' Values_Outgoing_Body
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
			this._environment._Add($1);		
		}
	| Value_Outgoing
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			this._environment._Add($1);		
		}
	;
	
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_Members
	: Value_Member ';' Values_Members
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
			this._environment._Add($1);		
		}
	| 
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

Values_Members_Commas
	: Values_Members_Commas_Body
		{
			$$ = $1;
		}
	| 
		{
			$$ = null;
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_Members_Commas_Body
	: Value_Member ',' Values_Members_Commas_Body
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
			this._environment._Add($1);		
		}
	| Value_Member
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			this._environment._Add($1);		
		}
	;
		     
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	    
Values_References
	: Value_Reference ',' Values_References
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
			$$.AddRange($3);		
		}
	| Value_Reference
		{
			$$ = new List<QS._qss_p_.Structure_.IValue_>();
			$$.Add($1);
		}
	;
			     
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

ValueClass
	: ValueClass_Unambiguous
		{
			$$ = $1;
		}
	| ValueClass_Reference
		{
			$$ = $1;
		} 
	;		
		     
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	
ValueClasses
    : ValueClass ',' ValueClasses
		{
			$$ = new List<QS._qss_p_.Structure_.IValueClass_>();
			$$.Add($1);
			$$.AddRange($3);
		}
    | ValueClass
		{
			$$ = new List<QS._qss_p_.Structure_.IValueClass_>();
			$$.Add($1);
		}
    ;	
    		     
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
	     
ValueClass_Declaration
	: KWVALUE KWCLASS Metadata 
		{
			this._environment = new QS._qss_p_.Structure_.Environment_(this._environment);
			this._metadata_stack.Push($3);			
		}	
		Parameters_Declarations_Classes_Optional 
		{
			this._parameters_stack.Push($5);
		}
		ValueClass_Declaration_Body
		{
			QS._qss_p_.Structure_.IValueClass_ _valueclass = $7;
			$$ = _valueclass;			
			this._environment = this._environment._Environment;					
			this._environment._Add	(_valueclass);
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = ((QS._qss_p_.Structure_.IDeclaration_) _valueclass)._Parameters;
			List<QS._qss_p_.Structure_.IDeclaration_> _declarations = null;
			if (_parameters != null)
			{
				_declarations = new List<QS._qss_p_.Structure_.IDeclaration_>();
				foreach (QS._qss_p_.Structure_.IParameter_ _parameter in _parameters)
					_declarations.Add(new QS._qss_p_.Structure_.Declaration_Reference_(_parameter, null));
			}
			string _identifier =  ((QS._qss_p_.Structure_.IDeclaration_) _valueclass)._Metadata._Name;
			QS._qss_p_.Structure_.IValue_ _value = 
				new QS._qss_p_.Structure_.Value_Custom_(
					_identifier, new QS._qss_p_.Structure_.Metadata_(QS.Fx.Base.ID.NewID().ToString(), "0", _identifier, string.Empty), _parameters, 
						new QS._qss_p_.Structure_.Declaration_Reference_(_valueclass, _declarations), 
						((QS._qss_p_.Structure_.IValueClass_Custom_) _valueclass)._Values);
			this._automatic_declarations.Add(_value);			
		}
	;

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

ValueClass_Declaration_Body
	: '{' Values_Members '}'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			$$ = new QS._qss_p_.Structure_.ValueClass_Custom_(_metadata._Name, _metadata, _parameters, $2);
		}
/*		
	| '(' Values_Members_Commas ')' ';'
		{
			QS._qss_p_.Structure_.IMetadata_ _metadata = this._metadata_stack.Pop();
			IEnumerable<QS._qss_p_.Structure_.IParameter_> _parameters = this._parameters_stack.Pop(); 
			$$ = new QS._qss_p_.Structure_.ValueClass_Custom_(_metadata._Name, _metadata, _parameters, $2);
		}
*/		
	;				

/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */

ValueClass_Reference
	: IDENTIFIER Parameters_Values_Optional
		{
			QS._qss_p_.Structure_.Declaration_Reference_ _reference = 
				new QS._qss_p_.Structure_.Declaration_Reference_(this._environment, $1, QS._qss_p_.Structure_.Declaration_.Type_._ValueClass, $2);
			this._unresolved.Add(_reference);
			$$ = _reference; 
		} 
	;		
		
/* @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ */
		
ValueClass_Unambiguous
	: KWBOOL
		{
			$$ = QS._qss_p_.Structure_.ValueClass_Primitive_._Boolean;
		}
	| KWINT
		{
			$$ = QS._qss_p_.Structure_.ValueClass_Primitive_._Integer;
		}
	| '(' ValueClasses ')'
		{
			$$ = new QS._qss_p_.Structure_.ValueClass_Tuple_($2);
		}
	| '{' ValueClass '}'
		{
			$$ = new QS._qss_p_.Structure_.ValueClass_Set_($2);
		}
	;		
    	        
%%



