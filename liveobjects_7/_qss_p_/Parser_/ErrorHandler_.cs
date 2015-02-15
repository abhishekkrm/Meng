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
using QS._qss_p_.ParserGenerator_;
using Babel.ParserGenerator;

namespace QS._qss_p_.Parser_
{
    public class ErrorHandler : IErrorHandler
    {
        const int errLev = 2; 

        List<Error> errors;
        int errNum = 0;
        int wrnNum = 0; 

        public bool Errors { get { return errNum > 0; } }
        public bool Warnings { get { return wrnNum > 0; } }
        public int ErrNum { get { return errNum; } }
        public int WrnNum { get { return wrnNum; } }

        public ErrorHandler()
        {
            errors = new List<Error>(8);
        }

        public List<Error> SortedErrorList()
        {
            if (errors.Count > 0)
            {
                errors.Sort();
                return errors;
            }
            else
            {
                return null;
            }
        }

        public void AddError(string msg, LexLocation span, int severity)
        {
            AddError(msg, span.sLin, span.sCol, span.eCol - span.sCol + 1, severity);
        }

        public void AddError(string msg, int lin, int col, int len, int severity)
        {
            bool warnOnly = severity < errLev;
            errors.Add(new Error(msg, lin, col, len, warnOnly));
            if (warnOnly) wrnNum++; else errNum++;
        }

        public void AddError(string msg, int lin, int col, int len)
        {
            errors.Add(new Error(msg, lin, col, len, false)); errNum++;
        }

        public void AddWarning(string msg, int lin, int col, int len)
        {
            errors.Add(new Error(msg, lin, col, len, true)); wrnNum++;
        }
    }
}
