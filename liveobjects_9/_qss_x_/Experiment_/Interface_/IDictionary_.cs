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

namespace QS._qss_x_.Experiment_.Interface_
{
    [QS.Fx.Reflection.InterfaceClass("8D86100B4D6C496AA70D2220D3DE643D")]
    public interface IDictionary_ : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Add")]
        [QS.Fx.Serialization.Serializable]
        void _Add(string _key, string _value);

        [QS.Fx.Reflection.Operation("Add Multiple")]
        [QS.Fx.Serialization.Serializable]
        void _AddMultiple(string[] _key, string[] _value);

        [QS.Fx.Reflection.Operation("Add From File")]
        [QS.Fx.Serialization.Serializable]
        void _AddFromFile(string _path, int start, int length);

        [QS.Fx.Reflection.Operation("Set Ratio")]
        void _Set_Ratio(double _ratio);

        [QS.Fx.Reflection.Operation("Get")]
        void _Get(string _key);

        [QS.Fx.Reflection.Operation("Clear")]
        void _Clear();

        [QS.Fx.Reflection.Operation("Count")]
        void _Count();

        [QS.Fx.Reflection.Operation("Dump Stats")]
        void _DumpStats();
    }
}
