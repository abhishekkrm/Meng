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

namespace QS._qss_x_.Language_.Output_
{
    public static class Output2
    {
        public static string Format(string template, IDictionary<string, string> substitutions)
        {
            StringBuilder _s = new StringBuilder();
            int _from = 0;
            while (_from < template.Length)
            {
                int _to = template.IndexOf('\n', _from);
                if (_to < _from)
                    _to = template.Length - 1;

                int _indent = 0;
                while (template[_from] == ' ')
                {
                    _s.Append(' ');
                    _from++;
                    _indent++;
                }

                while (_from <= _to)
                {
                    string _replacement = null;
                    foreach (KeyValuePair<string, string> _substitution in substitutions)
                    {
                        if (_substitution.Key.Length <= (_to - _from + 1))
                        {
                            bool _same = true;
                            for (int _k = 0; _same && _k < _substitution.Key.Length; _k++)
                            {
                                if (template[_from + _k] != _substitution.Key[_k])
                                    _same = false;
                            }
                            if (_same)
                            {
                                _from = _from + _substitution.Key.Length;
                                _replacement = _substitution.Value.Trim();
                                break;
                            }
                        }
                    }

                    if (_replacement != null)
                    {
                        for (int _k = 0; _k < _replacement.Length; _k++)
                        {
                            _s.Append(_replacement[_k]);
                            if (_replacement[_k] == '\n')
                            {
                                for (int _i = 0; _i < _indent; _i++)
                                    _s.Append(' ');
                            }
                        }
                    }
                    else
                    {
                        _s.Append(template[_from]);
                        _from++;
                    }
                }
            }
            return _s.ToString();
        }
    }
}
