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

#define DO_NOT_INTERCEPT_TWO_WAY_CALLS

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Proxies;

namespace QS._qss_x_.Reflection_.Internal_
{
    public static class _internal
    {
        #region _inject_interceptor

        public static C _inject_interceptor<C>(C _handler, QS._qss_x_.Object_.IInternal_ _context) where C : class, QS.Fx.Interface.Classes.IInterface
        {
            if (_handler != null)
            {
                _internal_interceptor _interceptor = QS._qss_x_.Reflection_.Internal_._internal._get_interceptor(typeof(C));
                object _o = _interceptor._convert(_handler, _context);
                return (C) _o;
            }
            else
                return null;
        }

        #endregion

        #region _cast_object

        public static C _cast_object<C>(QS.Fx.Object.Classes.IObject _o, bool _force) where C : class, QS.Fx.Object.Classes.IObject
        {
            return (C) QS._qss_x_.Reflection_.Internal_._internal._cast_object(_o, typeof(C), _force);
/*
            if (o != null)
            {
                if (o is C)
                    return (C) o;
                else
                {
                    _internal_object_proxy _proxy = new _internal_object_proxy(o, typeof(C));
                    C c = (C) _proxy.GetTransparentProxy();
                    return c;
                }
            }
            else
                return null;
*/
        }

        public static QS.Fx.Object.Classes.IObject _cast_object(QS.Fx.Object.Classes.IObject _o, Type _c, bool _force)
        {
            if (_o != null)
            {
                Type _b = _o.GetType();
                if (!_force && _c.IsAssignableFrom(_b))
                    return _o;
                else
                {
                    _internal_convertor _convertor = QS._qss_x_.Reflection_.Internal_._internal._get_object_convertor(_b, _c);
                    object _q = _convertor._convert(_o);
                    return (QS.Fx.Object.Classes.IObject) _q;
                }

/*
                if (c.IsAssignableFrom(o.GetType()))
                    return o;
                else
                {
                    _internal_object_proxy _proxy = new _internal_object_proxy(o, c);
                    return (QS.Fx.Object.Classes.IObject)_proxy.GetTransparentProxy();
                }
*/
            }
            else
                return null;
        }

        public static QS.Fx.Object.Classes.IObject _cast_object(QS.Fx.Object.Classes.IObject _o, Type _b, Type _c, bool _force)
        {
            if (_o != null)
            {
                if (!_force && _c.IsAssignableFrom(_b))
                    return _o;
                else
                {
                    _internal_convertor _convertor = QS._qss_x_.Reflection_.Internal_._internal._get_object_convertor(_b, _c);
                    object _q = _convertor._convert(_o);
                    return (QS.Fx.Object.Classes.IObject)_q;
                }
            }
            else
                return null;
        }

        #endregion

        #region _type_name_of

        private static string _type_name_of(System.Type t)
        {
            if (t.Equals(typeof(void)))
                return "void";
            else
            {
                StringBuilder sb = new StringBuilder();
                if (!t.IsGenericParameter)
                {
                    if (t.IsNested)
                        sb.Append(t.DeclaringType);
                    else
                        sb.Append(t.Namespace);
                    sb.Append(".");
                }
                int k = t.Name.IndexOf('`');
                sb.Append((k >= 0) ? t.Name.Substring(0, k) : t.Name);
                if (t.IsGenericType)
                {
                    sb.Append("<");
                    bool _is_first = true;
                    foreach (Type tt in t.GetGenericArguments())
                    {
                        if (_is_first)
                            _is_first = false;
                        else
                            sb.Append(", ");
                        sb.Append(_type_name_of(tt));
                    }
                    sb.Append(">");
                }
                return sb.ToString();
            }
        }

        #endregion

        #region _template_signatures_of

        private static void _generate_template_signatures_of(Type _interface_type, out string _generic_signature, out string _generic_constraint_block)
        {
            _generic_signature = string.Empty;
            _generic_constraint_block = string.Empty;
            if (_interface_type.IsGenericType)
            {
                if (!_interface_type.IsGenericTypeDefinition)
                    throw new Exception("Cannot produce a template signature of a generic interface type that is not a generic interface template.");
                StringBuilder _generic_signature_sb = new StringBuilder();
                StringBuilder _where_sb_0 = new StringBuilder();
                _generic_signature_sb.Append("<");
                bool _isfirstgenericargument = true;
                foreach (Type _genericargumenttype in _interface_type.GetGenericArguments())
                {
                    if (_isfirstgenericargument)
                        _isfirstgenericargument = false;
                    else
                        _generic_signature_sb.Append(", ");
                    _generic_signature_sb.Append(_genericargumenttype.Name);
                    System.Reflection.GenericParameterAttributes _genericparameterattributes =
                        _genericargumenttype.GenericParameterAttributes & System.Reflection.GenericParameterAttributes.SpecialConstraintMask;
                    Type[] _genericargumentconstraints = _genericargumenttype.GetGenericParameterConstraints();
                    if (_genericparameterattributes != System.Reflection.GenericParameterAttributes.None ||
                        (_genericargumentconstraints != null && _genericargumentconstraints.Length > 0))
                    {
                        _where_sb_0.Append("    where ");
                        _where_sb_0.Append(_genericargumenttype.Name);
                        _where_sb_0.Append(" : ");
                        bool _isfirstgenericargumentconstraint = true;
                        if ((_genericparameterattributes & System.Reflection.GenericParameterAttributes.ReferenceTypeConstraint) !=
                            System.Reflection.GenericParameterAttributes.None)
                        {
                            if (_isfirstgenericargumentconstraint)
                                _isfirstgenericargumentconstraint = false;
                            else
                                _where_sb_0.Append(", ");
                            _where_sb_0.Append("class");
                        }
                        //if ((_genericparameterattributes & System.Reflection.GenericParameterAttributes.DefaultConstructorConstraint) !=
                        //    System.Reflection.GenericParameterAttributes.None)
                        //{
                        //    if (_isfirstgenericargumentconstraint)
                        //        _isfirstgenericargumentconstraint = false;
                        //    else
                        //        _where_sb_0.Append(", ");
                        //    _where_sb_0.Append("new()");
                        //}
                        if ((_genericargumentconstraints != null && _genericargumentconstraints.Length > 0))
                        {
                            foreach (Type _genericargumentconstraint in _genericargumentconstraints)
                            {
                                if (_isfirstgenericargumentconstraint)
                                    _isfirstgenericargumentconstraint = false;
                                else
                                    _where_sb_0.Append(", ");
                                _where_sb_0.Append(_type_name_of(_genericargumentconstraint));
                            }
                        }
                        if ((_genericparameterattributes & System.Reflection.GenericParameterAttributes.DefaultConstructorConstraint) !=
                                    System.Reflection.GenericParameterAttributes.None)
                        {
                            if (_isfirstgenericargumentconstraint)
                                _isfirstgenericargumentconstraint = false;
                            else
                                _where_sb_0.Append(", ");
                            _where_sb_0.Append("new()");
                        }
                        _where_sb_0.Append("\n");
                    }
                }
                _generic_signature_sb.Append(">");
                _generic_signature = _generic_signature_sb.ToString();
                _generic_constraint_block = _where_sb_0.ToString();
            }
        }

        #endregion

        #region (class, method, and field naming)

        private static readonly string _name_of_internal_convertor_dispatch_method = "x";
        private static readonly string _name_of_internal_endpoint_convertor = "QS.Fx.Internal.I000009";
        private static readonly string _name_of_internal_argument_mapping = "QS.Fx.Internal.I000001";
        private static readonly string _name_of_internal_interface_frontend = "QS.Fx.Internal.I000002";
        private static readonly string _name_of_internal_interface_backend = "QS.Fx.Internal.I000003";
        private static readonly string _name_of_internal_interface_interceptor = "QS.Fx.Internal.I000008";
        private static readonly string _name_of_internal_interface_frontend_initialize_method = "x";
        private static readonly string _name_of_internal_interface_backend_initialize_method = "x";
        private static readonly string _name_of_internal_interface_interceptor_initialize_method = "x";
        private static readonly string _name_of_internal_interface_backend_dispatch_method = "y";
        private static readonly string _name_of_internal_interface_interceptor_arguments_event = "e";
        private static readonly string _name_of_internal_argument_mapping_method_index = "a";
        private static readonly string _name_of_internal_argument_mapping_number_of_values = "b";
        private static readonly string _name_of_internal_argument_mapping_incoming_values = "c";
        private static readonly string _name_of_internal_argument_mapping_outgoing_values = "d";
        private static readonly string _name_of_internal_argument_mapping_incoming_convertors = "e";
        private static readonly string _name_of_internal_argument_mapping_outgoing_convertors = "f";
        private static readonly string _name_of_internal_object_frontend = "QS.Fx.Internal.I000006";
        private static readonly string _name_of_internal_object_backend = "QS.Fx.Internal.I000007";
        private static readonly string _name_of_internal_object_frontend_initialize_method = "x";
        private static readonly string _name_of_internal_object_backend_initialize_method = "x";
        private static readonly string _name_of_internal_object_backend_dispatch_method = "y";

        #endregion

        #region _generate_interface_frontend_and_backend

        public static void _generate_interface_frontend_and_backend(
            Type _interface_type, QS.Fx.Base.ID _owner_ns_id, ulong _owner_ns_in, QS.Fx.Base.ID _owner_cc_id, ulong _owner_cc_in, 
            string _namespace_name, string _interface_name, 
            out QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass _o_internal_info_interfaceclass)
        {
            List<QS._qss_x_.Reflection_.Internal_._internal_info_operation> _operation_infos = 
                new List<QS._qss_x_.Reflection_.Internal_._internal_info_operation>();
            StringBuilder _frontend_code = new StringBuilder();
            StringBuilder _backend_code = new StringBuilder();
            StringBuilder _interceptor_code = new StringBuilder();
            string _generic_signature, _generic_constraint_block;
            _generate_template_signatures_of(_interface_type, out _generic_signature, out _generic_constraint_block);
            _frontend_code.Append("namespace ");
            _frontend_code.Append(_namespace_name);
            _frontend_code.Append("\n{\n\n");
            _frontend_code.Append("[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.i1, \"");
            _frontend_code.Append(_owner_ns_id.ToString());
            _frontend_code.Append("\", ");
            _frontend_code.Append(_owner_ns_in);
            _frontend_code.Append(", \"");
            _frontend_code.Append(_owner_cc_id.ToString());
            _frontend_code.Append("\", ");
            _frontend_code.Append(_owner_cc_in);
            _frontend_code.Append(")]\n");
            _frontend_code.Append("public sealed class _i1_");
            _frontend_code.Append(_interface_name);
            _frontend_code.Append(_generic_signature);
            _frontend_code.Append("\n    : ");
            _frontend_code.Append(_type_name_of(_interface_type));
            _frontend_code.Append(", ");
            _frontend_code.Append(_name_of_internal_interface_frontend);
            _frontend_code.Append("\n");
            _frontend_code.Append(_generic_constraint_block);
            _frontend_code.Append("{\n");
            _frontend_code.Append("    void ");
            _frontend_code.Append(_name_of_internal_interface_frontend);
            _frontend_code.Append(".");
            _frontend_code.Append(_name_of_internal_interface_frontend_initialize_method);
            _frontend_code.Append("(");
            _frontend_code.Append(_name_of_internal_interface_backend);
            _frontend_code.Append(" _b, ");
            _frontend_code.Append(_name_of_internal_argument_mapping);
            _frontend_code.Append("[] _m)\n    {\n        this._b = _b;\n        this._m = _m;\n    }\n\n");
            _frontend_code.Append("    private ");
            _frontend_code.Append(_name_of_internal_interface_backend);
            _frontend_code.Append(" _b;\n");
            _frontend_code.Append("    private ");
            _frontend_code.Append(_name_of_internal_argument_mapping);
            _frontend_code.Append("[] _m;\n\n");
            _backend_code.Append("namespace ");
            _backend_code.Append(_namespace_name);
            _backend_code.Append("\n{\n\n");
            _backend_code.Append("[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.i2, \"");
            _backend_code.Append(_owner_ns_id.ToString());
            _backend_code.Append("\", ");
            _backend_code.Append(_owner_ns_in);
            _backend_code.Append(", \"");
            _backend_code.Append(_owner_cc_id.ToString());
            _backend_code.Append("\", ");
            _backend_code.Append(_owner_cc_in);
            _backend_code.Append(")]\n");
            _backend_code.Append("public sealed class _i2_");
            _backend_code.Append(_interface_name);
            _backend_code.Append(_generic_signature);
            _backend_code.Append("\n    : ");
            _backend_code.Append(_name_of_internal_interface_backend);
            _backend_code.Append("\n");
            _backend_code.Append(_generic_constraint_block);
            _backend_code.Append("{\n");
            _backend_code.Append("    void ");
            _backend_code.Append(_name_of_internal_interface_backend);
            _backend_code.Append(".");
            _backend_code.Append(_name_of_internal_interface_backend_initialize_method);
            _backend_code.Append("(object _o)\n    {\n        this._o = (");
            _backend_code.Append(_type_name_of(_interface_type));
            _backend_code.Append(") _o;\n    }\n\n    private ");
            _backend_code.Append(_type_name_of(_interface_type));
            _backend_code.Append(" _o;\n\n    void ");
            _backend_code.Append(_name_of_internal_interface_backend);
            _backend_code.Append(".");
            _backend_code.Append(_name_of_internal_interface_backend_dispatch_method);
            _backend_code.Append("(");
            _backend_code.Append(_name_of_internal_argument_mapping);
            _backend_code.Append(" _m, object[] _v)\n    {\n        switch (_m.");
            _backend_code.Append(_name_of_internal_argument_mapping_method_index);
            _backend_code.Append(")\n        {\n");
            _interceptor_code.Append("namespace ");
            _interceptor_code.Append(_namespace_name);
            _interceptor_code.Append("\n{\n\n");
            _interceptor_code.Append("[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.i3, \"");
            _interceptor_code.Append(_owner_ns_id.ToString());
            _interceptor_code.Append("\", ");
            _interceptor_code.Append(_owner_ns_in);
            _interceptor_code.Append(", \"");
            _interceptor_code.Append(_owner_cc_id.ToString());
            _interceptor_code.Append("\", ");
            _interceptor_code.Append(_owner_cc_in);
            _interceptor_code.Append(")]\n");
            _interceptor_code.Append("public sealed class _i3_");
            _interceptor_code.Append(_interface_name);
            _interceptor_code.Append(_generic_signature);
            _interceptor_code.Append("\n    : ");
            _interceptor_code.Append(_type_name_of(_interface_type));
            _interceptor_code.Append(", ");
            _interceptor_code.Append(_name_of_internal_interface_interceptor);
            _interceptor_code.Append("\n");
            _interceptor_code.Append(_generic_constraint_block);
            _interceptor_code.Append("{\n");
            _interceptor_code.Append("    void ");
            _interceptor_code.Append(_name_of_internal_interface_interceptor);
            _interceptor_code.Append(".");
            _interceptor_code.Append(_name_of_internal_interface_interceptor_initialize_method);
            _interceptor_code.Append("(object _o, QS.Fx.Object.IInternal _i, QS.Fx.Base.SynchronizationOption[] _s)\n    {\n        this._o = (");
            _interceptor_code.Append(_type_name_of(_interface_type));
            _interceptor_code.Append(") _o;\n        this._i = _i;\n        this._s = _s;\n    }\n\n    private ");
            _interceptor_code.Append(_type_name_of(_interface_type));
            _interceptor_code.Append(" _o;\n    private QS.Fx.Object.IInternal _i;\n    private QS.Fx.Base.SynchronizationOption[] _s;\n");
            bool _is_first_method = true;
            int _method_index = 0;
            foreach (System.Reflection.MethodInfo _method_info in _interface_type.GetMethods())
            {                
                QS._qss_x_.Reflection_.Internal_._internal_info_operation _operation_info =
                    new QS._qss_x_.Reflection_.Internal_._internal_info_operation();
                _operation_info._operation_name =
                    ((QS.Fx.Reflection.OperationAttribute)_method_info.GetCustomAttributes(typeof(QS.Fx.Reflection.OperationAttribute), true)[0]).ID;
                object[] _method_serializable_attributes = _method_info.GetCustomAttributes(typeof(QS.Fx.Serialization.SerializableAttribute), true);
                bool _is_method_serializable = ((_method_serializable_attributes != null) && (_method_serializable_attributes.Length > 0));
                if (_is_first_method)
                    _is_first_method = false;
                else
                {
                    _frontend_code.Append("\n");
                    _interceptor_code.Append("\n");
                }
                _frontend_code.Append("    ");
                _frontend_code.Append(_type_name_of(_method_info.ReturnType));
                _frontend_code.Append(" ");
                _frontend_code.Append(_type_name_of(_interface_type));
                _frontend_code.Append(".");
                _frontend_code.Append(_method_info.Name);
                _frontend_code.Append("(");
                _backend_code.Append("            case ");
                _backend_code.Append(_method_index.ToString());
                _backend_code.Append(" :\n                {\n");
                bool _is_first_parameter = true;
                int _parameter_index = 0;
                int _p1_index = 0;
                int _p2_index = 0;
                StringBuilder _parameters_sb_0 = new StringBuilder();
                StringBuilder _parameters_sb_1 = new StringBuilder();
                StringBuilder _parameters_sb_2 = new StringBuilder();
                StringBuilder _parameters_sb_3 = new StringBuilder();
                StringBuilder _parameters_sc_0 = new StringBuilder();
                StringBuilder _parameters_sc_1 = new StringBuilder();
                StringBuilder _parameters_sc_2 = new StringBuilder();
                StringBuilder _parameters_sc_3 = new StringBuilder();
                StringBuilder _parameters_args = new StringBuilder();
                StringBuilder _parameters_event = new StringBuilder();
                StringBuilder _parameters_e_0 = new StringBuilder();
                StringBuilder _parameters_e_1 = new StringBuilder();
                StringBuilder _parameters_e_2 = new StringBuilder();
                StringBuilder _parameters_e_3 = new StringBuilder();
                StringBuilder _parameters_e_4 = new StringBuilder();
                Queue<System.Reflection.ParameterInfo> _parameters = 
                    new Queue<System.Reflection.ParameterInfo>(_method_info.GetParameters());
                bool _isretval = !_method_info.ReturnType.Equals(typeof(void));
                bool _issynchronous = _isretval;
                _parameters_sc_2.Append("                    ");
                bool _wrote_call = false;
                string _interceptor_event = "_" + _name_of_internal_interface_interceptor_arguments_event + "_" + _method_index.ToString();
                List<_internal_info_parameter> _in_parameter_internal_infos = null;
                List<_internal_info_parameter> _out_parameter_internal_infos = null;
                _operation_info._in_parameter_lookup = null;
                _operation_info._out_parameter_lookup = null;
                _operation_info._retval_lookup = -1;
                while (true)
                {
                    System.Reflection.ParameterInfo _parameter_info;
                    System.Type _parameter_type;
                    bool _is1, _is2;
                    if (_isretval)
                    {
                        _parameter_info = _method_info.ReturnParameter;
                        _parameter_type = _parameter_info.ParameterType;
                        _is1 = false;
                        _is2 = true;
                    }
                    else
                    {
                        if (!_wrote_call)
                        {
                            _parameters_sc_2.Append("this._o.");
                            _parameters_sc_2.Append(_method_info.Name);
                            _parameters_sc_2.Append("(");
                            _parameters_e_0.Append("this._o.");
                            _parameters_e_0.Append(_method_info.Name);
                            _parameters_e_0.Append("(");
                            _parameters_e_4.Append("this._o.");
                            _parameters_e_4.Append(_method_info.Name);
                            _parameters_e_4.Append("(");
                            _wrote_call = true;
                        }
                        do
                        {
                            if (_parameters.Count > 0)
                                _parameter_info = _parameters.Dequeue();
                            else
                                _parameter_info = null;
                        }
                        while ((_parameter_info != null) && _parameter_info.IsRetval);
                        if (_parameter_info == null)
                            break;
                        _parameter_type = _parameter_info.ParameterType;
                        if (_parameter_type.IsByRef)
                        {
                            _issynchronous = true;
                            _is2 = true;
                            _is1 = !_parameter_info.IsOut;
                            _parameter_type = _parameter_type.GetElementType();
                        }
                        else
                        {
                            _is1 = true;
                            _is2 = false;
                        }
                    }
                    string _parameter_type_name = _type_name_of(_parameter_type);
                    if (_is_method_serializable)
                        _parameters_event.Append("        [QS.Fx.Printing.Printable]\n");
                    _parameters_event.Append("        internal ");
                    _parameters_event.Append(_parameter_type_name);
                    _parameters_event.Append(" _m_");
                    _parameters_event.Append(_parameter_index.ToString());
                    _parameters_event.Append(";\n");
                    if (_isretval)
                    {
                        _parameters_sb_0.Append("        ");
                        _parameters_sb_0.Append(_parameter_type_name);
                        _parameters_sb_0.Append(" _p_");
                        _parameters_sb_0.Append(_parameter_index.ToString());
                        _parameters_sb_0.Append(";\n");
                        _parameters_sb_3.Append("        return _p_");
                        _parameters_sb_3.Append(_parameter_index.ToString());
                        _parameters_sb_3.Append(";\n");
                        _parameters_sc_2.Append("_p_");
                        _parameters_sc_2.Append(_parameter_index.ToString());
                        _parameters_sc_2.Append(" = ");
                        _parameters_e_0.Append("this._m_");
                        _parameters_e_0.Append(_parameter_index.ToString());
                        _parameters_e_0.Append(" = ");
                        _parameters_e_3.Append("            return _e._m_");
                        _parameters_e_3.Append(_parameter_index.ToString());
                        _parameters_e_3.Append(";\n");
                        _parameters_e_4.Append("return ");
                    }
                    else
                    {
                        if (_is_first_parameter)
                            _is_first_parameter = false;
                        else
                        {
                            _parameters_args.Append(", ");
                            _parameters_sc_2.Append(", ");
                            _parameters_e_0.Append(", ");
                            _parameters_e_4.Append(", ");
                        }
                        _parameters_args.Append("\n        ");
                        if (_is2)
                        {
                            _parameters_args.Append(_is1 ? "ref " : "out ");
                            _parameters_sc_2.Append(_is1 ? "ref " : "out ");
                            _parameters_e_0.Append(_is1 ? "ref " : "out ");
                            _parameters_e_4.Append(_is1 ? "ref " : "out ");
                        }
                        _parameters_args.Append(_parameter_type_name);
                        _parameters_args.Append(" _p_");
                        _parameters_args.Append(_parameter_index.ToString());
                        _parameters_sc_2.Append("_p_");
                        _parameters_sc_2.Append(_parameter_index.ToString());
                        _parameters_e_0.Append("this._m_");
                        _parameters_e_0.Append(_parameter_index.ToString());
                        _parameters_e_4.Append("_p_");
                        _parameters_e_4.Append(_parameter_index.ToString());
                    }
                    _parameters_sc_0.Append("                    ");
                    _parameters_sc_0.Append(_parameter_type_name);
                    _parameters_sc_0.Append(" _p_");
                    _parameters_sc_0.Append(_parameter_index.ToString());
                    _parameters_sc_0.Append(";\n");
                    _internal_info_parameter _parameter_internal_info = new _internal_info_parameter();
                    _parameter_internal_info._retval = _isretval;
                    _parameter_internal_info._in = _is1;
                    _parameter_internal_info._out = _is2;
                    if (_isretval)
                        _parameter_internal_info._name = null;
                    else
                        _parameter_internal_info._name = _parameter_info.Name;
                    _parameter_internal_info._type = _parameter_type;
                    if (_is1)
                    {
                        if (_in_parameter_internal_infos == null)
                            _in_parameter_internal_infos = new List<_internal_info_parameter>();
                        _in_parameter_internal_infos.Add(_parameter_internal_info);
                        if (_operation_info._in_parameter_lookup == null)
                            _operation_info._in_parameter_lookup = new Dictionary<string, int>();
                        _operation_info._in_parameter_lookup.Add(_parameter_info.Name, _p1_index);
                        _parameters_sb_1.Append("        int _n_");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append(" = _m.");
                        _parameters_sb_1.Append(_name_of_internal_argument_mapping_incoming_values);
                        _parameters_sb_1.Append("[");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append("];\n        if (_n_");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append(" >= 0)\n            _v[_n_");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append("] = ((_m.");
                        _parameters_sb_1.Append(_name_of_internal_argument_mapping_incoming_convertors);
                        _parameters_sb_1.Append("[");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append("] != null) ? (_m.");
                        _parameters_sb_1.Append(_name_of_internal_argument_mapping_incoming_convertors);
                        _parameters_sb_1.Append("[");
                        _parameters_sb_1.Append(_p1_index.ToString());
                        _parameters_sb_1.Append("].x((object) _p_");                        
                        _parameters_sb_1.Append(_parameter_index.ToString());
                        _parameters_sb_1.Append(")) : ((object) _p_");                        
                        _parameters_sb_1.Append(_parameter_index.ToString());
                        _parameters_sb_1.Append("));\n");                        
                        _parameters_sc_1.Append("                    ");
                        _parameters_sc_1.Append("_p_");
                        _parameters_sc_1.Append(_parameter_index.ToString());
                        _parameters_sc_1.Append(" = (");
                        _parameters_sc_1.Append(_parameter_type_name);
                        _parameters_sc_1.Append(") _v[");
                        _parameters_sc_1.Append(_p1_index.ToString());
                        _parameters_sc_1.Append("];\n");
                        _parameters_e_1.Append("            _e._m_");
                        _parameters_e_1.Append(_parameter_index.ToString());
                        _parameters_e_1.Append(" = _p_");
                        _parameters_e_1.Append(_parameter_index.ToString());
                        _parameters_e_1.Append(";\n");
                        _p1_index++;
                    }
                    if (_is2)
                    {
                        if (_out_parameter_internal_infos == null)
                            _out_parameter_internal_infos = new List<_internal_info_parameter>();
                        _out_parameter_internal_infos.Add(_parameter_internal_info);
                        if (_isretval)
                            _operation_info._retval_lookup = _p2_index;
                        else
                        {
                            if (_operation_info._out_parameter_lookup == null)
                                _operation_info._out_parameter_lookup = new Dictionary<string, int>();
                            _operation_info._out_parameter_lookup.Add(_parameter_info.Name, _p2_index);
                        }
                        _parameters_sb_2.Append("        _p_");
                        _parameters_sb_2.Append(_parameter_index.ToString());
                        _parameters_sb_2.Append(" = (");
                        _parameters_sb_2.Append(_parameter_type_name);
                        _parameters_sb_2.Append(") _v[_m.");
                        _parameters_sb_2.Append(_name_of_internal_argument_mapping_number_of_values);
                        _parameters_sb_2.Append(" - ");
                        _parameters_sb_2.Append((_p2_index + 1).ToString());
                        _parameters_sb_2.Append("];\n");
                        _parameters_sc_3.Append("                    int _n_");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append(" = _m.");
                        _parameters_sc_3.Append(_name_of_internal_argument_mapping_outgoing_values);
                        _parameters_sc_3.Append("[");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append("];\n                    if (_n_");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append(" >= 0)\n                        _v[_n_");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append("] = ((_m.");
                        _parameters_sc_3.Append(_name_of_internal_argument_mapping_outgoing_convertors);
                        _parameters_sc_3.Append("[");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append("] != null) ? (_m.");
                        _parameters_sc_3.Append(_name_of_internal_argument_mapping_outgoing_convertors);
                        _parameters_sc_3.Append("[");
                        _parameters_sc_3.Append(_p2_index.ToString());
                        _parameters_sc_3.Append("].x((object) _p_");
                        _parameters_sc_3.Append(_parameter_index.ToString());
                        _parameters_sc_3.Append(")) : ((object) _p_");
                        _parameters_sc_3.Append(_parameter_index.ToString());
                        _parameters_sc_3.Append("));\n");                 
                        if (!_isretval)
                        {
                            _parameters_e_2.Append("            _p_");
                            _parameters_e_2.Append(_parameter_index.ToString());
                            _parameters_e_2.Append(" = _e._m_");
                            _parameters_e_2.Append(_parameter_index.ToString());
                            _parameters_e_2.Append(";\n");
                        }
                        _p2_index++;
                    }
                    _parameter_index++;
                    _isretval = false;
                }
                if (_in_parameter_internal_infos != null)
                {
                    _operation_info._num_in_parameters = _in_parameter_internal_infos.Count;
                    _operation_info._in_parameter_infos = _in_parameter_internal_infos.ToArray();
                }
                else
                {
                    _operation_info._num_in_parameters = 0;
                    _operation_info._in_parameter_infos = null;
                }
                if (_out_parameter_internal_infos != null)
                {
                    _operation_info._num_out_parameters = _out_parameter_internal_infos.Count;
                    _operation_info._out_parameter_infos = _out_parameter_internal_infos.ToArray();
                }
                else
                {
                    _operation_info._num_out_parameters = 0;
                    _operation_info._out_parameter_infos = null;                
                }
                _parameters_sc_2.Append(");\n");
                _parameters_e_0.Append(");\n");
                _parameters_e_4.Append(");\n"); 
                _frontend_code.Append(_parameters_args.ToString());
                _frontend_code.Append(")\n    {\n");
                _frontend_code.Append("        ");
                _frontend_code.Append(_name_of_internal_argument_mapping);
                _frontend_code.Append(" _m = this._m[");
                _frontend_code.Append(_method_index.ToString());
                _frontend_code.Append("];\n        object[] _v = new object[_m.");
                _frontend_code.Append(_name_of_internal_argument_mapping_number_of_values);
                _frontend_code.Append("];\n");
                _frontend_code.Append(_parameters_sb_0.ToString());
                _frontend_code.Append(_parameters_sb_1.ToString());
                _frontend_code.Append("        this._b.");
                _frontend_code.Append(_name_of_internal_interface_backend_dispatch_method);
                _frontend_code.Append("(_m, _v);\n");
                _frontend_code.Append(_parameters_sb_2.ToString());
                _frontend_code.Append(_parameters_sb_3.ToString());
                _frontend_code.Append("    }\n");
                _backend_code.Append(_parameters_sc_0.ToString());
                _backend_code.Append(_parameters_sc_1.ToString());
                _backend_code.Append(_parameters_sc_2.ToString());
                _backend_code.Append(_parameters_sc_3.ToString());
                _backend_code.Append("                }\n                break;\n\n");
                bool _intercept_method = true;
#if DO_NOT_INTERCEPT_TWO_WAY_CALLS
                if (_issynchronous)
                    _intercept_method = false;
#endif
                if (_intercept_method)
                {
                    if (_is_method_serializable)
                    {
                        _interceptor_code.Append("\n    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]");
                        _interceptor_code.Append("\n    [System.Serializable]");
                    }
                    _interceptor_code.Append("\n    public sealed class ");
                    _interceptor_code.Append(_interceptor_event);
                    _interceptor_code.Append(" : QS.Fx.Base.IEvent, QS.Fx.Internal.I000010\n    {\n        public ");
                    _interceptor_code.Append(_interceptor_event);
                    _interceptor_code.Append("()\n        {\n        }\n\n");
                    _interceptor_code.Append(_parameters_event.ToString());
                    if (_is_method_serializable)
                        _interceptor_code.Append("\n        [System.NonSerialized]");
                    _interceptor_code.Append("\n        internal QS.Fx.Base.IEvent _n;");
                    if (_is_method_serializable)
                        _interceptor_code.Append("\n        [System.NonSerialized]");
                    _interceptor_code.Append("\n        internal QS.Fx.Base.SynchronizationOption _s;");
                    if (_is_method_serializable)
                        _interceptor_code.Append("\n        [System.NonSerialized]");
                    _interceptor_code.Append("\n        internal _i3_");
                    _interceptor_code.Append(_interface_name);
                    _interceptor_code.Append(_generic_signature);
                    _interceptor_code.Append(" _c;");
                    if (_is_method_serializable)
                        _interceptor_code.Append("\n        [System.NonSerialized]");
                    _interceptor_code.Append("\n        internal ");
                    _interceptor_code.Append(_type_name_of(_interface_type));
                    _interceptor_code.Append(" _o;\n");
                    if (_is_method_serializable)
                        _interceptor_code.Append("\n        [System.NonSerialized]");
                    _interceptor_code.Append("\n        internal QS.Fx.Internal.I000011 _i;\n");
                    if (_issynchronous)
                        _interceptor_code.Append("        internal System.Threading.ManualResetEvent _d = new System.Threading.ManualResetEvent(false);\n");
                    _interceptor_code.Append("\n        void QS.Fx.Internal.I000010.x(object _o, QS.Fx.Internal.I000011 _i)\n        {\n          this._o = (");
                    _interceptor_code.Append(_type_name_of(_interface_type));
                    _interceptor_code.Append(") _o;\n          this._i = _i;\n        }\n\n        void QS.Fx.Base.IEvent.Handle()\n        {\n            ");
                    _interceptor_code.Append(_parameters_e_0.ToString());
                    if (_issynchronous)
                        _interceptor_code.Append("            _d.Set();\n");
                    _interceptor_code.Append("            if (this._i != null)\n");
                    _interceptor_code.Append("              this._i.x();\n");
                    _interceptor_code.Append("        }\n\n        QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next\n");
                    _interceptor_code.Append("        {\n            get { return this._n; }\n            set { this._n = value; }\n        }\n");
                    _interceptor_code.Append("\n        QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption\n");
                    _interceptor_code.Append("        {\n            get { return this._s; }\n        }\n    }\n");
                }
                _interceptor_code.Append("\n    ");
                _interceptor_code.Append(_type_name_of(_method_info.ReturnType));
                _interceptor_code.Append(" ");
                _interceptor_code.Append(_type_name_of(_interface_type));
                _interceptor_code.Append(".");
                _interceptor_code.Append(_method_info.Name);
                _interceptor_code.Append("(");
                _interceptor_code.Append(_parameters_args.ToString());
                _interceptor_code.Append(")\n    {\n        ");
                if (_intercept_method)
                {
                    _interceptor_code.Append("if ((_s[");
                    _interceptor_code.Append(_method_index.ToString());
                    _interceptor_code.Append("] & QS.Fx.Base.SynchronizationOption.Synchronous) != QS.Fx.Base.SynchronizationOption.Synchronous)\n        {\n            ");
                    _interceptor_code.Append(_interceptor_event);
                    _interceptor_code.Append(" _e = new ");
                    _interceptor_code.Append(_interceptor_event);
                    _interceptor_code.Append("();\n            _e._c = this;\n            _e._o = this._o;\n            _e._s = _s[");
                    _interceptor_code.Append(_method_index.ToString());
                    _interceptor_code.Append("];\n");
                    _interceptor_code.Append(_parameters_e_1.ToString());
                    _interceptor_code.Append("            _i.Enqueue(_e);\n");
                    if (_issynchronous)
                        _interceptor_code.Append("            _e._d.WaitOne();\n");
                    _interceptor_code.Append(_parameters_e_2.ToString());
                    _interceptor_code.Append(_parameters_e_3.ToString());
                    _interceptor_code.Append("        }\n        else\n        {\n            ");
                    _interceptor_code.Append(_parameters_e_4.ToString());
                    _interceptor_code.Append("        }\n");
                }
                else
                {
                    _interceptor_code.Append(_parameters_e_4.ToString());
                }
                _interceptor_code.Append("    }\n");
                _method_index++;
                _operation_infos.Add(_operation_info);
            }
            _frontend_code.Append("}\n\n}\n");
            _backend_code.Append("            default:\n                throw new System.NotImplementedException();\n        }\n    }\n}\n\n}\n");
            _interceptor_code.Append("}\n\n}\n");
            _o_internal_info_interfaceclass = new _internal_info_interfaceclass();
            _o_internal_info_interfaceclass._frontend_code = _frontend_code.ToString();
            _o_internal_info_interfaceclass._backend_code = _backend_code.ToString();
            _o_internal_info_interfaceclass._interceptor_code = _interceptor_code.ToString();
            _o_internal_info_interfaceclass._operation_infos = _operation_infos.ToArray();
            _o_internal_info_interfaceclass._num_operations = _o_internal_info_interfaceclass._operation_infos.Length;
            _o_internal_info_interfaceclass._operation_lookup = new Dictionary<string, int>(_o_internal_info_interfaceclass._num_operations);
            for (int k = 0; k < _o_internal_info_interfaceclass._num_operations; k++)
                _o_internal_info_interfaceclass._operation_lookup.Add(_o_internal_info_interfaceclass._operation_infos[k]._operation_name, k);
        }

        #endregion

        #region _generate_object_frontend_and_backend

        public static void _generate_object_frontend_and_backend(
            Type _interface_type, QS.Fx.Base.ID _owner_ns_id, ulong _owner_ns_in, QS.Fx.Base.ID _owner_cc_id, ulong _owner_cc_in,
            string _namespace_name, string _interface_name, 
            out _internal_info_objectclass _o_internal_info_objectclass)
        {
            List<_internal_info_endpoint> _endpoint_infos = new List<_internal_info_endpoint>();
            StringBuilder _frontend_code = new StringBuilder();
            StringBuilder _backend_code = new StringBuilder();
            string _generic_signature, _generic_constraint_block;
            _generate_template_signatures_of(_interface_type, out _generic_signature, out _generic_constraint_block);
            _frontend_code.Append("namespace ");
            _frontend_code.Append(_namespace_name);
            _frontend_code.Append("\n{\n\n");
            _frontend_code.Append("[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.o1, \"");
            _frontend_code.Append(_owner_ns_id.ToString());
            _frontend_code.Append("\", ");
            _frontend_code.Append(_owner_ns_in);
            _frontend_code.Append(", \"");
            _frontend_code.Append(_owner_cc_id.ToString());
            _frontend_code.Append("\", ");
            _frontend_code.Append(_owner_cc_in);
            _frontend_code.Append(")]\n");
            _frontend_code.Append("public sealed class _o1_");
            _frontend_code.Append(_interface_name);
            _frontend_code.Append(_generic_signature);
            _frontend_code.Append("\n    : QS.Fx.Inspection.Inspectable, ");
            string _interface_type_name = _type_name_of(_interface_type);
            _frontend_code.Append(_interface_type_name);
            _frontend_code.Append(", ");
            _frontend_code.Append(_name_of_internal_object_frontend);
            _frontend_code.Append("\n");
            _frontend_code.Append(_generic_constraint_block);
            _frontend_code.Append("{\n");
            _frontend_code.Append("    void ");
            _frontend_code.Append(_name_of_internal_object_frontend);
            _frontend_code.Append(".");
            _frontend_code.Append(_name_of_internal_object_frontend_initialize_method);
            _frontend_code.Append("(");
            _frontend_code.Append(_name_of_internal_object_backend);
            _frontend_code.Append(" _b, int[] _m, ");
            _frontend_code.Append(_name_of_internal_endpoint_convertor);
            _frontend_code.Append("[] _cc)\n    {\n        this._b = _b;\n        this._m = _m;\n        this._cc = _cc;\n    }\n\n");
            _frontend_code.Append("    [QS.Fx.Base.Inspectable(\"backend\")]\n    private ");
            _frontend_code.Append(_name_of_internal_object_backend);
            _frontend_code.Append(" _b;\n");
            _frontend_code.Append("    private int[] _m;\n    private ");
            _frontend_code.Append(_name_of_internal_endpoint_convertor);
            _frontend_code.Append("[] _cc;\n\n");
            _backend_code.Append("namespace ");
            _backend_code.Append(_namespace_name);
            _backend_code.Append("\n{\n\n");
            _backend_code.Append("[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.o2, \"");
            _backend_code.Append(_owner_ns_id.ToString());
            _backend_code.Append("\", ");
            _backend_code.Append(_owner_ns_in);
            _backend_code.Append(", \"");
            _backend_code.Append(_owner_cc_id.ToString());
            _backend_code.Append("\", ");
            _backend_code.Append(_owner_cc_in);
            _backend_code.Append(")]\n");
            _backend_code.Append("public sealed class _o2_");
            _backend_code.Append(_interface_name);
            _backend_code.Append(_generic_signature);
            _backend_code.Append("\n    : QS.Fx.Inspection.Inspectable, ");
            _backend_code.Append(_name_of_internal_object_backend);
            _backend_code.Append("\n");
            _backend_code.Append(_generic_constraint_block);
            _backend_code.Append("{\n");
            _backend_code.Append("    void ");
            _backend_code.Append(_name_of_internal_object_backend);
            _backend_code.Append(".");
            _backend_code.Append(_name_of_internal_object_backend_initialize_method);
            _backend_code.Append("(object _o)\n    {\n        this._o = (");
            _backend_code.Append(_interface_type_name);
            _backend_code.Append(") _o;\n    }\n\n    [QS.Fx.Base.Inspectable(\"object\")]\n    private ");
            _backend_code.Append(_interface_type_name);
            _backend_code.Append(" _o;\n\n    object ");
            _backend_code.Append(_name_of_internal_object_backend);
            _backend_code.Append(".");
            _backend_code.Append(_name_of_internal_object_backend_dispatch_method);
            _backend_code.Append("(int _i)\n    {\n        switch (_i)\n        {\n");
            bool _is_first_property = true;
            int _property_index = 0;
            foreach (System.Reflection.PropertyInfo _property_info in _interface_type.GetProperties())
            {
                _internal_info_endpoint _endpoint_info = new _internal_info_endpoint();
                _endpoint_info._endpoint_name =
                    ((QS.Fx.Reflection.EndpointAttribute)_property_info.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointAttribute), true)[0]).ID;
                if (_is_first_property)
                    _is_first_property = false;
                else
                    _frontend_code.Append("\n");
                _frontend_code.Append("    ");
                string _property_type_name = _type_name_of(_property_info.PropertyType);
                _frontend_code.Append(_property_type_name);
                _frontend_code.Append(" ");
                _frontend_code.Append(_interface_type_name);
                _frontend_code.Append(".");
                _frontend_code.Append(_property_info.Name);
                _frontend_code.Append("\n    {\n        get { return (");
                _frontend_code.Append(_property_type_name);
                _frontend_code.Append(") _cc[");
                _frontend_code.Append(_property_index.ToString());                
                _frontend_code.Append("].");
                _frontend_code.Append(_name_of_internal_convertor_dispatch_method);
                _frontend_code.Append("(_b.");
                _frontend_code.Append(_name_of_internal_object_backend_dispatch_method);
                _frontend_code.Append("(_m[");
                _frontend_code.Append(_property_index.ToString());
                _frontend_code.Append("])); }\n    }\n");
                _backend_code.Append("            case ");
                _backend_code.Append(_property_index.ToString());
                _backend_code.Append(" : return _o.");
                _backend_code.Append(_property_info.Name);
                _backend_code.Append(";\n");
                _property_index++;
                _endpoint_infos.Add(_endpoint_info);
            }
            _frontend_code.Append("}\n");
            _backend_code.Append("            default:\n                throw new System.NotImplementedException();\n        }\n    }\n}\n");
            _frontend_code.Append("\n}\n");
            _backend_code.Append("\n}\n");
            _o_internal_info_objectclass = new _internal_info_objectclass();
            _o_internal_info_objectclass._frontend_code = _frontend_code.ToString();
            _o_internal_info_objectclass._backend_code = _backend_code.ToString();
            _o_internal_info_objectclass._endpoint_infos = _endpoint_infos.ToArray();
            _o_internal_info_objectclass._num_endpoints = _o_internal_info_objectclass._endpoint_infos.Length;
            _o_internal_info_objectclass._endpoint_lookup = new Dictionary<string, int>(_o_internal_info_objectclass._num_endpoints);
            for (int k = 0; k < _o_internal_info_objectclass._num_endpoints; k++)
                _o_internal_info_objectclass._endpoint_lookup.Add(_o_internal_info_objectclass._endpoint_infos[k]._endpoint_name, k);
        }

        #endregion

        #region _get_object_convertor

        private static IDictionary<TypePair_, _internal_convertor> _object_convertors =
            new Dictionary<TypePair_, _internal_convertor>();

        internal static _internal_convertor _get_object_convertor(Type _from, Type _to)
        {
            TypePair_ _k = new TypePair_(_from, _to);
            _internal_convertor _convertor;
            lock (_object_convertors)
            {
                if (!_object_convertors.TryGetValue(_k, out _convertor))
                {
                    _convertor = new _internal_convertor_for_objects(_from, _to);
                    _object_convertors.Add(_k, _convertor);
                }
            }
            return _convertor;
        }

        #endregion

        #region _get_endpoint_convertor

        private static IDictionary<TypePair_, _internal_convertor> _endpoint_convertors =
            new Dictionary<TypePair_, _internal_convertor>();

        internal static _internal_convertor _get_endpoint_convertor(Type _from, Type _to)
        {
            TypePair_ _k = new TypePair_(_from, _to);
            _internal_convertor _convertor;
            lock (_endpoint_convertors)
            {
                if (!_endpoint_convertors.TryGetValue(_k, out _convertor))
                {
                    _convertor = new _internal_convertor_for_endpoints(_from, _to);
                    _endpoint_convertors.Add(_k, _convertor);
                }
            }
            return _convertor;
        }

        #endregion

        #region _get_interface_convertor

        private static IDictionary<TypePair_, _internal_convertor> _interface_convertors =
            new Dictionary<TypePair_, _internal_convertor>();

        internal static _internal_convertor _get_interface_convertor(Type _from, Type _to)
        {
            TypePair_ _k = new TypePair_(_from, _to);
            _internal_convertor _convertor;
            lock (_interface_convertors)
            {
                if (!_interface_convertors.TryGetValue(_k, out _convertor))
                {
                    _convertor = new _internal_convertor_for_interfaces(_from, _to);
                    _interface_convertors.Add(_k, _convertor);
                }
            }
            return _convertor;
        }

        #endregion

        #region _get_value_convertor

        private static IDictionary<TypePair_, _internal_convertor> _value_convertors =
            new Dictionary<TypePair_, _internal_convertor>();

        internal static _internal_convertor _get_value_convertor(Type _from, Type _to)
        {
            if (_to.IsAssignableFrom(_from))
                return null;
            else
            {
                TypePair_ _k = new TypePair_(_from, _to);
                _internal_convertor _convertor;
                lock (_value_convertors)
                {
                    if (!_value_convertors.TryGetValue(_k, out _convertor))
                    {
                        _convertor = new _internal_convertor_for_values(_from, _to);
                        _value_convertors.Add(_k, _convertor);
                    }
                }
                return _convertor;
            }
        }

        #endregion

        #region _generate_valueclass_serialization_code

        public static void _generate_valueclass_serialization_code(
            Type _valueclass_type, 
            QS.Fx.Base.ID _owner_ns_id, ulong _owner_ns_in, QS.Fx.Base.ID _owner_cc_id, ulong _owner_cc_in, 
            string _namespace_name, string _valueclass_name, 
            out QS._qss_x_.Reflection_.Internal_._internal_info_valueclass _o_internal_info_valueclass)
        {
            _o_internal_info_valueclass = new _internal_info_valueclass();
            StringBuilder _serialization_code = new StringBuilder();
            string _generic_signature, _generic_constraint_block;
            _generate_template_signatures_of(_valueclass_type, out _generic_signature, out _generic_constraint_block);
            _serialization_code.Append("namespace ");
            _serialization_code.Append(_namespace_name);
            _serialization_code.Append("\n{\n\n[QS.Fx.Internal.I000004(QS.Fx.Internal.I000005.v1, \"");
            _serialization_code.Append(_owner_ns_id.ToString());
            _serialization_code.Append("\", ");
            _serialization_code.Append(_owner_ns_in);
            _serialization_code.Append(", \"");
            _serialization_code.Append(_owner_cc_id.ToString());
            _serialization_code.Append("\", ");
            _serialization_code.Append(_owner_cc_in);
            _serialization_code.Append(")]\npublic sealed class _v1_");
            _serialization_code.Append(_valueclass_name);
            _serialization_code.Append(_generic_signature);
            _serialization_code.Append("\n    : QS.Fx.Serialization.ISerializer<");
            string _valueclass_type_name = _type_name_of(_valueclass_type);
            _serialization_code.Append(_valueclass_type_name);
            _serialization_code.Append(">\n");
            _serialization_code.Append(_generic_constraint_block);
            _serialization_code.Append("{\n");
            _serialization_code.Append("    public static readonly _v1_");
            _serialization_code.Append(_valueclass_name);
            _serialization_code.Append(_generic_signature);
            _serialization_code.Append(" _serializer_ = new _v1_");
            _serialization_code.Append(_valueclass_name);
            _serialization_code.Append(_generic_signature);
            _serialization_code.Append("();\n\n    private _v1_");
            _serialization_code.Append(_valueclass_name);
            _serialization_code.Append("()\n    {\n    }\n\n    public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo(");
            _serialization_code.Append(_valueclass_type_name);
            _serialization_code.Append(" _value_)\n    {\n        QS.Fx.Serialization.SerializableInfo _info_ = new QS.Fx.Serialization.SerializableInfo(0);\n");

            // TODO: Add some serialization logic here.......

            _serialization_code.Append("        return _info_;\n    }\n\n    public unsafe void SerializeTo(\n        ");
            _serialization_code.Append(_valueclass_type_name);
            _serialization_code.Append(" _value_,\n        ref QS.Fx.Base.ConsumableBlock _header_,\n        ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)\n    {\n");

            // TODO: Add some serialization logic here.......

            _serialization_code.Append("    }\n\n    public unsafe void DeserializeFrom(\n        ref QS.Fx.Base.ConsumableBlock _header_,\n        ref QS.Fx.Base.ConsumableBlock data,\n        out ");
            _serialization_code.Append(_valueclass_type_name);
            _serialization_code.Append(" _value_)\n    {\n");

            // TODO: Add some serialization logic here.......
            _serialization_code.Append("        _value_ = default(");
            _serialization_code.Append(_valueclass_type_name);
            _serialization_code.Append(");\n");

            _serialization_code.Append("    }\n}\n\n}\n");
            _o_internal_info_valueclass._serialization_code = _serialization_code.ToString();

            // (new QS._qss_x_.Base1_.ExceptionForm(_valueclass_type_name, _o_internal_info_valueclass._serialization_code, null)).ShowDialog();
        }

        #endregion

        #region _get_interceptor

        private static IDictionary<Type, _internal_interceptor> _interceptors =
            new Dictionary<Type, _internal_interceptor>();

        private static _internal_interceptor _get_interceptor(Type _c)
        {
            _internal_interceptor _interceptor;
            lock (_interceptors)
            {
                if (!_interceptors.TryGetValue(_c, out _interceptor))
                {
                    _interceptor = new _internal_interceptor(_c);
                    _interceptors.Add(_c, _interceptor);
                }
            }
            return _interceptor;
        }

        #endregion

        #region _can_convert_value

        internal static bool _can_convert_value(Type _from, Type _to)
        {
            _internal_convertor_for_values _convertor = (_internal_convertor_for_values) _get_value_convertor(_from, _to);
            return _convertor._Mode != _internal_convertor_for_values.Mode_._Impossible;
        }
        
        #endregion
    }

    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    #region some stuff for testing

    [QS.Fx.Reflection.ValueClass("7D632E120C944FB292B61AB9D1A3C90A")]
    public sealed class Foo4
    {
        public Foo4()
        {
        }

        [QS.Fx.Serialization.Serializable]
        public bool x;

        [QS.Fx.Serialization.Serializable]
        public int Y
        {
            get { return this.y; }
            set { this.y = value; }
        }
        private int y;

    }

    // $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    [QS.Fx.Reflection.InterfaceClass("90E1B0E57AA741719D958642DCE67122`1")]
    public interface IFoo1 
        : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("foo")]
        double foo(double x, int[] y, string m, out string mm, out int[] w);
    }

    [QS.Fx.Reflection.InterfaceClass("C50F0744AD474A06832587B45F6B279C`1")]
    public interface IFoo2<[QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>  
        : QS.Fx.Interface.Classes.IInterface 
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        [QS.Fx.Reflection.Operation("foo")]
        void foo(MessageClass m);
    }

    [QS.Fx.Reflection.InterfaceClass("7BFE5E94BFB54FABBC92882DDCC0ED56`1")]
    public interface IFoo3<[QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] ObjectClass>
        : QS.Fx.Interface.Classes.IInterface 
        where ObjectClass : QS.Fx.Reflection.IObjectClass
    {
        [QS.Fx.Reflection.Operation("foo")]
        ObjectClass foo(string id);
    }

    #endregion
}
