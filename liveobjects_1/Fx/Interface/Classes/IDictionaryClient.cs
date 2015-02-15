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
    [QS.Fx.Reflection.InterfaceClass(QS.Fx.Reflection.InterfaceClasses.DictionaryClient)]
    public interface IDictionaryClient<
        [QS.Fx.Reflection.Parameter("KeyClass", QS.Fx.Reflection.ParameterClass.ValueClass)] KeyClass,
        [QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass>
        : IInterface
        where KeyClass : class
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Operation("Ready")]
        void Ready();

        [QS.Fx.Reflection.Operation("Added")]
        void Added(KeyClass _key, QS.Fx.Object.IReference<ObjectClass> _object);

        [QS.Fx.Reflection.Operation("Removed")]
        void Removed(KeyClass _key);
    }
}
