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
    internal sealed class _internal_convertor_for_interfaces : QS.Fx.Inspection.Inspectable, _internal_convertor, QS.Fx.Internal.I000009
    {
        #region Constructor

        public _internal_convertor_for_interfaces(Type _from, Type _to)
        {
            this._from = _from;
            this._to = _to;
            if (_to.IsAssignableFrom(_from))
                this._mode = Mode_._Cast;
            else
            {
                this._mode = Mode_._Forward;
                this._from_c = (QS._qss_x_.Reflection_.InterfaceClass)QS._qss_x_.Reflection_.Library.InterfaceClassOf(this._from);
                this._to_c = (QS._qss_x_.Reflection_.InterfaceClass) QS._qss_x_.Reflection_.Library.InterfaceClassOf(this._to);
                this._from_s = this._from_c.internal_info_;
                this._to_s = this._to_c.internal_info_;
                this._from_g = this._from_s._backend_type;
                if (this._from.IsGenericType)
                    this._from_g = this._from_g.MakeGenericType(this._from.GetGenericArguments());
                this._to_g = this._to_s._frontend_type;
                if (this._to.IsGenericType)
                    this._to_g = this._to_g.MakeGenericType(this._to.GetGenericArguments());
                this._from_m = this._from_g.GetConstructor(System.Type.EmptyTypes);
                this._to_m = this._to_g.GetConstructor(System.Type.EmptyTypes);
                this._mapping = new QS.Fx.Internal.I000001[this._to_s._num_operations];
                // this._endpointconvertors = new QS.Fx.Internal.I000009[this._to_s._num_endpoints];
                for (int _k = 0; _k < this._to_s._num_operations; _k++)
                {
                    _internal_info_operation _to_operation = this._to_s._operation_infos[_k];
                    int _operation_index = this._from_s._operation_lookup[_to_operation._operation_name];
                    _internal_info_operation _from_operation = this._from_s._operation_infos[_operation_index];
                    int _parameter_count = _from_operation._num_in_parameters + _to_operation._num_out_parameters;
                    int[] _in_map = (_to_operation._num_in_parameters > 0) ? new int[_to_operation._num_in_parameters] : null;
                    QS.Fx.Internal.I000009[] _in_convertors =
                        (_to_operation._num_in_parameters > 0) ? new QS.Fx.Internal.I000009[_to_operation._num_in_parameters] : null;
                    for (int _z = 0; _z < _to_operation._num_in_parameters; _z++)
                    {
                        _internal_info_parameter _to_parameter = _to_operation._in_parameter_infos[_z];
                        int _from_parameter_index;
                        if ((_from_operation._in_parameter_lookup == null) || !_from_operation._in_parameter_lookup.TryGetValue(_to_parameter._name, out _from_parameter_index))
                            _from_parameter_index = -1;
                        _in_map[_z] = _from_parameter_index;
                        if (_from_parameter_index >= 0)
                        {
                            _internal_info_parameter _from_parameter = _from_operation._in_parameter_infos[_from_parameter_index];
                            _in_convertors[_z] = _internal._get_value_convertor(_to_parameter._type, _from_parameter._type);
                        }
                    }
                    int[] _out_map = (_from_operation._num_out_parameters > 0) ? new int[_from_operation._num_out_parameters] : null;
                    QS.Fx.Internal.I000009[] _out_convertors = 
                        (_from_operation._num_out_parameters > 0) ? new QS.Fx.Internal.I000009[_from_operation._num_out_parameters] : null;
                    for (int _z = 0; _z < _from_operation._num_out_parameters; _z++)
                    {
                        _internal_info_parameter _from_parameter = _from_operation._out_parameter_infos[_z];
                        int _to_parameter_index;
                        if (_from_parameter._retval)
                            _to_parameter_index = _to_operation._retval_lookup;
                        else
                        {
                            if ((_to_operation._out_parameter_lookup == null) || !_to_operation._out_parameter_lookup.TryGetValue(_from_parameter._name, out _to_parameter_index))
                                _to_parameter_index = -1;
                        }
                        _out_map[_z] = (_to_parameter_index >= 0) ? (_parameter_count - 1 - _to_parameter_index) : _to_parameter_index;
                        if (_to_parameter_index >= 0)
                        {
                            _internal_info_parameter _to_parameter = _to_operation._out_parameter_infos[_to_parameter_index];
                            _out_convertors[_z] = _internal._get_value_convertor(_from_parameter._type, _to_parameter._type);
                        }
                    }
                    this._mapping[_k] = new QS.Fx.Internal.I000001(_operation_index, _parameter_count, _in_map, _out_map, _in_convertors, _out_convertors);
                }
            }
        }

        #endregion

        #region Mode_

        private enum Mode_
        {
            _Cast, _Impossible, _Forward
        }

        #endregion

        #region Fields

        private Mode_ _mode;
        private Type _from, _to, _from_g, _to_g;
        private QS._qss_x_.Reflection_.InterfaceClass _from_c, _to_c;
        private QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass _from_s, _to_s;
        private System.Reflection.ConstructorInfo _from_m, _to_m;
        private QS.Fx.Internal.I000001[] _mapping;

/*
        private QS.Fx.Internal.I000009[] _endpointconvertors;
*/

        #endregion

        #region I000009 Members

        object QS.Fx.Internal.I000009.x(object y)
        {
            return ((_internal_convertor) this)._convert(y);
        }

        #endregion

        #region _convert

        object _internal_convertor._convert(object _o)
        {
            switch (this._mode)
            {
                case Mode_._Forward:
                    {
                        QS.Fx.Internal.I000003 _backend = (QS.Fx.Internal.I000003) _from_m.Invoke(new object[0]);
                        _backend.x(_o);
                        QS.Fx.Internal.I000002 _frontend = (QS.Fx.Internal.I000002) _to_m.Invoke(new object[0]);
                        _frontend.x(_backend, this._mapping);
                        return _frontend;
                    }
                    break;

                case Mode_._Cast:
                    return _o;

                case Mode_._Impossible:
                    throw new NotSupportedException();

                default:
                    throw new NotSupportedException();
            }            
        }

        #endregion
    }
}
