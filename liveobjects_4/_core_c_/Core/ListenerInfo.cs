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

namespace QS._core_c_.Core
{
    public static class ListenerInfo
    {
        public static class Parameters
        {
            public const string HighPriority = "High Priority";
            public const string DrainSynchronously = "Drain Synchronously";
            public const string BufferSize = "Buffer Size";
            public const string NumberOfBuffers = "Number of Buffers";
            public const string AdfBufferSize = "Adf Buffer Size";
        }
        
        private static readonly QS.Fx.Base.IParametersInfo info = new QS._core_x_.Base.ParametersInfo(
            new QS.Fx.Base.IParameterInfo[] 
            {
                new QS._core_x_.Base.ParameterInfo(Parameters.HighPriority, typeof(bool)),
                new QS._core_x_.Base.ParameterInfo(Parameters.DrainSynchronously, typeof(bool)),
                new QS._core_x_.Base.ParameterInfo(Parameters.BufferSize, typeof(int), QS.Fx.Base.ParameterAccess.Readable),
                new QS._core_x_.Base.ParameterInfo(Parameters.NumberOfBuffers, typeof(int), QS.Fx.Base.ParameterAccess.Readable),
                new QS._core_x_.Base.ParameterInfo(Parameters.AdfBufferSize, typeof(int))
            });

        public static QS.Fx.Base.IParametersInfo ParametersInfo
        {
            get { return info; }
        }
    }
}
