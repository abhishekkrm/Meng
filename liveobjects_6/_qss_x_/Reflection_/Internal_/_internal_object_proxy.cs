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
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace QS._qss_x_.Reflection_.Internal_
{
    public sealed class _internal_object_proxy : RealProxy
    {
        #region Constructor

        public _internal_object_proxy(object _o, Type _from) : base(_from)
        {
            this._o = _o;
            this._type = _o.GetType();
            this._from = _from;
            this._to = _o.GetType();
            if (!typeof(QS.Fx.Object.Classes.IObject).IsAssignableFrom(_to))
                throw new Exception("Cannot create a proxy to something that isn't of type \"" +
                    typeof(QS.Fx.Object.Classes.IObject).FullName + "\".");
            if (!_to.IsAssignableFrom(_type))
                throw new Exception("Type \"" + _type.FullName + "\" of the object is not derived from \"" + _to.FullName + "\".");
            TypePair_ _typepair = new TypePair_(this._from, this._to);
            lock (_maps)
            {
                if (!_maps.TryGetValue(_typepair, out this._map))
                {
                    this._map = new _internal_object_proxy_map(this._from, this._to);
                    _maps.Add(_typepair, this._map);
                }
            }
        }

        #endregion

        #region Fields

        private object _o;
        private Type _from, _to, _type;
        private _internal_object_proxy_map _map;
        private static IDictionary<TypePair_, _internal_object_proxy_map> _maps = 
            new Dictionary<TypePair_, _internal_object_proxy_map>();

        #endregion

        #region Invoke

        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage request = (IMethodCallMessage) msg;
            object endpoint = this._map._invoke(this._o, request.MethodName);
            IMethodReturnMessage response = new ReturnMessage(endpoint, null, 0, null, request);
            return response;
        }

        #endregion
    }
}
