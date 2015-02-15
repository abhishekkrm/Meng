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
%using QS._qss_p_.Parser_;
%using QS._qss_p_.ParserGenerator_;

%namespace QS._qss_p_.Parser_

%x COMMENT

%{
		int GetIdToken(string txt)
		{
			int _token;
			if (Tokens_._StringToToken(txt, out _token))
				return _token;
			else
				return (int) Tokens.IDENTIFIER;
		}
       
       internal void LoadYylval()
       {
           yylval._identifier = tokTxt;
           yylloc = new LexLocation(tokLin, tokCol, tokLin, tokECol);
       }
       
       public override void yyerror(string s, params object[] a)
       {
           if (handler != null) handler.AddError(s, tokLin, tokCol, tokLin, tokECol);
       }
%}


White0          [ \t\r\f\v]
White           {White0}|\n

CmntStart    \/\*
CmntEnd      \*\/
ABStar       [^\*\n]*

%%

[a-zA-Z][a-zA-Z0-9_]*    { return GetIdToken(yytext); }
[0-9]+                    { yylval._number = Convert.ToInt32(yytext); return (int)Tokens.NUMBER; }
\"[^\"]*\"			{ yylval._string = yytext.Substring(1, yytext.Length - 2); return (int) Tokens.STRING; }
\%[a-fA-F0-9]*		{ yylval._uuid = yytext.Substring(1); return (int) Tokens.UUID; }

\[                         { return (int)'[';    }
\]                         { return (int)']';    }
:                         { return (int)':';    }
;                         { return (int)';';    }
,                         { return (int)',';    }
\(                        { return (int)'(';    }
\)                        { return (int)')';    }
\{                        { return (int)'{';    }
\}                        { return (int)'}';    }
=                         { return (int)'=';    }
\^                        { return (int)'^';    }
\+                        { return (int)'+';    }
\-                        { return (int)'-';    }
\*                        { return (int)'*';    }
\/                        { return (int)'/';    }
\!                        { return (int)'!';    }
@							{ return (int)'@'; }
:=                        { return (int)Tokens.ASSIGN;  }
==                        { return (int)Tokens.EQ;  }
\!=                       { return (int)Tokens.NEQ;   }
\>                        { return (int)Tokens.GT; }
\>=                       { return (int)Tokens.GTE;    }
\<                        { return (int)Tokens.LT;     }
\<=                       { return (int)Tokens.LTE;    }
\&                        { return (int)'&';    }
\&\&                      { return (int)Tokens.AMPAMP; }
\|                        { return (int)'|';    }
\|\|                      { return (int)Tokens.BARBAR; }
\.                        { return (int)'.';    }
`							{ return (int)'`'; }
~							{ return (int)'~'; }
\.\.						{ return (int)Tokens.DOTDOT; }

{CmntStart}{ABStar}\**{CmntEnd} { return (int)Tokens.LEX_COMMENT; } 
{CmntStart}{ABStar}\**          { BEGIN(COMMENT); return (int)Tokens.LEX_COMMENT; }
<COMMENT>\n                     |                                
<COMMENT>{ABStar}\**            { return (int)Tokens.LEX_COMMENT; }                                
<COMMENT>{ABStar}\**{CmntEnd}   { BEGIN(INITIAL); return (int)Tokens.LEX_COMMENT; }

{White0}+                  { return (int)Tokens.LEX_WHITE; }
\n                         { return (int)Tokens.LEX_WHITE; }
.                          { yyerror("illegal char");
                             return (int)Tokens.LEX_ERROR; }

%{
                      LoadYylval();
%}

%%

/* .... */
