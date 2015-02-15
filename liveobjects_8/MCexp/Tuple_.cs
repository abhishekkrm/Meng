/*
 
Copyright (c) 2008-2009 Chuck Sakoda. All rights reserved.

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

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("E243001CBEDB4447AF2651FAC03E903E")]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class Tuple_<[QS.Fx.Reflection.Parameter("T1", QS.Fx.Reflection.ParameterClass.ValueClass)] T1, [QS.Fx.Reflection.Parameter("T2", QS.Fx.Reflection.ParameterClass.ValueClass)] T2>
    {
        
        public Tuple_(T1 x, T2 y, int id)
        {
            this.id = id;
            this.xv = x;
            this.yv = y;
        }

        public Tuple_()
        {

        }
        public int id;
        private T1 xv;
        private T2 yv;

        #region ITuple_<T1,T2> Members

        public T1 x
        {
            get { return xv; }
            set { xv = value; }
        }

        public T2 y
        {
            get { return yv; }
            set { yv = value; }
        }

        #endregion
    }
}