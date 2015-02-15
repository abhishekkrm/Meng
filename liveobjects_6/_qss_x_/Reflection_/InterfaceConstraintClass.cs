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

namespace QS._qss_x_.Reflection_
{
    internal sealed class InterfaceConstraintClass : Class<QS.Fx.Reflection.IInterfaceConstraintClass, InterfaceConstraintClass>, QS.Fx.Reflection.IInterfaceConstraintClass
    {
        public InterfaceConstraintClass
        (
            Library.Namespace_ _namespace,
            QS.Fx.Base.ID _id,
            ulong _incarnation,
            string _name,
            string _comment,
            Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters,
            IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters
        )
        : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {

        }

        protected override InterfaceConstraintClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            throw new NotImplementedException();
        }
    }
}
