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
    public partial class Parser
    {
        public static QS._qss_p_.Structure_.ILibrary_ _Parse(string _text)
        {
            QS._qss_p_.Parser_.ErrorHandler _handler = new QS._qss_p_.Parser_.ErrorHandler();
            QS._qss_p_.Parser_.Scanner _scanner = new QS._qss_p_.Parser_.Scanner();
            _scanner.Handler = _handler;
            _scanner.SetSource(_text, 0);
            QS._qss_p_.Parser_.Parser _parser = new QS._qss_p_.Parser_.Parser();
/*
            _parser.MBWInit(null);
*/ 
            _parser.scanner = _scanner;
            _parser.SetHandler(_handler);
            _parser.Trace = false;
            if (!_parser.Parse())
            {
                List<QS._qss_p_.Parser_.Error> _errors = _handler.SortedErrorList();
                StringBuilder _sb = new StringBuilder();
                _sb.AppendLine("There were " + _errors.Count.ToString() + " errors.\n");
                bool _isfirst = true;
                foreach (QS._qss_p_.Parser_.Error _error in _errors)
                {
                    if (_isfirst)
                        _isfirst = false;
                    else
                        _sb.AppendLine();
                    _sb.AppendLine("Error in line " + _error.line.ToString() + ", at column " + _error.column.ToString() + ".");
                    int _from = 0, _lines = 1;
                    while (_lines < _error.line)
                    {
                        if (_text[_from] == '\n')
                            _lines++;
                        _from++;
                    }
                    int _count = 1;
                    while ((_from + _count) < _text.Length && _text[_from + _count] != '\n')
                        _count++;
                    _sb.AppendLine(_text.Substring(_from, _count).Replace('\t', ' '));
                    for (int _k = 1; _k <= _error.column; _k++)
                        _sb.Append(".");
                    _sb.AppendLine("^");
                    _sb.AppendLine("\nError: " + _error.message);
                }
                throw new Exception(_sb.ToString());
            }
            else
            {
                return _parser._Library;
            }
        }
    }
}
