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

namespace QS._qss_p_.Printing_
{
    public sealed class Printer_ : IPrinter_
    {
        #region Constructor

        public Printer_(Type_ _type)
        {
            this._type = _type;
        }

        #endregion

        #region Fields

        private StringBuilder _text = new StringBuilder();
        private int _margin = 0;
        private int _tabulation = 4;
        private bool _isnewline = true;
        private Type_ _type;

        #endregion

        #region Type_

        public enum Type_
        {
            _Source, _Compiled
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            return _text.ToString();
        }

        #endregion

        #region IPrinter_ Members

        bool IPrinter_._Blank
        {
            get { return this._isnewline; }
        }

        Printer_.Type_ IPrinter_._Type
        {
            get { return this._type; }
        }

        void IPrinter_._Shift(int _levels)
        {
            this._margin += this._tabulation * _levels;
        }

        void IPrinter_._Print(string _text)
        {
            foreach (char c in _text)
            {
                if (c == '\n')
                {
                    this._text.Append(c);
                    this._isnewline = true;
                }
                else
                {
                    if (this._isnewline)
                    {
                        this._text.Append(new string(' ', this._margin));
                        this._isnewline = false;
                    }
                    this._text.Append(c);
                }
            }
        }

        #endregion
    }
}
