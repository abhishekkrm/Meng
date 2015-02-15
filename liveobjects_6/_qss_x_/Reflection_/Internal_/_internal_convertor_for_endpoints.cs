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
    internal sealed class _internal_convertor_for_endpoints : QS.Fx.Inspection.Inspectable, _internal_convertor, QS.Fx.Internal.I000009
    {
        #region Constructor (static)

        static _internal_convertor_for_endpoints()
        {
            _generic_exportedinterface = typeof(QS.Fx.Endpoint.Classes.IExportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
            _generic_importedinterface = typeof(QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
            _generic_dualinterface = typeof(QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IInterface, QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
            _internal_exportedinterface = typeof(QS._qss_x_.Endpoint_.Internal_.ExportedInterface_<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
            _internal_importedinterface = typeof(QS._qss_x_.Endpoint_.Internal_.ImportedInterface_<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
            _internal_dualinterface = typeof(QS._qss_x_.Endpoint_.Internal_.DualInterface_<QS.Fx.Interface.Classes.IInterface, QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        }

        #endregion

        #region Constructor

        public _internal_convertor_for_endpoints(Type _from, Type _to)
        {
            this._from = _from;
            this._to = _to;
            if (_to.IsAssignableFrom(_from))
                this._mode = Mode_._Cast;
            else
            {
                if (this._from.IsGenericType && this._to.IsGenericType)
                {
                    Type _generic = this._from.GetGenericTypeDefinition();
                    if (_generic.Equals(this._to.GetGenericTypeDefinition()))
                    {
                        if (_generic.Equals(_generic_exportedinterface))
                        {
                            this._mode = Mode_._Exported;
                            this._from_exported = this._from.GetGenericArguments()[0];
                            this._to_exported = this._to.GetGenericArguments()[0];
                            this._internal = _internal_exportedinterface.MakeGenericType(this._to_exported);
                            this._constructor = this._internal.GetConstructor(
                                new Type[] { typeof(QS._qss_x_.Endpoint_.Internal_.Endpoint_), 
                                    typeof(QS._qss_x_.Reflection_.Internal_._internal_convertor) });
                            this._exported_convertor = QS._qss_x_.Reflection_.Internal_._internal._get_interface_convertor(this._from_exported, this._to_exported);
                        }
                        else if (_generic.Equals(_generic_importedinterface))
                        {
                            this._mode = Mode_._Imported;
                            this._from_imported = this._from.GetGenericArguments()[0];
                            this._to_imported = this._to.GetGenericArguments()[0];
                            this._internal = _internal_importedinterface.MakeGenericType(this._to_imported);
                            this._constructor = this._internal.GetConstructor(
                                new Type[] { typeof(QS._qss_x_.Endpoint_.Internal_.Endpoint_), 
                                    typeof(QS._qss_x_.Reflection_.Internal_._internal_convertor) });
                            this._imported_convertor = QS._qss_x_.Reflection_.Internal_._internal._get_interface_convertor(this._to_imported, this._from_imported);
                        }
                        else if (_generic.Equals(_generic_dualinterface))
                        {
                            this._mode = Mode_._Dual;
                            this._from_imported = this._from.GetGenericArguments()[0];
                            this._from_exported = this._from.GetGenericArguments()[1];
                            this._to_imported = this._to.GetGenericArguments()[0];
                            this._to_exported = this._to.GetGenericArguments()[1];
                            this._internal = _internal_dualinterface.MakeGenericType(this._to_imported, this._to_exported);
                            this._constructor = this._internal.GetConstructor(
                                new Type[] { typeof(QS._qss_x_.Endpoint_.Internal_.Endpoint_), 
                                    typeof(QS._qss_x_.Reflection_.Internal_._internal_convertor),
                                    typeof(QS._qss_x_.Reflection_.Internal_._internal_convertor)});
                            this._exported_convertor = QS._qss_x_.Reflection_.Internal_._internal._get_interface_convertor(this._from_exported, this._to_exported);
                            this._imported_convertor = QS._qss_x_.Reflection_.Internal_._internal._get_interface_convertor(this._to_imported, this._from_imported);
                        }
                        else
                            this._mode = Mode_._Impossible;
                    }
                    else
                        this._mode = Mode_._Impossible;
                }
                else
                    this._mode = Mode_._Impossible;
                if (this._mode == Mode_._Impossible)
                    throw new Exception("Cannot convert from endpoint type \"" + _from.ToString() + "\" to endpoint type \"" + _to.ToString() + "\".");
            }
        }

        #endregion

        #region Mode_

        private enum Mode_
        {
            _Cast, _Impossible, _Exported, _Imported, _Dual
        }

        #endregion

        #region Fields

        private Mode_ _mode;
        private Type _from, _to, _from_exported, _from_imported, _to_exported, _to_imported, _internal;
        private System.Reflection.ConstructorInfo _constructor;
        private _internal_convertor _imported_convertor, _exported_convertor;

        private static readonly Type _generic_exportedinterface, _generic_importedinterface, _generic_dualinterface,
            _internal_exportedinterface, _internal_importedinterface, _internal_dualinterface;

        #endregion

        #region I000009 Members

        object QS.Fx.Internal.I000009.x(object y)
        {
            return ((_internal_convertor)this)._convert(y);
        }

        #endregion

        #region _convert

        object _internal_convertor._convert(object _o)
        {
            switch (this._mode)
            {                
                case Mode_._Exported:
                    {
                        if (_o is QS._qss_x_.Endpoint_.Internal_.Endpoint_)
                            return this._constructor.Invoke(new object[] { _o, this._exported_convertor });
                        else
                            throw new NotSupportedException();
                    }
                    break;
                
                case Mode_._Imported:
                    {
                        if (_o is QS._qss_x_.Endpoint_.Internal_.Endpoint_)
                            return this._constructor.Invoke(new object[] { _o, this._imported_convertor });
                        else
                            throw new NotSupportedException();
                    }
                    break;

                case Mode_._Dual:
                    {
                        if (_o is QS._qss_x_.Endpoint_.Internal_.Endpoint_)
                            return this._constructor.Invoke(new object[] { _o, this._imported_convertor, this._exported_convertor });
                        else
                            throw new NotSupportedException();
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
