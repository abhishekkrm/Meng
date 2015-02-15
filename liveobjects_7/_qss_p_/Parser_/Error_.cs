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
using System.IO;
using System.Collections.Generic;
using System.Text;
//using QS._qss_p_.ParserGenerator_;

namespace QS._qss_p_.Parser_
{
    public class Error : IComparable<Error>
    {
        internal const int minErr = 50;
        internal const int minWrn = 100;

        public string message;
        public int line;
        public int column;
        public int length;
        public bool isWarn;

        internal Error(string msg, int lin, int col, int len, bool warningOnly)
        {
            isWarn = warningOnly;
            message = msg;
            line = lin;
            column = col;
            length = len;
        }

        public int CompareTo(Error r)
        {
            if (this.line < r.line) return -1;
            else if (this.line > r.line) return 1;
            else if (this.column < r.column) return -1;
            else if (this.column > r.column) return 1;
            else return 0;
        }

        public bool Equals(Error r)
        {
            return (this.line == r.line && this.column == r.column);
        }

        public void Report()
        {
            Console.WriteLine("Line " + line + ", column  " + column + ": " + message);
        }
    }
}
