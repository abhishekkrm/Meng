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

namespace QS._qss_c_.Base6_
{
    public struct Asynchronous<ArgumentClass, ContextClass> : QS._core_c_.Base6.IAsynchronous<ArgumentClass, ContextClass>
    {
        public Asynchronous(ArgumentClass argument, ContextClass context, QS._core_c_.Base6.CompletionCallback<ContextClass> completionCallback)
        {
            this.argument = argument;
            this.context = context;
            this.completionCallback = completionCallback;
        }

        private ArgumentClass argument;
        private ContextClass context;
        private QS._core_c_.Base6.CompletionCallback<ContextClass> completionCallback;

        #region IAsynchronous<ArgumentClass,ContextClass> Members

        public ArgumentClass Argument
        {
            get { return argument; }
        }

        public ContextClass Context
        {
            get { return context; }
        }

        public QS._core_c_.Base6.CompletionCallback<ContextClass> CompletionCallback
        {
            get { return completionCallback; }
        }

        #endregion
    }

    public struct Asynchronous<ArgumentClass> : QS._core_c_.Base6.IAsynchronous<ArgumentClass>
    {
        public Asynchronous(ArgumentClass argument, object context, QS._core_c_.Base6.CompletionCallback<object> completionCallback)
        {
            this.argument = argument;
            this.context = context;
            this.completionCallback = completionCallback;
        }

        private ArgumentClass argument;
        private object context;
        private QS._core_c_.Base6.CompletionCallback<object> completionCallback;

        #region IAsynchronous<ArgumentClass> Members

        public ArgumentClass Argument
        {
            get { return argument; }
        }

        public object Context
        {
            get { return context; }
        }

        public QS._core_c_.Base6.CompletionCallback<object> CompletionCallback
        {
            get { return completionCallback; }
        }
        
        #endregion
    }
}
