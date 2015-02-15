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

namespace QS.Fx.Interface.Classes
{

    [QS.Fx.Reflection.InterfaceClass("CDB208849B6F4bfc8ECEB59915FC7865")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_N : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Next")]
        void Next(QS.Fx.Value.Tuple_<int, byte[]> block);

        

    }

}
namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass("A0C64E7190B345be98935A249ED7A3C3")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IMCexp9_N : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Writer or Next Encrypter")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMCexp9_A, QS.Fx.Interface.Classes.IMCexp9_N> WriterOrNextEnc
        {
            get;
        }
    }
}
