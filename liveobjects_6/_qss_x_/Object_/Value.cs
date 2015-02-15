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

namespace QS._qss_x_.Object_
{
    public class Value<ValueClass> : IDisposable, QS.Fx.Interface.Classes.IValueClient<ValueClass>
        where ValueClass : class
    {
        #region Constructor

        public Value(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<ValueClass>> _objectref)
        {
            lock (this)
            {
                this._endpoint = _mycontext.DualInterface<QS.Fx.Interface.Classes.IValue<ValueClass>, QS.Fx.Interface.Classes.IValueClient<ValueClass>>(this);
                this._connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._endpoint).Connect(_objectref.Dereference(_mycontext).Endpoint);
                this._value = this._endpoint.Interface.Get();
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValue<ValueClass>, QS.Fx.Interface.Classes.IValueClient<ValueClass>> _endpoint;
        private ValueClass _value;
        private QS.Fx.Endpoint.IConnection _connection;

        #endregion

        #region Accessors

        public ValueClass Get()
        {
            return _value;
        }

        #endregion

        #region IValueClient<byte[]> Members

        void QS.Fx.Interface.Classes.IValueClient<ValueClass>.Set(ValueClass _value)
        {
            lock (this)
            {
                this._value = _value;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                this._connection.Dispose();
            }
        }

        #endregion
    }
}
