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
    internal sealed class _internal_convertor_for_values : QS.Fx.Inspection.Inspectable, _internal_convertor, QS.Fx.Internal.I000009
    {
        #region Constructor (static)

        static _internal_convertor_for_values()
        {
            _builtins = new Dictionary<Type, Builtin_>();
            _builtins.Add(typeof(System.Boolean), new Builtin_(typeof(System.Boolean), "Boolean", Builtin_.Kind_._Boolean));
            _builtins.Add(typeof(System.Char), new Builtin_(typeof(System.Char), "Char", Builtin_.Kind_._Char));
            _builtins.Add(typeof(System.SByte), new Builtin_(typeof(System.SByte), "SByte", Builtin_.Kind_._SByte));
            _builtins.Add(typeof(System.Byte), new Builtin_(typeof(System.Byte), "Byte", Builtin_.Kind_._Byte));
            _builtins.Add(typeof(System.Int16), new Builtin_(typeof(System.Int16), "Int16", Builtin_.Kind_._Int16));
            _builtins.Add(typeof(System.Int32), new Builtin_(typeof(System.Int32), "Int32", Builtin_.Kind_._Int32));
            _builtins.Add(typeof(System.Int64), new Builtin_(typeof(System.Int64), "Int64", Builtin_.Kind_._Int64));
            _builtins.Add(typeof(System.UInt16), new Builtin_(typeof(System.UInt16), "UInt16", Builtin_.Kind_._UInt16));
            _builtins.Add(typeof(System.UInt32), new Builtin_(typeof(System.UInt32), "UInt32", Builtin_.Kind_._UInt32));
            _builtins.Add(typeof(System.UInt64), new Builtin_(typeof(System.UInt64), "UInt64", Builtin_.Kind_._UInt64));
            _builtins.Add(typeof(System.Single), new Builtin_(typeof(System.Single), "Single", Builtin_.Kind_._Single));
            _builtins.Add(typeof(System.Double), new Builtin_(typeof(System.Double), "Double", Builtin_.Kind_._Double));
            _builtins.Add(typeof(System.Decimal), new Builtin_(typeof(System.Decimal), "Decimal", Builtin_.Kind_._Decimal));
            _builtins.Add(typeof(System.DateTime), new Builtin_(typeof(System.DateTime), "DateTime", Builtin_.Kind_._DateTime));
            _builtins.Add(typeof(System.String), new Builtin_(typeof(System.String), "String", Builtin_.Kind_._String));
        }

        #endregion

        #region Constructor

        public _internal_convertor_for_values(Type _from, Type _to)
        {
            this._from = _from;
            this._to = _to;
            if (_to.IsAssignableFrom(_from))
                this._mode = Mode_._Cast;
            else
            {
                Builtin_ _from_builtin, _to_builtin;
                if (_builtins.TryGetValue(_from, out _from_builtin) && _builtins.TryGetValue(_to, out _to_builtin))
                {
                    this._method = typeof(System.Convert).GetMethod("To" + _to_builtin._name, 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { _from }, null);
                    if (this._method == null)
                        throw new Exception("Conversion from \"" + _from.ToString() + "\" to \"" + _to.ToString() + "\" has failed: convertor is missing in System.Convert");
                    this._mode = Mode_._StaticMethod;
                }
                else
                {
                    this._constructor = _to.GetConstructor(new Type[] { _from });
                    if (this._constructor != null)
                        this._mode = Mode_._Constructor;
                    else
                    {
                        this._method = _from.GetMethod("op_Explicit", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { _from }, null);
                        if (this._method != null && this._method.ReturnType.Equals(_to))
                            this._mode = Mode_._StaticMethod;
                        else
                        {
                            this._method = _from.GetMethod("op_Implicit", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { _from }, null);
                            if (this._method != null && this._method.ReturnType.Equals(_to))
                                this._mode = Mode_._StaticMethod;
                            else
                            {
                                this._method = _to.GetMethod("op_Explicit", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { _from }, null);
                                if (this._method != null && this._method.ReturnType.Equals(_to))
                                    this._mode = Mode_._StaticMethod;
                                else
                                {
                                    this._method = _to.GetMethod("op_Implicit", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { _from }, null);
                                    if (this._method != null && this._method.ReturnType.Equals(_to))
                                        this._mode = Mode_._StaticMethod;
                                    else
                                    {
                                        this._method = null;
                                        this._mode = Mode_._Impossible;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Mode_

        internal enum Mode_
        {
            _Cast, _Impossible, _StaticMethod, _Constructor
        }

        #endregion

        #region Builtin_

        internal class Builtin_
        {
            public Builtin_(Type _type, string _name, Kind_ _kind)
            {
                this._type = _type;
                this._name = _name;
                this._kind = _kind;
            }

            internal Type _type;
            internal string _name;
            internal Kind_ _kind;

            internal enum Kind_
            {
                _Boolean,
                _Char,
                _SByte,
                _Byte,
                _Int16,
                _Int32,
                _Int64,
                _UInt16,
                _UInt32,
                _UInt64,
                _Single,
                _Double,
                _Decimal,
                _DateTime,
                _String
            }
        }

        #endregion

        #region Fields

        private Mode_ _mode;
        private Type _from, _to;
        private System.Reflection.MethodInfo _method;
        private System.Reflection.ConstructorInfo _constructor;

        private static IDictionary<Type, Builtin_> _builtins;

        #endregion

        #region I000009 Members

        object QS.Fx.Internal.I000009.x(object y)
        {
            return ((_internal_convertor) this)._convert(y);
        }

        #endregion

        #region Accessors

        public Mode_ _Mode
        {
            get { return this._mode; }
        }

        #endregion

        #region _convert

        object _internal_convertor._convert(object _o)
        {
            switch (this._mode)
            {
                case Mode_._Cast:
                    return _o;

                case Mode_._StaticMethod:
                    return this._method.Invoke(null, new object[] { _o });

                case Mode_._Constructor:
                    return this._constructor.Invoke(new object[] { _o });

                case Mode_._Impossible:
                    throw new Exception("Cannot convert from value class \"" + _from.ToString() + "\" to value class \"" + _to.ToString() + "\".");

                default:
                    throw new NotSupportedException();
            }            
        }

        #endregion
    }
}
