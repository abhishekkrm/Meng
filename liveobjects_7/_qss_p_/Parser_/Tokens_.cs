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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_p_.Parser_
{
    public static class Tokens_
    {
        static Tokens_()
        {
            _Register("bool", (int)Tokens.KWBOOL);
            _Register("class", (int)Tokens.KWCLASS);
            _Register("conjunction", (int)Tokens.KWCONJUNCTION);
            _Register("const", (int)Tokens.KWCONST);
            _Register("disjunction", (int)Tokens.KWDISJUNCTION);
            _Register("down", (int)Tokens.KWDOWN);
            _Register("elsewhere", (int)Tokens.KWELSEWHERE);
            _Register("empty", (int)Tokens.KWEMPTY);
            _Register("endpoint", (int)Tokens.KWENDPOINT);
            _Register("false", (int)Tokens.KWFALSE);
            _Register("fresh", (int)Tokens.KWFRESH);
            _Register("id", (int)Tokens.KWID);
            _Register("incomplete", (int)Tokens.KWINCOMPLETE);
            _Register("independently", (int)Tokens.KWINDEPENDENTLY);
            _Register("int", (int)Tokens.KWINT);
            _Register("interface", (int)Tokens.KWINTERFACE);
            _Register("intersection", (int)Tokens.KWINTERSECTION);                                    
            _Register("library", (int)Tokens.KWLIBRARY);
            _Register("max", (int)Tokens.KWMAX);
            _Register("message", (int)Tokens.KWMESSAGE);
            _Register("min", (int)Tokens.KWMIN);
            _Register("object", (int)Tokens.KWOBJECT);
            _Register("operator", (int)Tokens.KWOPERATOR);
            _Register("other", (int)Tokens.KWOTHER);
            _Register("product", (int)Tokens.KWPRODUCT);                
            _Register("same", (int)Tokens.KWSAME);
            _Register("singleton", (int)Tokens.KWSINGLETON);
            _Register("some", (int)Tokens.KWSOME);
            _Register("strong", (int)Tokens.KWSTRONG);
            _Register("sum", (int)Tokens.KWSUM);
            _Register("true", (int)Tokens.KWTRUE);                
            _Register("uncoordinated", (int)Tokens.KWUNCOORDINATED);                
            _Register("union", (int)Tokens.KWUNION);                
            _Register("unordered", (int)Tokens.KWUNORDERED);                
            _Register("up", (int)Tokens.KWUP);                
            _Register("uuid", (int)Tokens.KWUUID);                
            _Register("value", (int)Tokens.KWVALUE);                
            _Register("version", (int)Tokens.KWVERSION);                
            _Register("weak", (int)Tokens.KWWEAK);                
            _Register("where", (int)Tokens.KWWHERE);              
        }

        public static bool _StringToToken(string _string, out int _token)
        {
            return _stringtotoken.TryGetValue(_string, out _token);
        }

        public static bool _TokenToString(int _token, out string _string)
        {
            switch (_token)
            {
                case ((int)QS._qss_p_.Parser_.Tokens.IDENTIFIER):
                    _string = "alphanumeric identifier";
                    return true;
                case ((int)QS._qss_p_.Parser_.Tokens.NUMBER):
                    _string = "numeric constant";
                    return true;
                case ((int)QS._qss_p_.Parser_.Tokens.STRING):
                    _string = "string constant";
                    return true;
                case ((int)QS._qss_p_.Parser_.Tokens.UUID):
                    _string = "universally unique identifier";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.ASSIGN):
                    _string = "\":=\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.EQ):
                    _string = "\"==\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.NEQ):
                    _string = "\"!=\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.GT):
                    _string = "\'>\'";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.GTE):
                    _string = "\">=\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.LT):
                    _string = "\'<\'";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.LTE):
                    _string = "\"<=\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.AMPAMP):
                    _string = "\"&&\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.BARBAR):
                    _string = "\"||\"";
                    return true;
                case ((int) QS._qss_p_.Parser_.Tokens.DOTDOT):
                    _string = "\"..\"";
                    return true;
                default:
                    {
                        if (_tokentostring.TryGetValue(_token, out _string))
                        {
                            _string = "\"" + _string + "\"";
                            return true;
                        }
                        else
                            return false;
                    }
            }
        }

        private static IDictionary<string, int> _stringtotoken = new Dictionary<string, int>();
        private static IDictionary<int, string> _tokentostring = new Dictionary<int, string>();

        private static void _Register(string _string, int _token)
        {
            _stringtotoken.Add(_string, _token);
            _tokentostring.Add(_token, _string);
        }
    }
}
