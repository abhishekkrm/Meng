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

namespace QS.Fx.Interface.Classes
{
    [QS.Fx.Reflection.InterfaceClass(QS.Fx.Reflection.InterfaceClasses.Library)]
    public interface ILibrary : IInterface
    {
        [QS.Fx.Reflection.Operation("GetValueClass")]
        QS.Fx.Reflection.IValueClass GetValueClass(string id);

        [QS.Fx.Reflection.Operation("GetInterfaceClass")]
        QS.Fx.Reflection.IInterfaceClass GetInterfaceClass(string id);

        [QS.Fx.Reflection.Operation("GetEndpointClass")]
        QS.Fx.Reflection.IEndpointClass GetEndpointClass(string id);

        [QS.Fx.Reflection.Operation("GetObjectClass")]
        QS.Fx.Reflection.IObjectClass GetObjectClass(string id);

        [QS.Fx.Reflection.Operation("GetComponentClass")]
        QS.Fx.Reflection.IComponentClass GetComponentClass(string id);

        [QS.Fx.Reflection.Operation("GetInterfaceConstraintClass")]
        QS.Fx.Reflection.IInterfaceConstraintClass GetInterfaceConstraintClass(string id);

        [QS.Fx.Reflection.Operation("GetEndpointConstraintClass")]
        QS.Fx.Reflection.IEndpointConstraintClass GetEndpointConstraintClass(string id);

        [QS.Fx.Reflection.Operation("GetObjectConstraintClass")]
        QS.Fx.Reflection.IObjectConstraintClass GetObjectConstraintClass(string id);
    }
}
