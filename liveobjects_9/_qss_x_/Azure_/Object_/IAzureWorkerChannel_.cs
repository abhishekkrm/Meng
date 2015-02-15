/*

Copyright (c) 2009 Chuck Sakoda. All rights reserved.

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
using System.Linq;
using System.Text;

        

namespace QS._qss_x_.Azure_.Object_
{
    [QS.Fx.Reflection.ObjectClass("9527E9774AD24572B7C8B9DB081E0C9B")]
    public interface IAzureWorkerChannel_<
        [QS.Fx.Reflection.Parameter("DataClass", QS.Fx.Reflection.ParameterClass.ValueClass)] DataClass> : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Channel")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>, QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_> _Channel
        {
            get;
        }
    }
}
