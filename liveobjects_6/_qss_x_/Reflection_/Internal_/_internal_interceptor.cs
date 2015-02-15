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
    internal sealed class _internal_interceptor
    {
        public _internal_interceptor(Type _from)
        {
            this._from = _from;
            this._from_c = (QS._qss_x_.Reflection_.InterfaceClass) QS._qss_x_.Reflection_.Library.InterfaceClassOf(this._from);
            if (this._from_c == null)
                throw new Exception("Could not locate interface class metadata for \"" + this._from.ToString() + "\".");
            this._from_b = (QS._qss_x_.Reflection_.InterfaceClass) this._from_c._Original_Base_Template_InterfaceClass;
            if (this._from_b == null)
                throw new Exception("Could not locate base interface metadata for \"" + this._from.ToString() + "\".");
            this._from_s = this._from_b.internal_info_;
            if (this._from_s == null)
                throw new Exception("Could not locate interface class internals for \"" + this._from.ToString() + "\".");
            this._from_g = this._from_s._interceptor_type;
            if (this._from_g == null)
                throw new Exception("Could not locate interface class interceptor for \"" + this._from.ToString() + "\".");
            if (this._from.IsGenericType)
            {
                this._from_g = this._from_g.MakeGenericType(this._from.GetGenericArguments());
                if (this._from_g == null)
                    throw new Exception("Could not specialize a generic interceptor for \"" + this._from.ToString() + "\".");
            }
            this._from_m = this._from_g.GetConstructor(System.Type.EmptyTypes);
            if (this._from_m == null)
                throw new Exception("Could not locate an interceptor's constructor for \"" + this._from.ToString() + "\".");
        }

        private Type _from, _from_g;
        private QS._qss_x_.Reflection_.InterfaceClass _from_b, _from_c;
        private QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass _from_s;
        private System.Reflection.ConstructorInfo _from_m;
        private System.Reflection.MethodInfo[] _method_infos;
        private IDictionary<Type, QS.Fx.Base.SynchronizationOption[]> _options = new Dictionary<Type, QS.Fx.Base.SynchronizationOption[]>();

        public object _convert(object _o, QS._qss_x_.Object_.IInternal_ _context)
        {
            QS.Fx.Internal.I000008 _interceptor = (QS.Fx.Internal.I000008) _from_m.Invoke(new object[0]);
            QS.Fx.Base.SynchronizationOption[] _s;
            lock (_options)
            {
                Type _to = _o.GetType();
                if (!_options.TryGetValue(_to, out _s))
                {
                    _s = _readoptions(_to, _context);
                    _options.Add(_to, _s);
                }
            }
            _interceptor.x(_o, _context, _s); 
            return _interceptor;
        }

        private QS.Fx.Base.SynchronizationOption[] _readoptions(Type _to, QS._qss_x_.Object_.IInternal_ _context)
        {
            if (this._method_infos == null)
                this._method_infos = _from.GetMethods();
            QS.Fx.Base.SynchronizationOption[] _s = new QS.Fx.Base.SynchronizationOption[this._method_infos.Length];
            System.Reflection.InterfaceMapping _interface_mapping = _to.GetInterfaceMap(_from);            
            for (int _i = 0; _i < _s.Length; _i++)
            {
                System.Reflection.MethodInfo _m1 = this._method_infos[_i];
                System.Reflection.MethodInfo _m2 = _interface_mapping.InterfaceMethods[_i];
                System.Reflection.MethodInfo _m3 = _interface_mapping.TargetMethods[_i];
                if (_m1.Equals(_m2))
                {
                    QS.Fx.Base.SynchronizationOption _option = QS.Fx.Base.SynchronizationOption.None;
                    foreach (QS.Fx.Base.SynchronizationAttribute _attribute in _m3.GetCustomAttributes(typeof(QS.Fx.Base.SynchronizationAttribute), true))
                        _option = _option | _attribute.Option;
                    _s[_i] = QS.Fx.Base.Synchronization.CombineOptions(_context.SynchronizationOption, _option);
                }
                else
                    throw new Exception("Mismatch in the interface mapping.");
            }
            return _s;
        }
    }
}
