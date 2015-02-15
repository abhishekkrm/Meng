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

namespace QS._qss_x_.Reflection_.Internal_
{
    [QS.Fx.Base.Inspectable]
    internal sealed class _internal_convertor_for_objects : QS.Fx.Inspection.Inspectable, _internal_convertor
    {
        #region Constructor

        public _internal_convertor_for_objects(Type _from, Type _to)
        {
            this._from = _from;
            this._from_c =  (QS._qss_x_.Reflection_.ObjectClass) QS._qss_x_.Reflection_.Library.ObjectClassOf(this._from);
            this._from_s = this._from_c.internal_info_;            
            this._from_g = this._from_s._backend_type;
            if (this._from.IsGenericType)
                this._from_g = this._from_g.MakeGenericType(this._from.GetGenericArguments());
            this._from_m = this._from_g.GetConstructor(System.Type.EmptyTypes);
            this._to = _to;
            this._to_c = (QS._qss_x_.Reflection_.ObjectClass) QS._qss_x_.Reflection_.Library.ObjectClassOf(this._to);
            this._to_s = this._to_c.internal_info_;
            this._to_g = this._to_s._frontend_type;
            if (this._to.IsGenericType)
                this._to_g = this._to_g.MakeGenericType(this._to.GetGenericArguments());
            this._to_m = this._to_g.GetConstructor(System.Type.EmptyTypes);
            this._mapping = new int[this._to_s._num_endpoints];
            this._endpointconvertors = new QS.Fx.Internal.I000009[this._to_s._num_endpoints];
            for (int _k = 0; _k < this._to_s._num_endpoints; _k++)
            {
                this._mapping[_k] = this._from_s._endpoint_lookup[this._to_s._endpoint_infos[_k]._endpoint_name];
                string _en = this._to_s._endpoint_infos[_k]._endpoint_name;
                Type _t1 = (((QS.Fx.Reflection.IObjectClass) this._from_c).Endpoints[_en]).EndpointClass.UnderlyingType;
                Type _t2 = (((QS.Fx.Reflection.IObjectClass)this._to_c).Endpoints[_en]).EndpointClass.UnderlyingType;
                this._endpointconvertors[_k] = QS._qss_x_.Reflection_.Internal_._internal._get_endpoint_convertor(_t1, _t2);
            }
        }

        #endregion

        #region Fields

        private Type _from, _to, _from_g, _to_g;
        private QS._qss_x_.Reflection_.ObjectClass _from_c, _to_c;
        private QS._qss_x_.Reflection_.Internal_._internal_info_objectclass _from_s, _to_s;
        private System.Reflection.ConstructorInfo _from_m, _to_m;
        private int[] _mapping;
        private QS.Fx.Internal.I000009[] _endpointconvertors;

        #endregion

        #region _convert

        object _internal_convertor._convert(object _o)
        {
            QS.Fx.Internal.I000007 _backend = (QS.Fx.Internal.I000007) _from_m.Invoke(new object[0]);
            _backend.x(_o);
            QS.Fx.Internal.I000006 _frontend = (QS.Fx.Internal.I000006) _to_m.Invoke(new object[0]);
            _frontend.x(_backend, this._mapping, this._endpointconvertors);
            return _frontend;
        }

        #endregion

        #region I000009 Members

        object QS.Fx.Internal.I000009.x(object y)
        {
            return ((_internal_convertor) this)._convert(y);
        }

        #endregion
    }
}
